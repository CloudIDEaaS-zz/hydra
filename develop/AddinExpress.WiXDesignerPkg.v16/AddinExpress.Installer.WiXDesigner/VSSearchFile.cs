using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSSearchFile : VSSearchBase
	{
		private WiXDirectorySearch _dirSearch;

		[Browsable(true)]
		[DefaultValue(0)]
		[Description("Specifies the number of levels of subfolders to search for a file")]
		public int Depth
		{
			get
			{
				if (this._dirSearch == null)
				{
					return 0;
				}
				return Convert.ToInt32(this._dirSearch.Depth);
			}
			set
			{
				if (this._dirSearch != null)
				{
					this._dirSearch.Depth = value.ToString();
				}
			}
		}

		internal WiXDirectorySearch DirectorySearch
		{
			get
			{
				return this._dirSearch;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the name of a file to search for")]
		public string FileName
		{
			get
			{
				if (base.WiXElement == null)
				{
					return string.Empty;
				}
				return (base.WiXElement as WiXFileSearch).Name;
			}
			set
			{
				if (base.WiXElement != null)
				{
					(base.WiXElement as WiXFileSearch).Name = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the folder where a file search start")]
		[TypeConverter(typeof(VSSearchFileFolderConverter))]
		public string Folder
		{
			get
			{
				if (this._dirSearch == null)
				{
					return string.Empty;
				}
				return this._dirSearch.Path;
			}
			set
			{
				if (this._dirSearch != null)
				{
					this._dirSearch.Path = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the latest date for a file in a file search")]
		public string MaxDate
		{
			get
			{
				if (base.WiXElement == null)
				{
					return string.Empty;
				}
				return (base.WiXElement as WiXFileSearch).MaxDate;
			}
			set
			{
				if (base.WiXElement != null)
				{
					(base.WiXElement as WiXFileSearch).MaxDate = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the maximum size in bytes for a file in a file search")]
		public string MaxSize
		{
			get
			{
				if (base.WiXElement == null)
				{
					return string.Empty;
				}
				return (base.WiXElement as WiXFileSearch).MaxSize;
			}
			set
			{
				if (base.WiXElement != null)
				{
					(base.WiXElement as WiXFileSearch).MaxSize = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the maximum version number for a file in a file search")]
		public string MaxVersion
		{
			get
			{
				if (base.WiXElement == null)
				{
					return string.Empty;
				}
				return (base.WiXElement as WiXFileSearch).MaxVersion;
			}
			set
			{
				if (base.WiXElement != null)
				{
					(base.WiXElement as WiXFileSearch).MaxVersion = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the earliest date for a file in a file search")]
		public string MinDate
		{
			get
			{
				if (base.WiXElement == null)
				{
					return string.Empty;
				}
				return (base.WiXElement as WiXFileSearch).MinDate;
			}
			set
			{
				if (base.WiXElement != null)
				{
					(base.WiXElement as WiXFileSearch).MinDate = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the manimum size in bytes for a file in a file search")]
		public string MinSize
		{
			get
			{
				if (base.WiXElement == null)
				{
					return string.Empty;
				}
				return (base.WiXElement as WiXFileSearch).MinSize;
			}
			set
			{
				if (base.WiXElement != null)
				{
					(base.WiXElement as WiXFileSearch).MinSize = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the manimum version number for a file in a file search")]
		public string MinVersion
		{
			get
			{
				if (base.WiXElement == null)
				{
					return string.Empty;
				}
				return (base.WiXElement as WiXFileSearch).MinVersion;
			}
			set
			{
				if (base.WiXElement != null)
				{
					(base.WiXElement as WiXFileSearch).MinVersion = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the name used in the Launch Condition Editor to identify a selected file search")]
		[DisplayName("(Name)")]
		[ReadOnly(false)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		public VSSearchFile()
		{
		}

		public VSSearchFile(WiXProjectParser project, WiXEntity wixElement, WiXDirectorySearch dirSearch, AddinExpress.Installer.WiXDesigner.WiXProperty wixProperty, VSSearches collection) : base(project, wixElement, wixProperty, collection)
		{
			this._dirSearch = dirSearch;
		}

		public override void Delete()
		{
			this._dirSearch.Delete();
			base.Delete();
		}
	}
}