using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.ServerInterfaces;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using AbstraX;
using AbstraX.AssemblyInterfaces;
using AbstraX.XPathBuilder;
using CodeInterfaces.XPathBuilder;
using AbstraX.Models;

namespace AssemblyProvider.Web.Entities
{
    public abstract class Operation : BaseObject, IOperation, IBaseOperation
    {
        public abstract OperationDirection Direction { get; }
        public abstract IEnumerable<IElement> ChildElements { get; }
        public abstract IQueryable ExecuteWhere(string property, object value);
        public abstract IQueryable ExecuteWhere(System.Linq.Expressions.Expression expression);
        public abstract IQueryable ExecuteWhere(XPathAxisElement element);
        public abstract void ClearPredicates();

        public Operation(BaseObject parent) : base(parent)
        {
        }

        public IQueryable ExecuteWhere(XPathElement element)
        {
            throw new NotImplementedException();
        }

        Facet[] IBase.Facets
        {
            get { throw new NotImplementedException(); }
        }

        BaseType IOperation.ReturnType
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IBase> ChildNodes => throw new NotImplementedException();

        string IBaseOperation.ID { get; set; }
        public string ImageURL { get; }
    }
}
