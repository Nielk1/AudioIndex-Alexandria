using Alexandria.Models;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alexandria.Components
{
    public class ConfigLibrarySubitemViewComponent : ViewComponent
    {
        // switch to a library config service at some point

        //private readonly Library _library;
        [FromServices]
        public MusicContext MusicContext { get; set; }

        //public ConfigLibraryViewComponent(Library library)
        public ConfigLibrarySubitemViewComponent(MusicContext _MusicContext)
        {
            //_library = library;
            MusicContext = _MusicContext;
        }

        public IViewComponentResult Invoke(int ID)
        {
            var library = MusicContext.Librarys.Where(dr => dr.ID == ID);

            return View(library);
        }

        //public async Task<IViewComponentResult> InvokeAsync(int ID)
        //{
        //    var library = MusicContext.Librarys.Where(dr => dr.ID == ID);

        //    return View(library);
        //}
    }
}
