using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AbstraX.ServerInterfaces;
using AbstraX;

namespace EntityProvider.Web.Entities
{
    public class EntitiesBase
    {
        public XmlDocument EdmxDocument { get; set; }
        public XmlNamespaceManager NamespaceManager { get; set; }
    }
}
