
namespace AbstraX.Service
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using AbstraX.ServerInterfaces;
    using AbstraX.Contracts;
    using AbstraX.Bindings.Interfaces;
    using AbstraX.Bindings;
    using Entities = AbstraX.BindingsTreeEntities;
    using AbstraX.BindingsTreeEntities;
    using AbstraX.TypeMappings;
    using System.Text.RegularExpressions;
using System.Web;
    using System.Threading.Tasks;

    /// <summary>
    /// Todo:
    /// - make checkboxes mutually exlusive
    /// - assure menu has right checked item on load
    /// - handle keyboard events for space-bar selection of checkboxes
    /// - allow for renaming elements
    /// - use MvvmCommand for buttons
    /// </summary>

    [EnableClientAccess()]
    public class BindingsTreeService : DomainService, IBindingsTreeService
    {
        public IDomainHostApplication DomainServiceHostApplication { get; set; }

        [Query]
        public IQueryable<Entities.CachedBindingsTree> GetCachedBindingsTrees()
        {
            var host = this.DomainServiceHostApplication;

            return host.BindingsTreeCache.CachedTrees.Values.Select(t => new Entities.CachedBindingsTree(t.RootElementID, t.CachedNodes.BindingsTree)).AsQueryable();
        }

        [Invoke]
        public void BreakOnGenerator(string id)
        {
            lock (this.DomainServiceHostApplication)
            {
                var host = this.DomainServiceHostApplication;
                var cachedTree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(id));
                var baseNode = cachedTree.CachedNodes.Nodes[id];
                var elementID = cachedTree.RootElementID;
                var generator = host.BindingsGenerator;
                var rootID = AbstraXExtensions.GetRootID(elementID);
                var provider = this.DomainServiceHostApplication.RegisteredServices[rootID];
                var tree = (IBindingsTree)null;
                var internalObject = baseNode.InternalObject;

                if (internalObject is IBindingsTreeNode)
                {
                    var bindingsTreeNode = (IBindingsTreeNode)internalObject;
                    generator.BreakOnNode = bindingsTreeNode;

                    tree = generator.CreateBindings(provider, elementID);
                }
                else if (internalObject is IBindingsTreeNodeReference)
                {
                    var bindingsTreeNodeReference = (IBindingsTreeNodeReference)internalObject;
                    generator.BreakOnNode = bindingsTreeNodeReference;

                    tree = generator.CreateBindings(provider, elementID);
                }

                foreach (var binding in tree.RootBindings)
                {

                }

                generator.BreakOnNode = null;
            }
        }

        [Invoke]
        public void BreakOnBuilder(string id)
        {
            lock (this.DomainServiceHostApplication)
            {
                var host = this.DomainServiceHostApplication;
                var cachedTree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(id));
                var baseNode = cachedTree.CachedNodes.Nodes[id];
                var elementID = cachedTree.RootElementID;
                var generator = host.BindingsGenerator;
                var builder = host.BindingsBuilder;
                var rootID = AbstraXExtensions.GetRootID(elementID);
                var provider = this.DomainServiceHostApplication.RegisteredServices[rootID];
                var internalObject = baseNode.InternalObject;

                if (internalObject is IBindingsTreeNode)
                {
                    var bindingsTreeNode = (IBindingsTreeNode)internalObject;
                    var tree = generator.CreateBindings(provider, elementID);

                    builder.BreakOnNode = bindingsTreeNode;
                    builder.GenerateBuilds(new List<IBindingsTree>() { tree });
                }
                else if (internalObject is IBindingsTreeNodeReference)
                {
                    var bindingsTreeNodeReference = (IBindingsTreeNodeReference)internalObject;
                    var tree = generator.CreateBindings(provider, elementID);

                    builder.BreakOnNode = bindingsTreeNodeReference;
                    builder.GenerateBuilds(new List<IBindingsTree>() { tree });
                }

                builder.BreakOnNode = null;
            }
        }

        [Query]
        public IQueryable<Entities.BindingsTreeEntity> GetBindingsTreeForElementID(string elementID)
        {
            var host = this.DomainServiceHostApplication;
            IBindingsTree tree;
            IBindingsTreeCachedTree cachedTree;
            Entities.BindingsTreeEntity bindingsTree = null;

            if (host.BindingsTreeCache.CachedTrees.ContainsKey(elementID))
            {
                cachedTree = host.BindingsTreeCache.CachedTrees[elementID];
                tree = cachedTree.CachedNodes.BindingsTree;

                if (cachedTree.CachedNodes.Nodes.Values.Any(n => n is Entities.BindingsTreeEntity))
                {
                    bindingsTree = (Entities.BindingsTreeEntity) cachedTree.CachedNodes.Nodes.Values.Single(n => n is Entities.BindingsTreeEntity);
                }
            }
            else
            {
                var generator = host.BindingsGenerator;
                var rootID = AbstraXExtensions.GetRootID(elementID);
                var provider = this.DomainServiceHostApplication.RegisteredServices[rootID];

                tree = generator.CreateBindings(provider, elementID);

                cachedTree = new CachedBindingsTree(elementID, tree);
                host.BindingsTreeCache.CachedTrees.Add(elementID, cachedTree);
            }

            if (bindingsTree == null)
            {
                bindingsTree = new Entities.BindingsTreeEntity(tree, elementID);
                cachedTree.CachedNodes.Nodes.Add(bindingsTree.ID, bindingsTree);
            }

            var bindingsTreeCache = host.BindingsTreeCache;

            return new List<Entities.BindingsTreeEntity>() { bindingsTree }.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.BindingsTreeNode> GetRootBindingNodesForBindingsTree(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var bindingsTree = (Entities.BindingsTreeEntity) tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(bindingsTree.RootBindingEntities);

            return bindingsTree.RootBindingEntities.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.Folder> GetParentSourceElementForBindingsTreeNode(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var bindingsTreeNode = (Entities.BindingsTreeNode)tree.CachedNodes.Nodes[parentID];

            if (bindingsTreeNode.ParentSourceElementFolder != null)
            {
                tree.AddToCache(bindingsTreeNode.ParentSourceElementFolder);

                return new List<Entities.Folder>() { bindingsTreeNode.ParentSourceElementFolder }.AsQueryable();
            }
            else
            {
                return null;
            }
        }

        [Query]
        public IQueryable<Entities.Element> GetElementForParentSourceElement(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var folder = (Entities.Folder)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(folder.Elements);

            return folder.Elements.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.Folder> GetDataContextListForBindingsTreeNode(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var bindingsTreeNode = (Entities.BindingsTreeNode)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(bindingsTreeNode.DataContextListFolder);

            return new List<Entities.Folder>() { bindingsTreeNode.DataContextListFolder }.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.DataContextObject> GetDataContextForDataContextList(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var folder = (Entities.Folder)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(folder.ContextObjects);

            return folder.ContextObjects.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.Folder> GetContextObjectForDataContext(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var dataContextObject = (Entities.DataContextObject)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(dataContextObject.DataContextObjectFolder);

            return new List<Entities.Folder>() { dataContextObject.DataContextObjectFolder }.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.Element> GetElementForContextObject(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var folder = (Entities.Folder)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(folder.ContextObject);

            return new List<Entities.Element>() { folder.ContextObject }.AsQueryable();
        }

        public void UpdateElement(Entities.Element element)
        {
            var id = element.ID;
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(id));
            var cachedElement = (Entities.Element)tree.CachedNodes.Nodes[id];

            cachedElement.ConstructType = element.UpdatedConstructType;
            cachedElement.ContainerType = element.UpdatedContainerType;
            cachedElement.Name = element.Name;

            element.CloneFrom(cachedElement); 
        }

        [Query]
        public IQueryable<Entities.Folder> GetSupportedOperationsForDataContext(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var dataContextObject = (Entities.DataContextObject)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(dataContextObject.SupportedOperationsFolder);

            return new List<Entities.Folder>() { dataContextObject.SupportedOperationsFolder }.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.NodeProperty> GetOperationForSupportedOperations(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var folder = (Entities.Folder)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(folder.SupportedOperations);

            return folder.SupportedOperations.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.Folder> GetRemoteCallsForDataContext(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var dataContextObject = (Entities.DataContextObject)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(dataContextObject.RemoteCallsFolder);

            return new List<Entities.Folder>() { dataContextObject.RemoteCallsFolder }.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.Operation> GetOperationForRemoteCall(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var folder = (Entities.Folder)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(folder.Operations);

            return folder.Operations.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.Folder> GetPropertyBindingsForBindingsTreeNode(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var bindingsTreeNode = (Entities.BindingsTreeNode)tree.CachedNodes.Nodes[parentID];

            if (bindingsTreeNode.PropertyBindingsFolder.ChildNodes.Count > 0)
            {
                tree.AddToCache(bindingsTreeNode.PropertyBindingsFolder);

                return new List<Entities.Folder>() { bindingsTreeNode.PropertyBindingsFolder }.AsQueryable();
            }
            else
            {
                return null;
            }
        }

        [Query]
        public IQueryable<Entities.PropertyBinding> GetPropertyBindingForPropertyBindings(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var folder = (Entities.Folder)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(folder.PropertyBindings);

            return folder.PropertyBindings.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.AttributeProperty> GetPropertyForPropertyBinding(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var propertyBinding = (Entities.PropertyBinding)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(propertyBinding.PropertyAttribute);

            return new List<Entities.AttributeProperty>() { propertyBinding.PropertyAttribute }.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.Folder> GetBindingSourceForPropertyBinding(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var propertyBinding = (Entities.PropertyBinding)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(propertyBinding.BindingSourceFolder);

            return new List<Entities.Folder>() { propertyBinding.BindingSourceFolder }.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.AbstraXBindingSource> GetAbstraXBindingSourceForBindingSource(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var folder = (Entities.Folder)tree.CachedNodes.Nodes[parentID];

            if (folder.AbstraXBindingSource != null)
            {
                tree.AddToCache(folder.AbstraXBindingSource);

                return new List<Entities.AbstraXBindingSource>() { folder.AbstraXBindingSource }.AsQueryable();
            }
            else
            {
                return null;
            }
        }

        [Query]
        public IQueryable<Entities.QueryBindingSource> GetQueryBindingSourceForBindingSource(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var folder = (Entities.Folder)tree.CachedNodes.Nodes[parentID];

            if (folder.QueryBindingSource != null)
            {
                tree.AddToCache(folder.QueryBindingSource);

                return new List<Entities.QueryBindingSource>() { folder.QueryBindingSource }.AsQueryable();
            }
            else
            {
                return null;
            }
        }

        [Query]
        public IQueryable<Entities.NodeProperty> GetBindingModeForPropertyBinding(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var propertyBinding = (Entities.PropertyBinding)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(propertyBinding.BindingModeProperty);

            return new List<Entities.NodeProperty>() { propertyBinding.BindingModeProperty }.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.Folder> GetChildNodesForBindingsTreeNode(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var bindingsTreeNode = (Entities.BindingsTreeNode)tree.CachedNodes.Nodes[parentID];

            if (bindingsTreeNode.ChildNodes.Count() > 0)
            {
                tree.AddToCache(bindingsTreeNode.ChildNodesFolder);

                return new List<Entities.Folder>() { bindingsTreeNode.ChildNodesFolder }.AsQueryable();
            }
            else
            {
                return null;
            }
        }

        [Query]
        public IQueryable<Entities.BindingsTreeNode> GetChildNodeForChildNodes(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var folder = (Entities.Folder)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(folder.BindingsTreeNodes);

            return folder.BindingsTreeNodes.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.BindingsTreeNodeReference> GetChildNodeReferencesForChildNodes(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var folder = (Entities.Folder)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(folder.BindingsTreeNodeReferences);

            return folder.BindingsTreeNodeReferences.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.AttributeProperty> GetBindingAttributeForAbstraXBindingSource(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var source = (Entities.AbstraXBindingSource)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(source.BindingAttributeProperty);

            return new List<Entities.AttributeProperty>() { source.BindingAttributeProperty }.AsQueryable();
        }

        [Query]
        public IQueryable<Entities.NodeProperty> GetIsSearchableForAbstraXBindingSource(string parentID)
        {
            var host = this.DomainServiceHostApplication;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(parentID));
            var source = (Entities.AbstraXBindingSource)tree.CachedNodes.Nodes[parentID];

            tree.AddToCache(source.IsSearchableProperty);

            return new List<Entities.NodeProperty>() { source.IsSearchableProperty }.AsQueryable();
        }

        [Invoke]
        public void Build(string nodeID)
        {
            var host = this.DomainServiceHostApplication;
            var generator = host.BindingsGenerator;
            var tree = host.BindingsTreeCache.CachedTrees.Values.Single(t => t.CachedNodes.Nodes.ContainsKey(nodeID));

            generator.GenerateFrom(tree.CachedNodes.BindingsTree);
        }
    }
}


