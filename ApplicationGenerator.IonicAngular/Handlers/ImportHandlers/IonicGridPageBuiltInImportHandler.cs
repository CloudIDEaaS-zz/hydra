using System.Collections.Generic;
using AbstraX.FolderStructure;
using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX;
using System;
using System.Linq;
using AbstraX.ImportHandlers;

namespace AbstraX.Handlers.ImportHandlers
{
    [ImportHandler(ModuleImports.IONICGRIDPAGEBUILTIN_HANDLER_ID)]
    public class IonicGridPageBuiltInImportHandler : BaseBuiltInImportHandler
    {
        public override void AddBuiltInImport(ulong moduleId, IEnumerable<Module> builtInModules, Folder folder, int subFolderCount)
        {
            var importModule = (IonicGridPageBuiltInModules) moduleId;

            base.AddBuiltInImport(importModule, builtInModules, folder, subFolderCount);
        }
    }
}
