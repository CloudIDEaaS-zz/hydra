using EnvDTE;
using EnvDTE80;
using System;
using System.IO;
using System.Text;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class OutputWindowHelper
	{
		public static int EnableLog;

		public static int WriteLogToFile;

		public static string LogFileLocation;

		static OutputWindowHelper()
		{
			OutputWindowHelper.EnableLog = 0;
			OutputWindowHelper.WriteLogToFile = 0;
			OutputWindowHelper.LogFileLocation = string.Empty;
		}

		public OutputWindowHelper()
		{
		}

		public static void Clear()
		{
			if (OutputWindowHelper.EnableLog > 0)
			{
				if (OutputWindowHelper.WriteLogToFile > 0)
				{
					if (!string.IsNullOrEmpty(OutputWindowHelper.LogFileLocation))
					{
						try
						{
							if (File.Exists(OutputWindowHelper.LogFileLocation))
							{
								File.Delete(OutputWindowHelper.LogFileLocation);
							}
						}
						catch (Exception exception)
						{
							DTEHelperObject.ReportError(exception.Message);
						}
					}
				}
				else if (DTEHelperObject.DTEInstance != null)
				{
					try
					{
						if (DTEHelperObject.DTEInstance.ToolWindows.OutputWindow != null)
						{
							OutputWindowPane outputWindowPane = DTEHelperObject.DTEInstance.ToolWindows.OutputWindow.OutputWindowPanes.Item("Build");
							if (outputWindowPane != null)
							{
								TextDocument textDocument = outputWindowPane.TextDocument;
								outputWindowPane.Clear();
							}
						}
					}
					catch (Exception exception1)
					{
						DTEHelperObject.ReportError(exception1.Message);
					}
				}
			}
		}

		public static string GetLogFilePath()
		{
			string str;
			try
			{
				string str1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Add-in Express");
				if (!Directory.Exists(str1))
				{
					Directory.CreateDirectory(str1);
				}
				str = Path.Combine(str1, "Designer4WiX.log");
			}
			catch (Exception exception)
			{
				return string.Empty;
			}
			return str;
		}

		public static void Write(string text)
		{
			OutputWindowHelper.WriteInternal(text);
		}

		private static void WriteInternal(string text)
		{
			if (OutputWindowHelper.EnableLog > 0)
			{
				if (OutputWindowHelper.WriteLogToFile > 0)
				{
					if (!string.IsNullOrEmpty(OutputWindowHelper.LogFileLocation))
					{
						try
						{
							using (StreamWriter streamWriter = new StreamWriter(OutputWindowHelper.LogFileLocation, true, Encoding.UTF8))
							{
								streamWriter.Write(text);
							}
						}
						catch (Exception exception)
						{
							DTEHelperObject.ReportError(exception.Message);
						}
					}
				}
				else if (DTEHelperObject.DTEInstance != null)
				{
					try
					{
						if (DTEHelperObject.DTEInstance.ToolWindows.OutputWindow != null)
						{
							OutputWindow outputWindow = DTEHelperObject.DTEInstance.ToolWindows.OutputWindow;
							Window parent = outputWindow.Parent;
							if (parent != null)
							{
								if (!parent.Visible)
								{
									parent.Visible = true;
								}
								parent.Activate();
							}
							OutputWindowPane outputWindowPane = outputWindow.OutputWindowPanes.Item("Build");
							if (outputWindowPane != null)
							{
								outputWindowPane.Activate();
								TextDocument textDocument = outputWindowPane.TextDocument;
								outputWindowPane.OutputString(text);
							}
						}
					}
					catch (Exception exception1)
					{
						DTEHelperObject.ReportError(exception1.Message);
					}
				}
			}
		}

		public static void WriteLine(string text)
		{
			if (OutputWindowHelper.WriteLogToFile > 0)
			{
				OutputWindowHelper.WriteInternal(string.Concat("\r\n", text));
				return;
			}
			OutputWindowHelper.WriteInternal(string.Concat("\r\nDesigner for Visual Studio WiX Setup Projects: ", text));
		}
	}
}