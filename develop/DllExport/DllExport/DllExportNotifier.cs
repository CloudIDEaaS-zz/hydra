using DllExport.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DllExport
{
	public class DllExportNotifier : IDllExportNotifier, IDisposable
	{
		private readonly Stack<NotificationContext> _ContextScopes = new Stack<NotificationContext>();

		public NotificationContext Context
		{
			get
			{
				NotificationContext notificationContext;
				try
				{
					notificationContext = this._ContextScopes.Peek();
				}
				catch (Exception exception)
				{
					throw exception;
				}
				return notificationContext;
			}
		}

		public string ContextName
		{
			get
			{
				NotificationContext notifyContext = this.Context;
				if (notifyContext == null)
				{
					return null;
				}
				return notifyContext.Name;
			}
		}

		public object ContextObject
		{
			get
			{
				NotificationContext notifyContext = this.Context;
				if (notifyContext == null)
				{
					return null;
				}
				return notifyContext.Context;
			}
		}

		public DllExportNotifier()
		{
		}

		public IDisposable CreateContextName(object context, string name)
		{
			return new DllExportNotifier.ContextScope(this, new NotificationContext(name, context));
		}

		public void Dispose()
		{
			this.Notification = null;
		}

		public void Notify(DllExportNotificationEventArgs e)
		{
			this.OnNotification(this.Context ?? new NotificationContext(null, this), e);
		}

		public void Notify(int severity, string code, string message, params object[] values)
		{
			SourceCodePosition? nullable = null;
			SourceCodePosition? nullable1 = null;

			this.Notify(severity, code, null, nullable, nullable1, message, values);
		}

		public void Notify(int severity, string code, string fileName, SourceCodePosition? startPosition, SourceCodePosition? endPosition, string message, params object[] values)
		{
			DllExportNotificationEventArgs dllExportNotificationEventArg = new DllExportNotificationEventArgs()
			{
				Severity = severity,
				Code = code,
				Context = this.Context,
				FileName = fileName,
				StartPosition = startPosition,
				EndPosition = endPosition
			};

			DllExportNotificationEventArgs e = dllExportNotificationEventArg;
			
			e.Message = (values.NullSafeCall<object[], int>(() => (int)values.Length) == 0 ? message : string.Format(CultureInfo.InvariantCulture, message, values));

			if (!string.IsNullOrEmpty(e.Message))
			{
				this.Notify(e);
			}
		}

		private void OnNotification(object sender, DllExportNotificationEventArgs e)
		{
			EventHandler<DllExportNotificationEventArgs> h = this.Notification;
			if (h != null)
			{
				h(sender, e);
			}
		}

		public event EventHandler<DllExportNotificationEventArgs> Notification;

		private sealed class ContextScope : IDisposable
		{
			private readonly DllExportNotifier _Notifier;

			public NotificationContext Context
			{
				get;
				private set;
			}

			public ContextScope(DllExportNotifier notifier, NotificationContext context)
			{
				this.Context = context;
				this._Notifier = notifier;
				Stack<NotificationContext> contextScopes = this._Notifier._ContextScopes;
				lock (contextScopes)
				{
					contextScopes.Push(context);
				}
			}

			public void Dispose()
			{
				Stack<NotificationContext> contextScopes = this._Notifier._ContextScopes;
				lock (contextScopes)
				{
					if (contextScopes.Peek() != this.Context)
					{
						throw new InvalidOperationException(string.Format(Resources.Current_Notifier_Context_is___0____it_should_have_been___1___, contextScopes.Peek(), this.Context.Name));
					}
					contextScopes.Pop();
				}
			}
		}
	}
}