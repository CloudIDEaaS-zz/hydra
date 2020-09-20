using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using Utils;

namespace AbstraX
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ModuleImportDeclarationAttribute : Attribute
    {
        public string Module { get; }
        public string ImportPath { get; }

        public ModuleImportDeclarationAttribute(string module, string importPath)
        {
            this.Module = module;
            this.ImportPath = importPath;
        }

        public ModuleImportDeclarationAttribute(string importPath)
        {
            this.ImportPath = importPath;
        }
    }

    [DebuggerDisplay(" { DeclarationCode } ")]
    public class ModuleImportDeclaration
    {
        public string ImportPath { get; protected set; }
        public string[] ModuleNames { get; protected set; }
        public IModuleOrAssembly ModuleOrAssembly { get; protected set; }

        public ModuleImportDeclaration(string importPath, params string[] moduleNames)
        {
            this.ImportPath = importPath;
            this.ModuleNames = moduleNames;
        }

        public ModuleImportDeclaration(string importPath, IModuleOrAssembly moduleOrAssembly, string moduleName)
        {
            this.ImportPath = importPath;
            this.ModuleNames = new List<string> { moduleName }.ToArray();
            this.ModuleOrAssembly = moduleOrAssembly;
        }

        public string DeclarationCode
        {
            get
            {
                if (this.ModuleNames.IsEmpty())
                {
                    return string.Format("import \"{0}\";", this.ImportPath);
                }
                else
                {
                    return string.Format("import {{ {0} }} from \"{1}\";", this.ModuleNames.ToCommaDelimitedList(), this.ImportPath);
                }
            }
        }

        public override string ToString()
        {
            return this.DeclarationCode;
        }
    }
}
