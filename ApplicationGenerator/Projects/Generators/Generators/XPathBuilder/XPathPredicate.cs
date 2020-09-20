using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodePlex.XPathParser;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace AbstraX.XPathBuilder
{
    public class XPathPredicate
    {
        public string Left { get; set; }
        public string Right { get; set; }
        public string RightFormatted { get; set; }
        public XPathOperator Operator { get; set; }

        public XPathPredicate(string left, XPathOperator op, string right)
        {
            this.Left = left;
            this.Operator = op;
            this.Right = Regex.Replace(right, @"^'|'$", "");
            this.RightFormatted = right;
        }

        public override string ToString()
        {
            var text = this.Left;

            switch (this.Operator)
            {
                case XPathOperator.Eq:
                    text += "=";
                    break;
                default:

                    // todo

                    Debugger.Break();
                    break;
            }

            text += this.RightFormatted;

            return text;
        }
    }
}
