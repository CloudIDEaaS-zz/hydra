using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Text;
using System.Linq;

namespace Utils
{
    // https://www.postsharp.net/blog/post/dependency-injection-naturally

    public static class AspectServiceExtensions
    {
        private static CompositionContainer container;

        public static void UseAspectServices(this IServiceCollection services)
        {
            TypeCatalog typeCatalog;
            var aspectServiceImporter = new AspectServiceImporter(services);

            typeCatalog = new TypeCatalog(typeof(AspectServiceImporter));

            container = new CompositionContainer();
            container.ComposeExportedValue<IAspectServiceImporter>(aspectServiceImporter);
        }

        public static void BuildObject(object obj)
        {
            if (container == null)
            {
                return;
            }

            container.SatisfyImportsOnce(obj);
        }
    }
}
