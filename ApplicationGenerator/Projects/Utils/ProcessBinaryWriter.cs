#if INCLUDE_PROCESSDIAGNOSTICSLIBRARY
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace System.IO
{
    using System;
    using System.Runtime;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Diagnostics;
    using System.ComponentModel;
    using ProcessDiagnosticsLibrary;

    public class ProcessBinaryWriter : BinaryWriter
    {
        private Diagnostics.Process process;
        private ulong baseAddress;
        private uint hProcess;
        private ProcessStream baseStream;
        private static ProcessExtensions.SYSTEM_INFO systemInfo;
        private Dictionary<ulong, ProcessBinaryReader> regions;
        private IProcessDiagnostics processDiagnostics;

        static ProcessBinaryWriter()
        {
            ProcessExtensions.GetSystemInfo(out systemInfo);
        }

        private ProcessBinaryWriter(IntPtr baseAddress, Stream inputStream) : base(inputStream)
        {
            this.baseStream = (ProcessStream)this.BaseStream;
            this.baseAddress = (ulong)unchecked(baseAddress.ToInt64());
            this.processDiagnostics = new ProcessDiagnosticsWrapper();

            this.baseStream.ProcessDiagnostics = processDiagnostics;

            this.regions = new Dictionary<ulong, ProcessBinaryReader>();
        }

        public unsafe ProcessBinaryWriter(uint hProcess, IntPtr baseAddress, ulong size) : this(baseAddress, InitializeStream(hProcess, baseAddress, size))
        {
            baseStream.OnSeekOverun += new SeekOverunHandler(OnSeekOverun);
            this.hProcess = hProcess;
        }

        public unsafe ProcessBinaryWriter(uint hProcess, IntPtr baseAddress, ulong size, IProcessDiagnostics processDiagnostics) : this(hProcess, baseAddress, size)
        {
            this.processDiagnostics = new ProcessDiagnosticsWrapper(processDiagnostics);
            this.baseStream.ProcessDiagnostics = processDiagnostics;
        }

        public unsafe ProcessBinaryWriter(Diagnostics.Process process, IntPtr baseAddress, ulong size) : this(baseAddress, InitializeStream((uint) process.Handle, baseAddress, size))
        {
            this.process = process;
            this.hProcess = (uint) process.Handle;
        }

        public override void Close()
        {
            ProcessExtensions.CloseHandle(hProcess);
            base.Close();
        }

        private void OnSeekOverun(object sender, SeekOverunEventArgs e)
        {
            ProcessExtensions.MEMORY_BASIC_INFORMATION memBasicInfo;
            ulong address = 0;

            switch (e.Location)
            {
                case ProcessSeekOrigin.BeginningOfProcessMemory:
                    address = (ulong)e.Offset;
                    break;
                case ProcessSeekOrigin.Begin:
                    address = baseAddress + (ulong) e.Offset; 
                    break;
                case ProcessSeekOrigin.Current:
                    address = baseAddress + (ulong) (baseStream.Position + e.Offset); 
                    break;
                case ProcessSeekOrigin.End:
                    address = baseAddress + (ulong) (baseStream.Length + e.Offset);
                    break;
            }

            e.Position = (long) address;
            e.ContinueBaseSeek = false;

            memBasicInfo = ProcessExtensions.GetMemoryInfo(hProcess, address);

            if (memBasicInfo.Protect == ProcessExtensions.AllocationProtectEnum.Readonly | memBasicInfo.Protect == ProcessExtensions.AllocationProtectEnum.ReadWrite | memBasicInfo.Protect == ProcessExtensions.AllocationProtectEnum.ExecuteRead | memBasicInfo.Protect == ProcessExtensions.AllocationProtectEnum.ExecuteReadWrite)
            {
                var offsetFromBase = (long)(address - baseAddress);
                
                e.MemoryAllocated = true;
                e.ContinueBaseSeek = true;
                e.BaseSeekOrigin = SeekOrigin.Begin;
                e.BaseOffset = offsetFromBase;

                if (memBasicInfo.AllocationBase.As<ulong>() != baseAddress && (offsetFromBase > systemInfo.AllocationGranularity || offsetFromBase < 0))
                {
                    if (!regions.ContainsKey((ulong) memBasicInfo.AllocationBase))
                    {
                        regions.Add((ulong) memBasicInfo.AllocationBase, new ProcessBinaryReader(hProcess, (IntPtr)memBasicInfo.AllocationBase, (ulong) memBasicInfo.RegionSize));
                        processDiagnostics.RegisterRegion((ulong)memBasicInfo.BaseAddress, (ulong)memBasicInfo.AllocationBase, (int)memBasicInfo.AllocationProtect, (ulong)memBasicInfo.RegionSize, (int)memBasicInfo.State, (int)memBasicInfo.Protect, (int)memBasicInfo.Type);
                    }

                    e.OutOfRegionPosition = true;
                }
                else
                {
                    e.OutOfRegionPosition = false;
                }
            }
            else
            {
                throw new Win32Exception(unchecked((int) 0xc0000005), string.Format("Attempt to read memory marked as {0} by {1}", memBasicInfo.Protect, this.GetType().FullName));
            }
        }

        private unsafe static Stream InitializeStream(uint hProcess, IntPtr baseAddress, ulong size)
        {
            return new ProcessStream(size);
        }
    }
}
#endif
