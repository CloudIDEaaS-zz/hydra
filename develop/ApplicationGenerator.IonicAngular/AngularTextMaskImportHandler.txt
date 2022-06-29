using AbstraX.ImportHandlers.Packages;

namespace AbstraX.ImportHandlers
{
    [ImportHandler(ModuleImports.ANGULARTEXTMASK_HANDLER_ID)]
    public class AngularTextMaskImportHandler : BaseImportHandler<AngularTextMaskPackage>
    {
        public override void AddImport(ulong moduleId)
        {
            base.AddImport((AngularTextMaskModules)moduleId);
        }
    }
}
