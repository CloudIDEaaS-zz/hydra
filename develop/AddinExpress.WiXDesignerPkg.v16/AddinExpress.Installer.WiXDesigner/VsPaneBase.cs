using EnvDTE;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;

namespace AddinExpress.Installer.WiXDesigner
{
	internal abstract class VsPaneBase : ToolWindowPane, IVsWindowFrameNotify3, IOleComponent, IVsLinkedUndoClient
	{
		private int x;

		private int y;

		private int width;

		private int height;

		private int internalID;

		private uint _componentId;

		private bool dockable;

		private VsWiXProject projectImpl;

		private bool focused = true;

		private bool appActive;

		private bool solutionLoadedCompletely;

		private bool buildStarted;

		private bool refreshLocked;

		private VsViewManager viewManager;

		private IOleUndoManager undoManager;

		private int closeCounter;

		private Window vsWindow;

		private AddinExpress.Installer.WiXDesigner.ErrorView errorView;

		private bool lastFocusedState;

		private int idleCount;

		public bool AllowRefreshUI
		{
			get
			{
				if (this.buildStarted || !this.solutionLoadedCompletely || !this.focused)
				{
					return false;
				}
				return !this.refreshLocked;
			}
		}

		public bool BuildStarted
		{
			get
			{
				return this.buildStarted;
			}
		}

		internal AddinExpress.Installer.WiXDesigner.ErrorView ErrorView
		{
			get
			{
				return this.errorView;
			}
		}

		public bool Focused
		{
			get
			{
				if (this.projectImpl.VsDTE == null)
				{
					return false;
				}
				if (!this.focused)
				{
					return false;
				}
				return this.projectImpl.VsDTE.ActiveWindow == this.VsWindow;
			}
		}

		public int Height
		{
			get
			{
				return this.height;
			}
		}

		public IVsUIHierarchyWindow HierarchyWindow
		{
			get
			{
				object obj;
				IVsWindowFrame frame = base.get_Frame() as IVsWindowFrame;
				if (frame == null || frame.GetProperty(-3001, out obj) != 0)
				{
					return null;
				}
				return (IVsUIHierarchyWindow)obj;
			}
		}

		public int ID
		{
			get
			{
				return this.internalID;
			}
		}

		public bool IsDockable
		{
			get
			{
				return this.dockable;
			}
		}

		public VsWiXProject ProjectManager
		{
			get
			{
				return this.projectImpl;
			}
		}

		public IOleUndoManager UndoManager
		{
			get
			{
				return this.undoManager;
			}
		}

		public VsViewManager ViewManager
		{
			get
			{
				return this.viewManager;
			}
		}

		public Window VsWindow
		{
			get
			{
				object obj;
				if (this.vsWindow == null)
				{
					IVsWindowFrame frame = base.get_Frame() as IVsWindowFrame;
					if (frame != null && frame.GetProperty(-5003, out obj) == 0)
					{
						this.vsWindow = (Window)obj;
					}
				}
				return this.vsWindow;
			}
		}

		public int Width
		{
			get
			{
				return this.width;
			}
		}

		public int X
		{
			get
			{
				return this.x;
			}
		}

		public int Y
		{
			get
			{
				return this.y;
			}
		}

		public VsPaneBase() : base(null)
		{
			this.viewManager = new VsViewManager(this);
			this.errorView = new AddinExpress.Installer.WiXDesigner.ErrorView();
		}

		private void ApplyTextColorForMenuItems(Color color, ToolStripItemCollection items)
		{
			foreach (ToolStripItem item in items)
			{
				if (!(item is ToolStripMenuItem))
				{
					continue;
				}
				item.ForeColor = color;
				if ((item as ToolStripMenuItem).DropDownItems.Count <= 0)
				{
					continue;
				}
				this.ApplyTextColorForMenuItems(color, (item as ToolStripMenuItem).DropDownItems);
			}
		}

