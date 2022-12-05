using EnvDTE;
using EnvDTE80;
using EnvDTE90a;
using HydraDebugAssistant.Info;
using MailSlot;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using Utils;
using Utils.NamedPipes;
using static Utils.ControlExtensions;
using Process = System.Diagnostics.Process;

namespace HydraDebugAssistant
{
    public partial class HydraDebugToolWindowControl : UserControl, IOleComponent
    {
        private IntPtr hwndPane;
        private DirectoryInfo generatorsDirectory;
        private bool capturingTreeViewPaint;
        private NamedPipeServer namedPipeServer;
        private FileInfo userInfoFile;
        private UserInfo currentUserInfo;
        private RootFolderInfo currentRootFolderInfo;
        private uint dwComponentId;
        private IManagedLockObject lockObject;
        private Queue<Action> uiThreadActions;
        private int hwndMain;
        private DebuggerState debuggerState;
        private TreeNode treeNodeBreakpointPaused;
        public IVsWindowFrame WindowFrame { get; internal set; }
        public DTE DTE { get; internal set; }
        public IVsStatusbar Statusbar { get; internal set; }
        public IOleComponentManager ComponentManager { get; internal set; }
        public IVsUIHierarchy VSUIHierarchy { get; internal set; }
        public IMenuCommandService MenuCommandService { get; internal set; }

        public HydraDebugToolWindowControl()
        {
            lockObject = LockManager.CreateObject();
            uiThreadActions = new Queue<Action>();

            InitializeComponent();

            treeView.SetTheme("explorer");
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)WindowsMessage.DESTROY)
            {
                Terminate();
            }
            else if (m.Msg == (int)WindowsMessage.COPYDATA)
            {
                IntPtr messagePtr;
                ControlExtensions.COPYDATASTRUCT copyDataStruct;
                int processId;
                IntPtr hwndClient;
                string json;
                CommandPacket commandPacket;

                copyDataStruct = Marshal.PtrToStructure<ControlExtensions.COPYDATASTRUCT>(m.LParam);
                messagePtr = copyDataStruct.lpData;

                json = Marshal.PtrToStringAnsi(messagePtr);
                commandPacket = JsonExtensions.ReadJson<CommandPacket>(json);

                switch (commandPacket.Command)
                {
                    case "AttachToProcess":

                        processId = (int)((string)commandPacket.Arguments.Single(a => a.Key == "ProcessId").Value).FromHexString();
                        hwndClient = (IntPtr)((string)commandPacket.Arguments.Single(a => a.Key == "ClientHwnd").Value).FromHexString();

                        AttachToProcess(processId, hwndClient);

                        break;
                    default:
                        DebugUtils.Break();
                        break;
                }

                m.Result = (IntPtr)1;
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private void AttachToProcess(int processId, IntPtr hwndClient)
        {
            this.Invoke(() =>
            {
                Process4 process;
                Transport transport;
                Debugger2 debugger2;
                Engine engine;
                Engine[] engines;
                IntPtr messagePtr;
                IntPtr lresult;
                ControlExtensions.COPYDATASTRUCT copyDataStruct;
                CommandPacket commandPacket;
                var command = "ProcessAttached";
                string json;

                commandPacket = new CommandPacket(command);
                json = commandPacket.ToJsonText();

                messagePtr = Marshal.StringToCoTaskMemAnsi(json + "\0");

                copyDataStruct = new ControlExtensions.COPYDATASTRUCT
                {
                    dwData = (IntPtr) 0,
                    cbData = json.Length + 1,
                    lpData = messagePtr
                };

                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

                process = this.DTE.Debugger.LocalProcesses.Cast<Process4>().Single(p => p.ProcessID == processId);
                debugger2 = (Debugger2)this.DTE.Debugger;
                transport = debugger2.Transports.Item("Default");
                engine = transport.Engines.Item("Managed (.NET 4.x)");
                engines = new[] { engine };

                this.Statusbar.SetText($"Request to attach to process. Attaching");

                process.Attach2(engines);

                lresult = ControlExtensions.SendMessage<ControlExtensions.COPYDATASTRUCT>(hwndClient, ControlExtensions.WindowsMessage.COPYDATA, 0, copyDataStruct);

                Marshal.FreeCoTaskMem(messagePtr);
            });
        }

        private void HydraDebugToolWindowControl_Load(object sender, EventArgs e)
        {
            var programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var generatorApp = Path.Combine(programFilesPath, @"\CloudIDEaaS\Hydra\ApplicationGenerator.exe");
#if !NOT_VS
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
#endif
            DTE.Events.DTEEvents.OnStartupComplete += DTEEvents_OnStartupComplete;

            WindowFrame = (IVsWindowFrame)ServiceProvider.GlobalProvider.GetService(typeof(SVsWindowFrame));

            if (!File.Exists(generatorApp))
            {
                programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                generatorApp = Path.Combine(programFilesPath, @"\CloudIDEaaS\Hydra\ApplicationGenerator.exe");
            }

            if (!File.Exists(generatorApp))
            {
                var solutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
                var applicationGeneratorPath = Path.Combine(solutionPath, @"ApplicationGenerator\bin\Debug\ApplicationGenerator.exe");

                generatorApp = applicationGeneratorPath;
            }

            if (File.Exists(generatorApp))
            {
                var configFile = Path.Combine(Path.GetDirectoryName(generatorApp), "ApplicationGenerator.exe.config");
                var document = XDocument.Load(configFile);
                var hydraDebugAssistantAddressElement = document.Root.XPathSelectElement("/configuration/appSettings/add[@key='DebugAssistantAddress']");
                var address = hydraDebugAssistantAddressElement.Attribute("value").Value;

                generatorsDirectory = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(generatorApp), "Generators"));

                if (generatorsDirectory.Exists)
                {
                    InitializeWindow(address);
                }
            }

