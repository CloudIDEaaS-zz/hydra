using ApplicationGenerator.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Utils;
using System.Linq;

namespace ApplicationGenerator.Data
{
    public class AbstraXProviderLocatorService : IAbstraXProviderLocatorService
    {
        private IServiceCollection services;

        public AbstraXProviderLocatorService(IServiceCollection services)
        {
            this.services = services;
        }

        public IAbstraXDataProvider LocateProvider(DbContext context)
        {
            var assembly = Assembly.GetEntryAssembly();
            var types = assembly.GetTypes();

            return DebugUtils.BreakReturnNull();
        }
    }
}
