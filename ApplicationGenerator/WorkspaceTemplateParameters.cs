using CodeInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public class WorkspaceTemplateParameters : ICodeTemplateParameters
    {
        public string ProjectName { get; set; }
        public string FrameworkVersion { get; set; }
        public string RegisteredOrganization { get; set; }
        public string SolutionName { get; set; }
        public string CopyrightYear { get; set; }
    }
}
