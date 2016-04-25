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
    public class ConfigController : Controller
    {
        [FromServices]
        public MusicContext MusicContext { get; set; }

        [FromServices]
        public ILogger<ConfigController> Logger { get; set; }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var db = MusicContext;
            {
                db.Librarys.Where(library => library.Expire.HasValue && library.Expire.Value < DateTime.UtcNow).ToList().ForEach(library =>
                {
                    db.Librarys.Remove(library);
                });
                db.SaveChanges();
            }

            var libraries = MusicContext.Librarys.OrderBy(dr => dr.Name).ThenBy(dr => dr.Path);
                /*.Include(lib => lib.LibraryId); // appears to be for object children instead
                .ThenInclude(lib => lib.Name)
                .ThenInclude(lib => lib.Path)
                .ThenInclude(lib => lib.Enabled)
                .ThenInclude(lib => lib.Sort);*/
            return View(libraries);
        }

        [HttpPost]
        public IActionResult Library_SoftDelete(int id)
        {

            var db = MusicContext;
            {
                db.Librarys.Where(library => library.ID == id).ToList().ForEach(library =>
                {
                    library.Expire = DateTime.UtcNow.AddHours(1);
                    db.Librarys.Attach(library, GraphBehavior.SingleObject);
                    db.Entry(library).Property(x => x.Expire).IsModified = true;

                });
                db.SaveChanges();
            }
            return ViewComponent("ConfigLibrarySubitem", id);
        }

        [HttpPost]
        public IActionResult Library_UndoSoftDelete(int id)
        {

            var db = MusicContext;
            {
                db.Librarys.Where(library => library.ID == id).ToList().ForEach(library =>
                {
                    library.Expire = null;
                    db.Librarys.Attach(library, GraphBehavior.SingleObject);
                    db.Entry(library).Property(x => x.Expire).IsModified = true;

                });
                db.SaveChanges();
            }
            return ViewComponent("ConfigLibrarySubitem", id);
        }

        [HttpPost]
        public bool? Library_Enable(int id, bool toggle)
        {
            var db = MusicContext;
            {
                db.Librarys.Where(library => library.ID == id).ToList().ForEach(library =>
                {
                    library.Enabled = toggle;
                    db.Librarys.Attach(library, GraphBehavior.SingleObject);
                    db.Entry(library).Property(x => x.Enabled).IsModified = true;

                });
                db.SaveChanges();
                Library tmpLib = db.Librarys.Where(library => library.ID == id).ToList().FirstOrDefault();
                if (tmpLib != null)
                {
                    return tmpLib.Enabled;
                }
            }
            return null;
        }

        [HttpPost]
        public IActionResult Library_Edit(int id, string name, string path)
        {

            var db = MusicContext;
            {
                db.Librarys.Where(library => library.ID == id).ToList().ForEach(library =>
                {
                    library.Name = name;
                    library.Path = path;
                    db.Librarys.Attach(library, GraphBehavior.SingleObject);
                    db.Entry(library).Property(x => x.Name).IsModified = true;
                    db.Entry(library).Property(x => x.Path).IsModified = true;

                });
                db.SaveChanges();
            }
            return ViewComponent("ConfigLibrarySubitem", id);
        }

        [HttpPost]
        public IActionResult Library_AddNew(string name, string path)
        {
            Library newLib = new Library() { Name = name, Path = path, Enabled = false };
            var db = MusicContext;
            {
                db.Librarys.Add(newLib);
                db.SaveChanges();
            }
            return ViewComponent("ConfigLibrarySubitem", newLib.ID);
        }
    }
}
