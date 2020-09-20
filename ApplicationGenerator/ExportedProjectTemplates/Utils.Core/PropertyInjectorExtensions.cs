using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public static class PropertyInjectorExtensions
    {
        public static void Initialize(this IServiceProvider serviceProvider, object obj)
        {
            foreach (var property in obj.GetType().GetProperties().Where(p => p.CanWrite && p.HasCustomAttribute<InjectAttribute>()))
            {
                var propertyType = property.PropertyType;
                var propertyObject = serviceProvider.GetService(propertyType);

                property.SetValue(obj, propertyObject);
            }
        }
    }
}
