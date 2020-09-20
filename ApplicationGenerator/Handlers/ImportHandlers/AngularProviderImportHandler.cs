using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX.ImportHandlers;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.ANGULARPROVIDER_HANDLER_ID)]
    public class AngularProviderImportHandler : BaseImportHandler<AngularPackage>
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((AngularProviderModules)moduleId);
        }
    }
}
