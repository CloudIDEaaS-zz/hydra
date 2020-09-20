using System;
using System.Net;
using AbstraX.ServerInterfaces;
using System.Reflection;
using AbstraX.XPathBuilder;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using AbstraX;
using AbstraX.AssemblyInterfaces;
using System.Collections.Generic;
using CodeInterfaces.XPathBuilder;
using AbstraX.Models;

namespace AssemblyProvider.Web.Entities
{
    public class OperatorOperation : Operation, IOperatorOperation
    {
        private ConstructorInfo constructor;
        public event ChildrenLoadedHandler ChildrenLoaded;
        private IBase parent;
        private string name;
        private string queryWhereProperty;
        private object queryWhereValue;
        private float childOrdinal;

        public override string ID { get; protected set; }
        public override Modifiers Modifiers { get; }

        public OperatorOperation(ConstructorInfo constructor, BaseObject parent) : base(parent)
        {
            this.constructor = constructor;
            this.parent = parent;
            this.childOrdinal = 5;

            this.ID = this.MakeID("Constructor='" + constructor.Name + "'");
        }

        public override float ChildOrdinal
        {
            get
            {
                return childOrdinal;
            }
        }

        public override string DebugInfo
        {
            get
            {
                return this.GetDebugInfo();
            }
        }

        public ConstructorInfo Constructor
        {
            get
            {
                return constructor;
            }

            set
            {
                constructor = value;
            }
        }

        public override OperationDirection Direction
        {
            get 
            {
                return OperationDirection.Incoming;
            }
        }

        public override System.Collections.Generic.IEnumerable<IElement> ChildElements
        {
            get { throw new NotImplementedException(); }
        }

        public override string ParentID
        {
            get
            {
                return parent.ID;
            }
        }

        public override string Name
        {
            get 
            {
                if (name == null)
                {
                    name = this.GenerateName();
                }

                return name;
            }
        }

        public override string DesignComments
        {
            get
            {
                return string.Format("Type: {0}, Kind {1}, ID:'{2}'", this.GetType().Name, this.Kind, this.ID);
            }
        }

        public override string Documentation
        {
            get
            {
                return string.Empty;
            }
        }

        public override bool HasDocumentation
        {
            get
            {
                return false;
            }
        }

        public override string DocumentationSummary
        {
            get
            {
                return string.Empty;
            }
        }

        public override IBase Parent
        {
            get
            {
                return parent;
            }
        }

        public override string FolderKeyPair { get; set; }

        public override IQueryable ExecuteWhere(string property, object value)
        {
            if (value is XPathAxisElement && (property == "ID" || property == "ParentID"))
            {
                var predicate = ((XPathAxisElement)value).Predicates.First();

                queryWhereProperty = predicate.Left;
                queryWhereValue = predicate.Right;
            }
            else
            {
                Debugger.Break();

                queryWhereProperty = property;
                queryWhereValue = value;
            }

            return this.ChildElements.AsQueryable();
        }

        public override IQueryable ExecuteWhere(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public override IQueryable ExecuteWhere(XPathAxisElement element)
        {
            throw new NotImplementedException();
        }

        public override IRoot Root
        {
            get
            {
                IBase baseObject = this;

                while (baseObject != null)
                {
                    baseObject = baseObject.Parent;

                    if (baseObject is IRoot)
                    {
                        return (IRoot)baseObject;
                    }
                }

                return null;
            }
        }

        public override void ClearPredicates()
        {
            queryWhereProperty = null;
            queryWhereValue = null;
        }

        public bool IsConstructor
        {
            get 
            {
                return true;
            }
        }

        public override DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.NotApplicable;
            }
        }

        public override bool HasChildren
        {
            get 
            {
                return false;
            }
        }

        public override Facet[] Facets
        {
            get
            {
                return null;
            }
        }

        public BaseType ReturnType
        {
            get { throw new NotImplementedException(); }
        }

        public List<AssemblyType> OperationTypes
        {
            get { throw new NotImplementedException(); }
        }
    }
}
