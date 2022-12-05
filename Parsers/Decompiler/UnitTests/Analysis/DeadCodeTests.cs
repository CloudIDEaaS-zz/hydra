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
using Decompiler.Analysis;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;


namespace Decompiler.UnitTests.Analysis
{
	/// <summary>
	/// Tests to make sure DeadCodeElimination works.
	/// </summary>
	[TestFixture]
	public class DeadCodeTests : AnalysisTestBase
	{
		[Test]
		public void DeadPushPop()
		{
			RunTest("Fragments/pushpop.asm", "Analysis/DeadPushPop.txt");
		}

		[Test]
		public void DeadFactorialReg()
		{
			RunTest("Fragments/factorial_reg.asm", "Analysis/DeadFactorialReg.txt");
		}

		[Test]
		public void DeadFactorial()
		{
			RunTest("Fragments/factorial.asm", "Analysis/DeadFactorial.txt");
		}

		[Test]
		public void Dead3Converge()
		{
			RunTest("Fragments/3converge.asm", "Analysis/Dead3Converge.txt");
		}

		[Test]
		public void DeadCmpMock()
		{
			RunTest(new CmpMock(), "Analysis/DeadCmpMock.txt");
		}

		[Test]
		public void DeadFnReturn()
		{
			ProcedureBuilder m = new ProcedureBuilder("foo");
			Identifier unused = m.Local32("unused");
			m.Assign(unused, m.Fn("foo", Constant.Word32(1)));
			m.Return();
			RunTest(m, "Analysis/DeadFnReturn.txt");
		}

		protected override void RunTest(Program prog, TextWriter writer)
		{
			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, new FakeDecompilerEventListener());
			dfa.UntangleProcedures();
			foreach (Procedure proc in prog.Procedures.Values)
			{
				Aliases alias = new Aliases(proc, prog.Architecture);
				alias.Transform();
				SsaTransform sst = new SsaTransform(dfa.ProgramDataFlow, proc, proc.CreateBlockDominatorGraph());
				SsaState ssa = sst.SsaState;
				ConditionCodeEliminator cce = new ConditionCodeEliminator(ssa.Identifiers, prog.Platform);
				cce.Transform();

				DeadCode.Eliminate(proc, ssa);
				ssa.Write(writer);
				proc.Write(false, writer);
			}
		}
 	}
}
