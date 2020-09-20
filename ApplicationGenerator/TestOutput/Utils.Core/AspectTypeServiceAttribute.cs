using PostSharp.Aspects;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Utils
{
    [Serializable]
    public class AspectTypeServiceAttribute : TypeLevelAspect, IAspectProvider
    {
        private Type[] serviceTypes;
        private AspectServiceKinds serviceKinds;
        [NonSerialized, Import] private IAspectServiceImporter aspectServiceImporter;

        public AspectTypeServiceAttribute(params Type[] serviceTypes)
        {
            this.serviceTypes = serviceTypes;
        }

        public AspectTypeServiceAttribute(AspectServiceKinds serviceKinds)
        {
            this.serviceKinds = serviceKinds;
        }

        public override void RuntimeInitialize(Type type)
        {
            AspectServiceExtensions.BuildObject(this);

            base.RuntimeInitialize(type);
        }

        public IEnumerable<AspectInstance> ProvideAspects(object targetElement)
        {
            var type = (Type)targetElement;

            return type.GetMethods().Cast<MethodInfo>().Select(m => new AspectInstance(m, new AspectMethodServiceAttribute()));
        }
    }
}
