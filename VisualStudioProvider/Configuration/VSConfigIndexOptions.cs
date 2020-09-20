using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.Configuration
{
    public class VSConfigIndexOptions
    {
        public bool LoadEnvironmentServices { get; set; }
        public bool IndexProjectTemplates { get; set; }
        public string ProjectTemplatesFolderRegex { get; set; }
        public bool IndexItemTemplates { get; set; }
        public bool IndexPackages { get; set; }
    }
}
