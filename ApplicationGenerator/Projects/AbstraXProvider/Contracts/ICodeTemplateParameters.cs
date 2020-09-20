using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX.Contracts
{
    public interface ICodeTemplateParameters
    {
        string ProjectName { get; set; }
        string FrameworkVersion { get; set; }
        string RegisteredOrganization { get; set; }
        string CopyrightYear { get; set; }
    }
}
