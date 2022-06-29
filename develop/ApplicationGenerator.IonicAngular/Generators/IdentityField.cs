using AbstraX.DataAnnotations;
using AbstraX.Generators.Base;
using AbstraX.ServerInterfaces;
using AbstraX.TypeMappings;
using AbstraX.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Generators
{
    [DebuggerDisplay(" { Title } ")]
    public class IdentityField : FormField
    {
        public IdentityFieldKind IdentityFieldKind { get; }

        public IdentityField(IAttribute attribute, Facet entityFacet, IGeneratorConfiguration generatorConfiguration) : base(attribute, entityFacet, generatorConfiguration)
        {
            var identityFieldAttribute = attribute.GetFacetAttribute<IdentityFieldAttribute>();

            this.IdentityFieldKind = identityFieldAttribute.IdentityFieldKind;
        }
    }
}
