﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

namespace Utils
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(PREFIX + "DictionaryDebugView`2" + SUFFIX)]
    public abstract class BaseDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyPropertyChanged, INotifyPropertyChanging
    {
        private const string PREFIX = "System.Collections.Generic.Mscorlib_";
        private const string SUFFIX = ",mscorlib,Version=2.0.0.0,Culture=neutral,PublicKeyToken=b77a5c561934e089";

        private KeyCollection keys;
        private ValueCollection values;

        protected BaseDictionary() { }

        public abstract int Count { get; }
        public abstract void Clear();
        public abstract void Add(TKey key, TValue value);
        public abstract bool ContainsKey(TKey key);
        public abstract bool Remove(TKey key);
        public abstract bool TryGetValue(TKey key, out TValue value);
        public abstract IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();
        protected abstract void SetValue(TKey key, TValue value);
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        public bool IsReadOnly
        {
            get 
            {
                return false; 
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                if (this.keys == null)
                {
                    this.keys = new KeyCollection(this);
                }

                return this.keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                if (this.values == null)
                {
                    this.values = new ValueCollection(this);
                }

                return this.values;
            }
        }

        protected PropertyNotifier Notify(params string[] properties)
        {
            return this.Notify(PropertyChanging, PropertyChanged, properties);
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;

                if (!this.TryGetValue(key, out value))
                {
                    throw new KeyNotFoundException();
                }

                return value;
            }
            set
            {
                using (var notifier = this.Notify(PropertyChanging, PropertyChanged, "this[]", "Count"))
                {
                    SetValue(key, value);
                }
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            using (var notifier = this.Notify(PropertyChanging, PropertyChanged, "this[]", "Count"))
            {
                this.Add(item.Key, item.Value);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;

            if (!this.TryGetValue(item.Key, out value))
            {
                return false;
            }

            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Copy(this, array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!this.Contains(item))
            {
                return false;
            }

            using (var notifier = this.Notify(PropertyChanging, PropertyChanged, "this[]", "Count"))
            {
                return this.Remove(item.Key);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private abstract class Collection<T> : ICollection<T>
        {
            protected readonly IDictionary<TKey, TValue> dictionary;

            protected Collection(IDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
            }

            public int Count
            {
                get { return this.dictionary.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                Copy(this, array, arrayIndex);
            }

            public virtual bool Contains(T item)
            {
                foreach (T element in this)
                {
                    if (EqualityComparer<T>.Default.Equals(element, item))
                    {
                        return true;
                    }
                }

                return false;
            }

            public IEnumerator<T> GetEnumerator()
            {
                foreach (KeyValuePair<TKey, TValue> pair in this.dictionary)
                {
                    yield return GetItem(pair);
                }
            }

            protected abstract T GetItem(KeyValuePair<TKey, TValue> pair);

            public bool Remove(T item)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public void Add(T item)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public void Clear()
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        [DebuggerDisplay("Count = {Count}")]
        [DebuggerTypeProxy(PREFIX + "DictionaryKeyCollectionDebugView`2" + SUFFIX)]
        private class KeyCollection : Collection<TKey>
        {
            public KeyCollection(IDictionary<TKey, TValue> dictionary)
                : base(dictionary) { }

            protected override TKey GetItem(KeyValuePair<TKey, TValue> pair)
            {
                return pair.Key;
            }
            public override bool Contains(TKey item)
            {
                return this.dictionary.ContainsKey(item);
            }
        }

        [DebuggerDisplay("Count = {Count}")]
        [DebuggerTypeProxy(PREFIX + "DictionaryValueCollectionDebugView`2" + SUFFIX)]
        private class ValueCollection : Collection<TValue>
        {
            public ValueCollection(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }

            protected override TValue GetItem(KeyValuePair<TKey, TValue> pair)
            {
                return pair.Value;
            }
        }

        private static void Copy<T>(ICollection<T> source, T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            if ((array.Length - arrayIndex) < source.Count)
            {
                throw new ArgumentException("Destination array is not large enough. Check array.Length and arrayIndex.");
            }

            foreach (T item in source)
            {
                array[arrayIndex++] = item;
            }
        }
    }
}
