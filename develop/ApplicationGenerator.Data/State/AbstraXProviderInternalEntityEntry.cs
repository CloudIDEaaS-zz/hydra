using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.State
{
    public class AbstraXProviderInternalEntityEntry : InternalEntityEntry
    {
        public override object Entity { get; }
        public ValueBuffer ValueBuffer { get; }

        public AbstraXProviderInternalEntityEntry(object entity, IStateManager stateManager, IEntityType entityType, ValueBuffer valueBuffer = default(ValueBuffer)) : base(stateManager, entityType)
        {
            this.Entity = entity;
            this.ValueBuffer = valueBuffer;

            EnsureOriginalValues();
        }

        public string Item
        {
            get
            {
                return Entity.ToString();
            }
        }
    }
}
