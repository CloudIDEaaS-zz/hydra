using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.DataAnnotations;
using AbstraX.ServerInterfaces;
using Utils;

namespace AbstraX
{
    public class EntityPropertiesEventArgs
    {
        public IEnumerable EntityProperties { get; set; }
        public QueryInfo QueryInfo { get; }

        public EntityPropertiesEventArgs(QueryInfo queryInfo)
        {
            this.QueryInfo = queryInfo;
        }
    }

    public class QueryInfo
    {
        public string ServiceControllerMethodName { get; }
        public string QueryExpression { get; }
        public IBase SourceEntity { get; set; }
        public QueryKind QueryKind { get; internal set; }
        public static event EventHandlerT<EntityPropertiesEventArgs> GetEntityPropertiesForQuery;

        public QueryInfo(string serviceControllerMethodName, string queryExpresion)
        {
            this.ServiceControllerMethodName = serviceControllerMethodName;
            this.QueryExpression = queryExpresion;
        }

        public IEnumerable<T> GetEntityProperties<T>()
        {
            var eventArgs = new EntityPropertiesEventArgs(this);
            QueryInfo.GetEntityPropertiesForQuery.Raise(this, eventArgs);

            return eventArgs.EntityProperties.Cast<T>();
        }

        public string ClientProviderMethodName
        {
            get
            {
                return this.ServiceControllerMethodName.ToCamelCase();
            }
        }

        public IElement QueryEntity
        {
            get
            {
                switch (this.QueryKind)
                {
                    case QueryKind.LoadParentReference:
                        return (IElement) this.SourceEntity.Parent;
                    default:
                        DebugUtils.Break();
                        return null;
                }
            }
        }

        public string QueryEntityName
        {
            get
            {
                if (this.ReturnsSet)
                {
                    return this.QueryEntity.Name.Pluralize();
                }
                else
                {
                    return this.QueryEntity.Name;
                }
            }
        }

        public bool ReturnsSet
        {
            get
            {
                switch (this.QueryKind)
                {
                    case QueryKind.LoadParentReference:
                        return false;
                    default:
                        DebugUtils.Break();
                        return false;
                }
            }
        }

        public string ServiceControllerMethodReturnType
        {
            get
            {
                if (this.ReturnsSet)
                {
                    return $"{ this.QueryEntityName }[]";
                }
                else
                {
                    return $"{ this.QueryEntityName }";
                }
            }
        }

        public string ClientProviderApiMethodReturnType
        {
            get
            {
                if (this.ReturnsSet)
                {
                    return $"{ this.QueryEntityName }[]";
                }
                else
                {
                    return $"{ this.QueryEntityName }";
                }
            }
        }
    }
}
