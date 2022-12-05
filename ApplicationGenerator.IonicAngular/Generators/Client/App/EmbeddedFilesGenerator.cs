using AbstraX.Generators.Pages.TabPage;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static AbstraX.FolderStructure.FileSystemObject;
using Utils.Hierarchies;
using AbstraX.GeneratorEngines;

namespace AbstraX.Generators.Client.App
{
    public static class EmbeddedFilesGenerator
    {
        public static void Generate(string appProjectPath, IGeneratorConfiguration generatorConfiguration)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var assembly = Assembly.GetEntryAssembly();
            var type = typeof(AngularModules);

            try
            {
                if (pass == GeneratorPass.Files)
                {
                    var zipStream = type.ReadResource<Stream>("app.zip");

                    using (var package = ZipPackage.Open(zipStream))
                    {
                        var parts = package.GetParts();

                        foreach (var part in parts)
                        {
                            var uri = part.Uri;

                            using (var stream = part.GetStream(FileMode.Open, FileAccess.Read))
                            {
                                var filePath = PathCombine(appProjectPath, uri.OriginalString.BackSlashes().RemoveStartIfMatches(@"\"));
                                var fileLocation = Path.GetDirectoryName(filePath);
                                FolderStructure.File file;

                                if (File.Exists(filePath))
                                {
                                    File.Delete(filePath);
                                }

                                if (!Directory.Exists(fileLocation))
                                {
                                    Directory.CreateDirectory(fileLocation);
                                }

                                using (FileStream fileStream = File.Create(filePath))
                                {
                                    List<Module> modules = null;
                                    var contents = stream.ToText();

                                    if (Path.GetExtension(filePath) == ".ts")
                                    {
                                        modules = ModulesExtractor.GetModules(contents, filePath);

                                        generatorConfiguration.AddBuiltInModule(modules.ToArray());

                                        stream.Rewind();
                                    }

                                    fileStream.Write(stream.ToArray());
                                    generatorConfiguration.FileSystem.DeleteFile(filePath);

                                    file = generatorConfiguration.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath));

                                    if (modules != null)
                                    {
                                        modules.AddToFile(file);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
                throw;
            }
        }
    }
}
