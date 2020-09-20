using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.Serialization;
using AbstraX;
using AbstraX.TypeMappings;
using AbstraX.ServerInterfaces;
using System.IO;
using Utils;
using AbstraX.XPathBuilder;
using AbstraX.Models.Interfaces;

namespace EntityProvider.Web.Entities
{
    [DebuggerDisplay(" { Name } ")]
    [AbstraxProvider(AbstraXProviderGuids.Entities)]
    public class Model : EntityBase, IElement, IModel, IRoot
    {
        private string queryWhereProperty;
        private object queryWhereValue;
        private string modelName;

        public string Namespace { get; private set; }

        public Model(FileInfo edmxFile, string nameSpace = null)
        {
            modelName = Path.GetFileNameWithoutExtension(edmxFile.FullName);

            this.EdmxDocument = new XmlDocument();
            this.EdmxDocument.Load(edmxFile.OpenText());
            
            this.NamespaceManager = new XmlNamespaceManager(this.EdmxDocument.NameTable);

            this.NamespaceManager.AddNamespace("edmx", "http://schemas.microsoft.com/ado/2009/11/edmx");
            this.NamespaceManager.AddNamespace("s", "http://schemas.microsoft.com/ado/2009/11/edm/ssdl");
            this.NamespaceManager.AddNamespace("e", "http://schemas.microsoft.com/ado/2009/11/edm");

            this.Namespace = nameSpace;
        }

        public IEnumerable<IAttribute> Attributes
        {
            get { return null; }
        }

        public List<IEntityContainer> Containers
        {
            get
            {
                var containers = new List<Entity_Container>();

                try
                {
                    if (queryWhereProperty != null && queryWhereProperty == "Namespace")
                    {
                        var node = this.EdmxDocument.SelectSingleNode("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema/e:EntityContainer/@Name='" + (string)queryWhereValue + "'", this.NamespaceManager);

                        Debug.Assert(node != null);

                        containers.Add(new Entity_Container(this.EdmxDocument, this.NamespaceManager, node.Value, node.SelectSingleNode("parent::*"), this));
                    }
                    else
                    {
                        foreach (XmlNode node in this.EdmxDocument.SelectNodes("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema/e:EntityContainer", this.NamespaceManager))
                        {
                            containers.Add(new Entity_Container(this.EdmxDocument, this.NamespaceManager, node.Attributes["Name"].Value, node, this));
                        }
                    }
                }
                catch
                {
                }

                return containers.ToList<IEntityContainer>();
            }
        }

        public IEnumerable<IElement> ChildElements
        {
            get 
            {
                var elements = this.Containers.AsQueryable().Cast<IElement>().Select(e => e);

                return elements;
            }
        }

        public IEnumerable<IBase> ChildNodes
        {
            get
            {
                var nodes = this.Containers.AsQueryable().Cast<IBase>();

                return nodes;
            }
        }

        public IEnumerable<IOperation> Operations
        {
            get { return null; }
        }

        public override string ID
        {
            get
            {
                return this.MakeID("Model='" + modelName + "'");
            }

            protected set
            {
            }
        }

        public override string Name
        {
            get 
            {
                return modelName;
            }
        }

        public bool IsContainer
        {
            get 
            {
                return true;
            }
        }

        public BaseType DataType
        {
            get
            {
                return null;
            }
        }

        public IQueryable ExecuteWhere(string property, object value)
        {
            if (value is XPathElement && (property == "ID" || property == "ParentID"))
            {
                var predicate = ((XPathElement)value).Predicates.First();

                queryWhereProperty = predicate.Left.ToString();
                queryWhereValue = predicate.Right.ToString();
            }
            else
            {
                System.Diagnostics.Debugger.Break();

                queryWhereProperty = property;
                queryWhereValue = value;
            }

            return this.ChildElements.AsQueryable();
        }

        public IQueryable ExecuteWhere(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable ExecuteWhere(XPathElement element)
        {
            var predicate = element.Predicates.First();

            queryWhereProperty = predicate.Left.ToString();
            queryWhereValue = predicate.Right.ToString();

            return this.ChildElements.AsQueryable();
        }

        public void ClearPredicates()
        {
            queryWhereProperty = null;
            queryWhereValue = null;
        }

        public bool GetItemsOfType<T>(out bool doStandardTraversal, out int traversalDepthLimit, out IEnumerable<T> elements)
        {
            throw new NotImplementedException();
        }

        public void ExecuteGlobalWhere(XPathElement element)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
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

        public override IBase Parent
        {
            get
            {
                return null;
            }
        }

        public override string FolderKeyPair { get; set; }

        public override DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.Model;
            }
        }

        public override bool HasChildren
        {
            get
            {
                return this.Containers.Any();
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
            get { throw new NotImplementedException(); }
        }

        public ConstructType DefaultConstructType
        {
            get { throw new NotImplementedException(); }
        }

        public override Modifiers Modifiers
        {
            get
            {
                return Modifiers.Unknown;
            }
        }

        public override string ParentID
        {
            get
            {
                return null;
            }
        }

        public string ParentFieldName => throw new NotImplementedException();

        public ProviderType ProviderType => throw new NotImplementedException();

        public IEnumerable<IElement> RootElements => throw new NotImplementedException();
    }
}
