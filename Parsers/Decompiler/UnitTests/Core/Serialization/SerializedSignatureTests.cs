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
using Decompiler.Core.Expressions;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using Decompiler.Arch.X86;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.UnitTests.Core.Serialization
{
	[TestFixture]
	public class SerializedSignatureTests
	{
		private IntelArchitecture arch;
        private X86ProcedureSerializer sser;

		public SerializedSignatureTests()
		{
			this.arch = new IntelArchitecture(ProcessorMode.Real);
		}

        private void Given_X86ProcedureSerializer()
        {
            sser = new X86ProcedureSerializer(arch, new TypeLibraryLoader(arch, true), "stdapi");
        }

		[Test]
		public void SsigCreate()
		{
			SerializedSignature ssig = BuildSsigAxBxCl();
			Verify(ssig, "Core/SsigCreate.txt");
		}

		[Test]
		public void SsigReadAxBxCl()
		{
			SerializedSignature ssig;
			using (FileStream stm = new FileStream(FileUnitTester.MapTestPath("Core/AxBxCl.xml"), FileMode.Open))
			{
				XmlTextReader rdr = new XmlTextReader(stm);
                XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(typeof(SerializedSignature));
				ssig = (SerializedSignature) ser.Deserialize(rdr);
			}
            Given_X86ProcedureSerializer();
			Assert.AreEqual("Register word16 AxBxCl(Register word16 bx, Register byte cl)", sser.Deserialize(ssig, arch.CreateFrame()).ToString("AxBxCl"));
		}

		[Test]
		public void SsigBuildSig()
		{
			SerializedSignature ssig = BuildSsigAxBxCl();
            Given_X86ProcedureSerializer();
            ProcedureSignature sig = sser.Deserialize(ssig, arch.CreateFrame());
			Assert.AreEqual("Register int16 AxBxCl(Register word16 bx, Register byte cl)", sig.ToString("AxBxCl"));
			Assert.AreEqual(PrimitiveType.Int16, sig.ReturnValue.DataType);
		}

		[Test]
		public void SsigWriteStdapi()
		{
			SerializedSignature ssig = BuildSsigStack();
			ssig.Convention = "stdapi";
			Verify(ssig, "Core/SsigWriteStdapi.txt");
		}

		[Test]
		public void SsigDeserializeOutArgument()
		{
            Argument_v1 arg = new Argument_v1
            {
                Kind = new Register_v1("bp"),
                OutParameter = true
            };
			SerializedSignature sig = new SerializedSignature();
			sig.Arguments = new Argument_v1[] { arg };
            Given_X86ProcedureSerializer();
            ProcedureSignature ps = sser.Deserialize(sig, arch.CreateFrame());
			Assert.AreEqual("void foo(Register out ptr16 bpOut)", ps.ToString("foo"));
		}

		public static SerializedSignature BuildSsigAxBxCl()
		{
			SerializedSignature ssig = new SerializedSignature();
			
			Argument_v1 sarg = new Argument_v1();
			sarg.Type = new PrimitiveType_v1(Domain.SignedInt, 2);
			sarg.Kind = new Register_v1("ax");
			ssig.ReturnValue = sarg;

            ssig.Arguments = new Argument_v1[]
            {
                new Argument_v1 { Kind = new Register_v1("bx") },
                new Argument_v1 { Kind = new Register_v1("cl") },
            };
			return ssig;
		}

		public static ProcedureSignature MkSigAxBxCl()
		{
			Identifier ret = new Identifier(Registers.ax.Name, Registers.ax.DataType, Registers.ax);
			Identifier [] args = new Identifier[2];
			args[0] = new Identifier(Registers.bx.Name, Registers.bx.DataType, Registers.bx);
			args[1] = new Identifier(Registers.cl.Name, Registers.cl.DataType, Registers.cl);
			return new ProcedureSignature(ret, args);
		}

		private SerializedSignature BuildSsigStack()
		{
            SerializedSignature ssig = new SerializedSignature()
            {
                ReturnValue = new Argument_v1
                {
                    Type = new SerializedTypeReference("int"),
                    Kind = new Register_v1("ax")
                },
                Arguments = new Argument_v1[]
                {
                    new Argument_v1
                    {
                        Name = "lParam",
			            Kind = new StackVariable_v1(),
                        Type = new PrimitiveType_v1(Domain.SignedInt, 4),
                    },
                    new Argument_v1 
                    {
			            Kind = new StackVariable_v1(),
                        Type = new PrimitiveType_v1(Domain.SignedInt, 2),
                    }
                }
            };
			return ssig;
		}

		private void Verify(SerializedSignature ssig, string outputFilename)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFilename))
			{
				XmlTextWriter x = new FilteringXmlWriter(fut.TextWriter);
				x.Formatting = Formatting.Indented;
				XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(ssig.GetType());
				ser.Serialize(x, ssig);

				fut.AssertFilesEqual();
			}
		}
	}
}
