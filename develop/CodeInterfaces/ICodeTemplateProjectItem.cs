using System;

namespace CodeInterfaces
{
    public interface ICodeTemplateProjectItem : IWorkspaceTemplate
    {
        string Folder { get; set; }
        string TargetFileName { get; set; }
        string RelativePath { get; }
        string SubType { get; set; }
    }
}
