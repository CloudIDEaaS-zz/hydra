using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX.ImportHandlers;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.RXJSOBSERVABLE_HANDLER_ID)]
    public class RxJsObservableImportHandler : BaseImportHandler<RxJsPackage>
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((RxJsObservableModules)moduleId);
        }
    }
}
