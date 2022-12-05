using AbstraX.Angular;
using AbstraX.Generators.Client.Internationalization;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static AbstraX.FolderStructure.FileSystemObject;

namespace AbstraX.Generators.Client.Internationalization
{
    [GeneratorTemplate("LanguageTranslation", @"Generators\Client\Internationalization\LanguageTranslationFileTemplate.tt")]
    public static class LanguageTranslationGenerator
    {
        public static void GenerateTranslations(string i18nFolderPath, IGeneratorConfiguration generatorConfiguration)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var exports = new List<ESModule>();
            var declarations = new List<IDeclarable>();
            Dictionary<string, object> sessionVariables;
            FolderStructure.File file;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                foreach (var dictionary in generatorConfiguration.LanguageDictionary.LanguageSpecificDictionaries.Values)
                {
                    // LanguageTranslation file

                    sessionVariables = new Dictionary<string, object>();

                    sessionVariables.Add("Dictionary", dictionary);

                    fileLocation = i18nFolderPath;
                    filePath = PathCombine(fileLocation, dictionary.LanguageCode + ".json");

                    do
                    {
                        host.SetGenerator(typeof(LanguageTranslationGenerator), "LanguageTranslation");
                        output = host.Generate<LanguageTranslationFileTemplate>(sessionVariables, false);

                        if (pass == GeneratorPass.Files)
                        {
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }

                            if (!Directory.Exists(fileLocation))
                            {
                                generatorConfiguration.CreateDirectory(fileLocation);
                            }

                            using (var fileStream = generatorConfiguration.CreateFile(filePath))
                            {
                                fileStream.Write(output);
                                generatorConfiguration.FileSystem.DeleteFile(filePath);

                                file = generatorConfiguration.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath));
                            }
                        }
                        else if (pass == GeneratorPass.StructureOnly)
                        {
                            file = generatorConfiguration.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath), generatorConfiguration.GenerateInfo(sessionVariables, "{\r\n\t\"Test\": \"Test\"\r\n}"));
                        }
                    }
                    while (host.PostProcess() == PostProcessResult.RedoGenerate);
                }
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
