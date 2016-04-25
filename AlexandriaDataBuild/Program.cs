using Alexandria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNet.Hosting.Internal;
using System.IO;

namespace AlexandriaDataBuild
{
    public class Program
    {
        public Program()
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            DbContextOptionsBuilder dbbuilder = new DbContextOptionsBuilder();
            dbbuilder.UseSqlServer(Configuration["Data:ConnectionString"]);
            MusicContext = new MusicContext(dbbuilder.Options);

            metaCat = MusicContext.TagCategorys.Where(dr => dr.Name == "Meta").First();
            unTaggedTag = MusicContext.Tags.Where(dr => dr.Category.Equals(metaCat) && dr.Name == "New").First();
            systemAccount = MusicContext.UserAccounts.Where(dr => dr.System == true && dr.Email == "system@local").First();
        }

        public IConfigurationRoot Configuration { get; set; }
        public MusicContext MusicContext { get; set; }
        private List<string> ExtensionsToMonitor = new List<string>() { ".mp3", ".wav", ".flac" };

        private TagCategory metaCat;
        private Tag unTaggedTag;
        private UserAccount systemAccount;

        private void ProcessLibraries()
        {
            MusicContext.Librarys.Where(dr => dr.Enabled && !dr.Expire.HasValue).ToList().ForEach(dr =>
            {
                Console.WriteLine($"{dr.Name}\t{dr.Path}");
                recursivePathDive(dr, dr.Path);
            });
        }

        void recursivePathDive(Library lib, string dirPath)
        {
            string[] Directories = Directory.GetDirectories(dirPath, @"*", SearchOption.TopDirectoryOnly);
            string[] Files = Directory.GetFiles(dirPath, @"*", SearchOption.TopDirectoryOnly);

            foreach (string FullPath in Files)
            {
                string SubPath = FullPath.Substring(lib.Path.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                Alexandria.Models.File tmp = MusicContext.Files.Where(dr => dr.Library.Equals(lib) && dr.Path == SubPath).FirstOrDefault();
                if(tmp == null)
                {
                    // do we want to track this file?
                    string FileExtension = ExtensionsToMonitor.Where(dr => Path.GetFileName(FullPath).EndsWith(dr)).FirstOrDefault();
                    if (FileExtension == null)
                    {
                        ConsoleColor tmpConCol = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine($"Skip\t{lib.Path} | {SubPath}");
                        Console.ForegroundColor = tmpConCol;
                        continue;
                    }

                    {
                        ConsoleColor tmpConCol = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"New\t{lib.Path} | {SubPath}");
                        Console.ForegroundColor = tmpConCol;
                    }

                    // gather file data
                    // add to db
                    Alexandria.Models.File newFile = new Alexandria.Models.File()
                    {
                        Library = lib,
                        Archival = false,
                        FileModified = System.IO.File.GetLastWriteTimeUtc(FullPath),
                        RecordAdded = DateTime.UtcNow,
                        Path = SubPath,
                        Tags = new List<TagAssociation>()
                    };
                    //TagAdder tagAdderRecord = new TagAdder() { Timestamp = DateTime.UtcNow, User = systemAccount };
                    //TagAssociation newTagAssoc = new TagAssociation() { Tag = unTaggedTag, Added = new List<TagAdder>() };
                    //newTagAssoc.Added.Add(tagAdderRecord);
                    //newFile.Tags.Add(newTagAssoc);

                    newFile.AddTag(unTaggedTag, systemAccount);

                    MusicContext.Files.Add(newFile);

                    MusicContext.SaveChanges();
                }
                else
                {
                    /*
                    // skip file unless modified date has changed

                    // do we want to track this file?
                    string FileExtension = ExtensionsToMonitor.Where(dr => Path.GetFileName(FullPath).EndsWith(dr)).FirstOrDefault();
                    if (FileExtension == null) continue;

                    DateTime FileModDate = System.IO.File.GetLastWriteTimeUtc(FullPath);
                    if(FileModDate > tmp.Modified)
                    {
                        Console.WriteLine($"Changed\t{lib.Path} | {SubPath}");
                    }
                    */
                    {
                        ConsoleColor tmpConCol = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($"Known\t{lib.Path} | {SubPath}");
                        Console.ForegroundColor = tmpConCol;
                    }
                }
            }

            foreach (string Directory in Directories)
            {
                recursivePathDive(lib, Directory);
            }
        }

        // Entry point for the application.
        public static void Main(string[] args)
        {
            Program prog = new Program();
            prog.ProcessLibraries();
            Console.ReadKey(true);
        }
    }
}
