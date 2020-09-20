using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX.ImportHandlers;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.RXJSMAP_HANDLER_ID)]
    public class RxJsMapImportHandler : BaseImportHandler<RxJsPackage>
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((RxJsMapModules)moduleId);
        }
    }
}