		private void ApplyThemeForSpecificControls(UserControl viewControl, IContainer container, Control.ControlCollection controls, IVsUIShell2 uiShell, IUIService uiService)
		{
			VSColorTable vSColorTable = null;
			vSColorTable = new VSColorTable(uiShell);
			if (container != null)
			{
				foreach (Component component in container.Components)
				{
					if (!(component is ContextMenuStrip))
					{
						continue;
					}
					(component as ContextMenuStrip).Renderer = new ToolStripProfessionalRenderer(vSColorTable);
					this.ApplyTextColorForMenuItems(viewControl.ForeColor, (component as ContextMenuStrip).Items);
				}
			}
			if (controls != null)
			{
				foreach (Control control in controls)
				{
					if (control is TreeView)
					{
						VsShellUtilities.ApplyTreeViewThemeStyles(control as TreeView);
					}
					else if (control is ListView)
					{
						VsShellUtilities.ApplyListViewThemeStyles(control as ListView);
					}
					if (control.Controls == null || control.Controls.Count <= 0)
					{
						continue;
					}
					this.ApplyThemeForSpecificControls(viewControl, null, control.Controls, uiShell, uiService);
				}
			}
		}

		private void ClearResources(bool userAction)
		{
			if (this.projectImpl != null)
			{
				try
				{
					try
					{
						if (this.undoManager != null)
						{
							IVsLinkCapableUndoManager vsLinkCapableUndoManager = (IVsLinkCapableUndoManager)this.undoManager;
							if (vsLinkCapableUndoManager != null)
							{
								vsLinkCapableUndoManager.UnadviseLinkedUndoClient();
							}
							((IVsLifetimeControlledObject)this.undoManager).SeverReferencesToOwner();
							this.undoManager = null;
						}
						IOleComponentManager service = this.GetService(typeof(SOleComponentManager)) as IOleComponentManager;
						if (service != null)
						{
							service.FRevokeComponent(this._componentId);
						}
						if (userAction)
						{
							this.projectImpl.UnregisterPane(this.GetPaneName());
						}
						if (this.viewManager != null)
						{
							this.viewManager.OnCloseToolWindow(userAction);
							this.viewManager.Dispose();
							this.viewManager = null;
						}
						if (this.errorView != null)
						{
							this.errorView.Dispose();
						}
					}
					catch (Exception exception)
					{
						DTEHelperObject.ShowErrorDialog(this, exception);
					}
				}
				finally
				{
					this.projectImpl = null;
				}
			}
		}

