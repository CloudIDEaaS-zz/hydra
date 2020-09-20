using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if SILVERLIGHT
using AbstraX.ClientInterfaces;
#else
using AbstraX.ServerInterfaces;
#endif

namespace AbstraX.QueryProviders
{
    /// <summary>
    ///  Traced based on XPath expressions
    /// </summary>
    public abstract class XPathTraceableHierarchy<T> : PathTraceableHierarchy<T> where T : IBase
    {
        protected override IQueryable<T> QueryResults
        {
            get { throw new NotImplementedException(); }
        }

        public override IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public override IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public override TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public override object Execute(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
