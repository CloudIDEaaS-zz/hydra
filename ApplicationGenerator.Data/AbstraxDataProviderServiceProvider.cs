using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace ApplicationGenerator.Data
{
    internal class AbstraxProviderServiceProvider : IServiceProvider
    {
        private DbContextOptions options;
        private IServiceCollection services;

        public AbstraxProviderServiceProvider(DbContextOptions options, IServiceCollection services)
        {
            this.options = options;
            this.services = services;
        }

        public object GetService(Type serviceType)
        {
            var service = services.SingleOrDefault(d => d.ServiceType == serviceType);

            return service;
        }
    }
}