using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX.ImportHandlers;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.LINQCOLLECTIONS_HANDLER_ID)]
    public class LinqCollectionsImportHandler : BaseImportHandler<LinqCollectionsPackage>
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((LinqCollectionsModules)moduleId);
        }
    }
}
