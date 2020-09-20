using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeInterfaces;
using Microsoft.Build.Construction;

namespace VisualStudioProvider
{
    public class VSProjectElement : IVSProjectElement
    {
        protected ProjectElement element;
        public string Condition { get; private set; }
        public string Label { get; private set; }

        public VSProjectElement(Microsoft.Build.Construction.ProjectMetadataElement element)
        {
            this.element = element;
            this.Condition = element.Condition;
            this.Label = element.Label;
        }
    }
}
