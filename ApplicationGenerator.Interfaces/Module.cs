using AbstraX.FolderStructure;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    [DebuggerDisplay(" { Name } ")]
    public abstract class Module : IModuleOrAssembly
    {
        public abstract string Name { get; set; }
        public string[] Attributes { get; }
        public virtual File File { get; set; }
        public bool ReferencedByIndex { get; set; }
        public IBase BaseObject { get; set; }

        public Module()
        {
        }

        public Module(string name, params string[] attributes)
        {
            this.Name = name;
            this.Attributes = attributes;
        }

        public string GetDummyCode(int spaces)
        {
            var builder = new StringBuilder();

            foreach (var attribute in Attributes)
            {
                builder.AppendLineFormatSpaceIndent(spaces, "@{0}()", attribute);
            }

            builder.AppendLineFormatSpaceIndent(spaces, "export class {0}", this.Name);
            builder.AppendLineSpaceIndent(spaces, "{");
            builder.AppendLineSpaceIndent(spaces + 2, "// this is dummy code");
            builder.AppendLineSpaceIndent(spaces, "}");

            return builder.ToString();
        }

        public static bool operator ==(Module module1, Module module2)
        {
            return Module.Equals(module1, module2);
        }

        public static bool operator !=(Module module1, Module module2)
        {
            return !Module.Equals(module1, module2);
        }

        public override bool Equals(object obj)
        {
            return Module.Equals(this, obj as Module);
        }

        public override int GetHashCode()
        {
            var name = this.Name;

            if (this.BaseObject != null)
            {
                name = name.Append(string.Format("[{0}]", this.BaseObject.Name));
            }

            return name.GetHashCode();
        }

        private static bool Equals(Module module1, Module module2)
        {
            bool equals;

            if (CompareExtensions.CheckNullEquality(module1, module2, out equals))
            {
                return equals;
            }
            else 
            {
                if (CompareExtensions.CheckNullEquality(module1.BaseObject, module2.BaseObject, out equals))
                {
                    return equals;
                }
                else
                {
                    return module1.Name == module2.Name && module1.BaseObject.Name == module2.BaseObject.Name;
                }
            }
        }
    }
}
