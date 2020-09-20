using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ApplicationGenerator.Data
{
    public class AbstraXProviderDbContextServices : IDbContextServices, ICurrentDbContext
    {
        private IServiceProvider serviceProvider;
        private IDbContextOptions contextOptions;
        private DbContext context;

        public AbstraXProviderDbContextServices()
        {
        }

        public AbstraXProviderDbContextServices(IServiceProvider scopedProvider, IDbContextOptions contextOptions, DbContext context)
        {
            this.serviceProvider = scopedProvider;
            this.contextOptions = contextOptions;
            this.context = context;
        }

        public DbContext Context
        {
            get
            {
                return this.context;
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
                return null;
            }
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

        public IDbContextServices Initialize(IServiceProvider scopedProvider, IDbContextOptions contextOptions, DbContext context)
        {
            this.serviceProvider = scopedProvider;
            this.contextOptions = contextOptions;
            this.context = context;

            return new AbstraXProviderDbContextServices(scopedProvider, contextOptions, context);
        }
    }
}