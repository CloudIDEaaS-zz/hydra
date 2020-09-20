using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace Utils
{
    public class SubKeyEnumerable : IEnumerable<RegistryKeyIndexable>
    {
        private RegistryKeyIndexable parent;

        public SubKeyEnumerable(RegistryKeyIndexable parent)
        {
            this.parent = parent;
        }

        public IEnumerator<RegistryKeyIndexable> GetEnumerator()
        {
            foreach (var name in parent.Key.GetSubKeyNames())
            {
                var subKey = parent.Key.OpenSubKey(name, RegistryKeyPermissionCheck.ReadWriteSubTree);

                yield return new RegistryKeyIndexable(subKey);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class ValueEnumerable : IEnumerable<KeyValuePair<string, object>>
    {
        private RegistryKeyIndexable parent;

        public ValueEnumerable(RegistryKeyIndexable parent)
        {
            this.parent = parent;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var name in parent.Key.GetValueNames())
            {
                var value = parent.Key.GetValue(name);

                yield return new KeyValuePair<string, object>(name, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    [DebuggerDisplay(" { Name } ")]
    public class RegistryKeyIndexable
    {
        public RegistryKey Key { get; private set; }

        public RegistryKeyIndexable(RegistryKey key)
        {
            this.Key = key;
        }

        public string Name
        {
            get
            {
                return Key.Name;
            }
        }

        public string SubName
        {
            get
            {
                return Path.GetFileName(Key.Name);
            }
        }

        public IEnumerable<RegistryKeyIndexable> SubKeys
        {
            get
            {
                return new SubKeyEnumerable(this);
            }
        }

        public IEnumerable<KeyValuePair<string, object>> Values
        {
            get
            {
                return new ValueEnumerable(this);
            }
        }

        public object Default
        {
            get
            {
                return Key.GetValue("");
            }
        }

        public void DeleteSubKeys()
        {
            foreach (var subKey in this.SubKeys)
            {
                subKey.DeleteSubKeysAndValues();
                this.Key.DeleteSubKey(subKey.SubName);
            }
        }

        public void DeleteValues()
        {
            foreach (var pair in this.Values)
            {
                this.Key.DeleteValue(pair.Key);
            }
        }

        public void DeleteSubKeysAndValues()
        {
            this.DeleteSubKeys();
            this.DeleteValues();
        }

        public object this[string valueName]
        {
            get
            {
                if (valueName.IndexOf(@"\") == -1)
                {
                    return Key.GetValue(valueName);
                }
                else
                {
                    var subPaths = valueName.Split('\\');
                    RegistryKey subKey = Key;

                    foreach (var name in subPaths)
                    {
                        if (name.StartsWith("@"))
                        {
                            return subKey.GetValue(name.RemoveStart(1));
                        }
                        else
                        {
                            subKey = subKey.OpenSubKey(name);

                            if (subKey == null)
                            {
                                return null;
                            }
                        }
                    }

                    return null;
                }
            }
        }
    }

    public static class RegistryKeyExtensions
    {
        public static void DeleteSubKeys(this RegistryKey key)
        {
            foreach (var subKey in key.ToIndexable().SubKeys)
            {
                key.DeleteSubKey(subKey.Name, false);
            }
        }

        public class KeyEnumerator : IEnumerable<RegistryKeyIndexable>
        {
            private RegistryKey key;

            public KeyEnumerator(RegistryKey key)
            {
                this.key = key;
            }

            public IEnumerator<RegistryKeyIndexable> GetEnumerator()
            {
                foreach (string keyName in key.GetSubKeyNames())
                {
                    var subKey = key.OpenSubKey(keyName);

                    yield return subKey.ToIndexable();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static RegistryKeyIndexable ToIndexable(this RegistryKey key)
        {
            return new RegistryKeyIndexable(key);
        }

        public static IEnumerable<RegistryKeyIndexable> Enumerate(this RegistryKey key)
        {
            return new KeyEnumerator(key);
        }
    }
}
