using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class ModelInvocationNode : BaseNode
    {
        public string MethodName { get; }
        public SeparatedSyntaxList<TypeSyntax> TypeArguments { get; }

        public ModelInvocationNode(SyntaxTreeNode syntaxTreeNode, string contentPart, AssignmentNode assignmentNode, string methodName, SeparatedSyntaxList<TypeSyntax> typeArguments) : base(NodeKind.ModelInvocation, syntaxTreeNode, contentPart)
        {
            this.MethodName = methodName;
            this.TypeArguments = typeArguments;
        }

        public override void Accept(RazorSemanticVisitor visitor)
        {
        }
    }
}
