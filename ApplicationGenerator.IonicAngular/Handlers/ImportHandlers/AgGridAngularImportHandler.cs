using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX.ImportHandlers;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.AGGRIDANGULAR_HANDLER_ID)]
    public class AgGridAngularImportHandler : BaseImportHandler<AgGridAngularPackage>
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((AgGridAngularModules)moduleId);
        }
    }
}
