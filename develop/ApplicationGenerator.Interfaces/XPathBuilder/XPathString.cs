using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.XPathBuilder
{
    public class XPathString : XPathLiteral
    {
        public string StringValue;

        public XPathString(string value) : base(value)
        {
            this.StringValue = value;
        }

        public override string ToString()
        {
            return "\"" + this.StringValue + "\"";
        }
    }
}
