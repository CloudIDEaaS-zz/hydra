using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeInterfaces;
using Microsoft.Build.Construction;

namespace VisualStudioProvider
{
    public class VSProjectMetadataElement : VSProjectElement, IVSProjectMetadataElement
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public VSProjectMetadataElement(ProjectMetadataElement element) : base(element)
        {
            this.Name = element.Name;
            this.Value = element.Value;
        }
    }
}
