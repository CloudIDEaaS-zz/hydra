using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX.ImportHandlers;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.ANGULARVALIDATOR_HANDLER_ID)]
    public class AngularValidatorImportHandler : BaseImportHandler<AngularPackage>
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((AngularValidatorModules)moduleId);
        }
    }
}
