// file:	QueryInfoProxy.cs
//
// summary:	Implements the query information proxy class

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   A query information proxy. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/8/2020. </remarks>

    [TypeProxy(typeof(QueryInfo))]
    public class QueryInfoProxy : QueryInfo, IRuntimeProxy
    {
        /// <summary>   Gets or sets the service controller method name value. </summary>
        ///
        /// <value> The service controller method name value. </value>

        [JsonProperty(PropertyName = "ServiceControllerMethodName")]
        public string ServiceControllerMethodNameValue { get; set; }


        /// <summary>   Gets or sets the query expression value. </summary>
        ///
        /// <value> The query expression value. </value>

        [JsonProperty(PropertyName = "QueryExpression")]
        public string QueryExpressionValue { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/9/2020. </remarks>
        ///
        /// <param name="queryInfo">    Information describing the query. </param>

        public QueryInfoProxy(QueryInfo queryInfo) : base(queryInfo.ServiceControllerMethodName, queryInfo.QueryExpression)
        {
            this.ServiceControllerMethodNameValue = queryInfo.ServiceControllerMethodName;
            this.QueryExpressionValue = queryInfo.QueryExpression;
        }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/8/2020. </remarks>

        public QueryInfoProxy(dynamic queryInfoJsonObject) : base((string) queryInfoJsonObject.ServiceControllerMethodName, (string)queryInfoJsonObject.QueryExpression)
        {
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/8/2020. </remarks>
        ///
        /// <param name="serviceControllerMethodName">  Name of the service controller method. </param>
        /// <param name="queryExpresion">               The query expresion. </param>

        public QueryInfoProxy(string serviceControllerMethodName, string queryExpresion) : base(serviceControllerMethodName, queryExpresion)
        {
        }

        /// <summary>   Call method. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/8/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="identifier">   The identifier. </param>
        /// <param name="methodName">   Name of the method. </param>
        /// <param name="args">         A variable-length parameters list containing arguments. </param>
        ///
        /// <returns>   A T. </returns>

        public T CallMethod<T>(string identifier, string methodName, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Property get. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/8/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="identifier">   The identifier. </param>
        /// <param name="propertyName"> Name of the property. </param>
        ///
        /// <returns>   A T. </returns>

        public T PropertyGet<T>(string identifier, string propertyName)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Property set. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/8/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="identifier">   The identifier. </param>
        /// <param name="propertyName"> Name of the property. </param>
        /// <param name="value">        The value. </param>

        public void PropertySet<T>(string identifier, string propertyName, T value)
        {
            throw new NotImplementedException();
        }
    }
}
