using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.DomainServices.Server;
using AbstraX.ServerInterfaces;
using AbstraX.TypeMappings;
using AbstraX.Contracts;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using log4net;
using AbstraX.QueryCache;
using AbstraX.QueryProviders;
using System.Web;
using System.Threading;
using System.Reflection;

namespace AbstraX
{
    public abstract class AbstraXDomainService : DomainService, IAbstraXProviderService
    {
        public IDomainHostApplication DomainServiceHostApplication { get; set; }
        public abstract byte[] GetImageForFolder(string folderKey);
        public abstract byte[] GetImageForItemType(string itemTypeName);
        public abstract byte[] GetImageForUrl(string url);
        public abstract string GetQueryMethodForID(string id);
        public abstract string GetRootID();
        public abstract IBase GenerateByID(string id);
        public abstract ContainerType GetAllowableContainerTypes(string id);
        public abstract ConstructType GetAllowableConstructTypes(string id);
        public abstract SortedList<float, Contracts.IPipelineStep> InitialPipelineSteps { get; }
        public abstract void ClearCache();

        public override IEnumerable Query(QueryDescription queryDescription, out IEnumerable<ValidationResult> validationErrors, out int totalCount)
        {
            var historyItem = new QueryHistoryItem();

            historyItem.QueryDescription = queryDescription;
            historyItem.MethodName = queryDescription.Method.Name;
            historyItem.DomainType = this.GetType();
            historyItem.ParameterValues = queryDescription.ParameterValues;

            historyItem.ContextTimestamp = HttpContext.Current.Timestamp;
            historyItem.ThreadId = Thread.CurrentThread.ManagedThreadId;

            CachableQueryProvider.HistoryQueue.Enqueue(historyItem);

            var query = base.Query(queryDescription, out validationErrors, out totalCount);

            totalCount = query.Cast<object>().Count();

            historyItem.EndTime = DateTime.Now;
            historyItem.QueryResultsCount = totalCount;

            this.Log.Info(CachableQueryProvider.ProcessingOutput);

            return query;
        }

        public void LogGenerateByID(string id, MethodInfo method)
        {
            var historyItem = new QueryHistoryItem();

            historyItem.MethodName = method.Name;
            historyItem.DomainType = this.GetType();
            historyItem.ParameterValues = new string[1] { id };

            if (HttpContext.Current != null)
            {
                historyItem.ContextTimestamp = HttpContext.Current.Timestamp;
            }
            else
            {
                historyItem.ContextTimestamp = DateTime.Today;
            }

            historyItem.ThreadId = Thread.CurrentThread.ManagedThreadId;

            CachableQueryProvider.HistoryQueue.Enqueue(historyItem);
        }

        public void PostLogGenerateByID()
        {
            var historyItem = CachableQueryProvider.HistoryItem;
            var totalCount = 1;

            historyItem.EndTime = DateTime.Now;
            historyItem.QueryResultsCount = totalCount;

            this.Log.Info(CachableQueryProvider.ProcessingOutput);
        }

        public ILog Log
        {
            get
            {
                return DomainServiceHostApplication.Log;
            }
        }
    }
}
