using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace AbstraX.Expressions
{
    public class BinaryExpressionNode : BaseExpressionNode
    {
        public ExpressionType ExpressionType { get; set; }
        public ListItemNode Left { get; set; }
        public ListItemNode Right { get; set; }

        public BinaryExpressionNode(ExpressionType expressionType, BaseExpressionNode parentNode) : base(parentNode)
        {
            this.ExpressionType = expressionType;
        }

        public BinaryExpressionNode(ExpressionType expressionType)
        {
            this.ExpressionType = expressionType;
        }
    }
}
