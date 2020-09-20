using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using System.Security;
using System.Threading;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace Utils
{
    public enum LogonType
    {
        LOGON32_LOGON_INTERACTIVE = 2,
        LOGON32_LOGON_NETWORK = 3,
        LOGON32_LOGON_BATCH = 4,
        LOGON32_LOGON_SERVICE = 5,
        LOGON32_LOGON_UNLOCK = 7,
        LOGON32_LOGON_NETWORK_CLEARTEXT = 8, // Win2K or higher
        LOGON32_LOGON_NEW_CREDENTIALS = 9 // Win2K or higher
    };

    public enum LogonProvider
    {
        LOGON32_PROVIDER_DEFAULT = 0,
        LOGON32_PROVIDER_WINNT35 = 1,
        LOGON32_PROVIDER_WINNT40 = 2,
        LOGON32_PROVIDER_WINNT50 = 3
    };

    public enum ImpersonationLevel : int
    {
        SecurityAnonymous = 0,
        SecurityIdentification = 1,
        SecurityImpersonation = 2,
        SecurityDelegation = 3
    }

    class Win32NativeMethods
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LogonUser(string lpszUserName,
             string lpszDomain,
             IntPtr lpszPassword,
             int dwLogonType,
             int dwLogonProvider,
             out IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken,
              ImpersonationLevel impersonationLevel,
              ref IntPtr hNewToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int ImpersonateLoggedOnUser(
            IntPtr hToken
        );

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);
    }

    public class ImpersonationLogon : IDisposable
    {
        private IntPtr logonToken;
        private string userName;
        private string domainName;
        private SecureString password;
        private LogonType logonType;
        private LogonProvider logonProvider;
        private uint threadId;
        private IntPtr threadHandle;

        public ImpersonationLogon(string userName, string domainName, string password, LogonType logonType, LogonProvider logonProvider)
        {
            this.userName = userName;
            this.domainName = domainName;
            this.password = password.ToSecureString();
            this.logonType = logonType;
            this.logonProvider = logonProvider;
        }
 
        public ImpersonationLogon(string userName, string domainName, string password)
        {
            this.userName = userName;
            this.domainName = domainName;
            this.password = password.ToSecureString();
            this.logonType = LogonType.LOGON32_LOGON_INTERACTIVE;
            this.logonProvider = LogonProvider.LOGON32_PROVIDER_DEFAULT;
        }

        public string FullLogon
        {
            get
            {
                return domainName + @"\" + userName;
            }
        }

        public void RunImpersonated(Action action)
        {
            WindowsIdentity.RunImpersonated(new SafeAccessTokenHandle(logonToken), action);
        }

        public IDisposable Impersonate()
        {
            IntPtr passwordPtr = IntPtr.Zero;

            try
            {
                passwordPtr = Marshal.SecureStringToGlobalAllocAnsi(password);

                if (Win32NativeMethods.LogonUser(userName, domainName, passwordPtr, (int)logonType, (int)logonProvider, out logonToken) == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                if (Win32NativeMethods.ImpersonateLoggedOnUser(logonToken) == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(passwordPtr);
            }

            return this.AsDisposable(() => Revert());
        }

        public void Revert()
        {
            Win32NativeMethods.RevertToSelf();
        }

        public void Dispose()
        {
            if (logonToken != IntPtr.Zero)
            {
                Win32NativeMethods.RevertToSelf();
                Win32NativeMethods.CloseHandle(logonToken);
            }
        }

        public IDisposable CreateThread(ThreadStart action)
        {
            uint threadId;
            var duplicatedToken = IntPtr.Zero;

            Win32NativeMethods.DuplicateToken(logonToken, ImpersonationLevel.SecurityImpersonation, ref duplicatedToken);

            threadHandle = ThreadExtensions.CreateThread(() =>
            {
                if (!ThreadExtensions.SetThreadToken(IntPtr.Zero, duplicatedToken))
                {
                    throw new Exception("Unable to run thread as impersonated logon!");
                }

                action();

            }, out threadId);

            return new ThreadDisposable(threadHandle, threadId);
        }
    }
}
