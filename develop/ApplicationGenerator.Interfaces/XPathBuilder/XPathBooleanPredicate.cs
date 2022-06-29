// file:	XPathBuilder\XPathBooleanPredicate.cs
//
// summary:	Implements the path boolean predicate class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodePlex.XPathParser;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace AbstraX.XPathBuilder
{
    /// <summary>   A path boolean predicate. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/11/2020. </remarks>

    public class XPathBooleanPredicate : IXPathPredicate
    {
        /// <summary>   Gets or sets the left. </summary>
        ///
        /// <value> The left. </value>

        public IXPathOperand Left { get; set; }

        /// <summary>   Gets or sets the right. </summary>
        ///
        /// <value> The right. </value>

        public IXPathOperand Right { get; set; }

        /// <summary>   Gets or sets the operator. </summary>
        ///
        /// <value> The operator. </value>

        public XPathOperator Operator { get; set; }
        public string Name => throw new NotImplementedException();

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/11/2020. </remarks>
        ///
        /// <param name="left">     The left. </param>
        /// <param name="op">       The operation. </param>
        /// <param name="right">    The right. </param>

        public XPathBooleanPredicate(IXPathPredicate left, XPathOperator op, IXPathPredicate right)
        {
            this.Left = left;
            this.Operator = op;
            this.Right = right;
        }
        
        public XPathBooleanPredicate(IXPathPredicate left, XPathOperator op, IXPathOperand right)
        {
            this.Left = left;
            this.Operator = op;
            this.Right = right;
        }

        /// <summary>   Gets the text. </summary>
        ///
        /// <value> The text. </value>

        public string Text
        {
            get
            {
                var text = ((XPathPredicate) this.Left).Text;

                switch (this.Operator)
                {
                    case XPathOperator.And:
                        text += "and ";
                        break;
                    case XPathOperator.Unknown:
                        return text;
                    default:

                        // todo

                        Debugger.Break();
                        break;
                }

                text += ((XPathPredicate)this.Right).Text;

                return text;
            }
        }

        /// <summary>   Returns a string that represents the current object. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/11/2020. </remarks>
        ///
        /// <returns>   A string that represents the current object. </returns>

        public override string ToString()
        {
            var text = this.Left.ToString();

            switch (this.Operator)
            {
                case XPathOperator.And:
                    text += "and ";
                    break;
                case XPathOperator.Unknown:
                    return text;
                default:

                    // todo

                    Debugger.Break();
                    break;
            }

            text += this.Right.ToString();

            return text;
        }
    }
}
