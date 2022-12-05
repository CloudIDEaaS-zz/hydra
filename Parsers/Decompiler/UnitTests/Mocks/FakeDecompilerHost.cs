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
using Decompiler.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class FakeDecompilerHost : DecompilerHost
    {
        private StringWriter disassembly = new StringWriter();
        private StringWriter intermediate = new StringWriter();
        private StringWriter decompiled = new StringWriter();
        private StringWriter typesWriter = new StringWriter();
        private StringWriter globalsWriter = new StringWriter();
        private IConfigurationService config = new FakeDecompilerConfiguration();

        public TextWriter CreateDecompiledCodeWriter(string file)
        {
            return decompiled;
        }

        IConfigurationService DecompilerHost.Configuration
        {
            get { return config; }
        }

        public void WriteDisassembly(Program program, Action<TextWriter> writer)
        {
            writer(disassembly);
        }

        public void WriteIntermediateCode(Program program, Action<TextWriter> writer)
        {
            writer(intermediate);
        }

        public void WriteTypes(Program program, Action<TextWriter> writer)
        {
            writer(typesWriter);
        }

        public void WriteDecompiledCode(Program program, Action<TextWriter> writer)
        {
            writer(decompiled);
        }

        public void WriteGlobals(Program program, Action<TextWriter> writer)
        {
            writer(globalsWriter);
        }

        // probing methods.

        public StringWriter DisassemblyWriter
        {
            get { return disassembly; }
        }

        public StringWriter TypesWriter
        {
            get { return typesWriter; }
        }

        public StringWriter GlobalsWriter
        {
            get { return globalsWriter; }
        }
    }
}
