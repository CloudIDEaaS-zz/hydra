#if GENERATOR_TOKEN_TRACKS_USER_LOGINS
using HydraDevOps.Entities.Models;
#endif
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace $safeprojectname$.Providers
{
    public interface ILoginTimeoutService
    {
        void Start();
        void Stop();
    }

#if GENERATOR_TOKEN_TRACKS_USER_LOGINS
    public class LoginTimeoutService : BaseThreadedService, ILoginTimeoutService
    {
        private IServiceScope scope;
        private HydraDevOpsContext devOpsContext;
        private TimeSpan serviceLoginLastActivityTimeOut;

        public LoginTimeoutService(IServiceProvider serviceProvider, IConfiguration configuration) : base(ThreadPriority.Lowest, 15000)
        {
            scope = serviceProvider.CreateScope();

            this.devOpsContext = scope.ServiceProvider.GetService<HydraDevOpsContext>();
            this.serviceLoginLastActivityTimeOut = TimeSpan.Parse(configuration["ServiceLoginLastActivityTimeOut"]);
        }

        public override void Stop()
        {
            scope.Dispose();

            base.Stop();
        }

        public override void DoWork(bool stopping)
        {
            var now = DateTime.UtcNow;
            var devOpsDatabase = devOpsContext.Database;
            var sql = $"UPDATE dbo.UserLogins SET IsLoggedIn = 0, LoggedOutTime = '{ now.ToString() }' WHERE DATEDIFF(minute, LastActivity, '{ now.ToString() }') > 15";

            try
            {
                devOpsDatabase.ExecuteSqlCommand(sql);
            }
            catch (Exception ex)
            {
            }
        }
    }
#endif
}