		internal void DoSomethingChanged(VsWiXProject.ChangeContextType contextType, params object[] contextObject)
		{
			switch (contextType)
			{
				case VsWiXProject.ChangeContextType.SolutionLoaded:
				{
					this.solutionLoadedCompletely = true;
					if (this.viewManager == null)
					{
						break;
					}
					this.viewManager.OnSolutionLoaded();
					this.viewManager.SetBuffersDirty(true, false);
					return;
				}
				case VsWiXProject.ChangeContextType.ProjectClosing:
				{
					this.refreshLocked = true;
					if (this.viewManager == null)
					{
						break;
					}
					this.viewManager.OnProjectClosing();
					return;
				}
				case VsWiXProject.ChangeContextType.ProjectParentChanged:
				{
					if (this.viewManager == null)
					{
						break;
					}
					this.viewManager.OnProjectParentChanged();
					return;
				}
				case VsWiXProject.ChangeContextType.ProjectRenamed:
				{
					base.set_Caption(string.Concat(this.GetPaneName(), " (", this.projectImpl.VsProject.Name, ")"));
					if (this.viewManager == null)
					{
						break;
					}
					this.viewManager.OnProjectRenamed();
					return;
				}
				case VsWiXProject.ChangeContextType.ProjectAdded:
				{
					this.solutionLoadedCompletely = true;
					if (this.viewManager == null)
					{
						break;
					}
					this.viewManager.SetBuffersDirty(true, false);
					return;
				}
				case VsWiXProject.ChangeContextType.FileAdded:
				{
					if (this.viewManager == null)
					{
						break;
					}
					this.viewManager.OnFileAdded((string)contextObject[0]);
					this.viewManager.SetBuffersDirty(true, true);
					return;
				}
				case VsWiXProject.ChangeContextType.FileRemoved:
				{
					if (this.viewManager == null)
					{
						break;
					}
					this.viewManager.OnFileRemoved((string)contextObject[0]);
					this.viewManager.SetBuffersDirty(true, true);
					return;
				}
				case VsWiXProject.ChangeContextType.FileRenamed:
				{
					if (this.viewManager == null)
					{
						break;
					}
					this.viewManager.SetBuffersDirty(true, true);
					return;
				}
				case VsWiXProject.ChangeContextType.ReferenceAdded:
				{
					if (this.viewManager == null)
					{
						break;
					}
					this.viewManager.OnReferenceAdded((VsWiXProject.ReferenceDescriptor)contextObject[0]);
					return;
				}
				case VsWiXProject.ChangeContextType.ReferenceRemoved:
				{
					if (this.viewManager == null)
					{
						break;
					}
					this.viewManager.OnReferenceRemoved((VsWiXProject.ReferenceDescriptor)contextObject[0]);
					return;
				}
				case VsWiXProject.ChangeContextType.ReferenceRenamed:
				{
					if (this.viewManager == null)
					{
						break;
					}
					this.viewManager.OnReferenceRenamed((VsWiXProject.ReferenceDescriptor)contextObject[0], (VsWiXProject.ReferenceDescriptor)contextObject[1]);
					return;
				}
				case VsWiXProject.ChangeContextType.BuildStarted:
				{
					this.buildStarted = true;
					if (this.viewManager == null)
					{
						break;
					}
					this.viewManager.OnBuildStarted();
					return;
				}
				case VsWiXProject.ChangeContextType.BuildStopped:
				{
					this.buildStarted = false;
					if (this.viewManager == null)
					{
						break;
					}
					this.viewManager.OnBuildStopped();
					return;
				}
				case VsWiXProject.ChangeContextType.ReferenceRefreshed:
				{
					if (this.viewManager == null)
					{
						break;
					}
					this.viewManager.OnReferenceRefreshed((VsWiXProject.ReferenceDescriptor)contextObject[0]);
					return;
				}
				case VsWiXProject.ChangeContextType.ProjectPropertiesSelected:
				{
					this.focused = false;
					this.lastFocusedState = false;
					if (this.viewManager == null)
					{
						break;
					}
					this.viewManager.OnToolWindowDeactivate();
					break;
				}
				default:
				{
					return;
				}
			}
		}

		public abstract ViewControlBase GetInnerControl();

		public override object GetIVsWindowPane()
		{
			return this;
		}

		public abstract string GetPaneName();

		internal virtual void Initialize(VsWiXProject projectImpl, int id, bool solutionOpened, bool buildStarted)
		{
			this.internalID = id;
			this.projectImpl = projectImpl;
			this.solutionLoadedCompletely = solutionOpened;
			this.buildStarted = buildStarted;
			base.set_Caption(string.Concat(this.GetPaneName(), " (", this.projectImpl.VsProject.Name, ")"));
			if (solutionOpened && !buildStarted && this.viewManager != null)
			{
				this.viewManager.SetBuffersDirty(true, false);
			}
		}

		int Microsoft.VisualStudio.OLE.Interop.IOleComponent.FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
		{
			return 0;
		}

