using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml;
using CodeInterfaces;
using System.Text;
using Metaspec;
using System.Text.RegularExpressions;
using System.Collections;
using Microsoft.Build.Construction;

namespace VisualStudioProvider
{
    [DebuggerDisplay("{DebugInfo}")]
    public class VSProjectProperty : IVSProjectProperty
    {
        static readonly Type s_ProjectPropertyElement;
        static readonly PropertyInfo s_ProjectPropertyElement_Name;
        static readonly PropertyInfo s_ProjectPropertyElement_Value;
        static readonly PropertyInfo s_ProjectPropertyElement_XmlElement;
        public string Name { get; private set; }
        public string Value { get; private set; }
        public XmlElement XmlElement { get; private set; }

        private VSProject project;
        private object internalProjectProperty;

        static VSProjectProperty()
        {
            s_ProjectPropertyElement = Type.GetType("Microsoft.Build.Construction.ProjectPropertyElement, Microsoft.Build, Version=15.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);

            s_ProjectPropertyElement_Name = s_ProjectPropertyElement.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
            s_ProjectPropertyElement_Value = s_ProjectPropertyElement.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            s_ProjectPropertyElement_XmlElement = s_ProjectPropertyElement.GetProperty("XmlElement", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public string DebugInfo
        {
            get
            {
                return string.Format("{0}='{1}'", this.Name, this.Value);
            }
        }

        public IVSProject ParentProject
        {
            get
            {
                return project;
            }
        }

        public XmlElement ParentElement
        {
            get
            {
                return this.XmlElement.ParentNode as XmlElement;
            }
        }

        public object InternalProjectProperty
        {
            get 
            {
                return internalProjectProperty; 
            }
        }

        public VSProjectProperty(VSProject project, ProjectPropertyElement internalProjectProperty)
        {
            this.Name = (string) s_ProjectPropertyElement_Name.GetValue(internalProjectProperty, null) as string;
            this.Value = (string)s_ProjectPropertyElement_Value.GetValue(internalProjectProperty, null) as string;
            this.XmlElement = (XmlElement)s_ProjectPropertyElement_XmlElement.GetValue(internalProjectProperty, null) as XmlElement;
            this.project = project;
            this.internalProjectProperty = internalProjectProperty;
        }
    }
}
