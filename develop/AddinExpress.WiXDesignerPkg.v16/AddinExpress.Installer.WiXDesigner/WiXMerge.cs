using System;
using System.Collections.Generic;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXMerge : WiXEntity
	{
		private List<WiXMergeRef> _refs;

		internal string DiskId
		{
			get
			{
				return base.GetAttributeValue("DiskId");
			}
			set
			{
				base.SetAttributeValue("DiskId", value);
				this.SetDirty();
			}
		}

		internal string FileCompression
		{
			get
			{
				return base.GetAttributeValue("FileCompression");
			}
			set
			{
				base.SetAttributeValue("FileCompression", value);
				this.SetDirty();
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
				foreach (WiXMergeRef mergeRef in this.MergeRefs)
				{
					mergeRef.Id = value;
				}
				base.SetAttributeValue("Id", value);
				this.SetDirty();
			}
		}

		internal string Language
		{
			get
			{
				return base.GetAttributeValue("Language");
			}
			set
			{
				base.SetAttributeValue("Language", value);
				this.SetDirty();
			}
		}

		internal List<WiXMergeRef> MergeRefs
		{
			get
			{
				return this._refs;
			}
		}

		internal string SourceFile
		{
			get
			{
				return base.GetAttributeValue("SourceFile");
			}
			set
			{
				base.SetAttributeValue("SourceFile", value);
				this.SetDirty();
			}
		}

		internal string src
		{
			get
			{
				return base.GetAttributeValue("src");
			}
			set
			{
				base.SetAttributeValue("src", value);
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

		internal WiXMerge(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
			this._refs = new List<WiXMergeRef>();
		}

		internal override void Delete()
		{
			foreach (WiXMergeRef mergeRef in this.MergeRefs)
			{
				try
				{
					mergeRef.Delete();
				}
				catch
				{
				}
			}
			this.MergeRefs.Clear();
			base.Delete();
		}
	}
}