// file:	ObjectPropertiesDictionary.cs
//
// summary:	Implements the object properties dictionary class

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   Dictionary of object properties. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>

    public class ObjectPropertiesDictionary : BaseDictionary<string, dynamic>
    {
        /// <summary>   Dictionary of internals. </summary>
        private Dictionary<string, dynamic> internalDictionary;
        /// <summary>   Event queue for all listeners interested in OnChanged events. </summary>
        public event EventHandler OnChanged;

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>

        public ObjectPropertiesDictionary()
        {
            internalDictionary = new Dictionary<string, dynamic>();
        }

        /// <summary>   Gets the number of.  </summary>
        ///
        /// <value> The count. </value>

        public override int Count => internalDictionary.Count;

        /// <summary>
        /// Adds an element with the provided key and value to the
        /// <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <param name="key">      The object to use as the key of the element to add. </param>
        /// <param name="value">    The object to use as the value of the element to add. </param>

        public override void Add(string key, dynamic value)
        {
            using (var notifier = this.Notify("this[]", "Count"))
            {
                internalDictionary.Add(key, value);
            }

            if (OnChanged != null)
            {
                OnChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>

        public override void Clear()
        {
            using (var notifier = this.Notify("this[]", "Count"))
            {
                internalDictionary.Clear();
            }

            if (OnChanged != null)
            {
                OnChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an
        /// element with the specified key.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <param name="key">  The key to locate in the
        ///                     <see cref="T:System.Collections.Generic.IDictionary`2" />. </param>
        ///
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element
        /// with the key; otherwise, false.
        /// </returns>

        public override bool ContainsKey(string key)
        {
            return internalDictionary.ContainsKey(key);
        }

        /// <summary>   Returns an enumerator that iterates through the collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <returns>   An enumerator that can be used to iterate through the collection. </returns>

        public override IEnumerator<KeyValuePair<string, dynamic>> GetEnumerator()
        {
            return internalDictionary.GetEnumerator();
        }

        /// <summary>
        /// Removes the element with the specified key from the
        /// <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <param name="key">  The key of the element to remove. </param>
        ///
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns
        /// false if <paramref name="key" /> was not found in the original
        /// <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>

        public override bool Remove(string key)
        {
            bool removed;

            using (var notifier = this.Notify("this[]", "Count"))
            {
                removed =  internalDictionary.Remove(key);
            }

            if (OnChanged != null)
            {
                OnChanged(this, EventArgs.Empty);
            }

            return removed;
        }

        /// <summary>   Gets the value associated with the specified key. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <param name="key">      The key whose value to get. </param>
        /// <param name="value">    [out] When this method returns, the value associated with the
        ///                         specified key, if the key is found; otherwise, the default value for the
        ///                         type of the <paramref name="value" /> parameter. This parameter is passed
        ///                         uninitialized. </param>
        ///
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />
        /// contains an element with the specified key; otherwise, false.
        /// </returns>

        public override bool TryGetValue(string key, out dynamic value)
        {
            return internalDictionary.TryGetValue(key, out value);
        }

        /// <summary>   Sets a value. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <param name="key">      The key. </param>
        /// <param name="value">    The value. </param>

        protected override void SetValue(string key, dynamic value)
        {
            using (var notifier = this.Notify("this[]", "Count"))
            {
                internalDictionary[key] = value;
            }

            if (OnChanged != null)
            {
                OnChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>   Adds a range. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <param name="dictionary">                   An object of items to append to this. </param>
        /// <param name="recurseToObjectProperties">    (Optional) True to process recursively, false to
        ///                                             process locally only. </param>

        public void AddRange(Dictionary<string, dynamic> dictionary, bool recurseToObjectProperties = false)
        {
            foreach (var pair in dictionary)
            {
                if (recurseToObjectProperties && pair.Value is JObject jObject)
                {
                    var subDictionary = new ObjectPropertiesDictionary();

                    subDictionary.AddRange(jObject.ToObject<Dictionary<string, dynamic>>(), recurseToObjectProperties);

                    this.Add(pair.Key, subDictionary);
                }
                else
                {
                    this.Add(pair.Key, pair.Value);
                }
            }
        }
    }
}
