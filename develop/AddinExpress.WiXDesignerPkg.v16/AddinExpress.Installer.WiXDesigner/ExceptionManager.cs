using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class ExceptionManager
	{
		private const string _strDefaultMore = "No further information is available. If the problem persists, contact the Add-in Express support team: support@add-in-express.com";

		private static string ErrorText;

		private static Assembly _objParentAssembly;

		static ExceptionManager()
		{
			ExceptionManager.ErrorText = "'{0}' has fired an exception. Click the 'Details' button to see the detailed information about the error.";
			ExceptionManager._objParentAssembly = null;
		}

		public ExceptionManager()
		{
		}

		private static DateTime AssemblyBuildDate(Assembly objAssembly, bool blnForceFileDate)
		{
			DateTime dateTime;
			Version version = objAssembly.GetName().Version;
			if (!blnForceFileDate)
			{
				DateTime dateTime1 = DateTime.Parse("01/01/2000");
				dateTime1 = dateTime1.AddDays((double)version.Build);
				dateTime = dateTime1.AddSeconds((double)(version.Revision * 2));
				if (TimeZone.IsDaylightSavingTime(DateTime.Now, TimeZone.CurrentTimeZone.GetDaylightChanges(DateTime.Now.Year)))
				{
					dateTime = dateTime.AddHours(1);
				}
				if (dateTime > DateTime.Now || version.Build < 730 || version.Revision == 0)
				{
					dateTime = ExceptionManager.AssemblyFileTime(objAssembly);
				}
			}
			else
			{
				dateTime = ExceptionManager.AssemblyFileTime(objAssembly);
			}
			return dateTime;
		}

		private static DateTime AssemblyFileTime(Assembly objAssembly)
		{
			DateTime lastWriteTime;
			try
			{
				lastWriteTime = File.GetLastWriteTime(objAssembly.Location);
			}
			catch
			{
				lastWriteTime = DateTime.MaxValue;
			}
			return lastWriteTime;
		}

		private static string CurrentEnvironmentIdentity()
		{
			string empty;
			try
			{
				empty = string.Concat(Environment.UserDomainName, "\\", Environment.UserName);
			}
			catch
			{
				empty = string.Empty;
			}
			return empty;
		}

		private static string CurrentWindowsIdentity()
		{
			string name;
			try
			{
				name = WindowsIdentity.GetCurrent().Name;
			}
			catch
			{
				name = string.Empty;
			}
			return name;
		}

		private static string EnhancedStackTrace(StackTrace objStackTrace, string strSkipClassName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("---- Stack Trace ----");
			stringBuilder.Append(Environment.NewLine);
			for (int i = 0; i < objStackTrace.FrameCount; i++)
			{
				StackFrame frame = objStackTrace.GetFrame(i);
				MemberInfo method = frame.GetMethod();
				if (!(strSkipClassName != string.Empty) || method.DeclaringType.Name.IndexOf(strSkipClassName, StringComparison.InvariantCultureIgnoreCase) <= -1)
				{
					stringBuilder.Append(ExceptionManager.StackFrameToString(frame));
				}
			}
			stringBuilder.Append(Environment.NewLine);
			return stringBuilder.ToString();
		}

		private static string EnhancedStackTrace(Exception objException)
		{
			return ExceptionManager.EnhancedStackTrace(new StackTrace(objException, true), string.Empty);
		}

		private static string EnhancedStackTrace()
		{
			return ExceptionManager.EnhancedStackTrace(new StackTrace(true), "ExceptionManager");
		}

		private static string ExceptionToMore(Exception objException)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Concat("Detailed technical information follows: ", Environment.NewLine));
			stringBuilder.Append(string.Concat("---", Environment.NewLine));
			stringBuilder.Append(ExceptionManager.ExceptionToString(objException));
			return stringBuilder.ToString();
		}

		public static string ExceptionToString(Exception objException)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (objException.InnerException != null)
			{
				stringBuilder.Append("(Inner Exception)");
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(ExceptionManager.ExceptionToString(objException.InnerException));
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append("(Outer Exception)");
				stringBuilder.Append(Environment.NewLine);
			}
			stringBuilder.Append(ExceptionManager.SysInfoToString(objException, false));
			stringBuilder.Append("Exception Source:      ");
			try
			{
				stringBuilder.Append(objException.Source);
			}
			catch (Exception exception)
			{
				stringBuilder.Append(exception.Message);
			}
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("Exception Type:        ");
			try
			{
				stringBuilder.Append(objException.GetType().FullName);
			}
			catch (Exception exception1)
			{
				stringBuilder.Append(exception1.Message);
			}
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("Exception Message:     ");
			try
			{
				stringBuilder.Append(objException.Message);
			}
			catch (Exception exception2)
			{
				stringBuilder.Append(exception2.Message);
			}
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("Exception Target Site: ");
			try
			{
				stringBuilder.Append(objException.TargetSite.Name);
			}
			catch (Exception exception3)
			{
				stringBuilder.Append(exception3.Message);
			}
			stringBuilder.Append(Environment.NewLine);
			try
			{
				stringBuilder.Append(ExceptionManager.EnhancedStackTrace(objException));
			}
			catch (Exception exception4)
			{
				stringBuilder.Append(exception4.Message);
			}
			stringBuilder.Append(Environment.NewLine);
			return stringBuilder.ToString();
		}

		private static string GetCurrentIP()
		{
			string str;
			try
			{
				str = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
			}
			catch
			{
				str = "127.0.0.1";
			}
			return str;
		}

		private static string GetDefaultMore(Exception objException, string strMoreDetails)
		{
			if (strMoreDetails != string.Empty)
			{
				return strMoreDetails;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("No further information is available. If the problem persists, contact the Add-in Express support team: support@add-in-express.com");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(string.Concat("Basic technical information follows: ", Environment.NewLine));
			stringBuilder.Append(string.Concat("---", Environment.NewLine));
			stringBuilder.Append(ExceptionManager.SysInfoToString(objException, true));
			return stringBuilder.ToString();
		}

		public static string GetErrorText(Exception e)
		{
			return ExceptionManager.ExceptionToString(e);
		}

		private static Assembly ParentAssembly()
		{
			if (ExceptionManager._objParentAssembly == null)
			{
				if (Assembly.GetEntryAssembly() != null)
				{
					ExceptionManager._objParentAssembly = Assembly.GetEntryAssembly();
				}
				else
				{
					ExceptionManager._objParentAssembly = Assembly.GetCallingAssembly();
				}
			}
			return ExceptionManager._objParentAssembly;
		}

		public static DialogResult ShowDialog(IApplicationContext context, Exception objException, MessageBoxButtons buttons, MessageBoxIcon icon, ExceptionManager.UserErrorDefaultButton defaultButton)
		{
			return ExceptionManager.ShowDialog(context, null, objException, buttons, icon, defaultButton);
		}

		public static DialogResult ShowDialog(IApplicationContext context, object sender, Exception objException, MessageBoxButtons buttons, MessageBoxIcon icon, ExceptionManager.UserErrorDefaultButton defaultButton)
		{
			IWin32Window win32Window;
			string empty = string.Empty;
			if (context != null)
			{
				empty = context.ProductName;
				if (string.IsNullOrEmpty(empty))
				{
					empty = context.ModuleName;
					if (string.IsNullOrEmpty(empty))
					{
						empty = context.TypeName;
					}
				}
				return ExceptionManager.ShowDialogInternal(context.ParentWindowHandle, empty, ExceptionManager.ExceptionToMore(objException), buttons, icon, defaultButton);
			}
			if (sender != null)
			{
				object[] customAttributes = sender.GetType().Assembly.GetCustomAttributes(true);
				if (customAttributes.Length != 0)
				{
					object[] objArray = customAttributes;
					int num = 0;
					while (num < (int)objArray.Length)
					{
						Attribute attribute = (Attribute)objArray[num];
						if (!(attribute is AssemblyProductAttribute))
						{
							num++;
						}
						else
						{
							string product = (attribute as AssemblyProductAttribute).Product;
							if (string.IsNullOrEmpty(product))
							{
								break;
							}
							empty = product;
							break;
						}
					}
				}
				if (string.IsNullOrEmpty(empty))
				{
					empty = sender.GetType().FullName;
				}
			}
			if (string.IsNullOrEmpty(empty) && objException != null)
			{
				if (objException.InnerException != null)
				{
					empty = objException.InnerException.Source;
				}
				if (string.IsNullOrEmpty(empty))
				{
					empty = objException.Source;
				}
			}
			if (string.IsNullOrEmpty(empty))
			{
				empty = "Application error";
			}
			if (sender is IWin32Window)
			{
				win32Window = sender as IWin32Window;
			}
			else
			{
				win32Window = null;
			}
			return ExceptionManager.ShowDialogInternal(win32Window, empty, ExceptionManager.ExceptionToMore(objException), buttons, icon, defaultButton);
		}

		private static DialogResult ShowDialogInternal(IWin32Window parent, string applicationName, string strMoreDetails, MessageBoxButtons buttons, MessageBoxIcon icon, ExceptionManager.UserErrorDefaultButton defaultButton)
		{
			ErrorForm errorForm = new ErrorForm()
			{
				Text = applicationName
			};
			errorForm.ErrorBox.Text = string.Format(ExceptionManager.ErrorText, applicationName);
			errorForm.txtMore.Text = strMoreDetails;
			switch (buttons)
			{
				case MessageBoxButtons.OK:
				{
					errorForm.btn3.Text = "OK";
					errorForm.btn2.Visible = false;
					errorForm.btn1.Visible = false;
					errorForm.AcceptButton = errorForm.btn3;
					break;
				}
				case MessageBoxButtons.OKCancel:
				{
					errorForm.btn3.Text = "Cancel";
					errorForm.btn2.Text = "OK";
					errorForm.btn1.Visible = false;
					errorForm.AcceptButton = errorForm.btn2;
					errorForm.CancelButton = errorForm.btn3;
					break;
				}
				case MessageBoxButtons.AbortRetryIgnore:
				{
					errorForm.btn1.Text = "&Abort";
					errorForm.btn2.Text = "&Retry";
					errorForm.btn3.Text = "&Ignore";
					errorForm.AcceptButton = errorForm.btn2;
					errorForm.CancelButton = errorForm.btn3;
					break;
				}
				case MessageBoxButtons.YesNoCancel:
				{
					errorForm.btn3.Text = "Cancel";
					errorForm.btn2.Text = "&No";
					errorForm.btn1.Text = "&Yes";
					errorForm.CancelButton = errorForm.btn3;
					break;
				}
				case MessageBoxButtons.YesNo:
				{
					errorForm.btn3.Text = "&No";
					errorForm.btn2.Text = "&Yes";
					errorForm.btn1.Visible = false;
					break;
				}
				case MessageBoxButtons.RetryCancel:
				{
					errorForm.btn3.Text = "Cancel";
					errorForm.btn2.Text = "&Retry";
					errorForm.btn1.Visible = false;
					errorForm.AcceptButton = errorForm.btn2;
					errorForm.CancelButton = errorForm.btn3;
					break;
				}
			}
			if (icon <= MessageBoxIcon.Question)
			{
				if (icon == MessageBoxIcon.Hand)
				{
					errorForm.PictureBox1.Image = SystemIcons.Error.ToBitmap();
				}
				else
				{
					if (icon != MessageBoxIcon.Question)
					{
						goto Label0;
					}
					errorForm.PictureBox1.Image = SystemIcons.Question.ToBitmap();
				}
			}
			else if (icon == MessageBoxIcon.Exclamation)
			{
				errorForm.PictureBox1.Image = SystemIcons.Exclamation.ToBitmap();
			}
			else
			{
				if (icon != MessageBoxIcon.Asterisk)
				{
					goto Label0;
				}
				errorForm.PictureBox1.Image = SystemIcons.Information.ToBitmap();
			}
		Label3:
			switch (defaultButton)
			{
				case ExceptionManager.UserErrorDefaultButton.Button1:
				{
					errorForm.AcceptButton = errorForm.btn1;
					errorForm.btn1.TabIndex = 0;
					break;
				}
				case ExceptionManager.UserErrorDefaultButton.Button2:
				{
					errorForm.AcceptButton = errorForm.btn2;
					errorForm.btn2.TabIndex = 0;
					break;
				}
				case ExceptionManager.UserErrorDefaultButton.Button3:
				{
					errorForm.AcceptButton = errorForm.btn3;
					errorForm.btn3.TabIndex = 0;
					break;
				}
			}
			ServiceProvider serviceProvider = null;
			Microsoft.VisualStudio.OLE.Interop.IServiceProvider globalService = Package.GetGlobalService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)) as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
			if (globalService != null)
			{
				serviceProvider = new ServiceProvider(globalService);
			}
			if (serviceProvider != null)
			{
				IUIService service = (IUIService)serviceProvider.GetService(typeof(IUIService));
				if (service != null)
				{
					return service.ShowDialog(errorForm);
				}
			}
			if (parent == null)
			{
				return errorForm.ShowDialog();
			}
			return errorForm.ShowDialog(parent);
		Label0:
			errorForm.PictureBox1.Image = SystemIcons.Error.ToBitmap();
			goto Label3;
		}

		public static void ShowError(object sender, Exception e)
		{
			ExceptionManager.ShowDialog(null, sender, e, MessageBoxButtons.OK, MessageBoxIcon.Hand, ExceptionManager.UserErrorDefaultButton.Default);
		}

		private static string StackFrameToString(StackFrame sf)
		{
			StringBuilder stringBuilder = new StringBuilder();
			MemberInfo method = sf.GetMethod();
			stringBuilder.Append("   ");
			stringBuilder.Append(method.DeclaringType.Namespace);
			stringBuilder.Append(".");
			stringBuilder.Append(method.DeclaringType.Name);
			stringBuilder.Append(".");
			stringBuilder.Append(method.Name);
			ParameterInfo[] parameters = sf.GetMethod().GetParameters();
			stringBuilder.Append("(");
			int num = 0;
			ParameterInfo[] parameterInfoArray = parameters;
			for (int i = 0; i < (int)parameterInfoArray.Length; i++)
			{
				ParameterInfo parameterInfo = parameterInfoArray[i];
				num++;
				if (num > 1)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(parameterInfo.Name);
				stringBuilder.Append(" As ");
				stringBuilder.Append(parameterInfo.ParameterType.Name);
			}
			stringBuilder.Append(")");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("       ");
			if (sf.GetFileName() == null || sf.GetFileName().Length == 0)
			{
				stringBuilder.Append(Path.GetFileName(ExceptionManager.ParentAssembly().CodeBase));
				stringBuilder.Append(": N ");
				if (sf.GetILOffset() == -1)
				{
					stringBuilder.Append(string.Format("{0:#00000} (0x{0:X0000})", sf.GetNativeOffset()));
					stringBuilder.Append(" JIT ");
				}
				else
				{
					stringBuilder.Append(string.Format("{0:#0000} (0x{0:X0000})", sf.GetILOffset()));
					stringBuilder.Append(" IL ");
				}
			}
			else
			{
				stringBuilder.Append(Path.GetFileName(sf.GetFileName()));
				stringBuilder.Append(": line ");
				stringBuilder.Append(string.Format("{0:#0000}", sf.GetFileLineNumber()));
				stringBuilder.Append(", col ");
				stringBuilder.Append(string.Format("{0:#00}", sf.GetFileColumnNumber()));
				if (sf.GetILOffset() != -1)
				{
					stringBuilder.Append(", IL ");
					stringBuilder.Append(string.Format("{0:#0000} (0x{0:X0000})", sf.GetILOffset()));
				}
			}
			stringBuilder.Append(Environment.NewLine);
			return stringBuilder.ToString();
		}

		public static string SysInfoToString(Exception objException, bool blnIncludeStackTrace)
		{
			ExceptionManager._objParentAssembly = null;
			if (objException.TargetSite != null)
			{
				MethodBase targetSite = objException.TargetSite;
				if (targetSite.Module != null)
				{
					ExceptionManager._objParentAssembly = targetSite.Module.Assembly;
				}
				else if (targetSite.DeclaringType != null)
				{
					ExceptionManager._objParentAssembly = targetSite.DeclaringType.Assembly;
				}
				else if (targetSite.ReflectedType != null)
				{
					ExceptionManager._objParentAssembly = targetSite.ReflectedType.Assembly;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Date and Time:         ");
			stringBuilder.Append(DateTime.Now);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("Machine Name:          ");
			try
			{
				stringBuilder.Append(Environment.MachineName);
			}
			catch (Exception exception)
			{
				stringBuilder.Append(exception.Message);
			}
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("IP Address:            ");
			stringBuilder.Append(ExceptionManager.GetCurrentIP());
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("Current User:          ");
			stringBuilder.Append(ExceptionManager.UserIdentity());
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("Application Domain:    ");
			try
			{
				stringBuilder.Append(AppDomain.CurrentDomain.FriendlyName);
			}
			catch (Exception exception1)
			{
				stringBuilder.Append(exception1.Message);
			}
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("Assembly Codebase:     ");
			try
			{
				stringBuilder.Append(ExceptionManager.ParentAssembly().CodeBase);
			}
			catch (Exception exception2)
			{
				stringBuilder.Append(exception2.Message);
			}
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("Assembly Full Name:    ");
			try
			{
				stringBuilder.Append(ExceptionManager.ParentAssembly().FullName);
			}
			catch (Exception exception3)
			{
				stringBuilder.Append(exception3.Message);
			}
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("Assembly Version:      ");
			try
			{
				stringBuilder.Append(ExceptionManager.ParentAssembly().GetName().Version.ToString());
			}
			catch (Exception exception4)
			{
				stringBuilder.Append(exception4.Message);
			}
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(Environment.NewLine);
			if (blnIncludeStackTrace)
			{
				stringBuilder.Append(ExceptionManager.EnhancedStackTrace());
			}
			return stringBuilder.ToString();
		}

		private static string UserIdentity()
		{
			string str = ExceptionManager.CurrentWindowsIdentity();
			if (str == string.Empty)
			{
				str = ExceptionManager.CurrentEnvironmentIdentity();
			}
			return str;
		}

		public enum UserErrorDefaultButton
		{
			Default,
			Button1,
			Button2,
			Button3
		}
	}
}