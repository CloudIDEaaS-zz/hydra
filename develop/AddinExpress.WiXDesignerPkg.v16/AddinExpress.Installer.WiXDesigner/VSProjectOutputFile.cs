using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSProjectOutputFile : VSFile
	{
		private VsWiXProject.ReferenceDescriptor _referenceDescriptor;

		internal override bool CanRename
		{
			get
			{
				return false;
			}
		}

		[Browsable(false)]
		[DisplayName("(Name)")]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				if (this._referenceDescriptor == null || this._referenceDescriptor.ReferencedProject == null)
				{
					return string.Concat("Primary output from ", this.TargetName);
				}
				return string.Concat("Primary output from ", this._referenceDescriptor.ReferencedProject.Name);
			}
			set
			{
			}
		}

		internal VsWiXProject.ReferenceDescriptor ReferenceDescriptor
		{
			get
			{
				return this._referenceDescriptor;
			}
		}

		[Browsable(true)]
		[MergableProperty(false)]
		[TypeConverter(typeof(SourcePathPropertyConverter))]
		public override string SourcePath
		{
			get
			{
				return base.WiXElement.Source;
			}
		}

		[Browsable(true)]
		[MergableProperty(false)]
		[ReadOnly(true)]
		[TypeConverter(typeof(TargetNamePropertyConverter))]
		public override string TargetName
		{
			get
			{
				return base.WiXElement.Name;
			}
			set
			{
			}
		}

		public VSProjectOutputFile()
		{
		}

		public VSProjectOutputFile(WiXProjectParser project, VSComponent parent, WiXFile wixElement, VsWiXProject.ReferenceDescriptor refDescriptor) : base(project, parent, wixElement)
		{
			this._referenceDescriptor = refDescriptor;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._referenceDescriptor = null;
			}
			base.Dispose(disposing);
		}

		protected override string GetComponentName()
		{
			return "Primary output";
		}
	}
}