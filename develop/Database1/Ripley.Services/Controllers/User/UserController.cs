using Ripley.Entities;
using Ripley.Services.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;
using User = Ripley.Services.Models.User;

namespace Ripley.Services.Controllers
{
    [Authorize(Roles = "User")]
    [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
    public class UserController : ApiController
    {
        private IRipleyEntities ripleyEntities;

        public UserController(IRipleyEntities ripleyEntities)
        {
            this.ripleyEntities = ripleyEntities;
        }


        [HttpGet]
        [Route("~/api/user/{id:guid}")]
        public Models.ClientUser GetUser(Guid id)
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var users = ripleyEntities.Users;

                return users.Where(r => r.UserId == id).Select(r => new Models.ClientUser
                {
                    UserId = r.UserId,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    UserName = r.UserName,
                    EmailAddress = r.EmailAddress,
                }).Single();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public void CreateUser(Models.ClientUser user)
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var users = ripleyEntities.Users;

                users.Add(new Entities.User
                {
                    UserId = Guid.NewGuid(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    EmailAddress = user.EmailAddress,
                });

                ripleyEntities.SaveChanges();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        /*
        [HttpPost]
        [Route("~/api/register")]
        public void RegisterUser(Models.RegisterUser user)
        {
            var users = ripleyEntities.Users;

            users.Add(new Entities.User
            {
                UserId = Guid.NewGuid(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                EmailAddress = user.EmailAddress,
                PasswordHash = user.PasswordHash,
            });

            ripleyEntities.SaveChanges();
        }
        */

        [HttpPut]
        public void UpdateUser(Models.ClientUser user)
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var updateUser = ripleyEntities.Users.Single(r => r.UserId == user.UserId);

                updateUser.FirstName = user.FirstName;
                updateUser.LastName = user.LastName;
                updateUser.UserName = user.UserName;
                updateUser.EmailAddress = user.EmailAddress;

                ripleyEntities.SaveChanges();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        [HttpDelete]
        public void DeleteUser(Guid id)
        {
            var identity = this.RequestContext.Principal.Identity;

            if (identity.IsAuthenticated)
            {
                var users = ripleyEntities.Users;
                var deleteUser = users.Single(r => r.UserId == id);

                users.Remove(deleteUser);

                ripleyEntities.SaveChanges();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
