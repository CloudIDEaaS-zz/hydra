
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Utils;
using Serilog.Sinks.Elasticsearch;
using System.Net;

namespace $safeprojectname$
{ 
    public class Program
    {
        private static IConfigurationRoot configuration;
        private static CancellationTokenSource traceCancellationTokenSource;
        public static ILoggerRelay LoggerRelay { get; private set; }

        public static void Main(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            traceCancellationTokenSource = new CancellationTokenSource();

            CreateHostBuilder(args).Build().Run();

            traceCancellationTokenSource.Cancel();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var assembly = Assembly.GetEntryAssembly();
            var location = Path.GetDirectoryName(assembly.Location);
            var logPath = Path.GetFullPath(Path.Combine(location, @"..\..\..\Logs"));

            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureAppConfiguration(configBuilder =>
            {
                configBuilder.AddJsonFile("appsettings.shared.json", true, true);
            })
            .ConfigureLogging(logBuilder =>
            {
                if (configuration != null)
                {
                    logBuilder.AddConsole()
                    .AddEventLog()
                    .AddConfiguration(configuration.GetSection("Logging"));
                }
            });
        }
    }
}
