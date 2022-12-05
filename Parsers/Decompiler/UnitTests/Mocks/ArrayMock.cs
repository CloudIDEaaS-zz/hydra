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

namespace Decompiler.UnitTests.Mocks
{
	/// <summary>
	/// Mock procedure for reading an array of ints of the form:
	/// i = 0; while (i &lt; 100) { a[i] = 0; ++i }
	/// </summary>
	public class WhileLtIncMock : ProcedureBuilder
	{
		protected override void BuildBody()
		{
			var i = base.Local32("i");
			var a = base.Local32("a");
			Assign(i, 0);
			Jump("loopHdr");
			
			Label("loop");
			Store(IAdd(a, IMul(i, 4)), 0);
			Assign(i, IAdd(i, 1));
			Label("loopHdr");
			BranchIf(Lt(i, 100), "loop");
			Return();
		}
	}

	/// <summary>
	/// Mock procedure for reading an array of ints of the form:
	/// i = 100; while (i &gt; 0) { --i; a[i] = 0; }
	/// </summary>
	public class WhileGtDecMock : ProcedureBuilder
	{
		protected override void BuildBody()
		{
			var i = base.Local32("i");
			var a = base.Local32("a");
			Assign(i, 100);
			Jump("loopHdr");
			
			Label("loop");
			Sub(i, i, 1);
			Store(IAdd(a, IMul(i, 4)), 0);
			Label("loopHdr");
			BranchIf(Gt(i, 0), "loop");
			Return();
		}
	}
}
