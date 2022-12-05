﻿using Decompiler.Core;
using Decompiler.ImageLoaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Decompiler.UnitTests
{
    class DchexLoader : ImageLoader
    {
        private Address addrStart;
        private MemoryStream memStm;
        private Program results;

        public DchexLoader(string filename, IServiceProvider services, byte[] imgRaw) :
            base(services, filename, imgRaw)
        {
            using (TextReader rdr = new StreamReader(filename))
            {
                LoadFromFile(rdr);
            }
        }

        public override Address PreferredBaseAddress
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override Program Load(Address addrLoad)
        {
            return results;
        }

        private void LoadFromFile(TextReader rdr)
        {
            var arch = GetArchitecture(ReadLine(rdr).Trim());
            for (; ; )
            {
                var line = ReadLine(rdr);
                if (line == null)
                    break;
                ProcessLine(line);
            }
            var img = new LoadedImage(addrStart, memStm.ToArray());
            results = new Program(
                img,
                img.CreateImageMap(),
                arch,
                new DefaultPlatform(Services, arch));

        }

        private IProcessorArchitecture GetArchitecture(string archName)
        {
            switch (archName)
            {
            case "m68k": return new Decompiler.Arch.M68k.M68kArchitecture();
            default: throw new NotImplementedException();
            }
        }

        private void ProcessLine(string line)
        {
            // Get rid of comments.
            var segs = line.Split(';'); 

            // Tokenize
            var tokens = Regex.Replace(line.TrimEnd(), " +", " ").Split(' ');
            // If line didn't start with a space, the first token is address.
            int i = 0;
            if (tokens.Length > 1 && line[0] != ' ')
            {
                Address address;
                Address.TryParse32(tokens[0], out address);
                if (this.addrStart == null)
                {
                    addrStart = address;
                    memStm = new MemoryStream();
                }
                else
                {
                    memStm.Position = address - addrStart;
                }
                i = 1;
            }
            for (; i < tokens.Length; ++i)
            {
                memStm.WriteByte(ToByte(tokens[i]));
            }
        }

        private byte ToByte(string p)
        {
            return (byte) ((HexDigit(p[0]) << 4) | HexDigit(p[1]));
        }

        private int HexDigit(int ch)
        {
            if ('0' <= ch && ch <= '9')
                return (ch - '0');
            else if ('A' <= ch && ch <= 'F')
                return (10 + ch - 'A');
            else if ('a' <= ch && ch <= 'f')
                return (10 + ch - 'a');

            throw new BadImageFormatException();
        }

        private string ReadLine(TextReader rdr)
        {
            for (; ; )
            {
                var line = rdr.ReadLine();
                if (line == null)
                    return null;
                var i = line.IndexOf(';');
                if (i != 0)
                    return line;
            }
        }

        public override RelocationResults Relocate(Address addrLoad)
        {
            return new RelocationResults(new List<EntryPoint>(), new RelocationDictionary());
        }
    }
}
