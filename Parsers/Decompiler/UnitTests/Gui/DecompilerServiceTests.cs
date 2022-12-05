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
using Decompiler.Core.Services;
using Decompiler.Gui;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Decompiler.UnitTests.Gui
{
    [TestFixture]
    public class DecompilerServiceTests
    {
        ServiceContainer sc;
        IDecompilerService svc;
        private MockRepository mr;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            svc = new DecompilerService();
            sc.AddService(typeof(IDecompilerService), svc);
            sc.AddService(typeof(DecompilerEventListener), new FakeDecompilerEventListener());
        }
        
        [Test]
        public void DecSvc_NotifyOnChangedDecompiler()
        {
            var loader = mr.Stub<ILoader>();
            var host = mr.Stub<Decompiler.DecompilerHost>();
            mr.ReplayAll();

            DecompilerDriver d = new DecompilerDriver(loader, host, sc);
            bool decompilerChangedEventFired = true;
            svc.DecompilerChanged += delegate(object o, EventArgs e)
            {
                decompilerChangedEventFired = true;
            };

            svc.Decompiler = d;

            Assert.IsTrue(decompilerChangedEventFired, "Should have fired a change event");
        }

        [Test]
        public void DecSvc_EmptyDecompilerProjectName()
        {
            IDecompilerService svc = new DecompilerService();
            Assert.IsEmpty(svc.ProjectName, "Shouldn't have project name available.");
        }

        [Test]
        public void DecSvc_DecompilerProjectName()
        {
            IDecompilerService svc = new DecompilerService();
            var loader = mr.StrictMock<ILoader>();
            var host = mr.StrictMock<DecompilerHost>();
            var arch = mr.StrictMock<IProcessorArchitecture>();
            var platform = mr.StrictMock<Platform>(sc, arch);
            var fileName = "foo\\bar\\baz.exe";
            var bytes = new byte[100];
            var image = new LoadedImage(Address.Ptr32(0x1000), bytes);
            var imageMap = image.CreateImageMap();
            var prog = new Program(image, imageMap, arch, platform);
            loader.Stub(l => l.LoadImageBytes(fileName, 0)).Return(bytes);
            loader.Stub(l => l.LoadExecutable(fileName, bytes, null)).Return(prog);
            loader.Replay();
            var dec = new DecompilerDriver(loader, host, sc);
            mr.ReplayAll();

            svc.Decompiler = dec;
            svc.Decompiler.Load(fileName);

            Assert.IsNotNull(svc.Decompiler.Project);
            Assert.AreEqual("baz.exe",  svc.ProjectName, "Should have project name available.");
            mr.VerifyAll();
        }
    }
}
