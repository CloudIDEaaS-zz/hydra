using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.QueryCache;
#if SILVERLIGHT
using DanielVaughan.Logging;
using System.Collections;
#else
using log4net;
using AbstraX.ServerInterfaces;
using System.Web;
using AbstraX.XPathBuilder;
using System.Threading;
using System.Diagnostics;
#endif

namespace AbstraX.QueryProviders
{
    public abstract class CachableQueryProvider
    {
        protected object lockObject;
#if !SILVERLIGHT
        private static AbstraXQueryCache cache;
#endif
        private static bool logEnabled;
        private static ILog log;
        private static int indent;
        public static bool EnableCaching { get; set; }
        public static Queue<QueryHistoryItem> HistoryQueue { get; private set; }

        public static bool LogEnabled
        {
            get 
            {
                return CachableQueryProvider.logEnabled; 
            }
            
            set 
            {
                CachableQueryProvider.logEnabled = value; 
            }
        }

        static CachableQueryProvider()
        {
#if !SILVERLIGHT
		    cache = new AbstraXQueryCache(); 
#endif        

            HistoryQueue = new Queue<QueryHistoryItem>();
        }

#if SILVERLIGHT
        public static ILog Log
        {
            get
            {
                return CachableQueryProvider.log;
            }

            set
            {
                CachableQueryProvider.log = value;
            }
        }
#else
        public static ILog Log
        {
            get
            {
                return CachableQueryProvider.log;
            }

            set
            {
                CachableQueryProvider.log = value;
            }
        }
        
        public AbstraXQueryCache Cache
        {
            get
            {
                return cache;
            }
        }

        public static void ShutdownCache()
        {
            cache.Dispose();
        }

        public static void ClearCache()
        {
            cache.ClearCurrent();
        }
#endif

        protected static void WriteHistory(int indent, string entry)
        {
#if !SILVERLIGHT
            var stackTrace = new StackTrace(true);
            var lineNumber = stackTrace.GetFrame(1).GetFileLineNumber();
#else
            var lineNumber = -1;
#endif
            HistoryItem.QueryProcessingLog.AppendLine(new string(' ', indent * 2) + entry + "   line: " + lineNumber.ToString());
        }

        protected static void WriteHistory(int indent, string entry, params object[] args)
        {
#if !SILVERLIGHT
            var stackTrace = new StackTrace(true);
            var lineNumber = stackTrace.GetFrame(1).GetFileLineNumber();
#else
            var lineNumber = -1;
#endif
            HistoryItem.QueryProcessingLog.AppendFormat(new string(' ', indent * 2) + entry + "   line: " + lineNumber.ToString() + "\r\n", args);
        }

        protected static void WriteHistory(string entry)
        {
#if !SILVERLIGHT
            var stackTrace = new StackTrace(true);
            var lineNumber = stackTrace.GetFrame(1).GetFileLineNumber();
#else
            var lineNumber = -1;
#endif
            HistoryItem.QueryProcessingLog.AppendLine(entry + "   line: " + lineNumber.ToString());
        }

        protected static void WriteHistory(string entry, params object[] args)
        {
#if !SILVERLIGHT
            var stackTrace = new StackTrace(true);
            var lineNumber = stackTrace.GetFrame(1).GetFileLineNumber();
#else
            var lineNumber = -1;
#endif
            HistoryItem.QueryProcessingLog.AppendFormat(entry + "   line: " + lineNumber.ToString() + "\r\n", args);
        }

        public static string ProcessingOutput
        {
            get
            {
                if (HistoryItem != null)
                {
                    return HistoryItem.QueryProcessingOutput;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public static QueryHistoryItem HistoryItem
        {
            get
            {
#if SILVERLIGHT
                return CachableQueryProvider.HistoryQueue.LastOrDefault();
#else
                if (HttpContext.Current != null)
                {
                    var contextTimestamp = HttpContext.Current.Timestamp;
                    var threadId = Thread.CurrentThread.ManagedThreadId;

                    return CachableQueryProvider.HistoryQueue.Where(i => i.ContextTimestamp == contextTimestamp && i.ThreadId == threadId).LastOrDefault();
                }
                else
                {
                    var contextTimestamp = DateTime.Today;
                    var threadId = Thread.CurrentThread.ManagedThreadId;

                    return CachableQueryProvider.HistoryQueue.Where(i => i.ContextTimestamp == contextTimestamp && i.ThreadId == threadId).LastOrDefault();
                }
#endif
            }
        }

#if SILVERLIGHT
        protected IEnumerable WhereProcessed
        {
            set
            {
            }
        }
#else
        protected Queue<XPathAxisElement> WhereProcessed
        {
            set
            {
                WriteHistory(indent, "Where clause processed of {0} elements.", value.Count);

                HistoryItem.WhereProcessed = new Queue<XPathAxisElement>(value);
            }
        }
#endif
        protected int Indent
        {
            set
            {
                indent = value;
            }
        }

        protected static string WhereClause
        {
            set
            {
                WriteHistory(indent, "Handling where clause value of '{0}'", value);

                HistoryItem.WhereClause = value;
            }
        }

        public CachableQueryProvider()
        {

#if !SILVERLIGHT
            var lockObjectKey = DNSHelper.MakeURL("LockObject");

            if (HttpContext.Current == null)
            {
                lockObject = new object();
            }
            else if (HttpContext.Current.Session[lockObjectKey] == null)
            {
                lockObject = new object();

                HttpContext.Current.Session.Add(lockObjectKey, lockObject);
            }
            else
            {
                lockObject = HttpContext.Current.Session[lockObjectKey];
            }
#else
            lockObject = new object();
#endif
        }

        protected bool InCache(string expression, out IQueryable queryable)
        {
#if !SILVERLIGHT
            return this.Cache.InCache(expression, out queryable);
#else
            queryable = null;
            return false;
#endif
        }
    }
}
