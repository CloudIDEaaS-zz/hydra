// file:	Module.cs
//
// summary:	Implements the module class

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
    /// <summary>   A module. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/21/2020. </remarks>

    [DebuggerDisplay(" { Name } ")]
    public abstract class Module : IModuleOrAssembly
    {
        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        public abstract string Name { get; set; }

        /// <summary>   Gets the attributes. </summary>
        ///
        /// <value> The attributes. </value>

        public string[] Attributes { get; }

        /// <summary>   Gets or sets the file. </summary>
        ///
        /// <value> The file. </value>

        public virtual File File { get; set; }

        /// <summary>   Gets or sets a value indicating whether the referenced by index. </summary>
        ///
        /// <value> True if referenced by index, false if not. </value>

        public bool ReferencedByIndex { get; set; }

        /// <summary>   Gets or sets the base object. </summary>
        ///
        /// <value> The base object. </value>

        public IBase BaseObject { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/21/2020. </remarks>

        public Module()
        {
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/21/2020. </remarks>
        ///
        /// <param name="name">         The name. </param>
        /// <param name="attributes">   A variable-length parameters list containing attributes. </param>

        public Module(string name, params string[] attributes)
        {
            this.Name = name;
            this.Attributes = attributes;
        }

        /// <summary>   Gets dummy code. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/21/2020. </remarks>
        ///
        /// <param name="spaces">   The spaces. </param>
        ///
        /// <returns>   The dummy code. </returns>

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

        /// <summary>   Equality operator. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/21/2020. </remarks>
        ///
        /// <param name="module1">  The first instance to compare. </param>
        /// <param name="module2">  The second instance to compare. </param>
        ///
        /// <returns>   The result of the operation. </returns>

        public static bool operator ==(Module module1, Module module2)
        {
            return Module.Equals(module1, module2);
        }

        /// <summary>   Inequality operator. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/21/2020. </remarks>
        ///
        /// <param name="module1">  The first instance to compare. </param>
        /// <param name="module2">  The second instance to compare. </param>
        ///
        /// <returns>   The result of the operation. </returns>

        public static bool operator !=(Module module1, Module module2)
        {
            return !Module.Equals(module1, module2);
        }

        /// <summary>   Determines whether the specified object is equal to the current object. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/21/2020. </remarks>
        ///
        /// <param name="obj">  The object to compare with the current object. </param>
        ///
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>

        public override bool Equals(object obj)
        {
            return Module.Equals(this, obj as Module);
        }

        /// <summary>   Serves as the default hash function. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/21/2020. </remarks>
        ///
        /// <returns>   A hash code for the current object. </returns>

        public override int GetHashCode()
        {
            var name = this.Name;

            if (this.BaseObject != null)
            {
                name = name.Append(string.Format("[{0}]", this.BaseObject.Name));
            }

            return name.GetHashCode();
        }

        /// <summary>   Tests if two Module objects are considered equal. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/21/2020. </remarks>
        ///
        /// <param name="module1">  Module to be compared. </param>
        /// <param name="module2">  Module to be compared. </param>
        ///
        /// <returns>   True if the objects are considered equal, false if they are not. </returns>

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
