using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX.ImportHandlers;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.NGXTRANSLATE_HANDLER_ID)]
    public class NgxTranslateImportHandler : BaseImportHandler<NgxTranslatePackage>
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((NgxTranslateModules)moduleId);
        }
    }
}
