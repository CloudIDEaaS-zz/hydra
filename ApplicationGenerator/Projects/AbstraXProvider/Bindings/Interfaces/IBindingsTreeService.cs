using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using AbstraX.ServerInterfaces;
using System.Collections;
using AbstraX.BindingsTreeEntities;
using AbstraX.TypeMappings;

namespace AbstraX.Contracts
{
    public interface IBindingsTreeCollection : ICollection
    {
    }

    public interface IBindingsTreeService
    {
        IDomainHostApplication DomainServiceHostApplication { get; set; }
        IQueryable<CachedBindingsTree> GetCachedBindingsTrees();
        IQueryable<BindingsTreeEntity> GetBindingsTreeForElementID(string elementID);
        IQueryable<BindingsTreeNode> GetRootBindingNodesForBindingsTree(string parentID);
        IQueryable<Folder> GetParentSourceElementForBindingsTreeNode(string parentID);
        IQueryable<Element> GetElementForParentSourceElement(string parentID);
        IQueryable<Folder> GetDataContextListForBindingsTreeNode(string parentID);
        IQueryable<DataContextObject> GetDataContextForDataContextList(string parentID);
        IQueryable<Folder> GetContextObjectForDataContext(string parentID);
        IQueryable<Element> GetElementForContextObject(string parentID);
        IQueryable<Folder> GetSupportedOperationsForDataContext(string parentID);
        IQueryable<NodeProperty> GetOperationForSupportedOperations(string parentID);
        IQueryable<Folder> GetRemoteCallsForDataContext(string parentID);
        IQueryable<Operation> GetOperationForRemoteCall(string parentID);
        IQueryable<Folder> GetPropertyBindingsForBindingsTreeNode(string parentID);
        IQueryable<PropertyBinding> GetPropertyBindingForPropertyBindings(string parentID);
        IQueryable<AttributeProperty> GetPropertyForPropertyBinding(string parentID);
        IQueryable<Folder> GetBindingSourceForPropertyBinding(string parentID);
        IQueryable<AbstraXBindingSource> GetAbstraXBindingSourceForBindingSource(string parentID);
        IQueryable<QueryBindingSource> GetQueryBindingSourceForBindingSource(string parentID);
        IQueryable<NodeProperty> GetBindingModeForPropertyBinding(string parentID);
        IQueryable<Folder> GetChildNodesForBindingsTreeNode(string parentID);
        IQueryable<BindingsTreeNode> GetChildNodeForChildNodes(string parentID);
        void Build(string nodeID);
    }
}

//#endif