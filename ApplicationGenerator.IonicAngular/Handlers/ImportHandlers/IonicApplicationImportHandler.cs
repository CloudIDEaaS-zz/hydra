// file:	Handlers\ImportHandlers\IonicApplicationImportHandler.cs
//
// summary:	Implements the ionic application import handler class

using AbstraX.ImportHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.ImportHandlers
{
    /// <summary>   An ionic application import handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/22/2020. </remarks>

    [ImportHandler(ModuleImports.APPLICATION_IMPORT_HANDLER_ID)]
    public class IonicApplicationImportHandler : IImportHandler
    {
        /// <summary>   Gets or sets the packages. </summary>
        ///
        /// <value> The packages. </value>

        public Dictionary<string, Package> Packages { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/22/2020. </remarks>

        public IonicApplicationImportHandler()
        {
            this.Packages = new Dictionary<string, Package>();
        }

        /// <summary>   Adds an import. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/22/2020. </remarks>
        ///
        /// <param name="moduleId"> Identifier for the module. </param>

        public void AddImport(ulong moduleId)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
