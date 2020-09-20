//---------------------------------------------------------------------
//  This file is part of the CLR Managed Debugger (mdbg) Sample.
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Samples.Debugging.CorSymbolStore;
using System.Diagnostics;
using Microsoft.Samples.Debugging.Native;
using VisualStudioProvider.PDB.diaapi;

namespace Pdb
{
    public static class PdbReader
    {
        public static DiaDataSource GetPdbDataSource(string nonClrImage, bool searchForPdb = true)
        {
            var fileInfo = new FileInfo(Path.Combine(Path.GetDirectoryName(nonClrImage), Path.GetFileNameWithoutExtension(nonClrImage) + ".pdb"));
            var diaDataSource = new DiaDataSource();

            if (!fileInfo.Exists)
            {
                Debugger.Break();
            }

            diaDataSource.LoadExe(nonClrImage);

            return diaDataSource;
        }

        public static SymbolData GetPdbData(string asmPath, SymbolFormat symFormat, bool expandAttrs, bool skipMethods)
        {
            // Read the PDB into a SymbolData object
            SymbolDataReader reader = new SymbolDataReader(asmPath, symFormat, expandAttrs, skipMethods);
            SymbolData symData = reader.ReadSymbols();

            return symData;
        }

        public static void PdbToXML(string asmPath, string outputXml, SymbolFormat symFormat, bool expandAttrs, bool skipMethods)
        {
            // Read the PDB into a SymbolData object
            SymbolDataReader reader = new SymbolDataReader(asmPath, symFormat, expandAttrs, skipMethods);
            SymbolData symData = reader.ReadSymbols();

            if (symData != null)
            {
                PdbDataToXML(outputXml, symData);
            }
        }

        public static void PdbDataToXML(string outputXml, SymbolData symData)
        {
            // Use XML serialization to write out the symbol data
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.Indent = true;
            XmlSerializer ser = new XmlSerializer(typeof(SymbolData));

            using (XmlWriter writer = XmlWriter.Create(outputXml, settings))
            {
                ser.Serialize(writer, symData);
            }
        }

        public static SymbolData XMLToPdbData(string inputXml)
        {
            Console.WriteLine("Reading XML file: {0}", inputXml);

            // Use XML serialization to read in the symbol data
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Document;
            XmlSerializer ser = new XmlSerializer(typeof(SymbolData));
            SymbolData symData;

            using (XmlReader reader = XmlReader.Create(inputXml, settings))
            {
                symData = (SymbolData)ser.Deserialize(reader);
            }

            return symData;
        }
    }
} 
