using System;
using System.Collections.Generic;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXDirectorySearch : WiXEntity
	{
		private List<WiXDirectorySearchRef> _refs;

		internal string AssignToProperty
		{
			get
			{
				return base.GetAttributeValue("AssignToProperty");
			}
			set
			{
				base.SetAttributeValue("AssignToProperty", value);
				this.SetDirty();
			}
		}

		internal string Depth
		{
			get
			{
				return base.GetAttributeValue("Depth");
			}
			set
			{
				base.SetAttributeValue("Depth", value);
				this.SetDirty();
			}
		}

		internal List<WiXDirectorySearchRef> DirectorySearchRefs
		{
			get
			{
				return this._refs;
			}
		}

		internal string Id
		{
			get
			{
				return base.GetAttributeValue("Id");
			}
			set
			{
				foreach (WiXDirectorySearchRef _ref in this._refs)
				{
					_ref.Id = value;
				}
				base.SetAttributeValue("Id", value);
				this.SetDirty();
			}
		}

		internal string Path
		{
			get
			{
				return base.GetAttributeValue("Path");
			}
			set
			{
				base.SetAttributeValue("Path", value);
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

		internal WiXDirectorySearch(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
			this._refs = new List<WiXDirectorySearchRef>();
		}

		internal override void Delete()
		{
			foreach (WiXDirectorySearchRef _ref in this._refs)
			{
				_ref.Delete();
			}
			this._refs.Clear();
			base.Delete();
		}
	}
}