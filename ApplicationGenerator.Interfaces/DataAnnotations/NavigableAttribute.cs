using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    public abstract class NavigableAttribute : Attribute
    {
        public string UIHierarchyPath { get; set; }

        public NavigableAttribute(string uiHierarchyPath)
        {
            this.UIHierarchyPath = uiHierarchyPath;
        }
    }
}
