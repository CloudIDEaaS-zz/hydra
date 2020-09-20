using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX.ImportHandlers;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.ANGULARFORMS_HANDLER_ID)]
    public class AngularFormsImportHandler : BaseImportHandler<AngularPackage>
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((AngularFormsModules)moduleId);
        }
    }
}
