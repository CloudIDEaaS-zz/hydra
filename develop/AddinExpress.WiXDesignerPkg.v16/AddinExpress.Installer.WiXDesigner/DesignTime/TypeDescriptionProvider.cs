using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class TypeDescriptionProvider : System.ComponentModel.TypeDescriptionProvider
	{
		private System.ComponentModel.TypeDescriptionProvider m_parent;

		private ICustomTypeDescriptor m_ctd;

		public TypeDescriptionProvider()
		{
		}

		public TypeDescriptionProvider(System.ComponentModel.TypeDescriptionProvider parent) : base(parent)
		{
			this.m_parent = parent;
		}

		public TypeDescriptionProvider(System.ComponentModel.TypeDescriptionProvider parent, ICustomTypeDescriptor ctd) : base(parent)
		{
			this.m_parent = parent;
			this.m_ctd = ctd;
		}

		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			return this.m_ctd;
		}
	}
}