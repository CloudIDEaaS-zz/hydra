using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Utils
{
    public static class KeyValuePairFactory
    {
        [DebuggerStepThrough]
        public static KeyValuePair<K,V> Pair<K, V>(K key, V value)
        {
            return new KeyValuePair<K, V>(key, value);
        }

        public static T Get<T>(this KeyValuePair<string, object>[] pairs, string key)
        {
            return (T) pairs.Single(p => p.Key == key).Value;
        }
    }
}
