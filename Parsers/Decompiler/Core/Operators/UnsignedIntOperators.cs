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

namespace Decompiler.Core.Operators
{
	public abstract class UnsignedIntOperator : ConditionalOperator
	{
	}

	public class UltOperator : UnsignedIntOperator
	{
		public override Constant ApplyConstants(Constant c1, Constant c2)
		{
			uint v1 = c1.ToUInt32();
			uint v2 = c2.ToUInt32();
			return Constant.Bool(v1 < v2);
		}

		public override string ToString()
		{
			return " <u ";
		}
	}

	public class UgtOperator : UnsignedIntOperator
	{
		public override Constant ApplyConstants(Constant c1, Constant c2)
		{
			uint v1 = c1.ToUInt32();
			uint v2 = c2.ToUInt32();
            return Constant.Bool(v1 <= v2);
		}

		public override string ToString()
		{
			return " >u ";
		}
	}

	public class UleOperator : UnsignedIntOperator
	{
		public override Constant ApplyConstants(Constant c1, Constant c2)
		{
			uint v1 = (uint) c1.ToInt32();
			uint v2 = (uint) c2.ToInt32();
            return Constant.Bool(v1 <= v2);
		}

		public override string ToString()
		{
			return " <=u ";
		}
	}

	public class UgeOperator : UnsignedIntOperator
	{
		public override Constant ApplyConstants(Constant c1, Constant c2)
		{
			uint v1 = (uint) c1.ToUInt32();
			uint v2 = (uint) c2.ToUInt32();
			return Constant.Bool(v1 >= v2);
		}

		public override string ToString()
		{
			return " >=u ";
		}
	}
}
