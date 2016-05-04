using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Alexandria.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Entity;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Alexandria.Controllers
{
    public class API : Controller
    {
        [FromServices]
        public MusicContext MusicContext { get; set; }

        [FromServices]
        public ILogger<ConfigController> Logger { get; set; }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Stream()
        //public async Task<IActionResult> Stream()
        {
            //Console.BackgroundColor = ConsoleColor.White;
            //Console.ForegroundColor = ConsoleColor.Black;
            //Console.WriteLine("TEST");

            //Request.Headers.ToList().ForEach(dr => Logger.LogCritical($"{dr.Key}: {dr.Value}"));

            //https://blogs.msdn.microsoft.com/webdev/2012/11/23/asp-net-web-api-and-http-byte-range-support/

            /*if (Request.Query.ContainsKey("id"))
            {
                int fileID = -1;
                if (!int.TryParse(Request.Query["id"], out fileID))
                {
                    return HttpNotFound();
                }
                Models.File musicFile = MusicContext.Files
                    .Include(dr => dr.Library)
                    .Where(dr => dr.ID == fileID).FirstOrDefault();
                if (musicFile != null)
                {
                    string fullPath = Path.Combine(musicFile.Library.Path, musicFile.Path);
                    if (System.IO.File.Exists(fullPath))
                    {
                        //Response.Headers["X-API-Steam"] = fileID.ToString();

                        HttpContext.Response.ContentType = "audio/mpeg";

                        //HttpContext.Response.Headers["Content-Range"] = "bytes 0-1/*";

                        string bitrate = "320";
                        string args = $"-i \"{fullPath}\" -map 0:0 -b:a {bitrate}k -v 0 -f mp3 -";
                        Process ffmpeg;
                        ffmpeg = new Process();
                        ffmpeg.StartInfo.WorkingDirectory = @"D:\Program Files (loose)\ffmpeg\ffmpeg-20150717-git-8250943-win64-static\bin\";
                        ffmpeg.StartInfo.FileName = "\"D:\\Program Files (loose)\\ffmpeg\\ffmpeg-20150717-git-8250943-win64-static\\bin\\ffmpeg.exe\"";
                        ffmpeg.StartInfo.Arguments = args;
                        ffmpeg.StartInfo.RedirectStandardOutput = true;
                        //ffmpeg.StartInfo.CreateNoWindow = true;
                        ffmpeg.StartInfo.UseShellExecute = false;
                        ffmpeg.Start();
                        StreamReader reader = ffmpeg.StandardOutput;
                        //reader.

                        //Response.StatusCode = 206;
                        Response.Headers["Accept-Ranges"] = "bytes";

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

                        long contentLengthCounter = 0;
                        bool writing = true;
                        ProducerConsumerStream conStream = new ProducerConsumerStream();
                        (new Thread(() => {
                            byte[] buffer = new byte[32768];
                            int read;
                            while ((read = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                conStream.Write(buffer, 0, read);
                                contentLengthCounter += read;
                            }
                            Logger.LogInformation("Compleated Buffering Audio File");
                            Logger.LogInformation($"Length of data is {contentLengthCounter}");
                            writing = false;
                        })).Start();

                        //Response.ContentLength = contentLengthCounter;
                        //Response.Headers["Content-Range"] = $"bytes 0-1/{contentLengthCounter}";

                        {
                            byte[] buffer = new byte[32768];
                            int read;
                            while (((read = conStream.Read(buffer, 0, buffer.Length)) > 0) || writing)
                            {
                                if (read == 0)
                                {
                                    Thread.Sleep(100);
                                    continue;
                                }
                                Response.Body.Write(buffer, 0, read);
                            }
                            Logger.LogInformation("Compleated Sending Buffering Audio File");
                        }

                        return new ContentResult();

                        //return new FileStreamResult(memStream, HttpContext.Response.ContentType);
                        //return new FileStreamResult(reader.BaseStream, HttpContext.Response.ContentType);
                        //return new FileStreamResult(System.IO.File.OpenRead(fullPath), HttpContext.Response.ContentType);
                        //return new PhysicalFileResult(fullPath, HttpContext.Response.ContentType);
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
                else
                {
                    return HttpNotFound();
                }
            }*/

            return HttpBadRequest();
        }
    }
}
