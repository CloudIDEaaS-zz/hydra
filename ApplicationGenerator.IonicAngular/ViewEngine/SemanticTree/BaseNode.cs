using AbstraX.Generators.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;
using Utils;

namespace AbstraX.ViewEngine.SemanticTree
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public abstract class BaseNode : ISemanticTreeBaseNode
    {
        public BaseNode ParentNode { get; private set; }
        public BaseList<BaseNode> ChildNodes { get; private set; }
        public NodeKind Kind { get; }
        public string ContentPart { get; }
        public SyntaxTreeNode SyntaxTreeNode { get; }
        public int NodeKindInt => (int) Kind;

        public event EventHandlerT<BaseNode> OnParent;
        public abstract void Accept(RazorSemanticVisitor visitor);

        public BaseNode(NodeKind kind, SyntaxTreeNode syntaxTreeNode, string contentPart)
        {
            this.ChildNodes = new BaseList<BaseNode>();
            this.Kind = kind;
            this.ContentPart = contentPart;
            this.SyntaxTreeNode = syntaxTreeNode;

            this.ChildNodes.CollectionChanged += ChildNodes_CollectionChanged;
        }

        private void ChildNodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var baseNode = e.NewItems.Cast<BaseNode>().Single();

            baseNode.ParentNode = this;
            baseNode.RaiseOnParent();
        }

        protected void RaiseOnParent()
        {
            OnParent.Raise(this, this);
        }

        public void AddChild(BaseNode node)
        {
            this.ChildNodes.Add(node);
        }

        public string DebugInfo
        {
            get
            {
                return string.Format("Kind: {0}, "
        			+ "ChildCount: {1}, "
                    + "ContentPart: {2}, ",
                    this.Kind,
                    this.ChildNodes.Count,
        			this.ContentPart
                );
            }
        }
    }
}
