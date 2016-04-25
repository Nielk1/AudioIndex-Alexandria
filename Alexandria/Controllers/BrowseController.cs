using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Alexandria.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Entity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Alexandria.Controllers
{
    public class BrowseController : Controller
    {
        [FromServices]
        public MusicContext MusicContext { get; set; }

        [FromServices]
        public ILogger<ConfigController> Logger { get; set; }

        public IActionResult Files()
        {
            return View(new FileBrowseModel(MusicContext));
        }
    }

    public class FileBrowseModel
    {
        private MusicContext context;

        public FileBrowseModel(MusicContext context)
        {
            this.context = context;
        }

        public DbSet<File> Files { get; set; }

        public List<File> GetFiles(int page, int size)
        {
            return context.Files
                .Include(dr => dr.Library)
                .Where(dr => dr.Library != null && dr.Library.Enabled)
                .OrderByDescending(dr => dr.RecordAdded)
                .Skip(page * size).Take(size).ToList();
        }
        public List<Tag> GetTags(int page, int size)
        {
            return context.Files
                .Include(dr => dr.Library)
                .Where(dr => dr.Library != null && dr.Library.Enabled)
                .Include(dr => dr.Tags)
                .ThenInclude(dr => dr.Tag)
                .ThenInclude(dr => dr.Category)
                .OrderByDescending(dr => dr.RecordAdded)
                .Skip(page * size)
                .Take(size)
                .ToList()
                .SelectMany(dr => dr.GetTags())
                .ToList();
        }
        public List<Library> GetLibraries()
        {
            return context.Librarys
                .Where(dr => dr.Enabled)
                .OrderBy(dr => dr.Name)
                .ThenBy(dr => dr.Path)
                .ToList();
        }
    }
}
