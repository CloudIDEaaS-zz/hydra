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
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using System;

namespace Decompiler.UnitTests.Typing
{
	public class ConstantUnionMock : ProcedureBuilder
	{
		protected override void BuildBody()
		{
			var i = Constant.Int32(3);		// signed integer
			var r = Constant.Real32(3.0F);						// single-precision real
			var x = Local32("x");
			Assign(x, i);
			Assign(x, r);
		}
	}
}
