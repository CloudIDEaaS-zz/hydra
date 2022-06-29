using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class ComboTreeNodeCollection : IList<ComboTreeNode>, ICollection<ComboTreeNode>, IEnumerable<ComboTreeNode>, IEnumerable, IList, ICollection, INotifyCollectionChanged
	{
		private List<ComboTreeNode> _innerList;

		private ComboTreeNode _node;

		public int Count
		{
			get
			{
				return this._innerList.Count;
			}
		}

		public ComboTreeNode this[string name]
		{
			get
			{
				ComboTreeNode comboTreeNode;
				using (IEnumerator<ComboTreeNode> enumerator = this.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ComboTreeNode current = enumerator.Current;
						if (!object.Equals(current.Name, name))
						{
							continue;
						}
						comboTreeNode = current;
						return comboTreeNode;
					}
					return null;
				}
				return comboTreeNode;
			}
		}

		public ComboTreeNode this[int index]
		{
			get
			{
				return this._innerList[index];
			}
			set
			{
				ComboTreeNode item = this._innerList[index];
				this._innerList[index] = value;
				value.Parent = this._node;
				value.Nodes.CollectionChanged += this.CollectionChanged;
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, item));
			}
		}

		bool System.Collections.Generic.ICollection<AddinExpress.Installer.WiXDesigner.ComboTreeNode>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool System.Collections.ICollection.IsSynchronized
		{
			get
			{
				return ((ICollection)this._innerList).IsSynchronized;
			}
		}

		object System.Collections.ICollection.SyncRoot
		{
			get
			{
				return ((ICollection)this._innerList).SyncRoot;
			}
		}

		bool System.Collections.IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool System.Collections.IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (ComboTreeNode)value;
			}
		}

		public ComboTreeNodeCollection(ComboTreeNode node)
		{
			this._innerList = new List<ComboTreeNode>();
			this._node = node;
		}

		public ComboTreeNode Add(string text)
		{
			ComboTreeNode comboTreeNode = new ComboTreeNode(text);
			this.Add(comboTreeNode);
			return comboTreeNode;
		}

		public ComboTreeNode Add(string name, string text)
		{
			ComboTreeNode comboTreeNode = new ComboTreeNode(name, text);
			this.Add(comboTreeNode);
			return comboTreeNode;
		}

		public void Add(ComboTreeNode item)
		{
			this._innerList.Add(item);
			item.Parent = this._node;
			item.Nodes.CollectionChanged += this.CollectionChanged;
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
		}

		public void AddRange(IEnumerable<ComboTreeNode> items)
		{
			foreach (ComboTreeNode item in items)
			{
				this._innerList.Add(item);
				item.Parent = this._node;
				item.Nodes.CollectionChanged += this.CollectionChanged;
			}
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public void Clear()
		{
			foreach (ComboTreeNode comboTreeNode in this._innerList)
			{
				comboTreeNode.Nodes.CollectionChanged -= this.CollectionChanged;
			}
			this._innerList.Clear();
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public bool Contains(ComboTreeNode item)
		{
			return this._innerList.Contains(item);
		}

		public bool ContainsKey(string name)
		{
			bool flag;
			using (IEnumerator<ComboTreeNode> enumerator = this.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!object.Equals(enumerator.Current.Name, name))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		public void CopyTo(ComboTreeNode[] array, int arrayIndex)
		{
			this._innerList.CopyTo(array, arrayIndex);
		}

		public IEnumerator<ComboTreeNode> GetEnumerator()
		{
			return this._innerList.GetEnumerator();
		}

		public int IndexOf(string name)
		{
			for (int i = 0; i < this._innerList.Count; i++)
			{
				if (object.Equals(this._innerList[i].Name, name))
				{
					return i;
				}
			}
			return -1;
		}

		public int IndexOf(ComboTreeNode item)
		{
			return this._innerList.IndexOf(item);
		}

		public void Insert(int index, ComboTreeNode item)
		{
			this._innerList.Insert(index, item);
			item.Parent = this._node;
			item.Nodes.CollectionChanged += this.CollectionChanged;
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
		}

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, e);
			}
		}

		public bool Remove(string name)
		{
			for (int i = 0; i < this._innerList.Count; i++)
			{
				if (object.Equals(this._innerList[i].Name, name))
				{
					ComboTreeNode item = this._innerList[i];
					item.Nodes.CollectionChanged -= this.CollectionChanged;
					this._innerList.RemoveAt(i);
					this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
					return true;
				}
			}
			return false;
		}

		public bool Remove(ComboTreeNode item)
		{
			if (!this._innerList.Remove(item))
			{
				return false;
			}
			item.Nodes.CollectionChanged -= this.CollectionChanged;
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
			return true;
		}

		public void RemoveAt(int index)
		{
			ComboTreeNode item = this._innerList[index];
			item.Nodes.CollectionChanged -= this.CollectionChanged;
			this._innerList.RemoveAt(index);
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
		}

		internal void Sort(IComparer<ComboTreeNode> comparer)
		{
			if (comparer == null)
			{
				comparer = Comparer<ComboTreeNode>.Default;
			}
			this.SortInternal(comparer);
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		private void SortInternal(IComparer<ComboTreeNode> comparer)
		{
			this._innerList.Sort(comparer);
			foreach (ComboTreeNode comboTreeNode in this._innerList)
			{
				comboTreeNode.Nodes.Sort(comparer);
			}
		}

		void System.Collections.ICollection.CopyTo(Array array, int index)
		{
			((ICollection)this._innerList).CopyTo(array, index);
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this._innerList.GetEnumerator();
		}

		int System.Collections.IList.Add(object value)
		{
			this.Add((ComboTreeNode)value);
			return this.Count - 1;
		}

		bool System.Collections.IList.Contains(object value)
		{
			return this.Contains((ComboTreeNode)value);
		}

		int System.Collections.IList.IndexOf(object value)
		{
			return this.IndexOf((ComboTreeNode)value);
		}

		void System.Collections.IList.Insert(int index, object value)
		{
			this.Insert(index, (ComboTreeNode)value);
		}

		void System.Collections.IList.Remove(object value)
		{
			this.Remove((ComboTreeNode)value);
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}