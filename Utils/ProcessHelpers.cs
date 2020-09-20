using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using System.Linq;

namespace Utils.ProcessHelpers
{
    public class OpenFileInfo
    {
        public string FileName { get; set; }
        public IntPtr FileHandle { get; set; }
    }

    [DebuggerDisplay(" { DebugInfo } ")]
    public class HandleInfo
    {
        public string ObjectName { get; set; }
        public string LocalPath { get; set; }
        public IntPtr Handle { get; set; }
        public HandleType HandleType { get; set; }

        public string DebugInfo
        {
            get
            {
                return string.Format("Handle: 0x{0:X}, "
                    + "HandleType: {1}, "
                    + "ObjectName: '{2}', "
                    + "LocalPath: '{3}', ",
                    this.Handle,
                    this.HandleType,
                    this.ObjectName,
                    this.LocalPath.AsDisplayText()
                );
            }
        }
    }

    internal enum NT_STATUS
    {
        STATUS_SUCCESS = 0x00000000,
        STATUS_BUFFER_OVERFLOW = unchecked((int)0x80000005L),
        STATUS_INFO_LENGTH_MISMATCH = unchecked((int)0xC0000004L)
    }

    internal enum SYSTEM_INFORMATION_CLASS
    {
        SystemBasicInformation = 0,
        SystemPerformanceInformation = 2,
        SystemTimeOfDayInformation = 3,
        SystemProcessInformation = 5,
        SystemProcessorPerformanceInformation = 8,
        SystemHandleInformation = 16,
        SystemInterruptInformation = 23,
        SystemExceptionInformation = 33,
        SystemRegistryQuotaInformation = 37,
        SystemLookasideInformation = 45
    }

    internal enum OBJECT_INFORMATION_CLASS
    {
        ObjectBasicInformation = 0,
        ObjectNameInformation = 1,
        ObjectTypeInformation = 2,
        ObjectAllTypesInformation = 3,
        ObjectHandleInformation = 4
    }

    [Flags]
    internal enum ProcessAccessRights
    {
        PROCESS_DUP_HANDLE = 0x00000040
    }

    [Flags]
    internal enum DuplicateHandleOptions
    {
        DUPLICATE_CLOSE_SOURCE = 0x1,
        DUPLICATE_SAME_ACCESS = 0x2
    }

    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    internal sealed class SafeObjectHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeObjectHandle() : base(true)
        { }

