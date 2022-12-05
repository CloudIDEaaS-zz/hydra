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
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui
{
    /// <summary>
    /// Abstracts the notion of an item in a list view, for future platform
    /// independence.
    /// </summary>
    public interface IListViewItem
    {
        //$REVIEW: consider exposing an IconIndex property as well to cover th 90% case and avoid the delegate below.

        string Text { get; set; }
        object Tag { get; set; }

        void AddSubItem(string text);
    }

    public interface IListViewSubItem
    {
        string Text { get; set; }
    }
}
