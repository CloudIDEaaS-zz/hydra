// file:	Generators\MemoryModuleBuilder.cs
//
// summary:	Implements the memory module builder class

using AbstraX.TemplateObjects;
using CodeInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Generators
{
    /// <summary>   A memory module builder. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    public class DynamicModuleBuilder : IDynamicModuleBuilder
    {
        private AppDomain appDomain;
        private AssemblyBuilder assemblyBuilder;
        private ModuleBuilder moduleBuilder;
        private string path;
        private AssemblyName assemblyName;
        private IVSProject project;
        private static int buildNumber;

        /// <summary>   Gets the priority. </summary>
        ///
        /// <value> The priority. </value>

        public float Priority => 1.0f;

        /// <summary>   Creates memory module. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="project">  The project. </param>
        ///
        /// <returns>   The new memory module. </returns>

        public ModuleBuilder CreateDynamicModuleBuilder(IVSProject project)
        {
            buildNumber++;

            path = Path.Combine(project.OutputPath, Guid.NewGuid().ToString());

            assemblyName = new AssemblyName();

            this.project = project;

            assemblyName.Name = project.Name;
            assemblyName.Version = new Version(1, 0, 0, buildNumber);
            appDomain = AppDomain.CurrentDomain;
            assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, path);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");

            return moduleBuilder;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>

        public void Dispose()
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                var directory = new DirectoryInfo(path);

                foreach (var subDirectory in directory.Parent.GetDirectories())
                {
                    Guid guid;

                    if (Guid.TryParse(subDirectory.Name, out guid))
                    {
                        try
                        {
                            subDirectory.ForceDelete();
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            };
        }

        /// <summary>   Loads the assembly. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>

        public Assembly LoadAndAttachToAssembly(AppUIHierarchyNodeObject appUIHierarchyNodeObject)
        {
            var location = Path.Combine(path, assemblyName.Name + ".dll");
            Assembly entitiesAssembly;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            assemblyBuilder.Save(assemblyName.Name + ".dll");

            entitiesAssembly = Assembly.LoadFrom(location);

            foreach (var entity in appUIHierarchyNodeObject.AllEntities.Concat(new List<EntityObject> { appUIHierarchyNodeObject.EntityContainer }))
            {
                var type = entitiesAssembly.GetTypes().Single(t => t.FullName == entity.DynamicEntityType.FullName);
                var metadataType = entitiesAssembly.GetTypes().Single(t => t.FullName == entity.DynamicEntityMetadataType.FullName);

                entity.DynamicEntityType = type;
                entity.DynamicEntityMetadataType = metadataType;
            }

            return entitiesAssembly;
        }
    }
}
