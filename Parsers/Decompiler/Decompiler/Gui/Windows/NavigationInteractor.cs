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
using Decompiler.Gui.Components;
using Decompiler.Gui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Gui.Windows
{
    /// <summary>
    /// Performs back/forward navigation.
    /// </summary>
    /// <remarks>
    /// This interactor is connected to a navigable control, which will have a pair of buttons -- for "Back" and "Forward" -- a timer, and 
    /// a way to show a menu to the user if the timer times out.
    /// </remarks>
    public class NavigationInteractor
    {
        private INavigableControl navControl;
        private List<Address> navStack = new List<Address>();
        private int stackPosition = 0;

        public void Attach(INavigableControl navControl)
        {
            this.navControl = navControl;

            EnableControls();

            navControl.BackButton.Click += btnBack_Click;
            navControl.ForwardButton.Click += btnForward_Click;
        }

        private Address Location
        {
            get {
                if (stackPosition >= navStack.Count)
                    return null;
                return navStack[stackPosition];
            }
        }

        private void EnableControls()
        {
            navControl.BackButton.Enabled = stackPosition > 0;
            navControl.ForwardButton.Enabled = stackPosition < navStack.Count;
        }

        /// <summary>
        /// Call this when a user navigation action has occurred and you need to add an address
        /// to the "stack".
        /// </summary>
        /// <param name="address"></param>
        public void UserNavigateTo(Address address)
        {
            int itemsAhead = navStack.Count - stackPosition;
            if (stackPosition >= 0 && itemsAhead > 0)
            {
                Debug.Print("Removing {0}:{1}", stackPosition, itemsAhead);
                navStack.RemoveRange(stackPosition, itemsAhead);
            }
            navStack.Add(navControl.CurrentAddress);    // Remember where we were...
            ++stackPosition;
            EnableControls();
            navControl.CurrentAddress = address;        // ...and move to the new position.
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            if (stackPosition <= 0)
                return;
            --stackPosition;
            EnableControls();
            navControl.CurrentAddress = Location;        // ...and move to the new position.
        }

        void btnForward_Click(object sender, EventArgs e)
        {
            if (stackPosition >= navStack.Count)
                return;
            var loc = Location;
            ++stackPosition;
            EnableControls();
            navControl.CurrentAddress = loc;        // ...and move to the new position.
        }
    }
}
