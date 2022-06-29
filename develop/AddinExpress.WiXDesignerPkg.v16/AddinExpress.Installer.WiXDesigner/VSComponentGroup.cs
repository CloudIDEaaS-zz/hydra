using System;
using System.Collections.Generic;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSComponentGroup : VSComponentBase
	{
		private WiXComponentGroup _wixElement;

		private VSBaseFolder _parent;

		private List<WiXComponentGroupRef> _componentGroupRefs;

		internal List<WiXComponentGroupRef> ComponentGroupRefs
		{
			get
			{
				return this._componentGroupRefs;
			}
		}

		internal string Id
		{
			get
			{
				if (this._wixElement == null)
				{
					return string.Empty;
				}
				return this._wixElement.Id;
			}
			set
			{
				if (this._wixElement != null)
				{
					this._wixElement.Id = value;
				}
			}
		}

		internal WiXComponentGroup WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		public VSComponentGroup()
		{
			this._componentGroupRefs = new List<WiXComponentGroupRef>();
		}

		public VSComponentGroup(VSBaseFolder parent, WiXComponentGroup wixElement) : this()
		{
			this._wixElement = wixElement;
			this._parent = parent;
		}

		public override void Delete()
		{
			this._wixElement.Delete();
			foreach (WiXComponentGroupRef _componentGroupRef in this._componentGroupRefs)
			{
				_componentGroupRef.Delete();
			}
			this._componentGroupRefs.Clear();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._componentGroupRefs.Clear();
				this._componentGroupRefs = null;
				this._parent = null;
				this._wixElement = null;
			}
			base.Dispose(disposing);
		}
	}
}