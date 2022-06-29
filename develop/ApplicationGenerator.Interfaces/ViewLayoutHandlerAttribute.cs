using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewLayoutHandlerAttribute : HandlerAttribute
    {
        public string ViewLayout { get; private set; }

        public ViewLayoutHandlerAttribute(string viewLayout, params Type[] packageTypes)
        {
            this.ViewLayout = viewLayout;
            this.PackageTypes = packageTypes;
        }

        public ViewLayoutHandlerAttribute(string viewLayout, params string[] imports)
        {
            this.ViewLayout = viewLayout;
            this.Imports = imports;
        }
    }
}
