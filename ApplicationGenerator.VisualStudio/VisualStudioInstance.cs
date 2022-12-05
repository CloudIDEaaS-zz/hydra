// file:	VisualStudioInstance.cs
//
// summary:	Implements the visual studio instance class

using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace AbstraX
{
    /// <summary>   A visual studio instance. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 6/11/2022. </remarks>

    public class VisualStudioInstance : IDevIDEInstance
    {
        /// <summary>   Gets the DTE. </summary>
        ///
        /// <value> The DTE. </value>

        /// <summary>   Gets the process. </summary>
        ///
        /// <value> The process. </value>

        public System.Diagnostics.Process Process { get; }
        public string ProcessName => "devenv";

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/11/2022. </remarks>
        ///
        /// <param name="process">  The process. </param>

        private DTE GetDTE(string solutionName)
        {
            var process = System.Diagnostics.Process.GetProcessesByName("devenv.exe").Single(p => p.MainWindowTitle.StartsWith(solutionName + " - "));
            var dte = DTEExtensions.GetDTE(process.Id);

            return dte;
        }

        /// <summary>   Query if this  is debugging. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/11/2022. </remarks>
        ///
        /// <returns>   True if debugging, false if not. </returns>

        public bool IsDebugging(string solutionName)
        {
            var isDebugging = this.GetDTE(solutionName).Debugger.CurrentMode != dbgDebugMode.dbgDesignMode;

            if (isDebugging)
            {
                ControlExtensions.FlashWindow(this.Process.MainWindowHandle, FlashWindowFlags.FLASHW_ALL, 3, 3000);

                return true;
            }

            return isDebugging;
        }

        /// <summary>   Query if this  has errors. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/11/2022. </remarks>
        ///
        /// <returns>   True if errors, false if not. </returns>

        public bool HasErrors(string solutionName)
        {
            DTE2 dte2 = (DTE2)this.GetDTE(solutionName);
            ErrorList errorList;
            int count;

            this.GetDTE(solutionName).ExecuteCommand("View.ErrorList", " ");
            errorList = dte2.ToolWindows.ErrorList;
            count = errorList.ErrorItems.Count;

            if (count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>   Debug start. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/11/2022. </remarks>
        ///
        /// <param name="solutionName"> Name of the solution. </param>
        /// <param name="projectName">  Name of the project. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool DebugStart(string solutionName, string projectName)
        {
            try
            {
                DateTime start;
                UIHierarchyItem hierarchyItem;
                string name;
                DTE dte = this.GetDTE(solutionName);
                DTE2 dte2 = (DTE2)dte;
                Documents documents;
                List<Document> documentList;
                var retry = 0;
                var retryLimit = 5;
                var started = false;
                var hierarchy = $"{ solutionName }\\{ projectName }";

                documents = dte2.Documents;
                documentList = documents.GetEnumerator().GetList<Document>();

                documents.SaveAll();
                documents.CloseAll();

                while (!started)
                {
                    hierarchyItem = dte2.ToolWindows.SolutionExplorer.GetItem(hierarchy);

                    if (hierarchyItem == null)
                    {
                        DebugUtils.Break();
                    }

                    name = hierarchyItem.Name;
                    hierarchyItem.Select(vsUISelectionType.vsUISelectionTypeSelect);

                    while (!hierarchyItem.IsSelected)
                    {
                        System.Threading.Thread.Sleep(100);
                        hierarchyItem.Select(vsUISelectionType.vsUISelectionTypeSelect);
                    }

                    try
                    {
                        dte.ExecuteCommand("Project.SetAsStartupProject");
                    }
                    catch (Exception ex)
                    {
                    }

                    try
                    {
                        dte.ExecuteCommand("ClassViewContextMenus.ClassViewProject.Debug.Startnewinstance");
                    }
                    catch
                    {
                        try
                        {
                            dte.ExecuteCommand("Debug.Start");
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                dte.ExecuteCommand("SolutionExplorer.Refresh");
                            }
                            catch (Exception ex2)
                            {
                            }

                            System.Threading.Thread.Sleep(100);
                            retry++;

                            if (retry == retryLimit)
                            {
                                if (MessageBox.Show($"Issues starting { hierarchy }. Continue debugging?", "Debugging error", MessageBoxButtons.YesNo) == DialogResult.No)
                                {
                                    return false;
                                }
                            }

                            continue;
                        }
                    }

                    started = true;
                }

                start = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw;
            }

            return true;
        }

        /// <summary>   Builds. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/11/2022. </remarks>
        ///
        /// <param name="solutionName"> Name of the solution. </param>
        /// <param name="projectName">  Name of the project. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool Build(string solutionName, string projectName)
        {
            try
            {
                _dispBuildEvents_OnBuildBeginEventHandler onBuildBegin;
                _dispBuildEvents_OnBuildDoneEventHandler onBuildDone;
                var resetEvent = new ManualResetEvent(false);
                var start = DateTime.Now;
                string name;
                UIHierarchyItem hierarchyItem;
                DTE2 dte2 = (DTE2)this.GetDTE(solutionName);

                hierarchyItem = dte2.ToolWindows.SolutionExplorer.GetItem($"{ solutionName }\\{ projectName }");

                dte2.Documents.SaveAll();

                if (hierarchyItem == null)
                {
                    DebugUtils.Break();
                }

                name = hierarchyItem.Name;
                hierarchyItem.Select(vsUISelectionType.vsUISelectionTypeSelect);

                while (!hierarchyItem.IsSelected)
                {
                    System.Threading.Thread.Sleep(100);
                    hierarchyItem.Select(vsUISelectionType.vsUISelectionTypeSelect);
                }

                onBuildBegin = (scope, action) =>
                {
                };

                onBuildDone = (scope, action) =>
                {
                };

                this.GetDTE(solutionName).Events.BuildEvents.OnBuildBegin += onBuildBegin;
                this.GetDTE(solutionName).Events.BuildEvents.OnBuildDone += onBuildDone;

                this.GetDTE(solutionName).ExecuteCommand("Build.BuildSelection");

                while (!resetEvent.WaitOne(1))
                {
                    if (DateTime.Now - start >= TimeSpan.FromSeconds(120))
                    {
                        break;
                    }

                }

                this.GetDTE(solutionName).Events.BuildEvents.OnBuildBegin -= onBuildBegin;
                this.GetDTE(solutionName).Events.BuildEvents.OnBuildDone -= onBuildDone;
                System.Threading.Thread.Sleep(1000);

                return true;
            }
            catch (Exception ex)
            {
                DebugUtils.Break();
            }

            return true;
        }

        /// <summary>   Debug stop. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/11/2022. </remarks>
        ///
        /// <param name="disposable">   The disposable. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool DebugStop(string solutionName)
        {
            try
            {
                var dte = this.GetDTE(solutionName);
                var isDebugging = dte.Debugger.CurrentMode != dbgDebugMode.dbgDesignMode;

                if (isDebugging)
                {
                    dte.Debugger.Stop();
                    dte.ExecuteCommand("Debug.StopDebugging");
                }
                else
                {
                    try
                    {
                        dte.ExecuteCommand("Build.Cancel");
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                DebugUtils.Break();
            }

            return true;
        }

        /// <summary>   Closes this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/11/2022. </remarks>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool Close(string solutionName)
        {
            try
            {
                this.GetDTE(solutionName).Solution.Close(true);
            }
            catch
            {
            }

            this.Process.CloseMainWindow();

            return true;
        }

        public bool WindowTextMatches(string text, string workspaceName)
        {
            return text.Contains(workspaceName + " - Microsoft Visual Studio") && !text.Contains("Debugging");
        }

        [DebuggerHidden]
        public void DebugAttach(System.Diagnostics.Process[] processes, bool writeToConsole)
        {
            var command = "AttachToProcess";
            var thisProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
            var windowFound = false;

            if (writeToConsole)
            {
                Console.WriteLine("Looking for Debug Window");
            }

            foreach (var vsInstance in processes)
            {
                var hwndVisualStudio = vsInstance.MainWindowHandle;
                var childWindows = ControlExtensions.GetChildWindows(hwndVisualStudio);

                foreach (var hwndChild in childWindows)
                {
                    var text = ControlExtensions.GetWindowText(hwndChild);

                    if (text.ToString() == "Hydra Debug")
                    {
                        childWindows = ControlExtensions.GetChildWindows(hwndChild);

                        foreach (var hwndChild2 in childWindows)
                        {
                            var className = ControlExtensions.GetClassName(hwndChild2);

                            if (className.StartsWith("WindowsForms"))
                            {
                                IntPtr messagePtr;
                                IntPtr lresult;
                                ControlExtensions.COPYDATASTRUCT copyDataStruct;
                                CommandPacket commandPacket;
                                string json;
                                var clientWindow = new frmDebugClientWindow();

                                clientWindow.Show();

                                commandPacket = new CommandPacket(command, new KeyValuePair<string, object>("ClientHwnd", clientWindow.Handle.ToHexString()), new KeyValuePair<string, object>("ProcessId", thisProcessId.ToHexString()));
                                json = commandPacket.ToJsonText();

                                messagePtr = Marshal.StringToCoTaskMemAnsi(json + "\0");

                                copyDataStruct = new ControlExtensions.COPYDATASTRUCT
                                {
                                    dwData = (IntPtr)0,
                                    cbData = json.Length + 1,
                                    lpData = messagePtr
                                };

                                lresult = ControlExtensions.SendMessage<ControlExtensions.COPYDATASTRUCT>(hwndChild2, ControlExtensions.WindowsMessage.COPYDATA, 0, copyDataStruct);

                                Marshal.FreeCoTaskMem(messagePtr);

                                if (((int)lresult) != 0)
                                {
                                    while (!clientWindow.ManualResetAttached.WaitOne(100))
                                    {
                                        Application.DoEvents();
                                    }

                                    clientWindow.Close();

                                    DebugUtils.Break();
                                    windowFound = true;
                                    break;
                                }
                            }
                        }

                        if (windowFound)
                        {
                            break;
                        }
                    }

                    if (windowFound)
                    {
                        break;
                    }
                }

                if (windowFound)
                {
                    break;
                }
            }

            if (!windowFound)
            {
                foreach (var notifyWindow in ControlExtensions.GetWindows().Where(w => ControlExtensions.GetWindowText(w) == "SystemResourceNotifyWindow"))
                {
                    var childWindows = ControlExtensions.GetChildWindows(notifyWindow);

                    foreach (var hwndChild in childWindows)
                    {
                        var text = ControlExtensions.GetWindowText(hwndChild);

                        if (text.ToString() == "Hydra Debug")
                        {
                            childWindows = ControlExtensions.GetChildWindows(hwndChild);

                            foreach (var hwndChild2 in childWindows)
                            {
                                var className = ControlExtensions.GetClassName(hwndChild2);

                                if (className.StartsWith("WindowsForms"))
                                {
                                    IntPtr messagePtr;
                                    IntPtr lresult;
                                    ControlExtensions.COPYDATASTRUCT copyDataStruct;
                                    CommandPacket commandPacket;
                                    string json;
                                    var clientWindow = new frmDebugClientWindow();

                                    clientWindow.CreateControl();

                                    commandPacket = new CommandPacket(command, new KeyValuePair<string, object>("ClientHwnd", clientWindow.Handle.ToHexString()), new KeyValuePair<string, object>("ProcessId", thisProcessId.ToHexString()));
                                    json = commandPacket.ToJsonText();

                                    messagePtr = Marshal.StringToCoTaskMemAnsi(json + "\0");

                                    copyDataStruct = new ControlExtensions.COPYDATASTRUCT
                                    {
                                        dwData = (IntPtr)0,
                                        cbData = json.Length + 1,
                                        lpData = messagePtr
                                    };

                                    lresult = ControlExtensions.SendMessage<ControlExtensions.COPYDATASTRUCT>(hwndChild2, ControlExtensions.WindowsMessage.COPYDATA, 0, copyDataStruct);

                                    Marshal.FreeCoTaskMem(messagePtr);

                                    if (((int)lresult) != 0)
                                    {
                                        while (!clientWindow.ManualResetAttached.WaitOne(100))
                                        {
                                            Application.DoEvents();
                                        }

                                        clientWindow.Close();

                                        DebugUtils.Break();
                                        windowFound = true;
                                        break;
                                    }
                                }
                            }

                            if (windowFound)
                            {
                                break;
                            }
                        }

                        if (windowFound)
                        {
                            break;
                        }

                    }

                    if (windowFound)
                    {
                        break;
                    }
                }

                if (!windowFound)
                {
                    System.Diagnostics.Debugger.Launch();
                }
            }
        }
    }
}