using System;
using System.Runtime.CompilerServices;

namespace DllExport
{
	public sealed class NotificationContext : IEquatable<NotificationContext>
	{
		public object Context
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public NotificationContext(string name, object context)
		{
			this.Name = name;
			this.Context = context;
		}

		public bool Equals(NotificationContext other)
		{
			if (object.ReferenceEquals(null, other))
			{
				return false;
			}
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			if (!object.Equals(this.Context, other.Context))
			{
				return false;
			}
			return string.Equals(this.Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(null, obj))
			{
				return false;
			}
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			if (!(obj is NotificationContext))
			{
				return false;
			}
			return this.Equals((NotificationContext)obj);
		}

		public override int GetHashCode()
		{
			return (this.Context != null ? this.Context.GetHashCode() : 0) * 397 ^ (this.Name != null ? this.Name.GetHashCode() : 0);
		}

		public static bool operator ==(NotificationContext left, NotificationContext right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(NotificationContext left, NotificationContext right)
		{
			return !object.Equals(left, right);
		}
	}
}