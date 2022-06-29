using System;

namespace DllExport
{
	public interface IDllExportNotifier
	{
		IDisposable CreateContextName(object context, string name);

		void Notify(int severity, string code, string message, params object[] values);

		void Notify(int severity, string code, string fileName, SourceCodePosition? startPosition, SourceCodePosition? endPosition, string message, params object[] values);

		void Notify(DllExportNotificationEventArgs e);

		event EventHandler<DllExportNotificationEventArgs> Notification;
	}
}