using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.XPathBuilder
{
    public class XPathNumber : XPathLiteral
    {
        public XPathNumber(string value) : base(value)
        {
        }

        public override string ToString()
        {
            return base.Value.ToString();
        }
    }
}
