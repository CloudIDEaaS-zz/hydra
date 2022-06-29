using ApplicationGenerator.AST;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Utils;
using Utils.Core.Queryable;

namespace ApplicationGenerator.Data
{
    public partial class AbstraXProviderDbSet<TEntity>
    {
        private AbstraXProviderQuery query;

        public IQueryable CreateQuery(Expression expression)
        {
            query.Parse(expression);

            return DebugUtils.BreakReturnNull();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            query.Parse(expression);

            return (IQueryable<TElement>) this.AsQueryable<TEntity>();
        }

        public object Execute(Expression expression)
        {
            return DebugUtils.BreakReturnNull();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return DebugUtils.BreakReturnNull();
        }
    }
}