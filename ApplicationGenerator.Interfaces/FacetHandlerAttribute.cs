using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FacetHandlerAttribute : HandlerAttribute
    {
        public Type FacetType { get; private set; }
        public Guid Kind { get; private set; }

        public FacetHandlerAttribute(params Type[] packageTypes)
        {
            this.PackageTypes = packageTypes;
        }

        public FacetHandlerAttribute(Type facetType, string kind, params string[] imports)
        {
            this.FacetType = facetType;
            this.Kind = Guid.Parse(kind);
            this.Imports = imports;
        }

        public FacetHandlerAttribute(Type facetType, string kind)
        {
            this.FacetType = facetType;
            this.Kind = Guid.Parse(kind);
        }
    }
}
