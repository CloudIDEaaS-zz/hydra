using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Management.Automation;

namespace Utils
{
    public static class Enum<T> where T : Enum
    {
        public static string GetName(T value)
        {
            return Enum.GetName(typeof(T), value);
        }

        public static T Parse(string value)
        {
#if SILVERLIGHT
		        return (T)Enum.Parse(typeof(T), value, false);
#else
            return (T)Enum.Parse(typeof(T), value);
#endif
        }

        public static bool TryParse(string value, out T parsedValue)
        {
#if SILVERLIGHT
            parsedValue = default(T);

            return Enum.TryParse<T>(value, out parsedValue);
#else
            throw new NotImplementedException();
#endif
        }

        public static IList<T> GetValues()
        {
            var list = new List<T>();

            foreach (object value in Enum.GetValues(typeof(T)))
            {
                list.Add((T)value);
            }

            return list;
        }

        public static T GetMaxValue()
        {
            Type type = typeof(T);

            if (!type.IsSubclassOf(typeof(Enum)))
            {
                throw new InvalidCastException ("Cannot cast '" + type.FullName + "' to System.Enum.");
            }

            return (T)Enum.ToObject(type, Enum.GetValues(type).Cast<int>().Last());
        }

        public static IList<string> GetNames()
        {
            return Enum.GetNames(typeof(T));
        }
    }

    public static class EnumUtils
    {
        public static bool HasAnyFlag<T>(this T _enum, T flags) where T : Enum
        {
            var values = GetValues<T>(flags);

            return values.Any(v =>
            {
                var intEnum = (int)(ValueType)_enum;
                var intValue = (int)(ValueType)v;

                if ((intEnum & intValue) == intValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        public static IEnumerable<string> GetNames<T>() where T : Enum
        {
            return Enum.GetNames(typeof(T));
        }

        public static string GetName<T>(T _enum) where T : Enum
        {
            return Enum.GetName(typeof(T), _enum);
        }

        public static IEnumerable<FieldInfo> GetFields<T>() where T : Enum
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);

            return fields;
        }

        public static IEnumerable<FieldInfo> GetFields(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            return fields;
        }

        public static FieldInfo GetField<T>(T _enum) where T :  Enum
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);
            var field = fields.Single(f => ((T) f.GetValue(null)).Equals( _enum));

            return field;
        }

        public static FieldInfo GetField(Enum _enum)
        {
            var type = _enum.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            var field = fields.Single(f => f.GetValue(null).Equals(_enum));

            return field;
        }

