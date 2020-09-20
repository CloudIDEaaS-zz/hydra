using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CodePlex.XPathParser;

namespace CodeInterfaces.XPathBuilder
{
    [DebuggerDisplay("{ DebugInfo }")]
    public class XPathAxisElement
    {
        public string Element { get; set; }
        public List<XPathPredicate> Predicates { get; set;  }

        public XPathAxisElement(string element)
        {
            this.Element = element;
            this.Predicates = new List<XPathPredicate>();
        }

        public string DebugInfo
        {
            get
            {
                var debugInfo = new StringBuilder();

                debugInfo.Append("/" + this.Element);

                if (this.Predicates != null && this.Predicates.Count > 0)
                {
                    debugInfo.Append("[");

                    foreach (var predicate in this.Predicates)
                    {
                        debugInfo.Append("@" + predicate.Left);
                        debugInfo.Append(GetOperatorString(predicate.Operator));
                        debugInfo.Append("'" + predicate.Right + "'");
                    }

                    debugInfo.Append("]");
                }

                return debugInfo.ToString();
            }
        }

        public string GetOperatorString(XPathOperator op)
        {
            switch (op)
            {
                case XPathOperator.Eq:
                    return "=";
            }

            throw new Exception("Unhandled XPathOperator");
        }
    }
}
