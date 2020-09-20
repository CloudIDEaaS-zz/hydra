using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple =true)]
    public class UINavigationNameAttribute : Attribute
    {
        public string Name { get; set; }
        public UIKind UIKind { get; }
        public UILoadKind UILoadKind { get; }
        public string UIHierarchyPath { get; set; }

        public UINavigationNameAttribute(string name)
        {
            this.Name = name;
        }

        public UINavigationNameAttribute(string name, UIKind kind)
        {
            this.Name = name;
            this.UIKind = kind;
        }

        public UINavigationNameAttribute(string name, UIKind kind, UILoadKind loadKind)
        {
            this.Name = name;
            this.UIKind = kind;
            this.UILoadKind = loadKind;
        }

        public UINavigationNameAttribute(string uiHierarchyPath, string name)
        {
            this.UIHierarchyPath = uiHierarchyPath;
            this.Name = name;
        }
    }
}
