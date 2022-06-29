using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterfaces
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class AssemblyVersionVariantAttribute : Attribute
    {
        public string Variant { get; }

        public AssemblyVersionVariantAttribute(string variant)
        {
            this.Variant = variant;
        }
    }
}
