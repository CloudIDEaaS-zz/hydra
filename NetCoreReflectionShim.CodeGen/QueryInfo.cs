using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    public class QueryInfo
    { 
        public string ServiceControllerMethodName { get; set; }
        public string QueryExpression { get; set; }
        public int QueryKind { get; set; }
        public string ClientProviderMethodName { get; set; }
        public bool ReturnsSet { get; set; }
        public string ServiceControllerMethodReturnType { get; set; }
        public string ClientProviderApiMethodReturnType { get; set; }
    }
}
