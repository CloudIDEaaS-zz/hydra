using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSWebCustomFolder : VSWebFolder
	{
		private List<VSWebApplicationExtension> _vsWebApplicationExtensions = new List<VSWebApplicationExtension>();

		[Browsable(true)]
		[DefaultValue(AppProtection.vsdapMedium)]
		[Description("Sets the Internet Information Services Application Protection property for the selected folder")]
		public AppProtection ApplicationProtection
		{
			get
			{
				if (base.WiXWebApplication != null)
				{
					if (base.WiXWebApplication.Isolation == "low")
					{
						return AppProtection.vsdapLow;
					}
					if (base.WiXWebApplication.Isolation == "high")
					{
						return AppProtection.vsdapHigh;
					}
				}
				return AppProtection.vsdapMedium;
			}
			set
			{
				if (base.WiXWebApplication != null)
				{
					switch (value)
					{
						case AppProtection.vsdapLow:
						{
							base.WiXWebApplication.Isolation = "low";
							return;
						}
						case AppProtection.vsdapMedium:
						{
							base.WiXWebApplication.Isolation = "medium";
							return;
						}
						case AppProtection.vsdapHigh:
						{
							base.WiXWebApplication.Isolation = "high";
							break;
						}
						default:
						{
							return;
						}
					}
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(null)]
		[Description("Sets the Internet Information Services Application Mappings property for the selected folder")]
		[Editor(typeof(AppMappingsPropertyEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(AppMappingsPropertyConverter))]
		public List<VSWebApplicationExtension> AppMappings
		{
			get
			{
				this._vsWebApplicationExtensions.Clear();
				if (base.WiXWebApplication != null)
				{
					foreach (IISWebApplicationExtension iSWebApplicationExtension in base.WiXWebApplication.ChildEntities.FindAll((WiXEntity e) => e is IISWebApplicationExtension).ConvertAll<IISWebApplicationExtension>((WiXEntity e) => e as IISWebApplicationExtension))
					{
						this._vsWebApplicationExtensions.Add(new VSWebApplicationExtension(iSWebApplicationExtension));
					}
				}
				return this._vsWebApplicationExtensions;
			}
			set
			{
				if (base.WiXWebApplication != null && value != null && value != null)
				{
					List<IISWebApplicationExtension> iSWebApplicationExtensions = base.WiXWebApplication.ChildEntities.FindAll((WiXEntity e) => e is IISWebApplicationExtension).ConvertAll<IISWebApplicationExtension>((WiXEntity e) => e as IISWebApplicationExtension);
					for (int i = iSWebApplicationExtensions.Count - 1; i >= 0; i--)
					{
						iSWebApplicationExtensions[i].Delete();
					}
					foreach (VSWebApplicationExtension vSWebApplicationExtension in value)
					{
						XmlNode xmlNodes = Common.CreateXmlElementWithAttributes(base.WiXWebApplication.XmlNode.OwnerDocument, "WebApplicationExtension", base.WiXWebApplication.XmlNode.NamespaceURI, new string[] { "Executable", "Extension", "Verbs" }, new string[] { vSWebApplicationExtension.Executable, vSWebApplicationExtension.Extension, vSWebApplicationExtension.Verbs }, base.WiXWebApplication.XmlNode.Prefix, false);
						base.WiXWebApplication.XmlNode.AppendChild(xmlNodes);
						IISWebApplicationExtension iSWebApplicationExtension = new IISWebApplicationExtension(this._project, base.WiXWebApplication.Owner, base.WiXWebApplication, xmlNodes);
					}
					base.WiXWebApplication.SetDirty();
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("default.aspx")]
		[Description("Specifies the default (startup) document for the selected folder")]
		public string DefaultDocument
		{
			get
			{
				if (base.WiXWebDirProperties == null)
				{
					return string.Empty;
				}
				return base.WiXWebDirProperties.DefaultDocuments;
			}
			set
			{
				if (base.WiXWebDirProperties != null)
				{
					base.WiXWebDirProperties.DefaultDocuments = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(true)]
		[Description("Specifies whether an Internet Information Services application root will be created for the selected folder")]
		public bool IsApplication
		{
			get
			{
				return base.WiXWebApplication != null;
			}
			set
			{
				if (!value)
				{
					base.WiXWebApplication.Delete();
					base.WiXWebApplication = null;
				}
				else if (base.WiXWebApplication == null && this._project.WebSetupParameters.AppPools.Count > 0)
				{
					XmlNode xmlNodes = Common.CreateXmlElementWithAttributes(base.WiXWebVirtualDir.XmlNode.OwnerDocument, "WebApplication", base.WiXWebVirtualDir.XmlNode.NamespaceURI, new string[] { "Id", "Name", "Isolation", "WebAppPool" }, new string[] { string.Concat("webapp_", this.Property), base.WiXWebVirtualDir.Alias, "medium", this._project.WebSetupParameters.AppPools[0].Id }, base.WiXWebVirtualDir.XmlNode.Prefix, false);
					base.WiXWebVirtualDir.XmlNode.AppendChild(xmlNodes);
					base.WiXWebApplication = new IISWebApplication(this._project, base.WiXWebVirtualDir.Owner, base.WiXWebVirtualDir, xmlNodes);
					base.WiXWebVirtualDir.SetDirty();
					return;
				}
			}
		}

		[Browsable(true)]
		[ReadOnly(false)]
		public override string Property
		{
			get
			{
				return base.Property;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (base.WiXWebVirtualDir != null)
					{
						base.WiXWebVirtualDir.Directory = value;
					}
					base.Property = value.ToUpper();
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies a directory relative to a web server where a Web application folder will be installed")]
		public string VirtualDirectory
		{
			get
			{
				if (base.WiXWebVirtualDir != null)
				{
					return base.WiXWebVirtualDir.Alias;
				}
				if (base.WiXWebApplication == null)
				{
					return string.Empty;
				}
				return base.WiXWebApplication.Name;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (base.WiXWebVirtualDir != null)
					{
						base.WiXWebVirtualDir.Alias = value;
					}
					if (base.WiXWebApplication != null)
					{
						base.WiXWebApplication.Name = value;
					}
				}
			}
		}

		public VSWebCustomFolder(WiXProjectParser project, VSBaseFolder parent, WiXDirectory wixElement) : base(project, parent, wixElement)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._vsWebApplicationExtensions.Clear();
			}
			base.Dispose(disposing);
		}
	}
}