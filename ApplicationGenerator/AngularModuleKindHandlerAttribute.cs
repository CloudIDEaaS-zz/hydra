using AbstraX.DataAnnotations;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public class AngularModuleKindHandlerAttribute : ModuleKindHandlerAttribute
    {
        public AngularModuleKindHandlerAttribute(ModuleKind moduleKind, UIFeatureKind featureKind = UIFeatureKind.None) : base(moduleKind, DefinitionKind.NotApplicable, featureKind)
        {
        }

        public AngularModuleKindHandlerAttribute(ModuleKind moduleKind, DefinitionKind definitionKind, UIFeatureKind featureKind = UIFeatureKind.None) : base(moduleKind, definitionKind, featureKind)
        {
        }

        public AngularModuleKindHandlerAttribute(ModuleKind moduleKind, string kind, UIFeatureKind featureKind = UIFeatureKind.None) : base(moduleKind, kind, featureKind)
        {
        }
    }
}
