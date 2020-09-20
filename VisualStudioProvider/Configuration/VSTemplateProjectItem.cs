using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CodeInterfaces;
using Utils;

namespace VisualStudioProvider.Configuration
{
    [DebuggerDisplay(" { FileName } ")]
    public class VSTemplateProjectItem : ICodeTemplateProjectItem
    {
        public bool ReplaceParameters { get; set; }
        public string FileName { get; set; }
        public string Folder { get; set; }
        public string SubType { get; set; }
        public string TargetFileName { get; set; }

        public string RelativePath
        {
            get
            {
                if (this.Folder.IsNullOrEmpty())
                {
                    return @"\" + this.FileName;
                }
                else
                {
                    return @"\" + this.Folder + @"\" + this.FileName;
                }
            }
        }
    }
}
