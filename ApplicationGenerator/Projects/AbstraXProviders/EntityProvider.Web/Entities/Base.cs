using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AbstraX.ServerInterfaces;
using System.ServiceModel.DomainServices.Server;
using AbstraX;
using AbstraX.Contracts;


namespace EntityProvider.Web.Entities
{
    public class EntitiesBase
    {
        [Exclude]
        public XmlDocument EdmxDocument { get; set; }
        [Exclude]
        public XmlNamespaceManager NamespaceManager { get; set; }

        [AbstraXExtensionAttribute("IEntityProviderExtension", "EntityProviderExtension")]
        public IAbstraXExtension LoadExtension()
        {
            return null;
        }
    }
}
