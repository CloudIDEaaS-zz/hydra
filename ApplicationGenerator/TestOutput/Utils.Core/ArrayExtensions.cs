using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices; 

namespace Utils
{ 
    public class ArrayBuilder
    { 
        public byte[] Data { get; private set; }

        public ArrayBuilder(byte[] data)
        {
            this.Data = data;
        }

        public static ArrayBuilder operator +(byte left, ArrayBuilder right)
        {
            var destination = new byte[1] { left };

            return new ArrayBuilder(destination.Append(right.Data));
        }

        public static ArrayBuilder operator +(ArrayBuilder left, byte right)
        {
            var append = new byte[1] { right };

            return new ArrayBuilder(left.Data.Append(append));
        }
    }

    public static class ArrayExtensions
    {
#if !SILVERLIGHT
        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public unsafe static extern IntPtr MemSet(byte* dest, int c, int count);

        public static void Xor(this byte[] bytes, byte[] with)
        {
            var max = Math.Min(bytes.Length, with.Length);

            for (var x = 0; x < max; x++)
            {
                bytes[x] ^= with[x];
            }
        }

        public unsafe static void MemSet(this byte[] array, byte _byte, int count)
        {
            fixed (byte* ptr = &array[0])
            {
                MemSet(ptr, _byte, count);
            }
        }

        public static string ToBase64(this string str)
        {
            return Convert.ToBase64String(str.ToArray());
        }

        public static string ToBase64(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64ToString(this string str)
        {
            return Convert.FromBase64String(str).ToText();
        }

        public static byte[] FromBase64(this string str)
        {
            return Convert.FromBase64String(str);
        }

        public static void MemSet(this byte[] array, byte _byte)
        {
            array.MemSet(_byte, array.Length);
        }
#endif
        public static bool AllAreSetTo(this byte[] array, byte _byte, int startingIndex = 0)
        {
            return !array.Skip(startingIndex).Any(b => b != _byte);
        }

        public static bool AllAreZero(this byte[] array, int startingIndex = 0)
        {
            return array.AllAreSetTo(0, startingIndex);
        }

        public static ArrayBuilder ToBuilder(this byte[] data)
        {
            return new ArrayBuilder(data);
        }

        public static byte[] TrimRight(this byte[] data, int length)
        {
            return data.Copy(data.Length - length);
        }

        public static byte[] PadRight(this byte[] data, int length)
        {
            return data.Append(new byte[length - data.Length]);
        }

        public static byte[] ExpandRight(this byte[] data, int length = 1)
        {
            var result = new byte[data.Length + length];

            Array.Copy(data, result, data.Length);

            return result;
        }

        public static byte[] ExpandLeft(this byte[] data, int length = 1)
        {
            var result = new byte[data.Length + length];

            Array.Copy(data, 0, result, length, data.Length);

            return result;
        }

        public static byte[] RemoveLeft(this byte[] data, int length = 1)
        {
            var result = new byte[data.Length - length];

            Array.Copy(data, length, result, 0, data.Length - length);

            return result;
        }

        public static byte[] Copy(this byte[] data, int length = -1)
        {
            var result = new byte[length == -1 ? data.Length : length];

            if (length == -1)
            {
                Array.Copy(data, result, data.Length);
            }
            else
            {
                Array.Copy(data, result, length);
            }

            return result;
        }

        public static byte[] FromHex(this string text)
        {
            var bytes = new List<byte>();

            for (var x = 0; x < text.Length; x += 2)
            {
                var byteText = text.Substring(x, 2);
                var _byte = byte.Parse(byteText, System.Globalization.NumberStyles.HexNumber);

                bytes.Add(_byte);
            }

            return bytes.ToArray();
        }

        public static byte[] ToBytes(this string text)
        {
            return System.Text.UTF8Encoding.UTF8.GetBytes(text);
        }

        public static string ToText(this byte[] bytes)
        {
            return System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static byte[] Prepend(this byte[] destination, byte _byte)
        {
            var start = destination.Length;
            var source = new byte[1];

            destination = destination.ExpandLeft(1);
            source[0] = _byte;

            Array.Copy(source, 0, destination, 0, source.Length);

            return destination;
        }

        public static byte[] Append(this byte[] destination, byte _byte)
        {
            var start = destination.Length;
            var source = new byte[1];

            destination = destination.ExpandRight(1);
            source[0] = _byte;

            Array.Copy(source, 0, destination, start, source.Length);

            return destination;
        }

        public static byte[] Append(this byte[] destination, byte[] source)
        {
            int start;

            if (destination == null)
            {
                destination = new byte[0];
            }

            start = destination.Length;

            destination = destination.ExpandRight(source.Length);

            Array.Copy(source, 0, destination, start, source.Length);

            return destination;
        }
    }
}
