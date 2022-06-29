using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class EnumUtil
	{
		public EnumUtil()
		{
		}

		public static StandardValue[] GetStandardValues(object enumInstance)
		{
			return EnumUtil.GetStandardValues(enumInstance.GetType(), BindingFlags.Instance | BindingFlags.Public);
		}

		public static StandardValue[] GetStandardValues(Type enumType)
		{
			return EnumUtil.GetStandardValues(enumType, BindingFlags.Static | BindingFlags.Public);
		}

		private static StandardValue[] GetStandardValues(Type enumType, BindingFlags flags)
		{
			ArrayList arrayLists = new ArrayList();
			FieldInfo[] fields = enumType.GetFields(flags);
			for (int i = 0; i < (int)fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				StandardValue standardValue = new StandardValue(Enum.ToObject(enumType, fieldInfo.GetValue(null)))
				{
					DisplayName = Enum.GetName(enumType, standardValue.Value)
				};
				AddinExpress.Installer.WiXDesigner.DesignTime.DisplayNameAttribute[] customAttributes = fieldInfo.GetCustomAttributes(typeof(AddinExpress.Installer.WiXDesigner.DesignTime.DisplayNameAttribute), false) as AddinExpress.Installer.WiXDesigner.DesignTime.DisplayNameAttribute[];
				if (customAttributes != null && customAttributes.Length != 0)
				{
					standardValue.DisplayName = customAttributes[0].DisplayName;
				}
				DescriptionAttribute[] descriptionAttributeArray = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
				if (descriptionAttributeArray != null && descriptionAttributeArray.Length != 0)
				{
					standardValue.Description = descriptionAttributeArray[0].Description;
				}
				BrowsableAttribute[] browsableAttributeArray = fieldInfo.GetCustomAttributes(typeof(BrowsableAttribute), false) as BrowsableAttribute[];
				if (browsableAttributeArray != null && browsableAttributeArray.Length != 0)
				{
					standardValue.Visible = browsableAttributeArray[0].Browsable;
				}
				ReadOnlyAttribute[] readOnlyAttributeArray = fieldInfo.GetCustomAttributes(typeof(ReadOnlyAttribute), false) as ReadOnlyAttribute[];
				if (readOnlyAttributeArray != null && readOnlyAttributeArray.Length != 0)
				{
					standardValue.Enabled = !readOnlyAttributeArray[0].IsReadOnly;
				}
				arrayLists.Add(standardValue);
			}
			return arrayLists.ToArray(typeof(StandardValue)) as StandardValue[];
		}

		public static bool IsBitsOn(object enumInstance, object bits)
		{
			if (!EnumUtil.IsFlag(enumInstance.GetType()))
			{
				return enumInstance.Equals(bits);
			}
			if (EnumUtil.IsZeroDefinend(enumInstance.GetType()))
			{
				bool flag = EnumUtil.IsZero(enumInstance);
				bool flag1 = EnumUtil.IsZero(bits);
				if (flag & flag1)
				{
					return true;
				}
				if (flag && !flag1)
				{
					return false;
				}
				if (!flag & flag1)
				{
					return false;
				}
			}
			Type underlyingType = Enum.GetUnderlyingType(enumInstance.GetType());
			if (underlyingType == typeof(short))
			{
				short num = Convert.ToInt16(enumInstance);
				short num1 = Convert.ToInt16(bits);
				return (num & num1) == num1;
			}
			if (underlyingType == typeof(ushort))
			{
				ushort num2 = Convert.ToUInt16(enumInstance);
				ushort num3 = Convert.ToUInt16(bits);
				return (num2 & num3) == num3;
			}
			if (underlyingType == typeof(int))
			{
				int num4 = Convert.ToInt32(enumInstance);
				int num5 = Convert.ToInt32(bits);
				return (num4 & num5) == num5;
			}
			if (underlyingType == typeof(uint))
			{
				uint num6 = Convert.ToUInt32(enumInstance);
				uint num7 = Convert.ToUInt32(bits);
				return (num6 & num7) == num7;
			}
			if (underlyingType == typeof(long))
			{
				long num8 = Convert.ToInt64(enumInstance);
				long num9 = Convert.ToInt64(bits);
				return (num8 & num9) == num9;
			}
			if (underlyingType == typeof(ulong))
			{
				ulong num10 = Convert.ToUInt64(enumInstance);
				ulong num11 = Convert.ToUInt64(bits);
				return (num10 & num11) == num11;
			}
			if (underlyingType == typeof(sbyte))
			{
				sbyte num12 = Convert.ToSByte(enumInstance);
				sbyte num13 = Convert.ToSByte(bits);
				return (num12 & num13) == num13;
			}
			if (underlyingType != typeof(byte))
			{
				return false;
			}
			byte num14 = Convert.ToByte(enumInstance);
			byte num15 = Convert.ToByte(bits);
			return (num14 & num15) == num15;
		}

		public static bool IsFlag(Type enumType)
		{
			return enumType.GetCustomAttributes(typeof(FlagsAttribute), false).Length != 0;
		}

		public static bool IsZero(object enumInstance)
		{
			if (!EnumUtil.IsZeroDefinend(enumInstance.GetType()))
			{
				return false;
			}
			Type underlyingType = Enum.GetUnderlyingType(enumInstance.GetType());
			if (underlyingType == typeof(short))
			{
				short num = 0;
				return Enum.ToObject(enumInstance.GetType(), num).Equals(enumInstance);
			}
			if (underlyingType == typeof(ushort))
			{
				ushort num1 = 0;
				return Enum.ToObject(enumInstance.GetType(), num1).Equals(enumInstance);
			}
			if (underlyingType == typeof(int))
			{
				int num2 = 0;
				return Enum.ToObject(enumInstance.GetType(), num2).Equals(enumInstance);
			}
			if (underlyingType == typeof(uint))
			{
				uint num3 = 0;
				return Enum.ToObject(enumInstance.GetType(), num3).Equals(enumInstance);
			}
			if (underlyingType == typeof(long))
			{
				long num4 = (long)0;
				return Enum.ToObject(enumInstance.GetType(), num4).Equals(enumInstance);
			}
			if (underlyingType == typeof(ulong))
			{
				ulong num5 = (ulong)0;
				return Enum.ToObject(enumInstance.GetType(), num5).Equals(enumInstance);
			}
			if (underlyingType == typeof(sbyte))
			{
				sbyte num6 = 0;
				return Enum.ToObject(enumInstance.GetType(), num6).Equals(enumInstance);
			}
			if (underlyingType != typeof(byte))
			{
				return false;
			}
			byte num7 = 0;
			return Enum.ToObject(enumInstance.GetType(), num7).Equals(enumInstance);
		}

		public static bool IsZeroDefinend(Type enumType)
		{
			Type underlyingType = Enum.GetUnderlyingType(enumType);
			if (underlyingType == typeof(short))
			{
				return Enum.IsDefined(enumType, 0);
			}
			if (underlyingType == typeof(ushort))
			{
				return Enum.IsDefined(enumType, 0);
			}
			if (underlyingType == typeof(int))
			{
				return Enum.IsDefined(enumType, 0);
			}
			if (underlyingType == typeof(uint))
			{
				return Enum.IsDefined(enumType, 0);
			}
			if (underlyingType == typeof(long))
			{
				return Enum.IsDefined(enumType, (long)0);
			}
			if (underlyingType == typeof(ulong))
			{
				return Enum.IsDefined(enumType, (long)0);
			}
			if (underlyingType == typeof(sbyte))
			{
				return Enum.IsDefined(enumType, 0);
			}
			if (underlyingType != typeof(byte))
			{
				return false;
			}
			return Enum.IsDefined(enumType, 0);
		}

		public static bool TurnOffBits(ref object enumInstance, object bits)
		{
			if (!EnumUtil.IsFlag(enumInstance.GetType()))
			{
				return false;
			}
			if (!EnumUtil.IsBitsOn(enumInstance, bits))
			{
				return false;
			}
			if (EnumUtil.IsZeroDefinend(enumInstance.GetType()))
			{
				bool flag = EnumUtil.IsZero(enumInstance);
				bool flag1 = EnumUtil.IsZero(bits);
				if (flag & flag1)
				{
					return false;
				}
				if (flag && !flag1)
				{
					return false;
				}
				if (!flag & flag1)
				{
					return false;
				}
			}
			Type type = enumInstance.GetType();
			Type underlyingType = Enum.GetUnderlyingType(enumInstance.GetType());
			if (underlyingType == typeof(short))
			{
				int num = Convert.ToInt32(enumInstance);
				int num1 = Convert.ToInt32(bits);
				enumInstance = num & ~num1;
			}
			else if (underlyingType == typeof(ushort))
			{
				uint num2 = Convert.ToUInt32(enumInstance);
				uint num3 = Convert.ToUInt32(bits);
				enumInstance = num2 & ~num3;
			}
			else if (underlyingType == typeof(int))
			{
				int num4 = Convert.ToInt32(enumInstance);
				int num5 = Convert.ToInt32(bits);
				enumInstance = num4 & ~num5;
			}
			else if (underlyingType == typeof(uint))
			{
				uint num6 = Convert.ToUInt32(enumInstance);
				uint num7 = Convert.ToUInt32(bits);
				enumInstance = num6 & ~num7;
			}
			else if (underlyingType == typeof(long))
			{
				long num8 = Convert.ToInt64(enumInstance);
				long num9 = Convert.ToInt64(bits);
				enumInstance = num8 & ~num9;
			}
			else if (underlyingType == typeof(ulong))
			{
				ulong num10 = Convert.ToUInt64(enumInstance);
				ulong num11 = Convert.ToUInt64(bits);
				enumInstance = num10 & ~num11;
			}
			else if (underlyingType == typeof(sbyte))
			{
				int num12 = Convert.ToInt32(enumInstance);
				int num13 = Convert.ToInt32(bits);
				enumInstance = num12 & ~num13;
			}
			else if (underlyingType == typeof(byte))
			{
				int num14 = Convert.ToInt32(enumInstance);
				int num15 = Convert.ToInt32(bits);
				enumInstance = num14 & ~num15;
			}
			enumInstance = Enum.ToObject(type, enumInstance);
			return true;
		}

		public static bool TurnOnBits(ref object enumInstance, object bits)
		{
			if (!EnumUtil.IsFlag(enumInstance.GetType()))
			{
				if (enumInstance.Equals(bits))
				{
					return false;
				}
				enumInstance = bits;
				return true;
			}
			if (EnumUtil.IsBitsOn(enumInstance, bits))
			{
				return false;
			}
			if (EnumUtil.IsZeroDefinend(enumInstance.GetType()))
			{
				bool flag = EnumUtil.IsZero(enumInstance);
				bool flag1 = EnumUtil.IsZero(bits);
				if (flag & flag1)
				{
					return false;
				}
				if (flag && !flag1)
				{
					enumInstance = bits;
					return true;
				}
				if (!flag & flag1)
				{
					enumInstance = bits;
					return true;
				}
			}
			Type type = enumInstance.GetType();
			Type underlyingType = Enum.GetUnderlyingType(enumInstance.GetType());
			if (underlyingType == typeof(short))
			{
				int num = Convert.ToInt32(enumInstance);
				enumInstance = num | Convert.ToInt32(bits);
			}
			else if (underlyingType == typeof(ushort))
			{
				uint num1 = Convert.ToUInt32(enumInstance);
				enumInstance = num1 | Convert.ToUInt32(bits);
			}
			else if (underlyingType == typeof(int))
			{
				int num2 = Convert.ToInt32(enumInstance);
				enumInstance = num2 | Convert.ToInt32(bits);
			}
			else if (underlyingType == typeof(uint))
			{
				uint num3 = Convert.ToUInt32(enumInstance);
				enumInstance = num3 | Convert.ToUInt32(bits);
			}
			else if (underlyingType == typeof(long))
			{
				long num4 = Convert.ToInt64(enumInstance);
				enumInstance = num4 | Convert.ToInt64(bits);
			}
			else if (underlyingType == typeof(ulong))
			{
				ulong num5 = Convert.ToUInt64(enumInstance);
				enumInstance = num5 | Convert.ToUInt64(bits);
			}
			else if (underlyingType == typeof(sbyte))
			{
				int num6 = Convert.ToInt32(enumInstance);
				enumInstance = num6 | Convert.ToInt32(bits);
			}
			else if (underlyingType == typeof(byte))
			{
				int num7 = Convert.ToInt32(enumInstance);
				enumInstance = num7 | Convert.ToInt32(bits);
			}
			enumInstance = Enum.ToObject(type, enumInstance);
			return true;
		}
	}
}