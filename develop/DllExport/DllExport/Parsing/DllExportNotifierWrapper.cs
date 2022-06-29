using DllExport;
using System;
using System.Runtime.CompilerServices;

namespace DllExport.Parsing
{
	public abstract class DllExportNotifierWrapper : IDllExportNotifier, IDisposable
	{
		protected IDllExportNotifier Notifier
		{
			get
			{
				return JustDecompileGenerated_get_Notifier();
			}
			set
			{
				JustDecompileGenerated_set_Notifier(value);
			}
		}

		private IDllExportNotifier JustDecompileGenerated_Notifier_k__BackingField;

		protected virtual IDllExportNotifier JustDecompileGenerated_get_Notifier()
		{
			return this.JustDecompileGenerated_Notifier_k__BackingField;
		}

		private void JustDecompileGenerated_set_Notifier(IDllExportNotifier value)
		{
			this.JustDecompileGenerated_Notifier_k__BackingField = value;
		}

		protected virtual bool OwnsNotifier
		{
			get
			{
				return false;
			}
		}

		protected DllExportNotifierWrapper(IDllExportNotifier notifier)
		{
			this.Notifier = notifier;
		}

		public IDisposable CreateContextName(object context, string name)
		{
			return this.Notifier.CreateContextName(context, name);
		}

		public void Dispose()
		{
			if (!this.OwnsNotifier)
			{
				return;
			}
			IDisposable disposable = this.Notifier as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		public void Notify(DllExportNotificationEventArgs e)
		{
			this.Notifier.Notify(e);
		}

		public void Notify(int severity, string code, string message, params object[] values)
		{
			this.Notifier.Notify(severity, code, message, values);
		}

		public void Notify(int severity, string code, string fileName, SourceCodePosition? startPosition, SourceCodePosition? endPosition, string message, params object[] values)
		{
			this.Notifier.Notify(severity, code, fileName, startPosition, endPosition, message, values);
		}

		event EventHandler<DllExportNotificationEventArgs> DllExport.IDllExportNotifier.Notification
		{
			add
			{
				this.Notifier.Notification += value;
			}
			remove
			{
				this.Notifier.Notification -= value;
			}
		}
	}
}