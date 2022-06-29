using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;
using Ripley.Services.Models;
using Ripley.Services.Providers;

[assembly: OwinStartup(typeof(Ripley.Services.Startup))]

namespace Ripley.Services
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
