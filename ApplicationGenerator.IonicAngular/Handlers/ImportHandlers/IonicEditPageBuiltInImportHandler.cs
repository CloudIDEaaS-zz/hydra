using AbstraX.FolderStructure;
using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX.ImportHandlers;
using System.Collections.Generic;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.IONICEDITPAGEBUILTIN_HANDLER_ID)]
    public class IonicEditPageBuiltInImportHandler : BaseBuiltInImportHandler
    {
        public override void AddBuiltInImport(ulong moduleId, IEnumerable<Module> builtInModules, Folder folder, int subFolderCount)
        {
            var importModule = (IonicEditPageBuiltInModules)moduleId;

            base.AddBuiltInImport(importModule, builtInModules, folder, subFolderCount);
        }
    }
}
