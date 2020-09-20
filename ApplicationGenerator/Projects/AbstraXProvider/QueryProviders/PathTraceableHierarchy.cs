using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
#if SILVERLIGHT
using AbstraX.ClientInterfaces;
#else
using AbstraX.ServerInterfaces;
using System.Web;
#endif
using CodePlex.XPathParser;
using AbstraX.XPathBuilder;
using System.Xml.Linq;
#if !SILVERLIGHT
using AbstraX.QueryCache;
using log4net;
#endif

namespace AbstraX.QueryProviders
{
    /// <summary>
    /// Provides a custom queryable hierarchy that represents parent-child relationships, although the
    /// structure most likely will not be maintained on the server in memory.  The primary and 
    /// foreign key relationshps are specified as metadata (i.e. lambda expressions, xpath, urls, or named paths)
    /// that provide the means for recreation of the object graph from the root down to the 
    /// nodes being queried.  An example usage would be for the server file system.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PathTraceableHierarchy<T> : CachableQueryProvider, IOrderedQueryable<T>, IQueryProvider where T : IBase
    {
        protected Expression expression;

        public PathTraceableHierarchy()
        {
            expression = Expression.Constant(this);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T item in this.QueryResults.AsEnumerable<T>())
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (T item in this.QueryResults.AsEnumerable<T>())
            {
                yield return item;
            }
        }

        protected abstract IQueryable<T> QueryResults { get; }

        public Queue<string> GetSubjects(string whereClausesString)
        {
            Queue<string> queue = new Queue<string>();
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();

            parser.Parse(whereClausesString, builder);

            foreach (var axisElement in builder.AxisElementQueue)
            {
                queue.Enqueue(axisElement.Element);
            }

            return queue;
        }

        protected virtual Queue<XPathAxisElement> ProcessWhere(string whereClausesString)
        {
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();
            
            parser.Parse(whereClausesString, builder);

            return builder.AxisElementQueue;
        }
        
        public Type ElementType
        {
            get 
            {
                return typeof(T);
            }
        }

        public Expression Expression
        {
            get 
            {
                return expression;
            }
        }

        public IQueryProvider Provider
        {
            get 
            {
                return this;
            }
        }

        public abstract IQueryable<TElement> CreateQuery<TElement>(Expression expression);
        public abstract IQueryable CreateQuery(Expression expression);
        public abstract TResult Execute<TResult>(Expression expression);
        public abstract object Execute(Expression expression);
    }
}
