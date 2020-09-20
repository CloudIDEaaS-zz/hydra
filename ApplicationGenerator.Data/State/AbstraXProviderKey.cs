using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.State
{
    public class AbstraXProviderKey : Annotatable, IKey
    {
        public IReadOnlyList<IProperty> Properties { get; }
        public IEntityType DeclaringEntityType { get; }

        public AbstraXProviderKey(IEntityType declaringEntityType, IReadOnlyList<IProperty> keyProperties)
        {
            this.DeclaringEntityType = declaringEntityType;
            this.Properties = keyProperties;
        }
    }
}
