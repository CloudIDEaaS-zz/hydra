using System;
using AbstraX.Bindings;
using AbstraX.ServerInterfaces;

namespace AbstraX.Bindings.Interfaces
{
    public interface IBindingsGenerator
    {
        IBindingsTreeNode BreakOnNode { set; }
        IBindingsTree CreateBindings(IAbstraXProviderService providerService, string id);
        IBindingsTree CreateBindings(Type providerType, string id);
        IBindingsTree CreateBindings(string providerTypeName, string id);
        void GenerateFrom(IBindingsTree bindingsTree);
    }
}
