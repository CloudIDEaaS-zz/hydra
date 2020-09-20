using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    [Flags]
    public enum IdentityFieldCategory
    {
        Client = 1,
        Login = 1 << 1,
        Register = 1 << 2 | Login,
        Server = 1 << 2 | Register | Client,
    }
}
