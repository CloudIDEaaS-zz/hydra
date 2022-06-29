using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class HierarchyEventArgs : EventArgs
	{
		private uint itemId;

		private int propId;

		private uint itemidParent;

		private uint itemidSiblingPrev;

		private uint flags;

		public uint Flags
		{
			get
			{
				return this.flags;
			}
		}

		public uint ItemID
		{
			get
			{
				return this.itemId;
			}
		}

		public uint ItemIDParent
		{
			get
			{
				return this.itemidParent;
			}
		}

		public uint ItemIDSiblingPrev
		{
			get
			{
				return this.itemidSiblingPrev;
			}
		}

		public int PropertyID
		{
			get
			{
				return this.propId;
			}
		}

		public HierarchyEventArgs(uint itemId, uint itemidParent, uint itemidSiblingPrev, int propId, uint flags)
		{
			this.itemId = itemId;
			this.itemidParent = itemidParent;
			this.itemidSiblingPrev = itemidSiblingPrev;
			this.propId = propId;
			this.flags = flags;
		}
	}
}