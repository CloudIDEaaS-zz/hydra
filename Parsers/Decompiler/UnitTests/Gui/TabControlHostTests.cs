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

using Decompiler.Gui;
using Decompiler.Gui.Windows;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui
{
    [TestFixture]
    public class TabControlHostTests
    {
        private TabControl tabCtrl;
        private MockRepository mr;
        private IWindowPane pane;
        private IServiceProvider services;

        [SetUp]
        public void Setup()
        {
            this.tabCtrl = new TabControl();
            this.mr = new MockRepository();
            this.pane = mr.Stub<IWindowPane>();
            this.services = mr.Stub<IServiceProvider>();
        }

        [Test]
        public void Tch_AttachToPage()
        {
            tabCtrl.TabPages.Add("Test");
            Assert.AreEqual(1, tabCtrl.TabPages.Count);
            ITabControlHostService host = new TabControlHost(services, tabCtrl);
            IWindowFrame frame = host.Attach(pane, tabCtrl.TabPages[0]);
            frame.Title = "Foo";

            Assert.AreEqual("Foo", tabCtrl.TabPages[0].Text);
        }

        [Test]
        public void Tch_AddPage()
        {
            ITabControlHostService host = new TabControlHost(services, tabCtrl);
            IWindowFrame frame = host.Add(pane, "Foo");

            Assert.AreEqual("Foo", tabCtrl.TabPages[0].Text);
        }
    }
}
