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
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Mos6502
{
    public class Operand : MachineOperand
    {
        public AddressMode Mode;
        public RegisterStorage Register;
        public Constant Offset;

        public Operand(PrimitiveType size)
            : base(size)
        {
        }

        public override void Write(bool fExplicit, MachineInstructionWriter writer)
        {
            int o = Offset != null ? Offset.ToUInt16() : 0;
            string fmt;
            switch (Mode)
            {
            case AddressMode.Accumulator: return;        // Implicit, never displayed
            case AddressMode.Immediate: fmt = "#${0:X2}"; break;
            case AddressMode.ZeroPage: fmt = "${0:X2}"; break;
            case AddressMode.ZeroPageX: 
            case AddressMode.ZeroPageY: fmt = "${0:X2},{1}"; break;
            case AddressMode.Relative:
            case AddressMode.Absolute: fmt = "${0:X4}"; break;
            case AddressMode.AbsoluteX:
            case AddressMode.AbsoluteY: fmt = "${0:X4},{1}"; break;
            case AddressMode.Indirect: fmt = "(${0:X4})"; break;
            case AddressMode.IndexedIndirect: fmt = "(${0:X2},{1})"; break;
            case AddressMode.IndirectIndexed: fmt = "(%{0:X2}),{1}"; break;
            default: throw new NotSupportedException();
            }
            writer.Write(string.Format(fmt, o, Register));
        }
    }

    public enum AddressMode
    {
        Immediate,          // #$AA
        ZeroPage,           // $AA
        ZeroPageX,          // $AA,x
        ZeroPageY,          // $AA,y
        Relative,           // $AABB
        Absolute,           // $AABB
        AbsoluteX,          // $AABB,x
        AbsoluteY,          // $AABB,y
        Indirect,           // ($AABB)
        IndexedIndirect,    // $(AA,x)
        IndirectIndexed,    // $(AA),y
        Accumulator,        // a
    }
}
