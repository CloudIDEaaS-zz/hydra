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
        public IXPathOperand Left { get; set; }
        public IXPathOperand Right { get; set; }
        public string RightFormatted { get; set; }
        public XPathOperator Operator { get; set; }

        public XPathPredicate(IXPathOperand left)
        {
            this.Left = left;
            this.Operator = XPathOperator.Unknown;
        }

        public XPathPredicate(IXPathOperand left, XPathOperator op, IXPathOperand right)
        {
            this.Left = left;
            this.Operator = op;
            this.Right = right;
            this.RightFormatted = right.ToString();
        }

        public string Text
        {
            get
            {
                var text = this.Left.Name;

                switch (this.Operator)
                {
                    case XPathOperator.Eq:
                        text += "=";
                        break;
                    case XPathOperator.Unknown:
                        return text;
                    default:

                        // todo

                        Debugger.Break();
                        break;
                }

                text += this.RightFormatted;

                return text;
            }
        }

        public override string ToString()
        {
            var text = this.Left.ToString();

            switch (this.Operator)
            {
                case XPathOperator.Eq:
                    text += "=";
                    break;
                case XPathOperator.Unknown:
                    return text;
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
