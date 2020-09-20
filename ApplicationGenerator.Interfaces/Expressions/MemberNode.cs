using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AbstraX.Expressions
{
    public class MemberNode : BaseExpressionNode
    {
        public MemberInfo Member { get; set; }

        public MemberNode(MemberInfo m, BaseExpressionNode parentNode) : base(parentNode)
        {
            this.Member = m;
        }
    }
}
