using AbstraX.ImportHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.APPLICATION_IMPORT_HANDLER_ID)]
    public class IonicApplicationImportHandler : IImportHandler
    {
        public Dictionary<string, Package> Packages { get; set; }

        public IonicApplicationImportHandler()
        {
            this.Packages = new Dictionary<string, Package>();
        }

        public void AddImport(ulong moduleId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ModuleImportDeclaration> GetDeclarations()
        {
            throw new NotImplementedException();
        }
    }
}
