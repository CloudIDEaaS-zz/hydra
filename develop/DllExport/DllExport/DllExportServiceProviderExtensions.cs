using System;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;

namespace DllExport
{
	internal static class DllExportServiceProviderExtensions
	{
		public static TServiceProvider AddService<TServiceProvider, TService>(this TServiceProvider serviceProvider, TService service)
		where TServiceProvider : IServiceContainer
		{
			serviceProvider.AddService(typeof(TService), service);
			return serviceProvider;
		}

		public static TServiceProvider AddService<TServiceProvider, TService>(this TServiceProvider serviceProvider, TService service, bool promote)
		where TServiceProvider : IServiceContainer
		{
			serviceProvider.AddService(typeof(TService), service, promote);
			return serviceProvider;
		}

		public static TServiceProvider AddServiceFactory<TServiceProvider, TService>(this TServiceProvider serviceProvider, Func<IServiceProvider, TService> serviceFactory)
		where TServiceProvider : IServiceContainer
		{
			serviceProvider.AddService(typeof(TService), (IServiceContainer sp, Type t) => serviceFactory(sp));
			return serviceProvider;
		}

		public static TServiceProvider AddServiceFactory<TServiceProvider, TService>(this TServiceProvider serviceProvider, Func<IServiceProvider, TService> serviceFactory, bool promote)
		where TServiceProvider : IServiceContainer
		{
			serviceProvider.AddService(typeof(TService), (IServiceContainer sp, Type t) => serviceFactory(sp), promote);
			return serviceProvider;
		}

		public static TService GetService<TService>(this IServiceProvider serviceProvider)
		{
			return (TService)serviceProvider.GetService(typeof(TService));
		}
	}
}