using DllExport;
using System;
using System.Runtime.CompilerServices;

namespace DllExport.Parsing
{
	public abstract class IlToolBase : HasServiceProvider
	{
		public IInputValues InputValues
		{
			get;
			private set;
		}

		protected IDllExportNotifier Notifier
		{
			get
			{
				return base.ServiceProvider.GetService<IDllExportNotifier>();
			}
		}

		public string TempDirectory { get; set; }
		public int Timeout { get; set; }

		protected IlToolBase(IServiceProvider serviceProvider, IInputValues inputValues) : base(serviceProvider)
		{
			if (inputValues == null)
			{
				throw new ArgumentNullException("inputValues");
			}
			this.InputValues = inputValues;
		}
	}
}