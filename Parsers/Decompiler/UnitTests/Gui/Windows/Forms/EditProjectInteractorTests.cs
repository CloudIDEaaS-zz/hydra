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
using Decompiler.Core.Serialization;
using Decompiler.Core.Services;
using Decompiler.Gui;
using Decompiler.Gui.Windows.Forms;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Gui.Windows.Forms
{
    [TestFixture]
    public class EditProjectInteractorTests
    {
        private EditProjectDialog dlg;

        [SetUp]
        public void Setup()
        {
            dlg = new EditProjectDialog();
        }

        [TearDown]
        public void Teardown()
        {
            if (dlg != null)
                dlg.Dispose();
            dlg = null;
        }

        [Test]
        public void EditProjectAndCancel()
        {
            FakeShellUiService uiSvc = new FakeShellUiService();
            uiSvc.SimulateUserCancel = true;
            var project = CreateTestProject();
            var epi = new EditProjectInteractor();
            var ret = epi.EditProjectProperties(uiSvc, project, delegate(Project_v1 p)
            {
                Assert.Fail("Should not save if user cancels.");
            });
            Assert.IsFalse(ret);
            Assert.AreEqual("test.exe", project.Input.Filename);
            Assert.AreEqual("10000", project.Input.Address);
            Assert.AreEqual("test.asm", project.Output.DisassemblyFilename);
            Assert.AreEqual("test.dis", project.Output.IntermediateFilename);
            Assert.AreEqual("test.h", project.Output.TypesFilename);
            Assert.AreEqual("test.c", project.Output.OutputFilename);
        }

        [Test]
        public void EditProjectAndSave()
        {
            var uiSvc = new FakeShellUiService();
            var epi = new EditProjectInteractor();
            var p = CreateTestProject();

            var ret = epi.EditProjectProperties(uiSvc, p, delegate(Project_v1 project)
            {
                Assert.AreEqual("test.exe", project.Input.Filename);
                Assert.AreEqual("10000", project.Input.Address);
                Assert.AreEqual("test.asm", project.Output.DisassemblyFilename);
                Assert.AreEqual("test.dis", project.Output.IntermediateFilename);
                Assert.AreEqual("test.h", project.Output.TypesFilename);
                Assert.AreEqual("test.c", project.Output.OutputFilename);
            });
            Assert.IsTrue(ret);
        }

        [Test]
        public void ClickInputButton()
        {
            var interactor = new TestEditProjectInteractor();
            interactor.Attach(dlg);
            dlg.Show();
            interactor.FakeUiService.OpenFileResult = "booga.exe";
            interactor.UserClickBrowseBinaryFileButton();
            Assert.AreEqual("booga.exe", dlg.BinaryFilename.Text);

        }

        [Test]
        public void ClickOutputButton()
        {
            var interactor = new TestEditProjectInteractor();
            interactor.Attach(dlg);
            dlg.Show();
            interactor.FakeUiService.SaveFileResult = "fooga.exe";
            interactor.UserClickBrowseBinaryFileButton();
            Assert.AreEqual("fooga.exe", dlg.BinaryFilename.Text);
        }

        private  Project_v1 CreateTestProject()
        {
            var project = new Project_v1();
            project.Input.Filename = "test.exe";
            project.Input.Address = "10000";
            project.Output.DisassemblyFilename = "test.asm";
            project.Output.IntermediateFilename = "test.dis";
            project.Output.TypesFilename = "test.h";
            project.Output.OutputFilename = "test.c";
            return project;
        }
    }

    public class TestEditProjectInteractor : EditProjectInteractor
    {
        public FakeUiService FakeUiService;

        protected override IDecompilerUIService CreateUIService(System.Windows.Forms.Form dlg, System.Windows.Forms.OpenFileDialog ofd, System.Windows.Forms.SaveFileDialog sfd)
        {
            FakeUiService = new FakeUiService();
            return FakeUiService;
        }

        internal void UserClickBrowseBinaryFileButton()
        {
            base.BrowseBinaryFileButton_Click(null, EventArgs.Empty);
        }

        internal void UserClickBrowseOutputFileButton()
        {
            base.BrowseOutputFileButton_Click(null, EventArgs.Empty);
        }
    }
}
