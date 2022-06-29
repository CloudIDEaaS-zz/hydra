using System;
using System.Collections.Generic;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSFeatureGroup : VSComponentBase
	{
		private WiXFeatureGroup _wixElement;

		private WiXProjectParser _project;

		private List<WiXFeatureGroupRef> _featureGroupRefs;

		internal List<WiXFeatureGroupRef> FeatureGroupRefs
		{
			get
			{
				return this._featureGroupRefs;
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

		internal WiXFeatureGroup WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		public VSFeatureGroup()
		{
			this._featureGroupRefs = new List<WiXFeatureGroupRef>();
		}

		public VSFeatureGroup(WiXProjectParser project, WiXFeatureGroup wixElement) : this()
		{
			this._project = project;
			this._wixElement = wixElement;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._featureGroupRefs.Clear();
				this._featureGroupRefs = null;
				this._wixElement = null;
			}
			base.Dispose(disposing);
		}
	}
}