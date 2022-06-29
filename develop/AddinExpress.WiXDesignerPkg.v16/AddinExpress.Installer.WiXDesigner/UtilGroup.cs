using System;
using System.Collections.Generic;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class UtilGroup : WiXEntity
	{
		private List<UtilGroupRef> _refs;

		internal string Domain
		{
			get
			{
				return base.GetAttributeValue("Domain");
			}
			set
			{
				base.SetAttributeValue("Domain", value);
				this.SetDirty();
			}
		}

		internal List<UtilGroupRef> GroupRefs
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
				foreach (UtilGroupRef groupRef in this.GroupRefs)
				{
					groupRef.Id = value;
				}
				base.SetAttributeValue("Id", value);
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

		public override object SupportedObject
		{
			get
			{
				return this;
			}
		}

		internal UtilGroup(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
			this._refs = new List<UtilGroupRef>();
		}

		internal override void Delete()
		{
			foreach (UtilGroupRef groupRef in this.GroupRefs)
			{
				groupRef.Delete();
			}
			this.GroupRefs.Clear();
			base.Delete();
		}
	}
}