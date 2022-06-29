using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Storage;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Utils;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.Caching.Memory;

namespace ApplicationGenerator.Data
{
    public partial class AbstraXProviderContextOptionsExtension : IDbContextOptionsExtension
    {
        public IServiceCollection Services { get; private set; }
        public AbstraXProviderDataProvider Provider { get; private set; }

        public AbstraXProviderContextOptionsExtension()
        {
        }

        public long GetServiceProviderHashCode()
        {
            return typeof(AbstraXProviderContextOptionsExtension).FullName.GetHashCode();
        }

        public void Validate(IDbContextOptions options)
        {
        }

        public void ApplyServices(IServiceCollection services)
        {
            services.AddScoped<DatabaseProviderDependencies>();
            services.AddScoped<IChangeTrackerFactory, AbstraXProviderChangeTrackerFactory>();

            services.AddScoped<ISingletonOptionsInitializer, AbstraXProviderDataProvider>((p) =>
            {
                var dependencies = p.GetRequiredService<DatabaseProviderDependencies>();

                this.Provider = new AbstraXProviderDataProvider(dependencies, services);

                return this.Provider;
            });

            services.AddScoped<IDbContextOptions, IDbContextOptions>(p => this.Provider.ContextOptions);
            services.AddScoped<IDbContextDependencies, AbstraXProviderDataProvider>(p => this.Provider);
            services.AddScoped<IDbSetInitializer, AbstraXProviderDataProvider>(p => this.Provider);
            services.AddScoped<IDatabaseProvider, AbstraXProviderDataProvider>(p => this.Provider);
            services.AddScoped<ICurrentDbContext, AbstraXProviderDataProvider>(p => this.Provider);
            services.AddScoped<IDbContextServices, AbstraXProviderDataProvider>(p => this.Provider);
            services.AddScoped<ITypeMappingSource, AbstraXProviderDataProvider>(p => this.Provider);
            services.AddScoped<IDbSetFinder, AbstraXProviderDataProvider>(p => this.Provider);
            services.AddScoped<IEntityGraphAttacher, EntityGraphAttacher>();
            services.AddScoped<IDiagnosticsLogger<DbLoggerCategory.ChangeTracking>, AbstraXProviderChangeTrackingLogger>();
            services.AddScoped<IDiagnosticsLogger<DbLoggerCategory.Model>, AbstraXProviderModelLogger>();
            services.AddScoped<IDiagnosticsLogger<DbLoggerCategory.Model.Validation>, AbstraXProviderModelValidationLogger>();
            services.AddScoped<IUpdateAdapter, UpdateAdapter>();
            services.AddScoped<ILoggingOptions, AbstraXProviderLoggingOptions>();
            services.AddScoped<IModel, Model>(p => (Model) this.Provider.Model);
            services.AddScoped<IStateManager, AbstraXProviderDataProvider>(p => this.Provider);
            services.AddScoped<IChangeDetector, ChangeDetector>();
            services.AddScoped<ILocalViewListener, LocalViewListener>();
            services.AddScoped<INavigationFixer, NavigationFixer>();
            services.AddScoped<IInternalEntityEntryNotifier, InternalEntityEntryNotifier>();
            services.AddScoped<IEntityEntryGraphIterator, EntityEntryGraphIterator>();
            services.AddScoped<IAbstraXProviderServices, AbstraXProviderServices>();
            services.AddScoped<IInternalEntityEntryFactory, InternalEntityEntryFactory>();
            services.AddScoped<IValueGenerationManager, ValueGenerationManager>();
            services.AddScoped<IInternalEntityEntrySubscriber, InternalEntityEntrySubscriber>();
            services.AddScoped<IEntityTypeAddedConvention, OwnedEntityTypeAttributeConvention>();
            services.AddScoped<IEntityTypeAddedConvention, NotMappedMemberAttributeConvention>();
            services.AddScoped<IEntityTypeAddedConvention, BaseTypeDiscoveryConvention>();
            services.AddScoped<IEntityTypeAddedConvention, PropertyDiscoveryConvention>();
            services.AddScoped<IEntityTypeAddedConvention, ServicePropertyDiscoveryConvention>();
            services.AddScoped<IEntityTypeAddedConvention, KeyDiscoveryConvention>();
            services.AddScoped<IEntityTypeAddedConvention, InversePropertyAttributeConvention>();
            services.AddScoped<IEntityTypeAddedConvention, RelationshipDiscoveryConvention>();
            services.AddScoped<IEntityTypeAddedConvention, DerivedTypeDiscoveryConvention>();
            services.AddScoped<IModelFinalizedConvention, ServicePropertyDiscoveryConvention>();
            services.AddScoped<IModelFinalizedConvention, InversePropertyAttributeConvention>();
            services.AddScoped<IModelFinalizedConvention, KeyAttributeConvention>();
            services.AddScoped<IModelFinalizedConvention, TypeMappingConvention>();
            services.AddScoped<IPropertyAddedConvention, KeyAttributeConvention>();
            services.AddScoped<IPropertyAddedConvention, KeyDiscoveryConvention>();
            services.AddScoped<IPropertyAddedConvention, BackingFieldConvention>();
            services.AddScoped<IConventionSetBuilder, RuntimeConventionSetBuilder>();
            services.AddScoped<IProviderConventionSetBuilder, ProviderConventionSetBuilder>();
            services.AddScoped<IMemoryCache, MemoryCache>(p => new MemoryCache(new MemoryCacheOptions()));
            services.AddScoped<IModelSource, ModelSource>();
            services.AddScoped<IModelCustomizer, ModelCustomizer>();
            services.AddScoped<IModelCacheKeyFactory, ModelCacheKeyFactory>();
            services.AddScoped<IConstructorBindingFactory, ConstructorBindingFactory>();
            services.AddScoped<IPropertyParameterBindingFactory, PropertyParameterBindingFactory>();
            services.AddScoped<IParameterBindingFactory, EntityTypeParameterBindingFactory>();
            services.AddScoped<IParameterBindingFactories, ParameterBindingFactories>();
            services.AddScoped<IMemberClassifier, MemberClassifier>();
            services.AddScoped<IValueConverterSelector, ValueConverterSelector>();
            services.AddScoped<IModelValidator, ModelValidator>();
            services.AddScoped<IRegisteredServices, RegisteredServices>();
            services.AddScoped<ProviderConventionSetBuilderDependencies>();
            services.AddScoped<ModelValidatorDependencies>();
            services.AddScoped<TypeMappingSourceDependencies>();
            services.AddScoped<RelationalConventionSetBuilderDependencies>();
            services.AddScoped<ValueConverterSelectorDependencies>();
            services.AddScoped<ModelSourceDependencies>();
            services.AddScoped<ModelCustomizerDependencies>();
            services.AddScoped<ModelCacheKeyFactoryDependencies>();

            this.Services = services;
        }

        public string LogFragment
        {
            get
            {
                return DebugUtils.BreakReturnNull();
            }
        }

        public DbContextOptionsExtensionInfo Info
        {
            get
            {
                return new AbstraXProviderContextOptionsExtensionInfo(this);
            }
        }
    }
}