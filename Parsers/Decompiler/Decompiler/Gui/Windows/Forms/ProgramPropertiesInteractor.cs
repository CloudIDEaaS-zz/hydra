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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Gui.Windows.Forms
{
    public class ProgramPropertiesInteractor
    {
        private ProgramPropertiesDialog dlg;

        public void Attach(ProgramPropertiesDialog dlg)
        {
            this.dlg = dlg;
            dlg.Load += dlg_Load;
            dlg.EnableScript.CheckedChanged += delegate { EnableControls(); };
            dlg.OkButton.Click += OkButton_Click;
        }

        private void EnableControls()
        {
            dlg.LoadScript.Enabled = dlg.EnableScript.Checked;
        }

        private void dlg_Load(object sender, EventArgs e)
        {
            var loadedScript = dlg.Program.OnLoadedScript;
            if (loadedScript != null)
            {
                dlg.EnableScript.Checked = loadedScript.Enabled;
                dlg.LoadScript.Text = loadedScript.Script;
            }
            dlg.HeuristicScanning.Checked = dlg.Program.Options.HeuristicScanning;
        }

        void OkButton_Click(object sender, EventArgs e)
        {
            dlg.Program.OnLoadedScript = new Core.Serialization.Script_v2
            {
                Enabled = dlg.EnableScript.Checked,
                Script = dlg.LoadScript.Text,
            };
            dlg.Program.Options.HeuristicScanning = dlg.HeuristicScanning.Checked;
        }
    }
}
