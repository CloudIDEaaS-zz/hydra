using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !SILVERLIGHT
using System.ServiceModel.DomainServices.Server;
using System.ComponentModel.DataAnnotations;
#endif
using AbstraX.XPathBuilder;
using System.Diagnostics;

namespace AbstraX.QueryCache
{
    [DebuggerDisplay(" { QueryProcessingOutput } ")]
    public class QueryHistoryItem
    {
        private DateTime endTime;
        private Type domainType;
        private string methodName;
        private object[] parameterValues;
        private Exception exception;
        private int queryResultsCount;
        private DateTime contextTimestamp;
        private int threadId;
        public DateTime StartTime { get; private set; }
#if !SILVERLIGHT
        public QueryDescription QueryDescription { get; set; }
        public List<ValidationResult> ValidationErrors { get; set; }
#endif
        public StringBuilder QueryProcessingLog { get; private set; }
        public string WhereClause { get; set; }
        public Queue<XPathAxisElement> WhereProcessed { get; set; }

        public QueryHistoryItem()
        {
            QueryProcessingLog = new StringBuilder();
            StartTime = DateTime.Now;

            QueryProcessingLog.AppendFormat("Starting query at {0}\r\n", StartTime);
        }

        public DateTime ContextTimestamp
        {
            get
            {
                return contextTimestamp;
            }

            set
            {
                contextTimestamp = value;

                QueryProcessingLog.AppendFormat("Context timestamp at {0:hh:mm:ss.FFFFF}\r\n", contextTimestamp);
            }
        }

        public int ThreadId
        {
            get
            {
                return threadId;
            }

            set
            {
                threadId = value;

                QueryProcessingLog.AppendFormat("ThreadId={0}\r\n", threadId);
            }
        }

        public Exception Exception
        {
            get
            {
                return exception;
            }

            set
            {
                exception = value;

                QueryProcessingLog.AppendFormat("\r\nException thrown:\r\n{0}\r\n\r\nStackTrace:\r\n\r\n{1}\r\n\r\n", exception.Message, exception.StackTrace);
            }
        }

        public Type DomainType 
        {
            get
            {
                return domainType;
            }

            set
            {
                domainType = value;

                QueryProcessingLog.AppendFormat("From domain '{0}'\r\n", domainType.Name);
            }
        }

        public string MethodName
        {
            get
            {
                return methodName;
            }

            set
            {
                methodName = value;

                QueryProcessingLog.AppendFormat("Method name '{0}'\r\n", methodName);
            }
        }

        public int QueryResultsCount
        {
            get
            {
                return queryResultsCount;
            }

            set
            {
                queryResultsCount = value;

                QueryProcessingLog.AppendFormat("Query result count: {0}\r\n", queryResultsCount);
            }
        }

        public object[] ParameterValues
        {
            get
            {
                return parameterValues;
            }

            set
            {
                var parmIndex = 0;

                parameterValues = value;

                foreach (var val in parameterValues)
                {
                    QueryProcessingLog.AppendFormat("  Parameter[{0}]={1} \r\n", parmIndex, val is string ? "'" + val.ToString() + "'": val.ToString());

                    parmIndex++;
                }
            }
        }

        public string QueryProcessingOutput
        {
            get
            {
                return QueryProcessingLog.ToString();
            }
        }

        public DateTime EndTime
        {
            get
            {
                return endTime;
            }

            set
            {
                endTime = value;

                QueryProcessingLog.AppendFormat("Query finished at {0}\r\n", endTime);
                QueryProcessingLog.AppendFormat("Total processing time at {0} milliseconds\r\n", (endTime - StartTime).Milliseconds);
            }
        }
    }
}
