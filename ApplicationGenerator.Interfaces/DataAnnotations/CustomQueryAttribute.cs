using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    public class CustomQueryAttribute : ResourcesAttribute
    {
        public string ControllerMethodName { get; }
        public QueryKind QueryKind { get; }

        public CustomQueryAttribute(Type resourcesType, string controllerMethodName, QueryKind kind = QueryKind.None) : base(resourcesType)
        {
            this.ControllerMethodName = controllerMethodName;
            this.QueryKind = kind;
        }
    }
}
