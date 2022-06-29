using System;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class ValueProperty
	{
		public string Name;

		public ValueProperty(BuiltInProperties.SystemChecks cannedProperty)
		{
			this.Name = BuiltInProperties.SystemChecksEnumToString(cannedProperty);
		}

		public ValueProperty(string name)
		{
			this.Name = name;
		}
	}
}