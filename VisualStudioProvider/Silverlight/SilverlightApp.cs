using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeInterfaces;

namespace VisualStudioProvider.Silverlight
{
    public class SilverlightApp : ISilverlightApp
    {
        public Guid ProjectGuid { get; private set; }
        public IVSProject SilverlightProject { get; private set; }
        public string PathInWeb { get; private set; }
        public bool ConfigurationSpecificFolders { get; private set; }

        public SilverlightApp(Guid projectGuid, IVSProject silverlightProject, string pathInWeb, bool configurationSpecificFolders)
        {
            this.ProjectGuid = projectGuid;
            this.SilverlightProject = silverlightProject;
            this.PathInWeb = pathInWeb;
            this.ConfigurationSpecificFolders = configurationSpecificFolders;
        }
    }
}
