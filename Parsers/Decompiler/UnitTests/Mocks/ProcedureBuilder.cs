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
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System.Collections.Generic;
using System;

namespace Decompiler.UnitTests.Mocks
{
    /// <summary>
    /// Supports the building of a procedure without having to go through assembler.
    /// </summary>
    public class ProcedureBuilder : CodeEmitter
    {
        private Dictionary<string, Block> blocks;
        private Block branchBlock;
        private Block lastBlock;
        private int numBlock;
        private List<ProcUpdater> unresolvedProcedures;
        public uint LinearAddress;

        public ProcedureBuilder()
        {
            Init(new FakeArchitecture(), this.GetType().Name, null);
        }

        public ProcedureBuilder(string name)
        {
            Init(new FakeArchitecture(), name, null);
        }

        public ProcedureBuilder(IProcessorArchitecture arch)
        {
            Init(arch, this.GetType().Name, null);
        }

        public ProcedureBuilder(IProcessorArchitecture arch, string name)
        {
            Init(arch,name,null);
        }

        public ProcedureBuilder(IProcessorArchitecture arch, string name, Dictionary<string, Block> blocks)
        {
            Init(arch, name, blocks);
        }

        private void Init(IProcessorArchitecture arch, string name, Dictionary<string, Block> blocks)
        {
            if (arch == null)
                throw new ArgumentNullException("arch");
            this.Architecture = arch;
            this.Procedure = new Procedure(name, arch.CreateFrame());
            this.blocks = blocks ?? new Dictionary<string, Block>();
            this.unresolvedProcedures = new List<ProcUpdater>();
            BuildBody();
        }

        /// <summary>
        /// Current block, into which the next statement will be added.
        /// </summary>
        public Block Block { get; private set; }
        public Procedure Procedure { get; private set; }
        public ProgramBuilder ProgramMock { get; set; }
        public IProcessorArchitecture Architecture { get; private set; }

        private Block BlockOf(string label)
        {
            Block b;
            if (!blocks.TryGetValue(label, out b))
            {
                b = Procedure.AddBlock(label);
                blocks.Add(label, b);
            }
            return b;
        }

        public void BranchCc(ConditionCode cc, string label)
        {
            Identifier f;
            switch (cc)
            {
            case ConditionCode.EQ:
            case ConditionCode.NE: f = Flags("Z"); break;
            default: throw new ArgumentOutOfRangeException("Condition code: " + cc);
            }
            branchBlock = BlockOf(label);
            Emit(new Branch(new TestCondition(cc, f), branchBlock));
            TerminateBlock();
        }

        public Statement BranchIf(Expression expr, string label)
        {
            Block b = EnsureBlock(null);
            branchBlock = BlockOf(label);
            TerminateBlock();

            Statement stm = new Statement(0, new Branch(expr, branchBlock), b);
            b.Statements.Add(stm);
            return stm;
        }

        protected virtual void BuildBody()
        {
        }

        public Statement Call(string procedureName, int retSizeOnStack)
        {
            var ci = new CallInstruction(Constant.Invalid, new CallSite(retSizeOnStack, 0)); 
            unresolvedProcedures.Add(new ProcedureConstantUpdater(procedureName, ci));
            return Emit(ci);
        }

        public Statement Call(ProcedureBase callee, int retSizeOnStack)
        {
            ProcedureConstant c = new ProcedureConstant(PrimitiveType.Pointer32, callee);
            CallInstruction ci = new CallInstruction(c, new CallSite(retSizeOnStack, 0));  
            return Emit(ci);
        }

        public Statement Call(Expression e, int retSizeOnstack)
        {
            CallInstruction ci = new CallInstruction(e, new CallSite(retSizeOnstack, 0));
            return Emit(ci);
        }

        public void Compare(string flags, Expression a, Expression b)
        {
            Assign(Flags(flags), new ConditionOf(ISub(a, b)));
        }

        public Block CurrentBlock
        {
            get { return this.Block; }
        }

        public Identifier Declare(DataType dt, string name)
        {
            return Procedure.Frame.CreateTemporary(name, dt);
        }

        public Identifier Declare(DataType dt, string name, Expression expr)
        {
            Identifier id = Procedure.Frame.CreateTemporary(name, dt);
            Emit(new Declaration(id, expr));
            return id;
        }

