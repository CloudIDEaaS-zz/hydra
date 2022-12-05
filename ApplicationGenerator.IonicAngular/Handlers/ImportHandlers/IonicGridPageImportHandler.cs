using AbstraX.Handlers.ImportHandlers.Packages;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.IONICGRIDPAGE_HANDLER_ID)]
    public class IonicGridPageImportHandler : IonicImportHandler
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((IonicGridPageModules)moduleId);
        }
    }
}
