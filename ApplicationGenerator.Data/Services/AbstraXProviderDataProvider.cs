using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;
using Utils;
using Microsoft.Extensions.DependencyInjection;
using ApplicationGenerator.Data.Interfaces;
using ApplicationGenerator.State;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ApplicationGenerator.Data
{
    public partial class AbstraXProviderDataProvider : DatabaseProvider<AbstraXProviderContextOptionsExtension>, IDbSetInitializer, IDbContextServices, ICurrentDbContext, ISingletonOptionsInitializer, IClrPropertySetter
    {
        private AbstraXProviderDataContext providerContext;
        private IServiceProvider serviceProvider;
        private IDbContextOptions contextOptions;
        private IInternalEntityEntryNotifier internalEntityEntryNotifier;
        private IDiagnosticsLogger<DbLoggerCategory.ChangeTracking> changeTrackingLogger;
        private IChangeDetector changeDetector;
        private IServiceCollection services;
        private Model model;
        private Dictionary<IKey, IIdentityMap> identityMaps;
        private readonly EntityReferenceMap entityReferenceMap;
        private List<DbSetProperty> dbSets;

        public AbstraXProviderDataProvider(DatabaseProviderDependencies dependencies, IServiceCollection services) : base(dependencies)
        {
            this.services = services;
            this.identityMaps = new Dictionary<IKey, IIdentityMap>();
            this.entityReferenceMap = new EntityReferenceMap(hasSubMap: true);
            dbSets = new List<DbSetProperty>();
        }

        public IDbContextServices Initialize(IServiceProvider scopedProvider, IDbContextOptions contextOptions, DbContext context)
        {
            this.serviceProvider = scopedProvider;
            this.contextOptions = contextOptions;
            this.providerContext = (AbstraXProviderDataContext)context;

            IInternalEntityEntryNotifier internalEntityEntryNotifier = scopedProvider.GetService<IInternalEntityEntryNotifier>();

            return this;
        }

        public void EnsureInitialized(IServiceProvider serviceProvider, IDbContextOptions options)
        {
            this.serviceProvider = serviceProvider;
            this.contextOptions = options;

            internalEntityEntryNotifier = serviceProvider.GetService<IInternalEntityEntryNotifier>();
            changeTrackingLogger = serviceProvider.GetService<IDiagnosticsLogger<DbLoggerCategory.ChangeTracking>>();
            changeDetector = serviceProvider.GetService<IChangeDetector>();
            
        }

        public void InitializeSets(DbContext context)
        {
            var contextType = context.GetType();
            var properties = contextType.GetProperties().Where(p => p.PropertyType.Name == typeof(DbSet<>).Name);
            
            this.providerContext = (AbstraXProviderDataContext)context;

            foreach (var property in properties)
            {
                var genericType = property.PropertyType.GenericTypeArguments.Single();
                var abstraXProviderDbSetType = typeof(AbstraXProviderDbSet<>);
                var abstraXProviderDbSetGenericType = abstraXProviderDbSetType.MakeGenericType(genericType);
                var abstraXProviderDbSetConstructor = abstraXProviderDbSetGenericType.GetConstructor(new Type[] { typeof(AbstraXProviderDataProvider), typeof(AbstraXProviderDataContext), typeof(string) });
                var abstraXProviderDbSet = abstraXProviderDbSetConstructor.Invoke(new object[] { this, context, property.Name });
                var dbSetProperty = new DbSetProperty(property.Name, genericType, this, false);

                dbSets.Add(dbSetProperty);

                property.SetValue(context, abstraXProviderDbSet);
            }
        }

        public DbContext Context
        {
            get
            {
                return this.providerContext;
            }
        }

        public ICurrentDbContext CurrentContext
        {
            get
            {
                return this;
            }
        }

        public IModel Model
        {
            get
            {
                if (this.model == null)
                {
                    this.model = (Model)this.CreateModel();
                }

                return this.model;
            }
        }

        private IModel CreateModel()
        {
            return serviceProvider.GetService<IModelSource>().GetModel(CurrentContext.Context, serviceProvider.GetService<IConventionSetBuilder>());
        }

        public void SetClrValue(object instance, object value)
        {
        }

        public IDbContextOptions ContextOptions
        {
            get
            {
                return this.contextOptions;
            }
        }

        public IServiceProvider InternalServiceProvider
        {
            get
            {
                return this.serviceProvider;
            }
        }

        public object this[string name]
        {
            get
            {
                return "false";
            }
        }
    }
}
