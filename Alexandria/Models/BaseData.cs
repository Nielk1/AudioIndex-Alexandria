using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Metadata.Internal;

namespace Alexandria.Models
{
    public class BaseData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            MusicContext context = (MusicContext)serviceProvider.GetService(typeof(MusicContext));

            TagCategory metaCat;
            if (!context.TagCategorys.Where(dr => dr.Name == "Meta").Any())
            {
                metaCat = new TagCategory() { Name = "Meta" };
                context.TagCategorys.Add(metaCat);
                context.SaveChanges();
            } else {
                metaCat = context.TagCategorys.Where(dr => dr.Name == "Meta").First();
            }

            if (!context.Tags.Where(dr => dr.Category.Equals(metaCat) && dr.Name == "New").Any())
            {
                Tag untagged = new Tag() { Name = "New", Category = metaCat };
                context.Tags.Add(untagged);
                context.SaveChanges();
            }

            if (!context.UserAccounts.Where(dr => dr.System == true && dr.Email == "system@local").Any())
            {
                UserAccount systemAccount = new UserAccount() {
                     Email = "system@local",
                     Admin = false,
                     System = true,
                     UserName = "System"
                };
                context.UserAccounts.Add(systemAccount);
                context.SaveChanges();
            }
        }
    }
}