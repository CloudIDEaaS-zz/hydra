using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace AddinExpress.Installer.Prerequisites
{
	internal class MsiUtils : IDisposable
	{
		private bool disposed;

		private const int Logon32LogonInteractive = 2;

		private const int Logon32ProviderDefault = 0;

		private List<MsiUtils.fileData> m_filesToAdd;

		private string m_logFileName;

		private object m_msiDatabase;

		private object m_msiObjectLib;

		private string m_msiPath;

		private string m_productCode;

		private bool m_TrackCabFiles;

		private const int msiOpenDatbaseModeReadOnly = 0;

		private const int msiOpenDatbaseModeTransact = 1;

		private const int msiViewModifyAssign = 3;

		private const int msiViewModifyInsert = 1;

		private const int msiViewModifyUpdate = 2;

		private List<MsiUtils.fileData> FilesToAdd
		{
			get
			{
				return this.m_filesToAdd;
			}
		}

		private string LogFileName
		{
			get
			{
				return this.m_logFileName;
			}
			set
			{
				this.m_logFileName = value;
			}
		}

		public string Platform
		{
			get
			{
				string str = Convert.ToString(MsiUtils.LateGet(MsiUtils.LateGet(this.m_msiDatabase, null, "SummaryInformation", new object[0]), null, "Property", new object[] { MsiUtils.SummaryProperty.pidPlatform }));
				return str.Remove(str.LastIndexOf(";"), str.Length - str.LastIndexOf(";"));
			}
		}

		public string ProductCode
		{
			get
			{
				if (this.m_productCode.Length == 0)
				{
					this.m_productCode = this.GetPropertyTableValue("ProductCode");
				}
				return this.m_productCode;
			}
		}

		public MsiUtils.InstallStatus ProductState
		{
			get
			{
				return (MsiUtils.InstallStatus)Convert.ToInt32(MsiUtils.LateGet(this.m_msiObjectLib, null, "ProductState", new object[] { this.ProductCode }));
			}
		}

		public MsiUtils(string msiPath) : this(msiPath, null)
		{
		}

		public MsiUtils(string msiPath, string productCode)
		{
			this.m_productCode = "";
			this.m_msiPath = "";
			this.m_TrackCabFiles = true;
			this.m_filesToAdd = new List<MsiUtils.fileData>();
			Type typeFromProgID = Type.GetTypeFromProgID("WindowsInstaller.Installer", true);
			this.m_msiObjectLib = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(typeFromProgID));
			if (!File.Exists(msiPath))
			{
				throw new MsiUtils.Exceptions.MsiFileNotFoundException(string.Concat("MsiUtils.New: File ", msiPath, " was not found."));
			}
			this.m_msiPath = msiPath;
			if (productCode != null && productCode.Length > 0)
			{
				this.m_productCode = productCode;
			}
			this.OpenDatabase();
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.disposed = true;
				if (this.m_msiDatabase != null)
				{
					Marshal.ReleaseComObject(this.m_msiDatabase);
					this.m_msiDatabase = null;
				}
				if (this.m_msiObjectLib != null)
				{
					Marshal.ReleaseComObject(this.m_msiObjectLib);
					this.m_msiObjectLib = null;
				}
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		private int FieldColumn(string fieldName, object table)
		{
			int num = -1;
			object objectValue = null;
			try
			{
				objectValue = RuntimeHelpers.GetObjectValue(MsiUtils.LateGet(table, null, "ColumnInfo", new object[] { 0 }));
				int num1 = Convert.ToInt32(MsiUtils.LateGet(objectValue, null, "FieldCount", new object[0]));
				for (int i = 1; i <= num1; i++)
				{
					object[] objArray = new object[] { i };
					i = Convert.ToInt32(objArray[0]);
					if (Convert.ToString(MsiUtils.LateGet(objectValue, null, "StringData", objArray)) == fieldName)
					{
						num = i;
					}
				}
			}
			finally
			{
				if (objectValue != null)
				{
					Marshal.ReleaseComObject(objectValue);
				}
			}
			return num;
		}

		public string GetPropertyTableValue(string propertyName)
		{
			return this.GetValueFromTable("Property", "Property", propertyName, "Value");
		}

		public object GetRecordFromTable(object table, string keyColumnName, string keyValue)
		{
			return this.GetRecordFromTable(table, keyColumnName, keyValue, true);
		}

		public object GetRecordFromTable(object table, string keyColumnName, string keyValue, bool caseSensitive)
		{
			int num = this.FieldColumn(keyColumnName, table);
			if (!caseSensitive)
			{
				keyValue = keyValue.ToLower(CultureInfo.InvariantCulture);
			}
			bool flag = false;
			object i = null;
			try
			{
				for (i = RuntimeHelpers.GetObjectValue(MsiUtils.LateCall(table, null, "Fetch", new object[0])); i != null; i = RuntimeHelpers.GetObjectValue(MsiUtils.LateCall(table, null, "Fetch", new object[0])))
				{
					object[] objArray = new object[] { num };
					num = Convert.ToInt32(objArray[0]);
					string str = Convert.ToString(MsiUtils.LateGet(i, null, "StringData", objArray));
					if (caseSensitive)
					{
						if (str == keyValue)
						{
							flag = true;
							return i;
						}
					}
					else if (str.ToLower() == keyValue.ToLower())
					{
						flag = true;
						return i;
					}
					Marshal.ReleaseComObject(i);
				}
			}
			finally
			{
				if (i != null && !flag)
				{
					Marshal.ReleaseComObject(i);
				}
			}
			return i;
		}

		public object GetTable(string tableName)
		{
			return this.GetTable("*", tableName, "");
		}

		public object GetTable(string selectClause, string tableName, string whereClause)
		{
			object obj;
			string str = "";
			try
			{
				if (string.IsNullOrEmpty(selectClause) || selectClause.Length <= 0)
				{
					str = "SELECT *";
				}
				else
				{
					str = (!selectClause.ToLower(CultureInfo.InvariantCulture).StartsWith("select") ? string.Concat("SELECT ", selectClause) : selectClause);
				}
				str = string.Concat(str, " FROM ", tableName);
				if (!string.IsNullOrEmpty(whereClause) && whereClause.Length > 0)
				{
					str = (!whereClause.ToLower(CultureInfo.InvariantCulture).StartsWith("where") ? string.Concat(str, " WHERE ", whereClause) : string.Concat(str, " ", selectClause));
				}
				object[] objArray = new object[] { str };
				str = Convert.ToString(objArray[0]);
				object objectValue = RuntimeHelpers.GetObjectValue(MsiUtils.LateCall(this.m_msiDatabase, null, "OpenView", objArray));
				MsiUtils.LateCall(objectValue, null, "Execute", new object[0]);
				obj = objectValue;
			}
			catch (Exception exception)
			{
				throw;
			}
			return obj;
		}

		public string GetValueFromRecord(object table, object record, string columnName)
		{
			int num = this.FieldColumn(columnName, table);
			object[] objArray = new object[] { num };
			num = Convert.ToInt32(objArray[0]);
			return Convert.ToString(MsiUtils.LateGet(record, null, "StringData", objArray));
		}

		public string GetValueFromTable(string tableName, string keyColumnName, string keyValue, string valueColumnName)
		{
			string valueFromRecord;
			object table = null;
			object recordFromTable = null;
			try
			{
				table = this.GetTable(tableName);
				recordFromTable = this.GetRecordFromTable(table, keyColumnName, keyValue);
				valueFromRecord = this.GetValueFromRecord(table, recordFromTable, valueColumnName);
			}
			finally
			{
				if (recordFromTable != null)
				{
					Marshal.ReleaseComObject(recordFromTable);
				}
				if (table != null)
				{
					Marshal.ReleaseComObject(table);
				}
			}
			return valueFromRecord;
		}

		private static object LateCall(object Instance, System.Type Type, string MemberName, object[] Arguments)
		{
			if (Type != null)
			{
				return Type.InvokeMember(MemberName, BindingFlags.InvokeMethod, null, Instance, Arguments);
			}
			return Instance.GetType().InvokeMember(MemberName, BindingFlags.InvokeMethod, null, Instance, Arguments);
		}

		private static object LateGet(object Instance, System.Type Type, string MemberName, object[] Arguments)
		{
			if (Type != null)
			{
				return Type.InvokeMember(MemberName, BindingFlags.InvokeMethod, null, Instance, Arguments);
			}
			return Instance.GetType().InvokeMember(MemberName, BindingFlags.GetProperty, null, Instance, Arguments);
		}

		private static void LateSet(object Instance, System.Type Type, string MemberName, object[] Arguments)
		{
			if (Type != null)
			{
				Type.InvokeMember(MemberName, BindingFlags.InvokeMethod, null, Instance, Arguments);
			}
			Instance.GetType().InvokeMember(MemberName, BindingFlags.InvokeMethod, null, Instance, Arguments);
		}

		public static List<MsiUtils.InstalledProductInfo> ListInstalledProducts()
		{
			List<MsiUtils.InstalledProductInfo> installedProductInfos = new List<MsiUtils.InstalledProductInfo>();
			object objectValue = null;
			IEnumerator enumerator = null;
			try
			{
				objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(Type.GetTypeFromProgID("WindowsInstaller.Installer", true)));
				enumerator = ((IEnumerable)MsiUtils.LateGet(objectValue, null, "Products", new object[0])).GetEnumerator();
				while (enumerator.MoveNext())
				{
					string str = Convert.ToString(enumerator.Current);
					MsiUtils.InstalledProductInfo installedProductInfo = new MsiUtils.InstalledProductInfo();
					object[] objArray = new object[] { str, "InstalledProductName" };
					str = Convert.ToString(objArray[0]);
					installedProductInfo.Name = Convert.ToString(MsiUtils.LateGet(objectValue, null, "ProductInfo", objArray));
					installedProductInfo.Code = str;
					installedProductInfos.Add(installedProductInfo);
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
				if (objectValue != null)
				{
					Marshal.ReleaseComObject(objectValue);
				}
			}
			return installedProductInfos;
		}

		public static List<MsiUtils.InstalledProductInfo> ListInstalledProducts2()
		{
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder(39);
			List<MsiUtils.InstalledProductInfo> installedProductInfos = new List<MsiUtils.InstalledProductInfo>();
			while (true)
			{
				int num1 = num;
				num = num1 + 1;
				if (MsiUtils.MsiEnumProducts(num1, stringBuilder) != 0)
				{
					break;
				}
				int num2 = 512;
				StringBuilder stringBuilder1 = new StringBuilder(num2);
				if (MsiUtils.MsiGetProductInfo(stringBuilder.ToString(), "ProductName", stringBuilder1, ref num2) == 0)
				{
					MsiUtils.InstalledProductInfo installedProductInfo = new MsiUtils.InstalledProductInfo()
					{
						Name = stringBuilder1.ToString(),
						Code = stringBuilder.ToString()
					};
					installedProductInfos.Add(installedProductInfo);
				}
			}
			return installedProductInfos;
		}

		[DllImport("msi.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern int MsiEnumProducts(int iProductIndex, StringBuilder lpProductBuf);

		[DllImport("msi.dll", CharSet=CharSet.Unicode, ExactSpelling=false)]
		private static extern int MsiGetProductInfo(string product, string property, StringBuilder valueBuf, ref int len);

		private void OpenDatabase()
		{
			object[] mMsiPath = new object[] { this.m_msiPath, 1 };
			this.m_msiPath = Convert.ToString(RuntimeHelpers.GetObjectValue(mMsiPath[0]));
			this.m_msiDatabase = RuntimeHelpers.GetObjectValue(MsiUtils.LateCall(this.m_msiObjectLib, null, "OpenDatabase", mMsiPath));
		}

		public class Exceptions
		{
			public Exceptions()
			{
			}

			[Serializable]
			public class ComponentNotFoundException : MsiUtils.Exceptions.MsiUtilsException
			{
				public ComponentNotFoundException(string message) : base(message)
				{
				}

				public ComponentNotFoundException(string message, Exception innerException) : base(message, innerException)
				{
				}
			}

			[Serializable]
			public class InstallFailedException : MsiUtils.Exceptions.MsiUtilsException
			{
				public InstallFailedException(string message) : base(message)
				{
				}

				public InstallFailedException(string message, Exception innerException) : base(message, innerException)
				{
				}
			}

			[Serializable]
			public class MsiFileNotFoundException : MsiUtils.Exceptions.MsiUtilsException
			{
				public MsiFileNotFoundException(string message) : base(message)
				{
				}

				public MsiFileNotFoundException(string message, Exception innerException) : base(message, innerException)
				{
				}
			}

			[Serializable]
			public abstract class MsiUtilsException : Exception
			{
				protected MsiUtilsException(string message) : base(message)
				{
				}

				public MsiUtilsException(string message, Exception innerException) : base(message, innerException)
				{
				}
			}

			[Serializable]
			public class MultipleRecordsException : Exception
			{
				public MultipleRecordsException(string message) : base(message)
				{
				}

				public MultipleRecordsException(string message, Exception innerException) : base(message, innerException)
				{
				}
			}

			[Serializable]
			public class ProductAlreadyInstalledException : MsiUtils.Exceptions.MsiUtilsException
			{
				public ProductAlreadyInstalledException(string message) : base(message)
				{
				}

				public ProductAlreadyInstalledException(string message, Exception innerException) : base(message, innerException)
				{
				}
			}

			[Serializable]
			public class ProductNotInstalledException : MsiUtils.Exceptions.MsiUtilsException
			{
				public ProductNotInstalledException(string message) : base(message)
				{
				}

				public ProductNotInstalledException(string message, Exception innerException) : base(message, innerException)
				{
				}
			}

			[Serializable]
			public class RepairFailedException : MsiUtils.Exceptions.MsiUtilsException
			{
				public RepairFailedException(string message) : base(message)
				{
				}

				public RepairFailedException(string message, Exception innerException) : base(message, innerException)
				{
				}
			}

			[Serializable]
			public class UnexpectedFailureException : MsiUtils.Exceptions.MsiUtilsException
			{
				public UnexpectedFailureException(string message) : base(message)
				{
				}

				public UnexpectedFailureException(string message, Exception innerException) : base(message, innerException)
				{
				}
			}

			[Serializable]
			public class ValueNotFoundException : Exception
			{
				public ValueNotFoundException(string message) : base(message)
				{
				}

				public ValueNotFoundException(string message, Exception innerException) : base(message, innerException)
				{
				}
			}
		}

		private class fileData
		{
			public string fileKey;

			public FileInfo filePath;

			public fileData(FileInfo misPath, string key)
			{
				this.filePath = misPath;
				this.fileKey = key;
			}
		}

		public struct InstalledProductInfo
		{
			public string Name;

			public string Code;
		}

		public enum InstallStatus
		{
			InstallStateUnknown = -1,
			InstallStateAdvertised = 1,
			InstallStateAbsent = 2,
			InstallStateInstalled = 5
		}

		public class MSIFolder
		{
			public string DefaultName;

			public string InternalDirectoryName;

			private MsiUtils.MSIFolder m_parent;

			public MsiUtils.MSIFolder ParentFolder
			{
				get
				{
					return this.m_parent;
				}
			}

			public MSIFolder(MsiUtils.MSIFolder parent, string name)
			{
				this.m_parent = parent;
				this.InternalDirectoryName = name;
				this.DefaultName = name;
			}
		}

		public enum ReinstallModes
		{
			FileMissing = 2,
			FileOlderVersion = 4,
			FileEqualVersion = 8,
			FileExact = 16,
			FileVerify = 32,
			FileReplace = 64,
			Machinedata = 128,
			Userdata = 256,
			Shortcut = 512,
			Package = 1024
		}

		public enum SummaryProperty
		{
			pidCodepage = 1,
			pidTitle = 2,
			pidSubject = 3,
			pidAuthor = 4,
			pidKeywords = 5,
			pidComments = 6,
			pidPlatform = 7
		}

		public enum UILevel
		{
			None = 2,
			Full = 5
		}

		public enum UserModes
		{
			Administrator,
			CurrentUser,
			LocalUser
		}
	}
}