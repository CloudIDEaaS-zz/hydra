using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    public class ResourcesAttribute : Attribute
    {
        public Type ResourcesType { get; }

        public ResourcesAttribute(Type resourcesType)
        {
            this.ResourcesType = resourcesType;
        }

        public ResourcesHandler CreateHandler()
        {
            return new ResourcesHandler((IAppResources)Activator.CreateInstance(this.ResourcesType));
        }

    }
}
