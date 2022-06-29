using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IdentityAttribute : Attribute
    {
        public string IdentitySet { get; }
        public string RoleSet { get; }
        public string UserToRolesSet { get; }

        public IdentityAttribute(string identitySet, string roleSet = null, string userToRolesSet = null)
        {
            this.IdentitySet = identitySet;
            this.RoleSet = roleSet;
            this.UserToRolesSet = userToRolesSet;
        }
    }
}
