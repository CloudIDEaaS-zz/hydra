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

using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.TestCode
{
    public class ManyIncrements : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            Identifier r0 = Register(0);
            Identifier r1 = Register(1);

            Label("loopTop");
            Assign(r1, Load(PrimitiveType.Byte, r0));
            Assign(r0, IAdd(r0, 1));
            BranchIf(Ne(r1, base.Int8(1)), "not1");

            Assign(r1, Load(PrimitiveType.Byte, r0));
            Assign(r0, IAdd(r0, 1));
            Store(Int32(0x33333330), r1);
            Assign(r1, Load(PrimitiveType.Byte, r0));
            Assign(r0, IAdd(r0, 1));
            Store(Int32(0x33333331), r1);
            Jump("loopTop");

            Label("not1");
            BranchIf(Ne(r1, base.Int8(2)), "done");
            Assign(r1, Load(PrimitiveType.Byte, r0));
            Assign(r0, IAdd(r0, 1));
            Store(Int32(0x33333330), r1);
            Jump("loopTop");

            Label("done");
            Return();
        }
    }
}
