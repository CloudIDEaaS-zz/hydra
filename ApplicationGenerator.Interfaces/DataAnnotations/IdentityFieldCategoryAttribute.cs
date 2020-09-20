using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Field)]
    public class IdentityFieldCategoryAttribute : Attribute
    {
        public IdentityFieldCategory IdentityFieldCategoryFlags { get; }

        public IdentityFieldCategoryAttribute(IdentityFieldCategory categoryFlags)
        {
            this.IdentityFieldCategoryFlags = categoryFlags;
        }
    }
}
