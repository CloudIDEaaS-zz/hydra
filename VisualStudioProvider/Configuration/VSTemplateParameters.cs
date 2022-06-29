using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeInterfaces;

namespace VisualStudioProvider.Configuration
{
    public class VSTemplateParameters : ICodeTemplateParameters
    {
        public string AppName { get; set; }
        public string AppDescription { get; set; }
        public string ProjectName { get; set; }
        public string FrameworkVersion { get; set; }
        public string RegisteredOrganization { get; set; }
        public string CopyrightYear { get; set; }
        public string SolutionName { get; set; }
        public Dictionary<string, string> CustomParameters { get; }

        public VSTemplateParameters()
        {
            this.CustomParameters = new Dictionary<string, string>();
        }
    }
}
