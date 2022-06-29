using AbstraX;
using AbstraX.Projects;
using Hydra.Designer.Shell;
using Hydra.Shell.Implementations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignerPageWizard
{
    public class ProjectItemTypeWizard : IProjectItemTypeWizard
    {
        private IServiceProvider serviceProvider;

        public ProjectItemTypeWizard(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            var itemFind = Path.GetFileNameWithoutExtension(projectItem.Name) + ".css";
            var solution = (HydraSolution) serviceProvider.GetService(typeof(HydraSolution));
            var stylesFolder = solution.RootFolder.Folders.SingleOrDefault(f => f.Name == "styles");
            var existingStylesheet = stylesFolder.Files.SingleOrDefault(f => f.Name == itemFind);

            if (existingStylesheet == null)
            {
                solution.CreateProjectItemFromTemplate(stylesFolder, 0, itemFind, "Stylesheet", projectItem.UserCreated);
            }
        }

        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams, Project project, uint parentItemId)
        {
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
