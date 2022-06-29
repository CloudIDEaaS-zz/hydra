using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace Utils
{
    public static class SecurityExtensions
    {
        [DllImport("dllmain.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SecureZeroMem(IntPtr ptr, uint cnt);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct CREDUI_INFO
        {
            public int cbSize;
            public IntPtr hwndParent;
            public string pszMessageText;
            public string pszCaptionText;
            public IntPtr hbmBanner;
        }

        [DllImport("credui.dll", CharSet = CharSet.Auto)]
        private static extern bool CredUnPackAuthenticationBuffer(int dwFlags, IntPtr pAuthBuffer, uint cbAuthBuffer, StringBuilder pszUserName, ref int pcchMaxUserName, StringBuilder pszDomainName, ref int pcchMaxDomainame, StringBuilder pszPassword, ref int pcchMaxPassword);
        [DllImport("credui.dll", CharSet = CharSet.Auto)]
        private static extern int CredUIPromptForWindowsCredentials(ref CREDUI_INFO notUsedHere, int authError, ref uint authPackage, IntPtr InAuthBuffer,uint InAuthBufferSize, out IntPtr refOutAuthBuffer, out uint refOutAuthBufferSize, ref bool fSave, int flags);

        public static bool IsUserAdministrator()
        {
            //bool value to hold our return value
            bool isAdmin;
            WindowsIdentity user = null;
            try
            {
                //get the currently logged in user
                user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            finally
            {
                if (user != null)
                    user.Dispose();
            }

            return isAdmin;
        }

        public static byte[] Pfx2Snk(byte[] pfxData, string pfxPassword)
        {
            // load .pfx
            var cert = new X509Certificate2(pfxData, pfxPassword, X509KeyStorageFlags.Exportable);

            // create .snk
            var privateKey = (RSACryptoServiceProvider)cert.PrivateKey;

            return privateKey.ExportCspBlob(true);
        }

        public static byte[] ToSnk(this X509Certificate2 cert)
        {
            var privateKey = (RSACryptoServiceProvider)cert.PrivateKey;

            return privateKey.ExportCspBlob(true);
        }

        public static string Protect(this string str, string additionalEntropy)
        {
            var bytes = ProtectedData.Protect(ASCIIEncoding.ASCII.GetBytes(str), ASCIIEncoding.ASCII.GetBytes(additionalEntropy), DataProtectionScope.CurrentUser);

            return bytes.ToBase64();
        }

        public static string Unprotect(this string str, string additionalEntropy)
        {
            var bytes = ProtectedData.Unprotect(str.FromBase64(), ASCIIEncoding.ASCII.GetBytes(additionalEntropy), DataProtectionScope.CurrentUser);

            return ASCIIEncoding.ASCII.GetString(bytes);
        }

        public static NetworkCredential GetCredentials(string serverName, string message)
        {
            NetworkCredential networkCredential = null;
            var credUIInfo = new CREDUI_INFO();
            var outCredBuffer = new IntPtr();
            var usernameBuilder = new StringBuilder(100);
            var passwordBuilder = new StringBuilder(100);
            var domainBuilder = new StringBuilder(100);
            var maxUserName = 100;
            var maxDomain = 100;
            var maxPassword = 100;
            int result;
            uint authPackage = 0;
            uint outCredSize;
            bool save = false;

            credUIInfo.pszCaptionText = "Please enter the credentails for " + serverName;
            credUIInfo.pszMessageText = message;
            credUIInfo.cbSize = Marshal.SizeOf(credUIInfo);

            result = CredUIPromptForWindowsCredentials(ref credUIInfo, 0, ref authPackage, IntPtr.Zero, 0, out outCredBuffer, out outCredSize, ref save, 0x00002);

            try
            {
                if (result == 0)
                {
                    try
                    {
                        if (CredUnPackAuthenticationBuffer(0, outCredBuffer, outCredSize, usernameBuilder, ref maxUserName, domainBuilder, ref maxDomain, passwordBuilder, ref maxPassword))
                        {
                            networkCredential = new NetworkCredential(usernameBuilder.ToString(), passwordBuilder.ToString(), domainBuilder.ToString());
                        }
                    }
                    finally
                    {
                        passwordBuilder.Clear();
                    }
                }
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(outCredBuffer);
            }

            return networkCredential;
        }
    }
}
