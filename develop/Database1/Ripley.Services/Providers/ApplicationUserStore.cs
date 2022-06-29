using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Ripley.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Db = Ripley.Entities;
using Microsoft.AspNet.Identity.Owin;

namespace Ripley.Services.Providers
{
    public class ApplicationUserStore : IUserStore<User>, IUserPasswordStore<User>
    {
        private Db.RipleyEntities entities;
        private IOwinContext context;
        private ApplicationUserManager userManager;

        public ApplicationUserStore(IOwinContext context)
        {
            this.entities = new Db.RipleyEntities();
            this.context = context;
        }

        private ApplicationUserManager UserManager
        {
            get
            {
                if (userManager == null)
                {
                    userManager = context.GetUserManager<ApplicationUserManager>();
                }

                return userManager;
            }
        }

        public Task CreateAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(User user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public Task<User> FindByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<User> FindByNameAsync(string userName)
        {
            var dbUser = entities.Users.Single(u => u.UserName == userName);
            var user = new User();

            user.UserName = dbUser.UserName;
            user.EmailAddress = dbUser.EmailAddress;
            user.FirstName = dbUser.FirstName;
            user.LastName = dbUser.LastName;
            user.PasswordHash = null;

            user.Roles = dbUser.UserToRoles.Select(t => t.Role.RoleName).ToList();

            return Task.FromResult(user);
        }

        public Task<string> GetPasswordHashAsync(User user)
        {
            var dbUser = entities.Users.Single(u => u.UserName == user.UserName);

             return Task.FromResult(dbUser.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            var hasher = this.UserManager.PasswordHasher;

            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}