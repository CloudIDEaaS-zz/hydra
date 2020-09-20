using AbstraX.DataAnnotations;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class ModuleKindHandlerAttribute : Attribute
    {
        public Enum ModuleKind { get; private set; }
        public DefinitionKind DefinitionKind { get; private set; }
        public Guid UIKind { get; private set; }
        public UIFeatureKind FeatureKind { get; private set; }

        public ModuleKindHandlerAttribute(Enum moduleKind, DefinitionKind definitionKind, UIFeatureKind featureKind)
        {
            this.ModuleKind = moduleKind;
            this.DefinitionKind = definitionKind;
            this.FeatureKind = featureKind;
        }

        public ModuleKindHandlerAttribute(Enum moduleKind, string kind, UIFeatureKind featureKind)
        {
            this.ModuleKind = moduleKind;
            this.UIKind = Guid.Parse(kind);
            this.FeatureKind = featureKind;
        }
    }
}
