using System;

namespace AbstraX.Contracts
{
    public interface ICodeTemplateProjectItem
    {
        string FileName { get; set; }
        bool ReplaceParameters { get; set; }
        string Folder { get; set; }
        string TargetFileName { get; set; }
    }
}