        public static IEnumerable<T> GetValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static IEnumerable<T> GetValues<T>(T _enum) where T : Enum
        {
            foreach (var value in GetValues<T>())
            {
                var intEnum = (int)(ValueType)_enum;
                var intValue = (int)(ValueType)value;

                if (intValue == 0 && intEnum != 0)
                {
                    continue;
                }

                if (intValue == 0)
                {
                    if (intEnum == 0)
                    {
                        yield return (T)value;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if ((intEnum & intValue) == intValue)
                {
                    yield return (T)value;
                }
            }
        }

        public static IEnumerable<string> GetValueNames<T>(T _enum) where T : Enum
        {
            foreach (var value in GetValues<T>())
            {
                var intEnum = (int)(ValueType)_enum;
                var intValue = (int)(ValueType)value;

                if (intValue == 0)
                {
                    if (intEnum == 0)
                    {
                        yield return EnumUtils.GetName((T)value);
                    }
                    else
                    {
                        continue;
                    }
                }
                else if ((intEnum & intValue) == intValue)
                {
                    yield return EnumUtils.GetName((T)value);
                }
            }
        }

        public static T GetValue<T>(string name) where T : Enum
        {
            var x = 0;

            foreach (var value in GetValues<T>())
            {
                var nameCheck = GetName(value);

                if (nameCheck.AsCaseless() == name)
                {
                    return (T)value;
                }

                x++;
            }

            return default(T);
        }

        public static int GetIntegerValue(Type enumType, string name)
        {
            var x = 0;

            foreach (var value in Enum.GetValues(enumType))
            {
                var nameCheck = Enum.GetName(enumType, value);

                if (nameCheck.AsCaseless() == name)
                {
                    var underlyingType = enumType.GetEnumUnderlyingType().Name;

                    switch (underlyingType)
                    {
                        case "Int32":
                            return (int)value;
                        case "Byte":
                            return (int)(byte)value;
                        case "Uint":
                            return (int)(uint)value;
                        default:
                            throw new InvalidCastException(string.Format("Unsupported enum type {0}", underlyingType));
                    }
                }

                x++;
            }

            throw new ItemNotFoundException(string.Format("Enum name {0} not found", name));
        }

        public static object GetValue(Type enumType, string name)
        {
            var x = 0;

            foreach (var value in Enum.GetValues(enumType))
            {
                var nameCheck = Enum.GetName(enumType, value);

                if (nameCheck.AsCaseless() == name)
                {
                    return value;
                }

                x++;
            }

            return null;
        }

        public static T ShiftLeft<T>(T _enum, int positions = 1) where T : Enum
        {
            var intEnum = (int)(ValueType)_enum;

            if (intEnum == 0)
            {
                intEnum = positions;
            }
            else
            {
                intEnum <<= positions;
            }

            return (T)(ValueType)intEnum;
        }

        public static T Max<T>(T _enum1, T _enum2) where T :  Enum
        {
            return (T)(ValueType)Math.Max((int)(ValueType)_enum1, (int)(ValueType)_enum2);
        }

        public static T Min<T>(T _enum1, T _enum2) where T :  Enum
        {
            return (T)(ValueType)Math.Min((int)(ValueType)_enum1, (int)(ValueType)_enum2);
        }

        public static int GetPosition<T>(T _enum) where T :  Enum
        {
            var position = (int)Math.Log((int)(ValueType)_enum, 2);

            return position;
        }

        public static T ShiftRight<T>(T _enum) where T :  Enum
        {
            var intEnum = (int)(ValueType)_enum;

            if (intEnum == 0)
            {
                intEnum = 1;
            }
            else
            {
                intEnum >>= intEnum;
            }

            return (T)(ValueType)intEnum;
        }

        public static T Single<T>(Func<T, bool> filter) where T :  Enum
        {
            T returnValue = default(T);

            foreach (var value in GetValues<T>())
            {
                if (filter(value))
                {
                    if (!returnValue.Equals(default(T)))
                    {
                        throw new InvalidOperationException("Sequence contains more than one element");
                    }

                    returnValue = value;
                }
            }

            if (returnValue.Equals(default(T)))
            {
                throw new InvalidOperationException("Sequence contains more than one element");
            }

            return returnValue;
        }

        public static T ToEnum<T>(this string commaDelimitedText) where T :  Enum
        {
            var items = commaDelimitedText.Split(',').Select(t => t.Trim());
            int value = 0;

            foreach (var item in items)
            {
                value |= (int)(ValueType) Enum<T>.Parse(item);
            }

            return (T)(ValueType) value;
        }

        public static T SetFlag<T>(T _enum, T flag) where T :  Enum
        {
            var underlyingType = Enum.GetUnderlyingType(_enum.GetType());

            switch (underlyingType.Name)
            {
                case "Byte":
                case "Int16":
                case "Int32":
                    {
                        var enumInt = (int)(ValueType)_enum;
                        var flagInt = (int)(ValueType)flag;

                        enumInt |= flagInt;

                        return (T)(ValueType)enumInt;
                    }
                case "UInt16":
                case "UInt32":
                    {
                        var enumInt = (uint)(ValueType)_enum;
                        var flagInt = (uint)(ValueType)flag;

                        enumInt |= flagInt;

                        return (T)(ValueType)enumInt;
                    }
            }

            throw new NotSupportedException("Enum type not supported for Utils.EnumUtils.SetFlag");
        }

        public static T RemoveFlag<T>(T _enum, T flag) where T :  Enum
        {
            var underlyingType = Enum.GetUnderlyingType(_enum.GetType());

            switch (underlyingType.Name)
            {
                case "Byte":
                case "Int16":
                case "Int32":
                    {
                        var enumInt = (int)(ValueType)_enum;
                        var flagInt = (int)(ValueType)flag;

                        enumInt &= ~flagInt;

                        return (T)(ValueType)enumInt;
                    }
                case "UInt16":
                case "UInt32":
                    {
                        var enumInt = (uint)(ValueType)_enum;
                        var flagInt = (uint)(ValueType)flag;

                        enumInt &= ~flagInt;

                        return (T)(ValueType)enumInt;
                    }
            }

            throw new NotSupportedException("Enum type not supported for Utils.EnumUtils.RemoveFlag");
        }
    }
}
