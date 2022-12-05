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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Analysis
{
    /// <summary>
    /// Finds all provably dead registers within the basic blocks of the 
    /// program, and removes them. 
    /// </summary>
    /// <remarks>
    /// This is a cheap preprocessing stage that will remove the great majority 
    /// of unused registers, especially condition codes, of disassembled and 
    /// rewritten code. This avoids the unneccesary generation of SSA variables 
    /// for dead code.
    /// </remarks>
    public class IntraBlockDeadRegisters : 
        InstructionVisitor<bool>,
        StorageVisitor<bool, bool>
    {
        private ExpVisitor expVisitor;
        private HashSet<RegisterStorage> deadRegs;
        private uint deadFlags;

        public static void Apply(Program program)
        {
            foreach (var block in program.Procedures.Values.SelectMany(p => p.ControlGraph.Blocks))
            {
                var ibdr = new IntraBlockDeadRegisters();
                ibdr.Apply(block);
            }
        }

        public IntraBlockDeadRegisters()
        {
            this.expVisitor = new ExpVisitor(this);
            this.deadRegs = new HashSet<RegisterStorage>();
        }

        public void Apply(Block block)
        {
            var dead = new HashSet<int>();
            for (int i = block.Statements.Count - 1; i >= 0; --i)
            {
                if (block.Statements[i].Instruction.Accept(this))
                {
                    dead.Add(i);
                }
            }
            int iDst = 0;
            for (int iSrc = 0; iSrc < block.Statements.Count; ++iSrc)
            {
                if (!dead.Contains(iSrc))
                {
                    block.Statements[iDst] = block.Statements[iSrc];
                    ++iDst;
                }
            }
            block.Statements.RemoveRange(iDst, block.Statements.Count - iDst);
        }

        private class ExpVisitor : ExpressionVisitor<bool>
        {
            private IntraBlockDeadRegisters outer;

            public ExpVisitor(IntraBlockDeadRegisters outer)
            {
                this.outer = outer;
            }

            public bool VisitIdentifier(Identifier id)
            {
                outer.Use(id);
                return true;
            }

            public bool VisitBinaryExpression(BinaryExpression binExp)
            {
                bool dead = binExp.Left.Accept(this);
                dead &= binExp.Right.Accept(this);
                return dead;
            }

            public bool VisitAddress(Address addr)
            {
                return true;
            }

            public bool VisitApplication(Application appl)
            {
                foreach (var arg in appl.Arguments)
                {
                    arg.Accept(this);
                }
                return false;
            }

            public bool VisitArrayAccess(ArrayAccess acc)
            {
                throw new NotImplementedException();
            }

            public bool VisitCast(Cast cast)
            {
                return cast.Expression.Accept(this);
            }

            public bool VisitConditionOf(ConditionOf cof)
            {
                return cof.Expression.Accept(this);
            }

            public bool VisitConstant(Constant c)
            {
                return true;
            }

            public bool VisitDepositBits(DepositBits d)
            {
                var dead = d.InsertedBits.Accept(this);
                dead &= d.Source.Accept(this);
                return dead;
            }

            public bool VisitDereference(Dereference deref)
            {
                throw new NotImplementedException();
            }

            public bool VisitFieldAccess(FieldAccess acc)
            {
                throw new NotImplementedException();
            }

            public bool VisitMemberPointerSelector(MemberPointerSelector mps)
            {
                throw new NotImplementedException();
            }

            public bool VisitMemoryAccess(MemoryAccess access)
            {
                access.EffectiveAddress.Accept(this);
                return false;
            }

            public bool VisitMkSequence(MkSequence seq)
            {
                var dead = seq.Head.Accept(this);
                dead &= seq.Tail.Accept(this);
                return dead;
            }

            public bool VisitOutArgument(OutArgument outArg)
            {
                return false;
            }

            public bool VisitPhiFunction(PhiFunction phi)
            {
                throw new NotImplementedException();
            }

            public bool VisitPointerAddition(PointerAddition pa)
            {
                throw new NotImplementedException();
            }

            public bool VisitProcedureConstant(ProcedureConstant pc)
            {
                return false;
            }

            public bool VisitScopeResolution(ScopeResolution scopeResolution)
            {
                throw new NotImplementedException();
            }

            public bool VisitSegmentedAccess(SegmentedAccess access)
            {
                access.BasePointer.Accept(this);
                access.EffectiveAddress.Accept(this);
                return false;
            }

            public bool VisitSlice(Slice slice)
            {
                throw new NotImplementedException();
            }

            public bool VisitTestCondition(TestCondition tc)
            {
                return tc.Expression.Accept(this);
            }

            public bool VisitUnaryExpression(UnaryExpression unary)
            {
                return unary.Expression.Accept(this);
            }
        }

        private void Define(Identifier id)
        {
            id.Storage.Accept(this, true);
        }

        private void Use(Identifier id)
        {
            id.Storage.Accept(this, false);
        }

        private bool IsDead(Identifier id)
        {
            var reg = id.Storage as RegisterStorage;
            if (reg != null)
            {
                //Debug.Print("deadReg: {0}, F:{1}", reg, deadRegs.Contains(reg));
                return deadRegs.Contains(reg);
            }
            var flags = id.Storage as FlagGroupStorage;
            if (flags != null)
            {
                //Debug.Print("deadFlags: {0}, F:{1}", deadFlags, flags.FlagGroupBits);
                return (flags.FlagGroupBits & deadFlags) == flags.FlagGroupBits;
            }
            if (id.Storage is FpuStackStorage || id.Storage is StackLocalStorage || id.Storage is SequenceStorage)
                return false;
            if (id.Storage is MemoryStorage)
                return false;
            throw new NotImplementedException(id.Storage.GetType().Name);
        }

        public bool VisitAssignment(Assignment ass)
        {
            bool isDead = IsDead(ass.Dst);
            Define(ass.Dst);
            isDead &= ass.Src.Accept(expVisitor);
            return isDead;
        }

        public bool VisitBranch(Branch branch)
        {
            branch.Condition.Accept(expVisitor);
            return false;
        }

        public bool VisitCallInstruction(CallInstruction ci)
        {
            ci.Callee.Accept(expVisitor);
            deadRegs.Clear();
            deadFlags = 0;
            return false;
        }

        public bool VisitDeclaration(Declaration decl)
        {
            throw new NotImplementedException();
        }

        public bool VisitDefInstruction(DefInstruction def)
        {
            throw new NotImplementedException();
        }

        public bool VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            throw new NotImplementedException();
        }

        public bool VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotImplementedException();
        }

        public bool VisitReturnInstruction(ReturnInstruction ret)
        {
            if (ret.Expression != null)
                ret.Expression.Accept(expVisitor);
            return false;
        }

        public bool VisitSideEffect(SideEffect side)
        {
            side.Expression.Accept(expVisitor);
            return false;
        }

        public bool VisitStore(Store store)
        {
            store.Dst.Accept(expVisitor);
            store.Src.Accept(expVisitor);
            return false;
        }

        public bool VisitSwitchInstruction(SwitchInstruction si)
        {
            si.Expression.Accept(expVisitor);
            return false;
        }

        public bool VisitUseInstruction(UseInstruction use)
        {
            throw new NotImplementedException();
        }

        public bool VisitFlagGroupStorage(FlagGroupStorage grf, bool defining)
        {
            if (defining)
            {
                deadFlags |= grf.FlagGroupBits;
            }
            else
            {
                deadFlags &= ~grf.FlagGroupBits;
            }
            return true;
        }

        public bool VisitFpuStackStorage(FpuStackStorage fpu, bool defining)
        {
            return true;
        }

        public bool VisitMemoryStorage(MemoryStorage global, bool defining)
        {
            throw new NotImplementedException();
        }

        public bool VisitStackLocalStorage(StackLocalStorage local, bool defining)
        {
            return true;
        }

        public bool VisitOutArgumentStorage(OutArgumentStorage arg, bool defining)
        {
            throw new NotImplementedException();
        }

        public bool VisitRegisterStorage(RegisterStorage reg, bool defining)
        {
            if (defining)
            {
                deadRegs.Add(reg);
            }
            else
            {
                deadRegs.Remove(reg);
            }
            return true;
        }

        public bool VisitSequenceStorage(SequenceStorage seq, bool defining)
        {
            seq.Head.Storage.Accept(this, defining);
            seq.Tail.Storage.Accept(this, defining);
            return true;
        }

        public bool VisitStackArgumentStorage(StackArgumentStorage stack, bool defining)
        {
            throw new NotImplementedException();
        }

        public bool VisitTemporaryStorage(TemporaryStorage temp, bool defining)
        {
            throw new NotImplementedException();
        }
    }
}
