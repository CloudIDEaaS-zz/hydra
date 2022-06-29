using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VsViewManager : IViewManager, IDisposable
	{
		private bool disposed;

		private bool busy;

		private VsPaneBase parentPane;

		private bool buffersDirty;

		private bool designerDirty;

		private long dirtyTime;

		private object _xmlLanguageService;

		private System.IServiceProvider _serviceProvider;

		private ITrackSelection trackSelection;

		private Microsoft.VisualStudio.Shell.SelectionContainer selectionContainer;

		private bool gettingCheckoutStatus;

		private Dictionary<string, string> textContentList = new Dictionary<string, string>();

		private Dictionary<string, XmlDocument> xmlContentList = new Dictionary<string, XmlDocument>();

		public bool DesignerDirty
		{
			get
			{
				return this.designerDirty;
			}
			set
			{
				this.designerDirty = value;
			}
		}

		public bool IsBuffersDirty
		{
			get
			{
				return this.buffersDirty;
			}
		}

		public bool IsBusy
		{
			get
			{
				return this.busy;
			}
		}

		public bool IsXmlEditorParsing
		{
			get
			{
				object xmlLanguageService = this.GetXmlLanguageService();
				if (xmlLanguageService == null)
				{
					return false;
				}
				return (bool)xmlLanguageService.GetType().InvokeMember("IsParsing", BindingFlags.GetProperty, null, xmlLanguageService, null);
			}
		}

		public VsPaneBase Pane
		{
			get
			{
				return this.parentPane;
			}
		}

		public VsWiXProject ProjectManager
		{
			get
			{
				return this.parentPane.ProjectManager;
			}
		}

		public System.IServiceProvider ServiceProvider
		{
			get
			{
				return this._serviceProvider;
			}
		}

		internal ITrackSelection TrackSelection
		{
			get
			{
				return this.trackSelection;
			}
			set
			{
				if (value != null)
				{
					this.trackSelection = value;
					this.selectionContainer.set_SelectableObjects(null);
					this.selectionContainer.set_SelectedObjects(null);
					this.trackSelection.OnSelectChange(this.selectionContainer);
				}
			}
		}

		public VsViewManager(VsPaneBase parentPane)
		{
			this.parentPane = parentPane;
			this._serviceProvider = parentPane;
		}

		private bool CanEditFile(string filePath)
		{
			uint num;
			uint num1;
			bool flag;
			if (this.gettingCheckoutStatus)
			{
				return false;
			}
			try
			{
				this.gettingCheckoutStatus = true;
				IVsQueryEditQuerySave2 service = (IVsQueryEditQuerySave2)this._serviceProvider.GetService(typeof(SVsQueryEditQuerySave));
				if (service != null)
				{
					if (ErrorHandler.Succeeded(service.QueryEditFiles(0, 1, new string[] { filePath }, null, null, out num, out num1)) && num == 0)
					{
						flag = true;
						return flag;
					}
				}
				return false;
			}
			finally
			{
				this.gettingCheckoutStatus = false;
			}
			return flag;
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.disposed = true;
				this.textContentList.Clear();
				this.xmlContentList.Clear();
			}
			GC.SuppressFinalize(this);
		}

		internal void DoIdle()
		{
			if ((this.IsBuffersDirty || this.DesignerDirty) && this.ProjectManager != null && (ulong)this.GetTicks() - this.dirtyTime > (long)100)
			{
				if (this.IsXmlEditorParsing)
				{
					this.dirtyTime = (long)0;
					return;
				}
				if (this.DesignerDirty)
				{
					object xmlLanguageService = this.GetXmlLanguageService();
					try
					{
						try
						{
							if (xmlLanguageService != null)
							{
								xmlLanguageService.GetType().InvokeMember("IsParsing", BindingFlags.SetProperty, null, xmlLanguageService, new object[] { true });
							}
							foreach (KeyValuePair<string, string> keyValuePair in this.textContentList)
							{
								if (!this.CanEditFile(keyValuePair.Key))
								{
									continue;
								}
								this.ProjectManager.ModifyWiXFile(this.Pane.ID, keyValuePair.Key, keyValuePair.Value);
							}
							foreach (KeyValuePair<string, XmlDocument> keyValuePair1 in this.xmlContentList)
							{
								if (!this.CanEditFile(keyValuePair1.Key))
								{
									continue;
								}
								this.ProjectManager.ModifyWiXFile(this.Pane.ID, keyValuePair1.Key, keyValuePair1.Value);
							}
						}
						catch (Exception exception)
						{
							DTEHelperObject.ShowErrorDialog(this, exception);
							this.dirtyTime = (long)this.GetTicks();
						}
					}
					finally
					{
						if (xmlLanguageService != null)
						{
							xmlLanguageService.GetType().InvokeMember("IsParsing", BindingFlags.SetProperty, null, xmlLanguageService, new object[] { false });
						}
						this.textContentList.Clear();
						this.xmlContentList.Clear();
						this.buffersDirty = false;
						this.designerDirty = false;
					}
				}
				else if (this.IsBuffersDirty)
				{
					try
					{
						try
						{
							if (this.ProjectManager != null)
							{
								this.ProjectManager.WiXModel.OnViewTextModelChanged(this.Pane.ID);
								this.ProjectManager.WiXModel.OnViewXmlModelChanged(this.Pane.ID);
							}
							UserControl innerControl = this.Pane.GetInnerControl();
							if (innerControl.Controls.ContainsKey("ErrorView"))
							{
								innerControl.Controls.RemoveByKey("ErrorView");
								innerControl.Refresh();
							}
						}
						catch (Exception exception4)
						{
							Exception exception1 = exception4;
							bool flag = false;
							if (this.BeforeShowError != null)
							{
								this.BeforeShowError(exception1, ref flag);
							}
							if (this.ProjectManager != null && !flag)
							{
								this.ProjectManager.WiXModel.OnBeforeShowError(exception1, ref flag);
							}
							if (!flag)
							{
								try
								{
									UserControl userControl = this.Pane.GetInnerControl();
									if (!this.Pane.ErrorView.ComposeErrorMessage(this.ProjectManager.VsProject, exception1))
									{
										flag = true;
									}
									else
									{
										if (!userControl.Controls.ContainsKey("ErrorView"))
										{
											userControl.Controls.Add(this.Pane.ErrorView);
											this.Pane.ErrorView.Dock = DockStyle.Fill;
											this.Pane.ErrorView.BringToFront();
											this.Pane.ErrorView.Visible = true;
										}
										userControl.Refresh();
									}
								}
								catch (Exception exception3)
								{
									Exception exception2 = exception3;
									flag = true;
									exception1 = (Exception)Activator.CreateInstance(exception2.GetType(), new object[] { exception2.Message, exception1 });
								}
							}
							if (flag)
							{
								DTEHelperObject.ShowErrorDialog(this, exception1);
							}
							this.dirtyTime = (long)this.GetTicks();
						}
					}
					finally
					{
						this.buffersDirty = false;
					}
				}
			}
		}

		~VsViewManager()
		{
			this.Dispose();
		}

		private uint GetTicks()
		{
			return (uint)Environment.TickCount;
		}

		private object GetXmlLanguageService()
		{
			IntPtr intPtr;
			if (this._xmlLanguageService == null)
			{
				Microsoft.VisualStudio.OLE.Interop.IServiceProvider service = this._serviceProvider.GetService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)) as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
				Guid guid = new Guid("f6819a78-a205-47b5-be1c-675b3c7f0b8e");
				Guid guid1 = new Guid("00000000-0000-0000-C000-000000000046");
				if (ErrorHandler.Succeeded(service.QueryService(ref guid, ref guid1, out intPtr)))
				{
					try
					{
						this._xmlLanguageService = Marshal.GetObjectForIUnknown(intPtr);
					}
					finally
					{
						Marshal.Release(intPtr);
					}
				}
			}
			return this._xmlLanguageService;
		}

		public void OnActivatePropertiesWindow()
		{
			IVsUIShell service = (IVsUIShell)this._serviceProvider.GetService(typeof(IVsUIShell));
			if (service != null)
			{
				IVsWindowFrame vsWindowFrame = null;
				Guid propertyBrowser = StandardToolWindows.PropertyBrowser;
				service.FindToolWindow(524288, ref propertyBrowser, out vsWindowFrame);
				if (vsWindowFrame != null)
				{
					vsWindowFrame.Show();
				}
			}
		}

		public void OnBuildStarted()
		{
			if (this.BuildStarted != null)
			{
				this.BuildStarted();
			}
		}

		public void OnBuildStopped()
		{
			if (this.BuildStopped != null)
			{
				this.BuildStopped();
			}
		}

		internal void OnChangeLineAttributes(VsWiXProject.WiXFileDescriptor fileDesc, int iFirstLine, int iLastLine)
		{
		}

		internal void OnChangeLineText(VsWiXProject.WiXFileDescriptor fileDesc, TextLineChange[] pTextLineChange, int fLast)
		{
			if (!this.busy)
			{
				this.buffersDirty = true;
				this.dirtyTime = (long)this.GetTicks();
			}
		}

		internal void OnCloseToolWindow(bool userAction)
		{
			if (this.ToolWindowBeforeClose != null)
			{
				try
				{
					this.ToolWindowBeforeClose(this.Pane.ID, userAction);
				}
				catch (Exception exception)
				{
					DTEHelperObject.ShowErrorDialog(this, exception);
				}
			}
			if (this.ProjectManager != null)
			{
				this.ProjectManager.WiXModel.OnToolWindowBeforeClose(this.Pane.ID, userAction);
				if (this.Equals(this.ProjectManager.WiXModel.ViewManager))
				{
					this.ProjectManager.WiXModel.ViewManager = null;
				}
			}
		}

		public void OnFileAdded(string filePath)
		{
			if (this.FileAdded != null)
			{
				this.FileAdded(filePath);
			}
		}

		internal void OnFileBufferStatusChanged(VsWiXProject.WiXFileDescriptor fileDesc)
		{
			if (fileDesc.IsDirty)
			{
				if (!this.Pane.get_Caption().EndsWith("*"))
				{
					VsPaneBase pane = this.Pane;
					((ToolWindowPane)pane).set_Caption(string.Concat(((ToolWindowPane)pane).get_Caption(), "*"));
					return;
				}
			}
			else if (this.Pane.get_Caption().EndsWith("*") && this.ProjectManager.GetUnsavedFilesCount(this.Pane.ID) == 0)
			{
				this.Pane.set_Caption(this.Pane.get_Caption().Substring(0, this.Pane.get_Caption().Length - 1));
			}
		}

		public void OnFileRemoved(string filePath)
		{
			if (this.FileRemoved != null)
			{
				this.FileRemoved(filePath);
			}
		}

		public void OnLoadUserSettings(Hashtable userData)
		{
			if (this.LoadUserSettings != null)
			{
				this.LoadUserSettings(userData);
			}
		}

		public void OnProjectClosing()
		{
			if (this.ProjectClosing != null)
			{
				this.ProjectClosing();
			}
		}

		public void OnProjectParentChanged()
		{
			if (this.ProjectParentChanged != null)
			{
				this.ProjectParentChanged();
			}
		}

		public void OnProjectRenamed()
		{
			if (this.ProjectRenamed != null)
			{
				this.ProjectRenamed();
			}
		}

		public void OnReferenceAdded(VsWiXProject.ReferenceDescriptor reference)
		{
			if (this.ReferenceAdded != null)
			{
				this.ReferenceAdded(reference);
			}
		}

		public void OnReferenceRefreshed(VsWiXProject.ReferenceDescriptor reference)
		{
			if (this.ReferenceRefreshed != null)
			{
				this.ReferenceRefreshed(reference);
			}
		}

		public void OnReferenceRemoved(VsWiXProject.ReferenceDescriptor reference)
		{
			if (this.ReferenceRemoved != null)
			{
				this.ReferenceRemoved(reference);
			}
		}

		public void OnReferenceRenamed(VsWiXProject.ReferenceDescriptor oldReference, VsWiXProject.ReferenceDescriptor newReference)
		{
			if (this.ReferenceRenamed != null)
			{
				this.ReferenceRenamed(oldReference, newReference);
			}
		}

		public void OnSaveUserSettings(Hashtable userData)
		{
			if (this.SaveUserSettings != null)
			{
				this.SaveUserSettings(userData);
			}
		}

		public void OnSelectionChanged(ArrayList selectedObjects)
		{
			this.selectionContainer = new Microsoft.VisualStudio.Shell.SelectionContainer(true, false);
			this.selectionContainer.set_SelectableObjects(selectedObjects);
			this.selectionContainer.set_SelectedObjects(selectedObjects);
			ITrackSelection trackSelection = this.TrackSelection;
			if (trackSelection != null)
			{
				trackSelection.OnSelectChange(this.selectionContainer);
			}
		}

		public void OnSolutionLoaded()
		{
			if (this.SolutionLoaded != null)
			{
				this.SolutionLoaded();
			}
		}

		public void OnTextContentChanged(string filePath, string textLines)
		{
			if (!string.IsNullOrEmpty(filePath))
			{
				this.textContentList.Add(filePath, textLines);
				this.designerDirty = true;
				this.buffersDirty = false;
				this.dirtyTime = (long)this.GetTicks();
			}
		}

		public void OnThemeChanged()
		{
			if (this.ThemeChanged != null)
			{
				this.ThemeChanged();
			}
		}

		internal void OnToolWindowActivate()
		{
			if (this.ProjectManager != null)
			{
				this.ProjectManager.WiXModel.ViewManager = this;
				this.ProjectManager.WiXModel.OnToolWindowActivate();
			}
			if (this.ToolWindowActivate != null)
			{
				try
				{
					this.ToolWindowActivate();
				}
				catch (Exception exception)
				{
					DTEHelperObject.ShowErrorDialog(this, exception);
				}
			}
		}

		internal void OnToolWindowCreated(int x, int y, int cx, int cy)
		{
			this.selectionContainer = new Microsoft.VisualStudio.Shell.SelectionContainer();
			this.TrackSelection = (ITrackSelection)this._serviceProvider.GetService(typeof(STrackSelection));
			if (this.ProjectManager != null)
			{
				this.ProjectManager.WiXModel.ViewManager = this;
				this.ProjectManager.WiXModel.OnToolWindowCreated(this.Pane.ID, x, y, cx, cy);
			}
			if (this.ToolWindowCreated != null)
			{
				try
				{
					this.ToolWindowCreated(this.Pane.ID, x, y, cx, cy);
				}
				catch (Exception exception)
				{
					DTEHelperObject.ShowErrorDialog(this, exception);
				}
			}
		}

		internal void OnToolWindowDeactivate()
		{
			if (this.ProjectManager != null)
			{
				if (this.ProjectManager.WiXModel.ViewManager == this && this.ProjectManager.VsDTE.ActiveWindow != null && this.ProjectManager.VsDTE.ActiveWindow.ObjectKind == "{3AE79031-E1BC-11D0-8F78-00A0C9110057}")
				{
					this.ProjectManager.WiXModel.ViewManager = null;
				}
				this.ProjectManager.WiXModel.OnToolWindowDeactivate();
			}
			if (this.ToolWindowDeactivate != null)
			{
				try
				{
					this.ToolWindowDeactivate();
				}
				catch (Exception exception)
				{
					DTEHelperObject.ShowErrorDialog(this, exception);
				}
			}
		}

		internal void OnWiXFileClosed(VsWiXProject.WiXFileDescriptor fileDesc)
		{
			IVsTextManager service = (IVsTextManager)this._serviceProvider.GetService(typeof(SVsTextManager));
			if (service != null)
			{
				service.UnregisterIndependentView(this.Pane, fileDesc.TextLines);
			}
		}

		internal void OnWiXFileOpened(VsWiXProject.WiXFileDescriptor fileDesc)
		{
			IVsTextManager service = (IVsTextManager)this._serviceProvider.GetService(typeof(SVsTextManager));
			if (service != null)
			{
				service.RegisterIndependentView(this.Pane, fileDesc.TextLines);
			}
		}

		public void OnXmlDocumentContentChanged(string filePath, XmlDocument document)
		{
			if (!string.IsNullOrEmpty(filePath))
			{
				if (!this.xmlContentList.ContainsKey(filePath))
				{
					this.xmlContentList.Add(filePath, document);
				}
				else
				{
					this.xmlContentList[filePath] = document;
				}
				this.designerDirty = true;
				this.buffersDirty = false;
				this.dirtyTime = (long)this.GetTicks();
			}
		}

		public void SetBuffersDirty(bool dirty, bool allowParse = false)
		{
			this.buffersDirty = dirty;
			if (this.buffersDirty & allowParse && this.ProjectManager != null && this.ProjectManager.WiXModel != null)
			{
				this.ProjectManager.WiXModel.IsDirty = true;
			}
		}

		public event BeforeShowErrorEventHandler BeforeShowError;

		public event BuildNotificationEventHandler BuildStarted;

		public event BuildNotificationEventHandler BuildStopped;

		public event FileAddedEventHandler FileAdded;

		public event FileRemovedEventHandler FileRemoved;

		public event LoadUserSettingsEventHandler LoadUserSettings;

		public event ProjectNotificationEventHandler ProjectClosing;

		public event ProjectNotificationEventHandler ProjectParentChanged;

		public event ProjectNotificationEventHandler ProjectRenamed;

		public event ReferenceAddedEventHandler ReferenceAdded;

		public event ReferenceRefreshedEventHandler ReferenceRefreshed;

		public event ReferenceRemovedEventHandler ReferenceRemoved;

		public event ReferenceRenamedEventHandler ReferenceRenamed;

		public event SaveUserSettingsEventHandler SaveUserSettings;

		public event SolutionNotificationEventHandler SolutionLoaded;

		public event ThemeChangedEventHandler ThemeChanged;

		public event ToolWindowActivateEventHandler ToolWindowActivate;

		public event ToolWindowBeforeCloseEventHandler ToolWindowBeforeClose;

		public event ToolWindowCreatedEventHandler ToolWindowCreated;

		public event ToolWindowDeactivateEventHandler ToolWindowDeactivate;
	}
}