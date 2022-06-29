using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApplicationGenerator.State;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Utils;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ApplicationGenerator.Data
{
    public partial class AbstraXProviderDataProvider : IStateManager
    {
        public CascadeTiming DeleteOrphansTiming { get; set; }
        public CascadeTiming CascadeDeleteTiming { get; set; }
        public int ChangedCount { get; set; }
        public event EventHandler<EntityTrackedEventArgs> Tracked;
        public event EventHandler<EntityStateChangedEventArgs> StateChanged;
        private StateManagerDependencies stateManagerDependencies;

        public StateManagerDependencies Dependencies
        {
            get
            {
                return this.stateManagerDependencies;
            }
        }

        private IIdentityMap GetOrCreateIdentityMap(IKey key)
        {
            if (!identityMaps.TryGetValue(key, out var identityMap))
            {
                identityMap = new IdentityMap<object[]>(key, new CompositePrincipalKeyValueFactory(key), true);
                identityMaps[key] = identityMap;
            }

            return identityMap;
        }

        private void UpdateReferenceMaps(InternalEntityEntry entry, EntityState state, EntityState? oldState)
        {
            var entityType = entry.EntityType;
            var mapKey = entry.Entity ?? entry;

            if (entityType.HasDefiningNavigation())
            {
                foreach (var otherType in this.model.GetEntityTypes(entityType.Name).Where(et => et != entityType && TryGetEntry(mapKey, et) != null))
                {
                    UpdateLogger.DuplicateDependentEntityTypeInstanceWarning(entityType, otherType);
                }
            }

            entityReferenceMap.Update(entry, state, oldState);
        }

        public IEnumerable<InternalEntityEntry> Entries
        {
            get
            {
                return this.GetEntriesForState(true, true, true, true);
            }
        }

        public int Count
        {
            get
            {
                return DebugUtils.BreakReturn(0);
            }
        }

        public IInternalEntityEntryNotifier InternalEntityEntryNotifier
        {
            get
            {
                return this.internalEntityEntryNotifier;
            }
        }

        public IValueGenerationManager ValueGenerationManager
        {
            get
            {
                return DebugUtils.BreakReturnNull();
            }
        }

        public IEntityMaterializerSource EntityMaterializerSource
        {
            get
            {
                return DebugUtils.BreakReturnNull();
            }
        }

        public bool SensitiveLoggingEnabled
        {
            get
            {
                return false;
            }
        }

        public bool SavingChanges { get; }

        public void AcceptAllChanges()
        {
        }

        public void CascadeChanges(bool force)
        {
        }

        public void CascadeDelete(InternalEntityEntry entry, bool force, IEnumerable<IForeignKey> foreignKeys = null)
        {
        }

        public IEntityFinder CreateEntityFinder(IEntityType entityType)
        {
            return DebugUtils.BreakReturnNull();
        }

        public InternalEntityEntry CreateEntry(IDictionary<string, object> values, IEntityType entityType)
        {
            return DebugUtils.BreakReturnNull();
        }

        public InternalEntityEntry FindPrincipal(InternalEntityEntry dependentEntry, IForeignKey foreignKey)
        {
            return DebugUtils.BreakReturnNull();
        }

        public InternalEntityEntry FindPrincipalUsingPreStoreGeneratedValues(InternalEntityEntry dependentEntry, IForeignKey foreignKey)
        {
            return DebugUtils.BreakReturnNull();
        }

        public InternalEntityEntry FindPrincipalUsingRelationshipSnapshot(InternalEntityEntry dependentEntry, IForeignKey foreignKey)
        {
            return DebugUtils.BreakReturnNull();
        }

        public int GetCountForState(bool added = false, bool modified = false, bool deleted = false, bool unchanged = false)
        {
            var entries = this.GetEntriesForState(added, modified, deleted, unchanged);

            return entries.Count();
        }

        public IEnumerable<InternalEntityEntry> GetDependents(InternalEntityEntry principalEntry, IForeignKey foreignKey)
        {
            return DebugUtils.BreakReturnNull();
        }

        public IEnumerable<InternalEntityEntry> GetDependentsFromNavigation(InternalEntityEntry principalEntry, IForeignKey foreignKey)
        {
            return DebugUtils.BreakReturnNull();
        }

        public IEnumerable<InternalEntityEntry> GetDependentsUsingRelationshipSnapshot(InternalEntityEntry principalEntry, IForeignKey foreignKey)
        {
            return DebugUtils.BreakReturnNull();
        }

        public IEnumerable<InternalEntityEntry> GetEntriesForState(bool added = false, bool modified = false, bool deleted = false, bool unchanged = false)
        {
            return entityReferenceMap.GetEntriesForState(added, modified, deleted, unchanged);
        }

        public IList<IUpdateEntry> GetEntriesToSave(bool cascadeChanges)
        {
            return DebugUtils.BreakReturnNull();
        }

        public IEnumerable<TEntity> GetNonDeletedEntities<TEntity>() where TEntity : class
        {
            return DebugUtils.BreakReturnNull();
        }

        public InternalEntityEntry GetOrCreateEntry(object entity)
        {
            return DebugUtils.BreakReturnNull();
        }

        public InternalEntityEntry GetOrCreateEntry(object entity, IEntityType entityType)
        {
            return DebugUtils.BreakReturnNull();
        }

        public IEnumerable<Tuple<INavigation, InternalEntityEntry>> GetRecordedReferrers(object referencedEntity, bool clear)
        {
            return DebugUtils.BreakReturnNull();
        }

        public void OnStateChanged(InternalEntityEntry internalEntityEntry, EntityState oldState)
        {
        }

        public void OnTracked(InternalEntityEntry internalEntityEntry, bool fromQuery)
        {
            var trackEvent = Tracked;

            if (SensitiveLoggingEnabled)
            {
                changeTrackingLogger.StartedTrackingSensitive(internalEntityEntry);
            }
            else
            {
                changeTrackingLogger.StartedTracking(internalEntityEntry);
            }

            trackEvent?.Invoke(Context.ChangeTracker, new EntityTrackedEventArgs(internalEntityEntry, fromQuery));
        }

        public void RecordReferencedUntrackedEntity(object referencedEntity, INavigation navigation, InternalEntityEntry referencedFromEntry)
        {
        }

        public void ResetState()
        {
        }

        public Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            return DebugUtils.BreakReturnNull();
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            //this.ChangeDetector.DetectChanges(this);

            //foreach (var entry in this.ch)

            return 1;
        }

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return DebugUtils.BreakReturnNull();
        }

        public InternalEntityEntry StartTracking(InternalEntityEntry entry)
        {
            var entityType = (EntityType) entry.EntityType;

            if (entry.StateManager != this)
            {
                throw new InvalidOperationException(CoreStrings.WrongStateManager(entityType.DisplayName()));
            }

            var mapKey = entry.Entity ?? entry;
            var existingEntry = TryGetEntry(mapKey, entityType);

            if (existingEntry != null && existingEntry != entry)
            {
                throw new InvalidOperationException(CoreStrings.MultipleEntries(entityType.DisplayName()));
            }

            foreach (var key in entityType.GetKeys())
            {
                GetOrCreateIdentityMap(key).Add(entry);
            }

            return entry;
        }

        public InternalEntityEntry StartTrackingFromQuery(IEntityType baseEntityType, object entity, in ValueBuffer valueBuffer)
        {
            var existingEntry = TryGetEntry(entity);
            Type clrType;
            IEntityType entityType;
            AbstraXProviderInternalEntityEntry newEntry;

            if (existingEntry != null)
            {
                return existingEntry;
            }

            clrType = entity.GetType();
            entityType = baseEntityType.ClrType == clrType || baseEntityType.HasDefiningNavigation() ? baseEntityType : model.FindRuntimeEntityType(clrType);
            newEntry = valueBuffer.IsEmpty ? new AbstraXProviderInternalEntityEntry(entity, this.StateManager, baseEntityType) : new AbstraXProviderInternalEntityEntry(entity, this.StateManager, baseEntityType, valueBuffer);

            foreach (var key in baseEntityType.GetKeys())
            {
                GetOrCreateIdentityMap(key).AddOrUpdate(newEntry);
            }

            UpdateReferenceMaps(newEntry, EntityState.Unchanged, null);

            newEntry.MarkUnchangedFromQuery();

            return newEntry;
        }

        public void StateChanging(InternalEntityEntry entry, EntityState newState)
        {
            InternalEntityEntryNotifier.StateChanging(entry, newState);

            UpdateReferenceMaps(entry, newState, entry.EntityState);
        }

        public void StopTracking(InternalEntityEntry entry, EntityState oldState)
        {
            var entityType = entry.EntityType;

            foreach (var key in entityType.GetKeys())
            {
                FindIdentityMap(key)?.Remove(entry);
            }
        }

        private IIdentityMap FindIdentityMap(IKey key)
        {
            return identityMaps == null || !identityMaps.TryGetValue(key, out var identityMap) ? null : identityMap;
        }

        public InternalEntityEntry TryGetEntry(IKey key, object[] keyValues)
        {
            return DebugUtils.BreakReturnNull();
        }

        public InternalEntityEntry TryGetEntry(IKey key, object[] keyValues, bool throwOnNullKey, out bool hasNullKey)
        {
            hasNullKey = false;
            return DebugUtils.BreakReturnNull();
        }

        public InternalEntityEntry TryGetEntry(object entity, bool throwOnNonUniqueness = true)
        {
            return entityReferenceMap.TryGet(entity, null, out var entry, throwOnNonUniqueness) ? entry : null;
        }

        public InternalEntityEntry TryGetEntry(object entity, IEntityType type, bool throwOnTypeMismatch = true)
        {
            return DebugUtils.BreakReturnNull();
        }

        public void Unsubscribe()
        {
        }

        public void UpdateDependentMap(InternalEntityEntry entry, IForeignKey foreignKey)
        {
        }

        public void UpdateIdentityMap(InternalEntityEntry entry, IKey principalKey)
        {
        }
    }
}