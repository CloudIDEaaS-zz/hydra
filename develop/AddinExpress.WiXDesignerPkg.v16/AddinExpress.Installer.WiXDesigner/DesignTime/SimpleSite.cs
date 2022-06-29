using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal sealed class SimpleSite : ISite, IServiceProvider
	{
		private readonly IContainer container = new System.ComponentModel.Container();

		private Dictionary<Type, object> services;

		public IComponent Component
		{
			get
			{
				return JustDecompileGenerated_get_Component();
			}
			set
			{
				JustDecompileGenerated_set_Component(value);
			}
		}

		private IComponent JustDecompileGenerated_Component_k__BackingField;

		public IComponent JustDecompileGenerated_get_Component()
		{
			return this.JustDecompileGenerated_Component_k__BackingField;
		}

		public void JustDecompileGenerated_set_Component(IComponent value)
		{
			this.JustDecompileGenerated_Component_k__BackingField = value;
		}

		public bool DesignMode
		{
			get
			{
				return JustDecompileGenerated_get_DesignMode();
			}
			set
			{
				JustDecompileGenerated_set_DesignMode(value);
			}
		}

		private bool JustDecompileGenerated_DesignMode_k__BackingField;

		public bool JustDecompileGenerated_get_DesignMode()
		{
			return this.JustDecompileGenerated_DesignMode_k__BackingField;
		}

		public void JustDecompileGenerated_set_DesignMode(bool value)
		{
			this.JustDecompileGenerated_DesignMode_k__BackingField = value;
		}

		public string Name
		{
			get;
			set;
		}

		IContainer System.ComponentModel.ISite.Container
		{
			get
			{
				return this.container;
			}
		}

		public SimpleSite()
		{
		}

		public void AddService<T>(T service)
		where T : class
		{
			if (this.services == null)
			{
				this.services = new Dictionary<Type, object>();
			}
			this.services[typeof(T)] = service;
		}

		public void RemoveService<T>()
		where T : class
		{
			if (this.services != null)
			{
				this.services.Remove(typeof(T));
			}
		}

		object System.IServiceProvider.GetService(Type serviceType)
		{
			object obj;
			if (this.services != null && this.services.TryGetValue(serviceType, out obj))
			{
				return obj;
			}
			return null;
		}
	}
}