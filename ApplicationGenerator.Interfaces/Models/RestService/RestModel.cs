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

namespace RestEntityProvider.Web.Entities
{
    [DebuggerDisplay(" { Name } ")]
    public class RestModel : RestEntityBase, IElement, IRootWithOptions
    {
        private string queryWhereProperty;
        private object queryWhereValue;
        private string modelName;
        public string UrlBase { get; }
        public Dictionary<string, object> Variables { get;  }
        public new string Namespace { get; private set; }
        public bool IsGeneratedModel { get; } = true;

        public RestModel(FileInfo jsonFile, Dictionary<string, object> additionalOptions, string nameSpace = null)
        {
            string variablePrefix;

            modelName = Path.GetFileNameWithoutExtension(jsonFile.FullName);

            using (var reader = new StreamReader(jsonFile.FullName))
            {
                var jsonRootObject = (object) reader.ReadJson<dynamic>();

                this.JsonRootObject = jsonRootObject;
                this.JsonOriginalRootObject = jsonRootObject.CloneJson();
                this.JsonObject = this.JsonRootObject;
                this.JsonOriginalObject = this.JsonOriginalRootObject;
            }

            this.UrlBase = this.JsonRootObject.urlBase;
            variablePrefix = this.UrlBase + "/#";

            this.Variables = additionalOptions.Where(p => p.Key.StartsWith(variablePrefix)).ToDictionary(p => p.Key.RemoveStart(variablePrefix), p => p.Value);

            this.DoReplacements(this.Variables);

            this.PathPrefix = this.JsonRootObject.title + "Service";
            this.ConfigPrefix = this.JsonRootObject.title + "Config";
            this.ControllerNamePrefix = this.JsonRootObject.title;

            this.Namespace = nameSpace;
        }

        public IEnumerable<IAttribute> Attributes
        {
            get { return null; }
        }

        public List<RestEntityContainer> Containers
        {
            get
            {
                var containers = new List<RestEntityContainer>();

                try
                {
                    containers.Add(new RestEntityContainer(this.JsonRootObject, this.JsonObject, this.JsonOriginalRootObject, this.JsonOriginalObject, this.JsonRootObject.title + "Service", this));
                }
                catch
                {
                }

                return containers;
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
