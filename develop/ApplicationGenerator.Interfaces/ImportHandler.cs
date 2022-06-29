using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public interface IImportHandler
    {
        void AddImport(ulong moduleId);
        Dictionary<string, Package> Packages { get;  }
        IEnumerable<ModuleImportDeclaration> GetDeclarations();
    }
}
