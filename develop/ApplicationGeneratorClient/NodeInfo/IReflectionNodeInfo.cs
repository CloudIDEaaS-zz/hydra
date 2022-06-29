using System;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace Net2Html5ConfigurationTool.NodeInfo
{
    public interface IReflectionNodeInfo
    {
        AstNode AstNode { get; }
        NodeInfoType NodeInfoType { get; }
        IAssembly NotYetIncludedAssembly { get; }
    }
}
