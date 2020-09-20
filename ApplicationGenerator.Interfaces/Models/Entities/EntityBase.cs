using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AbstraX.ServerInterfaces;
using AbstraX;
using AbstraX.Models;

namespace EntityProvider.Web.Entities
{
    public abstract class EntityBase : BaseObject
    {
        public XmlDocument EdmxDocument { get; set; }
        public XmlNamespaceManager NamespaceManager { get; set; }
        protected XmlNode node;

        public EntityBase()
        {
        }

        public EntityBase(BaseObject parent) : base(parent)
        {

        }

        public XmlNode Node
        {
            get
            {
                return node;
            }
        }
    }
}
