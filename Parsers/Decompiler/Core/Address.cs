#region License
/* 
 * Copyright (C) 1999-2015 John K�ll�n.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.Core
{
	public abstract class Address : Expression, IComparable<Address>, IComparable
	{
        protected Address(DataType size)
            : base(size)
        {
        }

        public static Address Create(DataType size, ulong bitPattern) 
        {
            switch (size.Size)
            {
            default: throw new ArgumentException("size");
            case 2: return Ptr16((ushort)bitPattern);
            case 4: return Ptr32((uint)bitPattern);
            case 8: return Ptr64(bitPattern);
            }
        }

        public static Address Ptr16(ushort addr)
        {
            return new Address16(addr);
        }

        public static Address Ptr32(uint uAddr)
        {
            return new Address32(uAddr);
        }

        public static Address Ptr64(ulong addr)
        {
            return new Address64(addr);
        }

        public static Address SegPtr(ushort seg, uint off)
        {
            return new SegAddress32(seg, (ushort)off);
        }
        
        public static Address FromConstant(Constant value)
        {
            switch (value.DataType.BitSize)
            {
            case 16: return Ptr16(value.ToUInt16());
            case 32: return Ptr32(value.ToUInt32());
            case 64: return Ptr64(value.ToUInt64());
            default: throw new NotImplementedException();
            }
        }

        public abstract bool IsNull { get; }
        public abstract ulong Offset { get; }
        public abstract ushort Selector { get; }			// Segment selector.

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitAddress(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitAddress(this);
        }

        public override void Accept(IExpressionVisitor visit)
        {
            visit.VisitAddress(this);
        }

		public override bool Equals(object obj)
		{
			return CompareTo(obj) == 0;
		}

		public override int GetHashCode()
		{
			return ToLinear().GetHashCode();
		}

        public abstract string GenerateName(string prefix, string suffix);

		public static bool operator < (Address a, Address b)
		{
			return a.ToLinear() < b.ToLinear();
		}

		public static bool operator <= (Address a, Address b)
		{
			return a.ToLinear() <= b.ToLinear();
		}

		public static bool operator > (Address a, Address b)
		{
			return a.ToLinear() > b.ToLinear();
		}

		public static bool operator >= (Address a, Address b)
		{
			return a.ToLinear() >= b.ToLinear();
		}

        public static Address operator + (Address a, ulong off)
		{
			return a.Add((long) off);
		}

		public static Address operator + (Address a, long off)
		{
            return a.Add(off);
        }

        public abstract Address Add(long offset);

		public static Address operator - (Address a, int delta)
		{
			return a.Add(-delta);
		}

		public static long operator - (Address a, Address b)
		{
			return (long) a.ToLinear() - (long) b.ToLinear();
		}

        public int CompareTo(Address a)
        {
            return this.ToLinear().CompareTo(a.ToLinear());
        }

		public int CompareTo(object a)
		{
            return this.ToLinear().CompareTo(((Address)a).ToLinear());
		}

        public abstract ushort ToUInt16();
        public abstract uint ToUInt32();
        public abstract ulong ToLinear();

		/// <summary>
		/// Converts a string representation of an address to an Address.
		/// </summary>
		/// <param name="s">The string representation of the Address</param>
		/// <param name="radix">The radix used in the  representation, typically 16 for hexadecimal address representation.</param>
		/// <returns></returns>

        public static bool TryParse16(string s, out Address result)
        {
            if (s != null)
            {
                try
                {
                    result = Ptr16(Convert.ToUInt16(s, 16));
                    return true;
                }
                catch { }
            }
            result = null;
            return false;
        }

        public static bool TryParse32(string s, out Address result)
        {
            if (s != null)
            {
                try
                {
                    result = Ptr32(Convert.ToUInt32(s, 16));
                    return true;
                }
                catch { }
            }
            result = null;
            return false;
        }

        public static bool TryParse64(string s, out Address result)
        {
            if (s != null)
            {
                try
                {
                    result = Ptr64(Convert.ToUInt64(s, 16));
                    return true;
                }
                catch { }
            }
            result = null;
            return false;
        }

        public class Comparer : IEqualityComparer<Address>
        {
            public bool Equals(Address x, Address y)
            {
                return x.ToLinear() == y.ToLinear();
            }

            public int GetHashCode(Address obj)
            {
                return obj.ToLinear().GetHashCode();
            }
        }
    }

    public class Address16 : Address
    {
        private ushort uValue;

        public Address16(ushort addr)
            : base(PrimitiveType.Ptr16)
        {
            this.uValue = addr;
        }

        public override bool IsNull { get { return uValue == 0; } }
        public override ulong Offset { get { return uValue; } }
        public override ushort Selector { get { throw new NotSupportedException(); } }
        
        public override Address Add(long offset)
        {
            return new Address16((ushort)((int)uValue + (int)offset));
        }

        public override Expression CloneExpression()
        {
            return new Address16(uValue);
        }

        public override string GenerateName(string prefix, string suffix)
        {
            return string.Format("{0}{1:X4}{2}", prefix, uValue, suffix);
        }

        public override ushort ToUInt16()
        {
            return uValue;
        }

        public override uint ToUInt32()
        {
            return uValue;
        }

        public override ulong ToLinear()
        {
            return uValue;
        }

        public override string ToString()
        {
            return string.Format("{0:X4}", uValue);
        }
    }

    public class Address32 : Address
    {
        private uint uValue;

        public Address32(uint addr)
            : base(PrimitiveType.Pointer32)
        {
            this.uValue = addr;
        }

        public override bool IsNull { get { return uValue == 0; } }
        public override ulong Offset { get { return uValue; } }
        public override ushort Selector { get { throw new NotSupportedException(); } }

        public override Address Add(long offset)
        {
            var uNew = uValue + offset;
            return new Address32((uint)uNew);
        }

        public override Expression CloneExpression()
        {
            return new Address32(uValue);
        }

        public override string GenerateName(string prefix, string suffix)
        {
            return string.Format("{0}{1:X8}{2}", prefix, uValue, suffix);
        }

        public override ushort ToUInt16()
        {
            throw new InvalidOperationException("Returning UInt16 would lose precision.");
        }

        public override uint ToUInt32()
        {
            return uValue;
        }

        public override ulong ToLinear()
        {
            return uValue;
        }

        public override string ToString()
        {
            return string.Format("{0:X8}", uValue);
        }
    }

    public class SegAddress32 : Address
    {
        private ushort uSegment;
        private ushort uOffset;

        public SegAddress32(ushort segment, ushort offset)
            : base(PrimitiveType.SegPtr32)
        {
            this.uSegment = segment;
            this.uOffset = offset;
        }

        public override bool IsNull { get { return uSegment == 0 && uOffset == 0; } }
        public override ulong Offset { get { return uOffset; } }
        public override ushort Selector { get { return uSegment; } }

        public override Address Add(long offset)
        {
            ushort sel = this.Selector;
			uint newOff = (uint) (uOffset + offset);
			if (newOff > 0xFFFF)
			{
				sel += 0x1000;
				newOff &= 0xFFFF;
			}
			return new SegAddress32(sel, (ushort) newOff);
		}

        public override Expression CloneExpression()
        {
            return new SegAddress32(uSegment, uOffset);
        }

        public override string GenerateName(string prefix, string suffix)
        {
            return string.Format("{0}{1:X4}_{2:X4}{3}", prefix, uSegment, uOffset, suffix);
        }

        public override ushort ToUInt16()
        {
            throw new InvalidOperationException("Returning UInt16 would lose precision.");
        }

        public override uint ToUInt32()
        {
            return (((uint)uSegment) << 4) + uOffset;
        }

        public override ulong ToLinear()
        {
            return (((ulong)uSegment) << 4) + uOffset;
        }

        public override string ToString()
        {
            return string.Format("{0:X4}:{1:X4}", uSegment, uOffset);
        }

    }

    public class Address64 : Address
    {
        private readonly ulong uValue;

        public Address64(ulong addr)
            : base(PrimitiveType.Pointer64)
        {
            this.uValue = addr;
        }

        public override bool IsNull { get { return uValue == 0; } }
        public override ulong Offset { get { return uValue; } }
        public override ushort Selector { get { throw new NotSupportedException(); } }

        public override Address Add(long offset)
        {
            return new Address64(uValue + (ulong)offset);
        }

        public override Expression CloneExpression()
        {
            return new Address64(uValue);
        }

        public override string GenerateName(string prefix, string suffix)
        {
            return string.Format("{0}{1:X16}{2}", prefix, uValue, suffix);
        }

        public override ushort ToUInt16()
        {
            throw new InvalidOperationException("Returning UInt16 would lose precision.");
        }

        public override uint ToUInt32()
        {
            throw new InvalidOperationException("Returning UInt32 would lose precision.");
        }

        public override ulong ToLinear()
        {
            return uValue;
        }

        public override string ToString()
        {
            return string.Format("{0:X16}", uValue);
        }
    }
}
