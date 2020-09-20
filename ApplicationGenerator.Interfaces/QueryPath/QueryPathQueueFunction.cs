using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.ServerInterfaces;

namespace AbstraX.QueryPath
{
    public class QueryPathQueueFunction : IQueryPathOperand
    {
        public QueryPathFunctionKind FunctionKind { get; }
        public IList<string> Args { get; }

        public QueryPathQueueFunction(QueryPathFunctionKind functionKind, IList<string> args = null)
        {
            this.FunctionKind = functionKind;
            this.Args = args;
        }

        public override string ToString()
        {
            var result = this.FunctionKind.ToString() + '(';

            if (this.Args != null)
            {
                for (var i = 0; i < this.Args.Count; i++)
                {
                    if (i != 0)
                    {
                        result += ',';
                    }

                    result += this.Args[i];
                }
            }

            result += ')';

            return result;
        }
    }
}
