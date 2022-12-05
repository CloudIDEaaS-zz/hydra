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
using System.Collections.Generic;
using System.IO;

namespace Decompiler.Core.Types
{
	/// <summary>
	/// Represents a union of non-compatible types such as int and reals, or pointers and ints, or differently sized integers.
	/// </summary>
	/// <remarks>
	/// Union alternatives are required to be inserted in-order according to the total ordering introduced by DataTypeComparer.
	/// </remarks>
	public class UnionType : DataType
	{
		public DataType PreferredType;

		private UnionAlternativeCollection alts = new UnionAlternativeCollection();

		public UnionType() : base(null)
		{
			this.PreferredType = null;
		}

		public UnionType(string name, DataType preferredType) : base(name)
		{
			this.Name = name;
			this.PreferredType = preferredType; 
		}

		public UnionType(string name, DataType preferredType, ICollection<DataType> alternatives) : base(name)
		{
			this.Name = name; this.PreferredType = preferredType; 
			foreach (DataType dt in alternatives)
			{
				Alternatives.Add(new UnionAlternative(dt));
			}
		}

        public UnionType(string name, DataType preferredType, params DataType [] alternatives) : base(name)
        {
            this.PreferredType = preferredType;
            foreach (DataType dt in alternatives)
            {
                Alternatives.Add(new UnionAlternative(dt));
            }
        }

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitUnion(this);
        }

		public UnionAlternativeCollection Alternatives
		{
			get { return alts; }
		}

		public override DataType Clone()
		{
			DataType pre = PreferredType != null ? PreferredType.Clone() : null;
			UnionType u = new UnionType(Name, pre);
			foreach (UnionAlternative a in this.Alternatives.Values)
			{
				UnionAlternative aClone = new UnionAlternative(a.DataType.Clone());
				u.Alternatives.Add(aClone);
			}
			return u;
		}

		public UnionAlternative FindAlternative(DataType dtOrig)
		{
			foreach (UnionAlternative alt in Alternatives.Values)
			{
				if (Object.Equals(alt.DataType, dtOrig))
					return alt;
			}
			return null;
		}

		public override bool IsComplex
		{
			get { return true; }
		}

		public override string Prefix
		{
			get { return "u"; }
		}

		public override int Size
		{
			get
			{
				int size = 0;
				foreach (UnionAlternative alt in Alternatives.Values)
				{
					size = Math.Max(size, alt.DataType.Size);
				}
				return size;
			}
			set { ThrowBadSize(); }
		}

		public DataType Simplify()
		{
			if (Alternatives.Count == 1)
			{
				return Alternatives.Values[0].DataType;
			}
			else
				return this;
		}
	}

	public class UnionAlternative
	{
		public UnionAlternative(DataType t)
		{
			this.DataType = t;
		}

		public UnionAlternative(string name, DataType dt)
		{
			DataType = dt;
			Name = name;
		}

        public DataType DataType { get; set; }
        public string Name { get; set; }

		public string MakeName(int i)
		{
			if (Name == null)
				return string.Format("u{0}", i);
			else
				return Name;
		}
	}

	public class UnionAlternativeCollection : SortedList<DataType,UnionAlternative>
	{
        public UnionAlternativeCollection()
            : base(new DataTypeComparer())
        {
        }
		public void Add(UnionAlternative a)
		{
            base[a.DataType] = a;
		}

		public void Add(DataType dt)
		{
			Add(new UnionAlternative(dt));
		}

        public void AddRange(IEnumerable<UnionAlternative> alternatives)
        {
            foreach (var alt in alternatives)
            {
                Add(alt);
            }
        }
    }
}
