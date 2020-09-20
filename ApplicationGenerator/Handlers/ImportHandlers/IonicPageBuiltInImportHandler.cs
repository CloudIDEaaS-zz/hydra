using System.Collections.Generic;
using AbstraX.FolderStructure;
using AbstraX.ImportHandlers.Packages;
using System.Linq;

namespace AbstraX.ImportHandlers
{
    [ImportHandler(ModuleImports.IONICPAGEBUILTIN_HANDLER_ID)]
    public class IonicPageBuiltInImportHandler : BaseBuiltInImportHandler
    {
        public override void AddBuiltInImport(ulong moduleId, IEnumerable<Module> builtInModules, Folder folder, int subFolderCount)
        {
            var importModule = (IonicPageBuiltInModules)moduleId;

            base.AddBuiltInImport(importModule, builtInModules, folder, subFolderCount);
        }
    }
}