            this.Dock = DockStyle.Fill;
        }

        private void InitializeWindow(string address)
        {
            txtDirectory.Text = generatorsDirectory.FullName;

            treeView.BeforeExpand += TreeView_BeforeExpand;
            treeView.BeforeCollapse += TreeView_BeforeCollapse;
            treeView.NodeMouseDoubleClick += TreeView_NodeMouseDoubleClick;
            treeView.MouseDown += TreeView_MouseDown;
            treeView.AfterSelect += TreeView_AfterSelect;
            treeView.KeyDown += TreeView_KeyDown;

            FillTree(treeView.Nodes, generatorsDirectory);

#if !NOT_VS
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
#endif

            try
            {
                namedPipeServer = new NamedPipeServer(address);
            }
            catch (Exception ex)
            {
                this.Statusbar.SetText($"Cannot create HydraDebugAssistance service at { address }. Error { ex.Message }");
            }

            try
            {
                namedPipeServer.OnCommand += Server_OnCommand;
                namedPipeServer.OnError += Server_OnError;
                namedPipeServer.OnConnectionCreated += Server_OnConnectionCreated;
                namedPipeServer.OnConnectionMade += Server_OnConnectionMade;
                namedPipeServer.OnDisconnect += Server_OnDisconnect;

                namedPipeServer.Start();
            }
            catch (Exception ex)
            {
                this.Statusbar.SetText($"Cannot start HydraDebugAssistance service at { address }. Error { ex.Message }");
            }

            try
            {
                int hr;
                var flags = (uint)_OLECRF.olecrfNeedIdleTime | (uint)_OLECRF.olecrfNeedPeriodicIdleTime;
                var adviseFlags = (uint)_OLECADVF.olecadvfRedrawOff | (uint)_OLECADVF.olecadvfWarningsOff;

                userInfoFile = new FileInfo(Path.Combine(generatorsDirectory.FullName, Environment.UserName + ".hui"));

                currentUserInfo = UserInfo.Read(userInfoFile);

                if (currentUserInfo.RootFolders.Any(f => f.Path == generatorsDirectory.FullName))
                {
                    var hasBreakpoint = false;

                    currentRootFolderInfo = currentUserInfo.RootFolders.Single(f => f.Path == generatorsDirectory.FullName);

                    foreach (var node in treeView.GetAllNodes().Where(n => ((TreeNodeInfo)n.Tag).FileSystemInfo is FileInfo && currentRootFolderInfo.Breakpoints.Any(b => b.FilePath.AsCaseless() == ((FileInfo)((TreeNodeInfo)n.Tag).FileSystemInfo).FullName)))
                    {
                        SetBreakpoint(node, true, true);
                        hasBreakpoint = true;
                    }

                    if (hasBreakpoint)
                    {
                        currentRootFolderInfo.DebuggerAttachedProcess = Process.GetCurrentProcess().Id.ToHexString(true);
                        currentUserInfo.Save(userInfoFile);
                    }
                }
                else
                {
                    currentRootFolderInfo = new RootFolderInfo(generatorsDirectory.FullName);

                    currentUserInfo.RootFolders.Add(currentRootFolderInfo);
                }

                dwComponentId = (uint)0;

                hr = this.ComponentManager.FRegisterComponent(this, new OLECRINFO[] { new OLECRINFO { cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO)), grfcrf = flags, grfcadvf = adviseFlags, uIdleTimeInterval = 1000 } }, out dwComponentId);
            }
            catch (Exception ex)
            {
                this.Statusbar.SetText($"Cannot read user info. Error { ex.Message }");
            }
        }

        private void TreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var treeNode = treeView.GetNodeAt(e.X, e.Y);

                if (treeNode != null)
                {
                    treeNode.SelectNode();
                }
            }
        }

        private void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                e.Node.SelectNode();
            }
        }

        private void DTEEvents_OnStartupComplete()
        {
            IDisposable disposable;

#if !NOT_VS
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
#endif
            hwndMain = DTE.MainWindow.HWnd;

            disposable = toolStrip.EnableClickOnActivate((IntPtr)hwndMain);

            toolStrip.HandleDestroyed += (sender, e) =>
            {
                disposable.Dispose();
            };
        }

        private void Invoke(Action action)
        {
            using (this.Lock())
            {
                uiThreadActions.Enqueue(action);
            }
        }

        public IDisposable Lock()
        {
            return lockObject.Lock();
        }

        public T LockReturn<T>(Func<T> func)
        {
            T returnVal;

            using (this.Lock())
            {
                returnVal = func();
            }

            return returnVal;
        }

        public void LockSet(Action action)
        {
            using (this.Lock())
            {
                action();
            }
        }

        private void Server_OnDisconnect(object sender, EventArgs<PipeServerConnection> e)
        {
            this.Invoke(() =>
            {
#if !NOT_VS
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
#endif

                this.Statusbar.SetText($"HydraDebugAssistance service connection closed.");

                SetBreakpointState();
            });
        }

        private void SetBreakpointState()
        {
            DebuggerState debuggerState;

            using (this.Lock())
            {
                debuggerState = this.debuggerState;
                treeNodeBreakpointPaused = null;
            }

            if (debuggerState == DebuggerState.BreakpointPaused)
            {
                var treeNode = treeView.GetAllNodes().Single(n => ((TreeNodeInfo)n.Tag).PausedOnBreakpoint);
                var treeNodeInfo = (TreeNodeInfo)treeNode.Tag;
                var fileInfo = (FileInfo)treeNodeInfo.FileSystemInfo;
                var nodeRect = treeNode.Bounds;
                var selectionRect = new Rectangle(0, nodeRect.Y, treeView.Width, nodeRect.Height);

                treeNodeInfo.PausedOnBreakpoint = false;

                this.Invoke(() =>
                {
#if !NOT_VS
                    Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
#endif

                    this.Statusbar.SetText(string.Empty);
                });

                treeView.Invalidate(selectionRect);
                treeView.Update();

                btnContinue.Enabled = true;
                btnRetry.Enabled = true;

                this.LockSet(() => this.debuggerState = DebuggerState.None);
            }
        }

        private void Server_OnConnectionMade(object sender, EventArgs<PipeServerConnection> e)
        {
            this.Invoke(() =>
            {
                var currentUserInfo = UserInfo.Read(userInfoFile);
                var currentRootFolderInfo = currentUserInfo.RootFolders.Single(f => f.Path == generatorsDirectory.FullName);
                var remoteProcessId = currentRootFolderInfo.DebuggerRemoteProcess;
                var remoteProcess = Process.GetProcessById((int)remoteProcessId.FromHexString(true));

                remoteProcess.Exited += RemoteProcess_Exited;
                this.LockSet(() => this.debuggerState = DebuggerState.None);

#if !NOT_VS
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
#endif

                this.Statusbar.SetText($"HydraDebugAssistance service connection made from client.");
            });
        }

        private void RemoteProcess_Exited(object sender, EventArgs e)
        {
            this.Invoke(() =>
            {
#if !NOT_VS
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
#endif

                this.Statusbar.SetText($"HydraDebugAssistance service connection lost.");

                SetBreakpointState();
            });
        }

        private void Server_OnConnectionCreated(object sender, EventArgs<PipeServerConnection> e)
        {
            this.Invoke(() =>
            {
#if !NOT_VS
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
#endif

                this.Statusbar.SetText($"HydraDebugAssistance service connection created, listening for clients.");
            });
        }

        private void Server_OnError(object sender, EventArgs<Exception> e)
        {
            var exception = e.Value;

            this.Invoke(() =>
            {
#if !NOT_VS
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
#endif

                this.Statusbar.SetText($"Error with HydraDebugAssistance service. Error: { exception.Message }");
            });
        }

        private void Server_OnCommand(object sender, EventArgs<CommandPacket> e)
        {
            var message = e.Value;

            switch (message.Command)
            {
                case Commands.IsBreakpointSet:

                    var templateFile = (string)message.Arguments.SingleOrDefault(a => a.Key == "TemplateFile").Value;
                    var breakpointSet = false;
                    var breakpointPaused = false;
                    var breakpointUnpaused = false;
                    DebuggerState debuggerState;

                    using (this.Lock())
                    {
                        breakpointSet = currentRootFolderInfo.Breakpoints.Any(b => b.Enabled && b.FilePath == templateFile);

                        debuggerState = this.debuggerState;

                        if (debuggerState == DebuggerState.None)
                        {
                            this.debuggerState = DebuggerState.BreakpointSet;
                        }
                        else if (debuggerState == DebuggerState.BreakpointPaused)
                        {
                            breakpointPaused = true;
                        }

                        debuggerState = this.debuggerState;
                    }

                    if (breakpointSet)
                    {
                        switch (debuggerState)
                        {
                            case DebuggerState.BreakpointSet:

                                this.LockSet(() => this.debuggerState = DebuggerState.BreakpointPaused);
                                namedPipeServer.Send(new CommandPacket(Commands.BreakpointSet));

                                break;

                            case DebuggerState.BreakpointPaused:

                                namedPipeServer.Send(new CommandPacket(Commands.BreakpointSet));
                                break;

                            case DebuggerState.RequestContinue:

                                namedPipeServer.Send(new CommandPacket(Commands.Continue));

                                this.LockSet(() =>
                                {
                                    this.debuggerState = DebuggerState.BreakpointSet;
                                    treeNodeBreakpointPaused = null;
                                });

                                breakpointUnpaused = true;
                                break;

                            case DebuggerState.RequestRetry:

                                namedPipeServer.Send(new CommandPacket(Commands.RetryAndBreak));

                                this.LockSet(() =>
                                {
                                    this.debuggerState = DebuggerState.BreakpointSet;
                                    treeNodeBreakpointPaused = null;
                                });

                                breakpointUnpaused = true;
                                break;

                        }

                        if (breakpointUnpaused)
                        {
                            this.Invoke(() =>
                            {
#if !NOT_VS
                                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
#endif

                                SetBreakpointUnpaused(templateFile);
                            });
                        }
                        else if (!breakpointPaused)
                        {
                            this.Invoke(() =>
                            {
#if !NOT_VS
                                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
#endif

                                SetBreakpointPaused(templateFile);
                            });
                        }
                    }
                    else
                    {
                        namedPipeServer.Send(new CommandPacket(Commands.BreakpointNotSet));
                    }

                    break;
            }
        }

        private void SetBreakpointPaused(string templateFile)
        {
            TreeNode treeNode;
            TreeNodeInfo treeNodeInfo;
            Rectangle nodeRect;
            Rectangle selectionRect;
            FileInfo fileInfo;
            bool result;

#if !NOT_VS
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
#endif

            treeNode = treeView.GetAllNodes().Single(n => ((TreeNodeInfo)n.Tag).FileSystemInfo is FileInfo && ((FileInfo)((TreeNodeInfo)n.Tag).FileSystemInfo).FullName == templateFile);
            treeNodeInfo = (TreeNodeInfo)treeNode.Tag;
            fileInfo = (FileInfo)treeNodeInfo.FileSystemInfo;

#if !NOT_VS
            try
            {
                this.DTE.ItemOperations.OpenFile(fileInfo.FullName);
            }
            catch
            {
            }
#endif
            treeNode.EnsureVisible();
            treeNode.SelectNode();

            this.DoEvents();

            nodeRect = treeNode.Bounds;
            selectionRect = new Rectangle(0, nodeRect.Y, treeView.Width, nodeRect.Height);

            treeNodeInfo.PausedOnBreakpoint = true;

            this.Statusbar.SetText($"Paused on breakpoint");

            treeView.Invalidate(selectionRect);
            treeView.Update();

#if !NOT_VS
            result = Utils.ControlExtensions.FlashWindow(new IntPtr(hwndMain), FlashWindowFlags.FLASHW_ALL | FlashWindowFlags.FLASHW_TIMERNOFG, (int)(int.MaxValue / 2) - 1, 2000);

            if (!result)
            {
                DebugUtils.Break();
            }
#endif
            btnContinue.Enabled = true;
            btnRetry.Enabled = true;

            treeNodeBreakpointPaused = treeNode;
        }

        private void SetBreakpointUnpaused(string templateFile)
        {
            TreeNode treeNode;
            TreeNodeInfo treeNodeInfo;
            Rectangle nodeRect;
            Rectangle selectionRect;
            FileInfo fileInfo;

#if !NOT_VS
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
#endif

            treeNode = treeView.GetAllNodes().Single(n => ((TreeNodeInfo)n.Tag).FileSystemInfo is FileInfo && ((FileInfo)((TreeNodeInfo)n.Tag).FileSystemInfo).FullName == templateFile);
            treeNodeInfo = (TreeNodeInfo)treeNode.Tag;
            fileInfo = (FileInfo)treeNodeInfo.FileSystemInfo;

            nodeRect = treeNode.Bounds;
            selectionRect = new Rectangle(0, nodeRect.Y, treeView.Width, nodeRect.Height);

            treeNodeInfo.PausedOnBreakpoint = false;

            this.Statusbar.SetText($"");

            treeView.Invalidate(selectionRect);
            treeView.Update();

            btnContinue.Enabled = true;
            btnRetry.Enabled = true;

            treeNodeBreakpointPaused = null;
        }

        private void TreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9)
            {
                var node = treeView.SelectedNode;

                ToggleBreakpoint(node);
            }
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var treeNode = e.Node;
            var treeNodeInfo = (TreeNodeInfo)treeNode.Tag;
            var fileSystemInfo = (FileSystemInfo)treeNodeInfo.FileSystemInfo;
            TreeNode treeNodeBreakpointPaused;
            DebuggerState debuggerState;

            using (this.Lock())
            {
                treeNodeBreakpointPaused = this.treeNodeBreakpointPaused;
                debuggerState = this.debuggerState;
            }

            if (fileSystemInfo is FileInfo)
            {
                if (!btnSetBreakpoint.Enabled)
                {
                    btnSetBreakpoint.Enabled = true;
                }
            }
            else
            {
                btnSetBreakpoint.Enabled = false;
            }

            btnSetBreakpoint.Checked = treeNodeInfo.BreakpointSet;

            if (treeNodeBreakpointPaused == treeNode)
            {
                btnContinue.Enabled = true;
                btnRetry.Enabled = false;
            }
            else
            {
                btnContinue.Enabled = false;
                btnRetry.Enabled = false;
            }
        }

        private void TreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var treeNodeInfo = (TreeNodeInfo)e.Node.Tag;
            var fileSystemInfo = (FileSystemInfo)treeNodeInfo.FileSystemInfo;

