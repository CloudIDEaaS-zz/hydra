using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using System.Windows.Forms;
using EnvDTE;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.IO.Packaging;
using Utils;

namespace BuildTasks
{
    public class ZipTemplates : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            try
            {
                ZipStandaloneTemplates();
                ZipVSIXTemplates();

                Console.WriteLine("Finished zipping templates successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error zipping templates: '{0}'", ex));
                return false;
            }

            return true;
        }

        private void ZipVSIXTemplates()
        {
            var projectFile = new FileInfo(this.BuildEngine.ProjectFileOfTaskNode);
            var templateLocation = new DirectoryInfo(Path.Combine(projectFile.Directory.Parent.FullName, @"CodeGenerationPipeline\VSIX"));

            Console.WriteLine("Zipping VSIX templates");

            foreach (var subDir in templateLocation.GetDirectories())
            {
                Console.WriteLine("Zipping " + subDir.Name);

                var fileInfo = new FileInfo(Path.Combine(subDir.FullName, subDir.Name + ".vsix"));

                if (fileInfo.Exists && fileInfo.IsReadOnly)
                {
                    fileInfo.Attributes &= (~FileAttributes.ReadOnly);
                }

                using (var stream = fileInfo.Open(FileMode.Create, FileAccess.ReadWrite))
                {
                    using (var zipPackage = ZipPackage.Open(stream, FileMode.CreateNew))
                    {
                        RecurseVSIX(subDir, zipPackage, subDir.FullName);
                    }
                }
            }

            Console.WriteLine("Finished zipping VSIX templates successfully");
        }

        private void RecurseVSIX(DirectoryInfo directory, ZipOutputStream zipStream, string rootPath, bool skipZips = true)
        {
            foreach (var subDirectory in directory.GetDirectories())
            {
                Console.WriteLine("Zipping " + subDirectory.Name);

                RecurseVSIX(subDirectory, zipStream, rootPath, skipZips);
            }

            foreach (var file in directory.GetFiles())
            {
                if ((file.Extension.ToLower() != ".zip" || !skipZips) && file.Extension.ToLower() != ".vsix")
                {
                    var name = file.FullName.Remove(0, rootPath.Length + (rootPath.EndsWith(@"\") ? 0 : 1));
                    var entry = new ZipEntry(name);

                    zipStream.PutNextEntry(entry);

                    using (var inputStream = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write))
                    {
                        var buffer = new byte[2048];
                        int length;

                        while ((length = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            zipStream.Write(buffer, 0, length);
                        }
                    }
                }
            }
        }

        private void RecurseVSIX(DirectoryInfo directory, Package zipPackage, string rootPath, bool skipZips = true, bool zipSubDirectories = true)
        {
            var subZips = new List<string>();

            if (zipSubDirectories)
            {
                foreach (var subDirectory in directory.GetDirectories())
                {
                    var fileInfo = new FileInfo(Path.Combine(subDirectory.FullName, subDirectory.Name + ".zip"));

                    subZips.Add(fileInfo.FullName);

                    using (var stream = fileInfo.Open(FileMode.Create, FileAccess.ReadWrite))
                    {
                        using (var zipStream = new ZipOutputStream(stream))
                        {
                            zipStream.UseZip64 = UseZip64.Off;
                            RecurseVSIX(subDirectory, zipStream, subDirectory.FullName);
                        }
                    }

                    foreach (var file in subDirectory.GetFiles("*.zip"))
                    {
                        if (subZips.Contains(file.FullName))
                        {
                            var name = file.FullName.Remove(0, rootPath.Length).Replace("\\", "/");
                            var part = zipPackage.CreatePart(new Uri(name, UriKind.Relative), "application/zip", CompressionOption.Normal);
                            var outputStream = part.GetStream();

                            using (var inputStream = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write))
                            {
                                byte[] buffer = new byte[2048];
                                int length;

                                while ((length = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    outputStream.Write(buffer, 0, length);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var subDirectory in directory.GetDirectories())
                {
                    RecurseVSIX(subDirectory, zipPackage, subDirectory.FullName, skipZips, false);
                }
            }

            foreach (var file in directory.GetFiles())
            {
                if ((file.Extension.ToLower() != ".zip" || !skipZips || subZips.Contains(file.FullName)) && file.Extension.ToLower() != ".vsix" && file.Name.AsCaseless() != "[Content_Types].xml")
                {
                    var mimeType = string.Empty;

                    if (file.Extension.IsOneOf(".jpg", ".ico", ".png", ".bmp", ".gif", ".dll", ".exe", ".pdb"))
                    {
                        mimeType = "application/octet-stream";
                    }
                    else if (file.Extension.IsOneOf(".xml", ".vsixmanifest", ".vsix", ".vstemplate", ".rtf", ".htm", ".config"))
                    {
                        mimeType = "text/xml";
                    }
                    else if (file.Extension == ".zip")
                    {
                        mimeType = "application/zip";
                    }
                    else
                    {
                        var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, string.Format("Unexpected package file extension for '{0}'", file.Name), "", "", DateTime.Now);
                        this.BuildEngine.LogErrorEvent(message);

                        continue;
                    }

                    var name = file.FullName.Remove(0, rootPath.Length).Replace("\\", "/");
                    var part = zipPackage.CreatePart(new Uri(name, UriKind.Relative), mimeType, CompressionOption.Normal);
                    var outputStream = part.GetStream();

                    using (var inputStream = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write))
                    {
                        byte[] buffer = new byte[2048];
                        int length;

                        while ((length = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outputStream.Write(buffer, 0, length);
                        }
                    }
                }
            }
        }

        private void ZipStandaloneTemplates()
        {
            var projectFile = new FileInfo(this.BuildEngine.ProjectFileOfTaskNode);
            var templateLocation = new DirectoryInfo(Path.Combine(projectFile.Directory.Parent.FullName, @"CodeGenerationPipeline\VSTemplates"));

            Console.WriteLine("Zipping standalone templates");

            foreach (var subDir in templateLocation.GetDirectories())
            {
                if (subDir.Name != "CommonFiles")
                {
                    Console.WriteLine("Zipping " + subDir.Name);

                    var fileInfo = new FileInfo(Path.Combine(subDir.FullName, subDir.Name + ".zip"));

                    if (fileInfo.Exists && fileInfo.IsReadOnly)
                    {
                        fileInfo.Attributes &= (~FileAttributes.ReadOnly);
                    }

                    using (var stream = fileInfo.Open(FileMode.Create, FileAccess.ReadWrite))
                    {
                        using (var zipStream = new ZipOutputStream(stream))
                        {
                            zipStream.UseZip64 = UseZip64.Off;
                            RecurseStandalone(subDir, zipStream, subDir.FullName);
                        }
                    }
                }
            }

            Console.WriteLine("Finished zipping standalone templates successfully");
        }

        private void RecurseStandalone(DirectoryInfo directory, ZipOutputStream zipStream, string rootPath)
        {
            foreach (var file in directory.GetFiles())
            {
                if (file.Extension.ToLower() != ".zip")
                {
                    var name = file.FullName.Remove(0, rootPath.Length + (rootPath.EndsWith(@"\") ? 0 : 1));
                    var entry = new ZipEntry(name);

                    zipStream.PutNextEntry(entry);

                    using (var inputStream = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write))
                    {
                        byte[] buffer = new byte[2048];
                        int length;

                        while ((length = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            zipStream.Write(buffer, 0, length);
                        }
                    }
                }
            }

            foreach (var subDirectory in directory.GetDirectories())
            {
                //RecurseVSIX(subDirectory, zipStream, rootPath); 
            }
        }
    }
}
