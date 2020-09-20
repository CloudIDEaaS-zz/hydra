using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.QueryPath
{
    public enum QueryPathFunctionKind
    {
        [QueryExpectedCardinality(QueryExpectedCardinality.Single), QueryFunctionCode("{ identityNameVariable }")]
        identity_name,
        [QueryExpectedCardinality(QueryExpectedCardinality.Single), QueryFunctionCode("Last()")]
        last,
        custom,
        [QueryExpectedCardinality(QueryExpectedCardinality.Single), QueryFunctionCode("Single()")]
        single
    }
}
