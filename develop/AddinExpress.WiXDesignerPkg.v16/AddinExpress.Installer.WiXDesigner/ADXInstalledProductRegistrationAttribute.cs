using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AddinExpress.Installer.WiXDesigner
{
	[AttributeUsage(AttributeTargets.Class, Inherited=false)]
	[ComVisible(false)]
	public sealed class ADXInstalledProductRegistrationAttribute : RegistrationAttribute
	{
		private bool _usePackage;

		private bool _useInterface;

		private string _name = string.Empty;

		private bool _useVsProductId;

		private string _iconResourceId = string.Empty;

		private string _productDetails = string.Empty;

		private string _productId = string.Empty;

		private string _productName = string.Empty;

		public int IconResourceID
		{
			get
			{
				if (string.IsNullOrEmpty(this._iconResourceId) || this._iconResourceId.Length < 2)
				{
					return 0;
				}
				return int.Parse(this._iconResourceId.Substring(1), CultureInfo.InvariantCulture);
			}
			set
			{
				this._iconResourceId = string.Concat("#", value.ToString(CultureInfo.InvariantCulture));
			}
		}

		public string LanguageIndependentName
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public string ProductDetails
		{
			get
			{
				return this._productDetails;
			}
		}

		public int ProductDetailsResourceID
		{
			get
			{
				if (this._productDetails == null || this._productDetails.Length < 2 || this._productDetails[0] != '#' || !char.IsDigit(this._productDetails[1]))
				{
					return 0;
				}
				return int.Parse(this._productDetails.Substring(1), CultureInfo.InvariantCulture);
			}
		}

		public string ProductId
		{
			get
			{
				return this._productId;
			}
		}

		public string ProductName
		{
			get
			{
				return this._productName;
			}
		}

		public int ProductNameResourceID
		{
			get
			{
				if (this._productName == null || this._productName.Length < 2 || this._productName[0] != '#' || !char.IsDigit(this._productName[1]))
				{
					return 0;
				}
				return int.Parse(this._productName.Substring(1), CultureInfo.InvariantCulture);
			}
		}

		public bool UseInterface
		{
			get
			{
				return this._useInterface;
			}
		}

		public bool UsePackage
		{
			get
			{
				return this._usePackage;
			}
		}

		public bool UseVsProductId
		{
			get
			{
				return this._useVsProductId;
			}
		}

		public ADXInstalledProductRegistrationAttribute(string productName, string productDetails, string productId) : this(productName, productDetails, productId, false)
		{
			if (string.IsNullOrEmpty(productId))
			{
				productId = base.GetType().Assembly.GetName().Version.ToString(4);
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ADXInstalledProductRegistrationAttribute(string productName, string productDetails, string productId, bool useVsProductId)
		{
			this._useVsProductId = useVsProductId;
			if (string.IsNullOrEmpty(productId))
			{
				productId = base.GetType().Assembly.GetName().Version.ToString(3);
			}
			this.Initialize(productName, productDetails, productId);
		}

		private string GetNonEmptyName(RegistrationAttribute.RegistrationContext context)
		{
			string languageIndependentName = this.LanguageIndependentName;
			if (languageIndependentName != null)
			{
				languageIndependentName = languageIndependentName.Trim();
			}
			if (string.IsNullOrEmpty(languageIndependentName))
			{
				languageIndependentName = context.ComponentType.Name;
			}
			return languageIndependentName;
		}

		private void Initialize(string productName, string productDetails, string productId)
		{
			if (this.UseInterface)
			{
				throw new ArgumentException(string.Format(Resources.Culture, Resources.Reg_ErrorIncompatibleParametersValues, new object[] { "useInterface", "productName" }));
			}
			if (productName == null || productName.Trim().Length == 0)
			{
				throw new ArgumentNullException("productName");
			}
			productName = productName.Trim();
			if (productDetails != null)
			{
				productDetails = productDetails.Trim();
			}
			if (!this.UseVsProductId)
			{
				if (productId == null || productId.Trim().Length == 0)
				{
					throw new ArgumentNullException("productId");
				}
				productId = productId.Trim();
			}
			else if (productId != null)
			{
				throw new ArgumentException(string.Format(Resources.Culture, Resources.Reg_ErrorIncompatibleParametersValues, new object[] { "productId", "useVsProductId" }));
			}
			this._productName = productName;
			this._productDetails = productDetails;
			this._productId = productId;
			if (!string.IsNullOrEmpty(this.ProductDetails) && (this.ProductNameResourceID != 0 && this.ProductDetailsResourceID == 0 || this.ProductNameResourceID == 0 && this.ProductDetailsResourceID != 0))
			{
				throw new ArgumentException(string.Format(Resources.Culture, Resources.Reg_ErrorIncompatibleParametersTypes, new object[] { "productName", "productDetails" }));
			}
			this._usePackage = (this.ProductNameResourceID != 0 || this.ProductDetailsResourceID != 0 ? true : this.IconResourceID != 0);
		}

		public override void Register(RegistrationAttribute.RegistrationContext context)
		{
			if (!this.UseInterface)
			{
				context.Log.WriteLine(Resources.Reg_NotifyInstalledProduct, this.GetNonEmptyName(context), this.ProductId ?? this.ProductName);
			}
			else
			{
				context.Log.WriteLine(Resources.Reg_NotifyInstalledProductInterface);
			}
			using (RegistrationAttribute.Key key = context.CreateKey(this.RegKeyName(context)))
			{
				if (this.UsePackage)
				{
					Guid gUID = context.ComponentType.GUID;
					key.SetValue("Package", gUID.ToString("B").ToUpper());
				}
				if (!string.IsNullOrEmpty(this._name))
				{
					key.SetValue("UseRegNameAsSplashName", 1);
				}
				if (!this.UseInterface)
				{
					key.SetValue("", this.ProductName);
					if (!this.UseVsProductId)
					{
						key.SetValue("PID", this.ProductId);
					}
					else
					{
						key.SetValue("UseVsProductID", 1);
					}
					if (!string.IsNullOrEmpty(this.ProductDetails))
					{
						key.SetValue("ProductDetails", this.ProductDetails);
					}
					if (this.UsePackage && !string.IsNullOrEmpty(this._iconResourceId))
					{
						key.SetValue("LogoID", this._iconResourceId);
					}
				}
				else
				{
					key.SetValue("UseInterface", 1);
				}
			}
		}

		private string RegKeyName(RegistrationAttribute.RegistrationContext context)
		{
			return string.Format(CultureInfo.InvariantCulture, "InstalledProducts\\{0}", new object[] { this.GetNonEmptyName(context) });
		}

		public override void Unregister(RegistrationAttribute.RegistrationContext context)
		{
			context.RemoveKey(this.RegKeyName(context));
		}
	}
}