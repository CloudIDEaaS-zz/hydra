using Microsoft.Win32;
using System;
using System.Globalization;
using System.Text;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class EvaluationChecker
	{
		public EvaluationChecker()
		{
		}

		public static bool Initialize(int days)
		{
			bool flag = false;
			using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Add-in Express\\ADX Designer for WiX 2 Trial\\"))
			{
				if (registryKey != null)
				{
					flag = true;
					if (registryKey.GetValue("ProductID", null) == null)
					{
						string[] str = new string[] { string.Format("0x{0:X};", days), null, null, null, null, null };
						int day = DateTime.Now.Day;
						str[1] = day.ToString();
						str[2] = "/";
						day = DateTime.Now.Month;
						str[3] = day.ToString();
						str[4] = "/";
						day = DateTime.Now.Year;
						str[5] = day.ToString();
						string str1 = string.Concat(str);
						registryKey.SetValue("ProductID", Encoding.Unicode.GetBytes(str1));
					}
				}
			}
			return flag;
		}

		public static bool ValidateLicense()
		{
			bool flag;
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Add-in Express\\ADX Designer for WiX 2 Trial\\", true))
			{
				if (registryKey != null)
				{
					object value = registryKey.GetValue("ProductID", null);
					if (value is byte[])
					{
						try
						{
							string[] strArrays = Encoding.Unicode.GetString((byte[])value).Split(new char[] { ';' });
							if ((int)strArrays.Length == 2)
							{
								int days = int.Parse(strArrays[0].Replace("0x", string.Empty), NumberStyles.HexNumber);
								if (days > 0)
								{
									strArrays = strArrays[1].Split(new char[] { '/' });
									if ((int)strArrays.Length == 3)
									{
										DateTime dateTime = new DateTime(int.Parse(strArrays[2]), int.Parse(strArrays[1]), int.Parse(strArrays[0]));
										if (dateTime <= DateTime.Now)
										{
											TimeSpan now = DateTime.Now - dateTime;
											if (now.Days != 0)
											{
												days -= now.Days;
												if (days < 0)
												{
													days = 0;
												}
												string[] str = new string[] { string.Format("0x{0:X};", days), null, null, null, null, null };
												int day = DateTime.Now.Day;
												str[1] = day.ToString();
												str[2] = "/";
												day = DateTime.Now.Month;
												str[3] = day.ToString();
												str[4] = "/";
												day = DateTime.Now.Year;
												str[5] = day.ToString();
												string str1 = string.Concat(str);
												registryKey.SetValue("ProductID", Encoding.Unicode.GetBytes(str1));
												if (days > 0)
												{
													flag = true;
													return flag;
												}
											}
											else
											{
												flag = true;
												return flag;
											}
										}
									}
								}
							}
							return false;
						}
						catch (Exception exception)
						{
							return false;
						}
					}
				}
			}
			return false;
		}
	}
}