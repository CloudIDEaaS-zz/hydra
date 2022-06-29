// file:	BaseBuiltInImportHandler.cs
//
// summary:	Implements the base built in import handler class

using AbstraX;
using AbstraX.FolderStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.ImportHandlers
{
    /// <summary>   A base built in import handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/22/2020. </remarks>

    public abstract class BaseBuiltInImportHandler : IBuiltInImportHandler
    {
        /// <summary>   The import declarations. </summary>
        protected List<ModuleImportDeclaration> importDeclarations;

        /// <summary>   Adds a built in import. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/22/2020. </remarks>
        ///
        /// <param name="moduleId">         An enum constant representing the module Identifier option. </param>
        /// <param name="builtInModules">   The built in modules. </param>
        /// <param name="folder">           Pathname of the folder. </param>
        /// <param name="subFolderCount">   Number of sub folders. </param>

        public abstract void AddBuiltInImport(ulong moduleId, IEnumerable<Module> builtInModules, Folder folder, int subFolderCount);

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/22/2020. </remarks>

        public BaseBuiltInImportHandler()
        {
            importDeclarations = new List<ModuleImportDeclaration>();
        }

        /// <summary>   Gets the packages. </summary>
        ///
        /// <value> The packages. </value>

        public Dictionary<string, Package> Packages
        {
            get
            {
                return new Dictionary<string, Package>();
            }
        }

        /// <summary>   Adds an import. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/22/2020. </remarks>
        ///
        /// <param name="moduleId"> An enum constant representing the module Identifier option. </param>

        public void AddImport(ulong moduleId)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Adds a built in import. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/22/2020. </remarks>
        ///
        /// <param name="moduleId">         An enum constant representing the module Identifier option. </param>
        /// <param name="builtInModules">   The built in modules. </param>
        /// <param name="folder">           Pathname of the folder. </param>
        /// <param name="subFolderCount">   Number of sub folders. </param>

        protected void AddBuiltInImport(Enum moduleId, IEnumerable<Module> builtInModules, Folder folder, int subFolderCount)
        {
            var importDeclarationAttribute = moduleId.GetModuleImportDeclarationAttribute();

            if (builtInModules.Count() > 0)
            {
                var module = builtInModules.Single(m => m.Name == importDeclarationAttribute.Module);
                ModuleImportDeclaration declaration;

                declaration = module.CreateImportDeclaration(folder, subFolderCount);

                importDeclarations.Add(declaration);
            }
        }

        /// <summary>   Gets the declarations in this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/22/2020. </remarks>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the declarations in this collection.
        /// </returns>

        public IEnumerable<ModuleImportDeclaration> GetDeclarations()
        {
            return importDeclarations;
        }

        /// <summary>   Clears the declarations. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/22/2020. </remarks>

        public void ClearDeclarations()
        {
            importDeclarations.Clear();
        }
    }
}
