using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;

namespace Utils
{
    public static class Enum<T> where T : struct
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
        public static bool HasFlag(this Keys keys, Keys key)
        {
            return keys.HasFlag(key);
        }

        public static bool HasAnyFlag<T>(this T _enum, T flags) where T : struct, IComparable, IConvertible, IFormattable
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

        public static IEnumerable<string> GetNames<T>() where T : struct, IComparable, IConvertible, IFormattable
        {
            return Enum.GetNames(typeof(T));
        }

        public static string GetName<T>(T _enum) where T : struct, IComparable, IConvertible, IFormattable
        {
            return Enum.GetName(typeof(T), _enum);
        }

        public static FieldInfo GetField<T>(T _enum) where T : struct, IComparable, IConvertible, IFormattable
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

        public static IEnumerable<T> GetValues<T>() where T : struct, IComparable, IConvertible, IFormattable
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static IEnumerable<T> GetValues<T>(T _enum) where T : struct, IComparable, IConvertible, IFormattable
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

        public static IEnumerable<string> GetValueNames<T>(T _enum) where T : struct, IComparable, IConvertible, IFormattable
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

        public static T GetValue<T>(string name) where T : struct, IComparable, IConvertible, IFormattable
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

        public static T ShiftLeft<T>(T _enum, int positions = 1) where T : struct, IComparable, IConvertible, IFormattable
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

        public static T Max<T>(T _enum1, T _enum2) where T : struct, IComparable, IConvertible, IFormattable
        {
            return (T)(ValueType)Math.Max((int)(ValueType)_enum1, (int)(ValueType)_enum2);
        }

        public static T Min<T>(T _enum1, T _enum2) where T : struct, IComparable, IConvertible, IFormattable
        {
            return (T)(ValueType)Math.Min((int)(ValueType)_enum1, (int)(ValueType)_enum2);
        }

        public static int GetPosition<T>(T _enum) where T : struct, IComparable, IConvertible, IFormattable
        {
            var position = (int)Math.Log((int)(ValueType)_enum, 2);

            return position;
        }

        public static T ShiftRight<T>(T _enum) where T : struct, IComparable, IConvertible, IFormattable
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

        public static T Single<T>(Func<T, bool> filter) where T : struct, IComparable, IConvertible, IFormattable
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

        public static T ToEnum<T>(this string commaDelimitedText) where T : struct, IComparable, IConvertible, IFormattable
        {
            var items = commaDelimitedText.Split(',').Select(t => t.Trim());
            int value = 0;

            foreach (var item in items)
            {
                value |= (int)(ValueType) Enum<T>.Parse(item);
            }

            return (T)(ValueType) value;
        }

        public static T SetFlag<T>(T _enum, T flag) where T : struct, IComparable, IConvertible, IFormattable
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

        public static T RemoveFlag<T>(T _enum, T flag) where T : struct, IComparable, IConvertible, IFormattable
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
