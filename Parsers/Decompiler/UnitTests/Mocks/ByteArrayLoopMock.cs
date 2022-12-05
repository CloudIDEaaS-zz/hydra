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
using System;

namespace Decompiler.UnitTests.Mocks
{
	public class ByteArrayLoopMock : ProcedureBuilder
	{
		protected override void BuildBody()
		{
			var a = Local32("a");
			var i = Local32("i");
			Assign(i, 0);
			Label("loop");
			BranchIf(Lt(i,10), "body");
			Return();
			Label("body");
			Store(IAdd(a, i), Int8(0));
			Assign(i, IAdd(i, 1));
			Jump("loop");
		}
	}
}
