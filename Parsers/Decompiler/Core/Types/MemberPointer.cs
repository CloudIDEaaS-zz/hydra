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

using System;
using System.IO;

namespace Decompiler.Core.Types
{
	/// <summary>
	/// Represents a member pointer type, as in C++: int foo::*bar makes bar a pointer to a member of foo.
	/// </summary>
	public class MemberPointer : DataType
	{
		private DataType pointee;
		private DataType basePtr;
		private int byteSize;

		public MemberPointer(DataType basePtr, DataType pointee, int byteSize)
		{
			this.Pointee = pointee;
			this.BasePointer = basePtr;
			this.byteSize = byteSize;
		}

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitMemberPointer(this);
        }

		public override DataType Clone()
		{
			return new MemberPointer(BasePointer.Clone(), Pointee.Clone(), byteSize);
		}

		public override bool IsComplex
		{
			get { return true; }
		}

		/// <summary>
		/// The offset part of a member pointer.
		/// </summary>
		public DataType Pointee
		{
			get { return pointee; }
			set 
			{
				if (value == null) throw new ArgumentNullException("Pointee mustn't be null");
				pointee = value; 
			}
		}

		public override string Prefix
		{
			get { return "ptr"; }
		}

		public override int Size
		{
			get { return byteSize; }
			set { ThrowBadSize(); }
		}

		public DataType BasePointer
		{
			get { return basePtr; }
			set 
			{
				if (value == null) throw new NullReferenceException(); 
				basePtr = value;
			}
		}
	}
}
