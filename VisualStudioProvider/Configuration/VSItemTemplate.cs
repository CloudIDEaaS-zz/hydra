using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CodeInterfaces;

namespace VisualStudioProvider.Configuration
{
    public class VSItemTemplate : VSTemplate, ICodeItemTemplate
    {
        public List<ICodeTemplateProjectItem> ProjectItems { get; private set; }
        public List<ICodeReference> References { get; private set; }

        public VSItemTemplate()
        {
            ProjectItems = new List<ICodeTemplateProjectItem>();
            References = new List<ICodeReference>();
        }

        public override void CopyAndProcess(string copyToPath, ICodeTemplateParameters parameters, bool overwriteExisting = true, List<string> skip = null)
        {
            throw new NotImplementedException();
        }

        public override string ReplaceParameters(string content, ICodeTemplateParameters parameters)
        {
            throw new NotImplementedException();
        }

        public override void ReplaceParameters(Stream stream, ICodeTemplateParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
