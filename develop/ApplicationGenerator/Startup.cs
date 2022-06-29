// file:	Startup.cs
//
// summary:	Implements the startup class

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
    /// <summary>   A startup. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/15/2021. </remarks>

    public class Startup
    {
        /// <summary>   Gets a value indicating whether the log service messages. </summary>
        ///
        /// <value> True if log service messages, false if not. </value>

        public bool LogServiceMessages { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/15/2021. </remarks>
        ///
        /// <param name="logServiceMessages">   True to log service messages. </param>

        public Startup(bool logServiceMessages)
        {
            LogServiceMessages = logServiceMessages;
        }


        /// <summary>   Configurations the given application. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/15/2021. </remarks>
        ///
        /// <param name="app">  The application. </param>

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
