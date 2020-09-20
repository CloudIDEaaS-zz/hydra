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
    public abstract class EntitiesBase
    {
        public XmlDocument EdmxDocument { get; set; }
        public XmlNamespaceManager NamespaceManager { get; set; }
        protected List<GetAttributesEventHandler> getAttributesEventHandlers;
        protected List<GetAddInEntitiesEventHandler> getAddInsEventHandlers;
        public abstract DefinitionKind Kind { get; }
        public Dictionary<string, IBase> EntityDictionary { get; set; }
        protected XmlNode node;

        public XmlNode Node
        {
            get
            {
                return node;
            }
        }

        internal List<GetAttributesEventHandler> AttributesEventHandlers
        {
            get
            {
                return getAttributesEventHandlers;
            }
        }

        internal List<GetAddInEntitiesEventHandler> AddInsEventHandlers
        {
            get
            {
                return getAddInsEventHandlers;
            }
        }

        public EntitiesBase()
        {
            getAttributesEventHandlers = new List<GetAttributesEventHandler>();
            getAddInsEventHandlers = new List<GetAddInEntitiesEventHandler>();
        }

        public event GetAttributesEventHandler OnGetAttributes
        {
            add
            {
                getAttributesEventHandlers.Add(value);
            }

            remove
            {
                getAttributesEventHandlers.Remove(value);
            }
        }

        public event GetAddInEntitiesEventHandler OnGetAddIns
        {
            add
            {
                getAddInsEventHandlers.Add(value);
            }

            remove
            {
                getAddInsEventHandlers.Remove(value);
            }
        }
    }
}
