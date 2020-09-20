using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.ServerInterfaces;

namespace AbstraX.QueryPath
{
    public class QueryPathQueueAttribute : IQueryPathOperand
    {
        public IAttribute Attribute { get; }

        public QueryPathQueueAttribute(IAttribute attribute)
        {
            this.Attribute = attribute;
        }

        public override string ToString()
        {
            return $"@{ this.Attribute.Name }";
        }
    }
}
