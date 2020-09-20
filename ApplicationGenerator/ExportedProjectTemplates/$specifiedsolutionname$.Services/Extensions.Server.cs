using $safeprojectname$.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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
        public static IApplicationBuilder UseJwtServiceAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ServiceAuthorizationHandler>();
        }
#if GENERATOR_TOKEN_USES_USER_ANALYTICS
        public static IApplicationBuilder UseGoogleAnalytics(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AnalyticsTrackingHandler>();
        }
#endif

#if GENERATOR_TOKEN_USES_API_CLIENTKEY
        public static TokenValidationParameters GetTokenValidationParameters(this IHostEnvironment environment)
        {
            var config = environment.GetConfig();
            var clientKey = config.ClientKey;

            return new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clientKey.Issuer + ":" + clientKey.Key))
            };
        }
#else
        public static TokenValidationParameters GetTokenValidationParameters(this IConfiguration configuration)
        {
            var key = configuration["ApiKey"];

            return new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        }
#endif
    }
}
