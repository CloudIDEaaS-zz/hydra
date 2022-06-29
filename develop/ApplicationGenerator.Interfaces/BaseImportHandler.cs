// file:	BaseImportHandler.cs
//
// summary:	Implements the base import handler class

using AbstraX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.ImportHandlers
{
    /// <summary>   A base import handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/22/2020. </remarks>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>

    public abstract class BaseImportHandler<T> : IImportHandler where T : Package
    {
        /// <summary>   Gets or sets the packages. </summary>
        ///
        /// <value> The packages. </value>

        public Dictionary<string, Package> Packages { get; set; }

        /// <summary>   Adds an import. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/22/2020. </remarks>
        ///
        /// <param name="moduleId"> An enum constant representing the module Identifier option. </param>

        public abstract void AddImport(ulong moduleId);
        /// <summary>   Identifier for the application import handler. </summary>
        public const ulong APPLICATION_IMPORT_HANDLER_ID = ulong.MinValue;

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/22/2020. </remarks>

        public BaseImportHandler()
        {
            this.Packages = new Dictionary<string, Package>();
        }

        /// <summary>   Adds an import. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/22/2020. </remarks>
        ///
        /// <param name="moduleId"> An enum constant representing the module Identifier option. </param>

        public void AddImport(Enum moduleId)
        {
            var importModule = moduleId;
            var importDeclarationAttribute = importModule.GetModuleImportDeclarationAttribute();
            var package = (Package) Activator.CreateInstance(typeof(T), new object[] { this });
            var import = Packages.AddAndOrUpdateDictionary(importDeclarationAttribute.ImportPath, package, i => i.AddModule(importModule));
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
            var list = new List<ModuleImportDeclaration>();

            foreach (var package in Packages.Values)
            {
                list.AddRange(package.GetDeclarations());
            }

            return list;
        }
    }
}
