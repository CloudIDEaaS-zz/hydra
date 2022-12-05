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
using Decompiler.Core.Assemblers;
using Decompiler.Core.Configuration;
using Decompiler.Core.Serialization;
using Decompiler.Core.Services;
using Decompiler.Core.Types;
using Decompiler.Gui.Windows;
using Decompiler.Gui.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Forms
{
    /// <summary>
    /// Provices a component Container implementation, and specifically handles interactions 
    /// with the MainForm. This decouples platform-specific code from the user interaction 
    /// code. This will make it easier to port to other GUI platforms.
    /// </summary>
    public class MainFormInteractor :
        ICommandTarget,
        DecompilerHost,
        IStatusBarService
    {
        private IMainForm form;
        private IDecompilerService decompilerSvc;
        private IDecompilerShellUiService uiSvc;
        private IDiagnosticsService diagnosticsSvc;
        private ISearchResultService srSvc;
        private IWorkerDialogService workerDlgSvc;
        private IProjectBrowserService projectBrowserSvc;
        private IDialogFactory dlgFactory;
        private ITabControlHostService searchResultsTabControl;
        private ILoader loader;

        private IPhasePageInteractor currentPhase;
        private InitialPageInteractor pageInitial;
        private ILoadedPageInteractor pageLoaded;
        private IAnalyzedPageInteractor pageAnalyzed;
        private IFinalPageInteractor pageFinal;
        private Dictionary<IPhasePageInteractor, IPhasePageInteractor> nextPage;

        private MruList mru;
        private DecompilerMenus dm;
        private string projectFileName;
        private IServiceContainer sc;
        private IConfigurationService config;
        private ICommandTarget subWindowCommandTarget;
        private static string dirSettings;

        private const int MaxMruItems = 9;

        public MainFormInteractor(IServiceProvider services)
        {
            this.dlgFactory = services.RequireService<IDialogFactory>();
            this.mru = new MruList(MaxMruItems);
            this.mru.Load(MruListFile);
            this.sc = services.RequireService<IServiceContainer>();
            this.nextPage = new Dictionary<IPhasePageInteractor, IPhasePageInteractor>();
        }

        public IServiceProvider Services { get { return sc; } }

        private void CreatePhaseInteractors(IServiceFactory svcFactory)
        {
            pageInitial =  svcFactory.CreateInitialPageInteractor();
            pageLoaded = svcFactory.CreateLoadedPageInteractor();
            pageAnalyzed = new AnalyzedPageInteractorImpl(sc);
            pageFinal = new FinalPageInteractor(sc);

            nextPage[pageInitial] = pageLoaded;
            nextPage[pageLoaded] = pageAnalyzed;
            nextPage[pageAnalyzed] = pageFinal;
        }

        public virtual IDecompiler CreateDecompiler(ILoader ldr)
        {
            return new DecompilerDriver(ldr, this, sc);
        }

        public IMainForm LoadForm()
        {
            this.form = dlgFactory.CreateMainForm();

            dm = new DecompilerMenus(this);
            form.Menu = dm.MainMenu;
            dm.MainToolbar.Text = "";
            dm.MainToolbar.ImageList = form.ImageList;
            form.AddToolbar(dm.MainToolbar);

            var svcFactory = sc.RequireService<IServiceFactory>();
            CreateServices(svcFactory, sc, dm);
            CreatePhaseInteractors(svcFactory);

            form.Load += this.MainForm_Loaded;
            form.Closed += this.MainForm_Closed;
            form.ProcessCommandKey += this.MainForm_ProcessCommandKey;

            form.ToolBar.ItemClicked += toolBar_ItemClicked;
            //form.InitialPage.IsDirtyChanged += new EventHandler(InitialPage_IsDirtyChanged);//$REENABLE
            //MainForm.InitialPage.IsDirty = false;         //$REENABLE

            return form;
        }

        private void CreateServices(IServiceFactory svcFactory, IServiceContainer sc, DecompilerMenus dm)
        {
            config = svcFactory.CreateDecompilerConfiguration();
            sc.AddService(typeof(IConfigurationService), config);

            sc.AddService(typeof(IStatusBarService), (IStatusBarService)this);

            diagnosticsSvc = svcFactory.CreateDiagnosticsService(form.DiagnosticsList);
            sc.AddService(typeof(IDiagnosticsService), diagnosticsSvc);

            decompilerSvc = svcFactory.CreateDecompilerService();
            sc.AddService(typeof(IDecompilerService), decompilerSvc);

            uiSvc = svcFactory.CreateShellUiService(form, dm);
            subWindowCommandTarget = uiSvc;
            sc.AddService(typeof(IDecompilerShellUiService), uiSvc);
            sc.AddService(typeof(IDecompilerUIService), uiSvc);

            var codeViewSvc = new CodeViewerServiceImpl(sc);
            sc.AddService(typeof(ICodeViewerService), codeViewSvc);
            var segmentViewSvc = new ImageSegmentServiceImpl(sc);
            sc.AddService(typeof(ImageSegmentService), segmentViewSvc);

            var del = svcFactory.CreateDecompilerEventListener();
            workerDlgSvc = (IWorkerDialogService)del;
            sc.AddService(typeof(IWorkerDialogService), workerDlgSvc);
            sc.AddService(typeof(DecompilerEventListener), del);

            loader = svcFactory.CreateLoader();
            sc.AddService(typeof(ILoader), loader);

            var abSvc = svcFactory.CreateArchiveBrowserService();
            sc.AddService(typeof(IArchiveBrowserService), abSvc);

            sc.AddService(typeof(ILowLevelViewService), svcFactory.CreateMemoryViewService());
            sc.AddService(typeof(IDisassemblyViewService), svcFactory.CreateDisassemblyViewService());

            var tlSvc = svcFactory.CreateTypeLibraryLoaderService();
            sc.AddService(typeof(ITypeLibraryLoaderService), tlSvc);

            this.projectBrowserSvc = svcFactory.CreateProjectBrowserService(form.ProjectBrowser);
            sc.AddService<IProjectBrowserService>(projectBrowserSvc);

            var upSvc = svcFactory.CreateUiPreferencesService();
            sc.AddService<IUiPreferencesService>(upSvc);

            var fsSvc = svcFactory.CreateFileSystemService();
            sc.AddService<IFileSystemService>(fsSvc);

            this.searchResultsTabControl = svcFactory.CreateTabControlHost(form.TabControl);
            sc.AddService<ITabControlHostService>(this.searchResultsTabControl);

            srSvc = svcFactory.CreateSearchResultService(form.FindResultsList);
            sc.AddService<ISearchResultService>(srSvc);
            searchResultsTabControl.Attach((IWindowPane) srSvc, form.FindResultsPage);
            searchResultsTabControl.Attach((IWindowPane) diagnosticsSvc, form.DiagnosticsPage);
        }

        public virtual TextWriter CreateTextWriter(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return StreamWriter.Null;
            return new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write), new UTF8Encoding(false));
        }

        public IPhasePageInteractor CurrentPhase
        {
            get { return currentPhase; }
            set { currentPhase = value; }
        }

        public IMainForm MainForm
        {
            get { return form; }
        }

        public void OpenBinary(string file)
        {
            OpenBinary(file, (f) => pageInitial.OpenBinary(f, this));
        }

        /// <summary>
        /// Master function for opening a new project.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="openAction"></param>
        public void OpenBinary(string file, Func<string,bool> openAction)
        {
            try
            {
                CloseProject();
                SwitchInteractor(InitialPageInteractor);
                if (openAction(file))
                {
                    ProjectFileName = file;
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Caught exeption: {0}\r\n{1}", ex.Message, ex.StackTrace);
                uiSvc.ShowError(ex, "Couldn't open file '{0}'.", file);
            }
        }

        public void OpenBinaryWithPrompt()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                if (form.ShowDialog(form.OpenFileDialog) == DialogResult.OK)
                {
                    mru.Use(form.OpenFileDialog.FileName);
                    OpenBinary(form.OpenFileDialog.FileName, (filename) =>
                        pageInitial.OpenBinary(filename, this));
                }
            }
            finally
            {
                Cursor.Current = Cursors.Arrow;
                form.SetStatus("");
            }
        }

        public void AddMetadataFile()
        {
            var fileName = uiSvc.ShowOpenFileDialog(null);
            if (fileName == null)
                return;
            mru.Use(fileName);
            var typelib = decompilerSvc.Decompiler.LoadMetadata(fileName);
            decompilerSvc.Decompiler.Project.MetadataFiles.Add(new MetadataFile {
                Filename = fileName,
                ModuleName = Path.GetFileName(fileName),
                TypeLibrary = typelib
            });
        }

        public bool AssembleFile()
        {
            IAssembleFileDialog dlg = null;
            try
            {
                dlg = dlgFactory.CreateAssembleFileDialog();
                dlg.Services = sc;
                if (uiSvc.ShowModalDialog(dlg) != DialogResult.OK)
                    return true;
                mru.Use(dlg.FileName.Text);

                var typeName = dlg.SelectedArchitectureTypeName;
                var t = Type.GetType(typeName, true);
                var asm = (Assembler) t.GetConstructor(Type.EmptyTypes).Invoke(null);
                OpenBinary(dlg.FileName.Text, (f) => pageInitial.Assemble(f, asm, this));
            }
            catch (Exception e)
            {
                uiSvc.ShowError(e, "An error occurred while assembling {0}.", dlg.FileName.Text);
            }
            return true;
        }

        public bool OpenBinaryAs()
        {
            IOpenAsDialog dlg = null;
            IProcessorArchitecture arch = null;
            Platform platform = null;
            try
            {
                dlg = dlgFactory.CreateOpenAsDialog();
                dlg.Services = sc;
                if (uiSvc.ShowModalDialog(dlg) != DialogResult.OK)
                    return true;

                mru.Use(dlg.FileName.Text);

                var typeName = (string) ((ListOption) dlg.Architectures.SelectedValue).Value;
                Type t = Type.GetType(typeName, true);
                arch = (IProcessorArchitecture)Activator.CreateInstance(t);

                typeName = (string) ((ListOption) dlg.Platforms.SelectedValue).Value;
                t = Type.GetType(typeName);
                if (t == null)
                    throw new TypeLoadException(string.Format("Unable to load type {0}.", typeName));
                platform = (Platform) Activator.CreateInstance(t, sc, arch);

                Address addrBase;
                var sAddr = dlg.AddressTextBox.Text.Trim();
                if (!arch.TryParseAddress(sAddr, out addrBase))
                    throw new ApplicationException(string.Format("'{0}' doesn't appear to be a valid address.", sAddr));
                OpenBinary(dlg.FileName.Text, (f) =>
                    pageInitial.OpenBinaryAs(
                        f,
                        arch,
                        platform,
                        addrBase,
                        this));
            }
            catch (Exception ex)
            {
                uiSvc.ShowError(
                    ex,
                    string.Format("An error occurred when opening the binary file {0}.", dlg.FileName.Text));
            }
            return true;
        }

        public void CloseProject()
        {
            if (decompilerSvc.Decompiler != null && decompilerSvc.Decompiler.Project != null)
            {
                if (uiSvc.Prompt("Do you want to save any changes made to the decompiler project?"))
                {
                    if (!Save())
                        return;
                }
            }

            CloseAllDocumentWindows();
            sc.RequireService<IProjectBrowserService>().Clear();
            diagnosticsSvc.ClearDiagnostics();
            decompilerSvc.Decompiler = null;
        }

        private void CloseAllDocumentWindows()
        {
            foreach (var frame in uiSvc.DocumentWindows.ToArray())
            {
                frame.Close();
            }
        }

        public InitialPageInteractor InitialPageInteractor
        {
            get { return pageInitial; }
        }

        public ILoadedPageInteractor LoadedPageInteractor
        {
            get { return pageLoaded; }
        }

        public IAnalyzedPageInteractor AnalyzedPageInteractor
        {
            get { return pageAnalyzed; }
        }

        public IFinalPageInteractor FinalPageInteractor
        {
            get { return pageFinal; }
        }

        private static string MruListFile
        {
            get { return SettingsDirectory + "\\mru.txt"; }
        }

        public void RestartRecompilation()
        {
            if (decompilerSvc.Decompiler == null ||
                decompilerSvc.Decompiler.Project == null)
                return;

            SwitchInteractor(this.InitialPageInteractor);
            CloseAllDocumentWindows();
        }

        public void NextPhase()
        {
            try
            {
                IPhasePageInteractor next;
                if (nextPage.TryGetValue(CurrentPhase, out next))
                {
                    SwitchInteractor(next);
                }
            }
            catch (Exception ex)
            {
                uiSvc.ShowError(ex, "Unable to proceed.");
            }
            workerDlgSvc.FinishBackgroundWork();
        }

        public void FinishDecompilation()
        {
            try
            {
                IPhasePageInteractor prev = CurrentPhase;
                workerDlgSvc.StartBackgroundWork("Finishing decompilation.", delegate()
                {
                    IPhasePageInteractor next;
                    while (nextPage.TryGetValue(prev, out next))
                    {
                        next.PerformWork(workerDlgSvc);
                        prev = next;
                    }
                });
                prev.EnterPage();
            }
            catch (Exception ex)
            {
                uiSvc.ShowError(ex, "An error occurred while finishing decompilation.");
            }
            workerDlgSvc.FinishBackgroundWork();
        }

        public void LayoutMdi(DocumentWindowLayout layout)
        {
            form.LayoutMdi(layout);
        }

        public void ShowAboutBox()
        {
            using (AboutDialog dlg = new AboutDialog())
            {
                uiSvc.ShowModalDialog(dlg);
            }
        }

        public string ProjectFileName
        {
            get { return projectFileName; }
            set { projectFileName = value; }
        }

        public void EditFind()
        {
            using (ISearchDialog dlg = dlgFactory.CreateSearchDialog())
            {
                if (uiSvc.ShowModalDialog(dlg) == DialogResult.OK)
                {
                    Func<int, Program, bool> filter = GetScannedFilter(dlg);
                    var re = Scanning.Dfa.Automaton.CreateFromPattern(dlg.Patterns.Text);
                    if (re == null)
                        return;
                    var hits = this.decompilerSvc.Decompiler.Project.Programs
                        .SelectMany(program => 
                              re.GetMatches(program.Image.Bytes, 0)
                              .Where(o => filter(o, program))
                                .Select(offset => new AddressSearchHit 
                                {
                                    Program = program,
                                    Address = program.Image.BaseAddress + offset
                                }));
                    srSvc.ShowSearchResults(new AddressSearchResult(
                        this.sc,
                        hits));
                }
            }
        }

        private Func<int, Program, bool> GetScannedFilter(ISearchDialog dlg)
        {
            if (dlg.ScannedMemory.Checked)
            {
                if (dlg.UnscannedMemory.Checked)
                    return (o, map) => true;
                else
                    return (o, program) =>
                    {
                        var addr = program.ImageMap.MapLinearAddressToAddress(
                            (ulong)
                             ((long)program.Image.BaseAddress.ToLinear() + o));
                        ImageMapItem item;
                        return program.ImageMap.TryFindItem(addr, out item)
                            && item.DataType != null &&
                            !(item.DataType is UnknownType);
                    };
            }
            else if (dlg.UnscannedMemory.Checked)
            {
                return (o, program) =>
                    {
                        var addr = program.ImageMap.MapLinearAddressToAddress(
                              (uint)((long) program.Image.BaseAddress.ToLinear() + o));
                        ImageMapItem item;
                        return program.ImageMap.TryFindItem(addr, out item)
                            && item.DataType == null ||
                            item.DataType is UnknownType;
                    };
            }
            else
                throw new NotSupportedException();
        }

        public void FindProcedures(ISearchResultService svc)
        {
            var hits = this.decompilerSvc.Decompiler.Project.Programs
                .SelectMany(program => program.Procedures.Select(proc =>
                    new ProcedureSearchHit(program, proc.Key, proc.Value)))
                .ToList();
            svc.ShowSearchResults(new ProcedureSearchResult(this.sc, hits));
        }

        public void ViewDisassemblyWindow()
        {
            var dasmService = sc.GetService<IDisassemblyViewService>();
            dasmService.ShowWindow();
        }

        public void ViewMemoryWindow()
        {
            var memService = sc.GetService<ILowLevelViewService>();
            //$TODO: determine "current program".
            memService.ViewImage(this.decompilerSvc.Decompiler.Project.Programs.First());
            memService.ShowWindow();
        }

        /// <summary>
        /// Saves the project. 
        /// </summary>
        /// <returns>False if the user cancelled the save, true otherwise.</returns>
        public bool Save()
        {
            if (string.IsNullOrEmpty(this.ProjectFileName))
            {
                string newName = PromptForFilename(
                    Path.ChangeExtension(
                        decompilerSvc.Decompiler.Project.Programs[0].Filename,
                        Project_v1.FileExtension));
                if (newName == null)
                    return false;
                ProjectFileName = newName;
                mru.Use(newName);
            }

            using (TextWriter sw = CreateTextWriter(ProjectFileName))
            {
                //$REFACTOR: rule of Demeter, push this into a Save() method.
                var sp =  new ProjectSaver().Save(decompilerSvc.Decompiler.Project);
                new ProjectLoader(loader).Save(sp, sw);
            }
            return true;
        }

        protected virtual string PromptForFilename(string suggestedName)
        {
            form.SaveFileDialog.FileName = suggestedName;
            if (DialogResult.OK != form.ShowDialog(form.SaveFileDialog))
                return null;
            else
                return form.SaveFileDialog.FileName;
        }

        private static string SettingsDirectory
        {
            get
            {
                if (dirSettings == null)
                {
                    string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\jkl\\grovel";
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    dirSettings = dir;
                }
                return dirSettings;
            }
        }

        public void SwitchInteractor(IPhasePageInteractor interactor)
        {
            if (interactor == CurrentPhase)
                return;

            if (CurrentPhase != null)
            {
                if (!CurrentPhase.LeavePage())
                    return;
            }
            CurrentPhase = interactor;
            workerDlgSvc.StartBackgroundWork("Entering next phase...", delegate()
            {
                interactor.PerformWork(workerDlgSvc);
            });
            interactor.EnterPage();
        }

        public void UpdateWindowTitle()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(decompilerSvc.ProjectName))
            {
                sb.Append(decompilerSvc.ProjectName);
                //$REFACTOR: dirtiness of project is not limited to first page.
                //if (MainForm.InitialPage.IsDirty)
                //    sb.Append('*');
                sb.Append(" - ");
            }
            sb.Append("Decompiler");
            MainForm.TitleText = sb.ToString();
        }

        #region ICommandTarget members

        /// <summary>
        /// Determines a command target that should be handling commands. This 
        /// is in essence the "router" that routes commands.
        /// </summary>
        /// <returns></returns>
        private ICommandTarget GetSubCommandTarget()
        {
            if (form.TabControl.ContainsFocus)
                return searchResultsTabControl;
            if (form.ProjectBrowser.Focused)
                return projectBrowserSvc;
            return subWindowCommandTarget;
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus cmdStatus, CommandText cmdText)
        {
            var ct = GetSubCommandTarget();
            if (ct != null && ct.QueryStatus(cmdId, cmdStatus, cmdText))
                return true;
            
            if (currentPhase != null && currentPhase.QueryStatus(cmdId, cmdStatus, cmdText))
                return true;
            if (cmdId.Guid == CmdSets.GuidDecompiler)
            {
                if (QueryMruItem(cmdId.ID, cmdStatus, cmdText))
                    return true;

                switch (cmdId.ID)
                {
                case CmdIds.FileOpen:
                case CmdIds.FileExit:
                case CmdIds.FileOpenAs:
                case CmdIds.FileAssemble:
                case CmdIds.WindowsCascade: 
                case CmdIds.WindowsTileVertical:
                case CmdIds.WindowsTileHorizontal:
                case CmdIds.WindowsCloseAll: 
                case CmdIds.HelpAbout: 
                    cmdStatus.Status = MenuStatus.Enabled | MenuStatus.Visible;
                    return true;
                case CmdIds.FileMru:
                    cmdStatus.Status = MenuStatus.Visible;
                    return true;
                case CmdIds.ActionRestartDecompilation:
                    cmdStatus.Status = MenuStatus.Enabled | MenuStatus.Visible;
                    break;
                case CmdIds.ActionNextPhase:
                    cmdStatus.Status = currentPhase.CanAdvance
                        ? MenuStatus.Enabled | MenuStatus.Visible
                        : MenuStatus.Visible;
                    return true;
                case CmdIds.FileAddBinary:
                case CmdIds.FileAddMetadata:
                case CmdIds.FileCloseProject:
                case CmdIds.ViewFindAllProcedures:
                case CmdIds.FileSave:
                case CmdIds.EditFind:
                    cmdStatus.Status = IsDecompilerLoaded
                        ? MenuStatus.Enabled | MenuStatus.Visible
                        : MenuStatus.Visible;
                    return true;
                }
            }
            return false;
        }

        private bool QueryMruItem(int cmdId, CommandStatus cmdStatus, CommandText cmdText)
        {
            int iMru = cmdId - CmdIds.FileMru;
            if (0 <= iMru && iMru < mru.Items.Count)
            {
                cmdStatus.Status = MenuStatus.Visible | MenuStatus.Enabled;
                cmdText.Text = string.Format("&{0} {1}", iMru+1, mru.Items[iMru]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Dispatches menu commands.
        /// </summary>
        /// <param name="cmdSet"></param>
        /// <param name="cmdId"></param>
        /// <returns></returns>
        public bool Execute(CommandID cmdId)
        {
            var ct = GetSubCommandTarget();
            if (ct != null && ct.Execute(cmdId))
                return true;
          
            if (currentPhase != null && currentPhase.Execute(cmdId))
                return true;
            if (cmdId.Guid == CmdSets.GuidDecompiler)
            {
                if (ExecuteMruFile(cmdId.ID))
                    return false;

                switch (cmdId.ID)
                {
                case CmdIds.FileOpen: OpenBinaryWithPrompt(); return true;
                case CmdIds.FileOpenAs: return OpenBinaryAs();
                case CmdIds.FileAssemble: return AssembleFile();
                case CmdIds.FileSave: Save(); return true;
                case CmdIds.FileAddMetadata: AddMetadataFile(); return true;
                case CmdIds.FileCloseProject: CloseProject(); return true;
                case CmdIds.FileExit: form.Close(); return true;

                case CmdIds.ActionRestartDecompilation: RestartRecompilation(); return true;
                case CmdIds.ActionNextPhase: NextPhase(); return true;
                case CmdIds.ActionFinishDecompilation: FinishDecompilation(); return true;

                case CmdIds.EditFind: EditFind(); return true;

                case CmdIds.ViewDisassembly: ViewDisassemblyWindow(); return true;
                case CmdIds.ViewMemory: ViewMemoryWindow(); return true;
                case CmdIds.ViewFindAllProcedures: FindProcedures(srSvc); return true;

                case CmdIds.WindowsCascade: LayoutMdi(DocumentWindowLayout.None); return true;
                case CmdIds.WindowsTileVertical: LayoutMdi(DocumentWindowLayout.TiledVertical); return true;
                case CmdIds.WindowsTileHorizontal: LayoutMdi(DocumentWindowLayout.TiledHorizontal); return true;
                case CmdIds.WindowsCloseAll: CloseAllDocumentWindows(); return true;

                case CmdIds.HelpAbout: ShowAboutBox(); return true;
                }
            }
            return false;
        }

        private bool ExecuteMruFile(int cmdId)
        {
            int iMru = cmdId - CmdIds.FileMru;
            if (0 <= iMru && iMru < mru.Items.Count)
            {
                string file = mru.Items[iMru];
                OpenBinary(file, (f) => pageInitial.OpenBinary(file, this));
                mru.Use(file);
                return true;
            }
            return false;
        }

        private bool IsDecompilerLoaded
        {
            get
            {
                if (decompilerSvc.Decompiler == null)
                    return false;
                return decompilerSvc.Decompiler.Project != null;
            }
        }
        #endregion


        #region IStatusBarService Members ////////////////////////////////////

        public void SetText(string text)
        {
            form.StatusStrip.Items[0].Text = text;
        }

        #endregion


        #region DecompilerHost Members //////////////////////////////////

        public IConfigurationService Configuration
        {
            get { return config; }
        }

        public TextWriter CreateDecompiledCodeWriter(string fileName)
        {
            return new StreamWriter(fileName, false, new UTF8Encoding(false));
        }

        public void WriteDisassembly(Program program, Action<TextWriter> writer)
        {
            using (TextWriter output = CreateTextWriter(program.DisassemblyFilename))
            {
                writer(output);
            }
        }

        public void WriteIntermediateCode(Program program, Action<TextWriter> writer)
        {
            using (TextWriter output = CreateTextWriter(program.IntermediateFilename))
            {
                writer(output);
            }
        }

        public void WriteTypes(Program program, Action<TextWriter> writer)
        {
            using (TextWriter output = CreateTextWriter(program.TypesFilename))
            {
                writer(output);
            }
        }

        public void WriteDecompiledCode(Program program, Action<TextWriter> writer)
        {
            using (TextWriter output = CreateTextWriter(program.OutputFilename))
            {
                writer(output);
            }
        }

        public void WriteGlobals(Program program, Action<TextWriter> writer)
        {
            using (TextWriter output = CreateTextWriter(program.GlobalsFilename))
            {
                writer(output);
            }
        }

        #endregion ////////////////////////////////////////////////////


        // Event handlers //////////////////////////////

        private void miFileExit_Click(object sender, System.EventArgs e)
        {
            form.Close();
        }

        private void MainForm_Loaded(object sender, System.EventArgs e)
        {
            var uiPrefsSvc = sc.RequireService<IUiPreferencesService>();
            // It's ok if we can't load settings, just proceed with defaults.
            try
            {
                uiPrefsSvc.Load();
                if (uiPrefsSvc.WindowSize != new System.Drawing.Size())
                    form.Size = uiPrefsSvc.WindowSize;
                form.WindowState = uiPrefsSvc.WindowState;
            }
            catch { };
            SwitchInteractor(pageInitial);
        }

        private void MainForm_Closed(object sender, System.EventArgs e)
        {
            var uiPrefsSvc = sc.RequireService<IUiPreferencesService>();
            // It's OK if we can't save settings, just discard them.
            try
            {
                uiPrefsSvc.WindowSize = form.Size;
                uiPrefsSvc.WindowState = form.WindowState;
                uiPrefsSvc.Save();
                mru.Save(MruListFile);
            }
            catch { }
        }

        private void MainForm_ProcessCommandKey(object sender, KeyEventArgs e)
        {
            dm.ProcessKey(uiSvc, e);
        }


        private void statusBar_PanelClick(object sender, System.Windows.Forms.StatusBarPanelClickEventArgs e)
        {

        }

        private void txtLog_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void toolBar_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            MenuCommand cmd = e.ClickedItem.Tag as MenuCommand;
            if (cmd == null) throw new NotImplementedException("Button not hooked up.");
            Execute(cmd.CommandID);
        }

        public void OnBrowserTreeItemSelected(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.ByKeyboard ||
                e.Action == TreeViewAction.ByMouse)
            {
                throw new NotImplementedException();
            }
        }

        private void InitialPage_IsDirtyChanged(object sender, EventArgs e)
        {
            UpdateWindowTitle();
        }

        public virtual void Run()
        {
            Application.Run((Form)LoadForm());
        }
    }
}
