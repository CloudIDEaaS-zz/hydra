using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using AbstraX.ServerInterfaces;
using System.Runtime.Serialization;
using System.Diagnostics;
using AbstraX;
using AbstraX.Models.Interfaces;
using AbstraX.TypeMappings;
using Utils;

namespace EntityProvider.Web.Entities
{
    [DebuggerDisplay(" { Name } ")]
    public class EntityProperty : EntityBase, IAttribute, IEntityWithFacets, IEntityProperty
    {
        protected string name;
        protected EntityType parent;
        protected string typeAttribute;
        private string imageURL;
        protected bool hasDocumentation;
        protected string documentationSummary;
        protected string documentation;
        protected float childOrdinal;
        protected string debugInfo;
        protected ScalarType dataType;
        protected string designComments;
        protected string id;
        private bool nullable;

        public EntityProperty(XmlDocument edmxDocument, XmlNamespaceManager namespaceManager, string name, XmlNode node, EntityType parent) : base(parent)
        {
            this.EdmxDocument = edmxDocument;
            this.NamespaceManager = namespaceManager;
            this.name = name;
            this.node = node;
            this.parent = parent;

            id = this.MakeID("Property='" + this.name + "'");

            var keyNodes = parent.Node.SelectNodes("e:Key/e:PropertyRef[@Name='" + name + "']", namespaceManager);

            typeAttribute = node.Attributes["Type"].Value;

            if (node.Attributes["Nullable"] != null)
            {
                nullable = bool.Parse(node.Attributes["Nullable"].Value);
            }
            else if (typeAttribute != "String")
            {
                nullable = true;
            }

            var type = Type.GetType("System." + typeAttribute);

            if (type == null)
            {
                switch (typeAttribute)
                {
                    case "Binary":
                        type = Type.GetType("System.Byte[]");
                        break;
                }
            }

            if (keyNodes.Count > 0)
            {
                imageURL = "EntityProvider.Web.Images.EntityPropertyKey.png";
                childOrdinal = 0;
            }
            else
            {
                imageURL = "EntityProvider.Web.Images.EntityProperty.png";
                childOrdinal = 1;
            }

            dataType = new ScalarType(type, this);
            debugInfo = this.GetDebugInfo();
        }

        public override string ID
        {
            get
            {
                return id;
            }

            protected set
            {
            }
        }

        public override string Name
        {
            get 
            {
                return name;
            }
        }

        public ScalarType DataType
        {
            get
            {
                return dataType;
            }
        }

        public IEnumerable<IElement> ChildElements
        {
            get { return null; }
        }

        public IEnumerable<IBase> ChildNodes
        {
            get { return null; }
        }

        public IQueryable ExecuteWhere(string property, object value)
        {
            throw new NotImplementedException();
        }

        public IQueryable ExecuteWhere(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable ExecuteWhere(AbstraX.XPathBuilder.XPathElement element)
        {
            throw new NotImplementedException();
        }

        public void ClearPredicates()
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
                return documentation;
            }
        }

        public override bool HasDocumentation
        {
            get
            {
                return hasDocumentation;
            }
        }

        public override string DocumentationSummary
        {
            get
            {
                return documentationSummary;
            }
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
                return debugInfo;
            }
        }

        public override string ParentID
        {
            get 
            {
                return parent.ID;
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

        public override DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.SimpleProperty;
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
                var args = new GetAttributesEventArgs(this, this.parent.GetOriginalDataType(), this.Name);
                var facets = new List<Facet>();

                foreach (var handler in this.getAttributesEventHandlers)
                {
                    handler(this, args);

                    foreach (var attribute in args.Attributes)
                    {
                        var facet = new Facet(attribute);

                        facets.Add(facet);
                    }
                }

                return facets.ToArray();
            }
        }

        public override Modifiers Modifiers
        {
            get
            {
                return Modifiers.Unknown;
            }
        }

        public string DefaultValue
        {
            get
            {
                return null;
            }
        }

        public bool Nullable
        {
            get
            {
                return nullable;
            }
        }
    }
}
