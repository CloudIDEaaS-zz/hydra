using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Text;
using System.Xml.Serialization;

namespace Utils
{
    public interface IAspectServiceImporter
    {
        IServiceCollection Services { get; }
    }

    [Export(typeof(IAspectServiceImporter))]
    public class AspectServiceImporter : IAspectServiceImporter
    {
        public IServiceCollection Services { get; }

        public AspectServiceImporter(IServiceCollection services)
        {
            this.Services = services;
        }
    }
}
