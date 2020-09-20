using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidationHandlerAttribute : Attribute
    {
        public Type ValidationType { get; private set; }
        public string[] Imports { get; private set; }

        public ValidationHandlerAttribute(Type validationType, params string[] imports)
        {
            this.ValidationType = validationType;
            this.Imports = imports;
        }
    }
}
