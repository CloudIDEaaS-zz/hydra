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

namespace EntityProvider.Web.Entities
{
    public class Model : EntitiesBase, IElement
    {
        private IAbstraXService service;
        private string url;
        private string queryWhereProperty;
        private object queryWhereValue;
        private string modelName;

        public Model(IAbstraXService service)
        {
        }

        public Model(FileInfo edmxFile)
        {
            modelName = Path.GetFileNameWithoutExtension(edmxFile.FullName);

            this.EdmxDocument = new XmlDocument();
            this.EdmxDocument.Load(edmxFile.OpenText());

            this.NamespaceManager = new XmlNamespaceManager(this.EdmxDocument.NameTable);

            this.NamespaceManager.AddNamespace("edmx", "http://schemas.microsoft.com/ado/2009/11/edmx");
            this.NamespaceManager.AddNamespace("s", "http://schemas.microsoft.com/ado/2009/11/edm/ssdl");
            this.NamespaceManager.AddNamespace("e", "http://schemas.microsoft.com/ado/2009/11/edm");
        }

        public IEnumerable<IAttribute> Attributes
        {
            get { return null; }
        }

        public List<Entity_Container> Containers
        {
            get
            {
                var nameSpaces = new List<Entity_Container>();

                try
                {
                    if (queryWhereProperty != null && queryWhereProperty == "Namespace")
                    {
                        var node = this.EdmxDocument.SelectSingleNode("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema/e:EntityContainer/@Name='" + (string)queryWhereValue + "'", this.NamespaceManager);

                        Debug.Assert(node != null);

                        nameSpaces.Add(new Entity_Container(this.EdmxDocument, this.NamespaceManager, node.Value, node.SelectSingleNode("parent::*"), this));
                    }
                    else
                    {
                        foreach (XmlNode node in this.EdmxDocument.SelectNodes("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema/e:EntityContainer", this.NamespaceManager))
                        {
                            nameSpaces.Add(new Entity_Container(this.EdmxDocument, this.NamespaceManager, node.Attributes["Name"].Value, node, this));
                        }
                    }
                }
                catch
                {
                }

                return nameSpaces;
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

        public IEnumerable<IOperation> Operations
        {
            get { return null; }
        }

        public string ID
        {
            get
            {
                return this.MakeID("Model='" + modelName + "'");
            }
        }

        public string Name
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
            if (value is XPathAxisElement && (property == "ID" || property == "ParentID"))
            {
                var predicate = ((XPathAxisElement)value).Predicates.First();

                queryWhereProperty = predicate.Left;
                queryWhereValue = predicate.Right;
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

        public IQueryable ExecuteWhere(XPathAxisElement element)
        {
            var predicate = element.Predicates.First();

            queryWhereProperty = predicate.Left;
            queryWhereValue = predicate.Right;

            return this.ChildElements.AsQueryable();
        }

        public void ClearPredicates()
        {
            queryWhereProperty = null;
            queryWhereValue = null;
        }

        public IRoot Root
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


        public string DesignComments
        {
            get
            {
                return string.Empty;
            }
        }

        public string Documentation
        {
            get
            {
                return string.Empty;
            }
        }

        public bool HasDocumentation
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public string DocumentationSummary
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }
        }

        public float ChildOrdinal
        {
            get
            {
                return 0;
            }
        }

        public string DebugInfo
        {
            get
            {
                return this.GetDebugInfo();
            }
        }

        public IBase Parent
        {
            get
            {
                return null;
            }
        }

        public string FolderKeyPair { get; set; }

        public DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.NotApplicable;
            }
        }

        public bool HasChildren
        {
            get
            {
                return this.Containers.Any();
            }
        }

        public Facet[] Facets
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

        public Modifiers Modifiers
        {
            get
            {
                return Modifiers.Unknown;
            }
        }

        public string ParentID
        {
            get
            {
                return null;
            }
        }
    }
}
