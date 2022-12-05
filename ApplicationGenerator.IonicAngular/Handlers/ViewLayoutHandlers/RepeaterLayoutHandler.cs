using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Base;
using AbstraX.Generators.Pages.RepeaterPage;
using AbstraX.ServerInterfaces;
using AbstraX.ViewEngine;
using AbstraX.ViewEngine.SemanticTree;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.ViewLayoutHandlers
{
    [ViewLayoutHandler("_Repeater", ModuleImports.IONIC_ANGULAR_BASIC_PAGE_IMPORTS)]
    public class RepeaterLayoutHandler : BaseViewLayoutHandler
    {
        public RepeaterLayoutHandler()
        {
        }

        public override bool Process(IBase baseObject, Facet facet, IView view, IGeneratorConfiguration generatorConfiguration)
        {
            var uiAttribute = (UIAttribute)facet.Attribute;
            var name = baseObject.GetNavigationName();
            var parentObject = (IParentBase)baseObject;
            var pagesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Pages];
            var pagesFolder = (Folder) generatorConfiguration.FileSystem[pagesPath];
            var loadKind = uiAttribute.UILoadKind;
            var kind = uiAttribute.UIKind;
            var imports = generatorConfiguration.CreateImports(this, baseObject, pagesFolder, false, 1);
            IModuleAssembly module;

            this.BaseObject = baseObject;
            this.Facet = facet;
            this.Name = name;

            module = generatorConfiguration.PushModuleAssembly<AngularModule>(name);

            RepeaterPageGenerator.GeneratePage(baseObject, pagesPath, name, generatorConfiguration, module, imports, (View) view, this, loadKind, kind);

            return true;
        }

        public override void VisitNode(BaseNode node)
        {
            this.VisitedNodes.Add(node);

            base.VisitNode(node);
        }

        public override void VisitRoot(RootNode rootNode)
        {
            this.RootNode = rootNode;

            base.VisitRoot(rootNode);
        }

        public override void VisitDirective(DirectiveNode directiveNode)
        {
            base.VisitDirective(directiveNode);
        }

        public override void VisitMarkup(MarkupNode markupNode)
        {
            if (!markupNode.IsModelMarkup)
            {
                this.PageWriter.Write(markupNode.ContentPart);
            }

            base.VisitMarkup(markupNode);
        }

        public override void VisitModel(ModelNode modelNode)
        {
            this.ModelType = modelNode.ModelType;

            if (this.BaseObject.Name != this.ModelType)
            {
                if (this.BaseObject is IParentBase)
                {
                    var parentBase = (IParentBase)this.BaseObject;
                    var child = parentBase.ChildElements.Single(e => e.Name == this.ModelType);

                    this.IsChildSet = true;
                    this.SetObject = child;
                }
                else
                {
                    DebugUtils.Break();
                }
            }

            base.VisitModel(modelNode);
        }

        public override void VisitRender(RenderNode renderNode)
        {
            base.VisitRender(renderNode);
        }

        public override void VisitCode(CodeNode codeNode)
        {
            base.VisitCode(codeNode);
        }

        public override void VisitViewData(ViewDataNode viewDataNode)
        {
            this.ViewDataNodes.Add(viewDataNode);

            base.VisitViewData(viewDataNode);
        }

        public override void VisitViewBag(ViewBagNode viewBagNode)
        {
            this.ViewBagNodes.Add(viewBagNode);

            base.VisitViewBag(viewBagNode);
        }

        public override void VisitProperty(PropertyNode propertyNode)
        {
            this.PropertyNodes.Add(propertyNode);

            base.VisitProperty(propertyNode);
        }

        public override void VisitVariable(VariableNode variableNode)
        {
            this.VariableNodes.Add(variableNode);

            base.VisitVariable(variableNode);
        }

        public override void VisitModelInvocation(ModelInvocationNode modelInvocationNode)
        {
            this.ModelInvocationNodes.Add(modelInvocationNode);

            base.VisitModelInvocation(modelInvocationNode);
        }

        public override void VisitAssignment(AssignmentNode assignmentNode)
        {
            base.VisitAssignment(assignmentNode);
        }

        public override void VisitSection(SectionNode sectionNode)
        {
            base.VisitSection(sectionNode);
        }

        public override void VisitMarkupBlock(MarkupBlockNode markupBlockNode)
        {
            base.VisitMarkupBlock(markupBlockNode);
        }

        public override void VisitComment(CommentNode commentNode)
        {
            this.PageWriter.WriteLine(commentNode.Comment);

            base.VisitComment(commentNode);
        }

        public override void VisitCodeExpression(CodeExpressionNode codeExpressionNode)
        {
            base.VisitCodeExpression(codeExpressionNode);
        }

        public override void VisitHelperExpression(HelperExpressionNode helperExpressionNode)
        {
            base.VisitHelperExpression(helperExpressionNode);
        }

        public override void VisitCustomScriptsSection(CustomScriptsSectionNode customScriptsSectionNode)
        {
            base.VisitCustomScriptsSection(customScriptsSectionNode);
        }

        public override void VisitScriptExpression(ScriptExpressionNode scriptExpressionNode)
        {
            base.VisitScriptExpression(scriptExpressionNode);
        }
    }
}
