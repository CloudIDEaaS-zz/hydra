using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    public class InherentRoleAttribute : Attribute
    {
        public string Role { get; set; }

        public InherentRoleAttribute(string role)
        {
            this.Role = role;
        }
    }
}
