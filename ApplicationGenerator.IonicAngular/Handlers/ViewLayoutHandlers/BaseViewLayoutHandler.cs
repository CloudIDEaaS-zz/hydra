using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.Angular;
using AbstraX.Generators.Base;
using AbstraX.ServerInterfaces;
using AbstraX.ViewEngine;
using AbstraX.ViewEngine.SemanticTree;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using AbstraX.Parser;
using Utils;
using EntityProvider.Web.Entities;
using System.Diagnostics;

namespace AbstraX.Handlers.ViewLayoutHandlers
{
	public abstract class BaseViewLayoutHandler : RazorSemanticVisitor, IModuleAddViewLayoutHandler
	{
		public virtual float Priority => 4.0f;
		public List<Module> RelatedModules { get; }
		public RootNode RootNode { get; protected set; }
		public string ModelType { get; protected set; }
		public List<PropertyNode> PropertyNodes { get; }
		public List<VariableNode> VariableNodes { get; }
		public List<ViewDataNode> ViewDataNodes { get; }
		public Dictionary<string, object> ViewData { get; }
		public List<ViewBagNode> ViewBagNodes { get; }
		public Dictionary<string, object> ViewBag { get; }
		public List<ModelInvocationNode> ModelInvocationNodes { get; }
		public IBase BaseObject { get; protected set; }
		public Facet Facet { get; protected set; }
		public bool IsChildSet { get; protected set; }
		public IElement SetObject { get; protected set; }
		public string Name { get; protected set; }
		public string PartialComponentName { get; protected set; }
		public List<BaseNode> VisitedNodes { get; protected set; }
		public List<IBase> ModelObjectGraph { get; set; }

		public event ChildViewHandler OnChildView;

		public BaseViewLayoutHandler()
		{
			this.RelatedModules = new List<Module>();
			this.PropertyNodes = new List<PropertyNode>();
			this.VariableNodes = new List<VariableNode>();
			this.ModelInvocationNodes = new List<ModelInvocationNode>();
			this.ViewDataNodes = new List<ViewDataNode>();
			this.ViewBagNodes = new List<ViewBagNode>();
			this.ViewData = new Dictionary<string, object>();
			this.ViewBag = new Dictionary<string, object>();

			this.VisitedNodes = new List<BaseNode>();
		}

		public override void VisitNode(BaseNode node)
		{
			this.ClassWriter.CurrentNode = node;
			this.ClassWriter.CurrentNodeKind = node.NodeKindInt;
			this.PageWriter.CurrentNode = node;
			this.PageWriter.CurrentNodeKind = node.NodeKindInt;

			base.VisitNode(node);
		}

		public abstract bool Process(IBase baseObject, Facet facet, IView view, IGeneratorConfiguration generatorConfiguration);

		public virtual bool PostProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IViewLayoutHandler currentHandler)
		{
			return true;
		}

