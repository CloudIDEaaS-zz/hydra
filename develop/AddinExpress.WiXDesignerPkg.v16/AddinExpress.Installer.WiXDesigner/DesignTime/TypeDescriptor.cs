using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class TypeDescriptor : CustomTypeDescriptor
	{
		private PropertyDescriptorCollection m_pdc = new PropertyDescriptorCollection(null, false);

		private object m_instance;

		private Hashtable m_hashRM = new Hashtable();

		private List<AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor> m_dynamicProperties;

		private SortOrder m_PropertySortOrder = SortOrder.ByIdAscending;

		private SortOrder m_CategorySortOrder = SortOrder.ByIdAscending;

		private ISite m_site;

		private static Hashtable m_HashRef;

		public SortOrder CategorySortOrder
		{
			get
			{
				return this.m_CategorySortOrder;
			}
			set
			{
				this.m_CategorySortOrder = value;
			}
		}

		internal List<AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor> DynamicProperties
		{
			get
			{
				return this.m_dynamicProperties;
			}
			set
			{
				this.m_dynamicProperties = value;
			}
		}

		public SortOrder PropertySortOrder
		{
			get
			{
				return this.m_PropertySortOrder;
			}
			set
			{
				this.m_PropertySortOrder = value;
			}
		}

		static TypeDescriptor()
		{
			AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.m_HashRef = new Hashtable();
		}

		public TypeDescriptor(ICustomTypeDescriptor ctd, object instance) : base(ctd)
		{
			this.m_instance = instance;
		}

		public static bool AddDynamicProperties(object instance, List<AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor> pds)
		{
			AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor value = null;
			foreach (DictionaryEntry mHashRef in AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.m_HashRef)
			{
				WeakReference key = mHashRef.Key as WeakReference;
				if (!key.IsAlive || !instance.Equals(key.Target))
				{
					continue;
				}
				value = mHashRef.Value as AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor;
			}
			if (value == null)
			{
				return false;
			}
			value.DynamicProperties = pds;
			return true;
		}

		private static void CleanUpRef()
		{
			List<WeakReference> weakReferences = new List<WeakReference>();
			foreach (WeakReference key in AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.m_HashRef.Keys)
			{
				if (key.IsAlive)
				{
					continue;
				}
				weakReferences.Add(key);
			}
			foreach (WeakReference weakReference in weakReferences)
			{
				AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.m_HashRef.Remove(weakReference);
			}
		}

		private void GenericPropertyValueUIHandler(ITypeDescriptorContext context, System.ComponentModel.PropertyDescriptor propDesc, ArrayList itemList)
		{
			AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor propertyDescriptor = propDesc as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor;
			if (propertyDescriptor != null)
			{
				itemList.AddRange(propertyDescriptor.StateItems as ICollection);
			}
		}

		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			if (this.m_pdc.Count == 0)
			{
				this.GetProperties();
			}
			PropertyDescriptorCollection propertyDescriptorCollections = new PropertyDescriptorCollection(null);
			foreach (System.ComponentModel.PropertyDescriptor mPdc in this.m_pdc)
			{
				if (!mPdc.Attributes.Contains(attributes))
				{
					continue;
				}
				propertyDescriptorCollections.Add(mPdc);
			}
			this.PreProcess(propertyDescriptorCollections);
			return propertyDescriptorCollections;
		}

		public override PropertyDescriptorCollection GetProperties()
		{
			if (this.m_pdc.Count == 0)
			{
				foreach (System.ComponentModel.PropertyDescriptor property in base.GetProperties())
				{
					if (property is AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor)
					{
						this.m_pdc.Add(property);
					}
					else
					{
						AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor enumPropertyDescriptor = null;
						if (property.PropertyType.IsEnum)
						{
							enumPropertyDescriptor = new EnumPropertyDescriptor(property, this.m_instance);
						}
						else if (property.PropertyType != typeof(bool))
						{
							enumPropertyDescriptor = new AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor(property, this.m_instance);
						}
						else
						{
							enumPropertyDescriptor = new BooleanPropertyDescriptor(property, this.m_instance);
						}
						this.m_pdc.Add(enumPropertyDescriptor);
					}
				}
				if (this.m_dynamicProperties != null)
				{
					foreach (AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor mDynamicProperty in this.m_dynamicProperties)
					{
						this.m_pdc.Add(mDynamicProperty);
					}
				}
			}
			return this.m_pdc;
		}

		public ISite GetSite()
		{
			if (this.m_site == null)
			{
				SimpleSite simpleSite = new SimpleSite();
				IPropertyValueUIService propertyValueUIService = new PropertyValueUIService();
				propertyValueUIService.AddPropertyValueUIHandler(new PropertyValueUIHandler(this.GenericPropertyValueUIHandler));
				simpleSite.AddService<IPropertyValueUIService>(propertyValueUIService);
				this.m_site = simpleSite;
			}
			return this.m_site;
		}

		public static AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor GetTypeDescriptor(object instance)
		{
			AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor value;
			AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.CleanUpRef();
			IDictionaryEnumerator enumerator = AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.m_HashRef.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry current = (DictionaryEntry)enumerator.Current;
					WeakReference key = current.Key as WeakReference;
					if (!key.IsAlive || !instance.Equals(key.Target))
					{
						continue;
					}
					value = current.Value as AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor;
					return value;
				}
				return null;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return value;
		}

		public static bool InstallTypeDescriptor(object instance)
		{
			bool flag;
			AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.CleanUpRef();
			IDictionaryEnumerator enumerator = AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.m_HashRef.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					WeakReference key = ((DictionaryEntry)enumerator.Current).Key as WeakReference;
					if (!key.IsAlive || !instance.Equals(key.Target))
					{
						continue;
					}
					flag = false;
					return flag;
				}
				System.ComponentModel.TypeDescriptionProvider provider = System.ComponentModel.TypeDescriptor.GetProvider(instance);
				AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor typeDescriptor = new AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor(provider.GetTypeDescriptor(instance), instance);
				System.ComponentModel.TypeDescriptor.AddProvider(new AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptionProvider(provider, typeDescriptor), instance);
				WeakReference weakReference = new WeakReference(instance, true);
				AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.m_HashRef.Add(weakReference, typeDescriptor);
				return true;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return flag;
		}

		private void PreProcess(PropertyDescriptorCollection pdc)
		{
			if (pdc.Count > 0)
			{
				this.UpdateStringFromResource(pdc);
				PropertySorter propertySorter = new PropertySorter()
				{
					CategorySortOrder = this.CategorySortOrder,
					PropertySortOrder = this.PropertySortOrder
				};
				PropertyDescriptorCollection propertyDescriptorCollections = pdc.Sort(propertySorter);
				this.UpdateAppendCount(propertyDescriptorCollections);
				pdc.Clear();
				foreach (System.ComponentModel.PropertyDescriptor propertyDescriptor in propertyDescriptorCollections)
				{
					pdc.Add(propertyDescriptor);
				}
			}
		}

		public void ResetProperties()
		{
			this.m_pdc.Clear();
			this.GetProperties();
		}

		private void UpdateAppendCount(PropertyDescriptorCollection pdc)
		{
			if (this.CategorySortOrder == SortOrder.None)
			{
				return;
			}
			int num = 0;
			if (this.CategorySortOrder == SortOrder.ByNameAscending || this.CategorySortOrder == SortOrder.ByNameDescending)
			{
				string category = null;
				for (int i = pdc.Count - 1; i >= 0; i--)
				{
					AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor item = pdc[i] as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor;
					if (category == null)
					{
						category = item.Category;
						item.AppendCount = num;
					}
					else if (string.Compare(item.Category, category, true) != 0)
					{
						num++;
						category = pdc[i].Category;
						item.AppendCount = num;
					}
					else
					{
						item.AppendCount = num;
					}
				}
				return;
			}
			int? nullable = null;
			for (int j = pdc.Count - 1; j >= 0; j--)
			{
				AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor propertyDescriptor = pdc[j] as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor;
				if (!nullable.HasValue)
				{
					nullable = new int?(propertyDescriptor.CategoryId);
					propertyDescriptor.AppendCount = num;
				}
				int? nullable1 = nullable;
				if (!(propertyDescriptor.CategoryId == nullable1.GetValueOrDefault() & nullable1.HasValue))
				{
					num++;
					nullable = new int?(propertyDescriptor.CategoryId);
					propertyDescriptor.AppendCount = num;
				}
				else
				{
					propertyDescriptor.AppendCount = num;
				}
			}
		}

		private void UpdateStringFromResource(PropertyDescriptorCollection pdc)
		{
			ResourceAttribute resourceAttribute = (ResourceAttribute)this.GetAttributes().Get(typeof(ResourceAttribute), true);
			ResourceManager resourceManager = null;
			if (resourceAttribute == null)
			{
				return;
			}
			try
			{
				if (string.IsNullOrEmpty(resourceAttribute.BaseName) || string.IsNullOrEmpty(resourceAttribute.AssemblyFullName))
				{
					resourceManager = (string.IsNullOrEmpty(resourceAttribute.BaseName) ? new ResourceManager(this.m_instance.GetType()) : new ResourceManager(resourceAttribute.BaseName, this.m_instance.GetType().Assembly));
				}
				else
				{
					resourceManager = new ResourceManager(resourceAttribute.BaseName, Assembly.ReflectionOnlyLoad(resourceAttribute.AssemblyFullName));
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
				return;
			}
			string str = (resourceAttribute != null ? resourceAttribute.KeyPrefix : "");
			foreach (AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor propertyDescriptor in pdc)
			{
				if ((LocalizableAttribute)propertyDescriptor.Attributes.Get(typeof(LocalizableAttribute), true) != null && !propertyDescriptor.IsLocalizable || propertyDescriptor.LCID == CultureInfo.CurrentUICulture.LCID)
				{
					continue;
				}
				string empty = string.Empty;
				string empty1 = string.Empty;
				if (!string.IsNullOrEmpty(propertyDescriptor.CategoryResourceKey))
				{
					empty = string.Concat(str, propertyDescriptor.CategoryResourceKey);
					empty1 = string.Empty;
					try
					{
						empty1 = resourceManager.GetString(empty);
						if (!string.IsNullOrEmpty(empty1))
						{
							propertyDescriptor.Attributes.Add(new CategoryAttribute(empty1), true);
						}
					}
					catch (Exception exception1)
					{
						Console.WriteLine(string.Format("Key '{0}' does not exist in the resource.", empty));
					}
				}
				empty = string.Concat(str, propertyDescriptor.Name, "_Name");
				empty1 = string.Empty;
				try
				{
					empty1 = resourceManager.GetString(empty);
					if (!string.IsNullOrEmpty(empty1))
					{
						propertyDescriptor.Attributes.Add(new System.ComponentModel.DisplayNameAttribute(empty1), typeof(System.ComponentModel.DisplayNameAttribute));
					}
				}
				catch (Exception exception2)
				{
					Console.WriteLine(string.Format("Key '{0}' does not exist in the resource.", empty));
				}
				empty = string.Concat(str, propertyDescriptor.Name, "_Desc");
				empty1 = string.Empty;
				try
				{
					empty1 = resourceManager.GetString(empty);
					if (!string.IsNullOrEmpty(empty1))
					{
						propertyDescriptor.Attributes.Add(new DescriptionAttribute(empty1), true);
					}
				}
				catch (Exception exception3)
				{
					Console.WriteLine(string.Format("Key '{0}' does not exist in the resource.", empty));
				}
			}
		}
	}
}