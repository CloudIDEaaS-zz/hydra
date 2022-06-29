using System;
using System.Runtime.CompilerServices;

namespace DllExport
{
	public class DllExportNotificationEventArgs : EventArgs
	{
		public string Code
		{
			get;
			set;
		}

		public NotificationContext Context
		{
			get;
			set;
		}

		public SourceCodePosition? EndPosition
		{
			get;
			set;
		}

		public string FileName
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public int Severity
		{
			get;
			set;
		}

		public SourceCodePosition? StartPosition
		{
			get;
			set;
		}

		public DllExportNotificationEventArgs()
		{
		}
	}
}