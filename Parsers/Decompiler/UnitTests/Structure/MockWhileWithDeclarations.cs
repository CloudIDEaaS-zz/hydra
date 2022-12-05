#region License
/* 
 * Copyright (C) 1999-2015 John K�ll�n.
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
using Decompiler.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Structure
{
    public class MockWhileWithDeclarations : ProcedureBuilder
    {
        protected override void BuildBody()
        {
            Identifier i = Local32("i");
            Label("loopHeader");
            Identifier v = Declare(PrimitiveType.Byte, "v", Load(PrimitiveType.Byte, i));
            Assign(i, IAdd(i, 1));
            BranchIf(Eq(v, 0x20), "exit_loop");

            Store(Word32(0x00300000), v);
            Jump("loopHeader");
            Label("exit_loop");
            Return();
        }
    }
}
