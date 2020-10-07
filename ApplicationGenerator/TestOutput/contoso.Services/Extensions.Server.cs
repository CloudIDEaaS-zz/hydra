using contoso.Services.Providers;
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

namespace contoso.Services
{
    public static partial class Extensions
    {
        public static IApplicationBuilder UseJwtServiceAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ServiceAuthorizationHandler>();
        }

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
    }
}
