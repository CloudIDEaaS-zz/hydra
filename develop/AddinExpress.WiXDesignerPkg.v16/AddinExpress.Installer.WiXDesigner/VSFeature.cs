using System;
using System.Collections.Generic;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSFeature : VSComponentBase
	{
		private WiXFeature _wixElement;

		private WiXProjectParser _project;

		private List<WiXFeatureRef> _featureRefs;

		internal List<WiXFeatureRef> FeatureRefs
		{
			get
			{
				return this._featureRefs;
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

		internal WiXFeature WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		public VSFeature()
		{
			this._featureRefs = new List<WiXFeatureRef>();
		}

		public VSFeature(WiXProjectParser project, WiXFeature wixElement) : this()
		{
			this._project = project;
			this._wixElement = wixElement;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._featureRefs.Clear();
				this._featureRefs = null;
				this._wixElement = null;
			}
			base.Dispose(disposing);
		}
	}
}