        internal SafeObjectHandle(IntPtr preexistingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            base.SetHandle(preexistingHandle);
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(base.handle);
        }
    }

    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    internal sealed class SafeProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeProcessHandle()
            : base(true)
        { }

        internal SafeProcessHandle(IntPtr preexistingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            base.SetHandle(preexistingHandle);
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(base.handle);
        }
    }

    internal static class NativeMethods
    {
        [DllImport("ntdll.dll")]
        internal static extern NT_STATUS NtQuerySystemInformation(
            [In] SYSTEM_INFORMATION_CLASS SystemInformationClass,
            [In] IntPtr SystemInformation,
            [In] int SystemInformationLength,
            [Out] out int ReturnLength);

        [DllImport("ntdll.dll")]
        internal static extern NT_STATUS NtQueryObject(
            [In] IntPtr Handle,
            [In] OBJECT_INFORMATION_CLASS ObjectInformationClass,
            [In] IntPtr ObjectInformation,
            [In] int ObjectInformationLength,
            [Out] out int ReturnLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern SafeProcessHandle OpenProcess(
            [In] ProcessAccessRights dwDesiredAccess,
            [In, MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            [In] int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DuplicateHandle(
            [In] IntPtr hSourceProcessHandle,
            [In] IntPtr hSourceHandle,
            [In] IntPtr hTargetProcessHandle,
            [Out] out SafeObjectHandle lpTargetHandle,
            [In] int dwDesiredAccess,
            [In, MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            [In] DuplicateHandleOptions dwOptions);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int GetProcessId(
            [In] IntPtr Process);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(
            [In] IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int QueryDosDevice(
            [In] string lpDeviceName,
            [Out] StringBuilder lpTargetPath,
            [In] int ucchMax);
    }

    public enum HandleType
    {
        OB_TYPE_UNKNOWN = 0,
        OB_TYPE_TYPE = 1,
        OB_TYPE_DIRECTORY,
        OB_TYPE_SYMBOLIC_LINK,
        OB_TYPE_TOKEN,
        OB_TYPE_PROCESS,
        OB_TYPE_THREAD,
        OB_TYPE_UNKNOWN_7,
        OB_TYPE_EVENT,
        OB_TYPE_EVENT_PAIR,
        OB_TYPE_MUTANT,
        OB_TYPE_UNKNOWN_11,
        OB_TYPE_SEMAPHORE,
        OB_TYPE_TIMER,
        OB_TYPE_PROFILE,
        OB_TYPE_WINDOW_STATION,
        OB_TYPE_DESKTOP,
        OB_TYPE_SECTION,
        OB_TYPE_KEY,
        OB_TYPE_PORT,
        OB_TYPE_WAITABLE_PORT,
        OB_TYPE_UNKNOWN_21,
        OB_TYPE_UNKNOWN_22,
        OB_TYPE_UNKNOWN_23,
        OB_TYPE_UNKNOWN_24,
        //OB_TYPE_CONTROLLER, 
        //OB_TYPE_DEVICE, 
        //OB_TYPE_DRIVER, 
        OB_TYPE_IO_COMPLETION,
        OB_TYPE_FILE
    };

    public static class Files
    {
        private static Dictionary<string, string> deviceMap;
        private const string networkDevicePrefix = "\\Device\\LanmanRedirector\\";

        private const int MAX_PATH = 260;


        private const int handleTypeTokenCount = 27;
        private static readonly string[] handleTypeTokens = new string[] {  
            "", "", "Directory", "SymbolicLink", "Token", 
            "Process", "Thread", "Unknown7", "Event", "EventPair", "Mutant", 
            "Unknown11", "Semaphore", "Timer", "Profile", "WindowStation", 
            "Desktop", "Section", "Key", "Port", "WaitablePort", 
            "Unknown21", "Unknown22", "Unknown23", "Unknown24",  
            "IoCompletion", "File" 
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_HANDLE_ENTRY
        {
            public int OwnerPid;
            public byte ObjectType;
            public byte HandleFlags;
            public short HandleValue;
            public int ObjectPointer;
            public int AccessMask;
        }

        /// <summary> 
        /// Gets the open files enumerator. 
        /// </summary> 
        /// <param name="processId">The process id.</param> 
        /// <returns></returns> 
        public static IEnumerable<OpenFileInfo> GetOpenFiles(int processId)
        {
            return new OpenFiles(processId);
        }

        public static IEnumerable<OpenFileInfo> GetOpenFilesFor(int processId, string file)
        {
            return new OpenFiles(processId, file);
        }

        private static string FindIndexedProcessName(int pid)
        {
            var processName = Process.GetProcessById(pid).ProcessName;
            var processesByName = Process.GetProcessesByName(processName);
            string processIndexdName = null;

            for (var index = 0; index < processesByName.Length; index++)
            {
                processIndexdName = index == 0 ? processName : processName + "#" + index;
                var processId = new PerformanceCounter("Process", "ID Process", processIndexdName);

                if ((int)processId.NextValue() == pid)
                {
                    return processIndexdName;
                }
            }

            return processIndexdName;
        }

        private static Process FindPidFromIndexedProcessName(string indexedProcessName)
        {
            var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);
            return Process.GetProcessById((int)parentId.NextValue());
        }

        public static Process GetParent(this Process process)
        {
            return FindPidFromIndexedProcessName(FindIndexedProcessName(process.Id));
        }

        public static IEnumerable<Process> FindLockingProcesses(this FileInfo fileInfo, Func<Process, bool> processFilter = null)
        {
            var processes = Process.GetProcesses().AsEnumerable();

            if (processFilter != null)
            {
                processes = processes.Where(p => processFilter(p));
            }

            foreach (var process in processes)
            {
                var hasOpen = false;

                try
                {
                    if (process.GetOpenFiles().Any(f => f.LocalPath != null && f.LocalPath.AsCaseless() == fileInfo.FullName))
                    {
                        hasOpen = true;
                    }
                }
                catch
                {
                }

                if (hasOpen)
                {
                    yield return process;
                }
            }
        }

        public static IEnumerable<Process> FindLockingProcesses(this DirectoryInfo directoryInfo, Func<Process, bool> processFilter = null)
        {
            var processes = Process.GetProcesses().AsEnumerable();

            if (processFilter != null)
            {
                processes = processes.Where(p => processFilter(p));
            }

            foreach (var process in processes)
            {
                var hasOpen = false;

                try
                {
                    if (process.GetOpenFiles().Any(f => f.LocalPath != null && f.LocalPath.AsCaseless().StartsWith(directoryInfo.FullName)))
                    {
                        hasOpen = true;
                    }
                }
                catch
                {
                }

                if (hasOpen)
                {
                    yield return process;
                }
            }
        }

        public static IEnumerable<HandleInfo> GetOpenFiles(this Process process)
        {
            return process.GetOpenHandles(HandleType.OB_TYPE_FILE);
        }
        public static IEnumerable<HandleInfo> GetOpenDirectories(this Process process)
        {
            return process.GetOpenHandles(HandleType.OB_TYPE_DIRECTORY);
        }

        public static IEnumerable<HandleInfo> GetOpenHandles(this Process process, HandleType type)
        {
            return process.GetOpenHandles().Where(h => h.HandleType == type);
        }

        public static IEnumerable<HandleInfo> GetOpenHandles(this Process process)
        {
            NT_STATUS returnValue;
            int length = 0x10000;

            do
            {
                IntPtr ptr = IntPtr.Zero;

                try
                {
                    ptr = Marshal.AllocHGlobal(length);
                    int returnLength;

                    returnValue = NativeMethods.NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS.SystemHandleInformation, ptr, length, out returnLength);

                    if (returnValue == NT_STATUS.STATUS_INFO_LENGTH_MISMATCH)
                    {
                        length = ((returnLength + 0xffff) & ~0xffff);
                    }
                    else if (returnValue == NT_STATUS.STATUS_SUCCESS)
                    {
                        int handleCount = Marshal.ReadInt32(ptr);
                        int offset = sizeof(int);
                        int size = Marshal.SizeOf(typeof(SYSTEM_HANDLE_ENTRY));

                        for (int i = 0; i < handleCount; i++)
                        {
                            SYSTEM_HANDLE_ENTRY handleEntry = (SYSTEM_HANDLE_ENTRY)Marshal.PtrToStructure((IntPtr)((int)ptr + offset), typeof(SYSTEM_HANDLE_ENTRY));

                            if (handleEntry.OwnerPid == process.Id)
                            {
                                var handle = (IntPtr)handleEntry.HandleValue;
                                HandleInfo handleInfo;

                                if (GetHandleInfo(handle, handleEntry.OwnerPid, out handleInfo))
                                {
                                    string localPath;

                                    if (handleInfo.HandleType.IsOneOf(HandleType.OB_TYPE_FILE, HandleType.OB_TYPE_DIRECTORY))
                                    {
                                        if (ConvertDevicePathToLocalPath(handleInfo.ObjectName, out localPath))
                                        {
                                            handleInfo.LocalPath = localPath;
                                        }
                                    }

                                    yield return handleInfo;
                                }
                            }

                            offset += size;
                        }
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
            while (returnValue == NT_STATUS.STATUS_INFO_LENGTH_MISMATCH);
        }

        private sealed class OpenFiles : IEnumerable<OpenFileInfo>
        {
            private readonly int processId;
            private string file;

            internal OpenFiles(int processId)
            {
                this.processId = processId;
            }

            public OpenFiles(int processId, string file)
            {
                this.processId = processId;
                this.file = file;
            }

            public IEnumerator<OpenFileInfo> GetEnumerator()
            {
                NT_STATUS ret;
                int length = 0x10000;
                // Loop, probing for required memory. 

                do
                {
                    IntPtr ptr = IntPtr.Zero;
                    RuntimeHelpers.PrepareConstrainedRegions();

                    try
                    {
                        RuntimeHelpers.PrepareConstrainedRegions();
                        
                        try 
                        { 
                        }
                        finally
                        {
                            // CER guarantees that the address of the allocated  
                            // memory is actually assigned to ptr if an  
                            // asynchronous exception occurs. 
                            ptr = Marshal.AllocHGlobal(length);
                        }
                        
                        int returnLength;

                        ret = NativeMethods.NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS.SystemHandleInformation, ptr, length, out returnLength);

                        if (ret == NT_STATUS.STATUS_INFO_LENGTH_MISMATCH)
                        {
                            // Round required memory up to the nearest 64KB boundary. 
                            length = ((returnLength + 0xffff) & ~0xffff);
                        }
                        else if (ret == NT_STATUS.STATUS_SUCCESS)
                        {
                            int handleCount = Marshal.ReadInt32(ptr);
                            int offset = sizeof(int);
                            int size = Marshal.SizeOf(typeof(SYSTEM_HANDLE_ENTRY));

                            for (int i = 0; i < handleCount; i++)
                            {
                                SYSTEM_HANDLE_ENTRY handleEntry = (SYSTEM_HANDLE_ENTRY) Marshal.PtrToStructure((IntPtr)((int)ptr + offset), typeof(SYSTEM_HANDLE_ENTRY));

                                if (handleEntry.OwnerPid == processId)
                                {
                                    IntPtr handle = (IntPtr)handleEntry.HandleValue;
                                    HandleType handleType;

                                    if (GetHandleType(handle, handleEntry.OwnerPid, out handleType) && handleType == HandleType.OB_TYPE_FILE)
                                    {
                                        string devicePath;

                                        if (GetFileNameFromHandle(handle, handleEntry.OwnerPid, out devicePath))
                                        {
                                            string dosPath;

                                            if (ConvertDevicePathToLocalPath(devicePath, out dosPath))
                                            {
                                                if (File.Exists(dosPath))
                                                {
                                                    if (file != null)
                                                    {
                                                        if (dosPath == file)
                                                        {
                                                            yield return new OpenFileInfo { FileName = dosPath, FileHandle = handle };
                                                        }
                                                    }
                                                    else
                                                    {
                                                        yield return new OpenFileInfo { FileName = dosPath, FileHandle = handle };
                                                    }
                                                }
                                                else if (Directory.Exists(dosPath))
                                                {
                                                    if (file != null)
                                                    {
                                                        if (dosPath == file)
                                                        {
                                                            yield return new OpenFileInfo { FileName = dosPath, FileHandle = handle };
                                                        }
                                                    }
                                                    else
                                                    {
                                                        yield return new OpenFileInfo { FileName = dosPath, FileHandle = handle };
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                offset += size;
                            }
                        }
                    }
                    finally
                    {
                        // CER guarantees that the allocated memory is freed,  
                        // if an asynchronous exception occurs.  
                        Marshal.FreeHGlobal(ptr);
                        //sw.Flush(); 
                        //sw.Close(); 
                    }
                }
                while (ret == NT_STATUS.STATUS_INFO_LENGTH_MISMATCH);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private static bool GetHandleInfo(IntPtr handle, int processId, out HandleInfo handleInfo)
        {
            SafeProcessHandle processHandle = null;
            SafeObjectHandle objectHandle = null;
            var currentProcess = NativeMethods.GetCurrentProcess();

            handleInfo = null;

            try
            {
                processHandle = NativeMethods.OpenProcess(ProcessAccessRights.PROCESS_DUP_HANDLE, true, processId);

                if (NativeMethods.DuplicateHandle(processHandle.DangerousGetHandle(), handle, currentProcess, out objectHandle, 0, false, DuplicateHandleOptions.DUPLICATE_SAME_ACCESS))
                {
                    handle = objectHandle.DangerousGetHandle();
                    int length;
                    NativeMethods.NtQueryObject(handle, OBJECT_INFORMATION_CLASS.ObjectTypeInformation, IntPtr.Zero, 0, out length);
                    IntPtr ptrObjectType = IntPtr.Zero;

                    try
                    {
                        if (length > 0)
                        {
                            ptrObjectType = Marshal.AllocHGlobal(length);

                            if (NativeMethods.NtQueryObject(handle, OBJECT_INFORMATION_CLASS.ObjectTypeInformation, ptrObjectType, length, out length) == NT_STATUS.STATUS_SUCCESS)
                            {
                                var token = Marshal.PtrToStringUni((IntPtr)((int)ptrObjectType + 0x60));
                                HandleType handleType;

                                if (GetHandleTypeFromToken(token, out handleType))
                                {
                                    IntPtr ptrObjectName = IntPtr.Zero;
                                    NT_STATUS returnValue;
                                    string objectName = null;

                                    try
                                    {

                                        length = 0x200;  // 512 bytes 

                                        ptrObjectName = Marshal.AllocHGlobal(length);
                                        returnValue = NativeMethods.NtQueryObject(handle, OBJECT_INFORMATION_CLASS.ObjectNameInformation, ptrObjectName, length, out length);

                                        if (returnValue == NT_STATUS.STATUS_BUFFER_OVERFLOW)
                                        {
                                            RuntimeHelpers.PrepareConstrainedRegions();
                                            Marshal.FreeHGlobal(ptrObjectName);
                                            ptrObjectName = Marshal.AllocHGlobal(length);

                                            returnValue = NativeMethods.NtQueryObject(handle, OBJECT_INFORMATION_CLASS.ObjectNameInformation, ptrObjectName, length, out length);
                                        }

                                        if (returnValue == NT_STATUS.STATUS_SUCCESS)
                                        {
                                            objectName = Marshal.PtrToStringUni((IntPtr)((int)ptrObjectName + 8), (length - 9) / 2);
                                        }
                                    }
                                    finally
                                    {
                                        Marshal.FreeHGlobal(ptrObjectName);
                                    }

                                    handleInfo = new HandleInfo
                                    {
                                        Handle = handle,
                                        HandleType = handleType,
                                        ObjectName = objectName
                                    };

                                    return true;
                                }
                            }
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptrObjectType);
                    }
                }
            }
            finally
            {
                if (processHandle != null)
                {
                    processHandle.Close();
                }

                if (objectHandle != null)
                {
                    objectHandle.Close();
                }
            }

            return false;
        }

        private static bool GetFileNameFromHandle(IntPtr handle, int processId, out string fileName)
        {
            IntPtr currentProcess = NativeMethods.GetCurrentProcess();
            bool remote = (processId != NativeMethods.GetProcessId(currentProcess));
            SafeProcessHandle processHandle = null;
            SafeObjectHandle objectHandle = null;

            try
            {
                if (remote)
                {
                    processHandle = NativeMethods.OpenProcess(ProcessAccessRights.PROCESS_DUP_HANDLE, true, processId);

                    if (NativeMethods.DuplicateHandle(processHandle.DangerousGetHandle(), handle, currentProcess, out objectHandle, 0, false, DuplicateHandleOptions.DUPLICATE_SAME_ACCESS))
                    {
                        handle = objectHandle.DangerousGetHandle();
                    }
                }

                return GetFileNameFromHandle(handle, out fileName, 200);
            }
            finally
            {
                if (remote)
                {
                    if (processHandle != null)
                    {
                        processHandle.Close();
                    }
                    if (objectHandle != null)
                    {
                        objectHandle.Close();
                    }
                }
            }
        }

        private static bool GetFileNameFromHandle(IntPtr handle, out string fileName, int wait)
        {
            using (FileNameFromHandleState f = new FileNameFromHandleState(handle))
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(GetFileNameFromHandle), f);

                if (f.WaitOne(wait))
                {
                    fileName = f.FileName;
                    return f.RetValue;
                }
                else
                {
                    fileName = string.Empty;
                    return false;
                }
            }
        }

        private class FileNameFromHandleState : IDisposable
        {
            private ManualResetEvent _mr;
            private IntPtr _handle;
            private string _fileName;
            private bool _retValue;

            public IntPtr Handle
            {
                get
                {
                    return _handle;
                }
            }

            public string FileName
            {
                get
                {
                    return _fileName;
                }
                set
                {
                    _fileName = value;
                }

            }

            public bool RetValue
            {
                get
                {
                    return _retValue;
                }
                set
                {
                    _retValue = value;
                }
            }

            public FileNameFromHandleState(IntPtr handle)
            {
                _mr = new ManualResetEvent(false);
                this._handle = handle;
            }

            public bool WaitOne(int wait)
            {
                return _mr.WaitOne(wait, false);
            }

            public void Set()
            {
                try
                {
                    _mr.Set();
                }
                catch { }
            }


            public void Dispose()
            {
                if (_mr != null)
                    _mr.Close();
            }
        }

        private static void GetFileNameFromHandle(object state)
        {
            FileNameFromHandleState s = (FileNameFromHandleState)state;
            string fileName;
            s.RetValue = GetFileNameFromHandle(s.Handle, out fileName);
            s.FileName = fileName;
            s.Set();
        }

        private static bool GetFileNameFromHandle(IntPtr handle, out string fileName)
        {
            IntPtr ptr = IntPtr.Zero;
            RuntimeHelpers.PrepareConstrainedRegions();

            try
            {
                int length = 0x200;  // 512 bytes 
                RuntimeHelpers.PrepareConstrainedRegions();

                try { }
                finally
                {
                    // CER guarantees the assignment of the allocated  
                    // memory address to ptr, if an ansynchronous exception  
                    // occurs. 
                    ptr = Marshal.AllocHGlobal(length);
                }

                NT_STATUS ret = NativeMethods.NtQueryObject(handle, OBJECT_INFORMATION_CLASS.ObjectNameInformation, ptr, length, out length);

                if (ret == NT_STATUS.STATUS_BUFFER_OVERFLOW)
                {
                    RuntimeHelpers.PrepareConstrainedRegions();

                    try { }
                    finally
                    {
                        // CER guarantees that the previous allocation is freed, 
                        // and that the newly allocated memory address is  
                        // assigned to ptr if an asynchronous exception occurs. 
                        Marshal.FreeHGlobal(ptr);
                        ptr = Marshal.AllocHGlobal(length);
                    }

                    ret = NativeMethods.NtQueryObject(handle, OBJECT_INFORMATION_CLASS.ObjectNameInformation, ptr, length, out length);
                }

                if (ret == NT_STATUS.STATUS_SUCCESS)
                {
                    fileName = Marshal.PtrToStringUni((IntPtr)((int)ptr + 8), (length - 9) / 2);
                    return fileName.Length != 0;
                }
            }
            finally
            {
                // CER guarantees that the allocated memory is freed,  
                // if an asynchronous exception occurs. 
                Marshal.FreeHGlobal(ptr);
            }

            fileName = string.Empty;
            return false;
        }

        private static bool GetHandleType(IntPtr handle, int processId, out HandleType handleType)
        {
            var token = GetHandleTypeToken(handle, processId);
            return GetHandleTypeFromToken(token, out handleType);
        }

        private static bool GetHandleType(IntPtr handle, out HandleType handleType)
        {
            string token = GetHandleTypeToken(handle);
            return GetHandleTypeFromToken(token, out handleType);
        }

        private static bool GetHandleTypeFromToken(string token, out HandleType handleType)
        {
            for (int i = 1; i < handleTypeTokenCount; i++)
            {
                if (handleTypeTokens[i] == token)
                {
                    handleType = (HandleType)i;
                    return true;
                }
            }
            handleType = HandleType.OB_TYPE_UNKNOWN;
            return false;
        }

        private static string GetHandleTypeToken(IntPtr handle, int processId)
        {
            var currentProcess = NativeMethods.GetCurrentProcess();
            var remote = (processId != NativeMethods.GetProcessId(currentProcess));
            SafeProcessHandle processHandle = null;
            SafeObjectHandle objectHandle = null;

            try
            {
                if (remote)
                {
                    processHandle = NativeMethods.OpenProcess(ProcessAccessRights.PROCESS_DUP_HANDLE, true, processId);

                    if (NativeMethods.DuplicateHandle(processHandle.DangerousGetHandle(), handle, currentProcess, out objectHandle, 0, false, DuplicateHandleOptions.DUPLICATE_SAME_ACCESS))
                    {
                        handle = objectHandle.DangerousGetHandle();
                    }
                }

                return GetHandleTypeToken(handle);
            }
            finally
            {
                if (remote)
                {
                    if (processHandle != null)
                    {
                        processHandle.Close();
                    }
                    if (objectHandle != null)
                    {
                        objectHandle.Close();
                    }
                }
            }
        }

        private static string GetHandleTypeToken(IntPtr handle)
        {
            int length;
            NativeMethods.NtQueryObject(handle, OBJECT_INFORMATION_CLASS.ObjectTypeInformation, IntPtr.Zero, 0, out length);
            IntPtr ptr = IntPtr.Zero;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                RuntimeHelpers.PrepareConstrainedRegions();

                if (length > 0)
                {
                    ptr = Marshal.AllocHGlobal(length);

                    if (NativeMethods.NtQueryObject(handle, OBJECT_INFORMATION_CLASS.ObjectTypeInformation, ptr, length, out length) == NT_STATUS.STATUS_SUCCESS)
                    {
                        return Marshal.PtrToStringUni((IntPtr)((int) ptr + 0x60));
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return string.Empty;
        }

        public static bool ConvertDevicePathToLocalPath(string devicePath, out string dosPath)
        {
            if (devicePath != null)
            {
                int index;

                EnsureDeviceMap();

                index = devicePath.Length;

                while (index > 0 && (index = devicePath.LastIndexOf('\\', index - 1)) != -1)
                {
                    string drive;

                    if (deviceMap.TryGetValue(devicePath.Substring(0, index), out drive))
                    {
                        dosPath = string.Concat(drive, devicePath.Substring(index));

                        return dosPath.Length != 0;
                    }
                }
            }

            dosPath = string.Empty;
            return false;
        }

        private static void EnsureDeviceMap()
        {
            if (deviceMap == null)
            {
                var localDeviceMap = BuildDeviceMap();

                Interlocked.CompareExchange<Dictionary<string, string>>(ref deviceMap, localDeviceMap, null);
            }
        }

        private static Dictionary<string, string> BuildDeviceMap()
        {
            var logicalDrives = Environment.GetLogicalDrives();
            var localDeviceMap = new Dictionary<string, string>(logicalDrives.Length);
            var lpTargetPath = new StringBuilder(MAX_PATH);

            foreach (string drive in logicalDrives)
            {
                string lpDeviceName = drive.Substring(0, 2);
                NativeMethods.QueryDosDevice(lpDeviceName, lpTargetPath, MAX_PATH);
                localDeviceMap.Add(NormalizeDeviceName(lpTargetPath.ToString()), lpDeviceName);
            }

            localDeviceMap.Add(networkDevicePrefix.Substring(0, networkDevicePrefix.Length - 1), "\\");

            return localDeviceMap;
        }

        private static string NormalizeDeviceName(string deviceName)
        {
            if (string.Compare(deviceName, 0, networkDevicePrefix, 0, networkDevicePrefix.Length, StringComparison.InvariantCulture) == 0)
            {
                string shareName = deviceName.Substring(deviceName.IndexOf('\\', networkDevicePrefix.Length) + 1);
                return string.Concat(networkDevicePrefix, shareName);
            }
            return deviceName;
        }
    }
}