// file:	Generators\Server\EntityModel\EntityModelGenerator.cs
//
// summary:	Implements the entity model generator class

using AbstraX.Angular;
using AbstraX.Generators.Server.WebAPIModel;
using AbstraX.Models.Interfaces;
using AbstraX.ServerInterfaces;
using AbstraX.TemplateObjects;
using CodeInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static AbstraX.FolderStructure.FileSystemObject;

namespace AbstraX.Generators.Server.EntityModel
{
    /// <summary>   An entity model generator. </summary>
    ///
    /// <remarks>   Ken, 10/11/2020. </remarks>

    [GeneratorTemplate("Class", @"Generators\Server\EntityModel\EntityModelClassTemplate.tt")]
    [GeneratorTemplate("MetadataClass", @"Generators\Server\EntityModel\EntityModelMetadataClassTemplate.tt")]
    [GeneratorTemplate("AppResourcesClass", @"Generators\Server\EntityModel\AppResourcesClassTemplate.tt")]
    public static class EntityModelGenerator
    {
        /// <summary>   Generates a model. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="entityObject">             The entity object. </param>
        /// <param name="appUIHierarchyNodeObject"> The application user interface hierarchy node object. </param>
        /// <param name="modelsFolderPath">         Full pathname of the models folder file. </param>
        /// <param name="modelsMetadataFolderPath"> Full pathname of the models metadata folder file. </param>
        /// <param name="appResourcesPath">         Full pathname of the application resources file. </param>
        /// <param name="entitiesProject">          The entities project. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>

        public static void GenerateModel(EntityObject entityObject, AppUIHierarchyNodeObject appUIHierarchyNodeObject, string modelsFolderPath, string modelsMetadataFolderPath, string appResourcesPath, IVSProject entitiesProject, IGeneratorConfiguration generatorConfiguration)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string modelName;
            string output;

            try
            {
                // EntityModel class

                sessionVariables = new Dictionary<string, object>();
                modelName = entityObject.Name.RemoveSpaces();

                sessionVariables.Add("EntityObject", entityObject);

                fileLocation = modelsFolderPath;
                
                filePath = PathCombine(fileLocation, modelName + ".cs");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(EntityModelGenerator), "Class");

                do
                {
                    output = host.Generate<EntityModelClassTemplate>(sessionVariables, false);

                    if (generatorConfiguration.FileSystem.Contains(fileInfo.FullName))
                    {
                        if (pass != GeneratorPass.StructureOnly)
                        {
                            var file = (AbstraX.FolderStructure.File)generatorConfiguration.FileSystem[fileInfo.FullName];

                            if (file.Hash != 0 && file.Hash != output.GetHashCode())
                            {
                                DebugUtils.Break();
                            }

                            generatorConfiguration.CreateFile(fileInfo, output, FileKind.Entities, () => generatorConfiguration.GenerateInfo(sessionVariables, "EntityModel Class"));
                        }
                    }
                    else
                    {
                        generatorConfiguration.CreateFile(fileInfo, output, FileKind.Entities, () => generatorConfiguration.GenerateInfo(sessionVariables, "EntityModel Class"));
                    }
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                fileLocation = modelsMetadataFolderPath;

                filePath = PathCombine(fileLocation, modelName + ".metadata.cs");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(EntityModelGenerator), "MetadataClass");

                do
                {
                    output = host.Generate<EntityModelMetadataClassTemplate>(sessionVariables, false);

                    if (generatorConfiguration.FileSystem.Contains(fileInfo.FullName))
                    {
                        if (pass != GeneratorPass.StructureOnly)
                        {
                            var file = (AbstraX.FolderStructure.File)generatorConfiguration.FileSystem[fileInfo.FullName];

                            if (file.Hash != 0 && file.Hash != output.GetHashCode())
                            {
                                DebugUtils.Break();
                            }

                            generatorConfiguration.CreateFile(fileInfo, output, FileKind.Entities, () => generatorConfiguration.GenerateInfo(sessionVariables, "EntityModel Metadata Class"));
                        }
                    }
                    else
                    {
                        generatorConfiguration.CreateFile(fileInfo, output, FileKind.Entities, () => generatorConfiguration.GenerateInfo(sessionVariables, "EntityModel Metadata Class"));
                    }
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                if (entityObject == appUIHierarchyNodeObject.EntityContainer)
                {
                    var userEntity = appUIHierarchyNodeObject.AllEntities.SingleOrDefault(e => e.Name == "User");

                    fileLocation = appResourcesPath;

                    filePath = PathCombine(fileLocation, "AppResources.cs");
                    fileInfo = new FileInfo(filePath);

                    if (userEntity != null)
                    {
                        sessionVariables.Add("CustomQueries", userEntity.CustomQueries.Values.ToList());
                    }
                    else
                    {
                        sessionVariables.Add("CustomQueries", new List<CustomQuery>());
                    }

                    host.SetGenerator(typeof(EntityModelGenerator), "AppResourcesClass");

                    do
                    {
                        output = host.Generate<AppResourcesClassTemplate>(sessionVariables, false);

                        if (generatorConfiguration.FileSystem.Contains(fileInfo.FullName))
                        {
                            if (pass != GeneratorPass.StructureOnly)
                            {
                                var file = (AbstraX.FolderStructure.File)generatorConfiguration.FileSystem[fileInfo.FullName];

                                if (file.Hash != 0 && file.Hash != output.GetHashCode())
                                {
                                    DebugUtils.Break();
                                }

                                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Entities, () => generatorConfiguration.GenerateInfo(sessionVariables, "App Resources Class"));
                            }
                        }
                        else
                        {
                            generatorConfiguration.CreateFile(fileInfo, output, FileKind.Entities, () => generatorConfiguration.GenerateInfo(sessionVariables, "App Resources Class"));
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
