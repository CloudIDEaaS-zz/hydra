using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Utils.PortableExecutable
{
    public class DOSHeader
    {
        public ushort MagicBytes { get; set; }
        public ushort BytesOnLastPage { get; set; }
        public ushort PagesInFile { get; set; }
        public ushort Relocations { get; set; }
        public ushort SizeOfHeader { get; set; }
        public ushort MinExtraParagraphs { get; set; }
        public ushort MaxExtraParagraphs { get; set; }
        public ushort InitialSS { get; set; }
        public ushort InitialSP { get; set; }
        public ushort Checksum { get; set; }
        public ushort InitialIP { get; set; }
        public ushort InitialCS { get; set; }
        public ushort RelocTableAddress { get; set; }
        public ushort OverlayNumber { get; set; }
        public ushort Unknown01 { get; set; }
        public ushort Unknown02 { get; set; }
        public ushort Unknown03 { get; set; }
        public ushort Unknown04 { get; set; }
        public ushort OEMIdentifier { get; set; }
        public ushort OEMInfo { get; set; }
        public ushort Unknown05 { get; set; }
        public ushort Unknown06 { get; set; }
        public ushort Unknown07 { get; set; }
        public ushort Unknown08 { get; set; }
        public ushort Unknown09 { get; set; }
        public ushort Unknown10 { get; set; }
        public ushort Unknown11 { get; set; }
        public ushort Unknown12 { get; set; }
        public ushort Unknown13 { get; set; }
        public ushort Unknown14 { get; set; }
        public ushort COFFHeaderAddress { get; set; }

        public static DOSHeader ReadDOSHeader(BinaryReader reader)
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            return new DOSHeader
            {
                MagicBytes = reader.ReadUInt16(),
                BytesOnLastPage = reader.ReadUInt16(),
                PagesInFile = reader.ReadUInt16(),
                Relocations = reader.ReadUInt16(),
                SizeOfHeader = reader.ReadUInt16(),
                MinExtraParagraphs = reader.ReadUInt16(),
                MaxExtraParagraphs = reader.ReadUInt16(),
                InitialSS = reader.ReadUInt16(),
                InitialSP = reader.ReadUInt16(),
                Checksum = reader.ReadUInt16(),
                InitialIP = reader.ReadUInt16(),
                InitialCS = reader.ReadUInt16(),
                RelocTableAddress = reader.ReadUInt16(),
                OverlayNumber = reader.ReadUInt16(),
                Unknown01 = reader.ReadUInt16(),
                Unknown02 = reader.ReadUInt16(),
                Unknown03 = reader.ReadUInt16(),
                Unknown04 = reader.ReadUInt16(),
                OEMIdentifier = reader.ReadUInt16(),
                OEMInfo = reader.ReadUInt16(),
                Unknown05 = reader.ReadUInt16(),
                Unknown06 = reader.ReadUInt16(),
                Unknown07 = reader.ReadUInt16(),
                Unknown08 = reader.ReadUInt16(),
                Unknown09 = reader.ReadUInt16(),
                Unknown10 = reader.ReadUInt16(),
                Unknown11 = reader.ReadUInt16(),
                Unknown12 = reader.ReadUInt16(),
                Unknown13 = reader.ReadUInt16(),
                Unknown14 = reader.ReadUInt16(),
                COFFHeaderAddress = reader.ReadUInt16()            
            };
        }

        public string Magic
        {
            get
            {
                var bytes = BitConverter.GetBytes(this.MagicBytes);
                var magic = ASCIIEncoding.ASCII.GetString(bytes, 0, 2);

                return magic;
            }
        }

        public static int Size
        {
            get
            {
                return TypeExtensions.PrimitiveFieldSizeOf<DOSHeader>();
            }
        }
    }
}
