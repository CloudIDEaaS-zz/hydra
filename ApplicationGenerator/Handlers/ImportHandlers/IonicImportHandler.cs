using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX.ImportHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.IONIC_HANDLER_ID)]
    public class IonicImportHandler : BaseImportHandler<IonicPackage>
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((IonicModules)moduleId);
        }
    }
}

