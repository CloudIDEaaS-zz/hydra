using ApplicationGenerator.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;
using System.Linq;
using System.Reflection;

namespace ApplicationGenerator.Data
{
    public class AbstraXProviderServices : IAbstraXProviderServices
    {
        public IServiceProvider Provider { get; set; }

        public AbstraXProviderServices(IServiceProvider provider)
        {
            this.Provider = provider;
        }

        public IAbstraXDataProvider LocateProvider(DbContext context)
        {
            var assembly = Assembly.GetEntryAssembly();
            var types = assembly.GetTypes().Where(t => t.HasCustomAttribute<AbstraXDataProviderAttribute>());

            foreach (var type in types)
            {
                var dataProviderAttribute = type.GetCustomAttribute<AbstraXDataProviderAttribute>();

                if (dataProviderAttribute.ContextType == context.GetType())
                {
                    var service = (IAbstraXDataProvider)this.Provider.GetService(dataProviderAttribute.InterfaceType);

                    return service;
                }
            }

            return null;
        }
    }
}