        public Statement Declare(Identifier id, Expression initial)
        {
            return Emit(new Declaration(id, initial));
        }

        public override Statement Emit(Instruction instr)
        {
            EnsureBlock(null);
            Block.Statements.Add(LinearAddress++, instr);
            return Block.Statements.Last;
        }

        public Identifier Flags(string s)
        {
            uint grf = 0;
            for (int i = 0; i < s.Length; ++i)
            {
                switch (s[i])
                {
                case 'S': grf |= 0x01; break;
                case 'C': grf |= 0x02; break;
                case 'Z': grf |= 0x04; break;
                case 'O': grf |= 0x10; break;
                }
            }
            return base.Flags(grf, s);
        }

        public Application Fn(string name, params Expression[] exps)
        {
            Application appl = new Application(
                new ProcedureConstant(PrimitiveType.Pointer32, new PseudoProcedure(name, VoidType.Instance, 0)),
                PrimitiveType.Word32, exps);
            unresolvedProcedures.Add(new ApplicationUpdater(name, appl));
            return appl;
        }

        public void Jump(string name)
        {
            EnsureBlock(null);
            Block blockTo = BlockOf(name);
            Procedure.ControlGraph.AddEdge(Block, blockTo);
            Block = null;
        }

        public Block Label(string name)
        {
            TerminateBlock();
            return EnsureBlock(name);
        }

        private Block EnsureBlock(string name)
        {
            if (Block != null)
                return Block;

            if (name == null)
            {
                name = string.Format("l{0}", ++numBlock);
            }
            Block = BlockOf(name);
            if (Procedure.EntryBlock.Succ.Count == 0)
            {
                Procedure.ControlGraph.AddEdge(Procedure.EntryBlock, Block);
            }

            if (lastBlock != null)
            {
                if (branchBlock != null)
                {
                    Procedure.ControlGraph.AddEdge(lastBlock, Block);
                    Procedure.ControlGraph.AddEdge(lastBlock, branchBlock);
                    branchBlock = null;
                }
                else
                {
                    Procedure.ControlGraph.AddEdge(lastBlock, Block);
                }
                lastBlock = null;
            }
            return Block;
        }

        public void FinishProcedure()
        {
            TerminateBlock();
            Procedure.ControlGraph.AddEdge(lastBlock, Procedure.ExitBlock);
        }

        public ICollection<ProcUpdater> UnresolvedProcedures
        {
            get { return unresolvedProcedures; }
        }

        public override Frame Frame
        {
            get { return Procedure.Frame; }
        }

        public override Identifier Register(int i)
        {
            return Frame.EnsureRegister(Architecture.GetRegister(i));
        }

        public Identifier Register(string name)
        {
            return Frame.EnsureRegister(Architecture.GetRegister(name));
        }

        public Identifier Register(RegisterStorage reg)
        {
            return Frame.EnsureRegister(reg);
        }

        public override void Return()
        {
            base.Return();
            Procedure.ControlGraph.AddEdge(Block, Procedure.ExitBlock);
            TerminateBlock();
            lastBlock = null;
        }
        public override void Return(Expression exp)
        {
            base.Return(exp);
            Procedure.ControlGraph.AddEdge(Block, Procedure.ExitBlock);
            TerminateBlock();
            lastBlock = null;
        }

        public void Switch(Expression e, params string[] labels)
        {
            Block[] blox = new Block[labels.Length];
            for (int i = 0; i < blox.Length; ++i)
            {
                blox[i] = BlockOf(labels[i]);
            }

            Emit(new SwitchInstruction(e, blox));
            for (int i = 0; i < blox.Length; ++i)
            {
                Procedure.ControlGraph.AddEdge(this.Block, blox[i]);
            }
            lastBlock = null;
            Block = null;
        }

        private void TerminateBlock()
        {
            if (Block != null)
            {
                lastBlock = Block;
                Block = null;
            }
        }

        public Identifier Reg32(string name)
        {
            return Frame.EnsureRegister(new RegisterStorage(name, 1, PrimitiveType.Word32));
        }

        public Identifier Reg16(string name)
        {
            return Frame.EnsureRegister(new RegisterStorage(name, 1, PrimitiveType.Word16));
        }

        public Identifier Reg8(string name)
        {
            return Frame.EnsureRegister(new RegisterStorage(name, 1, PrimitiveType.Byte));
        }
    }
}
