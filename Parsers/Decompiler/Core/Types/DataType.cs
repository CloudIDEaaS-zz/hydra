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

using Decompiler.Core.Output;
using System;
using System.IO;

namespace Decompiler.Core.Types
{
	/// <summary>
	/// Represents concrete C-like data types inferred by the decompiler as part of the decompilation process.
	/// </summary>
	/// <remarks>
	/// The name 'DataType' is used to avoid conflicts with 'System.Type', which is part of the CLR.
	/// </remarks>
	public abstract class DataType : ICloneable
	{
		public const int BitsPerByte = 8;

		protected DataType()
		{
		}

		protected DataType(string name)
		{
			this.Name = name;
		}

        public int BitSize { get { return Size * BitsPerByte; } }		//$REVIEW: Wrong for 36-bit machines
        public virtual bool IsComplex { get { return false; } }
        public virtual string Name { get; set; }
        public virtual string Prefix { get { return "t"; } }            // Prefix to use when auto-generating field names.
        public abstract int Size { get; set; }  // Size in bytes of the concrete datatype.
        public abstract T Accept<T>(IDataTypeVisitor<T> v);
        public abstract DataType Clone();
        object ICloneable.Clone() { return Clone(); }

        public T ResolveAs<T>() where T : DataType
        {
            DataType dt = this;
            TypeVariable tv = dt as TypeVariable;
            while (tv != null)
            {
                dt = tv.Class.DataType ?? tv.DataType;
                tv = dt as TypeVariable;
            }
            EquivalenceClass eq = dt as EquivalenceClass;
            while (eq != null)
            {
                dt = eq.DataType;
                eq = dt as EquivalenceClass;
            }
            return dt as T;
        }

		protected void ThrowBadSize()
		{
			throw new InvalidOperationException(string.Format("Can't set size of {0}.", GetType().Name));
		}

		public sealed override string ToString()
		{
            var sw = new StringWriter();
            var typeGraphWriter = new TypeGraphWriter(new TextFormatter(sw));
            this.Accept(typeGraphWriter);
            return sw.ToString();
		}
	}
}
