using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ApplicationGenerator.Data
{
    public class AbstraXProviderChangeTrackerFactory : ChangeTrackerFactory
    {
        private ICurrentDbContext currentContext;
        private IStateManager stateManager;
        private IChangeDetector changeDetectory;
        private IModel model;
        private IEntityEntryGraphIterator graphIterator;

        public AbstraXProviderChangeTrackerFactory(ICurrentDbContext currentContext, IStateManager stateManager, IChangeDetector changeDetector, IModel model, IEntityEntryGraphIterator graphIterator) : base(currentContext, stateManager, changeDetector, model, graphIterator)
        {
            this.currentContext = currentContext;
            this.stateManager = stateManager;
            this.changeDetectory = changeDetector;
            this.model = model;
            this.graphIterator = graphIterator;
        }

        public override ChangeTracker Create()
        {
            var tracker = base.Create();

            return tracker;
        }
    }
}