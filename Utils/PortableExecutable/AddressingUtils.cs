using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.PortableExecutable
{
    public static class AddressingUtils
    {
        public static ulong RelativeVirtualAddressToFileOffset(ulong rva, IEnumerable<Section> sections)
        {
            var section = sections.First(s => s.VirtualAddress <= rva && s.VirtualAddress + s.SizeOfRawData >= rva);
            var fileOffset = section.PointerToRawData + (rva - section.VirtualAddress);

            return fileOffset;
        }
    }
}
