using AbstraX.ImportHandlers.Packages;

namespace AbstraX.ImportHandlers
{
    [ImportHandler(ModuleImports.SUPERTABS_HANDLER_ID)]
    public class SuperTabsImportHandler : BaseImportHandler<SuperTabsPackage>
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((SuperTabsModules)moduleId);
        }
    }
}
