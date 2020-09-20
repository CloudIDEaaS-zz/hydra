using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AbstraX.ServerInterfaces;

namespace EntityProvider.Web.Entities
{
    public interface IEntityParent : IParentBase
    {
        XmlDocument EdmxDocument { get; set; }
        XmlNamespaceManager NamespaceManager { get; set; }
    }
}
