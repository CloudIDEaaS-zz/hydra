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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Scanning
{
    [TestFixture]
    public class BlockClonerTests
    {
        private FakeArchitecture arch;
        private Procedure procCalling;
        private CallGraph callgraph;

        [SetUp]
        public void Setup()
        {
            this.arch = new FakeArchitecture();
            this.procCalling = new ProcedureBuilder(arch, "procCalling").Procedure;
            this.callgraph = new CallGraph();
        }

        [Test]
        public void BlockCloner_CloneBlock()
        {
            var m = new ProcedureBuilder(arch, "fn1000");
            var r1 = m.Register("r1");
            var r2 = m.Register("r2");
            var ass = m.Assign(r1, r2);
            m.Return();
            var block = m.Procedure.ControlGraph.Blocks[2];

            m = new ProcedureBuilder(arch, "procCalling");

            var blockCloner = new BlockCloner(block, m.Procedure, callgraph);
            var blockNew = blockCloner.Execute();

            var assNew = (Assignment) blockNew.Statements[0].Instruction;
            Assert.AreNotSame(ass.Dst, assNew.Dst);
            Assert.AreNotSame(ass.Src, assNew.Src);
        }

        private Procedure BuildTest(string procName, Action<ProcedureBuilder> builder)
        {
            var m = new ProcedureBuilder(arch, procName);
            builder(m);
            return m.Procedure;
        }

        [Test]
        public void BlockCloner_CloneBin()
        {
            var proc = BuildTest("fn1000", m =>
            {
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                m.Assign(r1, m.IAdd(r1, r2));
                m.Return();
            });
            var block = proc.ControlGraph.Blocks[2];
            
            var blockCloner = new BlockCloner(block, procCalling, callgraph);
            var blockNew = blockCloner.Execute();

            var assNew = (Assignment) blockNew.Statements[0].Instruction;
            Assert.AreEqual("r1 = r1 + r2", assNew.ToString());
        }

        [Test]
        public void BlockCloner_CloneMem()
        {
            var proc = BuildTest("fn1000", m =>
            {
                var r1 = m.Register("r1");
                var r2 = m.Register("r2");
                m.Assign(r1, m.LoadDw(r2));
                m.Return();
            });
            var block = proc.ControlGraph.Blocks[2];

            var blockCloner = new BlockCloner(block, procCalling, callgraph);
            var blockNew = blockCloner.Execute();

            var assNew = (Assignment) blockNew.Statements[0].Instruction;
            Assert.AreEqual("r1 = Mem0[r2:word32]", assNew.ToString());
            Assert.AreNotSame(proc.Frame.Memory, ((MemoryAccess) assNew.Src).MemoryId);
        }

        [Test]
        public void BlockCloner_CloneTwoBlocks()
        {
            var proc = BuildTest("fn01010", m =>
            {
                var r1 = m.Register("r1");
                m.Assign(r1, m.LoadDw(r1));
                m.Jump("next");
                m.Label("next");
                m.Assign(r1, m.LoadDw(m.Word32(0x123123)));
                m.Return();
            });

            var block = proc.EntryBlock.Succ[0];
            var cloner = new BlockCloner(block, procCalling, callgraph);
            var blockNew = cloner.Execute();

            Assert.AreEqual(4, procCalling.ControlGraph.Blocks.Count, "2 for entry and exit, 2 for cloned blocks");
            Assert.AreSame(procCalling.ExitBlock, blockNew.Succ[0].Succ[0]);
        }

        [Test]
        public void BlockCloner_CloneCall()
        {
            var call = new CallInstruction(new ProcedureConstant(arch.PointerType, procCalling), new CallSite(0, 0));
            var cloner = new BlockCloner(null, procCalling, callgraph);
            var block = new Block(procCalling, "test");
            cloner.Statement = new Statement(42, call, block);
            var newCall = (CallInstruction) call.Accept(cloner);
            Assert.AreEqual(call.Callee, newCall.Callee);
            Assert.AreEqual(1, callgraph.CallerStatements(procCalling).Count(), "Should've added a call to the callgraph");
        }

        [Test]
        public void BlockCloner_Store()
        {
            var proc = BuildTest("fn01010", m =>
            {
                var r1 = m.Register("r1");
                m.Store(r1, r1);
                m.Return();
            });

            var block = proc.EntryBlock.Succ[0];
            var cloner = new BlockCloner(block, procCalling, callgraph);
            var blockNew = cloner.Execute();
        }
    }
}
