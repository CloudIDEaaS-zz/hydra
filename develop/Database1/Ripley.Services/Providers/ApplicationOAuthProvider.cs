using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Ripley.Services.Models;
using System.Security.Claims;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security;
using Utils;
using System.Web.Hosting;
using System.IO;

namespace Ripley.Services.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private Config config;
        private ClientKey clientKey;
        private DateTime configWriteTime;

        public ApplicationOAuthProvider()
        {
            CheckLoadConfig();
        }

        private void CheckLoadConfig()
        {
            var rootPath = HostingEnvironment.MapPath("~/");
            var configFile = Path.Combine(rootPath, "Config.json");

            if (File.Exists(configFile))
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
        }

        public override Task MatchEndpoint(OAuthMatchEndpointContext context)
        {
            if (context.IsTokenEndpoint && context.Request.Method == "OPTIONS")
            {
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "authorization" });
                context.RequestCompleted();

                return Task.FromResult(0);
            }

            return base.MatchEndpoint(context);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;

            if (!string.IsNullOrEmpty(context.OwinContext.Request.Headers.Get("Origin")))
            {
                SetContextHeaders(context);
            }

            CheckLoadConfig();

            if (context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                if (clientId == clientKey.Id && clientSecret == clientKey.Secret)
                {
                    context.Validated();
                }
                else
                {
                    context.SetError("invalid_client", "Invalid client key");
                    context.Rejected();
                }
            }
            else
            {
                if (context.Request.Method.ToUpper() == "OPTIONS")
                {
                    // it returns OK to preflight requests having an empty body
                    context.Validated();
                }
                else
                {
                    context.SetError("invalid_client", "Client credentials could not be retrieved from the Authorization header");
                    context.Rejected();
                }
            }

            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            var user = await userManager.FindAsync(context.UserName, context.Password);
            IEnumerable<Guid> roleIds;
            ClaimsIdentity oAuthIdentity;
            ClaimsIdentity cookiesIdentity;
            AuthenticationProperties properties;
            AuthenticationTicket ticket;

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            CheckLoadConfig();

            oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);
            cookiesIdentity = await user.GenerateUserIdentityAsync(userManager, CookieAuthenticationDefaults.AuthenticationType);

            foreach (var role in user.Roles)
            {
                oAuthIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Role, "User"));

            roleIds = this.GetRoleIds(oAuthIdentity);

            properties = CreateProperties(user.UserName, roleIds.ToArray());
            ticket = new AuthenticationTicket(oAuthIdentity, properties);

            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        private IEnumerable<Guid> GetRoleIds(ClaimsIdentity claims)
        {
            var roleClaims = claims.FindAll(ClaimTypes.Role);

            return config.Roles.Where(r => roleClaims.Any(c => c.Value == r.Role)).Select(r => r.Id);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (var property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        private void SetContextHeaders(OAuthValidateClientAuthenticationContext context)
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, PUT, DELETE, POST, OPTIONS" });
            context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Content-Type, Accept, Authorization" });
            context.Response.Headers.Add("Access-Control-Max-Age", new[] { "1728000" });
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == clientKey.Id)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName, Guid[] roleIds)
        {
            var roles = string.Join(",", roleIds.Select(r => r.ToString()));

            var data = new Dictionary<string, string>
            {
                { "userName", userName },
                { "roles", roles }
            };

            return new AuthenticationProperties(data);
        }
    }
}