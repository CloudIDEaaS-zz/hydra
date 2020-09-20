using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using System.Reflection;

namespace AbstraX.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple =true)]
    public class UIAttribute : Attribute, IFacetHandlerKindAttribute
    {
        public string UIHierarchyPath { get; set; }
        public string PathRootAlias { get; set; }
        public UIKind UIKind { get; set; }
        public UILoadKind UILoadKind { get; set; }
        public bool UseRouter { get; set; }

        public UIAttribute(string uiHierarchyPath, UIKind kind = UIKind.None, bool useRouter = false)
        {
            this.UIHierarchyPath = uiHierarchyPath;
            this.UIKind = kind;
            this.UseRouter = useRouter;
        }

        public UIAttribute(string uiHierarchyPath, string pathRootAlias, UIKind kind = UIKind.None, bool useRouter = false)
        {
            this.UIHierarchyPath = uiHierarchyPath;
            this.UIKind = kind;
            this.PathRootAlias = pathRootAlias;
            this.UseRouter = useRouter;
        }

        public UIAttribute(UIKind kind, UILoadKind loadKind, bool useRouter = false)
        {
            this.UIKind = kind;
            this.UILoadKind = loadKind;
            this.UseRouter = useRouter;
        }

        public Guid Kind
        {
            get
            {
                var field = EnumUtils.GetField(this.UIKind);
                var kindGuidAttribute = field.GetCustomAttribute<KindGuidAttribute>();

                return kindGuidAttribute.Guid;
            }
        }
    }
}
