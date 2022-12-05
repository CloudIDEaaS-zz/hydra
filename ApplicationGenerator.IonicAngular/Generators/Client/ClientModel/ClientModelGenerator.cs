using AbstraX.Angular;
using AbstraX.Generators.Client.ClientModel;
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

namespace AbstraX.Generators.Client.ClientModel
{
    [GeneratorTemplate("Class", @"Generators\Client\ClientModel\ClientModelClassTemplate.tt")]
    public static class ClientModelGenerator
    {
        public static void GenerateModel(IBase baseObject, string modelsFolderPath, string modelName, IGeneratorConfiguration generatorConfiguration, IEnumerable<ModuleImportDeclaration> imports, List<FormField> formFields)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var moduleAssemblyProperties = new AngularModuleAssemblyProperties(baseObject, imports);
            var newModules = new List<ESModule>();
            var isIdentityEntity = generatorConfiguration.IsIdentityEntity(baseObject);
            List<ESModule> modules = null;
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // Client Model class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("FormFields", formFields);
                sessionVariables.Add("ModelName", modelName);
                sessionVariables.Add("EntityName", baseObject.Name);
                sessionVariables.Add("IsIdentityEntity", isIdentityEntity);
                sessionVariables.AddModuleAssemblyProperties(moduleAssemblyProperties);

                fileLocation = modelsFolderPath;
                filePath = Path.Combine(fileLocation, modelName.ToLower() + ".model.ts");
                fileInfo = new FileInfo(filePath);

                do
                {
                    host.SetGenerator(typeof(ClientModelGenerator), "Class");
                    output = host.Generate<ClientModelClassTemplate>(sessionVariables, false);

                    if (generatorConfiguration.KeyValuePairs.ContainsKey("Modules"))
                    {
                        modules = (List<ESModule>)generatorConfiguration.KeyValuePairs["Modules"];
                    }
                    else
                    {
                        modules = new List<ESModule>();

                        generatorConfiguration.KeyValuePairs.Add("Modules", modules);
                    }

                    foreach (var module in moduleAssemblyProperties.Modules)
                    {
                        module.BaseObject = baseObject;

                        if (!modules.Contains(module))
                        {
                            modules.Add(module);
                        }

                        newModules.Add(module);
                    }

                    generatorConfiguration.CreateFile(fileInfo, newModules.Cast<Module>(), output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Client Model Class"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
