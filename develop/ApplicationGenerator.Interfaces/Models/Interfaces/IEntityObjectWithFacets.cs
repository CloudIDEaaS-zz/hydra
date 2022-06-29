using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AbstraX.ServerInterfaces;

namespace AbstraX.Models.Interfaces
{
    public interface IEntityObjectWithFacets : IBase
    {
    }

    public interface IEntityWithOptionalFacets : IEntityObjectWithFacets
    {
        bool FollowWithout { get; }
        bool NoUIOrConfig { get; set; }
    }
}
