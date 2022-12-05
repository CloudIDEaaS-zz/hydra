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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Lib;
using Decompiler.Analysis;
using Decompiler.UnitTests.Mocks;
using Decompiler.UnitTests.TestCode;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using Rhino.Mocks;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class DataFlowAnalysisTests : AnalysisTestBase
	{
		private DataFlowAnalysis dfa;
        private MockRepository mr;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
        }

		[Test]
		public void DfaAsciiHex()
		{
			RunTest("Fragments/ascii_hex.asm", "Analysis/DfaAsciiHex.txt");
		}

		[Test]
        [Ignore("Stack arrays are not supported yet")]
		public void DfaAutoArray32()
		{
			RunTest32("Fragments/autoarray32.asm", "Analysis/DfaAutoArray32.txt");
		}

		[Test]
		public void DfaFactorial()
		{
			RunTest("Fragments/factorial.asm", "Analysis/DfaFactorial.txt");
		}

		[Test]
		public void DfaFactorialReg()
		{
			RunTest("Fragments/factorial_reg.asm", "Analysis/DfaFactorialReg.txt");
		}

		[Test]
		public void DfaFibonacci()
		{
			RunTest32("Fragments/multiple/fibonacci.asm", "Analysis/DfaFibonacci.txt");
		}


		[Test]
		public void DfaFpuOps()
		{
			RunTest("Fragments/fpuops.asm", "Analysis/DfaFpuOps.txt");
		}

		[Test]
		public void DfaMutualTest()
		{
			RunTest("Fragments/multiple/mutual.asm", "Analysis/DfaMutualTest.txt");
		}

		[Test]
		public void DfaChainTest()
		{
			RunTest("Fragments/multiple/chaincalls.asm", "Analysis/DfaChainTest.txt");
		}

		[Test]
		public void DfaGlobalHandle()
		{
            Given_FakeWin32Platform(mr);
            mr.ReplayAll();
            RunTest32("Fragments/import32/GlobalHandle.asm", "Analysis/DfaGlobalHandle.txt");
		}

		[Test]
		public void DfaMoveChain()
		{
			RunTest("Fragments/move_sequence.asm", "Analysis/DfaMoveChain.txt");
		}

		[Test]
		public void DfaNegsNots()
		{
			RunTest("Fragments/negsnots.asm", "Analysis/DfaNegsNots.txt");
		}

		[Test]
		public void DfaPreservedAlias()
		{
			RunTest("Fragments/multiple/preserved_alias.asm", "Analysis/DfaPreservedAlias.txt");
		}

		[Test]
		public void DfaReadFile()
		{
			RunTest("Fragments/multiple/read_file.asm", "Analysis/DfaReadFile.txt");
		}

		[Test]
		public void DfaStackPointerMessing()
		{
			RunTest("Fragments/multiple/stackpointermessing.asm", "Analysis/DfaStackPointerMessing.txt");
		}

		[Test]
		public void DfaStringInstructions()
		{
			RunTest("Fragments/stringinstr.asm", "Analysis/DfaStringInstructions.txt");
		}

		[Test]
		public void DfaSuccessiveDecs()
		{
			RunTest("Fragments/multiple/successivedecs.asm", "Analysis/DfaSuccessiveDecs.txt");
		}

		[Test]
		public void DfaWhileBigHead()
		{
			RunTest("Fragments/while_bighead.asm", "Analysis/DfaWhileBigHead.txt");
		}

		[Test]
		public void DfaWhileGoto()
		{
			RunTest("Fragments/while_goto.asm", "Analysis/DfaWhileGoto.txt");
		}

		[Test]
		public void DfaRecurseWithPushes()
		{
			RunTest("Fragments/multiple/recurse_with_pushes.asm", "Analysis/DfaRecurseWithPushes.txt");
		}

		[Test]
		public void DfaReg00009()
		{
			RunTest("Fragments/regressions/r00009.asm", "Analysis/DfaReg00009.txt");
		}

		[Test]
		public void DfaReg00010()
		{
			RunTest("Fragments/regressions/r00010.asm", "Analysis/DfaReg00010.txt");
		}

        [Test]
        public void DfaReg00011()
        {
            RunTest("Fragments/regressions/r00011.asm", "Analysis/DfaReg00011.txt");
        }

        [Test]
        public void DfaReg00015()
        {
            RunTest("Fragments/regressions/r00015.asm", "Analysis/DfaReg00015.txt");
        }

        [Test]
        public void DfaFstsw()
        {
           var prog = RewriteCodeFragment(@"
                fcomp   dword ptr [bx]
                fstsw   ax
                test    ah,0x41
                jpo     done
                mov     word ptr [si],4
done:   
                ret
");
           SaveRunOutput(prog, RunTest, "Analysis/DfaFstsw.txt");
        }

        [Test]
        public void DfaManyIncrements()
        {
            RunTest(new ManyIncrements(), "Analysis/DfaManyIncrements.txt");
        }

		protected override void RunTest(Program prog, TextWriter writer)
		{
			dfa = new DataFlowAnalysis(prog, new FakeDecompilerEventListener());
			dfa.AnalyzeProgram();
			foreach (Procedure proc in prog.Procedures.Values)
			{
				ProcedureFlow flow = dfa.ProgramDataFlow[proc];
				writer.Write("// ");
				flow.Signature.Emit(proc.Name, ProcedureSignature.EmitFlags.ArgumentKind|ProcedureSignature.EmitFlags.LowLevelInfo, writer);
				flow.Emit(prog.Architecture, writer);
				proc.Write(false, writer);
				writer.WriteLine();
			}
		}	

	}
}
