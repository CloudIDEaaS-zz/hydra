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

using Decompiler.Core.Services;
using Decompiler.Gui;
using Decompiler.Gui.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    /// <summary>
    /// Windows Forms implementation of the IDecompilerUIService service.
    /// </summary>
    public class DecompilerUiService : IDecompilerUIService
    {
        private Form form;
        private OpenFileDialog ofd;
        private SaveFileDialog sfd;

        public DecompilerUiService(Form form, OpenFileDialog ofd, SaveFileDialog sfd)
        {
            this.form = form;
            this.ofd = ofd;
            this.sfd = sfd;
        }

        #region IDecompilerUIService Members

        public bool Prompt(string prompt)
        {
            DialogResult dlgr = DialogResult.No;
            form.Invoke(new Action(
                () => { dlgr = MessageBox.Show(prompt, "Decompiler", MessageBoxButtons.YesNo, MessageBoxIcon.Question); }));
            return dlgr == DialogResult.Yes;
        }

        private DialogResult ShowModalDialog(Form dlg)
        {
            return (DialogResult)
                form.Invoke(new Func<Form, DialogResult>(delegate(Form dlgToShow)
                {
                    return dlgToShow.ShowDialog(form);
                }), dlg);
        }

        public virtual DialogResult ShowModalDialog(IDialog dlg)
        {
            return ShowModalDialog((Form)dlg);
        }

        public virtual string ShowOpenFileDialog(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                ofd.FileName = fileName;
            if (ofd.ShowDialog(form) == DialogResult.OK)
            {
                return ofd.FileName;
            }
            else
                return null;
        }

        public virtual string ShowSaveFileDialog(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                sfd.FileName = fileName;
            if (sfd.ShowDialog(form) == DialogResult.OK)
            {
                return sfd.FileName;
            }
            else
                return null;
        }

        public virtual void ShowError(Exception ex, string format, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(format, args);
            Exception e = ex;
            while (e != null)
            {
                sb.Append(" ");
                sb.Append(e.Message);
                e = e.InnerException;
            }
            form.Invoke(new Action<string>(delegate(string s)
                { MessageBox.Show(form, s, "Decompiler", MessageBoxButtons.OK, MessageBoxIcon.Error); }),
                sb.ToString() );
        }

        public virtual void ShowError(string format, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(format, args);
            form.Invoke(new Action<StringBuilder>(delegate(StringBuilder s)
                { MessageBox.Show(form, s.ToString(), "Decompiler", MessageBoxButtons.OK, MessageBoxIcon.Error); }),
                sb);
        }
        #endregion


        public void ShowMessage(string msg)
        {
            form.Invoke(new Action<string>(delegate(string s)
                { MessageBox.Show(form, s, "Decompiler", MessageBoxButtons.OK, MessageBoxIcon.Information); }),
                msg);
        }
    }
}