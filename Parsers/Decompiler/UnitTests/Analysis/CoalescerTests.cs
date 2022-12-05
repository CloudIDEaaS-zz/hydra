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
using Decompiler.Analysis;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class CoalescerTests : AnalysisTestBase
	{
		[Test]
		public void Coa3Converge()
		{
			RunTest("Fragments/3converge.asm", "Analysis/Coa3Converge.txt");
		}

		[Test]
		public void CoaAsciiHex()
		{
			RunTest("Fragments/ascii_hex.asm", "Analysis/CoaAsciiHex.txt");
		}

		[Test]
		public void CoaDataConstraint()
		{
			RunTest("Fragments/data_constraint.asm", "Analysis/CoaDataConstraint.txt");
		}

		[Test]
		public void CoaMoveChain()
		{
			RunTest("Fragments/move_sequence.asm", "Analysis/CoaMoveChain.txt");
		}

		[Test]
		public void CoaFactorialReg()
		{
			RunTest("Fragments/factorial_reg.asm", "Analysis/CoaFactorialReg.txt");
		}

		[Test]
		public void CoaMemoryTest()
		{
			RunTest("Fragments/simple_memoperations.asm", "Analysis/CoaMemoryTest.txt");
		}

		[Test]
		public void CoaSmallLoop()
		{
			RunTest("Fragments/small_loop.asm", "Analysis/CoaSmallLoop.txt");
		}

		[Test]
		public void CoaAddSubCarries()
		{
			RunTest("Fragments/addsubcarries.asm", "Analysis/CoaAddSubCarries.txt");
		}

		[Test]
		public void CoaConditionals()
		{
			RunTest("Fragments/multiple/conditionals.asm", "Analysis/CoaConditionals.txt");
		}

		[Test]
		public void CoaSliceReturn()
		{
			RunTest("Fragments/multiple/slicereturn.asm", "Analysis/CoaSliceReturn.txt");
		}

		[Test]
		public void CoaReg00002()
		{
			RunTest("Fragments/regression00002.asm", "Analysis/CoaReg00002.txt");
		}

		[Test]
		public void CoaWhileGoto()
		{
			RunTest("Fragments/while_goto.asm", "Analysis/CoaWhileGoto.txt");
		}

        [Test]
        public void CoaSideEffectCalls()
        {
            RunTest("Fragments/multiple/sideeffectcalls.asm", "Analysis/CoaSideEffectCalls.txt");
        }

		protected override void RunTest(Program prog, TextWriter fut)
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

				ValuePropagator vp = new ValuePropagator(ssa.Identifiers, proc);
				vp.Transform();
				DeadCode.Eliminate(proc, ssa);
				Coalescer co = new Coalescer(proc, ssa);
				co.Transform();

				ssa.Write(fut);
				proc.Write(false, fut);
				fut.WriteLine();
			}
		}
	}
}
