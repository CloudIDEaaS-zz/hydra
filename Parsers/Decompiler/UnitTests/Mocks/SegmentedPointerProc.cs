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
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class SegmentedPointerProc : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            var m = this;
            Identifier ds = m.Frame.EnsureRegister(new RegisterStorage("ds", 1, PrimitiveType.SegmentSelector));
            Identifier es = m.Frame.EnsureRegister(new RegisterStorage("es", 2, PrimitiveType.SegmentSelector));
            Identifier bx = m.Frame.EnsureRegister(new RegisterStorage("bx", 3, PrimitiveType.Word16));
            Identifier es_bx = m.Frame.EnsureSequence(es, bx, PrimitiveType.SegPtr32);
            m.SegStore(ds, m.Word16(0x300), m.Word16(0x1234));
            m.SegStore(ds, m.Word16(0x302), m.Word16(0x5550));
            m.Assign(es_bx, m.SegMem(PrimitiveType.SegPtr32, ds, m.Word16(0x300)));
            m.SegStore(ds, m.Word16(0x304), m.SegMem(
                PrimitiveType.Word16,
                m.Slice(PrimitiveType.SegmentSelector, es_bx, 16),
                m.IAdd(m.Slice(PrimitiveType.Word16, es_bx, 0), 4)));
        }
    }
}
