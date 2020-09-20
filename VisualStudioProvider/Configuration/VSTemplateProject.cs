using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeInterfaces;

namespace VisualStudioProvider.Configuration
{
    public class VSTemplateProject : ICodeTemplateProject
    {
        public List<ICodeTemplateProjectItem> ProjectItems { get; private set; }
        public string FileName { get; set; }
        public bool ReplaceParameters { get; set; }

        public VSTemplateProject()
        {
            ProjectItems = new List<ICodeTemplateProjectItem>();
        }

        public string RelativePath
        {
            get
            {
                return @"\" + this.FileName;
            }
        }

    }
}
