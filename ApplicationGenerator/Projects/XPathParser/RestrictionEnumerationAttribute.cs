using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPathParser
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RestrictionEnumerationAttribute : Attribute
    {
        public string[] Values { get; private set; }
        public string BaseType { get; private set; }

        public RestrictionEnumerationAttribute(string baseType, params string[] values)
        {
            this.Values = values;
            this.BaseType = baseType;
        }
    }
}
