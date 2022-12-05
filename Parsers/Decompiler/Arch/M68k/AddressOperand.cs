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
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public class M68kAddressOperand : Core.Machine.AddressOperand, M68kOperand
    {
        public M68kAddressOperand(Address addr) : base(addr, PrimitiveType.Pointer32)
        {
        }

        public M68kAddressOperand(uint addr)
            : this(Address.Ptr32(addr))
        { 
        }

        public T Accept<T>(M68kOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(bool fExplicit, Core.Machine.MachineInstructionWriter writer)
        {
            writer.WriteAddress(string.Format("${0:X8}", Address.Offset), Address);
        }
    }
}
