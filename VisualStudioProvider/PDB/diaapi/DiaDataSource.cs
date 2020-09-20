using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualStudioProvider.PDB.raw;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using Utils;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace VisualStudioProvider.PDB.diaapi
{
    public unsafe class DiaDataSource
    {
        private static Guid CLSID_DiaSource = Guid.Parse("B86AE24D-BF2F-4ac9-B5A2-34B14E4CE11D");
        private static Guid IID_IDiaSource = Guid.Parse("79F1BB5F-B66E-48e5-B6A9-1545C323CA3D");
        private IDiaDataSource dataSource;
        private IDiaSession session;
        public string Version { get; private set; }
        public string GlobalName { get; private set; }
        public DiaSymbol GlobalScope { get; private set; }
        private IDiaSymbol globalScope;

        public DiaDataSource()
        {
            int hr;
            var regKey = Registry.ClassesRoot.OpenSubKey(string.Format(@"CLSID\{{{0}}}", CLSID_DiaSource.ToString())).Enumerate();
            var value = (string)regKey.Single(k => k.SubName == "InprocServer32").Default;
            var fileInfo = new FileInfo(value);
            var details = fileInfo.GetDetails();
            var description = details["File description"];
            var version = details["File version"];

            this.Version = string.Format("{0} v{1}", description, version);

            hr = NativeMethods.CoCreateInstance(ref CLSID_DiaSource, IntPtr.Zero, 1, ref IID_IDiaSource, out dataSource);

            if (hr != VSConstants.S_OK)
            {
                Marshal.ThrowExceptionForHR(hr);
            }
        }

        public void LoadPdb(string pdbFile)
        {
            int hr;
            string globalName;

            hr = dataSource.loadDataFromPdb(pdbFile);

            if (hr != VSConstants.S_OK)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            hr = dataSource.openSession(out session);

            if (hr != VSConstants.S_OK)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            hr = session.get_globalScope(out globalScope);

            if (hr != VSConstants.S_OK)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            hr = globalScope.get_name(out globalName);

            if (hr != VSConstants.S_OK)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            this.GlobalName = globalName;
            this.GlobalScope = new DiaSymbol(globalScope);
        }

        public void LoadExe(string imageFile)
        {
            int hr;
            var searchPath = Environment.ExpandEnvironmentVariables(@"%localappdata%\Temp\SymbolCache");
            string globalName;

            hr = dataSource.loadDataForExe(imageFile, searchPath, null);

            if (hr != VSConstants.S_OK)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            hr = dataSource.openSession(out session);

            if (hr != VSConstants.S_OK)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            hr = session.get_globalScope(out globalScope);

            if (hr != VSConstants.S_OK)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            hr = globalScope.get_name(out globalName);

            if (hr != VSConstants.S_OK)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            this.GlobalName = globalName;
            this.GlobalScope = new DiaSymbol(globalScope);
        }

        public DiaSymbol FindSymbolByRVA(IntPtr rva, SymTagEnum symTag)
        {
            int hr;
            IDiaSymbol symbol;

            hr = session.findSymbolByRVA((uint)rva, symTag, out symbol);

            if (hr != VSConstants.S_OK)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            return new DiaSymbol(symbol);
        }

        public unsafe EnumObjects<DiaTable> Tables
        {
            get
            {
                int hr;
                IDiaEnumTables enumTables;

                hr = session.getEnumTables(out enumTables);

                if (hr != VSConstants.S_OK)
                {
                    Marshal.ThrowExceptionForHR(hr);
                }

                return new EnumObjects<DiaTable>(() =>      // count
                    {
                        var count = 0;

                        hr = enumTables.get_Count(ref count);

                        if (hr != VSConstants.S_OK)
                        {
                            Marshal.ThrowExceptionForHR(hr);
                        }

                        return count;

                    }, (t) =>                                // next
                    {
                        uint fetched = 0;
                        IntPtr pTable;
                        IDiaTable table;

                        hr = enumTables.Next(1, out pTable, ref fetched);

                        if (fetched > 0)
                        {
                            table = (IDiaTable)Marshal.GetObjectForIUnknown(pTable);

                            t.InternalValue = table;
                            t.Value = new DiaTable(table);

                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }, (i) =>                               // item
                    {
                        IDiaTable diaTable;

                        hr = enumTables.Item(i, out diaTable);

                        if (hr != VSConstants.S_OK)
                        {
                            Marshal.ThrowExceptionForHR(hr);
                        }

                        return new DiaTable(diaTable);

                    }, () => enumTables.Reset());           // reset
            }
        }
    }
}
