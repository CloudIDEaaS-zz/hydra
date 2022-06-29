using AbstraX.Angular;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Zu.TypeScript;
using Zu.TypeScript.TsTypes;

namespace AbstraX.Generators
{
    public static class ModulesExtractor
    {
        public static List<Module> GetModules(string contents, string fileName)
        {
            var currentAst = new TypeScriptAST();
            var modules = new List<Module>();

            currentAst.MakeAST(contents, fileName);

            foreach (var functionDeclaration in currentAst.RootNode.Children.OfType<FunctionDeclaration>().Where(f => f.Modifiers != null && f.Modifiers.Any(m => m.Kind == SyntaxKind.ExportKeyword)))
            {
                var module = new ExportedFunction(functionDeclaration.IdentifierStr);
                modules.Add(module);
            }

            foreach (var classDeclaration in currentAst.GetDescendants().OfType<ClassDeclaration>())
            {
                Module module = null;

                if (classDeclaration.Decorators != null)
                {
                    foreach (var decorator in classDeclaration.Decorators)
                    {
                        if (decorator.Expression is CallExpression)
                        {
                            var callExpression = (CallExpression)decorator.Expression;

                            switch (callExpression.IdentifierStr)
                            {
                                case "NgModule":
                                    {
                                        module = new AngularModule(classDeclaration.IdentifierStr);
                                        modules.Add(module);
                                    }

                                    break;

                                case "Component":
                                    {
                                        module = new Page(classDeclaration.IdentifierStr, DataAnnotations.UIKind.None);
                                        modules.Add(module);
                                    }

                                    break;

                                case "Pipe":
                                    {
                                        module = new Pipe(classDeclaration.IdentifierStr);
                                        modules.Add(module);
                                    }

                                    break;

                                case "Directive":
                                    {
                                        module = new Directive(classDeclaration.IdentifierStr);
                                        modules.Add(module);
                                    }

                                    break;

                                case "Injectable":
                                    {
                                        module = new Provider(classDeclaration.IdentifierStr);
                                        modules.Add(module);
                                    }

                                    break;

                                default:
                                    {
                                        DebugUtils.Break();
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }
                }

                if (module == null)
                {
                    module = new ESModule(classDeclaration.IdentifierStr);
                    modules.Add(module);
                }
            }

            return modules;
        }
    }
}
