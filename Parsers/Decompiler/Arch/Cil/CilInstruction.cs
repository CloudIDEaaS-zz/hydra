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

using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Decompiler.Arch.Cil
{
    public class CilInstruction : MachineInstruction
    {
        private static Dictionary<OpCode, string> mpopcodetostring = new Dictionary<OpCode, string>
        {
            { OpCodes.Ldc_I4_0,  "ldc.i4.0"}
        };

        public OpCode Opcode { get; set; }
        public override int OpcodeAsInteger { get { return (int)Opcode.Value; } }

        public override void Render(MachineInstructionWriter writer)
        {
            try
            {
                writer.WriteOpcode(mpopcodetostring[Opcode]);
            }
            catch
            {
                throw new NotImplementedException("Lolwut: " + Opcode);
            }
        }

        public object Operand { get; set; }
    }
}
