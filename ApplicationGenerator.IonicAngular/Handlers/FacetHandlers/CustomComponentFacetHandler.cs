using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Pages.TabPage;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor;
using VisualStudioProvider;
using Utils;
using AbstraX.ViewEngine;

namespace AbstraX.Handlers.FacetHandlers
{
    [FacetHandler(typeof(UICustomAttribute), UIKindGuids.CustomPage)]
    public class CustomComponentFacetHandler : BasePageFacetHandler
    {
        public override bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var uiAttribute = (UICustomAttribute)facet.Attribute;
            var viewRelativePath = uiAttribute.ViewRelativePath;
            var name = baseObject.GetNavigationName();
            var parentObject = (IParentBase)baseObject;
            var tabs = new List<Tab>();
            var pagesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Pages];
            var pagesFolder = (Folder) generatorConfiguration.FileSystem[pagesPath];
            var projectPath = Environment.ExpandEnvironmentVariables(uiAttribute.ProjectPath);
            var viewProjects = generatorConfiguration.ViewProjects;
            ViewProject viewProject;
            View view;

            if (!viewProjects.ContainsKey(projectPath))
            {
                viewProject = new ViewProject(projectPath);
                viewProjects.Add(projectPath, new ViewProject(projectPath));
            }
            else
            {
                viewProject = (ViewProject) viewProjects[projectPath];
            }

            if (!viewProject.ContainsView(viewRelativePath))
            {
                view = viewProject.AddView(viewRelativePath);
            }
            
            if (uiAttribute.UILoadKind == UILoadKind.HomePage)
            {
                this.Raise<ApplicationFacetHandler>();
            }

            generatorConfiguration.HandleViews(viewProject, baseObject, facet);

            return true;
        }
    }
}
