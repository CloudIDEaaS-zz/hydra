using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPathParser
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RestrictionPatternAttribute : Attribute
    {
        public string Pattern { get; private set; }
        public string BaseType { get; private set; }

        public RestrictionPatternAttribute(string pattern, string baseType)
        {
            this.Pattern = pattern;
            this.BaseType = baseType;
        }
    }
}
