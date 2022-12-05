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

using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Core.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Forms
{
    public interface IFinalPageInteractor : IPhasePageInteractor
    {
    }

    public class FinalPageInteractor : PhasePageInteractorImpl, IFinalPageInteractor
    {
        public FinalPageInteractor(IServiceProvider services) : base(services)
        {
        }

        public void ConnectToBrowserService()
        {
        }

        public void DisconnectFromBrowserService()
        {
        }

        public override void PerformWork(IWorkerDialogService workerDialogSvc)
        {
            try
            {
                workerDialogSvc.SetCaption("Reconstructing datatypes.");
                Decompiler.ReconstructTypes();
                workerDialogSvc.SetCaption("Structuring program.");
                Decompiler.StructureProgram();
            }
            catch (Exception ex)
            {
                //$REVIEW: need a new exception type which when thrown contains the activity we were doing.
                workerDialogSvc.Error(new NullCodeLocation(""), ex, "An error occurred while reconstructing types.");
            }
        }

        public override void EnterPage()
        {
        }

        public override bool LeavePage()
        {
            DisconnectFromBrowserService();
            return true;
        }
    }
}
