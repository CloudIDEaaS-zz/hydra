using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

namespace Utils
{
    [Flags]
    public enum ThreadAccess : int
    {
        TERMINATE = (0x0001),
        SUSPEND_RESUME = (0x0002),
        GET_CONTEXT = (0x0008),
        SET_CONTEXT = (0x0010),
        SET_INFORMATION = (0x0020),
        QUERY_INFORMATION = (0x0040),
        SET_THREAD_TOKEN = (0x0080),
        IMPERSONATE = (0x0100),
        DIRECT_IMPERSONATION = (0x0200)
    }

    public static class ThreadExtensions
    {
        [DllImport("kernel32.dll")]
        public static extern bool TerminateThread(IntPtr hThread, uint dwExitCode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetThreadToken(IntPtr thread, IntPtr token);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private unsafe static extern IntPtr CreateThread(
                uint* lpThreadAttributes,
                uint dwStackSize,
                ThreadStart lpStartAddress,
                uint* lpParameter,
                uint dwCreationFlags,
                out uint lpThreadId);

        public static void Terminate(this Thread thread)
        {
            var nativeThreadId = thread.GetNaviveThreadId();
            var handle = OpenThread(ThreadAccess.TERMINATE, false, nativeThreadId);

            TerminateThread(handle, 0);
        }

        public static T LockGet<T>(this IManagedLockObject lockObject, Func<T> getter)
        {
            T value;

            using (lockObject.Lock())
            {
                value = getter();
            }

            return value;
        }

        public static void LockSet(this IManagedLockObject lockObject, Action setter)
        {
            using (lockObject.Lock())
            {
                setter();
            }
        }

        public unsafe static IntPtr CreateThread(ThreadStart threadStart, out uint lpThreadID)
        {
            uint a = 0;
            uint* lpThrAtt = &a;
            uint i = 0;
            uint* lpParam = &i;
            IntPtr dwHandle = CreateThread(null, (uint)0, threadStart, lpParam, 0, out lpThreadID);

            if (dwHandle == IntPtr.Zero)
            {
                throw new Exception("Unable to create thread!");
            }

            return dwHandle;
        }

        public static uint GetNaviveThreadId(this Thread thread)
        {
            var field = typeof(Thread).GetField("_DONT_USE_InternalThread", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);

            var pInternalThread = (IntPtr)field.GetValue(thread);
            var nativeId = Marshal.ReadInt32(pInternalThread, (7 * 64) + 24); // found by analyzing the memory

            return (uint) nativeId;
        }
    }
}
