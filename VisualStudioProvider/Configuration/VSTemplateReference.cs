using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeInterfaces;

namespace VisualStudioProvider.Configuration
{
    public class VSTemplateReference : ICodeReference
    {
        public string Package { get; set; }

        public VSTemplateReference(string package)
        {
            this.Package = package;
        }
    }
}
