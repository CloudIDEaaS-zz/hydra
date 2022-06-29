using AbstraX.ImportHandlers.Packages;

namespace AbstraX.ImportHandlers
{
    [ImportHandler(ModuleImports.ANGULARROUTER_HANDLER_ID)]
    public class AngularRouterImportHandler : BaseImportHandler<AngularRouterPackage>
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((AngularRouterModules)moduleId);
        }
    }
}
