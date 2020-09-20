using System;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using CodeInterfaces.TypeMappings.Schemas;
using System.Xml.Linq;

namespace CodeInterfaces.TypeMappings
{
    public class Transform
    {
        protected string name;
        protected XDocument transform;
        protected XmlNamespaceManager namespaceManager;

        public Transform(Stream stream, string name)
        {
            this.transform = XDocument.Load(stream);
            this.namespaceManager = Schema.CreateNamespaceManager(transform);
            this.name = name;

            stream.Seek(0, SeekOrigin.Begin);
        }

        public Transform(XDocument transformCopy, string strName)
        {
            transform = XDocument.Load(XmlReader.Create(new StringReader(transformCopy.ToString()))); 
            namespaceManager = Schema.CreateNamespaceManager(transform);
            name = strName;
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }
    }
}
