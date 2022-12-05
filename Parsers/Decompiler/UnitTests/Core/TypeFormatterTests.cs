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

using Decompiler.Core;
using Decompiler.Core.Output;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.IO;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class TypeFormatterTests
	{
		private StringWriter sw;
        private TypeFormatter tyfo;
        private TypeReferenceFormatter tyreffo;
		private string nl = Environment.NewLine;

        [SetUp]
        public void SetUp()
        {
            sw = new StringWriter();
            tyfo = new TypeFormatter(new TextFormatter(sw), false);
            tyreffo = new TypeReferenceFormatter(new TextFormatter(sw), false);
        }
        
        [Test]
		public void TyfoInt()
		{
			DataType dt = PrimitiveType.Int32;
			tyreffo.WriteDeclaration(dt, "test");
			Assert.AreEqual("int32 test", sw.ToString());
		}

		[Test]
		public void TyfoBareInt()
		{
			DataType dt = PrimitiveType.Int32;
			tyreffo.WriteTypeReference(dt);
			Assert.AreEqual("int32", sw.ToString());
		}

		[Test]
		public void TyfoPtrReal()
		{
			DataType dt = new Pointer(PrimitiveType.Real32, 4);
			tyreffo.WriteDeclaration(dt, "test");
			Assert.AreEqual("real32* test", sw.ToString());
		}

		[Test]
		public void TyfoBarePtrReal()
		{
			DataType dt = new Pointer(PrimitiveType.Real32, 4);
            tyreffo.WriteTypeReference(dt);
			Assert.AreEqual("real32*", sw.ToString());
		}

		[Test]
		public void TyfoMem()
		{
			StructureType m = new StructureType( "foo", 0);
			m.Fields.Add(4, PrimitiveType.UInt16);
			m.Fields.Add(8, new Pointer(PrimitiveType.UInt32, 4));
			tyfo.Write(m, "bar");
			Assert.AreEqual(
@"struct foo {
	uint16 w0004;	// 4
	uint32 * ptr0008;	// 8
} bar",
				sw.ToString());
		}

		[Test]
		public void TyfoUnion()
		{
			UnionType u = new UnionType( "foo", null);
			u.Alternatives.Add(PrimitiveType.Real32);
			u.Alternatives.Add(PrimitiveType.Int32);
			tyfo.Write(u, "bar");
			Assert.AreEqual(
@"union foo {
	int32 u0;
	real32 u1;
} bar", 
				sw.ToString());
		}

		[Test]
		public void TyfoSelfref()
		{
			StructureType s = new StructureType("link", 0);
			s.Fields.Add(0, PrimitiveType.Int32, "data");
			s.Fields.Add(4, new Pointer(s, 4), "next");
			tyfo.Write(s, "list");
			Assert.AreEqual(
@"struct link {
	int32 data;	// 0
	struct link * next;	// 4
} list",
				sw.ToString());
		}

		[Test]
		public void TyfoCycle()
		{
			StructureType a = new StructureType("a", 0);
			StructureType b = new StructureType("b", 0);
			a.Fields.Add(0, new Pointer(b, 4), "pb");
			b.Fields.Add(0, new Pointer(a, 4), "pa");
			tyfo.Write(a, null);
			Assert.AreEqual(
@"struct b;
struct a {
	struct b * pb;	// 0
}",
			sw.ToString());
		}

		[Test]
		public void TyfoFn()
		{
			FunctionType fn = new FunctionType(null, PrimitiveType.Int32, 
				new DataType[] { PrimitiveType.Word32 }, null);
			tyreffo.WriteDeclaration(fn, "fn");
			Assert.AreEqual("int32 fn(word32)", 
				sw.ToString());
		}

		[Test]
		public void TyfoPfn()
		{
			FunctionType fn = new FunctionType(null, null, 
				new DataType[] { PrimitiveType.Word32 }, null);
			Pointer pfn = new Pointer(fn, 4);
			tyreffo.WriteDeclaration(pfn, "pfn");
			Assert.AreEqual("void (* pfn)(word32)", 
				sw.ToString());
		}

		[Test]
		public void TyfoMembptr()
		{
            var s = new StructureType("s", 0);
			MemberPointer mp = new MemberPointer(new Pointer(s, 4), PrimitiveType.Int32, 2);
			tyreffo.WriteDeclaration(mp, "mp");
			Assert.AreEqual("int32 s::* mp", sw.ToString());
		}

		[Test]
		public void TyfoManyArgs()
		{
			FunctionType fn = new FunctionType(null, null, 
				new DataType[] { PrimitiveType.Pointer32, PrimitiveType.Int64 }, null);
			tyreffo.WriteDeclaration(fn, "fn");
			Assert.AreEqual("void fn(ptr32, int64)", sw.ToString());
		}

        [Test]
        public void TypeReference()
        {
            tyreffo = new TypeReferenceFormatter(new TextFormatter(sw), true);
            EquivalenceClass b = new EquivalenceClass(new TypeVariable(1));
            b.DataType = new StructureType("b", 0) { Fields = { { 4, PrimitiveType.Word32 } } };

            tyfo.Write(new Pointer(b, 2), "pb");
            Assert.AreEqual("b * pb", sw.ToString());
        }

        [Test]
        public void TyfoMemberPointerMembers()
        {
            StructureType seg = new StructureType("seg", 0);

            StructureType s = new StructureType("s", 0);
            s.Fields.Add(
                42, new MemberPointer(seg, PrimitiveType.Word32, 2));
            tyfo.Write(s, "meeble");
            string sExp = 
@"struct s {
	word32 seg::*ptr002A;	// 2A
} meeble";
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        [Ignore("This test isn't working presently; focus on passing more important tests first then fix")]
        public void TyfoMemberPointerCycle()
        {
            var seg = new StructureType("seg", 100);

            var a = new StructureType("a", 0);
            var b = new StructureType("b", 0);

            a.Fields.Add(0, new MemberPointer(seg, b, 2));
            b.Fields.Add(0, new MemberPointer(seg, a, 2));

            tyfo.WriteTypes(new DataType[] { a, b });

            string sExp =
                "struct b;" + nl +
                "struct a {" + nl +
                "\tstruct b seg::*ptr0000;\t// 0" + nl +
                "};" + nl +
                nl +
                "struct b {" + nl +
                "\tstruct a seg::*ptr0000;\t// 0" + nl +
                "};" + nl;
            
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void TyfoArray()
        {
            ArrayType arr = new ArrayType(PrimitiveType.Int32, 10);
            tyreffo.WriteDeclaration(arr, "a");

            string sExp = "int32 a[10]";
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void TyfoArrayArray()
        {
            ArrayType arr = new ArrayType(PrimitiveType.Int32, 10);
            ArrayType arr2 = new ArrayType(arr, 4);
            tyreffo.WriteDeclaration(arr2, "a");

            string sExp = "int32 a[4][10]";
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void TyfoPtrPtr()
        {
            var ptr = new Pointer(PrimitiveType.Int32, 4);
            var ptr2 = new Pointer(ptr, 4);
            tyreffo.WriteDeclaration(ptr2, "ppi");

            string sExp = "int32** ppi";
            Assert.AreEqual(sExp, sw.ToString());
        }
	}
}
