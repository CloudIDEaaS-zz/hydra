using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using System.Diagnostics;
using GuidAttribute = System.Runtime.InteropServices.GuidAttribute;
using Utils;

namespace VisualStudioProvider
{
    public class OutputGlobalCommandTarget : GlobalCommandTargetBase
    {
        private IVsStatusbar statusBar;
        private IVsOutputWindowPane outputPane;
        private Dictionary<Guid, CommandSet> exclusions;

        internal OutputGlobalCommandTarget(Dictionary<Guid, CommandSet> exclusions = null)
        {
            statusBar = (IVsStatusbar)Package.GetGlobalService(typeof(SVsStatusbar));

            this.exclusions = exclusions;
        }

        public static Dictionary<Guid, CommandSet> GetCommonExcludedCommands()
        {
            /*
             VSStd2KCmdID	  OutputPaneCombo
             VSStd2KCmdID	  SolutionPlatform
             VSStd97CmdID	  OutputWindow
             VSStd97CmdID	  SearchCombo
             VSStd97CmdID	  SolutionCfg, SolutionCfg
             VSStd97CmdID	  SubsetCombo
             VSStd97CmdID	 0x679
             VSStd97CmdID	 0x6b5
            */

            var commands = new Dictionary<Guid, CommandSet>()
            {
                { typeof(VSConstants.VSStd2KCmdID).GUID, new CommandSet(typeof(VSConstants.VSStd2KCmdID).GUID, "VSStd2KCmdID", typeof(VSConstants.VSStd2KCmdID))
                    {
                        CmdIds = new Dictionary<uint,string>()
                        {
                            { (uint) VSConstants.VSStd2KCmdID.OutputPaneCombo, "OutputPaneCombo" },
                            { (uint) VSConstants.VSStd2KCmdID.SolutionPlatform, "SolutionPlatform" }
                        }
                    }
                },
                { typeof(VSConstants. VSStd97CmdID).GUID, new CommandSet(typeof(VSConstants. VSStd97CmdID).GUID, " VSStd97CmdID", typeof(VSConstants. VSStd97CmdID))
                    {
                        CmdIds = new Dictionary<uint,string>()
                        {
                            { (uint) VSConstants.VSStd97CmdID.OutputWindow, "OutputWindow" },
                            { (uint) VSConstants.VSStd97CmdID.SearchCombo, "SearchCombo" },
                            { (uint) VSConstants.VSStd97CmdID.SubsetCombo, "SubsetCombo" },
                            { (uint) VSConstants.VSStd97CmdID.SolutionCfg, "SolutionCfg" },
                            { 0x679, " 0x679" },
                            { 0x6b5, " 0x6b5" }
                        }
                    }
                }
            };

            return commands;
        }

        protected override void WriteLine(string format, params object[] args)
        {
            if (outputPane == null)
            {
                var output = (IVsOutputWindow)Package.GetGlobalService(typeof(SVsOutputWindow));
                var generalPaneGuid = VSConstants.GUID_OutWindowGeneralPane;

                if (ErrorHandler.Failed(output.GetPane(ref generalPaneGuid, out outputPane)) || outputPane == null)
                {
                    if (ErrorHandler.Failed(output.CreatePane(ref generalPaneGuid, "Global Command Capture Output", 1, 1)))
                    {
                        statusBar.SetText("Attempt to create output pane failed");

                        Stop();

                        return;
                    }

                    if (ErrorHandler.Failed(output.GetPane(ref generalPaneGuid, out outputPane)) || outputPane == null)
                    {
                        statusBar.SetText("Attempt to get output pane failed");

                        Stop();

                        return;
                    }
                }

                outputPane.SetName("Global Command Capture Output");

                WriteLine(format, args);
            }
            else
            {
                outputPane.OutputStringThreadSafe(string.Format(format + "\r\n", args));
            }
        }

        private bool Exclude(Guid pguidCmdGroup, uint nCmdID)
        {
            if (exclusions.ContainsKey(pguidCmdGroup))
            {
                var excludedGroup = exclusions[pguidCmdGroup];

                if (excludedGroup.CmdIds == null)
                {
                    return true;
                }
                else
                {
                    if (excludedGroup.CmdIds.ContainsKey(nCmdID))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            int hr;

            if (!Exclude(pguidCmdGroup, nCmdID))
            {
                try
                {
                    var inArg = pvaIn.GetObjectForVariant();
                    object outArg;

                    if (CommandSets.ContainsKey(pguidCmdGroup))
                    {
                        var commandSet = CommandSets[pguidCmdGroup];
                        var setName = commandSet.Name;

                        if (commandSet.CmdIds.ContainsKey(nCmdID))
                        {
                            var cmdName = commandSet.CmdIds[nCmdID];

                            WriteLine("CommandGroup: {0}\t CommandID: '{1}'\t CommandName: '{2}'\t Options: '{3}'\t Argument: {4}", setName, nCmdID, cmdName, nCmdexecopt, inArg.AsDisplayText());
                            hr = base.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

                            outArg = pvaOut.GetObjectForVariant();
                            WriteLine("CommandGroup: {0}\t CommandID: '{1}'\t CommandName: '{2}'\t Options: '{3}'\t Out Argument: {4}\t hResult: 0x{5:x}", setName, nCmdID, cmdName, nCmdexecopt, outArg.AsDisplayText(), hr);
                        }
                        else
                        {
                            WriteLine("CommandGroup: {0}\t CommandID 0x{1:x}\t Options: {2}\t Argument: {3}", setName, nCmdID, nCmdexecopt, inArg.AsDisplayText());
                            hr = base.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

                            outArg = pvaOut.GetObjectForVariant();
                            WriteLine("CommandGroup: {0}\t CommandID 0x{1:x}\t Options: {2}\t Out Argument: {3}\t hResult: 0x{4:x}", setName, nCmdID, nCmdexecopt, outArg.AsDisplayText(), hr);
                        }
                    }
                    else
                    {
                        WriteLine("CommandGroup: {0}\t CommandID: 0x{1:x}\t Options: {2}\t Argument: {3}", pguidCmdGroup.ToString(), nCmdID, nCmdexecopt, inArg.AsDisplayText());
                        hr = base.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

                        outArg = pvaOut.GetObjectForVariant();
                        WriteLine("CommandGroup: {0}\t CommandID: 0x{1:x}\t Options: {2}\t Out Argument: {3}\t hResult: 0x{4:x}", pguidCmdGroup.ToString(), nCmdID, nCmdexecopt, outArg.AsDisplayText(), hr);
                    }
                }
                catch (Exception ex)
                {
                    WriteLine("Error with OutputGlobalCommandTarget: '{0}'", ex.ToString());
                }
            }

            hr = base.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

            return hr;
        }

        public override int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return base.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}
