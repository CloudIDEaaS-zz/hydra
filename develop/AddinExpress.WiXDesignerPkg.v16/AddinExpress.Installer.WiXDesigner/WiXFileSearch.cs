using System;
using System.Collections.Generic;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXFileSearch : WiXEntity
	{
		private List<WiXFileSearchRef> _refs;

		internal List<WiXFileSearchRef> FileSearchRefs
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
				foreach (WiXFileSearchRef _ref in this._refs)
				{
					_ref.Id = value;
				}
				base.SetAttributeValue("Id", value);
				this.SetDirty();
			}
		}

		internal string Languages
		{
			get
			{
				return base.GetAttributeValue("Languages");
			}
			set
			{
				base.SetAttributeValue("Languages", value);
				this.SetDirty();
			}
		}

		internal string LongName
		{
			get
			{
				return base.GetAttributeValue("LongName");
			}
			set
			{
				base.SetAttributeValue("LongName", value);
				this.SetDirty();
			}
		}

		internal string MaxDate
		{
			get
			{
				return base.GetAttributeValue("MaxDate");
			}
			set
			{
				base.SetAttributeValue("MaxDate", value);
				this.SetDirty();
			}
		}

		internal string MaxSize
		{
			get
			{
				return base.GetAttributeValue("MaxSize");
			}
			set
			{
				base.SetAttributeValue("MaxSize", value);
				this.SetDirty();
			}
		}

		internal string MaxVersion
		{
			get
			{
				return base.GetAttributeValue("MaxVersion");
			}
			set
			{
				base.SetAttributeValue("MaxVersion", value);
				this.SetDirty();
			}
		}

		internal string MinDate
		{
			get
			{
				return base.GetAttributeValue("MinDate");
			}
			set
			{
				base.SetAttributeValue("MinDate", value);
				this.SetDirty();
			}
		}

		internal string MinSize
		{
			get
			{
				return base.GetAttributeValue("MinSize");
			}
			set
			{
				base.SetAttributeValue("MinSize", value);
				this.SetDirty();
			}
		}

		internal string MinVersion
		{
			get
			{
				return base.GetAttributeValue("MinVersion");
			}
			set
			{
				base.SetAttributeValue("MinVersion", value);
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

		internal string ShortName
		{
			get
			{
				return base.GetAttributeValue("ShortName");
			}
			set
			{
				base.SetAttributeValue("ShortName", value);
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

		internal WiXFileSearch(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
			this._refs = new List<WiXFileSearchRef>();
		}

		internal override void Delete()
		{
			foreach (WiXFileSearchRef _ref in this._refs)
			{
				_ref.Delete();
			}
			this._refs.Clear();
			base.Delete();
		}
	}
}