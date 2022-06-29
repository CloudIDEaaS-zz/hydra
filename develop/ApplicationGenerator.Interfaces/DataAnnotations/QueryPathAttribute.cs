// file:	DataAnnotations\QueryPathAttribute.cs
//
// summary:	Implements the query path attribute class

using AbstraX.XPathBuilder;
using CodePlex.XPathParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Attribute for query path. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/29/2020. </remarks>

    public class QueryPathAttribute : Attribute
    {
        /// <summary>   Gets the path expression. </summary>
        ///
        /// <value> The x coordinate path expression. </value>

        public string XPathExpression { get; }

        /// <summary>   Gets a queue of parts. </summary>
        ///
        /// <value> A queue of parts. </value>

        public Queue<IXPathPart> PartQueue { get; }

        /// <summary>   Gets the query path kind. </summary>
        ///
        /// <value> The query path kind. </value>

        public QueryKind QueryPathKind { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/29/2020. </remarks>
        ///
        /// <param name="xpathExpression">  The xpath expression. </param>
        /// <param name="kind">             (Optional) The kind. </param>

        public QueryPathAttribute(string xpathExpression, QueryKind kind = QueryKind.None)
        {
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();
            var id = string.Empty;
            var path = xpathExpression;

            this.XPathExpression = xpathExpression;
            this.QueryPathKind = kind;

            parser.Parse(path, builder);

            this.PartQueue = builder.PartQueue;
        }
    }
}
