using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Text;

namespace $safeprojectname$
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var rootPath = Configuration["RootPath"];

            services.AddControllersWithViews();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = rootPath;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            var baseUrl = Configuration["BaseUrl"];
            var sourcePath = Configuration["SourcePath"];
            var thisAssembly = typeof(Startup).Assembly;
            var assemblyName = thisAssembly.GetName();
            var versionAttribute = thisAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var name = $"{ assemblyName.Name } v{ versionAttribute.Version }";

            logger.LogInformation(name);
            logger.LogInformation("Configuring startup");

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

            AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            {
                logger.LogError($"Error: { e.Exception }");
            };

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action=Index}/{id?}");
            });

            app.Map(baseUrl, builder =>
            {
                app.UseSpaStaticFiles(new StaticFileOptions
                {
                    RequestPath = baseUrl
                });

                app.UseSpa(spa =>
                {
                    spa.Options.DefaultPage = baseUrl + "/index.html";
                    spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
                    {
                        RequestPath = baseUrl
                    };

                    spa.Options.SourcePath = sourcePath;

                    if (env.IsDevelopment())
                    {
                        spa.UseAngularCliServer(npmScript: "start");
                    }
                });
            });

            logger.LogInformation("Startup complete");
        }
    }
}
