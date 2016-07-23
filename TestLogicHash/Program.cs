using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestLogicHash
{
    class Program
    {
        static List<string> MusicLibraries = new List<string>()
        {
            //ConfigurationManager.AppSettings["LocationAudio"],
            //ConfigurationManager.AppSettings["LocationAltAudio"]
        };

        static void Main(string[] args)
        {
            foreach (string keyString in ConfigurationManager.AppSettings.Keys)
            {
                if (keyString.StartsWith("Location"))
                {
                    MusicLibraries.Add(ConfigurationManager.AppSettings[keyString]);
                }
            }

            foreach (string basePath in MusicLibraries)
            {
                recursivePathDive(basePath);
            }
        }

        static void recursivePathDive(string Path)
        {
            string[] Directories = Directory.GetDirectories(Path, @"*", SearchOption.TopDirectoryOnly);
            string[] Files = Directory.GetFiles(Path, @"*", SearchOption.TopDirectoryOnly);

            foreach (string File in Files)
            {
                Console.WriteLine(File);
            }

            foreach (string Directory in Directories)
            {
                recursivePathDive(Directory);
            }
        }
    }
}
