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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decompiler.Core;
using Decompiler.Core.Types;
using Decompiler.Core.Expressions;
using Decompiler.UnitTests.Mocks;

namespace Decompiler.UnitTests.Fragments
{
    public class FramePointerFragment : ProcedureBuilder
    {
        private TypeFactory factory;

        public FramePointerFragment(TypeFactory factory)
        {
            this.factory = factory;
        }

        protected override void BuildBody()
        {
            Identifier frame = Declare(new StructureType("frame_t", 0), "frame");
            Identifier fp = Local32("fp");
            Assign(fp, AddrOf(frame));
            Store(IAdd(fp, 4), Load(PrimitiveType.Word32, IAdd(fp, 8)));
        }
    }
}