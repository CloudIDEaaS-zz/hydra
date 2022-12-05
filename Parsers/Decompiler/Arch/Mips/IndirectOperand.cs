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
using Decompiler.Core.Lib;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Decompiler.Arch.Mips
{
    public class IndirectOperand : MachineOperand
    {
        public int Offset;
        public RegisterStorage Base;

        public IndirectOperand(PrimitiveType dataWidth, int offset, RegisterStorage baseReg) : base(dataWidth)
        {
            this.Offset = offset;
            this.Base = baseReg;
        }

        public override void Write(bool fExplicit, MachineInstructionWriter writer)
        {
            string fmt;
            int offset;
            if (Offset >= 0)
            {
                fmt = "{0:X4}({1})";
                offset = Offset;
            }
            else 
            {
                fmt = "-{0:X4}({1})";
                offset = -Offset;
            }
            writer.Write(string.Format(fmt, offset, Base));
        }
    }
}
