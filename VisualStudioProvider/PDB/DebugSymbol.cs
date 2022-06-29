using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.PortableExecutable;
using VisualStudioProvider.PDB;
using VisualStudioProvider.PDB.diaapi;
using VisualStudioProvider.PDB.raw;

namespace Pdb
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public class DebugSymbol
    {
        public virtual uint IndexId { get; set; }
        public virtual string Name { get; set; }
        public virtual VisualStudioProvider.PDB.raw.DataKind DataKind { get; set; }
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public virtual uint AddressOffset { get; set; }
        public virtual uint AddressSection { get; set; }
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public virtual int Offset { get; set; }
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public virtual ulong Length { get; set; }
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public virtual System.IntPtr RVA { get; set; }
        public virtual VisualStudioProvider.PDB.raw.SymTagEnum SymTag { get; set; }
        public virtual VisualStudioProvider.PDB.raw.LocationType LocationType { get; set; }
        public virtual VisualStudioProvider.PDB.raw.BasicType BasicType { get; set; }
        public virtual VisualStudioProvider.PDB.raw.Language Language { get; set; }
        public virtual System.Guid Guid { get; set; }
        public virtual string LibraryName { get; set; }
        public virtual string SymbolsFileName { get; set; }
        public virtual string SourceFileName { get; set; }
        public virtual string UndecoratedName { get; set; }
        public List<DiaSymbol> Children { get; private set; }
        [Browsable(false)]
        public ActionQueueService ActionQueueService { get; private set; }
        private IntPtr readBinaryAddress;

        [TypeConverter(typeof(HexNumberTypeConverter))]
        public IntPtr ReadBinaryAddress
        {
            get
            {
                return readBinaryAddress;
            }

            private set
            {
                readBinaryAddress = value;
            }
        }

        public static DebugSymbol FromImportByName(ImportByName importByName, PEHeader peHeader, IntPtr baseAddress, ActionQueueService actionQueueService)
        {
            var debugSymbol = new DebugSymbol();
            int readBinaryAddress;

            debugSymbol.Name = importByName.Name;
            debugSymbol.RVA = (IntPtr) importByName.Offset;

            if (debugSymbol.RVA != IntPtr.Zero)
            {
                readBinaryAddress = baseAddress.ToInt32() + debugSymbol.RVA.ToInt32();
                debugSymbol.ReadBinaryAddress = (IntPtr)readBinaryAddress;
            }

            debugSymbol.ActionQueueService = actionQueueService;

            return debugSymbol;
        }

        public static DebugSymbol FromExport(ExportData exportData, PEHeader peHeader, IntPtr baseAddress, ActionQueueService actionQueueService)
        {
            var debugSymbol = new DebugSymbol();
            int readBinaryAddress;

            debugSymbol.Name = exportData.Name;
            debugSymbol.RVA = (IntPtr)exportData.AddressOfFunction;

            if (debugSymbol.RVA != IntPtr.Zero)
            {
                readBinaryAddress = baseAddress.ToInt32() + debugSymbol.RVA.ToInt32();
                debugSymbol.ReadBinaryAddress = (IntPtr)readBinaryAddress;
            }

            debugSymbol.ActionQueueService = actionQueueService;

            return debugSymbol;
        }

        public static DebugSymbol FromThunkData(ThunkData thunkData, PEHeader peHeader, IntPtr baseAddress, ActionQueueService actionQueueService)
        {
            var debugSymbol = new DebugSymbol();
            int readBinaryAddress;

            debugSymbol.Name = thunkData.ImportByName.Name;
            debugSymbol.RVA = (IntPtr)thunkData.Offset;

            if (debugSymbol.RVA != IntPtr.Zero)
            {
                readBinaryAddress = baseAddress.ToInt32() + debugSymbol.RVA.ToInt32();
                debugSymbol.ReadBinaryAddress = (IntPtr)readBinaryAddress;
            }

            debugSymbol.ActionQueueService = actionQueueService;

            return debugSymbol;
        }

        public static DebugSymbol FromDiaSymbol(DiaSymbol diaSymbol, PEHeader peHeader, IntPtr baseAddress, ActionQueueService actionQueueService)
        {
            var debugSymbol = new DebugSymbol();
            int readBinaryAddress;

            if (diaSymbol.IndexId == 76302)
            {

            }

            try
            {
                debugSymbol.IndexId = diaSymbol.IndexId;
            }
            catch
            {
            }

            try
            {
                debugSymbol.Name = diaSymbol.Name;
            }
            catch
            {
            }

            try
            {
                debugSymbol.DataKind = diaSymbol.DataKind;
            }
            catch
            {
            }

            try
            {
                debugSymbol.AddressOffset = diaSymbol.AddressOffset;
            }
            catch
            {
            }

            try
            {
                debugSymbol.AddressSection = diaSymbol.AddressSection;
            }
            catch
            {
            }

            try
            {
                debugSymbol.Offset = diaSymbol.Offset;
            }
            catch
            {
            }

            try
            {
                debugSymbol.Length = diaSymbol.Length;
            }
            catch
            {
            }

            try
            {
                debugSymbol.RVA = diaSymbol.RVA;
            }
            catch
            {
            }

            try
            {
                debugSymbol.SymTag = diaSymbol.SymTag;
            }
            catch
            {
            }

            try
            {
                debugSymbol.LocationType = diaSymbol.LocationType;
            }
            catch
            {
            }

            try
            {
                debugSymbol.BasicType = diaSymbol.BasicType;
            }
            catch
            {
            }

            try
            {
                debugSymbol.Language = diaSymbol.Language;
            }
            catch
            {
            }

            try
            {
                debugSymbol.Guid = diaSymbol.Guid;
            }
            catch
            {
            }

            try
            {
                debugSymbol.LibraryName = diaSymbol.LibraryName;
            }
            catch
            {
            }

            try
            {
                debugSymbol.SymbolsFileName = diaSymbol.SymbolsFileName;
            }
            catch
            {
            }

            try
            {
                debugSymbol.SourceFileName = diaSymbol.SourceFileName;
            }
            catch
            {
            }

            try
            {
                debugSymbol.UndecoratedName = diaSymbol.UndecoratedName;
            }
            catch
            {
            }

            try
            {
                debugSymbol.Children = diaSymbol.Children.ToList();
            }
            catch
            {
            }

            if (debugSymbol.RVA != IntPtr.Zero)
            {
                readBinaryAddress = baseAddress.ToInt32() + debugSymbol.RVA.ToInt32();
                debugSymbol.ReadBinaryAddress = (IntPtr)readBinaryAddress;
            }

            debugSymbol.ActionQueueService = actionQueueService;

            return debugSymbol;
        }

        public string DebugInfo
        {
            get
            {
                if (this.SymTag == SymTagEnum.SymTagCompiland)
                {
                    return string.Format("{0} [{1}]", Path.GetFileName(this.LibraryName), Path.GetFileName(this.Name));
                }
                else
                {
                    return string.Format("{0}, RVA=[{1:X8}]", this.UndecoratedName.AsDisplayText(), (ulong)this.RVA);
                }
            }
        }
    }
}
