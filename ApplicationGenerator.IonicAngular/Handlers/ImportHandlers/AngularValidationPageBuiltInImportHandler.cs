using AbstraX.FolderStructure;
using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX.ImportHandlers;
using System.Collections.Generic;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.ANGULARVALIDATIONPAGEBUILTIN_HANDLER_ID)]
    public class AngularValidationPageBuiltInImportHandler : BaseBuiltInImportHandler
    {
        public override void AddBuiltInImport(ulong moduleId, IEnumerable<Module> builtInModules, Folder folder, int subFolderCount)
        {
            var importModule = (AngularValidationPageBuiltInModules)moduleId;

            base.AddBuiltInImport(importModule, builtInModules, folder, subFolderCount);
        }
    }
}
