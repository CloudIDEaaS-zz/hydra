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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using System;

namespace Decompiler.Evaluation
{
	public class SliceConstant_Rule
	{
		private Constant c;
		private Slice slice;

		public bool Match(Slice slice)
		{
			this.slice = slice;
			this.c = slice.Expression as Constant;
            return c != null && c != Constant.Invalid;
		}

		public Expression Transform()
		{
			return Constant.Create(slice.DataType, Slice(c.ToInt32()));
		}

		public int Slice(int val)
		{
			int mask = ((1 << slice.DataType.BitSize) - 1) << slice.Offset;
			return (val & mask) >> slice.Offset;
		}
	}
}
