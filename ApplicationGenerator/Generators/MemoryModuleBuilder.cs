// file:	Generators\MemoryModuleBuilder.cs
//
// summary:	Implements the memory module builder class

using CodeInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Generators
{
    /// <summary>   A memory module builder. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    public class MemoryModuleBuilder : IMemoryModuleBuilder
    {
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

        public ModuleBuilder CreateMemoryModuleBuilder(IVSProject project)
        {
            var assemblyName = new AssemblyName();
            AppDomain appDomain;
            AssemblyBuilder assemblyBuilder;
            ModuleBuilder moduleBuilder;

            assemblyName.Name = project.Name;
            appDomain = AppDomain.CurrentDomain;
            assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, Path.GetFileNameWithoutExtension(project.FileName) + ".dll");

            return moduleBuilder;
        }
    }
}
