using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeAttribute : Attribute
    {
        public string Roles { get; set; }
        public AuthorizationState State { get; }

        public AuthorizeAttribute()
        {
        }

        public AuthorizeAttribute(AuthorizationState authorizationState)
        {
            this.State = authorizationState;
        }
    }
}
