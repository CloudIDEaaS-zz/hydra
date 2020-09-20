using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Utils
{
    public class BaseList<T> : IList<T>, IDisposable, INotifyPropertyChanged, INotifyPropertyChanging, INotifyCollectionChanged 
    {
        protected List<T> internalList;
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public BaseList(List<T> internalList)
        {
            this.internalList = internalList;
        }

        public BaseList()
        {
            this.internalList = new List<T>();
        }

        public virtual void Dispose()
        {
            Clear();
        }

        public virtual int IndexOf(T item)
        {
            return internalList.IndexOf(item);
        }

        protected PropertyNotifier Notify(NotifyCollectionChangedAction action, T item = default(T), int index = -1)
        {
            PropertyNotifier notifier = null;

            switch (action)
            {
                case NotifyCollectionChangedAction.Add:

                    if (index == -1)
                    {
                        notifier = this.Notify(PropertyChanging, PropertyChanged, CollectionChanged, NotifyCollectionChangedAction.Add, item, "this[]", "Count");
                    }
                    else
                    {
                        notifier = this.Notify(PropertyChanging, PropertyChanged, CollectionChanged, NotifyCollectionChangedAction.Add, item, index, "this[]", "Count");
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    notifier = this.Notify(PropertyChanging, PropertyChanged, CollectionChanged, NotifyCollectionChangedAction.Remove, item, index, "this[]", "Count");
                    break;
                case NotifyCollectionChangedAction.Replace:
                    notifier = this.Notify(PropertyChanging, PropertyChanged, CollectionChanged, NotifyCollectionChangedAction.Replace, item, index, "this[]", "Count");
                    break;
                case NotifyCollectionChangedAction.Reset:
                    notifier = this.Notify(PropertyChanging, PropertyChanged, CollectionChanged, NotifyCollectionChangedAction.Reset, "this[]", "Count");
                    break;
                default:
                    Debugger.Break();
                    break;
            }

            return notifier;
        }

        protected void PostNotify(PropertyNotifier notifier, NotifyCollectionChangedAction action, T item, int index)
        {
            notifier.Dispose();
        }

        public virtual void Insert(int index, T item)
        {
            using (var notifier = this.Notify(PropertyChanging, PropertyChanged, CollectionChanged, NotifyCollectionChangedAction.Add, item, index, "this[]", "Count"))
            {
                internalList.Insert(index, item);
            }
        }

        public virtual void RemoveAt(int index)
        {
            object item = internalList[index];

            using (var notifier = this.Notify(PropertyChanging, PropertyChanged, CollectionChanged, NotifyCollectionChangedAction.Remove, item, index, "this[]", "Count"))
            {
                internalList.RemoveAt(index);
            }
        }

        public virtual T this[int index]
        {
            get
            {
                return internalList[index];
            }

            set
            {
                object item = internalList[index];

                using (var notifier = this.Notify(PropertyChanging, PropertyChanged, CollectionChanged, NotifyCollectionChangedAction.Replace, item, value, index, "this[]", "Count"))
                {
                    internalList[index] = value;
                }
            }
        }

        public virtual void Add(T item)
        {
            using (var notifier = this.Notify(PropertyChanging, PropertyChanged, CollectionChanged, NotifyCollectionChangedAction.Add, item, "this[]", "Count"))
            {
                internalList.Add(item);
            }
        }

        public virtual void AddRange(IEnumerable<T> items)
        {
            using (var notifier = this.Notify(PropertyChanging, PropertyChanged, CollectionChanged, NotifyCollectionChangedAction.Add, items.ToList(), "this[]", "Count"))
            {
                internalList.AddRange(items);
            }
        }

        public virtual void Clear()
        {
            using (var notifier = this.Notify(PropertyChanging, PropertyChanged, CollectionChanged, NotifyCollectionChangedAction.Reset, "this[]", "Count"))
            {
                internalList.Clear();
            }
        }

        public virtual bool Contains(T item)
        {
            return internalList.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            internalList.CopyTo(array, arrayIndex);
        }

        public virtual int Count
        {
            get
            {
                return internalList.Count;
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public virtual bool Remove(T item)
        {
            using (var notifier = this.Notify(PropertyChanging, PropertyChanged, CollectionChanged, NotifyCollectionChangedAction.Remove, item, "this[]", "Count"))
            {
                return internalList.Remove(item);
            }
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalList.GetEnumerator();
        }
    }

    public static class Subscriptions
    {
        public class SubscriptionTarget<T> : INotifyPropertyChanged, INotifyPropertyChanging, INotifyCollectionChanged 
        {
            private BaseList<T> list;

            public event PropertyChangedEventHandler PropertyChanged
            {
                add
                {
                    list.PropertyChanged += value;
                }

                remove
                {
                    list.PropertyChanged -= value;
                }
            }

            public event PropertyChangingEventHandler PropertyChanging
            {
                add
                {
                    list.PropertyChanging += value;
                }

                remove
                {
                    list.PropertyChanging -= value;
                }
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged
            {
                add
                {
                    list.CollectionChanged += value;
                }

                remove
                {
                    list.CollectionChanged -= value;
                }
            }

            public SubscriptionTarget(BaseList<T> list)
            {
                this.list = list;
            }
        }

        public static SubscriptionTarget<T> Subscribe<T>(this BaseList<T> list)
        {
            return new SubscriptionTarget<T>(list);
        }
    }
}
