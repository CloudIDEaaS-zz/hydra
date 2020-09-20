using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class UINavigationAttribute : NavigableAttribute
    {
        public string Icon { get; set; }

        public UINavigationAttribute(string uiHierarchyPath, string icon = null) : base(uiHierarchyPath)
        {
            this.Icon = icon;
        }
    }
}
