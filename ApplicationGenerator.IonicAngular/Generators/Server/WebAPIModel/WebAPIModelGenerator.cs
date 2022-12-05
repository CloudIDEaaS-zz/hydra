﻿using AbstraX.Angular;
using AbstraX.Generators.Server.WebAPIModel;
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

namespace AbstraX.Generators.Server.WebAPIModel
{
    [GeneratorTemplate("IdentityClass", @"Generators\Server\WebAPIModel\WebAPIIdentityModelClassTemplate.tt")]
    [GeneratorTemplate("Class", @"Generators\Server\WebAPIModel\WebAPIModelClassTemplate.tt")]
    public static class WebAPIModelGenerator
    {
        public static void GenerateModel(IBase baseObject, string modelsFolderPath, string modelName, IGeneratorConfiguration generatorConfiguration, List<Generators.EntityProperty> entityProperties)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var exports = new List<ESModule>();
            var declarations = new List<IDeclarable>();
            var isIdentityEntity = generatorConfiguration.IsIdentityEntity(baseObject);
            var root = baseObject.Root;
            var isGeneratedModel = false;
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // WebAPIModelGenerator class

                if (root is IRootWithOptions)
                {
                    var rootWithOptions = (IRootWithOptions)root;

                    isGeneratedModel = rootWithOptions.IsGeneratedModel;
                }

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("EntityProperties", entityProperties);
                sessionVariables.Add("ModelName", modelName);
                sessionVariables.Add("IsGeneratedModel", isGeneratedModel);
                sessionVariables.Add("EntityName", baseObject.Name);

                if (baseObject is IEntityWithPrefix)
                {
                    var entityWithPathPrefix = baseObject.CastTo<IEntityWithPrefix>();

                    fileLocation = PathCombine(modelsFolderPath, entityWithPathPrefix.PathPrefix);

                    sessionVariables.Add("RootNamespace", entityWithPathPrefix.Namespace);
                    sessionVariables.Add("NamespaceSuffix", entityWithPathPrefix.PathPrefix);
                }
                else
                {
                    fileLocation = modelsFolderPath;
                    sessionVariables.Add("RootNamespace", generatorConfiguration.AppName);
                }

                filePath = PathCombine(fileLocation, modelName + ".cs");
                fileInfo = new FileInfo(filePath);

                do
                {
                    if (isIdentityEntity)
                    {
                        host.SetGenerator(typeof(WebAPIModelGenerator), "IdentityClass");
                        output = host.Generate<WebAPIIdentityModelClassTemplate>(sessionVariables, false);
                    }
                    else
                    {
                        host.SetGenerator(typeof(WebAPIModelGenerator), "Class");
                        output = host.Generate<WebAPIModelClassTemplate>(sessionVariables, false);
                    }

                    if (generatorConfiguration.FileSystem.Contains(fileInfo.FullName))
                    {
                        if (pass != GeneratorPass.StructureOnly)
                        {
                            var file = (AbstraX.FolderStructure.File)generatorConfiguration.FileSystem[fileInfo.FullName];

                            if (file.Hash != 0 && file.Hash != output.GetHashCode())
                            {
                                DebugUtils.Break();
                            }
                        }
                    }
                    else
                    {
                        generatorConfiguration.CreateFile(fileInfo, output, FileKind.Services, () => generatorConfiguration.GenerateInfo(sessionVariables, "WebAPIModel Class"));
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
