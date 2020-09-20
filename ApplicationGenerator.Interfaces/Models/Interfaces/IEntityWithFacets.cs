using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AbstraX.ServerInterfaces;

namespace AbstraX.Models.Interfaces
{
    public interface IEntityWithFacets : IBase
    {
    }

    public interface IEntityWithOptionalFacets : IEntityWithFacets
    {
        bool FollowWithout { get; }
        bool NoUIOrConfig { get; set; }
    }
}
