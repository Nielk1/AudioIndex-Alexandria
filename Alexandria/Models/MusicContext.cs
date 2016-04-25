using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;

namespace Alexandria.Models
{
    public class MusicContext : DbContext
    {
        public MusicContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Library> Librarys { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<VirtualTrack> VirtualTracks { get; set; }
        public DbSet<VirtualAlbum> VirtualAlbums { get; set; }
        public DbSet<TagCategory> TagCategorys { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagAssociation> TagAssociations { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
    }
}
