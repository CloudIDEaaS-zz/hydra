using System;
using System.Collections.Generic;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXRegistrySearch : WiXEntity
	{
		private List<WiXRegistrySearchRef> _refs;

		internal string Id
		{
			get
			{
				return base.GetAttributeValue("Id");
			}
			set
			{
				foreach (WiXRegistrySearchRef _ref in this._refs)
				{
					_ref.Id = value;
				}
				base.SetAttributeValue("Id", value);
				this.SetDirty();
			}
		}

		internal string Key
		{
			get
			{
				return base.GetAttributeValue("Key");
			}
			set
			{
				base.SetAttributeValue("Key", value);
				this.SetDirty();
			}
		}

		internal new string Name
		{
			get
			{
				return base.GetAttributeValue("Name");
			}
			set
			{
				base.SetAttributeValue("Name", value);
				this.SetDirty();
			}
		}

		internal List<WiXRegistrySearchRef> RegistrySearchRefs
		{
			get
			{
				return this._refs;
			}
		}

		internal string Root
		{
			get
			{
				return base.GetAttributeValue("Root");
			}
			set
			{
				base.SetAttributeValue("Root", value);
				this.SetDirty();
			}
		}

		public override object SupportedObject
		{
			get
			{
				return this;
			}
		}

		internal string Type
		{
			get
			{
				return base.GetAttributeValue("Type");
			}
			set
			{
				base.SetAttributeValue("Type", value);
				this.SetDirty();
			}
		}

		internal string Win64
		{
			get
			{
				return base.GetAttributeValue("Win64");
			}
			set
			{
				base.SetAttributeValue("Win64", value);
				this.SetDirty();
			}
		}

		internal WiXRegistrySearch(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
			this._refs = new List<WiXRegistrySearchRef>();
		}

		internal override void Delete()
		{
			foreach (WiXRegistrySearchRef _ref in this._refs)
			{
				_ref.Delete();
			}
			this._refs.Clear();
			base.Delete();
		}
	}
}