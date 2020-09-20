using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.WorkspaceHandlers.CSharpWorkspaceFileType.Nodes
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public abstract class IfDirectiveNodeBase
    {
        public DirectiveTriviaSyntax Node { get; }
        public BaseList<IfDirectiveNodeBase> ChildNodes { get; }
        public IfDirectiveNodeBase Parent { get; set; }
        public abstract string Code { get; }
        public int BlockEnd { get; set; }
        public IfDirectiveNodeBase PreviousNode { get; internal set; }
        public IfDirectiveNodeBase NextNode { get; internal set; }
        public int ShiftAmount { get; set; }

        public IfDirectiveNodeBase(DirectiveTriviaSyntax node)
        {
            this.ChildNodes = new BaseList<IfDirectiveNodeBase>();
            this.Node = node;
        }

        public bool? BranchTaken
        {
            get
            {
                switch (this.Node)
                {
                    case IfDirectiveTriviaSyntax ifDirective:
                        return ifDirective.BranchTaken;
                    case ElifDirectiveTriviaSyntax elifDirective:
                        return elifDirective.BranchTaken;
                    case ElseDirectiveTriviaSyntax elseDirective:
                        return elseDirective.BranchTaken;
                    case EndIfDirectiveTriviaSyntax endIfDirective:
                        return null;
                }

                throw new NotImplementedException();
            }
        }

        public int Start
        {
            get
            {
                return this.Node.FullSpan.Start;
            }
        }

        public int End
        {
            get
            {
                if (this.NextNode != null)
                {
                    return this.NextNode.Node.FullSpan.End;
                }
                else
                {
                    return this.Node.FullSpan.End;
                }
            }
        }

        public int Length
        {
            get
            {
                return this.End - this.Start;
            }
        }

        public void AddChild(IfDirectiveNodeBase childNode)
        {
            childNode.Parent = this;

            this.ChildNodes.Add(childNode);
        }

        public string DebugInfo
        {
            get
            {
                return string.Format("{0} Code:'{1}', ChildNodes:{2}, BranchTaken:{3}", this.GetType(), this.Code, this.ChildNodes.Count, this.BranchTaken);
            }
        }

        public override string ToString()
        {
            return this.DebugInfo;
        }
    }
}
