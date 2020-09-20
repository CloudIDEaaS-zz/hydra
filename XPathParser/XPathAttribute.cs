using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPathParser
{
    public class XPathAttribute : Attribute
    {
        public string XPathExpression { get; private set; }

        public XPathAttribute(string XPathExpression)
        {
            this.XPathExpression = XPathExpression;
        }
    }
}
