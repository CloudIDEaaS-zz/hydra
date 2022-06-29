using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodePlex.XPathParser;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Utils;

namespace AbstraX.XPathBuilder
{
    public class XPathVariable : IXPathPart
    {
        public string Prefix { get; private set; }
        public string Name { get; private set; }

        public XPathVariable(string prefix, string name)
        {
            this.Prefix = prefix;
            this.Name = name;
        }

        public override string ToString()
        {
            if (this.Prefix.IsNullOrEmpty())
            {
                var text = string.Format("${0}", this.Name);

                return text;
            }
            else
            {
                var text = string.Format("${0}:{1}", this.Prefix, this.Name);

                return text;
            }
        }
    }
}
