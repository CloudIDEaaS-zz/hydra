using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX.ImportHandlers;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.LINQJAVASCRIPT_HANDLER_ID)]
    public class LinqJavascriptImportHandler : BaseImportHandler<LinqJavascriptPackage>
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((LinqJavascriptModules)moduleId);
        }
    }
}