		int Microsoft.VisualStudio.OLE.Interop.IOleComponent.FDoIdle(uint grfidlef)
		{
			if (this.focused && this.projectImpl.VsDTE.ActiveWindow != null)
			{
				int hWnd = this.projectImpl.VsDTE.ActiveWindow.HWnd;
				string objectKind = this.projectImpl.VsDTE.ActiveWindow.ObjectKind;
				if (hWnd != this.VsWindow.HWnd && objectKind != "{EEFA5220-E298-11D0-8F78-00A0C9110057}" && objectKind != "{74946810-37A0-11D2-A273-00C04F8EF4FF}")
				{
					this.focused = false;
					this.lastFocusedState = false;
					if (this.viewManager != null)
					{
						this.viewManager.OnToolWindowDeactivate();
					}
				}
			}
			if (this.AllowRefreshUI && this.viewManager != null)
			{
				this.viewManager.DoIdle();
			}
			return 0;
		}

		int Microsoft.VisualStudio.OLE.Interop.IOleComponent.FPreTranslateMessage(MSG[] pMsg)
		{
			return 0;
		}

		int Microsoft.VisualStudio.OLE.Interop.IOleComponent.FQueryTerminate(int fPromptUser)
		{
			return 1;
		}

		int Microsoft.VisualStudio.OLE.Interop.IOleComponent.FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
		{
			return 0;
		}

		IntPtr Microsoft.VisualStudio.OLE.Interop.IOleComponent.HwndGetWindow(uint dwWhich, uint dwReserved)
		{
			return IntPtr.Zero;
		}

		void Microsoft.VisualStudio.OLE.Interop.IOleComponent.OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved)
		{
		}

		void Microsoft.VisualStudio.OLE.Interop.IOleComponent.OnAppActivate(int fActive, uint dwOtherThreadID)
		{
			this.appActive = (fActive > 0 ? true : false);
		}

		void Microsoft.VisualStudio.OLE.Interop.IOleComponent.OnEnterState(uint uStateID, int fEnter)
		{
		}

		void Microsoft.VisualStudio.OLE.Interop.IOleComponent.OnLoseActivation()
		{
		}

		void Microsoft.VisualStudio.OLE.Interop.IOleComponent.Terminate()
		{
		}

		int Microsoft.VisualStudio.Shell.Interop.IVsWindowFrameNotify3.OnClose(ref uint pgrfSaveOptions)
		{
			bool flag;
			if ((pgrfSaveOptions & 66560) > 0 && this.closeCounter == 0)
			{
				this.projectImpl.SaveWiXFiles(this.internalID, true, true, true, out flag);
				if (flag)
				{
					return -2147467260;
				}
				this.closeCounter++;
			}
			return 0;
		}

		int Microsoft.VisualStudio.Shell.Interop.IVsWindowFrameNotify3.OnDockableChange(int fDockable, int x, int y, int w, int h)
		{
			this.x = x;
			this.y = y;
			this.width = w;
			this.height = h;
			this.dockable = fDockable != 0;
			return 0;
		}

		int Microsoft.VisualStudio.Shell.Interop.IVsWindowFrameNotify3.OnMove(int x, int y, int w, int h)
		{
			this.x = x;
			this.y = y;
			this.width = w;
			this.height = h;
			return 0;
		}

		int Microsoft.VisualStudio.Shell.Interop.IVsWindowFrameNotify3.OnShow(int fShow)
		{
			if (fShow != 2 && fShow != 12 && fShow != 14 && fShow != 1)
			{
				this.focused = false;
			}
			else if (fShow == 2 || fShow == 12 || fShow == 14)
			{
				this.focused = true;
			}
			if (this.lastFocusedState != this.focused)
			{
				this.lastFocusedState = this.focused;
				if (this.viewManager != null)
				{
					if (!this.focused)
					{
						this.viewManager.OnToolWindowDeactivate();
					}
					else
					{
						this.viewManager.OnToolWindowActivate();
					}
				}
			}
			return 0;
		}

