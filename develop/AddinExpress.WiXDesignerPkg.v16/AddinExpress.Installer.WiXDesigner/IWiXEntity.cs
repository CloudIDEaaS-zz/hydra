using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal interface IWiXEntity
	{
		WiXEntityList ChildEntities
		{
			get;
		}

		IWiXEntity FirstChild
		{
			get;
		}

		bool HasChildEntities
		{
			get;
		}

		bool IsDirty
		{
			get;
			set;
		}

		bool IsSupported
		{
			get;
		}

		IWiXEntity LastChild
		{
			get;
		}

		string Name
		{
			get;
		}

		IWiXEntity NextSibling
		{
			get;
		}

		IWiXEntity Owner
		{
			get;
		}

		IWiXEntity Parent
		{
			get;
		}

		IWiXEntity PreviousSibling
		{
			get;
		}

		object SupportedObject
		{
			get;
		}

		void SetDirty();
	}
}