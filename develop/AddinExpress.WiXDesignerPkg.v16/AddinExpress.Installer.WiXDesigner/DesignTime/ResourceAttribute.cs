using System;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
	internal class ResourceAttribute : Attribute
	{
		public string AssemblyFullName
		{
			get;
			set;
		}

		public string BaseName
		{
			get;
			set;
		}

		public string KeyPrefix
		{
			get;
			set;
		}

		public ResourceAttribute()
		{
		}

		public ResourceAttribute(string baseString)
		{
			this.BaseName = baseString;
		}

		public ResourceAttribute(string baseString, string keyPrefix)
		{
			this.BaseName = baseString;
			this.KeyPrefix = keyPrefix;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ResourceAttribute))
			{
				return false;
			}
			ResourceAttribute resourceAttribute = obj as ResourceAttribute;
			if (string.Compare(this.BaseName, resourceAttribute.BaseName, true) == 0 && string.Compare(this.AssemblyFullName, resourceAttribute.AssemblyFullName, true) == 0)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.BaseName.GetHashCode() ^ this.KeyPrefix.GetHashCode() ^ this.AssemblyFullName.GetHashCode();
		}

		public override bool Match(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (!(obj is ResourceAttribute))
			{
				return false;
			}
			return (((ResourceAttribute)obj).GetHashCode() & this.GetHashCode()) == this.GetHashCode();
		}
	}
}