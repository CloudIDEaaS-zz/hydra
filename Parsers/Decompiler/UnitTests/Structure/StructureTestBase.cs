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

using Decompiler;
using Decompiler.Analysis;
using Decompiler.Assemblers.x86;
using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Core.Services;
using Decompiler.Loading;
using Decompiler.Scanning;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Decompiler.Environments.Msdos;
using Decompiler.Core.Configuration;

namespace Decompiler.UnitTests.Structure
{
	public class StructureTestBase
	{
		protected Program program;

        protected StructureNode GetNode(ProcedureStructure proc, string nodeName)
        {
            return proc.Nodes.Find(node => node.Name == nodeName);
        }

		protected Program RewriteProgramMsdos(string sourceFilename, Address addrBase)
		{
            var sc = new ServiceContainer();
            sc.AddService<IConfigurationService>(new FakeDecompilerConfiguration());
            var ldr = new Loader(sc);
            var arch = new X86ArchitectureReal();

            program = ldr.AssembleExecutable(
                FileUnitTester.MapTestPath(sourceFilename),
                new X86TextAssembler(arch) { Platform = new MsdosPlatform(null, arch) },
                addrBase);
            return RewriteProgram();
		}

        protected Program RewriteProgram32(string sourceFilename, Address addrBase)
        {
            var sc = new ServiceContainer();
            sc.AddService<IConfigurationService>(new FakeDecompilerConfiguration());
            var ldr = new Loader(sc);
            var arch = new X86ArchitectureFlat32();
            program = ldr.AssembleExecutable(
                FileUnitTester.MapTestPath(sourceFilename),
                new X86TextAssembler(arch) { Platform = new DefaultPlatform(null, arch) },
                addrBase);
            return RewriteProgram();
        }

        protected Program RewriteX86RealFragment(string asmFragment, Address addrBase)
        {
            var asm = new X86TextAssembler(new X86ArchitectureReal());
            program = asm.AssembleFragment(addrBase, asmFragment);
            program.Platform = new DefaultPlatform(null, program.Architecture);
            program.EntryPoints.Add(new EntryPoint(addrBase, program.Architecture.CreateProcessorState()));
            return RewriteProgram();
        }


        protected Program RewriteX86_32Fragment(string asmFragment, Address addrBase)
        {
            var asm = new X86TextAssembler(new X86ArchitectureFlat32());
            program = asm.AssembleFragment(addrBase, asmFragment);
            program.Platform = new DefaultPlatform(null, program.Architecture);
            program.EntryPoints.Add(new EntryPoint(addrBase, program.Architecture.CreateProcessorState()));
            return RewriteProgram();
        }

        private Program RewriteProgram()
        {
            var project = new Project { Programs = { program } };
            var scan = new Scanner(
                program,
                new Dictionary<Address, ProcedureSignature>(),
                new ImportResolver(project),
                new FakeDecompilerEventListener());
            foreach (EntryPoint ep in program.EntryPoints)
            {
                scan.EnqueueEntryPoint(ep);
            }
            scan.ScanImage();

            DecompilerEventListener eventListener = new FakeDecompilerEventListener();
            DataFlowAnalysis da = new DataFlowAnalysis(program, eventListener);
            da.AnalyzeProgram();

            return program;
        }
	}
}
