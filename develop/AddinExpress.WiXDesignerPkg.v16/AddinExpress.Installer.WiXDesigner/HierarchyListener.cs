using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class HierarchyListener : IVsHierarchyEvents, IDisposable
	{
		private bool disposed;

		private IVsHierarchy hierarchy;

		private uint cookie;

		private EventHandler<HierarchyEventArgs> onItemAdded;

		private EventHandler<HierarchyEventArgs> onItemDeleted;

		private EventHandler<HierarchyEventArgs> onItemChanged;

		public bool IsListening
		{
			get
			{
				return this.cookie != 0;
			}
		}

		public HierarchyListener(IVsHierarchy hierarchy)
		{
			if (hierarchy == null)
			{
				throw new ArgumentNullException("hierarchy");
			}
			this.hierarchy = hierarchy;
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.disposed = true;
				this.InternalStopListening(false);
				this.cookie = 0;
				this.hierarchy = null;
			}
			GC.SuppressFinalize(this);
		}

		private static uint GetItemId(object variantValue)
		{
			if (variantValue == null)
			{
				return (uint)-1;
			}
			if (variantValue is int)
			{
				return (uint)variantValue;
			}
			if (variantValue is uint)
			{
				return (uint)variantValue;
			}
			if (variantValue is short)
			{
				return (uint)variantValue;
			}
			if (variantValue is ushort)
			{
				return (ushort)variantValue;
			}
			if (!(variantValue is long))
			{
				return (uint)-1;
			}
			return (uint)((long)variantValue);
		}

		private void InternalScanHierarchy(uint itemId)
		{
		}

		private bool InternalStopListening(bool throwOnError)
		{
			if (this.hierarchy == null || this.cookie == 0)
			{
				return false;
			}
			int num = 0;
			try
			{
				num = this.hierarchy.UnadviseHierarchyEvents(this.cookie);
			}
			catch (Exception exception)
			{
			}
			if (throwOnError)
			{
				ErrorHandler.ThrowOnFailure(num);
			}
			this.cookie = 0;
			return ErrorHandler.Succeeded(num);
		}

		public int OnInvalidateIcon(IntPtr hicon)
		{
			return 0;
		}

		public int OnInvalidateItems(uint itemidParent)
		{
			return 0;
		}

		public int OnItemAdded(uint itemidParent, uint itemidSiblingPrev, uint itemidAdded)
		{
			if (this.onItemAdded != null)
			{
				HierarchyEventArgs hierarchyEventArg = new HierarchyEventArgs(itemidAdded, itemidParent, itemidSiblingPrev, 0, 0);
				this.onItemAdded(this.hierarchy, hierarchyEventArg);
			}
			return 0;
		}

		public int OnItemDeleted(uint itemid)
		{
			if (this.onItemDeleted != null)
			{
				HierarchyEventArgs hierarchyEventArg = new HierarchyEventArgs(itemid, 0, 0, 0, 0);
				this.onItemDeleted(this.hierarchy, hierarchyEventArg);
			}
			return 0;
		}

		public int OnItemsAppended(uint itemidParent)
		{
			return 0;
		}

		public int OnPropertyChanged(uint itemid, int propid, uint flags)
		{
			if (this.onItemChanged != null)
			{
				HierarchyEventArgs hierarchyEventArg = new HierarchyEventArgs(itemid, 0, 0, propid, flags);
				this.onItemChanged(this.hierarchy, hierarchyEventArg);
			}
			return 0;
		}

		public void StartListening(bool doInitialScan)
		{
			if (this.cookie != 0)
			{
				return;
			}
			ErrorHandler.ThrowOnFailure(this.hierarchy.AdviseHierarchyEvents(this, out this.cookie));
			if (doInitialScan)
			{
				this.InternalScanHierarchy(-2);
			}
		}

		public void StopListening()
		{
			this.InternalStopListening(true);
		}

		public event EventHandler<HierarchyEventArgs> OnAddItem
		{
			add
			{
				this.onItemAdded += value;
			}
			remove
			{
				this.onItemAdded -= value;
			}
		}

		public event EventHandler<HierarchyEventArgs> OnChangedItem
		{
			add
			{
				this.onItemChanged += value;
			}
			remove
			{
				this.onItemChanged -= value;
			}
		}

		public event EventHandler<HierarchyEventArgs> OnDeleteItem
		{
			add
			{
				this.onItemDeleted += value;
			}
			remove
			{
				this.onItemDeleted -= value;
			}
		}
	}
}