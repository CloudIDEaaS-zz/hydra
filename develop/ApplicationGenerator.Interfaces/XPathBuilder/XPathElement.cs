using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CodePlex.XPathParser;

namespace AbstraX.XPathBuilder
{
    public class XPathElement : IXPathPart
    {
        public string Text { get; set; }
        public List<IXPathPredicate> Predicates { get; set;  }

        public XPathElement(string element)
        {
            this.Text = element;
            this.Predicates = new List<IXPathPredicate>();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("/" + this.Text);

            if (this.Predicates != null && this.Predicates.Count > 0)
            {
                foreach (var predicate in this.Predicates)
                {
                    builder.Append("[");
                    builder.Append(predicate.ToString());
                    builder.Append("]");
                }
            }

            return builder.ToString();
        }

        public static string GetCSharpOperatorString(XPathOperator op)
        {
            switch (op)
            {
                case XPathOperator.Eq:
                    return "==";
            }

            throw new Exception("Unhandled XPathOperator");
        }

        public static string GetOperatorString(XPathOperator op)
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
