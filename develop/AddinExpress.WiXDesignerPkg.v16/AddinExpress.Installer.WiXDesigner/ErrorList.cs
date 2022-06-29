using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class ErrorList : System.IServiceProvider, IDisposable
	{
		private bool disposed;

		private bool UIServiceChecked;

		public ErrorListProvider ErrorProvider;

		public List<ErrorTask> ErrorTaskList;

		public bool ErrorListAvailable;

		private readonly static Guid IID_IUnknown;

		public bool IsErrorListVisible
		{
			get
			{
				if (DTEHelperObject.DTEInstance != null && DTEHelperObject.DTEInstance.ToolWindows.ErrorList != null)
				{
					Window parent = DTEHelperObject.DTEInstance.ToolWindows.ErrorList.Parent;
					if (parent != null)
					{
						return parent.Visible;
					}
				}
				return false;
			}
		}

		static ErrorList()
		{
			AddinExpress.Installer.WiXDesigner.ErrorList.IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");
		}

		public ErrorList()
		{
			this.ErrorTaskList = new List<ErrorTask>();
			this.ErrorProvider = new ErrorListProvider(this);
			this.ErrorProvider.set_ProviderName("Designer for Visual Studio WiX Setup Projects Error Provider");
			this.ErrorProvider.set_ProviderGuid(new Guid("F35BAF15-1B80-431A-AA64-78BDD2F8374D"));
			this.ErrorProvider.Show();
			this.UIServiceChecked = true;
		}

		public void ActivateErrorListToolWindow()
		{
			if (DTEHelperObject.DTEInstance != null && DTEHelperObject.DTEInstance.ToolWindows.ErrorList != null)
			{
				Window parent = DTEHelperObject.DTEInstance.ToolWindows.ErrorList.Parent;
				if (parent != null)
				{
					parent.Visible = true;
					parent.Activate();
				}
			}
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.disposed = true;
				if (this.ErrorTaskList != null && this.ErrorTaskList.Count > 0)
				{
					this.RemoveAllTasksInternal();
					this.ErrorTaskList = null;
				}
				if (this.ErrorProvider != null)
				{
					this.ErrorProvider.Dispose();
					this.ErrorProvider = null;
				}
			}
			GC.SuppressFinalize(this);
		}

		~ErrorList()
		{
			this.Dispose();
		}

		public object GetService(object serviceProviderObject, Guid guid)
		{
			IntPtr intPtr;
			int num = 0;
			Guid guid1 = guid;
			Guid iDIUnknown = AddinExpress.Installer.WiXDesigner.ErrorList.IID_IUnknown;
			object objectForIUnknown = null;
			try
			{
				Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider = serviceProviderObject as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
				if (serviceProvider != null)
				{
					num = serviceProvider.QueryService(ref guid1, ref iDIUnknown, out intPtr);
					if (num != 0)
					{
						Marshal.ThrowExceptionForHR(num);
					}
					else if (!intPtr.Equals(IntPtr.Zero))
					{
						objectForIUnknown = Marshal.GetObjectForIUnknown(intPtr);
						Marshal.Release(intPtr);
					}
				}
			}
			catch (Exception exception)
			{
			}
			return objectForIUnknown;
		}

		public object GetService(Type serviceType)
		{
			object service = null;
			try
			{
				if (DTEHelperObject.DTEInstance != null)
				{
					service = this.GetService(DTEHelperObject.DTEInstance, serviceType.GUID);
				}
				if (service == null)
				{
					service = Package.GetGlobalService(serviceType);
				}
				if (!this.UIServiceChecked)
				{
					this.UIServiceChecked = true;
					this.ErrorListAvailable = service != null;
				}
			}
			catch (Exception exception)
			{
			}
			return service;
		}

		private bool IsTaskExists(string text)
		{
			bool flag;
			List<ErrorTask>.Enumerator enumerator = this.ErrorTaskList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ErrorTask current = enumerator.Current;
					if (string.IsNullOrEmpty(current.get_Text()) || !current.get_Text().Equals(text, StringComparison.InvariantCultureIgnoreCase))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		public void RemoveAllTasks()
		{
			if (this.ErrorProvider != null && this.ErrorTaskList.Count > 0)
			{
				this.RemoveAllTasksInternal();
			}
		}

		private void RemoveAllTasksInternal()
		{
			try
			{
				this.ErrorProvider.SuspendRefresh();
				foreach (ErrorTask errorTaskList in this.ErrorTaskList)
				{
					this.ErrorProvider.get_Tasks().Remove(errorTaskList);
				}
				this.ErrorTaskList.Clear();
				this.ErrorProvider.ResumeRefresh();
			}
			catch (Exception exception)
			{
			}
		}

		public void ReportError(string text, Project project)
		{
			this.ReportMessage(text, 0, project);
		}

		public void ReportError(string text)
		{
			this.ReportMessage(text, 0, null);
		}

		public void ReportError(string text, string document, int line, int column)
		{
			this.ReportMessage(text, document, line, column, 0, null);
		}

		public void ReportInfo(string text, Project project)
		{
			this.ReportMessage(text, 2, project);
		}

		public void ReportInfo(string text)
		{
			this.ReportMessage(text, 2, null);
		}

		private void ReportMessage(string text, TaskErrorCategory category, Project project)
		{
			if (this.IsTaskExists(text))
			{
				return;
			}
			if (this.ErrorProvider != null)
			{
				try
				{
					if (!this.IsErrorListVisible)
					{
						this.ErrorProvider.Show();
					}
					if (this.ErrorTaskList.Count == 0)
					{
						this.ActivateErrorListToolWindow();
					}
					IVsHierarchy vsHierarchy = null;
					if (project != null && !string.IsNullOrEmpty(project.UniqueName))
					{
						(this.GetService(typeof(IVsSolution)) as IVsSolution).GetProjectOfUniqueName(project.UniqueName, out vsHierarchy);
					}
					ErrorTask errorTask = new ErrorTask();
					errorTask.set_ErrorCategory(category);
					errorTask.set_HierarchyItem(vsHierarchy);
					errorTask.set_Document("Designer for Visual Studio WiX Setup Projects");
					errorTask.set_Text(text);
					this.ErrorTaskList.Add(errorTask);
					this.ErrorProvider.get_Tasks().Add(errorTask);
				}
				catch (Exception exception)
				{
				}
			}
		}

		private void ReportMessage(string text, string document, int line, int column, TaskErrorCategory category, Project project)
		{
			if (this.IsTaskExists(text))
			{
				return;
			}
			if (this.ErrorProvider != null)
			{
				try
				{
					if (!this.IsErrorListVisible)
					{
						this.ErrorProvider.Show();
					}
					if (this.ErrorTaskList.Count == 0)
					{
						this.ActivateErrorListToolWindow();
					}
					IVsHierarchy vsHierarchy = null;
					if (project != null && !string.IsNullOrEmpty(project.UniqueName))
					{
						(this.GetService(typeof(IVsSolution)) as IVsSolution).GetProjectOfUniqueName(project.UniqueName, out vsHierarchy);
					}
					ErrorTask errorTask = new ErrorTask();
					errorTask.set_ErrorCategory(category);
					errorTask.set_HierarchyItem(vsHierarchy);
					errorTask.set_Document(document);
					errorTask.set_Text(text);
					errorTask.set_Line(line);
					errorTask.set_Column(column);
					this.ErrorTaskList.Add(errorTask);
					this.ErrorProvider.get_Tasks().Add(errorTask);
				}
				catch (Exception exception)
				{
				}
			}
		}

		public void ReportWarning(string text, Project project)
		{
			this.ReportMessage(text, 1, project);
		}

		public void ReportWarning(string text)
		{
			this.ReportMessage(text, 1, null);
		}
	}
}