#if !NOT_VS
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
#endif

            if (fileSystemInfo is FileInfo fileInfo)
            {
                this.DTE.ItemOperations.OpenFile(fileInfo.FullName);
            }
        }

        private void TreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            var closedIndex = imageList.GetImageIndex("ClosedFolderIcon");

            e.Node.ImageIndex = closedIndex;
        }

        private void TreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var openIndex = imageList.GetImageIndex("OpenFolderIcon");

            e.Node.ImageIndex = openIndex;
        }

        private void FillTree(TreeNodeCollection treeNodeCollection, DirectoryInfo generatorsDirectory)
        {
            if (treeView.Nodes.Count == 0)
            {
                var index = imageList.GetImageIndex("OpenFolderIcon");
                var node = new TreeNode(generatorsDirectory.Name, index, index);

                node.Tag = new TreeNodeInfo(generatorsDirectory);
                treeNodeCollection.Add(node);

                treeNodeCollection = node.Nodes;

                this.DelayInvoke(100, () =>
                {
                    node.Expand();
                });
            }

            foreach (var directory in generatorsDirectory.GetDirectories())
            {
                var index = imageList.GetImageIndex("OpenFolderIcon");
                var node = new TreeNode(directory.Name, index, index);

                node.Tag = new TreeNodeInfo(directory);
                treeNodeCollection.Add(node);

                FillTree(node.Nodes, directory);

                node.Expand();
            }

            foreach (var file in generatorsDirectory.GetFiles("*.tt"))
            {
                var index = imageList.GetImageIndex("T4Icon");
                var node = new TreeNode(file.Name, index, index);

                node.Tag = new TreeNodeInfo(file);
                treeNodeCollection.Add(node);
            }
        }

        public void SetPane(IntPtr hwnd)
        {
            this.hwndPane = hwnd;
        }

        private void btnSetBreakpoint_Click(object sender, EventArgs e)
        {
            var node = treeView.SelectedNode;

            ToggleBreakpoint(node);
        }

        private void SetBreakpoint(TreeNode node, bool enabled, bool skipSave = false)
        {
            var treeNodeInfo = (TreeNodeInfo)node.Tag;
            var fileSystemInfo = (FileSystemInfo)treeNodeInfo.FileSystemInfo;

            if (fileSystemInfo is FileInfo fileInfo)
            {
                var breakPointRect = node.Bounds;
                var verticalScrollVisible = treeView.VerticalScrollVisible();
                var scrollBarWidth = 0;

                if (verticalScrollVisible)
                {
                    scrollBarWidth = SystemInformation.VerticalScrollBarWidth;
                }

                breakPointRect.Width = breakPointRect.Height;
                breakPointRect.Location = new Point(treeView.Width - scrollBarWidth - 26, breakPointRect.Top);

                btnSetBreakpoint.Checked = enabled;
                treeNodeInfo.BreakpointSet = btnSetBreakpoint.Checked;

                if (!skipSave)
                {
                    if (treeNodeInfo.BreakpointSet)
                    {
                        var breakPointInfo = new BreakpointInfo(fileInfo.FullName, true);

                        currentRootFolderInfo.Breakpoints.Add(breakPointInfo);
                    }
                    else
                    {
                        var breakPointInfo = currentRootFolderInfo.Breakpoints.Single(b => b.FilePath == fileInfo.FullName);

                        currentRootFolderInfo.Breakpoints.Remove(breakPointInfo);
                    }

                    currentRootFolderInfo.DebuggerAttachedProcess = Process.GetCurrentProcess().Id.ToHexString(true);
                    currentUserInfo.Save(userInfoFile);
                }

                if (!capturingTreeViewPaint)
                {
                    CaptureTreeViewPaint();
                }

                treeView.Invalidate(breakPointRect);
                treeView.Update();
            }
        }

        private void ToggleBreakpoint(TreeNode node)
        {
            var treeNodeInfo = (TreeNodeInfo)node.Tag;
            var fileSystemInfo = (FileSystemInfo)treeNodeInfo.FileSystemInfo;

            if (fileSystemInfo is FileInfo fileInfo)
            {
                var breakPointRect = node.Bounds;
                var verticalScrollVisible = treeView.VerticalScrollVisible();
                var scrollBarWidth = 0;

                if (verticalScrollVisible)
                {
                    scrollBarWidth = SystemInformation.VerticalScrollBarWidth;
                }

                breakPointRect.Width = breakPointRect.Height;
                breakPointRect.Location = new Point(treeView.Width - scrollBarWidth - 26, breakPointRect.Top);

                btnSetBreakpoint.Checked = !btnSetBreakpoint.Checked;
                treeNodeInfo.BreakpointSet = btnSetBreakpoint.Checked;

                if (treeNodeInfo.BreakpointSet)
                {
                    var breakPointInfo = new BreakpointInfo(fileInfo.FullName, true);

                    currentRootFolderInfo.Breakpoints.Add(breakPointInfo);
                }
                else
                {
                    var breakPointInfo = currentRootFolderInfo.Breakpoints.Single(b => b.FilePath == fileInfo.FullName);

                    currentRootFolderInfo.Breakpoints.Remove(breakPointInfo);
                }

                currentRootFolderInfo.DebuggerAttachedProcess = Process.GetCurrentProcess().Id.ToHexString(true);
                currentUserInfo.Save(userInfoFile);

                if (!capturingTreeViewPaint)
                {
                    CaptureTreeViewPaint();
                }

                treeView.Invalidate(breakPointRect);
                treeView.Update();
            }
        }

        private void CaptureTreeViewPaint()
        {
            capturingTreeViewPaint = true;

            treeView.GetMessages(m => true, (m) =>
            {
                if (m.Msg == (int)WindowsMessage.PAINT)
                {
                    var verticalScrollVisible = treeView.VerticalScrollVisible();
                    var scrollBarWidth = 0;

                    if (verticalScrollVisible)
                    {
                        scrollBarWidth = SystemInformation.VerticalScrollBarWidth;
                    }

                    using (var graphics = treeView.CreateGraphics())
                    {
                        foreach (var node in treeView.GetAllNodes().Where(n => ((TreeNodeInfo)n.Tag).BreakpointSet))
                        {
                            var treeNodeInfo = (TreeNodeInfo)node.Tag;
                            var nodeRect = node.Bounds;
                            var selectionRect = new Rectangle(0, nodeRect.Y, treeView.Width, nodeRect.Height);

                            if (treeNodeInfo.PausedOnBreakpoint)
                            {
                                var brush = new SolidBrush(treeView.ForeColor);
                                var font = new Font(treeView.Font, FontStyle.Bold);

                                graphics.FillRectangle(Brushes.LightYellow, selectionRect);
                                graphics.DrawString(node.Text, font, brush, nodeRect.X, nodeRect.Y);
                            }

                            nodeRect.Width = nodeRect.Height;
                            nodeRect.Location = new Point(treeView.Width - scrollBarWidth - 26, nodeRect.Top);

                            nodeRect.Inflate(-6, -6);

                            graphics.FillEllipse(Brushes.Red, nodeRect);
                            graphics.DrawEllipse(Pens.LightSalmon, nodeRect);
                        }
                    }
                }
            });
        }

        public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
        {
            return VSConstants.S_OK;
        }

        public int FPreTranslateMessage(Microsoft.VisualStudio.OLE.Interop.MSG[] pMsg)
        {
            return VSConstants.S_OK;
        }

        public void OnEnterState(uint uStateID, int fEnter)
        {
        }

        public void OnAppActivate(int fActive, uint dwOtherThreadID)
        {
        }

        public void OnLoseActivation()
        {
        }

        public void OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved)
        {
        }

        public int FDoIdle(uint grfidlef)
        {
            Action action = null;
            var actionCount = 0;

            using (this.Lock())
            {
                actionCount = uiThreadActions.Count;
            }

            while (actionCount > 0)
            {
                using (this.Lock())
                {
                    action = uiThreadActions.Dequeue();
                }

                action();

                using (this.Lock())
                {
                    actionCount = uiThreadActions.Count;
                }
            }

            return VSConstants.S_OK;
        }

        public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, Microsoft.VisualStudio.OLE.Interop.MSG[] pMsgPeeked)
        {
            return VSConstants.S_OK;
        }

        public int FQueryTerminate(int fPromptUser)
        {
            return 1;
        }

        public void Terminate()
        {
            if (currentRootFolderInfo != null)
            {
                if (currentRootFolderInfo.DebuggerAttachedProcess != null)
                {
                    currentRootFolderInfo.DebuggerAttachedProcess = Process.GetCurrentProcess().Id.ToHexString(true);
                    currentUserInfo.Save(userInfoFile);
                }

                namedPipeServer.Stop();
                currentRootFolderInfo = null;
            }
        }

        public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved)
        {
            return this.Handle;
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            this.LockSet(() => this.debuggerState = DebuggerState.RequestContinue);

            continueToolStripMenuItem.Enabled = false;
            btnContinue.Enabled = false;
            retryAndBreakToolStripMenuItem.Enabled = false;
            btnRetry.Enabled = false;

            this.DoEvents();
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            this.LockSet(() => this.debuggerState = DebuggerState.RequestRetry);

            retryAndBreakToolStripMenuItem.Enabled = false;
            btnRetry.Enabled = false;
            continueToolStripMenuItem.Enabled = false;
            btnContinue.Enabled = false;

            this.DoEvents();
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            this.DoEvents();

            setBreakpointToolStripMenuItem.Checked = btnSetBreakpoint.Checked;
            setBreakpointToolStripMenuItem.Enabled = btnSetBreakpoint.Enabled;
            continueToolStripMenuItem.Enabled = btnContinue.Enabled;
            retryAndBreakToolStripMenuItem.Enabled = btnRetry.Enabled;
        }

        internal void Showing()
        {
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
        }

        private void btnHideButDontClose_Click(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            this.WindowFrame.Hide();
            this.Show();
        }
    }
}
