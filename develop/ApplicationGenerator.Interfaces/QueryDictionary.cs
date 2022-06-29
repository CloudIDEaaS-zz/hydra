using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using Utils;

namespace AbstraX
{
    public class QueryDictionary : BaseDictionary<IBase, List<QueryInfo>>
    {
        private Dictionary<IBase, List<QueryInfo>> internalDictionary;

        public override int Count => internalDictionary.Count;

        public QueryDictionary()
        {
            internalDictionary = new Dictionary<IBase, List<QueryInfo>>();
        }

        public override void Add(IBase key, List<QueryInfo> value)
        {
            internalDictionary.Add(key, value);
        }

        public override void Clear()
        {
            internalDictionary.Clear();
        }

        public override bool ContainsKey(IBase key)
        {
            if (internalDictionary.ContainsKey(key))
            {
                return true;
            }

            return false;
        }

        public bool ContainsNavigationKey(IBase key)
        {
            if (internalDictionary.Keys.OfType<NavigationProperty>().Any(n => n.DataType.IsCollectionType && n.ChildElements.Single().Name == key.Name))
            {
                var trueKey = internalDictionary.Keys.OfType<NavigationProperty>().Single(n => n.DataType.IsCollectionType && n.ChildElements.Single().Name == key.Name);

                return true;
            }

            return false;
        }


        public List<QueryInfo> GetNavigationValue(IBase key)
        {
            if (internalDictionary.Keys.OfType<NavigationProperty>().Any(n => n.DataType.IsCollectionType && n.ChildElements.Single().Name == key.Name))
            {
                var trueKey = internalDictionary.Keys.OfType<NavigationProperty>().Single(n => n.DataType.IsCollectionType && n.ChildElements.Single().Name == key.Name);

                return this[trueKey];
            }

            return null;
        }

        public override IEnumerator<KeyValuePair<IBase, List<QueryInfo>>> GetEnumerator()
        {
            return internalDictionary.GetEnumerator();
        }

        public override bool Remove(IBase key)
        {
            return internalDictionary.Remove(key);
        }

        public override bool TryGetValue(IBase key, out List<QueryInfo> value)
        {
            return internalDictionary.TryGetValue(key, out value);
        }

        protected override void SetValue(IBase key, List<QueryInfo> value)
        {
            internalDictionary[key] = value;
        }
    }
}
