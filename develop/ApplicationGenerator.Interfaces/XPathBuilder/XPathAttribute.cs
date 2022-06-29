using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.XPathBuilder
{
    public class XPathAttribute : IXPathOperand
    {
        public string Name { get; }

        public XPathAttribute(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return $"@{ this.Name }";
        }
    }
}
