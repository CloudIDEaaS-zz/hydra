using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvvmTreeView;
using NodeExpander;
#if SILVERLIGHT
using AbstraX.ClientInterfaces;
using Hydra.Shell.Interfaces;
using System.ServiceModel.DomainServices.Client;
#else
using AbstraX.ServerInterfaces;
#endif

#if SILVERLIGHT
namespace AbstraX.ClientInterfaces
#else
namespace AbstraX.Contracts
#endif
{
#if !SILVERLIGHT
    public interface IHydraPersistHierarchyItem { };
    public interface IAbstraXDocument { };
    public interface IPersistObject<T> { };
    public interface IHydraPersistDocData { };
    public interface IAbstraXDocumentSave { };
    public interface IObjectWithSite { };
    public interface IUnknown { };
#endif

    public interface IAbstraXExtension : IHydraPersistHierarchyItem, IHydraPersistDocData, IPersistObject<IAbstraXDocumentSave>, IAbstraXDocumentSave, IObjectWithSite, IDocument, IUnknown
    {
        ContextMenu ContextMenu { get; }
        INodeExpander NodeExpander { get; }
        IBase BaseObject { get; }
        void Initialize(IBase baseObject, ITreeNode treeNode);
        void Initialize(IBase baseObject);
        EnumInvokeOperation<CommittedState> GetCommittedState();
        bool DoesAllowRename(IBase baseObject, ITreeNode treeNode);
    }
}
