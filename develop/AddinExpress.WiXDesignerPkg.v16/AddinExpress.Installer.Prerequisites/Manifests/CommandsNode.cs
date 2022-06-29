using System;
using System.Collections;
using System.Reflection;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class CommandsNode : CollectionBase
	{
		private CommandsNode.RebootOptions m_Reboot;

		private bool m_RebootInput;

		public Command this[int index]
		{
			get
			{
				return (Command)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public CommandsNode.RebootOptions Reboot
		{
			get
			{
				return this.m_Reboot;
			}
			set
			{
				this.m_Reboot = value;
				this.m_RebootInput = true;
			}
		}

		public CommandsNode()
		{
			this.m_RebootInput = false;
		}

		public CommandsNode(CommandsNode.RebootOptions rebootOption)
		{
			this.m_RebootInput = false;
			this.Reboot = rebootOption;
		}

		public Command Add(Command value)
		{
			if (value == null)
			{
				return null;
			}
			base.List.Add(value);
			return value;
		}

		public void AddRange(Command[] value)
		{
			for (int i = 0; i < (int)value.Length; i++)
			{
				this.Add(value[i]);
			}
		}

		public bool Contains(Command value)
		{
			return base.List.Contains(value);
		}

		public int IndexOf(Command value)
		{
			return base.List.IndexOf(value);
		}

		public void Insert(int index, Command value)
		{
			base.List.Insert(index, value);
		}

		public XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("Not Yet Implemented");
		}

		public string RebootEnumToString(CommandsNode.RebootOptions rebootOption)
		{
			switch (rebootOption)
			{
				case CommandsNode.RebootOptions.Defer:
				{
					return "Defer";
				}
				case CommandsNode.RebootOptions.Immediate:
				{
					return "Immediate";
				}
				case CommandsNode.RebootOptions.Force:
				{
					return "Force";
				}
				case CommandsNode.RebootOptions.None:
				{
					return "None";
				}
			}
			return null;
		}

		public void Remove(Command value)
		{
			base.List.Remove(value);
		}

		public XmlElement WriteXML(XmlElement parentElement)
		{
			Element.BuildMessage("  Commands... ", BootstrapperProduct.BuildErrorLevel.None);
			XmlElement xmlElement = parentElement.OwnerDocument.CreateElement("Commands", parentElement.NamespaceURI);
			parentElement.AppendChild(xmlElement);
			if (this.m_RebootInput)
			{
				XmlAttribute str = xmlElement.OwnerDocument.CreateAttribute("Reboot");
				str.Value = this.RebootEnumToString(this.Reboot);
				xmlElement.Attributes.Append(str);
			}
			return xmlElement;
		}

		public class RebootOptionNames
		{
			public const string Defer = "Defer";

			public const string Force = "Force";

			public const string Immediate = "Immediate";

			public const string None = "None";

			public RebootOptionNames()
			{
			}
		}

		public enum RebootOptions
		{
			Defer,
			Immediate,
			Force,
			None
		}

		private class XMLStrings
		{
			public const string ElementName = "Commands";

			public const string RebootAttribute = "Reboot";

			public XMLStrings()
			{
			}
		}
	}
}