using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace CheckForLargeFiles
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hydraSolutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
            var directory = new DirectoryInfo(hydraSolutionPath);
            var largeFiles = directory.GetFiles("*", SearchOption.AllDirectories).Where(f => f.Length > (NumberExtensions.MB * 20)).OrderByDescending(f => f.Length).ToList();
            var repository = new Repository(directory.FullName);

            if (largeFiles.Count > 0)
            {
                Console.WriteLine("List of large files:\r\n");
            }
            else
            {
                Console.WriteLine("No large files detected");
            }

            foreach (var largeFile in largeFiles)
            {
                var status = repository.RetrieveStatus(largeFile.FullName);

                if (status.IsOneOf(FileStatus.NewInWorkdir, FileStatus.ModifiedInWorkdir, FileStatus.RenamedInWorkdir, FileStatus.TypeChangeInWorkdir))
                {
                    Console.WriteLine(largeFile.FullName);
                }
            }

            Console.WriteLine("\r\nHit any key to exit.");
            Console.ReadKey();
        }
    }
}
