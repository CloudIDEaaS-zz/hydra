using AbstraX.Angular;
using AbstraX.Generators.Pages.SassPage;
using AbstraX.Generators.Pages.TabPage;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static AbstraX.FolderStructure.FileSystemObject;

namespace AbstraX.Generators.Pages.NavigationGridPage
{
    public static class NavigationGridPageGenerator
    {
        public static void GeneratePage(IBase baseObject, string pagesFolderPath, string pageName, IGeneratorConfiguration generatorConfiguration, IModuleAssembly module, IEnumerable<ModuleImportDeclaration> imports, IModuleAssembly angularModule, List<ManagedList> managedLists, List<GridColumn> gridColumns, bool isComponent)
        {
            var host = new TemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var moduleAssemblyProperties = new AngularModuleAssemblyProperties(baseObject, imports);
            var parentElement = (IParentBase)baseObject;
            var childElement = parentElement.ChildElements.Single();
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // grid page

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("EntityName", childElement.Name);
                sessionVariables.Add("IsComponent", isComponent);

                fileLocation = PathCombine(pagesFolderPath, childElement.Name.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".html");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<NavigationGridPageTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Navigation Grid Page"));

                // grid class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("Authorize", generatorConfiguration.AuthorizedRoles);
                sessionVariables.Add("EntityName", childElement.Name);
                sessionVariables.Add("ParentEntityName", parentElement.Name);
                sessionVariables.Add("IsComponent", isComponent);

                if (baseObject is IRelationProperty)
                {
                    var navigationProperty = (IRelationProperty)baseObject;
                    var thisPropertyRef = navigationProperty.ThisPropertyRef;
                    var parentPropertyRef = navigationProperty.ParentPropertyRef;
                    var thisPropertyType = thisPropertyRef.GetScriptTypeName();
                    var thisShortType = thisPropertyRef.GetShortType().ToLower();

                    switch (navigationProperty.ThisMultiplicity)
                    {
                        case "*":
                            sessionVariables.Add("EntityParentRefName", baseObject.Parent.Name);
                            sessionVariables.Add("EntityPropertyRefName", thisPropertyRef.Name);

                            break;
                        case "0..1":
                        case "1":
                            DebugUtils.Break();
                            break;
                        default:
                            DebugUtils.Break();
                            break;
                    }
                }

                sessionVariables.Add("ManagedLists", managedLists);
                sessionVariables.Add("GridColumns", gridColumns);

                if (generatorConfiguration.CustomQueries.ContainsKey(baseObject))
                {
                    var queriesList = generatorConfiguration.CustomQueries[baseObject];

                    sessionVariables.Add("CustomQueries", queriesList);
                }

                sessionVariables.AddModuleAssemblyProperties(moduleAssemblyProperties);

                filePath = PathCombine(fileLocation, pageName.ToLower() + ".ts");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<NavigationGridClassTemplate>(sessionVariables, false);

                module.ExportedComponents = sessionVariables.GetExportedComponents();
                module.ForChildFile = generatorConfiguration.CreateFile(moduleAssemblyProperties, fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Navigation Grid Class"));

                // sass

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("PageName", pageName);

                filePath = PathCombine(fileLocation, pageName.ToLower() + ".scss");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<SassStyleSheetTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "page \r\n{\r\n}"));
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
