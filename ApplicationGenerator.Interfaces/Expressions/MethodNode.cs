using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AbstraX.Expressions
{
    public class MethodNode : BaseExpressionNode
    {
        public MethodInfo Method { get; set; }
        public Dictionary<string, ListItemNode> Parameters { get; set; }

        public MethodNode(MethodInfo methodInfo, BaseExpressionNode parentNode) : base(parentNode)
        {
            this.Method = methodInfo;

            this.Parameters = new Dictionary<string, ListItemNode>();
        }

        public MethodNode(MethodInfo methodInfo)
        {
            this.Method = methodInfo;

            this.Parameters = new Dictionary<string, ListItemNode>();
        }
    }
}
