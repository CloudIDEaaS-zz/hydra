using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Samples.Debugging.CorDebug;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.WorkspaceHandlers.CSharpWorkspaceFileType.Nodes
{
    public class IfDirectiveBlock : IfDirectiveNodeBase
    {
        public IfBlock IfBlock { get; private set; }
        public ElseBlock ElseBlock { get; private set; }
        public List<ElifBlock> ElifBlocks { get; private set; }
        public EndIfBlock EndIfBlock { get; private set; }
        public IfDirectiveNodeBase ActiveBlock { get; private set; }
        public string Condition { get; }
        public bool IsGeneratorToken { get; }

        public IfDirectiveBlock(IfDirectiveTriviaSyntax node, string conditionToken, bool isGeneratorToken) : base(node)
        {
            var childNodesCollectionChanged = (INotifyCollectionChanged)this.ChildNodes;

            this.ElifBlocks = new List<ElifBlock>();
            this.Condition = conditionToken;
            this.IsGeneratorToken = isGeneratorToken;

            childNodesCollectionChanged.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems.Cast<IfDirectiveNodeBase>())
                    {
                        switch (item.Node)
                        {
                            case IfDirectiveTriviaSyntax ifDirective:
                                this.IfBlock = (IfBlock)item;
                                this.ActiveBlock = item;
                                break;
                            case ElifDirectiveTriviaSyntax elifDirective:
                                this.ElifBlocks.Add((ElifBlock)item);
                                this.ActiveBlock = item;
                                break;
                            case ElseDirectiveTriviaSyntax elseDirective:
                                this.ElseBlock = (ElseBlock)item;
                                this.ActiveBlock = item;
                                break;
                            case EndIfDirectiveTriviaSyntax endIfDirective:
                                this.EndIfBlock = (EndIfBlock)item;
                                break;
                        }
                    }
                }
            };

            this.AddChild(new IfBlock(node));
        }

        public void CalculateSpans()
        {
            LinkedListNode<IfDirectiveNodeBase> linkedListNode;
            
            this.LinkedList = new LinkedList<IfDirectiveNodeBase>(this.ChildNodes);

            linkedListNode = this.LinkedList.First;

            while (linkedListNode != null)
            {
                var node = linkedListNode.Value;

                if (linkedListNode.Previous != null)
                {
                    var previousNode = linkedListNode.Previous.Value;

                    node.PreviousNode = previousNode;
                }

                if (linkedListNode.Next != null)
                {
                    var nextNode = linkedListNode.Next.Value;

                    if (nextNode == null)
                    {
                        node.BlockEnd = node.End;
                    }
                    else
                    {
                        node.BlockEnd = nextNode.Node.Span.End;
                    }

                    node.NextNode = nextNode;
                }

                linkedListNode = linkedListNode.Next;
            }
        }

        public override string Code
        {
            get
            {
                return IfBlock.Code;
            }
        }

        public LinkedList<IfDirectiveNodeBase> LinkedList { get; private set; }
    }
}
