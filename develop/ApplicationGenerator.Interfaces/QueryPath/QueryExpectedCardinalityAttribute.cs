using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.QueryPath
{
    public enum QueryExpectedCardinality
    {
        Unknown,
        Single,
        Multiple
    }

    public class QueryExpectedCardinalityAttribute : Attribute
    {
        public QueryExpectedCardinality Cardinality { get; }

        public QueryExpectedCardinalityAttribute(QueryExpectedCardinality cardinality)
        {
            this.Cardinality = cardinality;
        }
    }
}
