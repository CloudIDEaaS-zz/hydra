using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using AbstraX.ImportHandlers;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.ANGULAR_HANDLER_ID)]
    public class AngularImportHandler : BaseImportHandler<AngularPackage>
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((AngularModules)moduleId);
        }
    }
}