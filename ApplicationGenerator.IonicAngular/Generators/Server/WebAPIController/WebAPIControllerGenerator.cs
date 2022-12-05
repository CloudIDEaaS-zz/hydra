using AbstraX.Angular;
using AbstraX.Generators.Server.WebAPIController;
using AbstraX.Models.Interfaces;
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

namespace AbstraX.Generators.Server.WebAPIController
{
    [GeneratorTemplate("IdentityClass", @"Generators\Server\WebAPIController\WebAPIIdentityControllerClassTemplate.tt")]
    [GeneratorTemplate("Class", @"Generators\Server\WebAPIController\WebAPIControllerClassTemplate.tt")]
    public static class WebAPIControllerGenerator
    {
        public static void GenerateController(IBase baseObject, string controllersFolderPath, string controllerName, IGeneratorConfiguration generatorConfiguration, List<RelatedEntity> relatedEntities, List<Generators.EntityProperty> entityProperties)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var exports = new List<ESModule>();
            var declarations = new List<IDeclarable>();
            var isIdentityEntity = generatorConfiguration.IsIdentityEntity(baseObject);
            var element = (IElement)baseObject;
            var keyAttribute = element.GetKey();
            var keyType = keyAttribute.GetShortType();
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // WebAPI controller class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("ControllerName", controllerName);
                sessionVariables.Add("EntityName", baseObject.Name);
                sessionVariables.Add("RelatedEntities", relatedEntities);
                sessionVariables.Add("EntityProperties", entityProperties);
                sessionVariables.Add("Container", baseObject.GetContainer());
                sessionVariables.Add("ContainerSet", baseObject.GetContainerSet().Name);
                sessionVariables.Add("RootNamespace", generatorConfiguration.AppName);
                sessionVariables.Add("KeyName", keyAttribute.Name);
                sessionVariables.Add("KeyType", keyType);

                if (generatorConfiguration.CustomQueries.ContainsKey(baseObject))
                {
                    var queriesList = generatorConfiguration.CustomQueries[baseObject];

                    sessionVariables.Add("CustomQueries", queriesList);
                }
                else if (generatorConfiguration.CustomQueries.ContainsNavigationKey(baseObject))
                {
                    var queriesList = generatorConfiguration.CustomQueries.GetNavigationValue(baseObject);

                    sessionVariables.Add("CustomQueries", queriesList);
                }

                if (baseObject is IEntityWithPrefix)
                {
                    var entityWithPrefix = baseObject.CastTo<IEntityWithPrefix>();

                    fileLocation = PathCombine(controllersFolderPath, entityWithPrefix.PathPrefix, controllerName);

                    controllerName = entityWithPrefix.ControllerNamePrefix + controllerName;
                }
                else
                {
                    fileLocation = PathCombine(controllersFolderPath, controllerName);
                }

                filePath = PathCombine(fileLocation, controllerName + "Controller.cs");
                fileInfo = new FileInfo(filePath);

                if (baseObject is IElementWithSurrogateTemplateType)
                {
                    var elementWithSurrogateTemplateType = (IElementWithSurrogateTemplateType)baseObject;

                    if (elementWithSurrogateTemplateType.HasSurrogateTemplateType<WebAPIControllerClassTemplate>())
                    {
                        var templateType = elementWithSurrogateTemplateType.GetSurrogateTemplateType<WebAPIControllerClassTemplate>();

                        output = host.Generate(templateType, sessionVariables, false);

                        if (generatorConfiguration.FileSystem.Contains(fileInfo.FullName))
                        {
                            if (pass != GeneratorPass.StructureOnly)
                            {
                                var file = (AbstraX.FolderStructure.File)generatorConfiguration.FileSystem[fileInfo.FullName];

                                if (file.Hash != output.GetHashCode())
                                {
                                    // DebugUtils.Break();
                                }
                            }
                        }
                        else
                        {
                            generatorConfiguration.CreateFile(fileInfo, output, FileKind.Services, () => generatorConfiguration.GenerateInfo(sessionVariables, "WebAPIController Class"));
                        }

                        return;
                    }
                }

                do
                {
                    if (isIdentityEntity)
                    {
                        host.SetGenerator(typeof(WebAPIControllerGenerator), "IdentityClass");
                        output = host.Generate<WebAPIIdentityControllerClassTemplate>(sessionVariables, false);
                    }
                    else
                    {
                        host.SetGenerator(typeof(WebAPIControllerGenerator), "Class");
                        output = host.Generate<WebAPIControllerClassTemplate>(sessionVariables, false);
                    }

                    if (generatorConfiguration.FileSystem.Contains(fileInfo.FullName))
                    {
                        if (pass != GeneratorPass.StructureOnly)
                        {
                            var file = (AbstraX.FolderStructure.File)generatorConfiguration.FileSystem[fileInfo.FullName];

                            if (file.Hash != output.GetHashCode())
                            {
                                // DebugUtils.Break();
                            }
                        }
                    }
                    else
                    {
                        generatorConfiguration.CreateFile(fileInfo, output, FileKind.Services, () => generatorConfiguration.GenerateInfo(sessionVariables, "WebAPIController Class"));
                    }
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
