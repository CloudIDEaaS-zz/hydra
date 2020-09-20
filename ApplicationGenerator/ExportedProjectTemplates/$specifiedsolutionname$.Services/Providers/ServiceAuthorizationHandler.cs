#if GENERATOR_TOKEN_USES_DATABASE
using $specifiedsolutionname$.Entities.Models;
#endif
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace $safeprojectname$.Providers
{
    internal class ServiceAuthorizationHandler : DelegatingHandler
    {
        private readonly RequestDelegate next;
        private readonly IHostEnvironment environment;
        private readonly IConfiguration configuration;

        public ServiceAuthorizationHandler(RequestDelegate next, IHostEnvironment environment, IConfiguration configuration)
        {
            this.next = next;
            this.environment = environment;
            this.configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var response = new HttpResponseMessage();
            var request = context.Request;

            if (TokenExists(request) && await ValidateTokenAsync(context))
            {
                await next(context);
            }
            else
            {
                try
                {
                    if (HasValidAuthHeader(request))
                    {
                        response.StatusCode = HttpStatusCode.OK;
                    }
                    else
                    {
                        response.StatusCode = HttpStatusCode.Unauthorized;
                    }

                    await next(context);
                }
                catch (SecurityTokenExpiredException)
                {
                    response.StatusCode = HttpStatusCode.Unauthorized;
                }
                catch (SecurityTokenValidationException)
                {
                    response.StatusCode = HttpStatusCode.Unauthorized;
                }
                catch (SecurityTokenException)
                {
                    response.StatusCode = HttpStatusCode.Unauthorized;
                }
                catch (Exception)
                {
                    response.StatusCode = HttpStatusCode.Unauthorized;
                }

                await Task.FromResult(response);
            }
        }

        private bool TokenExists(HttpRequest request)
        {
            if (request.Headers.Any(h => h.Key == "Authorization"))
            {
                var authHeaders = request.Headers.Where(h => h.Key == "Authorization");
                var authHeaderValue = (string) authHeaders.ElementAt(0).Value;
                var token = authHeaderValue.RegexGet("Bearer (?<token>.*$)", "token");

                return token != null;
            }

            return false;
        }

        private bool HasValidAuthHeader(HttpRequest request)
        {
#if GENERATOR_TOKEN_USES_API_CLIENTKEY
            if (request.Headers.Any(h => h.Key == "Authorization"))
            {
                var authHeaders = request.Headers.Where(h => h.Key == "Authorization");
                var authHeaderValue = (string)authHeaders.ElementAt(0).Value;
                var token = authHeaderValue.RegexGet("Basic (?<token>.*$)", "token");
                var config = environment.GetConfig();
                var clientKey = config.ClientKey;

                if (token != null)
                {
                    var clientSecret = token.FromBase64ToString();
                    var parts = clientSecret.Split(":");
                    var issuer = parts[0];
                    var key = parts[1];

                    return issuer.AsCaseless() == clientKey.Issuer && key.AsCaseless() == clientKey.Key;
                }
            }
            return false;
#else
        return true;
#endif
        }

        private async Task<bool> ValidateTokenAsync(HttpContext context)
        {
            var request = context.Request;

            if (request.Headers.Any(h => h.Key == "Authorization"))
            {
                var authHeaders = request.Headers.Where(h => h.Key == "Authorization");
                var authHeaderValue = (string)authHeaders.ElementAt(0).Value;
                var token = authHeaderValue.RegexGet("Bearer (?<token>.*$)", "token");

                return await ValidateTokenAsync(token, context);
            }

            return await Task.FromResult(false);
        }

        private async Task<bool> ValidateTokenAsync(string token, HttpContext context)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetTokenValidationParameters();
#if GENERATOR_TOKEN_TRACKS_USER_LOGINS
            var devOpsContext = GetDevOpsContext(context);
            var userLogin = devOpsContext.UserLogins.SingleOrDefault(l => l.Token == token && l.IsLoggedIn); 

            if (userLogin == null)
            {
                throw new SecurityTokenValidationException();
            }
#endif
            Thread.CurrentPrincipal = jwtSecurityTokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

            if (securityToken == null)
            {
                throw new SecurityTokenValidationException();
            }

            return await Task.FromResult(true);
        }

#if GENERATOR_TOKEN_USES_DATABASE
        private $specifiedsolutionname$Context Get$specifiedsolutionname$Context(HttpContext context)
        {
            var serviceProvider = context.RequestServices;
            var dbContext =  serviceProvider.GetService<$specifiedsolutionname$Context>();

            return dbContext;
        }
#endif

#if GENERATOR_TOKEN_USES_API_CLIENTKEY
        private TokenValidationParameters GetTokenValidationParameters()
        {
            return environment.GetTokenValidationParameters();
        }
#else
        private TokenValidationParameters GetTokenValidationParameters()
        {
            return configuration.GetTokenValidationParameters();
        }
#endif
    }
}