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

using Decompiler.Core.Types;
using Decompiler.Typing;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Typing
{
    [TestFixture]
    public class NestedComplexTypeExtractorTests
    {
        private TypeStore store;
        private TypeFactory factory;
        private NestedComplexTypeExtractor nct;

        [SetUp]
        public void Setup()
        {
            store = new TypeStore();
            factory = new TypeFactory();
            nct = new NestedComplexTypeExtractor(factory, store);

        }

        [Test]
        public void ArrayOfStructures()
        {
            StructureType s = new StructureType();
            s.Fields.Add(0, PrimitiveType.Word32);
            s.Fields.Add(4, PrimitiveType.Real64);

            ArrayType a = new ArrayType(s, 0);

            TypeVariable tv = store.CreateTypeVariable(factory);
            tv.Class.DataType = a;
            Assert.AreEqual(1, store.UsedEquivalenceClasses.Count);

            DataType dt = tv.Class.DataType.Accept(nct);

            Assert.AreEqual(2, store.UsedEquivalenceClasses.Count);
            Assert.AreEqual("(arr Eq_2)", store.UsedEquivalenceClasses[0].DataType.ToString());
            Assert.AreEqual("(struct (0 word32 dw0000) (4 real64 r0004))", store.UsedEquivalenceClasses[1].DataType.ToString());
        }

        [Test]
        public void StructureContainingArray()
        {
            ArrayType a = new ArrayType(PrimitiveType.Int32, 4);

            StructureType s = new StructureType(null, 0) { Fields = { { 8, a } } };

            TypeVariable tv = store.CreateTypeVariable(factory);
            tv.Class.DataType = s;
            Assert.AreEqual(1, store.UsedEquivalenceClasses.Count);

            DataType dt = tv.Class.DataType.Accept(nct);
            Assert.AreEqual(1, store.UsedEquivalenceClasses.Count);
            Assert.AreEqual("(struct (8 (arr int32 4) a0008))", store.UsedEquivalenceClasses[0].DataType.ToString()); 
        }

    }
}
