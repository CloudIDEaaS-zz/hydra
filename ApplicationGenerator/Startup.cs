using AbstraX.PackageCache;
using AbstraX.Services;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unity;
using Unity.Lifetime;

[assembly: OwinStartup(typeof(AbstraX.Startup))]

namespace AbstraX
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure Web API for self-host. 
            // Note: Routes to be "api/{controller}/{action}" instead of "api/{controller}/{id}"

            var config = new HttpConfiguration();
            var formatters = GlobalConfiguration.Configuration.Formatters;
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;
            var container = new UnityContainer();

            AppDomain.CurrentDomain.SetData("UnityContainer", container);

            container.RegisterType<IPackageCacheStatusProvider, PackageCacheStatusProvider>(new SingletonLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);
             
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            app.UseWebApi(config);
        }
    }
}
