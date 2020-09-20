using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    public enum IdentityFieldKind
    {
        [IdentityFieldCategory(IdentityFieldCategory.Client)]
        UserId,
        [IdentityFieldCategory(IdentityFieldCategory.Client | IdentityFieldCategory.Register)]
        Client,
        [IdentityFieldCategory(IdentityFieldCategory.Login | IdentityFieldCategory.Client)]
        UserName,
        [IdentityFieldCategory(IdentityFieldCategory.Register)]
        PasswordHash,
    }
}
