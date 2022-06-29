using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ripley.Services
{
    public class RoleId
    {
        public string Role { get; set; }
        public Guid Id { get; set; }
    }

    public class ClientKey
    {
        public string Id { get; set; }
        public string Secret { get; set; }
    }

    public class Config
    {
        public RoleId[] Roles { get; set; }
        public ClientKey ClientKey { get; set; }
    }
}