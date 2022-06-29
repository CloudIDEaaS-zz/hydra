using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.QueryPath
{
    public class QueryFunctionCodeAttribute : Attribute
    {
        public string CodeExpression { get; }

        public QueryFunctionCodeAttribute(string codeExpression)
        {
            this.CodeExpression = codeExpression;
        }
    }
}
