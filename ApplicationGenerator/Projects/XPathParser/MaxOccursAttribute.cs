using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPathParser
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxOccursAttribute : Attribute
    {
        public int MaxOccurs { get; private set; }

        public MaxOccursAttribute(int maxOccurs)
        {
            this.MaxOccurs = maxOccurs;
        }
    }
}
