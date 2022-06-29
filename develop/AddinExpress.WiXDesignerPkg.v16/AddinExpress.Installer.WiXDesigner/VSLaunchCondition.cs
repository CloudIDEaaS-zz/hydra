using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSLaunchCondition : VSComponentBase
	{
		private WiXCondition _wixElement;

		private WiXProjectParser _project;

		private VSLaunchConditions _collection;

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies a condition that must be satisfied (evaluate to true) at installation time on a target computer")]
		public string Condition
		{
			get
			{
				if (this._wixElement == null)
				{
					return string.Empty;
				}
				return this._wixElement.Condition;
			}
			set
			{
				if (this._wixElement != null)
				{
					this._wixElement.Condition = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies a message to be displayed when a condition evaluates to false at installation time")]
		public string Message
		{
			get
			{
				if (this._wixElement == null)
				{
					return string.Empty;
				}
				return this._wixElement.Message;
			}
			set
			{
				if (this._wixElement != null)
				{
					this._wixElement.Message = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the name used in the Launch Conditions Editor to identify a selected launch condition")]
		[DisplayName("(Name)")]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				int num = this._project.LaunchConditions.IndexOf(this) + 1;
				return string.Concat("Condition #", num.ToString());
			}
			set
			{
			}
		}

		internal WiXCondition WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		public VSLaunchCondition()
		{
		}

		public VSLaunchCondition(WiXProjectParser project, WiXCondition wixElement, VSLaunchConditions collection) : this()
		{
			this._project = project;
			this._wixElement = wixElement;
			this._collection = collection;
		}

		public override void Delete()
		{
			this.WiXElement.Delete();
			this._collection.Remove(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._project = null;
				this._wixElement = null;
				this._collection = null;
			}
			base.Dispose(disposing);
		}

		protected override string GetClassName()
		{
			return "Launch Condition Properties";
		}

		protected override string GetComponentName()
		{
			return this.Name;
		}
	}
}