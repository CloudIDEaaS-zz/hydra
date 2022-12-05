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

using Decompiler.Gui.Controls;
using Decompiler.Gui.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public partial class OpenAsDialog : Form, IOpenAsDialog
    {
        public OpenAsDialog()
        {
            InitializeComponent();

            AddressTextBox = new TextBoxWrapper(txtAddress);
            Architectures = new ComboBoxWrapper(ddlArchitectures);
            Platforms = new ComboBoxWrapper(ddlEnvironments);
            FileName = new TextBoxWrapper(textBox1);
            BrowseButton = new ButtonWrapper(btnBrowse);
            OkButton = new ButtonWrapper(btnOk);

            new OpenAsInteractor().Attach(this);
        }

        public IServiceProvider Services { get; set; }

        public ITextBox FileName { get; private set; }

        public ITextBox AddressTextBox { get; private set; }
        public IComboBox Architectures { get; private set; }
        public IComboBox Platforms { get; private set; }
        public IButton BrowseButton { get; private set; }
        public IButton OkButton { get; private set; }
    }
}
