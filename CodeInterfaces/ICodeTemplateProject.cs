using System;
using System.Collections.Generic;

namespace CodeInterfaces
{
    public interface ICodeTemplateProject : IWorkspaceTemplate
    {
        List<ICodeTemplateProjectItem> ProjectItems { get; }
    }
}
