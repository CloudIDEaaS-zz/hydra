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
using Decompiler.Core;
using Decompiler.Core.Configuration;
using Decompiler.Core.Services;
using Decompiler.Loading;
using Decompiler.Scanning;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Text;

namespace Decompiler.UnitTests.Loading
{
	[TestFixture]
	public class LoaderTests
	{
        private MockRepository mr;
        private IServiceContainer sc;
        private FakeDecompilerEventListener eventListener;
        private IConfigurationService cfgSvc;
        private List<SignatureFileElement> signatureFiles;
        private IProcessorArchitecture x86arch;
        private Platform msdosPlatform;
        private byte[] testImage;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            eventListener = new FakeDecompilerEventListener();
            cfgSvc = mr.Stub<IConfigurationService>();
            signatureFiles = new List<SignatureFileElement>();
            sc.AddService<DecompilerEventListener>(eventListener);
            sc.AddService<IConfigurationService>(cfgSvc);
            cfgSvc.Stub(d => d.GetSignatureFiles()).Return(signatureFiles);
        }

        [Test]
        public void Ldr_Match()
        {
            mr.ReplayAll();

            Loader ldr = new Loader(sc);
            Assert.IsTrue(ldr.ImageHasMagicNumber(new byte[] { 0x47, 0x11 }, "4711", "0"));

            mr.VerifyAll();
        }

        [Test(Description="Unless otherwise specified, fail loading unknown file formats.")]
        public void Ldr_UnknownImageType()
        {
            cfgSvc.Stub(d => d.GetImageLoaders()).Return(new ArrayList());
            cfgSvc.Stub(d => d.GetRawFile(null)).IgnoreArguments().Return(null);
            var testImage = new byte[] { 42, 42, 42, 42, };
            mr.ReplayAll();
            Loader ldr = mr.PartialMock<Loader>(sc);
            ldr.Replay();

            Program prog = ldr.LoadExecutable("", testImage, null);

            Assert.AreEqual("WarningDiagnostic -  - The format of the file is unknown." , eventListener.LastDiagnostic);
            Assert.AreEqual(0, prog.Image.BaseAddress.Offset);
            Assert.IsNull(prog.Architecture);
            Assert.IsAssignableFrom<DefaultPlatform>(prog.Platform);
            mr.VerifyAll();
        }

        [Test(Description = "Use default settings when loading unknown file formats.")]
        public void Ldr_UnknownImageType_DefaultSpecified()
        {
            Given_MsDosRawFileFormat();
            cfgSvc.Stub(d => d.GetImageLoaders()).Return(new ArrayList());

            var testImage = new byte[] { 42, 42, 42, 42, };
            mr.ReplayAll();
            Loader ldr = mr.PartialMock<Loader>(sc);
            ldr.Replay();

            ldr.DefaultToFormat = "ms-dos-com";
            Program prog = ldr.LoadExecutable("", testImage, null);

            Assert.IsNull(eventListener.LastDiagnostic);
            Assert.AreEqual("0C00:0100", prog.Image.BaseAddress.ToString());
            Assert.AreSame(x86arch, prog.Architecture);
            Assert.AreSame(msdosPlatform, prog.Platform);
            mr.VerifyAll();
        }

        private void Given_MsDosRawFileFormat()
        {
            this.x86arch = mr.Stub<IProcessorArchitecture>();
            var env = mr.Stub<OperatingEnvironment>();
            this.msdosPlatform = mr.Stub<Platform>(sc, x86arch);
            var state = mr.Stub<ProcessorState>();
            var rawFile = new RawFileElementImpl
            {
                BaseAddress = "0C00:0100",
                Environment = "ms-dos",
                Architecture = "x86-real-16",
            };
            rawFile.EntryPoint.Address = null;
            rawFile.EntryPoint.Name = "Start_Here";
            cfgSvc.Stub(d => d.GetRawFile("ms-dos-com")).Return(rawFile);
            cfgSvc.Stub(d => d.GetArchitecture("x86-real-16")).Return(x86arch);
            cfgSvc.Stub(d => d.GetEnvironment("ms-dos")).Return(env);
            env.Stub(e => e.Load(null, null)).IgnoreArguments().Return(msdosPlatform);
            x86arch.Stub(a => a.TryParseAddress(
                Arg<string>.Is.Equal("0C00:0100"),
                out Arg<Address>.Out(Address.SegPtr(0x0C00, 0x0100)).Dummy))
                .Return(true);
            x86arch.Stub(a => a.CreateProcessorState()).Return(state);
        }

        [Test]
        public void Ldr_AtOffset()
        {
            cfgSvc.Stub(d => d.GetImageLoaders()).Return(new ArrayList
            {
                new LoaderElementImpl {
                    Offset = "0002",
                    MagicNumber = "A0A0",
                    TypeName = typeof(TestImageLoader).AssemblyQualifiedName,
                }
            });
            Given_Image();
            mr.ReplayAll();

            Loader ldr = mr.PartialMock<Loader>(sc);
            ldr.Stub(l => l.LoadImageBytes("", 0)).Return(testImage);
            mr.ReplayAll();

            var imgLoader = ldr.FindImageLoader<ImageLoader>("", testImage, () => null);

            Assert.IsInstanceOf<TestImageLoader>(imgLoader);
            mr.VerifyAll();
        }

        private void Given_Image()
        {
            this.testImage = new byte[] { 0x2A, 0x2A, 0xA0, 0xA0 };
        }

        private class FakeImageLoader  : ImageLoader
        {
            public FakeImageLoader(IServiceProvider services, string filename, byte[]imgRaw) :
                base(services, filename, imgRaw)
            {

            }

            public override Address PreferredBaseAddress {get; set; }

            public override Program Load(Address addrLoad)
            {
                return new Program();
            }

            public override RelocationResults Relocate(Address addrLoad)
            {
                return new RelocationResults(new List<EntryPoint>(), new RelocationDictionary());
            }
        }

        private void Given_ImageLoader()
        {
            var ldrs = new List<LoaderElement>{
                new LoaderElementImpl {
                    MagicNumber = "2A2A",
                    TypeName = typeof(FakeImageLoader).AssemblyQualifiedName,
                }
            };
            cfgSvc.Expect(c => c.GetImageLoaders()).Return(ldrs);
        }

        [Test]
        public void Ldr_CreateDefaultImageLoader_GivenDefault()
        {
            Given_MsDosRawFileFormat();
            mr.ReplayAll();
            var ldr = mr.PartialMock<Loader>(sc);
            ldr.Replay();

            ldr.DefaultToFormat = "ms-dos-com";
            var imgLoader = ldr.CreateDefaultImageLoader("foo.com", new byte[30]);
            var program = imgLoader.Load(null);
        }

        public class TestImageLoader : ImageLoader
        {
            public TestImageLoader(IServiceProvider services, string filename, byte[] imgRaw) : base(services, filename, imgRaw)
            {
            }

            public override Address PreferredBaseAddress
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public override Program Load(Address addrLoad)
            {
                throw new NotImplementedException();
            }

            public override RelocationResults Relocate(Address addrLoad)
            {
                throw new NotImplementedException();
            }
        }
	}
}
