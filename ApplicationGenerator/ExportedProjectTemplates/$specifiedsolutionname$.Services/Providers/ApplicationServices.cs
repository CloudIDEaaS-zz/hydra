using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace $safeprojectname$.Providers
{
    public interface IApplicationServices : IDisposable
    {
    }

    public class ApplicationServices : IApplicationServices
    {
#if GENERATOR_TOKEN_TRACKS_USER_LOGINS
        private LoginTimeoutService loginTimeoutService;

        public ApplicationServices(LoginTimeoutService loginTimeoutService, IServiceProvider serviceProvider, IHostEnvironment hostEnvironment)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
            };

            this.loginTimeoutService = loginTimeoutService;

            loginTimeoutService.Start();
        }

        public void Dispose()
        {
            loginTimeoutService.Stop();
        }
#else
    public void Dispose()
        {
        }

#endif
    }
}
