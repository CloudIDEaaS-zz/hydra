using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Utils.PortableExecutable
{
    public class PEHeader
    {
        public uint SignatureBytes { get; set; }
        public ushort Machine { get; set; }
        public ushort NumberOfSections { get; set; }
        public uint DateTimeStamp { get; set; }
        public uint PtrToSymbolTable { get; set; }
        public uint NumberOfSymbols { get; set; }
        public ushort SizeOfOptionalHeaders { get; set; }
        public ushort Characteristics { get; set; }
        public ushort OptionalMagic { get; set; }
        public byte MajorLinkerVersion { get; set; }
        public byte MinorLinkerVersion { get; set; }
        public uint SizeOfCode { get; set; }
        public uint SizeOfInitData { get; set; }
        public uint SizeOfUninitData { get; set; }
        public uint AddressOfEntryPoint { get; set; }
        public uint BaseOfCode { get; set; }
        public uint BaseOfData { get; set; }
        public uint ImageBase { get; set; }
        public uint SectionAlignment { get; set; }
        public uint FileAlignment { get; set; }
        public ushort MajorOSVersion { get; set; }
        public ushort MinorOSVersion { get; set; }
        public ushort MajorImageVersion { get; set; }
        public ushort MinorImageVersion { get; set; }
        public ushort MajorSubsystemVersion { get; set; }
        public ushort MinorSubsystemVersion { get; set; }
        public uint Reserved1 { get; set; }
        public uint SizeOfImage { get; set; }
        public uint SizeOfHeaders { get; set; }
        public uint PEChecksum { get; set; }
        public ushort Subsystem { get; set; }
        public ushort DLLCharacteristics { get; set; }
        public uint SizeOfStackReserve { get; set; }
        public uint SizeOfStackCommit { get; set; }
        public uint SizeOfHeapReserve { get; set; }
        public uint SizeOfHeapCommit { get; set; }
        public uint LoaderFlags { get; set; }
        public uint DirectoryLength { get; set; }
        public List<DataDirectory> Directories { get; set; }
        public List<Section> Sections { get; set; }
        private static BinaryReader reader;

        public static PEHeader ReadPEHeader(BinaryReader reader, ushort headerAddress)
        {
            PEHeader.reader = reader;

            reader.BaseStream.Seek(headerAddress, SeekOrigin.Begin);

            var header = new PEHeader
            {
                SignatureBytes = reader.ReadUInt32(),
                Machine = reader.ReadUInt16(),
                NumberOfSections = reader.ReadUInt16(),
                DateTimeStamp = reader.ReadUInt32(),
                PtrToSymbolTable = reader.ReadUInt32(),
                NumberOfSymbols = reader.ReadUInt32(),
                SizeOfOptionalHeaders = reader.ReadUInt16(),
                Characteristics = reader.ReadUInt16(),
                OptionalMagic = reader.ReadUInt16(),
                MajorLinkerVersion = reader.ReadByte(),
                MinorLinkerVersion = reader.ReadByte(),
                SizeOfCode = reader.ReadUInt32(),
                SizeOfInitData = reader.ReadUInt32(),
                SizeOfUninitData = reader.ReadUInt32(),
                AddressOfEntryPoint = reader.ReadUInt32(),
                BaseOfCode = reader.ReadUInt32(),
                BaseOfData = reader.ReadUInt32(),
                ImageBase = reader.ReadUInt32(),
                SectionAlignment = reader.ReadUInt32(),
                FileAlignment = reader.ReadUInt32(),
                MajorOSVersion = reader.ReadUInt16(),
                MinorOSVersion = reader.ReadUInt16(),
                MajorImageVersion = reader.ReadUInt16(),
                MinorImageVersion = reader.ReadUInt16(),
                MajorSubsystemVersion = reader.ReadUInt16(),
                MinorSubsystemVersion = reader.ReadUInt16(),
                Reserved1 = reader.ReadUInt32(),
                SizeOfImage = reader.ReadUInt32(),
                SizeOfHeaders = reader.ReadUInt32(),
                PEChecksum = reader.ReadUInt32(),
                Subsystem = reader.ReadUInt16(),
                DLLCharacteristics = reader.ReadUInt16(),
                SizeOfStackReserve = reader.ReadUInt32(),
                SizeOfStackCommit = reader.ReadUInt32(),
                SizeOfHeapReserve = reader.ReadUInt32(),
                SizeOfHeapCommit = reader.ReadUInt32(),
                LoaderFlags = reader.ReadUInt32(),
                DirectoryLength = reader.ReadUInt32()
            };

            return header;
        }

        public string Signature
        {
            get
            {
                var bytes = BitConverter.GetBytes(this.SignatureBytes);
                var signature = ASCIIEncoding.ASCII.GetString(bytes, 0, 2);

                return signature;
            }
        }

        public static int Size
        {
            get
            {
                return TypeExtensions.PrimitiveFieldSizeOf<PEHeader>();
            }
        }
    }
}
