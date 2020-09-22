using AbstraX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace PackageCacheStatus
{
    public partial class frmPackageCacheStatus : Form
    {
        private bool allowShowDisplay;
        private CacheStatusProperties cacheStatusProperties;
        private bool menuClose;
        private string toolsFolder;
        private string reportsFolder;
        private string sweepsFolder;
        private int lastLinesLength;
        private bool paused;
        private CacheStatusAgent cacheStatusAgent;
        private FolderViewService folderViewService;
        private string statusPrefix;
        private int lastSweepCount;
        //private IDisposable progressShow;
        private DateTime lastFolderFilterStatusTime;
        private DateTime lastSweepStart;
        private bool caching;
        private bool watchingCache;
        private IDisposable progressShow;
        private string clearCacheOriginalMenuText;
        private string clearWorkingFolderOriginalMenuText;
        private string deleteWorkingFolderOriginalMenuText;
        private string deleteSrcAndNodeModulesOriginalMenuText;
        private string killNodeOriginalMenuText;
        private int timerCounter;

        public frmPackageCacheStatus()
        {
            allowShowDisplay = Environment.CommandLine.Contains("/Show");
            cacheStatusProperties = new CacheStatusProperties();

            InitializeComponent();
            this.CreateHandle();

            menuStrip.EnableClickOnActivate();

            this.Text = this.GetCommonCaption();
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(allowShowDisplay ? value : allowShowDisplay);
        }

        private void frmPackageCacheStatus_Load(object sender, EventArgs e)
        {
            notifyIcon.RefreshTray();
            this.ShowInSecondaryMonitor();

            cacheStatusAgent = new CacheStatusAgent();
            cacheStatusAgent.OnCacheStatus += CacheStatusService_OnCacheStatus;
            cacheStatusAgent.OnInstallFromCacheStatus += CacheStatusAgent_OnInstallFromCacheStatus;

            clearCacheOriginalMenuText = clearCacheToolStripMenuItem.Text;
            clearWorkingFolderOriginalMenuText = clearWorkingFolderToolStripMenuItem.Text;
            deleteWorkingFolderOriginalMenuText = deleteWorkingFolderToolStripMenuItem.Text;
            deleteSrcAndNodeModulesOriginalMenuText = deleteSrcAndNodeModulesToolStripMenuItem.Text;
            killNodeOriginalMenuText = killNodeToolStripMenuItem.Text;

            cacheStatusAgent.Start();

            listViewFolders.AssignToImageList(columnHeaderName, columnHeaderDateModified, columnHeaderType);

            timerStatus.Start();
        }

        private void CacheStatusAgent_OnInstallFromCacheStatus(object sender, EventArgs<AbstraX.PackageCache.PackageInstallsFromCacheStatus> e)
        {
            this.Invoke(() =>
            {
                var status = e.Value;

                cacheStatusProperties.Total = status.Total;
                cacheStatusProperties.TotalRemaining = status.TotalRemaining;
                cacheStatusProperties.Requested = status.Requested;
                cacheStatusProperties.RequestedRemaining = status.RequestedRemaining;
                cacheStatusProperties.StatusSummary = status.StatusSummary;
                cacheStatusProperties.StatusText = status.StatusText;
            });
        }

        private void CacheStatusService_OnCacheStatus(object sender, EventArgs<AbstraX.PackageCache.PackageCacheStatus> e)
        {
            this.Invoke(() =>
            {
                var status = e.Value;

                cacheStatusProperties.AddingToCacheCount = status.AddingToCacheCount;
                cacheStatusProperties.ProcessingCount = status.ProcessingCount;
                cacheStatusProperties.AddedToCacheCount = status.AddedToCacheCount;
                cacheStatusProperties.WithErrorsCount = status.WithErrorsCount;
                cacheStatusProperties.InstallsFromCache = status.InstallsFromCache;
                cacheStatusProperties.CopiedToCache = status.CopiedToCache;
                cacheStatusProperties.InstallErrorsFromCache = status.InstallErrorsFromCache;
                cacheStatusProperties.StatusText = status.StatusText;
                cacheStatusProperties.CacheStatus = status.CacheStatus;
                cacheStatusProperties.InstallStatus = status.InstallStatus;
                cacheStatusProperties.LastUpdate = DateTime.Now == DateTime.MinValue ? string.Empty : DateTime.Now.ToDateTimeText();
                cacheStatusProperties.MemoryStatus = new MemoryStatusProperties(status.MemoryStatus);

                if (status.NoCaching)
                {
                    toolStripAlert1.Text = "NO CACHING";
                }
                else
                {
                    toolStripAlert1.Text = null;
                }

                if (status.NoInstallFromCache)
                {
                    toolStripAlert2.Text = "NO INSTALL FROM CACHE";
                }
                else
                {
                    toolStripAlert2.Text = null;
                }

                if (cacheStatusAgent.LastAttemptedUpdate != DateTime.MinValue)
                {
                }

                propertyGridStatus.RefreshProperties(cacheStatusProperties);

                if (status.CacheStatus == "SweepingFiles")
                {
                    var sweepIndex = status.SweepIndex;
                    var sweepCount = status.SweepCount;

                    lastSweepCount = sweepCount;
                    lastSweepStart = status.LastSweepStart;

                    if (progressShow == null)
                    {
                        progressShow = this.ShowProgress();
                    }

                    caching = true;

                    progressBar.Value = (int)(100f * (sweepIndex.As<float>() / sweepCount.As<float>()));

                    this.SetStatus("{0} {1} of {2} packages", status.CurrentActionVerb, sweepIndex, sweepCount);

                    statusStrip.Refresh();
                }
                else if (caching)
                {
                    this.SetTempStatus("Cached {0} packages", lastSweepCount);

                    statusStrip.Refresh();

                    if (progressShow != null)
                    {
                        progressShow.Dispose();
                        progressShow = null;
                    }

                    caching = false;
                }
            });
        }

        private void frmPackageCacheStatus_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (cacheStatusProperties.CacheStatus != null)
                {
                    var statusReportDirectory = new DirectoryInfo(Path.Combine(cacheStatusProperties.PackageCachePath, "reports"));
                    var statusReportFile = Path.Combine(statusReportDirectory.FullName, DateTime.Now.ToSortableDateTimeText() + ".json");
                    var jsonReport = cacheStatusProperties.ToJson();

                    if (!statusReportDirectory.Exists)
                    {
                        statusReportDirectory.Create();
                    }

                    File.WriteAllText(statusReportFile, jsonReport);
                }

                cacheStatusAgent.Stop();

                if (folderViewService != null)
                {
                    folderViewService.Stop();
                }
            }
            catch
            {
            }
        }

        private void frmPackageCacheStatus_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!menuClose)
            {
                e.Cancel = true;
                this.Visible = false;
            }
            else
            {
                this.SetStatus("Shutting down");
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        public string CurrentWorkingDirectory
        {
            get
            {
                return cacheStatusProperties.CurrentWorkingDirectory;
            }

            set
            {
                cacheStatusProperties.CurrentWorkingDirectory = value;
            }
        }

        public string PackageCachePath
        {
            get
            {
                return cacheStatusProperties.PackageCachePath;
            }

            set
            {
                cacheStatusProperties.PackageCachePath = value;
            }
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            var logPath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\hydra\cache\Log.txt");
            var registrySettings = new RegistrySettings();
            var doEvents = false;
            string logDirectoryPath;

            if (paused)
            {
                return;
            }

            registrySettings.Initialize();

            this.CurrentWorkingDirectory = registrySettings.CurrentWorkingDirectory;
            this.PackageCachePath = registrySettings.PackagePathCache;

            if (!this.PackageCachePath.IsNullOrEmpty())
            {
                logPath = Path.Combine(this.PackageCachePath, "Log.txt");
            }

            logDirectoryPath = Path.GetDirectoryName(logPath);

            if (Directory.Exists(logDirectoryPath))
            {
                this.PackageCachePath = logDirectoryPath;

                toolsFolder = Path.Combine(this.PackageCachePath, "tools");
                reportsFolder = Path.Combine(this.PackageCachePath, "reports");
                sweepsFolder = Path.Combine(this.PackageCachePath, "sweeps");

                if (!watchingCache)
                {
                    folderViewService = new FolderViewService(listViewFolders, logDirectoryPath);
                    folderViewService.Start();

                    watchingCache = true;
                }

                if (Directory.Exists(sweepsFolder))
                {
                    var directorySweeps = new DirectoryInfo(sweepsFolder);
                    var sweepLogFiles = directorySweeps.GetFiles();
                    var names = new List<string>();

                    if (sweepLogFiles.Length > 0)
                    {
                        if (!panelSweeps.Visible)
                        {
                            panelSweeps.Visible = true;
                            sweepsToolStripMenuItem.Checked = panelSweeps.Visible;
                        }

                        foreach (var sweepLogFile in sweepLogFiles.OrderBy(f => f.Name))
                        {
                            var regex = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})_(?<hour>\d{2})(?<minute>\d{2})(?<second>\d{2})_");

                            regex.Match(sweepLogFile.Name, (match) =>
                            {
                                var year = int.Parse(match.GetGroupValue("year"));
                                var month = int.Parse(match.GetGroupValue("month"));
                                var day = int.Parse(match.GetGroupValue("day"));
                                var hour = int.Parse(match.GetGroupValue("hour"));
                                var minute = int.Parse(match.GetGroupValue("minute"));
                                var second = int.Parse(match.GetGroupValue("second"));
                                var dateTime = new DateTime(year, month, day, hour, minute, second);
                                var name = dateTime.ToDateTimeText();

                                names.Add(name);

                                if (!listBoxSweeps.Items.Cast<SweepLogItem>().Any(i => i.Name == name))
                                {
                                    var logItem = new SweepLogItem(name, dateTime, sweepLogFile);

                                    listBoxSweeps.Items.Add(logItem);
                                    listBoxSweeps.ScrollToLast();
                                }
                            });
                        }
                    }

                    foreach (var logItemDelete in listBoxSweeps.Items.Cast<SweepLogItem>().ToList().Where(i => !names.Any(n => n == i.Name)))
                    {
                        listBoxSweeps.Items.Remove(logItemDelete);
                    }
                }
                else
                {
                    listBoxSweeps.Items.Clear();
                }
            }

            if (File.Exists(logPath))
            {
                IEnumerable<string> logLines = null;

                try
                {
                    using (var reader = new StreamReader(File.Open(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        var linesLength = richTextBoxStatus.Lines.Length;

                        if (linesLength > 0)
                        {
                            logLines = reader.ReadToEnd().GetLines().Skip(richTextBoxStatus.Lines.Length);

                            if (linesLength > lastLinesLength)
                            {
                            }

                            lastLinesLength = linesLength;
                        }
                        else
                        {
                            logLines = reader.ReadToEnd().GetLines();
                        }
                    }

                    if (logLines.Count() > 0)
                    {
                        richTextBoxStatus.AppendText(logLines.ToMultiLineList() + "\r\n");
                        richTextBoxStatus.ScrollToEnd();
                    }
                }
                catch (Exception ex)
                {
                    SetTempStatus($"Error with log file: { ex.Message }");
                }
            }

            if (!this.CurrentWorkingDirectory.IsNullOrEmpty())
            {
                if (timerCounter == 0)
                {
                    var directory = new DirectoryInfo(Path.Combine(this.CurrentWorkingDirectory, "src"));
                    var processes = Process.GetProcessesByName("node");
                    int count;

                    if (processes.Length > 0)
                    {
                        count = processes.Length;

                        killNodeToolStripMenuItem.Text = $"{ killNodeOriginalMenuText } ({ count })";
                    }
                    else
                    {
                        killNodeToolStripMenuItem.Text = killNodeOriginalMenuText;
                    }

                    if (directory.Exists)
                    {
                        try
                        {
                            count = directory.GetFileSystemInfos().Length;

                            directory = new DirectoryInfo(Path.Combine(this.CurrentWorkingDirectory, "node_modules"));
                            count += directory.GetFileSystemInfos().Length;

                            deleteSrcAndNodeModulesToolStripMenuItem.Text = $"{ deleteSrcAndNodeModulesOriginalMenuText } ({ count })";
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        deleteSrcAndNodeModulesToolStripMenuItem.Text = deleteSrcAndNodeModulesOriginalMenuText;
                    }

                    directory = new DirectoryInfo(this.CurrentWorkingDirectory);

                    if (directory.Exists)
                    {
                        try
                        {
                            count = directory.GetFileSystemInfos().Length;

                            deleteWorkingFolderToolStripMenuItem.Text = $"{ deleteWorkingFolderOriginalMenuText } ({ count })";
                            clearConfigPackagesToolStripMenuItem.Enabled = true;
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        deleteWorkingFolderToolStripMenuItem.Text = deleteWorkingFolderOriginalMenuText;
                    }

                    directory = new DirectoryInfo(this.PackageCachePath);

                    if (directory.Exists)
                    {
                        try
                        {
                            count = directory.GetFileSystemInfos().Length;

                            clearCacheToolStripMenuItem.Enabled = true;
                            clearCacheToolStripMenuItem.Text = $"{ clearCacheOriginalMenuText } ({ count })";
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        clearCacheToolStripMenuItem.Text = clearCacheOriginalMenuText;
                    }
                }

                if (!deleteSrcAndNodeModulesToolStripMenuItem.Enabled)
                {
                    deleteSrcAndNodeModulesToolStripMenuItem.Enabled = true;
                    doEvents = true;
                }

                if (!deleteWorkingFolderToolStripMenuItem.Enabled)
                {
                    deleteWorkingFolderToolStripMenuItem.Enabled = true;
                    doEvents = true;
                }

                if (!clearWorkingFolderToolStripMenuItem.Enabled)
                {
                    clearWorkingFolderToolStripMenuItem.Enabled = true;
                    doEvents = true;
                }
            }

            if (!clearCacheToolStripMenuItem.Enabled && !this.PackageCachePath.IsNullOrEmpty())
            {
                doEvents = true;
            }

            cacheStatusProperties.LastAttemptedUpdate = cacheStatusAgent.LastAttemptedUpdate == DateTime.MinValue ? string.Empty : cacheStatusAgent.LastAttemptedUpdate.ToDateTimeText();

            if (cacheStatusAgent.LastAttemptedUpdate != DateTime.MinValue)
            {
            }

            cacheStatusProperties.LastAttemptedError = cacheStatusAgent.LastAttemptedError;

            if (doEvents)
            {
                this.DelayInvoke(1, () =>
                {
                    this.DoEvents();
                });
            }

            timerCounter = ++timerCounter % 10;
        }

        internal void MenuClose()
        {
            menuClose = true;
            this.Close();
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.allowShowDisplay = true;
                this.Visible = !this.Visible;

                this.Activate();
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.allowShowDisplay = true;
            this.Visible = true;

            this.Activate();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.allowShowDisplay = true;
            this.Visible = !this.Visible;

            HideProgress();

            this.Activate();
        }

        private bool DeleteFolderContents(DirectoryInfo directory, out string errorMessage, bool deleteTopLevel = false, Func<FileSystemInfoStatus, bool> filter = null)
        {
            try
            {
                if (!directory.Exists)
                {
                    throw new IOException("Directory does not exist.");
                }

                directory.ForceDeleteAllFilesAndSubFolders(true, filter);

                if (deleteTopLevel)
                {
                    if (filter == null || filter(new FileSystemInfoStatus(directory)))
                    {
                        directory.Delete();
                    }
                }

                errorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        private void SetStatus(string value, bool isPrefix = false)
        {
            toolStripStatus.Text = value;

            if (isPrefix)
            {
                statusPrefix = value;
            }

            this.DoEvents();
        }

        private void AppendStatus(string value)
        {
            toolStripStatus.Text = statusPrefix + " " + value;
            this.DoEvents();
        }

        private void SetTempStatus(string value, Action action)
        {
            toolStripStatus.Text = value;
            this.DoEvents();

            this.DelayInvoke(5000, () =>
            {
                toolStripStatus.Text = string.Empty;
                action();
            });
        }

        public void SetTempStatus(string format, params object[] args)
        {
            toolStripStatus.Text = string.Format(format, args);
            this.DoEvents();

            this.DelayInvoke(5000, () =>
            {
                toolStripStatus.Text = string.Empty;
            });
        }

        private void SetTempStatus(string value)
        {
            toolStripStatus.Text = value;
            this.DoEvents();

            this.DelayInvoke(5000, () =>
            {
                toolStripStatus.Text = string.Empty;
            });
        }

        public void SetStatus(string format, params object[] args)
        {
            toolStripStatus.Text = string.Format(format, args);
            this.DoEvents();
        }

        private void ShowErrorMessage(string errorMessage)
        {
            MessageBox.Show(this, errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void HideProgress()
        {
            if (progressBar.Visible)
            {
                progressBar.Visible = false;
                progressBar.Value = 0;
            }
        }

        private IDisposable ShowProgress(bool delayHide = false)
        {
            progressBar.ProgressBar.Visible = true;

            return progressBar.AsDisposable(() =>
            {
                if (delayHide)
                {
                    this.DelayInvoke(3000, () =>
                    {
                        progressBar.Visible = false;
                    });
                }
                else
                {
                    progressBar.Visible = false;
                }
            });
        }

        private void clearCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var directory = new DirectoryInfo(this.PackageCachePath);
            string errorMessage = null;

            this.SetStatus("Clearing cache...", true);
            HideProgress();

            using (var waitCursor = WaitCursor.Wait())
            {
                using (ShowProgress())
                {
                    using (folderViewService.SuppressUpdate())
                    {
                        if (DeleteFolderContents(directory, out errorMessage, false, (f) => FolderFilter(directory, f)))
                        {
                            folderViewService.Update();

                            this.SetTempStatus("Cache cleared");
                        }
                        else
                        {
                            folderViewService.Update();

                            this.SetTempStatus("Cache cleared with errors");
                        }
                    }
                }
            }

            richTextBoxStatus.Clear();

            this.DoEvents();

            if (errorMessage != null)
            {
                ShowErrorMessage(errorMessage);
            }
        }

        private void deleteWorkingFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var directory = new DirectoryInfo(this.CurrentWorkingDirectory);
            string errorMessage = null;

            this.SetStatus("Deleting working folder...", true);
            HideProgress();

            using (var waitCursor = WaitCursor.Wait())
            {
                using (ShowProgress())
                {
                    if (DeleteFolderContents(directory, out errorMessage, true, (f) => FolderFilter(directory, f)))
                    {
                        progressBar.Value = 100;
                        statusStrip.Refresh();

                        this.SetTempStatus("Working folder deleted");
                    }
                    else
                    {
                        this.SetTempStatus("Working folder deleted with errors");
                    }
                }
            }

            if (errorMessage != null)
            {
                ShowErrorMessage(errorMessage);
            }
        }

        private void clearWorkingFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var directory = new DirectoryInfo(this.CurrentWorkingDirectory);
            string errorMessage = null;

            this.SetStatus("Clearing working folder...", true);
            HideProgress();

            using (var waitCursor = WaitCursor.Wait())
            {
                using (ShowProgress())
                {
                    if (DeleteFolderContents(directory, out errorMessage, false, (f) => FolderFilter(directory, f)))
                    {
                        progressBar.Value = 100;
                        statusStrip.Refresh();

                        this.SetTempStatus("Working folder deleted");
                    }
                    else
                    {
                        this.SetTempStatus("Working folder deleted with errors");
                    }
                }
            }

            if (errorMessage != null)
            {
                ShowErrorMessage(errorMessage);
            }
        }

        private void deleteSrcAndNodeModulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var directory = new DirectoryInfo(Path.Combine(this.CurrentWorkingDirectory, "src"));
            string errorMessage = null;
            string status = null;

            this.SetStatus("Deleting src folder...", true);
            HideProgress();

            using (var waitCursor = WaitCursor.Wait())
            {
                using (ShowProgress())
                {
                    if (DeleteFolderContents(directory, out errorMessage, true, (f) => FolderFilter(directory, f)))
                    {
                        progressBar.Value = 100;
                        statusStrip.Refresh();

                        status = "Folders deleted";
                    }
                    else
                    {
                        status = "Folders deleted with errors";
                    }
                }
            }

            this.SetStatus(status);

            if (errorMessage != null)
            {
                ShowErrorMessage(errorMessage);
            }

            directory = new DirectoryInfo(Path.Combine(this.CurrentWorkingDirectory, "node_modules"));

            this.SetStatus("Deleting node_modules folder...", true);

            using (var waitCursor = WaitCursor.Wait())
            {
                using (ShowProgress())
                {
                    if (!DeleteFolderContents(directory, out errorMessage, true, (f) => FolderFilter(directory, f)))
                    {
                        status = "Folders deleted with errors";
                    }
                }
            }

            this.SetTempStatus(status);

            if (errorMessage != null)
            {
                ShowErrorMessage(errorMessage);
            }
        }

        private bool FolderFilter(DirectoryInfo topLevelDirectory, FileSystemInfoStatus fileSystemInfoStatus)
        {
            var fileSystemInfo = fileSystemInfoStatus.FileSystemInfo;

            if (fileSystemInfo.FullName.StartsWith(toolsFolder))
            {
                return false;
            }
            else if (fileSystemInfo.FullName.StartsWith(reportsFolder))
            {
                return false;
            }
            else if (fileSystemInfo.Name == "hydra.json")
            {
                return false;
            }
            //else if (fileSystemInfo is DirectoryInfo)
            //{
            //    var directoryDelete = (DirectoryInfo)fileSystemInfo;

            //    if (topLevelDirectory.GetDirectories().Any(d => d.FullName.StartsWith(topLevelDirectory.FullName)))
            //    {
            //        toolStripStatus.Text = $"Deleting { directoryDelete.FullName }";
            //    }
            //}

            if (fileSystemInfoStatus.Count != -1)
            {
                var originalCount = fileSystemInfoStatus.OriginalCount;
                var count = fileSystemInfoStatus.Count;
                var now = DateTime.Now;
                var countFloat = (float)count;
                var originalFloat = (float)originalCount;
                var progressValue = 100 - (int)(100f * (countFloat / originalFloat));
                var secondsDelay = (int)(countFloat / 1000f) + 1;

                if (now - lastFolderFilterStatusTime > TimeSpan.FromSeconds(secondsDelay))
                {
                    var deleted = originalCount - count;

                    lastFolderFilterStatusTime = now;

                    if (deleted > 0)
                    {
                        this.AppendStatus(string.Format("{0} of {1} directories", deleted, originalCount));
                    }

                    progressBar.Value = progressValue;
                    statusStrip.Refresh();
                }
            }

            return true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HideProgress();
            MenuClose();
        }

        private void listViewFolders_DoubleClick(object sender, EventArgs e)
        {
            var selectedItem = listViewFolders.SelectedItems.Cast<ListViewItem>().Last();

            selectedItem.OpenDefault();
        }

        private void toolStripMenuItemPause_Click(object sender, EventArgs e)
        {
            paused = toolStripMenuItemPause.Checked;
        }

        private void clearWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HideProgress();
            richTextBoxStatus.Clear();
        }

        private void sweepsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HideProgress();
            panelSweeps.Visible = !panelSweeps.Visible;
            sweepsToolStripMenuItem.Checked = panelSweeps.Visible;
        }

        private void listBoxSweeps_SelectedIndexChanged(object sender, EventArgs e)
        {
            var sweepLogItem = (SweepLogItem) listBoxSweeps.SelectedItem;

            richTextBoxSweep.Text = File.ReadAllText(sweepLogItem.LogFile.FullName);

            richTextBoxSweep.Visible = true;
            richTextBoxSweep.BringToFront();
        }

        private void listBoxSweeps_Leave(object sender, EventArgs e)
        {
            richTextBoxSweep.SendToBack();
            richTextBoxSweep.Visible = false;
        }

        private void killNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var processes = Process.GetProcessesByName("node");

            HideProgress();

            foreach (var process in processes)
            {
                process.Kill();
            }
        }

        private void killBuildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var processes = Process.GetProcessesByName("ApplicationGenerator");

            HideProgress();

            foreach (var process in processes)
            {
                process.Kill();
            }

            processes = Process.GetProcessesByName("powershell");

            foreach (var process in processes)
            {
                process.Kill();
            }

            processes = Process.GetProcessesByName("cmd");

            foreach (var process in processes)
            {
                process.Kill();
            }
        }

        private void deleteLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var logPath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\hydra\cache\Log.txt");

            HideProgress();
            this.SetStatus("Deleting log file...", true);

            using (var waitCursor = WaitCursor.Wait())
            {
                try
                {
                    File.Delete(logPath);
                }
                catch
                {
                    Thread.Sleep(100);
                    File.Delete(logPath);
                }

                folderViewService.Update();
            }

            richTextBoxStatus.Clear();
            this.DoEvents();

            this.SetTempStatus("Log file deleted", true);
        }

        private void clearSweepsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var directory = new DirectoryInfo(Path.Combine(this.PackageCachePath, "sweeps"));
            string errorMessage = null;

            HideProgress();
            this.SetStatus("Clearing sweeps...", true);

            if (directory.Exists)
            {
                using (var waitCursor = WaitCursor.Wait())
                {
                    using (ShowProgress())
                    {
                        using (folderViewService.SuppressUpdate())
                        {
                            if (DeleteFolderContents(directory, out errorMessage, false, (f) => FolderFilter(directory, f)))
                            {
                                folderViewService.Update();

                                panelSweeps.Visible = false;
                                sweepsToolStripMenuItem.Checked = false;

                                this.SetTempStatus("Sweeps cleared");
                            }
                            else
                            {
                                folderViewService.Update();

                                this.SetTempStatus("Sweeps cleared with errors");
                            }
                        }
                    }
                }
            }

            if (errorMessage != null)
            {
                ShowErrorMessage(errorMessage);
            }
        }

        private void clearConfigPackagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var disposableRichTextBox = richTextBoxStatus.Wait();
            var disposableThis = this.Wait();

            HideProgress();

            this.ClearConfigPackages(() =>
            {
                this.ClearConfigPackages(() => 
                {
                    disposableRichTextBox.Dispose();
                    disposableThis.Dispose();

                }, true);
            });
        }

        private void ClearConfigPackages(Action postAction, bool saveDev = false)
        {
            Process process;
            string exe;
            string arguments;
            ProcessStartInfo startInfo;
            int exitCode;
            var output = string.Empty;
            var error = string.Empty;
            var hydraSolutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%")).ForwardSlashes();
            var directory = new DirectoryInfo(this.CurrentWorkingDirectory);

            exe = Environment.ExpandEnvironmentVariables(@"%ProgramW6432%\nodejs\node.exe");

            if (saveDev)
            {
                arguments = string.Format("{0}/HydraCli/HydraCli/out/cli.js {1}", hydraSolutionPath, "setPackages --save-dev --clear");
            }
            else
            {
                arguments = string.Format("{0}/HydraCli/HydraCli/out/cli.js {1}", hydraSolutionPath, "setPackages --clear");
            }

            startInfo = new ProcessStartInfo(exe, arguments);

            process = new Process();

            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.WorkingDirectory = directory.FullName;

            process.StartInfo = startInfo;

            if (Keys.ControlKey.IsPressed())
            {
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = true;
            }
            else
            {
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
            }


            process.OutputDataReceived += (s, e2) =>
            {
                output += e2.Data + "\r\n";
            };

            process.ErrorDataReceived += (s, e2) =>
            {
                error += e2.Data + "\r\n";
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            exitCode = process.ExitCode;

            error = error.Trim();
            output = output.Trim();

            if (exitCode == 0)
            {
                SetTempStatus(output.RemoveStartEnd(5), postAction);
            }
            else
            {
                ShowErrorMessage(error.RemoveStartEnd(5));
                postAction();
            }
        }
    }
}
