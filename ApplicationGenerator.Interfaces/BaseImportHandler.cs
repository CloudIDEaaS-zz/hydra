using AbstraX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.ImportHandlers
{
    public abstract class BaseImportHandler<T> : IImportHandler where T : Package, new()
    {
        public Dictionary<string, Package> Packages { get; set; }
        public abstract void AddImport(ulong moduleId);
        public const ulong APPLICATION_IMPORT_HANDLER_ID = ulong.MinValue;

        public BaseImportHandler()
        {
            this.Packages = new Dictionary<string, Package>();
        }

        public void AddImport(Enum moduleId)
        {
            var importModule = moduleId;
            var importDeclarationAttribute = importModule.GetModuleImportDeclarationAttribute();
            var import = Packages.AddAndOrUpdateDictionary(importDeclarationAttribute.ImportPath, new T(), i => i.AddModule(importModule));
        }

        public IEnumerable<ModuleImportDeclaration> GetDeclarations()
        {
            var list = new List<ModuleImportDeclaration>();

            foreach (var package in Packages.Values)
            {
                list.AddRange(package.GetDeclarations());
            }

            return list;
        }
    }
}
