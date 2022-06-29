using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.ServerInterfaces;
using Utils;

namespace AbstraX.QueryPath
{
    public class QueryPathQueueItem
    {
        private bool assumeSingleOperator;
        public IBase BaseObject { get; }
        public BaseList<QueryPathQueuePredicate> Predicates { get;  }

        public QueryPathQueueItem(IBase baseObject, bool assumeSingleOperator = false)
        {
            this.BaseObject = baseObject;
            this.Predicates = new BaseList<QueryPathQueuePredicate>();
            this.AssumeSingleOperator = assumeSingleOperator;

            this.Predicates.CollectionChanged += Predicates_CollectionChanged;
        }

        private void Predicates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var predicates = e.NewItems;

            foreach (var predicate in predicates.Cast<QueryPathQueuePredicate>())
            {
                var operand = predicate.Operands.Last();

                if (operand is QueryPathQueueFunction pathQueueFunction)
                {
                    if (pathQueueFunction.FunctionKind.GetExpectedCardinality() == QueryExpectedCardinality.Multiple)
                    {
                        assumeSingleOperator = true;
                    }
                }
            }
        }

        public bool AssumeSingleOperator
        {
            set
            {
                assumeSingleOperator = value;

                if (assumeSingleOperator)
                {
                    var predicate = new QueryPathQueuePredicate(new QueryPathQueueFunction(QueryPathFunctionKind.single));

                    this.Predicates.Add(predicate);
                }
            }

            get
            {
                return assumeSingleOperator;
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder(this.BaseObject.Name);

            foreach (var predicate in this.Predicates)
            {
                builder.AppendFormat("[{0}]", predicate.ToString());
            }

            return builder.ToString();
        }
    }
}
