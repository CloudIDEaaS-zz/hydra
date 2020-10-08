using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics;
using CodeInterfaces;
using System.Text.RegularExpressions;
using Utils;
using System.Drawing;

namespace VisualStudioProvider.Configuration
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public abstract class VSTemplate : ICodeTemplate
    {
        public string TemplateID { get; set; }
        public string TemplateGroupID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Icon Icon { get; set; }
        public string DefaultName { get; set; }
        public string RequiredFrameworkVersion { get; set; }
        public string TypeName { get; set; }
        public string ProjectTypeName { get; set; }
        public string ProjectSubTypeName { get; set; }
        public int SortOrder { get; set; }
        public FileInfo ZippedTemplate { get; set; }
        public DirectoryInfo CommonLocation { get; set; }
        public XDocument TemplateDocument { get; set; }
        public string SubType { get; set; }
        public abstract void CopyAndProcess(string copyToPath, ICodeTemplateParameters parameters, bool overwriteExisting = true, List<string> skip = null);
        public abstract string ReplaceParameters(string content, ICodeTemplateParameters parameters);
        public abstract void ReplaceParameters(Stream stream, ICodeTemplateParameters parameters);
        protected ICodeTemplateParameters parameters;

        public VSTemplate()
        {
        }

        public string DebugInfo
        {
            get
            {
                if (this.ZippedTemplate != null)
                {
                    return this.ZippedTemplate.FullName;
                }
                else
                {
                    return this.Name;
                }
            }
        }

        public string TemplateLocation
        {
            get
            {
                return ZippedTemplate.Directory.FullName;
            }
        }

        public TemplateType TemplateType
        {
            get
            {
                return (TemplateType) Enum.Parse(typeof(TemplateType), this.TypeName);
            }
        }

        public string FullName
        {
            get
            {
                return string.Format("{0} - {1}", this.Name, this.Description);
            }
        }

        public static string ReplaceParameterText(string text, ICodeTemplateParameters parameters)
        {
            for (var x = 0; x < 255; x++)
            {
                text = text.Replace($"$guid{ x }$", Guid.NewGuid().ToString());
            }

            text = text.Replace("$appname$", parameters.AppName);
            text = text.Replace("$appdescription$", parameters.AppDescription);
            text = text.Replace("$projectname$", parameters.ProjectName);
            text = text.Replace("$safeprojectname$", parameters.ProjectName);
            text = text.Replace("$specifiedsolutionname$", parameters.SolutionName);
            text = text.Replace("$targetframeworkversion$", parameters.FrameworkVersion);
            text = text.Replace("$registeredorganization$", parameters.RegisteredOrganization);
            text = text.Replace("$year$", parameters.CopyrightYear);

            return text;
        }
    }
}
