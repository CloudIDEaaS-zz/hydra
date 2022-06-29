using System;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class StandardValue
	{
		private object m_Value;

		public string Description
		{
			get;
			set;
		}

		public string DisplayName
		{
			get;
			set;
		}

		public bool Enabled
		{
			get;
			set;
		}

		public object Value
		{
			get
			{
				return this.m_Value;
			}
		}

		public bool Visible
		{
			get;
			set;
		}

		public StandardValue(object value)
		{
			this.m_Value = value;
			this.Enabled = true;
			this.Visible = true;
		}

		public StandardValue(object value, string displayName)
		{
			this.DisplayName = displayName;
			this.m_Value = value;
			this.Enabled = true;
			this.Visible = true;
		}

		public StandardValue(object value, string displayName, string description)
		{
			this.m_Value = value;
			this.DisplayName = displayName;
			this.Description = description;
			this.Enabled = true;
			this.Visible = true;
		}

		public override string ToString()
		{
			if (!string.IsNullOrWhiteSpace(this.DisplayName) || this.Value == null)
			{
				return this.DisplayName;
			}
			return this.Value.ToString();
		}
	}
}