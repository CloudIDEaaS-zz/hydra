using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX.Expressions
{
    public class PrimitiveNode : BaseExpressionNode, IValueNode
    {
        public object Value { get; set; }

        public PrimitiveNode(object value, BaseExpressionNode parentNode) : base(parentNode)
        {
            this.Value = value;
        }
    }
}
