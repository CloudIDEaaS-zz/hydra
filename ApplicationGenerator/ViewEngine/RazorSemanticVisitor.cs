using AbstraX.Generators.Base;
using AbstraX.ViewEngine.SemanticTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.ViewEngine
{
    public abstract class RazorSemanticVisitor
    {
        public CancellationToken? CancelToken { get; set; }
        public DelayedWriterTemplateBase ClassWriter { get; set; }
        public DelayedWriterTemplateBase PageWriter { get; set; }

        public virtual void OnComplete()
        {
        }

        public virtual void ThrowIfCanceled()
        {
            if (this.CancelToken.HasValue && this.CancelToken.Value.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
        }
        public virtual void VisitNode(BaseNode node)
        {
            this.ThrowIfCanceled();
        }

        private void VisitChildren(BaseNode node)
        {
            foreach (var child in node.ChildNodes)
            {
                VisitNode(child);

                SwitchExtensions.Switch(child, () => child.GetType().Name,

                    SwitchExtensions.Case<DirectiveNode>("DirectiveNode", (directiveNode) =>
                    {
                        VisitDirective(directiveNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<MarkupNode> ("MarkupNode", (markupNode) =>
				    {   
                        VisitMarkup(markupNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<ModelNode>("ModelNode", (modelNode) =>
                    {
                        VisitModel(modelNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<RenderNode>("RenderNode", (renderNode) =>
                    {
                        VisitRender(renderNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<CodeNode>("CodeNode", (codeNode) =>
                    {
                        VisitCode(codeNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<ModelInvocationNode>("ModelInvocationNode", (modelInvocationNode) =>
                    {
                        VisitModelInvocation(modelInvocationNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<ViewDataNode>("ViewDataNode", (viewDataNode) =>
                    {
                        VisitViewData(viewDataNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<ViewBagNode>("ViewBagNode", (viewBagNode) =>
                    {
                        VisitViewBag(viewBagNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<PropertyNode>("PropertyNode", (propertyNode) =>
                    {
                        VisitProperty(propertyNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<VariableNode>("VariableNode", (variableNode) =>
                    {
                        VisitVariable(variableNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<SectionNode>("SectionNode", (sectionNode) =>
                    {
                        VisitSection(sectionNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<MarkupBlockNode>("MarkupBlockNode", (markupBlockNode) =>
                    {
                        VisitMarkupBlock(markupBlockNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<CommentNode>("CommentNode", (commentNode) =>
                    {
                        VisitComment(commentNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<CodeExpressionNode>("CodeExpressionNode", (codeExpressionNode) =>
                    {
                        VisitCodeExpression(codeExpressionNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<HelperExpressionNode>("HelperExpressionNode", (helperExpressionNode) =>
                    {
                        VisitHelperExpression(helperExpressionNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<CustomScriptsSectionNode>("CustomScriptsSectionNode", (customScriptsSectionNode) =>
                    {
                        VisitCustomScriptsSection(customScriptsSectionNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.Case<ScriptExpressionNode>("ScriptExpressionNode", (scriptExpressionNode) =>
                    {
                        VisitScriptExpression(scriptExpressionNode);

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.CaseElse(() =>
                    {
                        var a = child;
                        var t = child.GetType().Name;

                        // implementation here or throw error

                        DebugUtils.Break();
                    })
			    );
            }
        }

        public virtual void VisitViewBag(ViewBagNode viewBagNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(viewBagNode);
        }

        public virtual void VisitModelInvocation(ModelInvocationNode modelInvocationNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(modelInvocationNode);
        }

        public virtual void VisitScriptExpression(ScriptExpressionNode scriptExpressionNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(scriptExpressionNode);
        }

        public virtual void VisitCustomScriptsSection(CustomScriptsSectionNode customScriptsSectionNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(customScriptsSectionNode);
        }

        public virtual void VisitHelperExpression(HelperExpressionNode helperExpressionNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(helperExpressionNode);
        }

        public virtual void VisitComment(CommentNode commentNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(commentNode);
        }

        public virtual void VisitMarkupBlock(MarkupBlockNode markupBlockNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(markupBlockNode);
        }

        public virtual void VisitSection(SectionNode sectionNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(sectionNode);
        }

        public virtual void VisitVariable(VariableNode variableNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(variableNode);
        }

        public virtual void VisitProperty(PropertyNode propertyNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(propertyNode);
        }

        public virtual void VisitViewData(ViewDataNode viewDataNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(viewDataNode);
        }

        public virtual void VisitRender(RenderNode renderNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(renderNode);
        }

        public virtual void VisitModel(ModelNode modelNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(modelNode);
        }

        public virtual void VisitCode(CodeNode codeNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(codeNode);
        }

        public virtual void VisitMarkup(MarkupNode markupNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(markupNode);
        }

        public virtual void VisitRoot(RootNode rootNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(rootNode);
        }

        public virtual void VisitDirective(DirectiveNode directiveNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(directiveNode);
        }

        public virtual void VisitAssignment(AssignmentNode assignmentNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(assignmentNode);
        }

        public virtual void VisitCodeExpression(CodeExpressionNode codeExpressionNode)
        {
            this.ThrowIfCanceled();

            VisitChildren(codeExpressionNode);
        }
    }
}
