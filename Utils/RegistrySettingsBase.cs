using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Utils;
using System.Diagnostics;
using System.Collections;

namespace Utils
{
    public abstract class RegistrySettingsBase
    {
        private string rootKeyPath;

        public RegistrySettingsBase(string rootKey)
        {
            this.rootKeyPath = rootKey;
        }

        public virtual void Initialize()
        {
            var rootKey = Registry.CurrentUser.CreateSubKey(rootKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);

            DoLoad(rootKey, this);
        }

        private void DoLoad(RegistryKey key, object obj)
        {
            // public strings

            foreach (var propertyValue in obj.GetPublicPropertyValuesOfType<string>())
            {
                if (obj is IRegistryKey)
                {
                    if (propertyValue.Key != "KeyName")
                    {
                        TryGetSet(key, obj, propertyValue.Key, RegistryValueKind.String);
                    }
                }
                else
                {
                    TryGetSet(key, obj, propertyValue.Key, RegistryValueKind.String);
                }
            }

            // non strings

            foreach (var attributeProperty in obj.GetPublicPropertiesWithAttributeType<RegistryNonStringValueAttribute>())
            {
                var attribute = attributeProperty.Key;
                var property = attributeProperty.Value;
                var valueKind = attribute.ValueKind;

                TryGetSet(key, obj, property.Name, valueKind);
            }

            // enumerables

            foreach (var attributeProperty in obj.GetPublicPropertiesWithAttributeType<RegistryKeyEnumerableAttribute>())
            {
                var attribute = attributeProperty.Key;
                var property = attributeProperty.Value;
                var factoryType = attribute.FactoryType;
                var value = property.GetValue(obj, null);
                var subKey = key.OpenSubKey(attribute.KeyName);

                if (subKey != null)
                {
                    var enumerableKey = key.OpenSubKey(attribute.KeyName).ToIndexable();
                    var list = (IList)value;
                    var x = 1;

                    foreach (var subSubKey in enumerableKey.SubKeys)
                    {
                        var listItem = factoryType.CreateInstance<object>();

                        list.Add(listItem);
                        DoLoad(subSubKey.Key, listItem);

                        x++;
                    }
                }
            }

            // public properties of type IRegistryKey

            foreach (var property in obj.GetPublicPropertiesOfType<IRegistryKey>())
            {
                var propertyType = property.PropertyType;
                var propertyItem = propertyType.CreateInstance<IRegistryKey>();
                var subKey = key.OpenSubKey(propertyItem.KeyName);

                property.SetValue(obj, propertyItem, null);

                DoLoad(subKey, propertyItem);
            }
        }

        public virtual void Save()
        {
            var rootKey = Registry.CurrentUser.CreateSubKey(rootKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);

            DoSave(rootKey, this);
        }

        private void DoSave(RegistryKey key, object obj)
        {
            // public strings

            foreach (var propertyValue in obj.GetPublicPropertyValuesOfType<string>())
            {
                if (!propertyValue.Value.IsNullWhiteSpaceOrEmpty())
                {
                    if (obj is IRegistryKey)
                    {
                        if (propertyValue.Key != "KeyName")
                        {
                            key.SetValue(propertyValue.Key, propertyValue.Value);
                        }
                    }
                    else
                    {
                        key.SetValue(propertyValue.Key, propertyValue.Value);
                    }
                }
            }

            // non strings

            foreach (var attributeProperty in obj.GetPublicPropertiesWithAttributeType<RegistryNonStringValueAttribute>())
            {
                var attribute = attributeProperty.Key;
                var property = attributeProperty.Value;
                var value = property.GetValue(obj, null);
                var valueKind = attribute.ValueKind;

                switch (valueKind)
                {
                    case RegistryValueKind.String:

                        if (value != null)
                        {
                            key.SetValue(property.Name, value.ToString(), valueKind);
                        }

                        break;

                    case RegistryValueKind.DWord:

                        if (value != null)
                        {
                            switch (property.PropertyType.Name)
                            {
                                case "Boolean":
                                    key.SetValue(property.Name, (bool)value ? 1 : 0, valueKind);
                                    break;
                                default:
                                    key.SetValue(property.Name, (int)value, valueKind);
                                    break;
                            }
                        }

                        break;

                    default:
                        DebugUtils.Break();
                        break;
                }
            }

            // enumerables

            foreach (var attributeProperty in obj.GetPublicPropertiesWithAttributeType<RegistryKeyEnumerableAttribute>())
            {
                var attribute = attributeProperty.Key;
                var property = attributeProperty.Value;
                var value = property.GetValue(obj, null);
                var subKey = key.CreateSubKey(attribute.KeyName);

                if (value is IRegistryKeyWithSubKeys)
                {
                    DebugUtils.Break();
                }
                else
                {
                    var enumerable = (IEnumerable)value;
                    var x = 1;

                    foreach (var subValue in enumerable)
                    {
                        if (subValue is IRegistryKey)
                        {
                            var registryKeyObject = (IRegistryKey)subValue;

                            if (registryKeyObject is IRegistryKeyWithSubKeys)
                            {
                                DebugUtils.Break();
                            }
                            else
                            {
                                subKey = subKey.CreateSubKey(string.Format(registryKeyObject.KeyName, x));
                            }
                        }

                        DoSave(subKey, subValue);

                        x++;
                    }
                }
            }

            // public properties of type IRegistryKey

            foreach (var propertyValue in obj.GetPublicPropertyValuesOfType<IRegistryKey>())
            {
                var value = propertyValue.Value;

                if (value is IRegistryKey)
                {
                    var registryKeyObject = (IRegistryKey)value;

                    if (registryKeyObject is IRegistryKeyWithSubKeys)
                    {
                        DebugUtils.Break();
                    }
                    else
                    {
                        var subKey = key.CreateSubKey(registryKeyObject.KeyName);

                        DoSave(subKey, registryKeyObject);
                    }
                }
            }
        }

        private void TryGetSet(RegistryKey key, object obj, string keyName, RegistryValueKind valueKind)
        {
            object value = null;

            try
            {
                value = key.GetValue(keyName);
            }
            catch
            {
            }

            if (value != null)
            {
                try
                {
                    var property = obj.GetType().GetProperty(keyName);

                    if (property.PropertyType.IsEnum)
                    {
                        var enumValue = Enum.Parse(property.PropertyType, value.ToString());
                        obj.SetPropertyValue(keyName, enumValue);
                    }
                    else
                    {
                        switch (property.PropertyType.Name)
                        {
                            case "DateTime":

                                switch (valueKind)
                                {
                                    case RegistryValueKind.String:

                                        var date = DateTime.Parse(value.ToString());
                                        obj.SetPropertyValue(keyName, date);
                                        break;

                                    default:
                                        DebugUtils.Break();
                                        break;
                                }

                                break;

                            case "String":

                                switch (valueKind)
                                {
                                    case RegistryValueKind.String:
                                        obj.SetPropertyValue(keyName, value.ToString());
                                        break;
                                    default:
                                        DebugUtils.Break();
                                        break;
                                }

                                break;

                            case "Int32":
                                {
                                    obj.SetPropertyValue(keyName, (int)value);
                                    break;
                                }


                            case "Boolean":
                                {
                                    obj.SetPropertyValue(keyName, (int)value == 0 ? false : true);
                                    break;
                                }

                            default:
                                DebugUtils.Break();
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    DebugUtils.Break();
                }
            }
        }
    }
}
