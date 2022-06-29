using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DockerBuildTest
{
    public class Service : IHostedService
    {
        private ILogger<Service> serviceLogger;

        public Service(IHostEnvironment hostEnvironment, IConfiguration configuration, ILogger<Service> serviceLogger)
        {
            var environmentName = hostEnvironment.EnvironmentName;
            var userName = configuration[$"{ environmentName }:HydraDevOps.Database:AutomationServiceAccount"];
            var password = configuration[$"{ environmentName }:HydraDevOps.Database:AutomationServiceAccountPassword"];

            this.serviceLogger = serviceLogger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            serviceLogger.LogInformation("{0} has started", nameof(Service));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            serviceLogger.LogInformation("{0} has stopped", nameof(Service));

            return Task.CompletedTask;
        }
    }
}
