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

using Decompiler.Analysis;
using Decompiler.Evaluation;
using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Evaluation
{
	[TestFixture]
	public class IdConstantTests
	{
		private ProcedureBuilder m;
        private SsaIdentifierCollection ssa;

		[SetUp]
		public void Setup()
		{
			m = new ProcedureBuilder();
            ssa = new SsaIdentifierCollection();
		}

		[Test]
		public void ConstantPropagate()
		{
			Identifier ds = m.Frame.EnsureRegister(Registers.ds);
            var c = Constant.Word16(0x1234);
            m.Assign(ds, c);
			m.SideEffect(ds);
            var def = m.Block.Statements[0];
            var use = m.Block.Statements[1];
			SsaIdentifier sid_ds = ssa.Add(ds, def, c, false);
            var ass = (Assignment)def.Instruction;
            ass.Dst = sid_ds.Identifier;
            ((SideEffect)use.Instruction).Expression = sid_ds.Identifier;
			sid_ds.Uses.Add(use);

			IdConstant ic = new IdConstant(new SsaEvaluationContext(ssa), new Unifier(null));
            Assert.IsTrue(ic.Match(sid_ds.Identifier));
			Expression e = ic.Transform();
			Assert.AreEqual("selector", e.DataType.ToString());
		}
	}
}
