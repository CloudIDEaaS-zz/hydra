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

using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Core.Rtl
{
    /// <summary>
    /// Builder class that accumulates RtlInstructions.
    /// </summary>
    public class RtlEmitter : ExpressionEmitter
    {
        private List<RtlInstruction> instrs;

        public RtlEmitter(List<RtlInstruction> instrs)
        {
            this.instrs = instrs;
        }

        public RtlEmitter Assign(Expression dst, Expression src)
        {
            var ass = new RtlAssignment(dst, src);
            instrs.Add(ass);
            return this;
        }

        public RtlEmitter Assign(Expression dst, int src)
        {
            var ass = new RtlAssignment(dst, Constant.Create(dst.DataType, src));
            instrs.Add(ass);
            return this;
        }

        public RtlEmitter Branch(Expression condition, Address target, RtlClass rtlClass)
        {
            instrs.Add(new RtlBranch(condition, target, rtlClass));
            return this;
        }

        /// <summary>
        /// Called when we need to generate an RtlBranch in the middle of an operation.
        /// Normally, branches are at the end of the Rtl's of a translated instruction,
        /// but in some cases, they are not.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="target"></param>
        /// <param name="?"></param>
        public void BranchInMiddleOfInstruction(Expression condition, Address target, RtlClass rtlClass)
        {
            var branch = new RtlBranch(condition, target, rtlClass);
            branch.NextStatementRequiresLabel = true;
            instrs.Add(branch);
        }

        /// <summary>
        /// Called to generate a RtlCall instruction. The <paramref name="retSize"/> is the
        /// size of the return value as placed on the stack. It will be 0 on machines
        /// which use link registers to store the return address at a function call.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="retSize"></param>
        /// <param name="rtlClass"></param>
        public RtlEmitter Call(Expression target, byte retSize)
        {
            instrs.Add(new RtlCall(target, retSize, RtlClass.Transfer));
            return this;
        }

        public RtlEmitter CallD(Expression target, byte retSize)
        {
            instrs.Add(new RtlCall(target, retSize, RtlClass.Transfer|RtlClass.Delay));
            return this;
        }

        /// <summary>
        /// Standard goto, for architectures where there are no delay slots.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public RtlEmitter Goto(Expression target)
        {
            instrs.Add(new RtlGoto(target, RtlClass.Transfer));
            return this;
        }

        /// <summary>
        /// Delayed goto (for RISC architectures with delay slots)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public RtlEmitter GotoD(Expression target)
        {
            instrs.Add(new RtlGoto(target, RtlClass.Transfer|RtlClass.Delay));
            return this;
        }

        /// <summary>
        /// Return to caller.
        /// </summary>
        /// <param name="returnAddressBytes">How large the return address is on the stack.</param>
        /// <param name="extraBytesPopped">Some architectures (like x86) pop the caller's extra
        /// bytes off the stack.</param>
        /// <returns></returns>
        public RtlEmitter Return(
            int returnAddressBytes,
            int extraBytesPopped)
        {
            instrs.Add(new RtlReturn(returnAddressBytes, extraBytesPopped, RtlClass.Transfer));
            return this;
        }

        public RtlEmitter ReturnD(
            int returnAddressBytes,
            int extraBytesPopped)
        {
            var ret = new RtlReturn(returnAddressBytes, extraBytesPopped, RtlClass.Transfer | RtlClass.Delay);
            instrs.Add(ret);
            return this;
        }

        public RtlEmitter SideEffect(Expression sideEffect)
        {
            instrs.Add(new RtlSideEffect(sideEffect));
            return this;
        }

        public Expression Const(DataType dataType, int p)
        {
            return Constant.Create(dataType, p);
        }

        public RtlEmitter If(Expression test, RtlInstruction rtl)
        {
            instrs.Add(new RtlIf(test, rtl));
            return this;
        }

    }
}
