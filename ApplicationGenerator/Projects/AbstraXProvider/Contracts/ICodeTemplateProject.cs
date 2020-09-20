using System;
using System.Collections.Generic;

namespace AbstraX.Contracts
{
    public interface ICodeTemplateProject
    {
        string File { get; set; }
        List<ICodeTemplateProjectItem> ProjectItems { get; }
        bool ReplaceParameters { get; set; }
    }
}
