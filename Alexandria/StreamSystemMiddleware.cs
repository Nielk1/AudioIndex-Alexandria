using Alexandria.Models;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Alexandria
{
    public class StreamSystemMiddlewareOptions
    {
        public string ffmpeg { get; set; }
    }

    public class StreamSystemMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger Logger;
        private readonly StreamSystemMiddlewareOptions Options;
        private readonly MusicContext MusicContext;

        private Dictionary<string, PlaybackData> Buffer = new Dictionary<string, PlaybackData>();

        public StreamSystemMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, StreamSystemMiddlewareOptions options, MusicContext MusicContext)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (MusicContext == null)
            {
                throw new ArgumentNullException(nameof(MusicContext));
            }

            this._next = next;
            this.Logger = loggerFactory.CreateLogger<StreamSystemMiddleware>();
            this.Options = options;
            this.MusicContext = MusicContext;
        }

        public async Task Invoke(HttpContext context)
        {
            Logger.LogInformation($"{context.Request.Path}{context.Request.QueryString}\n{context.Request.Headers["Range"]}");

            if (context.Request.Path.HasValue && context.Request.Path.Value.StartsWith("/API/"))
            {
                string[] pathElements = context.Request.Path.Value.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                int i = 0;
                pathElements.ToList().ForEach(dr =>
                {
                    context.Response.Headers[$"X-API-Path-{i}"] = dr;
                });
                context.Request.Query.ToList().ForEach(dr =>
                {
                    context.Response.Headers[$"X-API-QS-{dr.Key}"] = dr.Value;
                });

                if (pathElements.Length > 1 && pathElements[1].Equals("Stream")/* && context.Request.Query.ContainsKey("id")*/)
                {
                    if (context.Request.Query.ContainsKey("id"))
                    {
                        int fileID = -1;
                        if (!int.TryParse(context.Request.Query["id"], out fileID))
                        {
                            context.Response.StatusCode = 404;
                            return;
                        }
                        Models.File musicFile = MusicContext.Files
                            .Include(dr => dr.Library)
                            .Where(dr => dr.ID == fileID).FirstOrDefault();
                        if (musicFile != null)
                        {
                            string fullPath = Path.Combine(musicFile.Library.Path, musicFile.Path);
                            if (System.IO.File.Exists(fullPath))
                            {
                                PlaybackData dat = null;
                                bool newStream = false;
                                lock (Buffer)
                                {
                                    if(Buffer.ContainsKey(fileID.ToString()))
                                    {
                                        lock(dat = Buffer[fileID.ToString()])
                                        {
                                            dat.RefreshAccessTime();
                                        }
                                    }
                                    else
                                    {
                                        Buffer[fileID.ToString()] = dat = new PlaybackData();
                                        newStream = true;
                                    }
                                }

                                if (newStream)
                                {
                                    {// note how long the file is for output aproximation
                                        dat.Seconds = 600;
                                        dat.bitrate = 320;
                                    }


                                    dat.contentLengthCounter = 0;
                                    dat.writing = true;
                                    (new Thread(() =>
                                    {
                                        string argsP = $"-i \"{fullPath}\" -map 0:0 -b:a {dat.bitrate}k -v 0 -f mp3 -";
                                        Process ffmpeg;
                                        ffmpeg = new Process();
                                        ffmpeg.StartInfo.WorkingDirectory = Options.ffmpeg;
                                        ffmpeg.StartInfo.FileName = "\"" + Options.ffmpeg + "ffmpeg.exe\"";
                                        ffmpeg.StartInfo.Arguments = argsP;
                                        ffmpeg.StartInfo.RedirectStandardOutput = true;
                                        //ffmpeg.StartInfo.CreateNoWindow = true;
                                        ffmpeg.StartInfo.UseShellExecute = false;
                                        ffmpeg.Start();
                                        StreamReader reader = ffmpeg.StandardOutput;

                                        byte[] buffer = new byte[32768];
                                        int read;
                                        while ((read = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                                        {
                                            dat.dataStream.Write(buffer, 0, read);
                                            dat.contentLengthCounter += read;
                                        }
                                        Logger.LogInformation("Compleated Buffering Audio File");
                                        Logger.LogInformation($"Length of data is {dat.contentLengthCounter}");
                                        dat.writing = false;
                                    })).Start();
                                }

                                int defaultStripeSize = 1024 * 256;
                                int stillTranscodingStripeSize = 1024 * 16;
                                int stripeSize = defaultStripeSize;
                                string totalSizeString = "*";
                                //string totalSizeString = "999999999999";
                                //string totalSizeString = "";
                                int totalSize = -1;
                                string startIndexString = "0";
                                string endIndexString = null;
                                if (context.Request.Headers.ContainsKey("Range"))
                                {
                                    string range = context.Request.Headers["Range"];
                                    string[] range_eq_split = range.Split(new char[] { '=' });
                                    string[] range_parts = range_eq_split[1].Split(new char[] { '-' });
                                    startIndexString = range_parts[0];
                                    if(range_parts.Length > 0 && range_parts[1].Length > 0)
                                    {
                                        endIndexString = range_parts[1];
                                    }
                                }
                                int startIndex = int.Parse(startIndexString);

                                if(endIndexString != null)
                                {
                                    int requestedStripSize = int.Parse(endIndexString) - startIndex + 1;
                                    stripeSize = Math.Min(stripeSize, requestedStripSize);
                                }

                                int startingIndex = startIndex;

                                // wait until at least enough data is read
                                // this area is a little sketchy due to threading but consider that old values are OK here and we aren't writing
                                while (dat.writing && dat.contentLengthCounter < (startingIndex + stillTranscodingStripeSize))
                                {
                                    Thread.Sleep(100);
                                }

                                lock (dat)
                                {
                                    if (dat.writing)
                                        stripeSize = Math.Min(stripeSize, stillTranscodingStripeSize);

                                    stripeSize = Math.Min(stripeSize, dat.contentLengthCounter - startingIndex);
                                    if (!dat.writing) // we're done writing to the stream so clearly it contains the entire file
                                    {
                                        totalSize = dat.contentLengthCounter;
                                        totalSizeString = totalSize.ToString();
                                    }
                                    //else // failed hack to get browsers to download partial
                                    //{
                                    //    totalSizeString = (startIndex + stripeSize * 2).ToString();
                                    //}
                                    else if(dat.Seconds > 0)
                                    {
                                        totalSizeString = ((int)((dat.Seconds * (dat.bitrate * 1000)) / 8)).ToString();
                                    }
                                }

                                //check stripeSize isn't negative (can only happen if fully transcoded else the system would avoid it)
                                //check startIndexString isn't greater than totalSize
                                //check startingIndex + stripeSize isn't greater than totalSize
                                if (totalSize > 0 && startIndex >= totalSize)
                                {
                                    context.Response.StatusCode = 416;
                                    context.Response.ContentType = "audio/mpeg";
                                    context.Response.Headers["Accept-Ranges"] = "bytes";
                                    context.Response.Headers["Content-Range"] = $"bytes {startIndex}-*/{totalSizeString}";
                                    //context.Response.Headers["Content-Length"] = stripeSize.ToString();
                                }
                                else
                                {
                                    //if (totalSize >= 0 && (startIndex + stripeSize) >= totalSize)
                                    //{
                                    //    context.Response.StatusCode = 206;
                                    //}
                                    //else
                                    //{
                                    //    context.Response.StatusCode = 206;
                                    //}

                                    context.Response.StatusCode = 206;

                                    context.Response.ContentType = "audio/mpeg";
                                    context.Response.Headers["Accept-Ranges"] = "bytes";
                                    context.Response.Headers["Content-Range"] = $"bytes {startIndex}-{startIndex + stripeSize - 1}/{totalSizeString}";
                                    context.Response.Headers["Content-Length"] = stripeSize.ToString();

                                    //context.Response.Headers["Transfer-Encoding"] = @"chunked";
                                    //context.Response.Headers["Cache-control"] = "";
                                    //context.Response.Headers["Pragma"] = "";

                                    //X-Content-Duration: 63.23 
                                    lock (dat)
                                    {
                                        byte[] outputBuffer = new byte[stripeSize];
                                        dat.dataStream.readPosition = startIndex;
                                        dat.dataStream.Read(outputBuffer, 0, stripeSize);
                                        dat.RefreshAccessTime();
                                        context.Response.Body.Write(outputBuffer, 0, stripeSize);
                                    }
                                }


                                //MemoryStream memStream = new MemoryStream();
                                //await reader.BaseStream.CopyToAsync(memStream);
                                //memStream.Position = 0;
                                //Response.ContentLength = memStream.Length;
                                //await memStream.CopyToAsync(Response.Body);

                                //await reader.BaseStream.CopyToAsync(Response.Body);

                                //(new Thread(() => {
                                //    memStream
                                //})).Start();

                                //memStream.Position = 0;

                                //ffmpeg.WaitForExit();
                                //ffmpeg.Kill(); //ffmpeg.close();

                                //Response.Headers["Content-Range"] = "bytes 0-1/*";



                                //Response.ContentLength = contentLengthCounter;
                                //Response.Headers["Content-Range"] = $"bytes 0-1/{contentLengthCounter}";

                                //{
                                //    byte[] buffer = new byte[32768];
                                //    int read;
                                //    while (((read = dat.dataStream.Read(buffer, 0, buffer.Length)) > 0) || dat.writing)
                                //    {
                                //        if (read == 0)
                                //        {
                                //            Thread.Sleep(100);
                                //            continue;
                                //        }
                                //        context.Response.Body.Write(buffer, 0, read);
                                //        lock(dat)
                                //        {
                                //            dat.RefreshAccessTime();
                                //        }
                                //    }
                                //    Logger.LogInformation("Compleated Sending Buffering Audio File");
                                //}

                                //return new FileStreamResult(memStream, HttpContext.Response.ContentType);
                                //return new FileStreamResult(reader.BaseStream, HttpContext.Response.ContentType);
                                //return new FileStreamResult(System.IO.File.OpenRead(fullPath), HttpContext.Response.ContentType);
                                //return new PhysicalFileResult(fullPath, HttpContext.Response.ContentType);
                            }
                            else
                            {
                                context.Response.StatusCode = 404;
                                return;
                            }
                        }
                        else
                        {
                            context.Response.StatusCode = 404;
                            return;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        return;
                    }
                }
                else
                {
                    await _next.Invoke(context);
                }
                //if (context.Response.Headers.Keys.Contains("X-API-Steam"))
                //{
                //    context.Response.Headers["X-API-SteamID"] = context.Response.Headers["X-API-Steam"];
                //    context.Response.Headers.Remove("X-API-Steam");
                //    return;
                //}

                //return;
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }

    class PlaybackData
    {
        public ProducerConsumerStream dataStream { get; private set; }
        public DateTime lastReferenced { get; private set; }

        public int contentLengthCounter = 0;
        public bool writing = true;

        public float Seconds = 0;
        public int bitrate;

        public PlaybackData()
        {
            dataStream = new ProducerConsumerStream();
            lastReferenced = DateTime.UtcNow;
        }

        public void RefreshAccessTime()
        {
            lastReferenced = DateTime.UtcNow;
        }
    }

    class ProducerConsumerStream : Stream
    {
        private readonly MemoryStream innerStream;
        public long readPosition;
        public long writePosition;

        public ProducerConsumerStream()
        {
            innerStream = new MemoryStream();
        }

        public override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return true; } }

        public override void Flush()
        {
            lock (innerStream)
            {
                innerStream.Flush();
            }
        }

        public override long Length
        {
            get
            {
                lock (innerStream)
                {
                    return innerStream.Length;
                }
            }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (innerStream)
            {
                innerStream.Position = readPosition;
                int red = innerStream.Read(buffer, offset, count);
                readPosition = innerStream.Position;

                return red;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (innerStream)
            {
                innerStream.Position = writePosition;
                innerStream.Write(buffer, offset, count);
                writePosition = innerStream.Position;
            }
        }
    }
}
