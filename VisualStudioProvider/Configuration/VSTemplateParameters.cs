using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeInterfaces;

namespace VisualStudioProvider.Configuration
{
    public class VSTemplateParameters : ICodeTemplateParameters
    {
        public string ProjectName { get; set; }
        public string FrameworkVersion { get; set; }
        public string RegisteredOrganization { get; set; }
        public string CopyrightYear { get; set; }
        public string SolutionName { get; set; }
    }
}
