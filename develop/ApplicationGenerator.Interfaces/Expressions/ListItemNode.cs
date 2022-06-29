using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX.Expressions
{
    public class ListItemNode : BaseExpressionNode, IValueNode
    {
        public object Value { get; set; }

        public ListItemNode(BaseExpressionNode parentNode) : base(parentNode)
        {
        }

        public ListItemNode(BaseExpressionNode value, BaseExpressionNode parentNode) : base(parentNode)
        {
            this.Value = value;
        }
    }
}
