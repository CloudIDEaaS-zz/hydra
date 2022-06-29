using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.State
{
    public class AbstraXProviderEntityEntry : EntityEntry
    {
        public AbstraXProviderEntityEntry(AbstraXProviderInternalEntityEntry internalEntry) : base(internalEntry)
        {
        }

        public override void DetectChanges()
        {
            base.DetectChanges();
        }
    }
}
