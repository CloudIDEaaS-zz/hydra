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
using System;
using System.Collections.Generic;

namespace Decompiler.Core.Types
{
    /// <summary>
    /// Describes a field of a structure.
    /// </summary>
	public class StructureField
	{
        public TypeVariable TypeVariable;

		private string name;

        public StructureField()
        {
        }

		public StructureField(int offset, DataType type)
		{
            if (type == null)
                throw new ArgumentNullException("type");
			this.Offset = offset; this.DataType = type;
		}

		public StructureField(int offset, DataType type, string name)
		{
            if (type == null)
                throw new ArgumentNullException("type");
            this.Offset = offset; this.DataType = type; this.name = name;
		}

		public StructureField Clone()
		{
			return new StructureField(Offset, DataType.Clone(), name);
		}

        public DataType DataType { get; set; }

		public string Name
		{
			get 
			{ 
				if (name != null) return name;
				return string.Format("{0}{1:X4}", DataType.Prefix, Offset);         //$Naming should be given at a different level.
			}
		}

        public int Offset;

        public static int ToOffset(Constant offset)
        {
            if (offset == null)
                return 0;
            PrimitiveType pt = (PrimitiveType) offset.DataType;
            if (pt.Domain == Domain.SignedInt)
                return (int) offset.ToInt32();
            else
                return (int) offset.ToUInt32();
        }

        public override string ToString()
        {
            return string.Format("{0:X}: {1}: {2}", Offset, Name, DataType);
        }
	}

	public class StructureFieldCollection : ICollection<StructureField>
	{
        private List<StructureField> innerList = new List<StructureField>();

        public StructureField this[int i]
        {
            get { return innerList[i]; }
        }

		public void Add(int offset, DataType dt)
		{
			Add(new StructureField(offset, dt));
		}

		public void Add(int offset, DataType dt, string name)
		{
			Add(new StructureField(offset, dt, name));
		}

		public void Add(StructureField f)
		{
			int i;
			StructureField ff = null;
			for (i = 0; i < innerList.Count; ++i)
			{
				ff = innerList[i];
				if (f.Offset == ff.Offset)
				{
					if (f.DataType == ff.DataType)
						return;
				}
				if (f.Offset <= ff.Offset)
					break;
			}
			innerList.Insert(i, f);
		}

        public void AddRange(IEnumerable<StructureField> fields)
        {
            foreach (var field in fields)
            {
                Add(field);
            }
        }

        /// <summary>
        /// Returns the structurefield exactly located at the specified offset.
        /// </summary>
        /// <param name="offset">Offset (in bytes) of the field to retrieve.</param>
        /// <returns>The requested StructureField if it exists at <paramref>offset</paramref>, otherwise null.</returns>
        public StructureField AtOffset(int offset)
        {
            foreach (StructureField f in innerList)
            {
                if (f.Offset == offset)
                    return f;
            }
            return null;
        }

        public void Insert(int i, StructureField f)
		{
			innerList.Insert(i, f);
		}

        /// <summary>
        /// Gets the field with the highest offset that is less than or equal to the
        /// specified <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
		public StructureField LowerBound(int offset)
		{
			StructureField fPrev = null;
			foreach (StructureField f in innerList)
			{
				if (f.Offset <= offset)
					fPrev = f;
			}
			return fPrev;
		}

        public void RemoveAt(int i)
        {
            innerList.RemoveAt(i);
        }

        #region ICollection<StructureField> Members


        public void Clear()
        {
            innerList.Clear();
        }

        public bool Contains(StructureField item)
        {
            return innerList.Contains(item);
        }

        public void CopyTo(StructureField[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(StructureField item)
        {
            return innerList.Remove(item);
        }

        #endregion

        #region IEnumerable<StructureField> Members

        public IEnumerator<StructureField> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion
    }
}
