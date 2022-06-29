using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class MsiHelper : IDisposable
	{
		private bool _disposed;

		private const int ERROR_SUCCESS = 0;

		private const int ERROR_NO_MORE_ITEMS = 259;

		public MsiHelper()
		{
		}

		public void Dispose()
		{
			if (!this._disposed)
			{
				this._disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		internal string FindStandardMSM(string id, string language, string version)
		{
			string str;
			string str1;
			string str2;
			string str3 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "Merge Modules");
			if (Directory.Exists(str3))
			{
				string[] files = Directory.GetFiles(str3, "*.msm");
				for (int i = 0; i < (int)files.Length; i++)
				{
					string str4 = files[i];
					this.GetModuleSignatureInfo(str4, out str, out str1, out str2);
					if (str.ToLower() == id.ToLower())
					{
						return str4;
					}
				}
			}
			return string.Empty;
		}

		internal List<ModuleDependency> GetModuleDependencies(string filePath)
		{
			List<ModuleDependency> moduleDependencies = new List<ModuleDependency>();
			if (File.Exists(filePath))
			{
				IntPtr zero = IntPtr.Zero;
				MsiHelper.MsiOpenDatabase(filePath, (IntPtr)0, out zero);
				if (zero != IntPtr.Zero)
				{
					IntPtr intPtr = IntPtr.Zero;
					MsiHelper.MsiDatabaseOpenView(zero, "SELECT `RequiredID`, `RequiredLanguage`, `RequiredVersion` FROM `ModuleDependency`", out intPtr);
					if (intPtr != IntPtr.Zero)
					{
						if (MsiHelper.MsiViewExecute(intPtr, IntPtr.Zero) == 0)
						{
							IntPtr zero1 = IntPtr.Zero;
							while (MsiHelper.MsiViewFetch(intPtr, out zero1) == 0)
							{
								if (zero1 == IntPtr.Zero)
								{
									continue;
								}
								StringBuilder stringBuilder = new StringBuilder((int)(MsiHelper.MsiRecordDataSize(zero1, 1) + 1));
								uint capacity = (uint)stringBuilder.Capacity;
								MsiHelper.MsiRecordGetString(zero1, 1, stringBuilder, ref capacity);
								string str = stringBuilder.ToString();
								stringBuilder = new StringBuilder((int)(MsiHelper.MsiRecordDataSize(zero1, 2) + 1));
								capacity = (uint)stringBuilder.Capacity;
								MsiHelper.MsiRecordGetString(zero1, 2, stringBuilder, ref capacity);
								string str1 = stringBuilder.ToString();
								stringBuilder = new StringBuilder((int)(MsiHelper.MsiRecordDataSize(zero1, 3) + 1));
								capacity = (uint)stringBuilder.Capacity;
								MsiHelper.MsiRecordGetString(zero1, 3, stringBuilder, ref capacity);
								string str2 = stringBuilder.ToString();
								string str3 = this.FindStandardMSM(str, str1, str2);
								moduleDependencies.Add(new ModuleDependency(str3, str, str1, str2));
								MsiHelper.MsiCloseHandle(zero1);
							}
						}
						MsiHelper.MsiCloseHandle(intPtr);
					}
					MsiHelper.MsiCloseHandle(zero);
				}
			}
			return moduleDependencies;
		}

		internal List<string> GetModuleFiles(string filePath)
		{
			List<string> strs = new List<string>();
			if (File.Exists(filePath))
			{
				IntPtr zero = IntPtr.Zero;
				MsiHelper.MsiOpenDatabase(filePath, (IntPtr)0, out zero);
				if (zero != IntPtr.Zero)
				{
					IntPtr intPtr = IntPtr.Zero;
					MsiHelper.MsiDatabaseOpenView(zero, "SELECT `FileName` FROM `File`", out intPtr);
					if (intPtr != IntPtr.Zero)
					{
						if (MsiHelper.MsiViewExecute(intPtr, IntPtr.Zero) == 0)
						{
							IntPtr zero1 = IntPtr.Zero;
							while (MsiHelper.MsiViewFetch(intPtr, out zero1) == 0)
							{
								if (zero1 == IntPtr.Zero)
								{
									continue;
								}
								StringBuilder stringBuilder = new StringBuilder((int)(MsiHelper.MsiRecordDataSize(zero1, 1) + 1));
								uint capacity = (uint)stringBuilder.Capacity;
								MsiHelper.MsiRecordGetString(zero1, 1, stringBuilder, ref capacity);
								string str = stringBuilder.ToString();
								if (string.IsNullOrEmpty(str) || !str.Contains("|"))
								{
									strs.Add(str);
								}
								else
								{
									strs.Add(str.Substring(str.IndexOf("|") + 1));
								}
								MsiHelper.MsiCloseHandle(zero1);
							}
						}
						MsiHelper.MsiCloseHandle(intPtr);
					}
					MsiHelper.MsiCloseHandle(zero);
				}
			}
			strs.Sort();
			return strs;
		}

		internal string GetModuleSignatureInfo(string filePath, uint index)
		{
			string str;
			if (File.Exists(filePath))
			{
				IntPtr zero = IntPtr.Zero;
				MsiHelper.MsiOpenDatabase(filePath, (IntPtr)0, out zero);
				if (zero != IntPtr.Zero)
				{
					try
					{
						IntPtr intPtr = IntPtr.Zero;
						MsiHelper.MsiDatabaseOpenView(zero, "SELECT * FROM `ModuleSignature`", out intPtr);
						if (intPtr == IntPtr.Zero)
						{
							return string.Empty;
						}
						else
						{
							try
							{
								if (MsiHelper.MsiViewExecute(intPtr, IntPtr.Zero) == 0)
								{
									IntPtr zero1 = IntPtr.Zero;
									if (MsiHelper.MsiViewFetch(intPtr, out zero1) == 0 && zero1 != IntPtr.Zero)
									{
										try
										{
											StringBuilder stringBuilder = new StringBuilder((int)(MsiHelper.MsiRecordDataSize(zero1, index) + 1));
											uint capacity = (uint)stringBuilder.Capacity;
											MsiHelper.MsiRecordGetString(zero1, index, stringBuilder, ref capacity);
											str = stringBuilder.ToString();
											return str;
										}
										finally
										{
											MsiHelper.MsiCloseHandle(zero1);
										}
									}
								}
								return string.Empty;
							}
							finally
							{
								MsiHelper.MsiCloseHandle(intPtr);
							}
						}
					}
					finally
					{
						MsiHelper.MsiCloseHandle(zero);
					}
					return str;
				}
			}
			return string.Empty;
		}

		internal void GetModuleSignatureInfo(string filePath, out string moduleSignature, out string language, out string version)
		{
			moduleSignature = string.Empty;
			language = string.Empty;
			version = string.Empty;
			if (File.Exists(filePath))
			{
				IntPtr zero = IntPtr.Zero;
				MsiHelper.MsiOpenDatabase(filePath, (IntPtr)0, out zero);
				if (zero != IntPtr.Zero)
				{
					try
					{
						IntPtr intPtr = IntPtr.Zero;
						MsiHelper.MsiDatabaseOpenView(zero, "SELECT * FROM `ModuleSignature`", out intPtr);
						if (intPtr != IntPtr.Zero)
						{
							try
							{
								if (MsiHelper.MsiViewExecute(intPtr, IntPtr.Zero) == 0)
								{
									IntPtr zero1 = IntPtr.Zero;
									if (MsiHelper.MsiViewFetch(intPtr, out zero1) == 0 && zero1 != IntPtr.Zero)
									{
										try
										{
											StringBuilder stringBuilder = new StringBuilder((int)(MsiHelper.MsiRecordDataSize(zero1, 1) + 1));
											uint capacity = (uint)stringBuilder.Capacity;
											MsiHelper.MsiRecordGetString(zero1, 1, stringBuilder, ref capacity);
											moduleSignature = stringBuilder.ToString();
											stringBuilder = new StringBuilder((int)(MsiHelper.MsiRecordDataSize(zero1, 2) + 1));
											capacity = (uint)stringBuilder.Capacity;
											MsiHelper.MsiRecordGetString(zero1, 2, stringBuilder, ref capacity);
											language = stringBuilder.ToString();
											stringBuilder = new StringBuilder((int)(MsiHelper.MsiRecordDataSize(zero1, 3) + 1));
											capacity = (uint)stringBuilder.Capacity;
											MsiHelper.MsiRecordGetString(zero1, 3, stringBuilder, ref capacity);
											version = stringBuilder.ToString();
											return;
										}
										finally
										{
											MsiHelper.MsiCloseHandle(zero1);
										}
									}
								}
							}
							finally
							{
								MsiHelper.MsiCloseHandle(intPtr);
							}
						}
					}
					finally
					{
						MsiHelper.MsiCloseHandle(zero);
					}
				}
			}
		}

		internal string GetModuleSummaryInfo(string filePath, MsiSummaryInfoProperties propName)
		{
			string property;
			if (File.Exists(filePath))
			{
				IntPtr zero = IntPtr.Zero;
				MsiHelper.MsiGetSummaryInformation(IntPtr.Zero, filePath, 0, out zero);
				if (zero != IntPtr.Zero)
				{
					try
					{
						if (propName != MsiSummaryInfoProperties.Template)
						{
							property = this.GetProperty(zero, propName);
						}
						else
						{
							string str = this.GetProperty(zero, propName);
							if (string.IsNullOrEmpty(str) || !str.Contains(";"))
							{
								return string.Empty;
							}
							else
							{
								property = str.Substring(str.IndexOf(";") + 1);
							}
						}
					}
					finally
					{
						MsiHelper.MsiCloseHandle(zero);
					}
					return property;
				}
			}
			return string.Empty;
		}

		private string GetProperty(IntPtr hSummary, MsiSummaryInfoProperties prop)
		{
			int num;
			System.Runtime.InteropServices.ComTypes.FILETIME fILETIME;
			VarEnum varEnum;
			string str;
			try
			{
				int capacity = 0;
				MsiHelper.MsiSummaryInfoGetProperty(hSummary, (int)prop, out varEnum, out num, out fILETIME, null, ref capacity);
				StringBuilder stringBuilder = new StringBuilder(capacity + 1);
				capacity = stringBuilder.Capacity;
				MsiHelper.MsiSummaryInfoGetProperty(hSummary, (int)prop, out varEnum, out num, out fILETIME, stringBuilder, ref capacity);
				str = stringBuilder.ToString();
			}
			catch
			{
				return string.Empty;
			}
			return str;
		}

		[DllImport("msi.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern uint MsiCloseHandle(IntPtr hAny);

		[DllImport("msi.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern uint MsiDatabaseOpenView(IntPtr hDatabase, string szQuery, out IntPtr phView);

		[DllImport("msi.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern uint MsiGetSummaryInformation(IntPtr hDatabase, string szDatabasePath, int uiUpdateCount, out IntPtr phSummaryInfo);

		[DllImport("msi.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern uint MsiOpenDatabase(string szDatabasePath, IntPtr szPersist, out IntPtr pDatabase);

		[DllImport("msi.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern uint MsiRecordDataSize(IntPtr hRecord, uint iField);

		[DllImport("msi.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern uint MsiRecordGetFieldCount(IntPtr hRecord);

		[DllImport("msi.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern int MsiRecordGetInteger(IntPtr hRecord, uint iField);

		[DllImport("msi.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern uint MsiRecordGetString(IntPtr hRecord, uint iField, StringBuilder szValueBuf, ref uint pcchValueBuf);

		[DllImport("msi.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern int MsiRecordIsNull(IntPtr hRecord, uint iField);

		[DllImport("msi.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern uint MsiSummaryInfoGetProperty(IntPtr hSummaryInfo, int uiProperty, out VarEnum puiDataType, out int piValue, out System.Runtime.InteropServices.ComTypes.FILETIME pftValue, StringBuilder szValueBuf, ref int pcchValueBuf);

		[DllImport("msi.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern uint MsiSummaryInfoGetPropertyCount(IntPtr hSummaryInfo, out int puiPropertyCount);

		[DllImport("msi.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern uint MsiViewExecute(IntPtr hView, IntPtr hRecord);

		[DllImport("msi.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern uint MsiViewFetch(IntPtr hView, out IntPtr phRecord);

		[Flags]
		private enum MsiOpenMode
		{
			MSIDBOPEN_READONLY = 0,
			MSIDBOPEN_TRANSACT = 1,
			MSIDBOPEN_DIRECT = 2,
			MSIDBOPEN_CREATE = 3,
			MSIDBOPEN_CREATEDIRECT = 4,
			MSIDBOPEN_PATCHFILE_W = 16,
			MSIDBOPEN_PATCHFILE_A = 32
		}
	}
}