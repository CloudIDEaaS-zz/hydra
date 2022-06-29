using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ripley.Services.Models
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public class ClientUser
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }

        internal string DebugInfo
        {
            get
            {

                return string.Format(
                    "UserId: {0}\r\n" +
                    "FirstName: {1}\r\n" +
                    "LastName: {2}\r\n" +
                    "UserName: {3}\r\n" +
                    "EmailAddress: {4}",
                    this.UserId.ToString(),
                    this.FirstName,
                    this.LastName,
                    this.UserName,
                    this.EmailAddress
                );
            }
        }

        public override string ToString()
        {
            return this.DebugInfo;
        }
    }

    [DebuggerDisplay(" { DebugInfo } ")]
    public class RegisterUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }

        internal string DebugInfo
        {
            get
            {

                return string.Format(
                    "FirstName: {0}\r\n" +
                    "LastName: {1}\r\n" +
                    "UserName: {2}\r\n" +
                    "EmailAddress: {3}\r\n" +
                    "Password: {4}",
                    this.FirstName,
                    this.LastName,
                    this.UserName,
                    this.EmailAddress,
                    this.Password
                );
            }
        }

        public override string ToString()
        {
            return this.DebugInfo;
        }
    }

    [DebuggerDisplay(" { DebugInfo } ")]
    public class User : IUser<string>
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string PasswordHash { get; set; }
        public string Id => this.UserId.ToString();
        public List<string> Roles { get; set; }

        internal string DebugInfo
        {
            get
            {

                return string.Format(
                    "UserId: {0}\r\n" +
                    "FirstName: {1}\r\n" +
                    "LastName: {2}\r\n" +
                    "UserName: {3}\r\n" +
                    "EmailAddress: {4}\r\n" +
                    "PasswordHash: {5}",
                    this.UserId.ToString(),
                    this.FirstName,
                    this.LastName,
                    this.UserName,
                    this.EmailAddress,
                    this.PasswordHash
                );
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);

            return userIdentity;
        }

        public override string ToString()
        {
            return this.DebugInfo;
        }
    }
}
