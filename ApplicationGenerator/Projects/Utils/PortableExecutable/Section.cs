using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Utils.PortableExecutable
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public class Section
    {
        public string Name { get; set; }
        public ulong VirtualSize { get; set; }
        public ulong VirtualAddress { get; set; }
        public ulong SizeOfRawData { get; set; }
        public ulong PointerToRawData { get; set; }
        public ulong PointerToRelocations { get; set; }
        public ulong PointerToLinenumbers { get; set; }
        public ushort NumberOfRelocations { get; set; }
        public ushort NumberOfLinenumbers { get; set; }
        public ulong Characteristics { get; set; }

        public string DebugInfo
        {
            get
            {
                return string.Format("Name: {0}, VirtualAddress: 0x{1:x8}, VirtualSizeHex: 0x{2:x8}, VirtualSize: {2}, RawSizeHex: 0x{3:x8}, RawSize: {3}", this.Name, this.VirtualAddress, this.VirtualSize, this.SizeOfRawData);
            }
        }
    }
}
