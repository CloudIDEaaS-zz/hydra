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
using Decompiler.Core.Rtl;
using Decompiler.Core.Expressions;
using System;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Mocks
{
    public class RtlStatementStream : ExpressionEmitter
    {
        private Frame frame;
        private List<RtlInstructionCluster> stms;
        private IProcessorArchitecture arch;
        private uint linAddress;

        public RtlStatementStream(uint address, Frame frame)
        {
            this.linAddress = address;
            this.arch = new FakeArchitecture();
            this.frame = frame;
            this.stms = new List<RtlInstructionCluster>();   
        }

        public RtlInstruction Emit(RtlInstruction instr)
        {
            stms.Add(new RtlInstructionCluster(Address.Ptr32(linAddress), 4, instr));
            linAddress += 4;
            return instr;
        }

        public RtlInstruction Assign(Expression dst, int n)
        {
             return Assign(dst, Constant.Create(dst.DataType, n));
        }

        public RtlInstruction Assign(Expression dst, Expression src)
        {
            var ass = new RtlAssignment(dst, src);
            return Emit(ass);
        }

        public RtlInstruction Branch(Expression cond, Address target, RtlClass rtlClass)
        {
            var br = new RtlBranch(cond, target, rtlClass);
            return Emit(br);
        }

        public RtlInstruction Call(Expression target)
        {
            var call = new RtlCall(target, 4, RtlClass.Transfer);
            return Emit(call);
        }

        // Delayed call (for SPARC / MIPS)
        public RtlInstruction CallD(Expression target)
        {
            var call = new RtlCall(target, 4, RtlClass.Transfer|RtlClass.Delay);
            return Emit(call);
        }

        public IEnumerator<RtlInstructionCluster> GetRewrittenInstructions()
        {
            foreach (var x in stms)
                yield return x;
        }

        public RtlInstruction Goto(uint target)
        {
            var g = new RtlGoto(Address.Ptr32(target), RtlClass.Transfer);
            return Emit(g);
        }

        public Identifier Register(int iReg)
        {
            return frame.EnsureRegister(arch.GetRegister(iReg));
        }

        public RtlInstruction Return()
        {
            var ret = new RtlReturn(0, 0, RtlClass.Transfer);
            return Emit(ret);
        }

        public RtlInstruction ReturnD()
        {
            var ret = new RtlReturn(0, 0, RtlClass.Transfer|RtlClass.Delay);
            return Emit(ret);
        }

        public RtlInstruction SideEffect(Expression exp)
        {
            var side = new RtlSideEffect(exp);
            return Emit(side);
        }
    }
}
