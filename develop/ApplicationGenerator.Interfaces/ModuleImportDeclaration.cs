// file:	ModuleImportDeclaration.cs
//
// summary:	Implements the module import declaration class

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using Utils;

namespace AbstraX
{
    /// <summary>   Attribute for module import declaration. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/21/2021. </remarks>

    [AttributeUsage(AttributeTargets.Field)]
    public class ModuleImportDeclarationAttribute : Attribute
    {
        /// <summary>   Gets the module. </summary>
        ///
        /// <value> The module. </value>

        public string Module { get; }

        /// <summary>   Gets the full pathname of the import file. </summary>
        ///
        /// <value> The full pathname of the import file. </value>

        public string ImportPath { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/21/2021. </remarks>
        ///
        /// <param name="module">       The module. </param>
        /// <param name="importPath">   Full pathname of the import file. </param>

        public ModuleImportDeclarationAttribute(string module, string importPath)
        {
            this.Module = module;
            this.ImportPath = importPath;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/21/2021. </remarks>
        ///
        /// <param name="importPath">   Full pathname of the import file. </param>

        public ModuleImportDeclarationAttribute(string importPath)
        {
            this.ImportPath = importPath;
        }
    }

    /// <summary>   A module import declaration. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/21/2021. </remarks>

    [DebuggerDisplay(" { DeclarationCode } ")]
    public class ModuleImportDeclaration
    {
        /// <summary>   Gets or sets the full pathname of the import file. </summary>
        ///
        /// <value> The full pathname of the import file. </value>

        public string ImportPath { get; protected set; }

        /// <summary>   Gets or sets a list of names of the modules. </summary>
        ///
        /// <value> A list of names of the modules. </value>

        public string[] ModuleNames { get; protected set; }

        /// <summary>   Gets or sets the module or assembly. </summary>
        ///
        /// <value> The module or assembly. </value>

        public IModuleOrAssembly ModuleOrAssembly { get; protected set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/21/2021. </remarks>
        ///
        /// <param name="importPath">   Full pathname of the import file. </param>
        /// <param name="moduleNames">  A variable-length parameters list containing module names. </param>

        public ModuleImportDeclaration(string importPath, params string[] moduleNames)
        {
            this.ImportPath = importPath;
            this.ModuleNames = moduleNames;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/21/2021. </remarks>
        ///
        /// <param name="importPath">       Full pathname of the import file. </param>
        /// <param name="moduleOrAssembly"> The module or assembly. </param>
        /// <param name="moduleName">       Name of the module. </param>

        public ModuleImportDeclaration(string importPath, IModuleOrAssembly moduleOrAssembly, string moduleName)
        {
            this.ImportPath = importPath;
            this.ModuleNames = new List<string> { moduleName }.ToArray();
            this.ModuleOrAssembly = moduleOrAssembly;
        }

        /// <summary>   Gets the declaration code. </summary>
        ///
        /// <value> The declaration code. </value>

        public string DeclarationCode
        {
            get
            {
                if (this.ModuleNames.IsEmpty())
                {
                    return string.Format("import '{0}';", this.ImportPath);
                }
                else
                {
                    return string.Format("import {{ {0} }} from '{1}';", this.ModuleNames.ToCommaDelimitedList(), this.ImportPath);
                }
            }
        }

        /// <summary>   Returns a string that represents the current object. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/21/2021. </remarks>
        ///
        /// <returns>   A string that represents the current object. </returns>

        public override string ToString()
        {
            return this.DeclarationCode;
        }
    }
}
