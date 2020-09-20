using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Utils.PortableExecutable
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public class DataDirectory
    {
        public ulong Address { get; set; }
        public ulong Size { get; set; }

        public static byte[] ReadVirtualDirectory(BinaryReader reader, DataDirectory dataDirectory, IList<Section> sections)
        {
            var fileOffset = AddressingUtils.RelativeVirtualAddressToFileOffset(dataDirectory.Address, sections);

            reader.Seek(fileOffset);

            return reader.ReadBytes((int)dataDirectory.Size);
        }

        public string DebugInfo
        {
            get
            {
                return string.Format("Address: 0x{0:x8}, SizeHex: 0x{1:x8}, Size: {1}", this.Address, this.Size);
            }
        }
    }
}
