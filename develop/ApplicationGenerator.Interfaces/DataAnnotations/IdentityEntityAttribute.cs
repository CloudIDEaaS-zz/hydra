using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IdentityEntityAttribute : Attribute
    {
        public IdentityEntityKind IdentityEntityKind { get; }

        public IdentityEntityAttribute(IdentityEntityKind identityEntityKind)
        {
            this.IdentityEntityKind = IdentityEntityKind;
        }
    }
}
