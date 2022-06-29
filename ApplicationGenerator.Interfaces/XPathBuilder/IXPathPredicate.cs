using CodePlex.XPathParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.XPathBuilder
{
    public interface IXPathPredicate : IXPathOperand
    {
        IXPathOperand Left { get; set; }
        IXPathOperand Right { get; set; }
        XPathOperator Operator { get; set; }
    }
}
