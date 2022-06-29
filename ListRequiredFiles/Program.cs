using Hydra.ReleaseManagement.Entities.Models.ValueTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace ListRequiredFiles
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string json;
            var root = @"D:\MC\CloudIDEaaS\develop\ApplicationGenerator\bin\Debug";
            var requiredFiles = new RequiredFiles();
            var folders = new[]
            {
                "Defaults",
                "Generators",
                "GeneratorTemplates",
                "Handlers",
                "Marketing",
                "Start Menu",
                "Templates",
                "Themes"
            };

            requiredFiles.RequiredFolders = new List<Folder>();

            foreach (var folder in folders)
            {
                var directory = new DirectoryInfo(Path.Combine(root, folder));
                var folder2 = new Folder(directory.FullName.RemoveStart(root));

                foreach (var file in directory.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    folder2.Files.Add(file.FullName.RemoveStart(directory.FullName));
                }

                requiredFiles.RequiredFolders.Add(folder2);
            }

            json = requiredFiles.ToJsonText();

            Console.WriteLine(json.RemoveSurrounding("{", "}"));
            Console.WriteLine("\r\nHit any key to exit");

            Console.ReadKey();
        }
    }
}
