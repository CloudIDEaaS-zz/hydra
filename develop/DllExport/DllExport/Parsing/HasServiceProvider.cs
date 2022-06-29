using System;

namespace DllExport.Parsing
{
	public abstract class HasServiceProvider : IDisposable, IServiceProvider
	{
		private readonly IServiceProvider _ServiceProvider;

		public IServiceProvider ServiceProvider
		{
			get
			{
				return this._ServiceProvider;
			}
		}

		protected HasServiceProvider(IServiceProvider serviceProvider)
		{
			this._ServiceProvider = serviceProvider;
		}

		public void Dispose()
		{
			IDisposable d = this._ServiceProvider as IDisposable;
			if (d == null)
			{
				return;
			}
			d.Dispose();
		}

		object System.IServiceProvider.GetService(Type serviceType)
		{
			return this._ServiceProvider.GetService(serviceType);
		}
	}
}