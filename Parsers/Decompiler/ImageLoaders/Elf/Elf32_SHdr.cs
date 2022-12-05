﻿#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    public class Elf32_SHdr
    {
        public uint sh_name;
        public SectionHeaderType sh_type;
        public uint sh_flags;
        public uint sh_addr;        // Address
        public uint sh_offset;
        public uint sh_size;
        public uint sh_link;
        public uint sh_info;
        public uint sh_addralign;
        public uint sh_entsize;

        public static Elf32_SHdr Load(ImageReader rdr)
        {
            return new Elf32_SHdr
            {
                sh_name = rdr.ReadUInt32(),
                sh_type = (SectionHeaderType)rdr.ReadUInt32(),
                sh_flags = rdr.ReadUInt32(),
                sh_addr = rdr.ReadUInt32(),        // Address
                sh_offset = rdr.ReadUInt32(),
                sh_size = rdr.ReadUInt32(),
                sh_link = rdr.ReadUInt32(),
                sh_info = rdr.ReadUInt32(),
                sh_addralign = rdr.ReadUInt32(),
                sh_entsize = rdr.ReadUInt32(),
            };
        }

        public bool ContainsAddress(uint uAddress)
        {
            return sh_addr <= uAddress && uAddress < sh_addr + sh_size;
        }
    }

    public class Elf64_SHdr
    {
        public uint sh_name;
        public SectionHeaderType sh_type;
        public ulong sh_flags;
        public ulong sh_addr;        // Address
        public ulong sh_offset;
        public ulong sh_size;
        public uint sh_link;
        public uint sh_info;
        public ulong sh_addralign;
        public ulong sh_entsize;

        public static Elf64_SHdr Load(ImageReader rdr)
        {
            return new Elf64_SHdr
            {
                sh_name = rdr.ReadUInt32(),
                sh_type = (SectionHeaderType)rdr.ReadUInt32(),
                sh_flags = rdr.ReadUInt64(),
                sh_addr = rdr.ReadUInt64(),        // Address
                sh_offset = rdr.ReadUInt64(),
                sh_size = rdr.ReadUInt64(),
                sh_link = rdr.ReadUInt32(),
                sh_info = rdr.ReadUInt32(),
                sh_addralign = rdr.ReadUInt64(),
                sh_entsize = rdr.ReadUInt64(),
            };
        }
    }
}