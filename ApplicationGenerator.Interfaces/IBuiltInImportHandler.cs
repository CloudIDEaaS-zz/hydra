using AbstraX.FolderStructure;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public interface IBuiltInImportHandler : IImportHandler
    {
        void AddBuiltInImport(ulong moduleId, IEnumerable<Module> builtInModules, Folder folder, int subFolderCount);
        void ClearDeclarations();
    }
}
