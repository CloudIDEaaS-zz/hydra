using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DllExport
{
	internal static class Null
	{
		public static string IfEmpty(this string input, string replacement)
		{
			if (!string.IsNullOrEmpty(input))
			{
				return input;
			}
			return replacement;
		}

		public static T IfNull<T>(this T input, T replacement)
		{
			if (input != null)
			{
				return input;
			}
			return replacement;
		}

		public static string NullIfEmpty(this string input)
		{
			if (!string.IsNullOrEmpty(input))
			{
				return input;
			}
			return null;
		}

		public static TValue NullSafeCall<T, TValue>(this T input, Func<T, TValue> method)
		{
			if (input != null)
			{
				return method(input);
			}
			return default(TValue);
		}

		public static TValue NullSafeCall<T, TValue>(this T input, Func<TValue> method)
		{
			if (input != null)
			{
				return method();
			}
			return default(TValue);
		}

		public static int NullSafeCount<T>(this ICollection<T> items)
		{
			if (items == null)
			{
				return 0;
			}
			return items.Count;
		}

		public static string NullSafeToLowerInvariant(this string input)
		{
			if (input == null)
			{
				return null;
			}
			return input.ToLowerInvariant();
		}

		public static string NullSafeToString(this object input)
		{
			if (input == null)
			{
				return null;
			}
			return input.ToString();
		}

		public static string NullSafeToUpperInvariant(this string input)
		{
			if (input == null)
			{
				return null;
			}
			return input.ToUpperInvariant();
		}

		public static string NullSafeTrim(this string input, params char[] trimChars)
		{
			if (input == null)
			{
				return null;
			}
			return input.Trim(trimChars);
		}

		public static string NullSafeTrimEnd(this string input, params char[] trimChars)
		{
			if (input == null)
			{
				return null;
			}
			return input.TrimEnd(trimChars);
		}

		public static string NullSafeTrimStart(this string input, params char[] trimChars)
		{
			if (input == null)
			{
				return null;
			}
			return input.TrimStart(trimChars);
		}
	}
}