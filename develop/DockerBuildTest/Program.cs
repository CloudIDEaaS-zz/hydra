using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Serilog;
using Utils;

namespace DockerBuildTest
{
    public class Program
    {
        private static IConfigurationRoot configuration;
        private static CancellationTokenSource traceCancellationTokenSource;

        public static void Main(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory(); 

            configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("appsettings.shared.json", true, true)
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
            var serilogFile = Path.Combine(logPath, $"{ assembly.GetNameParts().AssemblyName }.log");
            var loggingTraceListenerAddress = configuration["LoggingTraceListenerAddress"].ToString();
            var loggingTraceListenerPort = int.Parse(configuration["LoggingTraceListenerPort"].ToString());
            var serilogLogger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.File(serilogFile, rollingInterval: RollingInterval.Day, fileSizeLimitBytes: NumberExtensions.MB, rollOnFileSizeLimit: true)
                .WriteTo.Trace(loggingTraceListenerAddress, loggingTraceListenerPort, configuration.GetSection("Serilog"), traceCancellationTokenSource.Token)
                .CreateLogger();

            return Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Service>();
            })
            .ConfigureLogging(logBuilder =>
            {
                logBuilder.AddConsole()
                .AddEventLog()
                .AddSerilog(serilogLogger, true)
                .AddConfiguration(configuration.GetSection("Logging"));
            });
        }
    }
}
