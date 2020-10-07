using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Utils;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Reflection;
using contoso.Entities.Models;
using Microsoft.EntityFrameworkCore;
using contoso.Services.Providers;
using Microsoft.AspNetCore.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Http;
using Utils.Wrappers.Interfaces;
using Newtonsoft.Json.Serialization;
using IApplicationLifetime = Microsoft.Extensions.Hosting.IApplicationLifetime;
using contoso.Services.Providers.Filters;

namespace contoso.Services
{
    public class Startup
    {
        private const string AllowAll = "AllowAll";
        public IConfiguration Configuration { get; }
        public IHostEnvironment Environment { get; }

        public Startup(IHostEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment environment, IConfiguration configuration, IApplicationServices applicationServices, IApplicationLifetime applicationLifetime, ILogger<Startup> logger)
        {
            var thisAssembly = typeof(Startup).Assembly;
            var assemblyName = thisAssembly.GetName();
            var versionAttribute = thisAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var name = $"{ assemblyName.Name } API v{ versionAttribute.Version }";
            var hostName = Dns.GetHostName();
            var pathBase = configuration["PathBase"];

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                applicationServices.Dispose();
            });

            logger.LogInformation("Configuring startup");

            configuration["HydraDevOpsServicesBaseUrl"] = hostName + pathBase;

            app.UseExceptionHandler(o =>
            {
                o.Run(async context =>
                {
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        logger.LogError($"Error: { contextFeature.Error }");

                        await context.Response.BodyWriter.WriteAsync(ASCIIEncoding.UTF8.GetBytes(contextFeature.Error.Message));
                    }
                });
            });

            app.UsePathBase(pathBase);

            if (environment.IsDevelopment() || environment.IsStaging())
            {
                app.UseOpenApi();
                app.UseSwaggerUi3(c =>
                {
                    c.ServerUrl = pathBase;
                });

                app.UseDeveloperExceptionPage();
            }

            app.UseCors(AllowAll);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseJwtServiceAuthorization();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

            logger.LogInformation("End configuring startup");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var thisAssembly = typeof(Startup).Assembly;
            var assemblyName = thisAssembly.GetName();
            var versionAttribute = thisAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var environment = this.Environment;
            var configuration = this.Configuration;
            var environmentName = environment.EnvironmentName;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = configuration.GetTokenValidationParameters();
            });

            services.AddCors(options =>
            {
                options.AddPolicy(AllowAll,
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddMvc(o =>
            {
                o.EnableEndpointRouting = false;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            .AddNewtonsoftJson()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            if (environment.IsDevelopment() || environment.IsStaging())
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }

            services.AddDbContext<contosoContext>(options =>
            {
                var connectionStringKey = $"{ environmentName }:contoso.Database:ContextConnectionString";
                var connectionString = Configuration[connectionStringKey];

                //if (environment.IsDevelopment() || environment.IsStaging())
                //{
                //    options.EnableDetailedErrors();
                //    options.EnableSensitiveDataLogging();
                //}

                options.UseSqlServer(connectionString, o => 
                {
                    o.MigrationsAssembly("contoso.Services");
                });
            });
            services.AddSingleton<ILoggerRelay>((p) => Program.LoggerRelay);
            services.AddSingleton<ISocketFactory, SocketFactory>();
            services.AddSingleton<IHttpMessageHandlerFactory, DefaultHttpMessageHandlerFactory>();
            services.AddSingleton<IApplicationServices, ApplicationServices>();

            services.AddWebSocketManager();

            services.AddControllers(o =>
            {
                o.Filters.Add(new HttpResponseExceptionFilter());
            });

            services.AddRazorPages(); 

            services.AddSwaggerDocument(c =>
            {
                c.DocumentName = $"{ assemblyName.Name }v{ versionAttribute.Version }-{ environmentName }";
                c.Title = $"{ assemblyName.Name }v{ versionAttribute.Version }-{ environmentName }";
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
        }
    }
}
