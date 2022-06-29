using $specifiedsolutionname$.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

        public ApplicationServices($specifiedsolutionname$Context $specifiedsolutionname$Context, LoginTimeoutService loginTimeoutService, IServiceProvider serviceProvider, IHostEnvironment hostEnvironment, ILogger<ApplicationServices> logger)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
            };

            try
            {
                var $specifiedsolutionname$Database = $specifiedsolutionname$Context.Database;

                $specifiedsolutionname$Database.Migrate();
            }
            catch (Exception ex)
            {
                logger.LogError("Error during database migration: {0}", ex);
            }

            this.loginTimeoutService = loginTimeoutService;

            loginTimeoutService.Start();
        }

        public void Dispose()
        {
            loginTimeoutService.Stop();
        }
# elseif GENERATOR_TOKEN_USES_DATABASE
        public ApplicationServices($specifiedsolutionname$Context $specifiedsolutionname$Context, ILogger<ApplicationServices> logger)
        {
            try
            {
                var $specifiedsolutionname$Database = $specifiedsolutionname$Context.Database;

                $specifiedsolutionname$Database.Migrate();
            }
            catch (Exception ex)
            {
                logger.LogError("Error during database migration: {0}", ex);
            }
        }

        public void Dispose()
        {
        }
#else
        public ApplicationServices(ILogger<ApplicationServices> logger)
        {
        }

        public void Dispose()
        {
        }
#endif
    }
}
