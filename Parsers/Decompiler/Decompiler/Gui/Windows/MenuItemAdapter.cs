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

using Decompiler.Gui.Windows.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    public class MenuItemAdapter : IMenuAdapter
    {
        private IList menuItems;

        public MenuItemAdapter(IList menuItems)
        {
            this.menuItems = menuItems;
        }

        public int Count { get { return menuItems.Count; } }

        public CommandID GetCommandID(int i)
        {
            var item = (CommandMenuItem) menuItems[i];
            return item.MenuCommand.CommandID;
        }

        public void RemoveAt(int i)
        {
            var item = (CommandMenuItem) menuItems[i];
            //item.Popup -= new EventHandler(subMenu_Popup);
            //item.Click -= new CommandMenuEventHandler(item_Click);
            menuItems.RemoveAt(i);
        }

        public bool IsDynamic(int i)
        {
            var item = (CommandMenuItem) menuItems[i];
            return item.IsDynamic;
        }

        public bool IsTemporary(int i)
        {
            var item = (CommandMenuItem) menuItems[i];
            return item.IsTemporary;
        }

        public bool IsSeparator(int i)
        {
            var item = (CommandMenuItem) menuItems[i];
            return item.Text == "-";
        }

        public void InsertAt(int i, object item)
        {
            menuItems.Insert(i, item);
        }

        public void SetText(int i, string text)
        {
            var item = (CommandMenuItem) menuItems[i];
            item.Text = text;
        }

        public void SetStatus(int i, MenuStatus s)
        {
            var item = (CommandMenuItem) menuItems[i];
            item.Visible = (s & MenuStatus.Visible) != 0;
            item.Enabled = (s & MenuStatus.Enabled) != 0;
            item.Checked = (s & MenuStatus.Checked) != 0;
        }
    }
}
