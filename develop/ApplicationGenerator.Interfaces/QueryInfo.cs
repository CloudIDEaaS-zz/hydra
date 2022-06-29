// file:	QueryInfo.cs
//
// summary:	Implements the query information class

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.DataAnnotations;
#if !NET_CORE
using AbstraX.ServerInterfaces;
#endif
using Utils;

namespace AbstraX
{
    /// <summary>   Additional information for entity properties events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>

    public class EntityPropertiesEventArgs
    {
        /// <summary>   Gets or sets the entity properties. </summary>
        ///
        /// <value> The entity properties. </value>

        public IEnumerable EntityProperties { get; set; }

        /// <summary>   Gets information describing the query. </summary>
        ///
        /// <value> Information describing the query. </value>

        public QueryInfo QueryInfo { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>
        ///
        /// <param name="queryInfo">    Information describing the query. </param>

        public EntityPropertiesEventArgs(QueryInfo queryInfo)
        {
            this.QueryInfo = queryInfo;
        }
    }

    /// <summary>   Information about the query. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>

    public class QueryInfo
    { 
        /// <summary>   Gets the name of the service controller method. </summary>
        ///
        /// <value> The name of the service controller method. </value>

        public virtual string ServiceControllerMethodName { get; }

        /// <summary>   Gets the query expression. </summary>
        ///
        /// <value> The query expression. </value>

        public string QueryExpression { get; }
#if !NET_CORE

        /// <summary>   Gets or sets source entity. </summary>
        ///
        /// <value> The source entity. </value>

        public IBase SourceEntity { get; set; }
#endif

        /// <summary>   Gets or sets the query kind. </summary>
        ///
        /// <value> The query kind. </value>

        public QueryKind QueryKind { get; internal set; }

        /// <summary>
        /// Event queue for all listeners interested in GetEntityPropertiesForQuery events.
        /// </summary>

        public static event EventHandlerT<EntityPropertiesEventArgs> GetEntityPropertiesForQuery;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>
        ///
        /// <param name="serviceControllerMethodName">  Name of the service controller method. </param>
        /// <param name="queryExpresion">               The query expresion. </param>

        public QueryInfo(string serviceControllerMethodName, string queryExpresion)
        {
            this.ServiceControllerMethodName = serviceControllerMethodName;
            this.QueryExpression = queryExpresion;
        }

        /// <summary>   Gets the entity properties in this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the entity properties in this
        /// collection.
        /// </returns>

        public IEnumerable<T> GetEntityProperties<T>()
        {
            var eventArgs = new EntityPropertiesEventArgs(this);
            QueryInfo.GetEntityPropertiesForQuery.Raise(this, eventArgs);

            return eventArgs.EntityProperties.Cast<T>();
        }

        /// <summary>   Gets the name of the client provider method. </summary>
        ///
        /// <value> The name of the client provider method. </value>

        public string ClientProviderMethodName
        {
            get
            {
                return this.ServiceControllerMethodName.ToCamelCase();
            }
        }

#if !NET_CORE

        /// <summary>   Gets the query entity. </summary>
        ///
        /// <value> The query entity. </value>

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

        /// <summary>   Gets the name of the query entity. </summary>
        ///
        /// <value> The name of the query entity. </value>

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
#endif

        /// <summary>   Gets a value indicating whether the returns set. </summary>
        ///
        /// <value> True if returns set, false if not. </value>

        public bool ReturnsSet
        {
            get
            {
                switch (this.QueryKind)
                {
                    case QueryKind.LoadParentReference:
                        return false;
                    default:
                        return false;
                }
            }
        }

#if !NET_CORE

        /// <summary>   Gets the type of the service controller method return. </summary>
        ///
        /// <value> The type of the service controller method return. </value>

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

        /// <summary>   Gets the type of the client provider API method return. </summary>
        ///
        /// <value> The type of the client provider API method return. </value>

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
#endif
    }
}
