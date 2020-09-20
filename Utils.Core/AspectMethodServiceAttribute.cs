using PostSharp.Aspects;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Text;

namespace Utils
{

    [Serializable]
    public class AspectMethodServiceAttribute : OnMethodBoundaryAspect
    {
        private Type[] serviceTypes;
        private AspectServiceKinds serviceKinds;
        [NonSerialized, Import] private IAspectServiceImporter aspectServiceImporter;

        public AspectMethodServiceAttribute(params Type[] serviceTypes)
        {
            this.serviceTypes = serviceTypes;
        }

        public AspectMethodServiceAttribute(AspectServiceKinds serviceKinds)
        {
            this.serviceKinds = serviceKinds;
        }

        public override void RuntimeInitialize(MethodBase method)
        {
            AspectServiceExtensions.BuildObject(this);

            base.RuntimeInitialize(method);
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            Console.WriteLine("The {0} method has been entered.", args.Method.Name);
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            Console.WriteLine("The {0} method executed successfully.", args.Method.Name);
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            Console.WriteLine("The {0} method has exited.", args.Method.Name);
        }

        public override void OnException(MethodExecutionArgs args)
        {
            Console.WriteLine("An exception was thrown in {0}.", args.Method.Name);
        }
    }
}
