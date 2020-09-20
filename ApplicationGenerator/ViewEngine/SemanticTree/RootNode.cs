using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;
using Utils;
using Utils.Hierarchies;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class RootNode : MarkupBlockNode
    {
        public BaseList<DirectiveNode> Directives { get; private set; }
        public ModelNode modelNode;

        public RootNode(SyntaxTreeNode syntaxTreeNode, string contentPart) : base(NodeKind.Root, syntaxTreeNode, contentPart)
        {
            this.Directives = new BaseList<DirectiveNode>();
            this.Directives.CollectionChanged += Directives_CollectionChanged;
        }

        private void Directives_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var directive = e.NewItems.Cast<DirectiveNode>().Single();

            this.AddChild(directive);
        }

        public override void Accept(RazorSemanticVisitor visitor)
        {
            visitor.VisitRoot(this);
        }

        public IEnumerable<BaseNode> AllNodes
        {
            get
            {
                var list = new List<BaseNode>();

                ((BaseNode)this).GetDescendantsAndSelf((n) => n.ChildNodes, (n) => list.Add(n));

                return list;
            }
        }

        public ModelNode ModelNode
        {
            get
            {
                return modelNode;
            }

            set
            {
                modelNode = value;

                this.AddChild(modelNode);
            }
        }
    }
}
