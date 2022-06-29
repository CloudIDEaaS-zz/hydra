using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using CodeInterfaces.XPathBuilder;

namespace CodeInterfaces
{
    public interface IPathQueryable
    {
        IQueryable ExecuteWhere(string property, object value);
        IQueryable ExecuteWhere(Expression expression);
        IQueryable ExecuteWhere(XPathAxisElement element);
        void ClearPredicates();
    }
}
