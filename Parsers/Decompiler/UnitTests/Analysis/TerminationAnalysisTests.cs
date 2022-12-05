﻿#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using Decompiler.Evaluation;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Analysis
{
    [TestFixture]
    public class TerminationAnalysisTests
    {
        ProcedureBase exit;
        ProgramBuilder progMock;
        private ProgramDataFlow flow;
        private Program prog;

        [SetUp]
        public void Setup()
        {
            exit = new ExternalProcedure("exit", 
                new ProcedureSignature(null, new Identifier("retCode", PrimitiveType.Int32, new StackArgumentStorage(0, PrimitiveType.Int32))));
            exit.Characteristics = new ProcedureCharacteristics();
            exit.Characteristics.Terminates = true;

            progMock = new ProgramBuilder();
            flow = new ProgramDataFlow();
        }

        private BlockFlow CreateBlockFlow(Block block, Frame frame)
        {
            return new BlockFlow(
                block,
                prog.Architecture.CreateRegisterBitset(),
                new SymbolicEvaluationContext(prog.Architecture, frame));
        }

        [Test]
        public void BlockTerminates()
        {
            var m = new ProcedureBuilder();
            m.Call(exit, 4);
            var b = m.CurrentBlock;
            m.Return();

            var a = new TerminationAnalysis(flow);
            flow[b] = CreateBlockFlow(b, m.Frame);
            a.Analyze(b);
            Assert.IsTrue(flow[b].TerminatesProcess);
        }

        [Test]
        public void BlockDoesntTerminate()
        {
            var m = new ProcedureBuilder();
            m.Store(m.Word32(0x1231), m.Byte(0));
            var b = m.Block;
            m.Return();
            var a = new TerminationAnalysis(flow);
            prog = new Program
            {
                Architecture = new FakeArchitecture()
            };
            flow[b] = CreateBlockFlow(b, m.Frame);
            a.Analyze(b);
            Assert.IsFalse(flow[b].TerminatesProcess);
        }

        [Test]
        public void ProcedureTerminatesIfBlockTerminates()
        {
            var proc = CompileProcedure("proc", delegate(ProcedureBuilder m)
            {
                m.Call(exit, 4);
                m.Return();
            });
            var prog = progMock.BuildProgram();

            flow = new ProgramDataFlow(prog);
            var a = new TerminationAnalysis(flow);
            a.Analyze(proc);
            Assert.IsTrue(flow[proc].TerminatesProcess);
        }

        [Test]
        public void ProcedureDoesntTerminatesIfOneBranchDoesnt()
        {
            var proc = CompileProcedure("proc", delegate(ProcedureBuilder m)
            {
                m.BranchIf(m.Eq(m.Local32("foo"), m.Word32(0)), "bye");
                m.Call(exit, 4);
                m.Label("bye");
                m.Return();
            });
            var prog = progMock.BuildProgram();

            flow = new ProgramDataFlow(prog);
            var a = new TerminationAnalysis(flow);
            a.Analyze(proc);
            Assert.IsFalse(flow[proc].TerminatesProcess);
        }

        [Test]
        public void ProcedureTerminatesIfAllBranchesDo()
        {
            var proc = CompileProcedure("proc", m => 
            {
                m.BranchIf(m.Eq(m.Local32("foo"), m.Word32(0)), "whee");
                m.Call(exit, 4);
                m.FinishProcedure();
                m.Label("whee");
                m.Call(exit, 4);
                m.FinishProcedure();
            });
            var prog = progMock.BuildProgram();
            flow = new ProgramDataFlow(prog);
            var a = new TerminationAnalysis(flow);
            a.Analyze(proc);
            Assert.IsTrue(flow[proc].TerminatesProcess);
        }

        [Test]
        public void TerminatingSubProcedure()
        {
            var sub = CompileProcedure("sub", m =>
            {
                m.Call(exit, 4);
                m.FinishProcedure();
            });

            Procedure caller = CompileProcedure("caller", m =>
            {
                m.Call(sub, 4);
                m.Return();
            });

            var prog = progMock.BuildProgram();
            flow = new ProgramDataFlow(prog);
            var a = new TerminationAnalysis(flow);
            a.Analyze(prog);
            Assert.IsTrue(flow[sub].TerminatesProcess);
            Assert.IsTrue(flow[caller].TerminatesProcess);
        }

        [Test]
        public void TerminatingApplication()
        {
            var test= CompileProcedure("test", m =>
            {
                m.SideEffect(m.Fn(new ProcedureConstant(PrimitiveType.Pointer32, exit)));
                m.FinishProcedure();
            });
            var prog = progMock.BuildProgram();
            flow = new ProgramDataFlow(prog);
            var a = new TerminationAnalysis(flow);
            a.Analyze(test);
            Assert.IsTrue(flow[test].TerminatesProcess);
        }

        private Procedure CompileProcedure(string procName, Action<ProcedureBuilder> builder)
        {
            var m = new ProcedureBuilder(procName);
            builder(m);
            progMock.Add(m);
            return m.Procedure;
        }
    }
}
