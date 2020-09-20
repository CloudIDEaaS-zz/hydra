using AbstraX;
using AbstraX.FolderStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.ImportHandlers
{
    public abstract class BaseBuiltInImportHandler : IBuiltInImportHandler
    {
        protected List<ModuleImportDeclaration> importDeclarations;
        public abstract void AddBuiltInImport(ulong moduleId, IEnumerable<Module> builtInModules, Folder folder, int subFolderCount);

        public BaseBuiltInImportHandler()
        {
            importDeclarations = new List<ModuleImportDeclaration>();
        }

        public Dictionary<string, Package> Packages
        {
            get
            {
                return new Dictionary<string, Package>();
            }
        }

        public void AddImport(ulong moduleId)
        {
            throw new NotImplementedException();
        }

        protected void AddBuiltInImport(Enum moduleId, IEnumerable<Module> builtInModules, Folder folder, int subFolderCount)
        {
            var importDeclarationAttribute = moduleId.GetModuleImportDeclarationAttribute();

            if (builtInModules.Count() > 0)
            {
                var module = builtInModules.Single(m => m.Name == importDeclarationAttribute.Module);
                ModuleImportDeclaration declaration;

                declaration = module.CreateImportDeclaration(folder, subFolderCount);

                importDeclarations.Add(declaration);
            }
        }

        public IEnumerable<ModuleImportDeclaration> GetDeclarations()
        {
            return importDeclarations;
        }

        public void ClearDeclarations()
        {
            importDeclarations.Clear();
        }
    }
}
