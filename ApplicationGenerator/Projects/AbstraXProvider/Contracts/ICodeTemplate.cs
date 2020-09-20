using System;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using System.Drawing;

namespace AbstraX.Contracts
{
    public enum TemplateType
    {
        ProjectGroup,
        Project,
        Item,
        Custom
    }

    public interface ICodeTemplate
    {
        string TemplateID { get; set; }
        string TemplateGroupID { get; set; }
        string DefaultName { get; set; }
        string Description { get; set; }
        string FullName { get; }
        string Name { get; set; }
        string TemplateLocation { get; }
        Icon Icon { get; set; }
        int SortOrder { get; set; }
        string ProjectTypeName { get; set; }
        string ProjectSubTypeName { get; set; }
        XDocument TemplateDocument { get; set; }
        TemplateType TemplateType { get; }
        string TypeName { get; set; }
        FileInfo ZippedTemplate { get; set; }
        void CopyAndProcess(string copyToPath, ICodeTemplateParameters parameters, bool overwriteExisting = true, List<string> skip = null);
    }

    public interface ICodeProjectTemplate : ICodeTemplate
    {
        List<ICodeTemplateProject> Projects { get; }
    }

    public interface ICodeItemTemplate : ICodeTemplate
    {
        List<ICodeTemplateProjectItem> ProjectItems { get; }
        List<ICodeReference> References { get; }
    }
}
