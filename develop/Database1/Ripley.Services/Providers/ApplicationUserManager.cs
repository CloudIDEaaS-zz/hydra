using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Ripley.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Db = Ripley.Entities;
using Utils;
using System.Web.Hosting;

namespace Ripley.Services.Providers
{
    public class ApplicationUserManager : UserManager<User>
    {
        private IOwinContext context;

        public ApplicationUserManager(IOwinContext context, IUserStore<User> store) : base(store)
        {
            this.context = context;
        }

        public override Task<IdentityResult> CreateAsync(User user)
        {
            return base.CreateAsync(user);
        }

        protected override Task<bool> VerifyPasswordAsync(IUserPasswordStore<User, string> store, User user, string password)
        {
            return base.VerifyPasswordAsync(store, user, password);
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(context, new ApplicationUserStore(context));
            IDataProtectionProvider dataProtectionProvider;

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<User>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            dataProtectionProvider = options.DataProtectionProvider;

            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }

        public override Task<User> FindByNameAsync(string userName)
        {
            var principal = new GenericPrincipal(new GenericIdentity(userName), null);

            Thread.CurrentPrincipal = principal;

            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }

            return base.FindByNameAsync(userName);
        }
    }
}
