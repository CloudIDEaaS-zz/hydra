using System;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal abstract class Element
	{
		protected Element()
		{
		}

		protected XmlAttribute AddAttribute(XmlNode toXMLNode, string name, string value)
		{
			XmlAttribute xmlAttribute = toXMLNode.OwnerDocument.CreateAttribute(name);
			xmlAttribute.Value = value;
			toXMLNode.Attributes.Append(xmlAttribute);
			return xmlAttribute;
		}

		protected XmlElement AddElement(XmlNode toXMLNode, string name)
		{
			XmlElement xmlElement = toXMLNode.OwnerDocument.CreateElement(name, toXMLNode.NamespaceURI);
			toXMLNode.AppendChild(xmlElement);
			return xmlElement;
		}

		public static void BuildMessage(string message, BootstrapperProduct.BuildErrorLevel level)
		{
			BootstrapperProduct.RaiseBuildMessage(message, level);
		}

		public abstract XmlElement LoadFromXML(XmlElement workingElement);

		public abstract void WriteXML(XmlElement parentElement);
	}
}