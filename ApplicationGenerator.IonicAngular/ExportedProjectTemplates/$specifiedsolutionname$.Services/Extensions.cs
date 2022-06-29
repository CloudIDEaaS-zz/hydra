#if GENERATOR_TOKEN_USES_DATABASE
using $specifiedsolutionname$.Entities.Models;
#endif
#if GENERATOR_TOKEN_USES_USER_ANALYTICS
using MaxMind.GeoIP2;
#endif
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace $safeprojectname$
{
    public static partial class Extensions
    {
#if GENERATOR_TOKEN_USES_API_CLIENTKEY
        private static Config config;
        private static ClientKey clientKey;
#endif
        private static DateTime configWriteTime;
#if GENERATOR_TOKEN_USES_USER_ANALYTICS
        public static string GetUserLocation(this ConnectionInfo connection, IHostEnvironment hostEnvironment)
        {
            using (var reader = new DatabaseReader(hostEnvironment.ContentRootPath + @"\wwwroot\GeoLite\GeoLite2-City.mmdb"))
            {
                var ipAddress = connection.RemoteIpAddress;

                if (ipAddress == null)
                {
                    return "Invalid IP Address";
                }

                try
                {
                    var city = reader.City(ipAddress);

                    return city.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
#endif
#if GENERATOR_TOKEN_TRACKS_USER_LOGINS

        public static UserLogin GetUserLogin(this HydraDevOpsContext devOpsContext, HttpRequest request)
        {
            var authorization = (string)request.Headers["Authorization"];

            if (authorization.StartsWith("Bearer "))
            {
                var token = authorization.RemoveStart("Bearer ");
                var userLogin = devOpsContext.UserLogins.SingleOrDefault(l => l.Token == token);

                return userLogin;
            }

            return null;
        }
#endif
#if GENERATOR_TOKEN_USES_API_CLIENTKEY
        public static Config GetConfig(this IHostEnvironment environment)
        {
            var rootPath = environment.ContentRootPath;
            var configFile = Path.Combine(rootPath, "Config.json");

            if (System.IO.File.Exists(configFile))
            {
                var file = new FileInfo(configFile);

                if (file.LastWriteTime != configWriteTime)
                {
                    using (var reader = new StreamReader(configFile))
                    {
                        config = reader.ReadJson<Config>();
                        clientKey = config.ClientKey;

                        configWriteTime = file.LastWriteTime;
                    }
                }
            }

            return config;
        }
#endif
    }
}
