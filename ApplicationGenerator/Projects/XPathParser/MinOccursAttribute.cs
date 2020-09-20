using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPathParser
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MinOccursAttribute : Attribute
    {
        public int MinOccurs { get; private set; }

        public MinOccursAttribute(int minOccurs)
        {
            this.MinOccurs = minOccurs;
        }
    }
}
