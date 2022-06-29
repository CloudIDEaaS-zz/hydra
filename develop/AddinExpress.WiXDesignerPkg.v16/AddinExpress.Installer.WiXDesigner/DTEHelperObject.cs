using EnvDTE80;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class DTEHelperObject
	{
		public static DTE2 DTEInstance;

		public static AddinExpress.Installer.WiXDesigner.ErrorList ErrorToolWindow;

		public static bool IsErrorToolWindowAvailable
		{
			get
			{
				if (DTEHelperObject.ErrorToolWindow == null)
				{
					return false;
				}
				return DTEHelperObject.ErrorToolWindow.ErrorListAvailable;
			}
		}

		static DTEHelperObject()
		{
		}

		public DTEHelperObject()
		{
		}

		public static string ExceptionToMore(Exception objException)
		{
			return DTEHelperObject.ExceptionToMore(string.Empty, objException);
		}

		public static string ExceptionToMore(string source, Exception objException)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string str = string.Concat("Designer for Visual Studio WiX Setup Projects error: ", (string.IsNullOrEmpty(source) ? string.Empty : string.Concat(source, " ")), objException.Message);
			stringBuilder.Append(string.Concat(str, Environment.NewLine));
			string errorText = ExceptionManager.GetErrorText(objException);
			if (!string.IsNullOrEmpty(errorText))
			{
				string empty = string.Empty;
				for (int i = 0; i < str.Length; i++)
				{
					empty = string.Concat(empty, "-");
				}
				stringBuilder.Append(string.Concat(empty, Environment.NewLine));
				stringBuilder.Append(errorText);
			}
			return stringBuilder.ToString();
		}

		public static void ReportError(string text)
		{
			if (DTEHelperObject.ErrorToolWindow != null && DTEHelperObject.ErrorToolWindow.ErrorListAvailable)
			{
				DTEHelperObject.ErrorToolWindow.ReportError(text);
			}
		}

		public static void ReportInfo(string text)
		{
			if (DTEHelperObject.ErrorToolWindow != null && DTEHelperObject.ErrorToolWindow.ErrorListAvailable)
			{
				DTEHelperObject.ErrorToolWindow.ReportInfo(text);
			}
		}

		public static void ReportWarning(string text)
		{
			if (DTEHelperObject.ErrorToolWindow != null && DTEHelperObject.ErrorToolWindow.ErrorListAvailable)
			{
				DTEHelperObject.ErrorToolWindow.ReportWarning(text);
			}
		}

		public static void ReportXmlError(XmlException xmlException, string filePath)
		{
			if (DTEHelperObject.ErrorToolWindow != null && DTEHelperObject.ErrorToolWindow.ErrorListAvailable)
			{
				DTEHelperObject.ErrorToolWindow.ReportError(xmlException.Message, Path.GetFileName(filePath), xmlException.LineNumber - 1, xmlException.LinePosition - 1);
			}
		}

		internal static void ShowErrorDialog(object sender, Exception e)
		{
			ExceptionManager.ShowDialog(null, sender, e, MessageBoxButtons.OK, MessageBoxIcon.Hand, ExceptionManager.UserErrorDefaultButton.Default);
		}

		public static void UnregisterErrorTask()
		{
			if (DTEHelperObject.ErrorToolWindow != null && DTEHelperObject.ErrorToolWindow.ErrorListAvailable)
			{
				DTEHelperObject.ErrorToolWindow.RemoveAllTasks();
			}
		}
	}
}