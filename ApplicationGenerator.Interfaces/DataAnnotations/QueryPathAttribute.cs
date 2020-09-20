using AbstraX.XPathBuilder;
using CodePlex.XPathParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    public class QueryPathAttribute : Attribute
    {
        public string XPathExpression { get; }
        public Queue<IXPathPart> PartQueue { get; }
        public QueryKind QueryPathKind { get; }

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
