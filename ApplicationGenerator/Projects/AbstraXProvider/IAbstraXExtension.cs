using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if SILVERLIGHT
using Mvvm;
using MvvmTreeView;
using AbstraX.ClientInterfaces;
#else
using AbstraX.ServerInterfaces;
#endif


namespace AbstraX
{
    public interface IAbstraXExtension
    {
#if SILVERLIGHT
        ContextMenu ContextMenu { get; }
        void Initialize(IBase baseObject, ITreeNode treeNode);
#endif
    }
    
    public interface IEntityProviderExtension: IAbstraXExtension
    {

    }
}
