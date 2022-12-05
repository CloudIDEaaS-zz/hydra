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
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Decompiler.UnitTests.Mocks
{
    public class FakeWorkerDialogService : IWorkerDialogService
    {
        public bool StartBackgroundWork(string caption, Action backgroundWork)
        {
            backgroundWork();
            return true;
        }

        public void FinishBackgroundWork()
        {
        }

        public void ShowError(string context, Exception ex)
        {
        }

        public void SetCaption(string newCaption)
        {
        }

        public void Error(Decompiler.Core.ICodeLocation location, Exception ex, string message)
        {
        }
    }
}