		int Microsoft.VisualStudio.Shell.Interop.IVsWindowFrameNotify3.OnSize(int x, int y, int w, int h)
		{
			this.x = x;
			this.y = y;
			this.width = w;
			this.height = h;
			this.GetInnerControl().Size = new Size(this.width, this.height);
			return 0;
		}

		int Microsoft.VisualStudio.TextManager.Interop.IVsLinkedUndoClient.OnInterveningUnitBlockingLinkedUndo()
		{
			return -2147467259;
		}

		protected override void OnClose()
		{
			this.ClearResources(this.projectImpl != null);
			base.OnClose();
		}

		internal virtual void OnProjectUnload()
		{
			this.ClearResources(false);
		}

		internal void OnThemeChanged()
		{
			this.UpdateColors();
			if (this.viewManager != null)
			{
				this.viewManager.OnThemeChanged();
			}
		}

		public override void OnToolWindowCreated()
		{
			Guid guid;
			int num;
			int num1;
			int num2;
			int num3;
			base.OnToolWindowCreated();
			this.UpdateColors();
			IVsWindowFrame frame = base.get_Frame() as IVsWindowFrame;
			if (frame != null)
			{
				frame.SetProperty(-3011, this);
				frame.SetProperty(-3008, VSFRAMEMODE.VSFM_MdiChild);
				if (this.viewManager != null)
				{
					VSSETFRAMEPOS[] vSSETFRAMEPOSArray = new VSSETFRAMEPOS[1];
					frame.GetFramePos(vSSETFRAMEPOSArray, out guid, out num, out num1, out num2, out num3);
					this.x = num;
					this.y = num1;
					this.width = num2;
					this.height = num3;
				}
			}
			IOleComponentManager service = (IOleComponentManager)this.GetService(typeof(SOleComponentManager));
			if (this._componentId == 0 && service != null)
			{
				OLECRINFO[] oLECRINFOArray = new OLECRINFO[1];
				oLECRINFOArray[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
				oLECRINFOArray[0].grfcrf = 3;
				oLECRINFOArray[0].grfcadvf = 7;
				oLECRINFOArray[0].uIdleTimeInterval = 100;
				service.FRegisterComponent(this, oLECRINFOArray, out this._componentId);
			}
			this.undoManager = (IOleUndoManager)this.GetService(typeof(SOleUndoManager));
			if (this.undoManager != null)
			{
				IVsLinkCapableUndoManager vsLinkCapableUndoManager = (IVsLinkCapableUndoManager)this.undoManager;
				if (vsLinkCapableUndoManager != null)
				{
					vsLinkCapableUndoManager.AdviseLinkedUndoClient(this);
				}
			}
		}

		private void UpdateColors()
		{
			uint num;
			IVsUIShell2 service = this.GetService(typeof(SVsUIShell)) as IVsUIShell2;
			if (service != null)
			{
				ViewControlBase innerControl = this.GetInnerControl();
				if (service.GetVSSysColorEx(-162, out num) == 0)
				{
					innerControl.BackColor = ColorTranslator.FromWin32((int)num);
					this.errorView.BackColor = innerControl.BackColor;
				}
				if (service.GetVSSysColorEx(-170, out num) == 0)
				{
					innerControl.ForeColor = ColorTranslator.FromWin32((int)num);
					this.errorView.ForeColor = innerControl.ForeColor;
				}
				Font environmentFont = VsShellUtilities.GetEnvironmentFont(VsPackage.CurrentInstance);
				if (environmentFont != null)
				{
					innerControl.Font = environmentFont;
					this.errorView.Font = innerControl.Font;
				}
				IUIService uIService = this.GetService(typeof(IUIService)) as IUIService;
				this.ApplyThemeForSpecificControls(innerControl, innerControl.GetContainer(), innerControl.Controls, service, uIService);
				this.ApplyThemeForSpecificControls(this.errorView, this.errorView.GetContainer(), this.errorView.Controls, service, uIService);
			}
		}
	}
}