using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX.Expressions
{
    public abstract class BaseExpressionNode
    {
        public BaseExpressionNode ParentNode { get; set; }

        public BaseExpressionNode()
        {
        }

        public BaseExpressionNode(BaseExpressionNode parentNode)
        {
            this.ParentNode = parentNode;
        }
    }
}
