using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.XPathBuilder
{
    public class XPathFunction : IXPathOperand
    {
        public string Name { get;  }
        public IList<string> Args { get; }
        public string Text { get; set; }

        public XPathFunction(string name, IList<string> args)
        {
            this.Name = name;
            this.Args = args;
        }

        public override string ToString()
        {
            return this.Text;
        }
    }
}
