using System;
using System.Collections.Generic;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IISCertificate : WiXEntity
	{
		private List<IISCertificateRef> _refs;

		internal string BinaryKey
		{
			get
			{
				return base.GetAttributeValue("BinaryKey");
			}
			set
			{
				base.SetAttributeValue("BinaryKey", value);
				this.SetDirty();
			}
		}

		internal string CertificatePath
		{
			get
			{
				return base.GetAttributeValue("CertificatePath");
			}
			set
			{
				base.SetAttributeValue("CertificatePath", value);
				this.SetDirty();
			}
		}

		internal List<IISCertificateRef> CertificateRefs
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
				foreach (IISCertificateRef certificateRef in this.CertificateRefs)
				{
					certificateRef.Id = value;
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

		internal string Overwrite
		{
			get
			{
				return base.GetAttributeValue("Overwrite");
			}
			set
			{
				base.SetAttributeValue("Overwrite", value);
				this.SetDirty();
			}
		}

		internal string PFXPassword
		{
			get
			{
				return base.GetAttributeValue("PFXPassword");
			}
			set
			{
				base.SetAttributeValue("PFXPassword", value);
				this.SetDirty();
			}
		}

		internal string Request
		{
			get
			{
				return base.GetAttributeValue("Request");
			}
			set
			{
				base.SetAttributeValue("Request", value);
				this.SetDirty();
			}
		}

		internal string StoreLocation
		{
			get
			{
				return base.GetAttributeValue("StoreLocation");
			}
			set
			{
				base.SetAttributeValue("StoreLocation", value);
				this.SetDirty();
			}
		}

		internal string StoreName
		{
			get
			{
				return base.GetAttributeValue("StoreName");
			}
			set
			{
				base.SetAttributeValue("StoreName", value);
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

		internal IISCertificate(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
			this._refs = new List<IISCertificateRef>();
		}

		internal override void Delete()
		{
			foreach (IISCertificateRef certificateRef in this.CertificateRefs)
			{
				certificateRef.Delete();
			}
			this.CertificateRefs.Clear();
			base.Delete();
		}
	}
}