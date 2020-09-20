using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodePlex.XPathParser;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace AbstraX.QueryPath
{
    public class QueryPathQueuePredicate
    {
        public IQueryPathOperand Left { get; set; }
        public IQueryPathOperand Right { get; set; }
        public XPathOperator Operator { get; set; }

        public QueryPathQueuePredicate(IQueryPathOperand left)
        {
            this.Left = left;
            this.Operator = XPathOperator.Unknown;
        }

        public QueryPathQueuePredicate(IQueryPathOperand left, XPathOperator op, IQueryPathOperand right)
        {
            this.Left = left;
            this.Operator = op;
            this.Right = right;
        }

        public IEnumerable<IQueryPathOperand> Operands
        {
            get
            {
                yield return this.Left;
                yield return this.Right;
            }
        }

        public override string ToString()
        {
            var text = this.Left.ToString();

            switch (this.Operator)
            {
                case XPathOperator.Eq:
                    text += "==";
                    break;
                case XPathOperator.Unknown:
                    return text;
                default:

                    // todo

                    Debugger.Break();
                    break;
            }

            text += this.Right;

            return text;
        }
    }
}
