using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Diagnostics;
using System.Xml.Linq; 

namespace AbstraX.TypeMappings.Schemas
{
    public class Schema
    {
        private Stream stream;
        private XmlSchema schema;

        public Schema(Stream stream)
        {
            this.stream = stream;
        }

        public XmlSchema XmlSchema
        {
            get
            {
                return schema;
            }
        }

        public Stream Stream
        {
            get
            {
                return stream;
            }

            set
            {
                stream = value;
            }
        }

        public XDocument GetXML()
        {
            XDocument xml;
            MemoryStream memoryStream;
            var length = this.stream.Length;
            var bytes = new byte[stream.Length];

            stream.Position = 0;

            stream.Read(bytes, 0, (int)length);
            memoryStream = new MemoryStream(bytes);

            xml = XDocument.Load(memoryStream);

            return xml;
        }

        public XDocument GetXPath()
        {
            XDocument document;
            MemoryStream memoryStream;
            var length = stream.Length;
            var bytes = new byte[stream.Length];

            stream.Position = 0;

            stream.Read(bytes, 0, (int)length);
            memoryStream = new MemoryStream(bytes);

            document = XDocument.Load(memoryStream);

            return document;
        }

        public XPathNodeIterator Select(string path)
        {
            var manager = CreateNamespaceManager(this.GetXML());

            return this.GetXPath().CreateNavigator().Select(path, manager);
        }

        public static XmlNamespaceManager CreateNamespaceManager(XDocument document)
        {
            var manager = new XmlNamespaceManager(new NameTable());

            foreach (XAttribute attribute in document.XPathSelectElement("/*").Attributes())
            {
                if (attribute.Name.NamespaceName == "http://www.w3.org/2000/xmlns/")
                {
                    manager.AddNamespace(attribute.Name.LocalName, attribute.Value);
                }
                else if (attribute.Name.LocalName == "xmlns")
                {
                    manager.AddNamespace("xs", attribute.Value);
                }
            }

            return manager;
        }

        public XmlNamespaceManager NamespaceManager
        {
            get 
            {
                var manager = CreateNamespaceManager(this.GetXML());

                return manager;
            }
        }
    }
}
