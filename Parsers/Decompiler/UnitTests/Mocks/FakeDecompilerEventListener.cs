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
using Decompiler.Core.Services;
using Decompiler.Gui;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class FakeDecompilerEventListener : DecompilerEventListener, IWorkerDialogService
    {
        private string lastDiagnostic;
        private string lastProgress;
        private bool finishedCalled;
        private string lastStatus;

        public void Finished()
        {
            finishedCalled = true;
        }

        public void ShowError(string context, Exception ex)
        {
        }

        public void AddDiagnostic(ICodeLocation location, Diagnostic d)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} - {1} - {2}", d.GetType().Name, location.Text, d.Message);
            lastDiagnostic = sb.ToString();
            Debug.WriteLine(lastDiagnostic);
        }

        public void Warn(ICodeLocation location, string message)
        {
            AddDiagnostic(location, new WarningDiagnostic(message));
        }

        public void Error(ICodeLocation location, string message)
        {
            AddDiagnostic(location, new ErrorDiagnostic(message));
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            AddDiagnostic(location, new ErrorDiagnostic(message, ex));
        }

        public void ShowProgress(string caption, int numerator, int denominator)
        {
            lastProgress = string.Format("{0}: {1}%", caption, (numerator * 100) / denominator);
        }

        public void ShowStatus(string status)
        {
            lastStatus = status;
        }

        public void CodeStructuringComplete()
        {
        }

        public void DecompilationFinished()
        {
        }

        public void InterproceduralAnalysisComplete()
        {
        }

        public void MachineCodeRewritten()
        {
        }

        public void ProceduresTransformed()
        {
        }

        public void ProgramLoaded()
        {
        }

        public void ProgramScanned()
        {
        }

        public void TypeReconstructionComplete()
        {
        }

        // Diagnostic methods.

        public bool FinishedCalled
        {
            get { return finishedCalled; }
        }

        public string LastDiagnostic
        {
            get { return lastDiagnostic; }
        }

        public string LastProgress
        {
            get { return lastProgress; }
        }


        public ICodeLocation CreateAddressNavigator(Program program, Address address)
        {
            return new NullCodeLocation(address.ToString());
        }

        public ICodeLocation CreateProcedureNavigator(Procedure proc)
        {
            return new NullCodeLocation(proc.Name);
        }

        public ICodeLocation CreateBlockNavigator(Block block)
        {
            return new NullCodeLocation(block.Name);
        }


        #region IWorkerDialogService Members

        public bool StartBackgroundWork(string caption, Action backgroundWork)
        {
            backgroundWork();
            return true;
        }

        public void FinishBackgroundWork()
        { 
        }

        public void SetCaption(string newCaption)
        {
        }

        #endregion


    }
}
