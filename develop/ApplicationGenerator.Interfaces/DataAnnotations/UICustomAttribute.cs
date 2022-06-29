using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple =true)]
    public class UICustomAttribute : UIAttribute, IFacetHandlerKindAttribute
    {
        public string ProjectPath { get; }
        public string ViewRelativePath { get; }
        public bool IsPartialOnly { get; }
        public string ViewName { get; }

        public UICustomAttribute(string uiHierarchyPath, string projectPath, string viewRelativePath, bool isPartialOnly = false) : base(uiHierarchyPath, UIKind.CustomPage)
        {
            this.ProjectPath = projectPath;
            this.ViewRelativePath = viewRelativePath;
            this.IsPartialOnly = isPartialOnly;

            this.ViewName = Path.GetFileNameWithoutExtension(viewRelativePath);
        }
    }
}
