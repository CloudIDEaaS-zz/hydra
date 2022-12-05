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

using Decompiler.Core.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Core.Types
{
    public class TypeGraphWriter : IDataTypeVisitor<Formatter>
    {
        private HashSet<DataType> visited;
        private Formatter writer;
        private bool reference;

        public TypeGraphWriter(Formatter writer)
        {
            this.writer = writer;
        }
        
        public Formatter VisitArray(ArrayType at)
        {
            writer.Write("(arr ");
            at.ElementType.Accept(this);
            if (at.Length != 0)
            {
                writer.Write(" {0}", at.Length);
            }
            writer.Write(")");
            return writer;
        }

        public Formatter VisitCode(CodeType c)
        {
            writer.Write("code", c.Size);
            return writer;
        }

        public Formatter VisitEnum(EnumType e)
        {
            throw new NotImplementedException();
        }

        public Formatter VisitEquivalenceClass(EquivalenceClass eq)
        {
            writer.Write(eq.Name);
            return writer;
        }

        public Formatter VisitFunctionType(FunctionType ft)
        {
            writer.Write("(fn ");
            if (ft.ReturnType != null)
                ft.ReturnType.Accept(this);
            else
                writer.Write("void");
            writer.Write(" (");

            string separator = "";
            for (int i = 0; i < ft.ArgumentTypes.Length; ++i)
            {
                writer.Write(separator);
                separator = ", ";
                ft.ArgumentTypes[i].Accept(this);
            }
            writer.Write("))");
            return writer;
        }

        public Formatter VisitPrimitive(PrimitiveType pt)
        {
            writer.Write(pt.Name);
            return writer;
        }

        public Formatter VisitMemberPointer(MemberPointer memptr)
        {
            writer.Write("(memptr ");
            memptr.BasePointer.Accept(this);
            writer.Write(" ");
            memptr.Pointee.Accept(this);
            writer.Write(")");
            return writer;
        }

        public Formatter VisitPointer(Pointer ptr)
        {
			writer.Write("(ptr ");
			WriteReference(ptr.Pointee);
			writer.Write(")");
            return writer;
		}

        public Formatter VisitString(StringType str)
        {
            writer.Write("(str");
            if (str.LengthPrefixType != null)
            {
                writer.Write(" length-");
                str.LengthPrefixType.Accept(this);
                if (str.PrefixOffset != 0)
                    writer.Write(" {0}", str.PrefixOffset);
            }
            writer.Write(" ");
            str.ElementType.Accept(this);
            writer.Write(")");
            return writer;
        }

        public Formatter VisitStructure(StructureType str)
        {
            if (this.visited == null)
                visited = new HashSet<DataType>();

            writer.Write("({0}", str.IsSegment ? "segment" : "struct");
            if (str.Name != null)
            {
                writer.Write(" \"{0}\"", str.Name);
            }
            if (str.Size != 0)
            {
                writer.Write(" {0:X4}", str.Size);
            }

            if (!visited.Contains(str) && (!reference || str.Name == null))
            {
                visited.Add(str);
                foreach (StructureField f in str.Fields)
                {
                    writer.Write(" ({0:X} ", f.Offset);
                    f.DataType.Accept(this);
                    writer.Write(" {0})", f.Name);
                }
            }
            writer.Write(")");
            return writer;
        }

        public Formatter VisitTypeReference(TypeReference typeref)
        {
            if (string.IsNullOrEmpty(typeref.Name))
            {
                typeref.Referent.Accept(this);
            }
            else
            {
                writer.Write(typeref.Name);
            }
            return writer;
        }

        public Formatter VisitTypeVariable(TypeVariable tv)
        {
            writer.Write(tv.Name);
            return writer;
        }

        public Formatter VisitUnion(UnionType ut)
        {
            if (visited == null)
                visited = new HashSet<DataType>(); 

            writer.Write("(union");
            if (ut.Name != null)
            {
                writer.Write(" \"{0}\"", ut.Name);
            }
            int i = 0;
            if (!visited.Contains(ut) && (!reference || ut.Name == null))
            {
                visited.Add(ut);
                foreach (UnionAlternative alt in ut.Alternatives.Values)
                {
                    writer.Write(" (");
                    alt.DataType.Accept(this);
                    writer.Write(" {0})", alt.MakeName(i));
                    ++i;
                }
            }
            writer.Write(")");
            return writer;
        }

        public Formatter VisitUnknownType(UnknownType ut)
        {
            writer.Write("<unknown>");
            return writer;
        }

        public Formatter VisitVoidType(VoidType vt)
        {
            writer.Write("void");
            return writer;
        }

        internal void WriteReference(DataType dataType)
        {
            var old = reference;
            reference = true;
            try
            {
                dataType.Accept(this);
            }
            finally
            {
                reference = old;
            }
        }
    }
}
