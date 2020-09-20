using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.ServerInterfaces;
using AbstraX;
using System.ComponentModel.DataAnnotations;
using Reflection = System.Reflection;
using E = System.Linq.Expressions;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using AbstraX.XPathBuilder;
using System.Diagnostics;
using AbstraX.TypeMappings;
using AbstraX.AssemblyInterfaces;
using CodeInterfaces.XPathBuilder;
using AbstraX.Models;

namespace AssemblyProvider.Web.Entities
{
    public class Assembly : BaseObject, IElement, IAssembly
    {
        private string assemblyFile;
        private Reflection.Assembly assembly;
        private string queryWhereProperty;
        private object queryWhereValue;
        private Func<Type, bool> typesFilter;

        public Assembly(Reflection.Assembly assembly, string file, Func<Type, bool> typesFilter = null)
        {
            this.assembly = assembly;
            this.assemblyFile = file;
            this.typesFilter = typesFilter;
        }

        public Assembly(Reflection.Assembly assembly, Func<Type, bool> typesFilter = null)
        {
            this.assembly = assembly;
            this.typesFilter = typesFilter;
        }

        public override float ChildOrdinal
        {
            get
            {
                return 0;
            }
        }

        public override string DebugInfo
        {
            get
            {
                return this.GetDebugInfo();
            }
        }

        public override string ID
        {
            get 
            {
                if (assemblyFile != null)
                {
                    return this.MakeID("AssemblyFile='" + this.assemblyFile + "'");
                }
                else
                {
                    return this.MakeID("Assembly='" + this.assembly.FullName + "'");
                }
            }

            protected set
            {
            }
        }

        public override string FolderKeyPair { get; set; }

        public override string ParentID
        {
            get
            {
                return null;
            }
        }

        public IEnumerable<IAttribute> Attributes
        {
            get 
            {
                return null;
            }
        }

        [Association("Assembly_Types", "ID", "ParentID")]
        public List<AssemblyType> Types
        {
            get
            {
                var types = new List<AssemblyType>();

                try
                {
                    if (typesFilter != null)
                    {
                        foreach (System.Type type in assembly.GetTypes().Where(t => typesFilter(t))) 
                        {
                            types.Add(new AssemblyType(type, this));
                        }
                    }
                    else if (queryWhereProperty != null && queryWhereProperty == "Type")
                    {
                        var type = assembly.GetType((string)queryWhereValue);

                        Debug.Assert(type != null);

                        types.Add(new AssemblyType(type, this));
                    }
                    else
                    {
                        foreach (System.Type type in assembly.GetTypes().Where(t => t.IsPublic))
                        {
                            types.Add(new AssemblyType(type, this));
                        }
                    }
                }
                catch
                {
                }

                return types;
            }
        }

        public IEnumerable<IElement> ChildElements
        {
            get 
            {
                var elements = this.Types.AsQueryable().Cast<IElement>().Select(e => e);

                return elements;
            }
        }

        public IEnumerable<IOperation> Operations
        {
            get 
            {
                return null;
            }
        }

        public BaseType DataType
        {
            get 
            {
                return null;
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

        public string ImageURL
        {
            get 
            {
                return string.Empty;
            }
        }

        public override string Name
        {
            get
            {
                return assembly.FullName;
            }
        }

        public override IBase Parent
        {
            get 
            {
                return null;
            }
        }

        public bool IsContainer
        {
            get 
            {
                return true;
            }
        }

        public IQueryable ExecuteWhere(string property, object value)
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

        public IQueryable ExecuteWhere(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable ExecuteWhere(XPathAxisElement element)
        {
            if (element.Predicates.Count > 0)
            {
                var predicate = element.Predicates.First();

                queryWhereProperty = predicate.Left;
                queryWhereValue = predicate.Right;
            }

            return this.ChildElements.AsQueryable();
        }

        public void ClearPredicates()
        {
            queryWhereProperty = null;
            queryWhereValue = null;
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
                return assembly.GetTypes().Any();
            }
        }

        IEnumerable<IAssemblyType> IAssembly.Types
        {
            get
            {
                foreach (var type in this.Types)
                {
                    yield return type;
                }
            }
        }

        public override Facet[] Facets
        {
            get
            {
                return null;
            }
        }

        public ContainerType AllowableContainerTypes
        {
            get { throw new NotImplementedException(); }
        }

        public ConstructType AllowableConstructTypes
        {
            get { throw new NotImplementedException(); }
        }

        public ContainerType DefaultContainerType
        {
            get 
            {
                return ContainerType.NotSet;
            }
        }

        public ConstructType DefaultConstructType
        {
            get 
            {
                return ConstructType.NotSet;
            }
        }

        public IQueryable ExecuteWhere(XPathElement element)
        {
            throw new NotImplementedException();
        }

        public override Modifiers Modifiers
        {
            get
            {
                return Modifiers.Unknown;
            }
        }

        public IEnumerable<IBase> ChildNodes => throw new NotImplementedException();
    }
}
 
