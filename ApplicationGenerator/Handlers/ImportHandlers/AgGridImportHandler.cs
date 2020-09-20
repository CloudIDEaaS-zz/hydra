using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX.ImportHandlers;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.AGGRID_HANDLER_ID)]
    public class AgGridImportHandler : BaseImportHandler<AgGridPackage>
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((AgGridModules)moduleId);
        }
    }
}
