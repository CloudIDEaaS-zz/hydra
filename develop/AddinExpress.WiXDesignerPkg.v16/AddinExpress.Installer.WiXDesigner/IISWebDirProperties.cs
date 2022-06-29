using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IISWebDirProperties : WiXEntity
	{
		internal string AccessSSL
		{
			get
			{
				return base.GetAttributeValue("AccessSSL");
			}
			set
			{
				base.SetAttributeValue("AccessSSL", value);
				this.SetDirty();
			}
		}

		internal string AccessSSL128
		{
			get
			{
				return base.GetAttributeValue("AccessSSL128");
			}
			set
			{
				base.SetAttributeValue("AccessSSL128", value);
				this.SetDirty();
			}
		}

		internal string AccessSSLMapCert
		{
			get
			{
				return base.GetAttributeValue("AccessSSLMapCert");
			}
			set
			{
				base.SetAttributeValue("AccessSSLMapCert", value);
				this.SetDirty();
			}
		}

		internal string AccessSSLNegotiateCert
		{
			get
			{
				return base.GetAttributeValue("AccessSSLNegotiateCert");
			}
			set
			{
				base.SetAttributeValue("AccessSSLNegotiateCert", value);
				this.SetDirty();
			}
		}

		internal string AccessSSLRequireCert
		{
			get
			{
				return base.GetAttributeValue("AccessSSLRequireCert");
			}
			set
			{
				base.SetAttributeValue("AccessSSLRequireCert", value);
				this.SetDirty();
			}
		}

		internal string AnonymousAccess
		{
			get
			{
				return base.GetAttributeValue("AnonymousAccess");
			}
			set
			{
				base.SetAttributeValue("AnonymousAccess", value);
				this.SetDirty();
			}
		}

		internal string AnonymousUser
		{
			get
			{
				return base.GetAttributeValue("AnonymousUser");
			}
			set
			{
				base.SetAttributeValue("AnonymousUser", value);
				this.SetDirty();
			}
		}

		internal string AspDetailedError
		{
			get
			{
				return base.GetAttributeValue("AspDetailedError");
			}
			set
			{
				base.SetAttributeValue("AspDetailedError", value);
				this.SetDirty();
			}
		}

		internal string AuthenticationProviders
		{
			get
			{
				return base.GetAttributeValue("AuthenticationProviders");
			}
			set
			{
				base.SetAttributeValue("AuthenticationProviders", value);
				this.SetDirty();
			}
		}

		internal string BasicAuthentication
		{
			get
			{
				return base.GetAttributeValue("BasicAuthentication");
			}
			set
			{
				base.SetAttributeValue("BasicAuthentication", value);
				this.SetDirty();
			}
		}

		internal string CacheControlCustom
		{
			get
			{
				return base.GetAttributeValue("CacheControlCustom");
			}
			set
			{
				base.SetAttributeValue("CacheControlCustom", value);
				this.SetDirty();
			}
		}

		internal string CacheControlMaxAge
		{
			get
			{
				return base.GetAttributeValue("CacheControlMaxAge");
			}
			set
			{
				base.SetAttributeValue("CacheControlMaxAge", value);
				this.SetDirty();
			}
		}

		internal string ClearCustomError
		{
			get
			{
				return base.GetAttributeValue("ClearCustomError");
			}
			set
			{
				base.SetAttributeValue("ClearCustomError", value);
				this.SetDirty();
			}
		}

		internal string DefaultDocuments
		{
			get
			{
				return base.GetAttributeValue("DefaultDocuments");
			}
			set
			{
				base.SetAttributeValue("DefaultDocuments", value);
				this.SetDirty();
			}
		}

		internal string DigestAuthentication
		{
			get
			{
				return base.GetAttributeValue("DigestAuthentication");
			}
			set
			{
				base.SetAttributeValue("DigestAuthentication", value);
				this.SetDirty();
			}
		}

		internal string Execute
		{
			get
			{
				return base.GetAttributeValue("Execute");
			}
			set
			{
				base.SetAttributeValue("Execute", value);
				this.SetDirty();
			}
		}

		internal string HttpExpires
		{
			get
			{
				return base.GetAttributeValue("HttpExpires");
			}
			set
			{
				base.SetAttributeValue("HttpExpires", value);
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
				base.SetAttributeValue("Id", value);
				this.SetDirty();
			}
		}

		internal string IIsControlledPassword
		{
			get
			{
				return base.GetAttributeValue("IIsControlledPassword");
			}
			set
			{
				base.SetAttributeValue("IIsControlledPassword", value);
				this.SetDirty();
			}
		}

		internal string Index
		{
			get
			{
				return base.GetAttributeValue("Index");
			}
			set
			{
				base.SetAttributeValue("Index", value);
				this.SetDirty();
			}
		}

		internal string LogVisits
		{
			get
			{
				return base.GetAttributeValue("LogVisits");
			}
			set
			{
				base.SetAttributeValue("LogVisits", value);
				this.SetDirty();
			}
		}

		internal string PassportAuthentication
		{
			get
			{
				return base.GetAttributeValue("PassportAuthentication");
			}
			set
			{
				base.SetAttributeValue("PassportAuthentication", value);
				this.SetDirty();
			}
		}

		internal string Read
		{
			get
			{
				return base.GetAttributeValue("Read");
			}
			set
			{
				base.SetAttributeValue("Read", value);
				this.SetDirty();
			}
		}

		internal string Script
		{
			get
			{
				return base.GetAttributeValue("Script");
			}
			set
			{
				base.SetAttributeValue("Script", value);
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

		internal string WindowsAuthentication
		{
			get
			{
				return base.GetAttributeValue("WindowsAuthentication");
			}
			set
			{
				base.SetAttributeValue("WindowsAuthentication", value);
				this.SetDirty();
			}
		}

		internal string Write
		{
			get
			{
				return base.GetAttributeValue("Write");
			}
			set
			{
				base.SetAttributeValue("Write", value);
				this.SetDirty();
			}
		}

		internal IISWebDirProperties(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}