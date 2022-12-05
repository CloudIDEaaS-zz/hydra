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
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Mos6502
{
    public class Mos6502ProcessorState : ProcessorState
    {
        private Mos6502ProcessorArchitecture arch;
        private byte[] regs;
        private bool[] valid;
        private Address ip;

        public Mos6502ProcessorState(Mos6502ProcessorArchitecture arch)
        {
            this.arch = arch;
            this.regs = new byte[4];
            this.valid = new bool[4];
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override ProcessorState Clone()
        {
            return new Mos6502ProcessorState(arch)
            {
                regs = (byte[])regs.Clone(),
                valid = (bool[])valid.Clone(),
                ip = ip
            };
        }

        public override Constant GetRegister(RegisterStorage r)
        {
            if (valid[r.Number])
                return Constant.Byte(regs[r.Number]);
            else
                return Constant.Invalid;
        }

        public override void SetRegister(RegisterStorage r, Core.Expressions.Constant v)
        {
            if (v != null && v.IsValid)
            {
                valid[r.Number] = true;
                regs[r.Number] = v.ToByte();
            }
            else
            {
                valid[r.Number] = false;
            }
        }

        public override void SetInstructionPointer(Address addr)
        {
            this.ip = addr;
        }

        public override void OnProcedureEntered()
        {
        }

        public override void OnProcedureLeft(ProcedureSignature procedureSignature)
        {
        }

        public override CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize)
        {
            throw new NotImplementedException();
        }

        public override void OnAfterCall(Identifier stackReg, ProcedureSignature sigCallee, ExpressionVisitor<Core.Expressions.Expression> eval)
        {
            throw new NotImplementedException();
        }
    }
}