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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.IO;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class StorageTests
	{
		[Test]
		public void CreateOutArgumentRegister()
		{
			RegisterStorage mr = new RegisterStorage("r1", 1, PrimitiveType.Word32);
			Identifier oarg = new Identifier("r1Out", PrimitiveType.Word32, new OutArgumentStorage(
				new Identifier(mr.Name, PrimitiveType.Word32, mr)));
			StringWriter w = new StringWriter();
			oarg.Write(true, w);
			Assert.AreEqual("Register out word32 r1Out", w.ToString());
		}
	}

	[TestFixture]
	public class StackArgumentTests
	{
		private Frame f;
		private Identifier argOff;
		private Identifier argSeg;
		private Identifier arg_alias;
        private IntelArchitecture arch;

		[SetUp]
		public void Setup()
		{
            arch = new IntelArchitecture(ProcessorMode.Real);
			f = new Frame(PrimitiveType.Word16);
			argOff = f.EnsureStackArgument(4, PrimitiveType.Word16);
			argSeg = f.EnsureStackArgument(6, PrimitiveType.SegmentSelector);
			arg_alias = f.EnsureStackArgument(4, PrimitiveType.Pointer32);
		}

		[Test]
		public void OffsetOfStackArgumentOffset()
		{
//			Assert.AreNotSame(argOff, arg_alias);
			Assert.AreEqual(0, arg_alias.Storage.OffsetOf(argOff.Storage));
		}

		[Test]
		public void OffsetOfStackArgumentSegment()
		{
//			Assert.AreNotSame(argOff, arg_alias);
			Assert.AreEqual(16, arg_alias.Storage.OffsetOf(argSeg.Storage));
		}


	}

	[TestFixture]
	public class StackLocalTests
	{
		private Frame f;
		private StackLocalStorage varOff;
		private StackLocalStorage varSeg;
		private StackLocalStorage varPointer;

		[SetUp]
		public void Setup()
		{
			f = new Frame(PrimitiveType.Word16);
			varOff = (StackLocalStorage) f.EnsureStackLocal(-4, PrimitiveType.Word16).Storage;
			varSeg = (StackLocalStorage) f.EnsureStackLocal(-2, PrimitiveType.SegmentSelector).Storage;
			varPointer = (StackLocalStorage) f.EnsureStackLocal(-4, PrimitiveType.Pointer32).Storage;
		}

		[Test]
		public void OffsetOfLocalVariableOffset()
		{
//			Assert.AreNotSame(varOff, varPointer);
			Assert.AreEqual(0, varPointer.OffsetOf(varOff));
		}

	}
}
