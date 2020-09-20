using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Utils
{
    public static class WebSocketExtensions
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app, PathString path, Func<IServiceProvider, WebSocketHandler> getWebSocketHandler)
        {
            return app.Map(path, (a) => a.UseMiddleware<WebSocketManagerMiddleware>(getWebSocketHandler));
        }

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<WebSocketConnectionManager>();

            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }
    }
}