		public virtual bool PreProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IViewLayoutHandler currentHandler)
		{
			return true;
		}

		public override void VisitViewData(ViewDataNode viewDataNode)
		{
			var key = viewDataNode.Key;
			var right = viewDataNode.Right;
			object value = null;

			if (right is LiteralExpressionSyntax)
			{
				var literalExpression = (LiteralExpressionSyntax)right;
				value = literalExpression.GetValue();
			}
			else
			{
				DebugUtils.Break();
			}

			if (this.ViewData.ContainsKey(key))
			{
				this.ViewData[key] = value;
			}
			else
			{
				this.ViewData.Add(key, value);
			}

			base.VisitViewData(viewDataNode);
		}

		public override void VisitViewBag(ViewBagNode viewBagNode)
		{
			var name = viewBagNode.PropertyName;
			var right = viewBagNode.Right;
			object value = null;

			if (right is LiteralExpressionSyntax)
			{
				var literalExpression = (LiteralExpressionSyntax)right;
				value = literalExpression.GetValue();
			}
			else
			{
				DebugUtils.Break();
			}

			this.ViewBag.Add(name, value);
			
			base.VisitViewBag(viewBagNode);
		}

		public override void VisitHelperExpression(HelperExpressionNode helperExpressionNode)
		{
			if (helperExpressionNode.Method == "Partial")
			{
				var partialViewNameArg = helperExpressionNode.Arguments[0].Expression;
				var modelArg = helperExpressionNode.Arguments[1]?.Expression;
				var viewDataArg = helperExpressionNode.Arguments[2]?.Expression;
				var viewData = new Dictionary<string, object>();
				string partialViewName = null;
				var baseObject = this.BaseObject;
				var modelObjectGraph = new List<IBase>() { baseObject };

				SwitchExtensions.Switch(partialViewNameArg, () => partialViewNameArg.GetType().Name,

					SwitchExtensions.Case<LiteralExpressionSyntax>("LiteralExpressionSyntax", (literalExpression) =>
					{
						partialViewName = literalExpression.Token.Text.RemoveQuotes();

						DebugUtils.NoOp();
					}),
					SwitchExtensions.CaseElse(() =>
					{
						var a = partialViewNameArg;
						var t = partialViewNameArg.GetType().Name;

						// implementation here or throw error

						DebugUtils.Break();
					})
				);

				if (modelArg != null)
				{ 
					SwitchExtensions.Switch(modelArg, () => modelArg.GetType().Name,

						SwitchExtensions.Case<MemberAccessExpressionSyntax>("MemberAccessExpressionSyntax", (memberAccessExpression) =>
						{
							var identifiers = memberAccessExpression.GetMemberIdentifiers();
							var joined = string.Empty;

							foreach (var identifier in identifiers)
							{
								if (identifier is IdentifierNameSyntax)
								{
									var identifierName = (IdentifierNameSyntax)identifier;
									var identifierText = identifierName.Identifier.Text;
									joined = joined.Append(identifierText.PrependIf(".", !joined.IsNullOrEmpty()));

									if (joined.StartsWith("Model.Entity."))
									{
										var parentBase = (IParentBase)baseObject;
										IBase childObject = null;

										if (parentBase is Entity_Set)
										{
											baseObject = parentBase.ChildElements.Single();
											parentBase = (IParentBase)baseObject;

											modelObjectGraph.Add(parentBase);
										}

										childObject = parentBase.ChildNodes.Single(b => b.Name == identifierText);
										modelObjectGraph.Add(childObject);
									}
								}
								else
								{
									DebugUtils.Break();
								}
							}

							DebugUtils.NoOp();
						}),
						SwitchExtensions.CaseElse(() =>
						{
							var a = modelArg;
							var t = modelArg.GetType().Name;

							// implementation here or throw error

							DebugUtils.Break();
						})
					);
				}

				if (viewDataArg != null)
				{
					SwitchExtensions.Switch(viewDataArg, () => viewDataArg.GetType().Name,

						SwitchExtensions.Case<ObjectCreationExpressionSyntax>("ObjectCreationExpressionSyntax", (objectCreationExpression) =>
						{
							var typeName = objectCreationExpression.Type.GetName();
							var argList = objectCreationExpression.ArgumentList.Arguments;
							var initializer = objectCreationExpression.Initializer;

							if (typeName == "ViewDataDictionary")
							{
								var includeOurViewData = false;

								if (argList.Count > 0)
								{
									var sourceArg = argList.Single();
									var sourceArgExpression = sourceArg.Expression;

									SwitchExtensions.Switch(sourceArgExpression, () => sourceArgExpression.GetType().Name,

										SwitchExtensions.Case<MemberAccessExpressionSyntax>("MemberAccessExpressionSyntax", (memberAccessExpression) =>
										{
											if (memberAccessExpression.HasThisExpression())
											{
												var identifiers = memberAccessExpression.GetMemberIdentifiers(true);

												if (identifiers.Count() == 1)
												{
													var identifier = identifiers.Single();

													if (identifier is IdentifierNameSyntax)
													{
														var identifierName = (IdentifierNameSyntax)identifier;
														var identifierText = identifierName.Identifier.Text;

														if (identifierText == "ViewData")
														{
															includeOurViewData = true;
														}
														else
														{
															DebugUtils.Break();
														}
													}
													else
													{
														DebugUtils.Break();
													}
												}
											}
											else
											{
												DebugUtils.Break();
											}

											DebugUtils.NoOp();
										}),
										SwitchExtensions.CaseElse(() =>
										{
											var a = sourceArgExpression;
											var t = sourceArgExpression.GetType().Name;

											// implementation here or throw error

											DebugUtils.Break();
										})
									);

									if (initializer != null)
									{
										var kind = initializer.Kind();

										if (kind == SyntaxKind.CollectionInitializerExpression)
										{
											foreach (var expression in initializer.Expressions.Cast<InitializerExpressionSyntax>())
											{
												var expressions = expression.Expressions.ToList();
												var key = ((LiteralExpressionSyntax)expressions[0]).Token.Text.RemoveQuotes();
												var value = ((LiteralExpressionSyntax)expressions[1]).Token.Value;

												viewData.Add(key, value);
											}
										}
										else
										{
											DebugUtils.Break();
										}
									}

									if (includeOurViewData)
									{
										viewData = viewData.Concat(this.ViewData).ToDictionary(k => k.Key, k => k.Value);
									}
								}
							}
							else
							{
								DebugUtils.Break();
							}

							DebugUtils.NoOp();
						}),
						SwitchExtensions.CaseElse(() =>
						{
							var a = viewDataArg;
							var t = viewDataArg.GetType().Name;

							// implementation here or throw error

							DebugUtils.Break();
						})
					);
				}

				var args = new ChildViewHandlerEventArgs(partialViewName, modelObjectGraph, viewData);

				OnChildView(this, args);

				/// kn - you are here, raise OnChildView, handled by GeneratorConfiguration.HandleViews, remove from sortedList
				/// 1. pass args, i.e. Model, ViewDataDictionary
				/// 2. retrieve PartialView component name back
			}
			else
			{
				DebugUtils.Break();
			}

			base.VisitHelperExpression(helperExpressionNode);
		}

		public virtual void AddRange(IBase baseObject, IGeneratorConfiguration generatorConfiguration, List<Module> modules, IEnumerable<Module> addModules, ModuleAddType moduleAddType)
		{
			modules.AddRange(addModules);
		}

		public virtual void AddRange(IBase baseObject, IGeneratorConfiguration generatorConfiguration, List<Module> modules, ModuleAddType moduleAddType, Func<Module, bool> filter)
		{
			var generatorModules = (IEnumerable<ESModule>)generatorConfiguration.KeyValuePairs["Modules"];
			var addModules = generatorModules.Where(m => filter(m));

			modules.AddRange(addModules);

			generatorModules = (IEnumerable<ESModule>)generatorConfiguration.KeyValuePairs["Providers"];
			addModules = generatorModules.Where(m => filter(m));

			modules.AddRange(addModules);
		}
	}
}
