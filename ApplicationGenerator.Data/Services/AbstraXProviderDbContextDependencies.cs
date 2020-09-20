using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Utils;

namespace ApplicationGenerator.Data
{
    public partial class AbstraXProviderDataProvider : IDbContextDependencies
    {
        public IDbSetSource SetSource
        {
            get
            {
                return DebugUtils.BreakReturnNull();
            }
        }

        public IEntityFinderFactory EntityFinderFactory
        {
            get
            {
                return DebugUtils.BreakReturnNull();
            }
        }

        public IAsyncQueryProvider QueryProvider
        {
            get
            {
                return DebugUtils.BreakReturnNull();
            }
        }

        public IStateManager StateManager
        {
            get
            {
                return this;
            }
        }

        public IChangeDetector ChangeDetector
        {
            get
            {
                return changeDetector;
            }
        }

        public IEntityGraphAttacher EntityGraphAttacher
        {
            get
            {
                return DebugUtils.BreakReturnNull();
            }
        }

        public IDiagnosticsLogger<DbLoggerCategory.Update> UpdateLogger
        {
            get
            {
                return new AbstraXProviderUpdateLogger();
            }
        }

        public IDiagnosticsLogger<DbLoggerCategory.Infrastructure> InfrastructureLogger
        {
            get
            {
                return new AbstraXProviderInfrastructureLogger();
            }
        }
    }
}