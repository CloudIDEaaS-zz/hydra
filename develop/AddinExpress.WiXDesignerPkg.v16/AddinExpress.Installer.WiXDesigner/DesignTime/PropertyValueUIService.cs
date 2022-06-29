using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class PropertyValueUIService : IPropertyValueUIService
	{
		private PropertyValueUIHandler m_ValueUIHandler;

		private EventHandler m_NotifyHandler;

		public PropertyValueUIService()
		{
		}

		void System.Drawing.Design.IPropertyValueUIService.AddPropertyValueUIHandler(PropertyValueUIHandler newHandler)
		{
			if (newHandler == null)
			{
				throw new ArgumentNullException("newHandler");
			}
			lock (this)
			{
				this.m_ValueUIHandler += newHandler;
			}
		}

		PropertyValueUIItem[] System.Drawing.Design.IPropertyValueUIService.GetPropertyUIValueItems(ITypeDescriptorContext context, System.ComponentModel.PropertyDescriptor propDesc)
		{
			PropertyValueUIItem[] array;
			if (propDesc == null)
			{
				throw new ArgumentNullException("propDesc");
			}
			if (this.m_ValueUIHandler == null)
			{
				return new PropertyValueUIItem[0];
			}
			lock (this)
			{
				ArrayList arrayLists = new ArrayList();
				this.m_ValueUIHandler(context, propDesc, arrayLists);
				array = (PropertyValueUIItem[])arrayLists.ToArray(typeof(PropertyValueUIItem));
			}
			return array;
		}

		void System.Drawing.Design.IPropertyValueUIService.NotifyPropertyValueUIItemsChanged()
		{
			if (this.m_NotifyHandler != null)
			{
				this.m_NotifyHandler(this, EventArgs.Empty);
			}
		}

		void System.Drawing.Design.IPropertyValueUIService.RemovePropertyValueUIHandler(PropertyValueUIHandler newHandler)
		{
			if (newHandler == null)
			{
				throw new ArgumentNullException("newHandler");
			}
			this.m_ValueUIHandler -= newHandler;
		}

		event EventHandler System.Drawing.Design.IPropertyValueUIService.PropertyUIValueItemsChanged
		{
			add
			{
				lock (this)
				{
					this.m_NotifyHandler += value;
				}
			}
			remove
			{
				lock (this)
				{
					this.m_NotifyHandler -= value;
				}
			}
		}
	}
}