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
            //https://blogs.msdn.microsoft.com/webdev/2012/11/23/asp-net-web-api-and-http-byte-range-support/

            if (Request.Query.ContainsKey("id"))
            {
                int fileID = -1;
                if(!int.TryParse(Request.Query["id"], out fileID))
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
                        HttpContext.Response.ContentType = "audio/mpeg";

                        HttpContext.Response.Headers["Content-Range"] = "bytes 0-1/*";

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
                        reader.BaseStream.CopyTo(HttpContext.Response.Body);
                        //await reader.BaseStream.CopyToAsync(HttpContext.Response.Body);
                        HttpContext.Response.StatusCode = 416;
                        return new ContentResult();
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
            }

            return HttpBadRequest();
        }
    }
}
