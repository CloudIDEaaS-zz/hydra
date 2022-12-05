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
using Decompiler.Core.Machine;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Gui
{
    public class SignatureParser
    {
        private IProcessorArchitecture arch;
        private string str;
        private int idx;

        public SignatureParser(IProcessorArchitecture arch)
        {
            this.arch = arch;
        }

        public void Parse(string signatureString)
        {
            this.str = signatureString;
            idx = 0;
            try
            {
                var ret = ParseReturn();
                var procName = ParseProcedureName();
                if (procName == null)
                    return;
                var procArgs = ParseProcedureArgs();
                if (procArgs == null)
                    return;

                ProcedureName = procName;
                Signature = new SerializedSignature();
                Signature.ReturnValue = ret;
                Signature.Arguments = procArgs;
                IsValid = true;
            }
            catch
            {
                IsValid = false;
            }
        }

        public static string UnparseSignature(SerializedSignature sig, string procName)
        {
            StringBuilder sb = new StringBuilder();
            if (sig.ReturnValue == null)
                sb.Append("void");
            else
                sb.Append(sig.ReturnValue.Name);
            sb.Append(" ");
            sb.Append(procName);
            sb.Append("(");
            string sep = "";
            foreach (var arg in sig.Arguments)
            {
                sb.Append(sep);
                sep = ", ";
                sb.Append(arg.Type);
                sb.Append(" ");
                sb.Append(arg.Name);
            }
            sb.Append(")");
            return sb.ToString();
        }


        public string ProcedureName { get; private set;}

        public SerializedSignature Signature { get; private set; }

        public bool IsValid { get; private set; }

        private Argument_v1 ParseReturn()
        {
            string w = GetNextWord();
            if (w == "void")
                return null;
            Console.WriteLine(w);
            RegisterStorage reg;
            if (arch.TryGetRegister(w, out reg))
            {
                return new Argument_v1(
                    reg.Name,
                    null,
                    new Register_v1(reg.Name),
                    true);
            }
            if (w.Contains("_"))
            {
                return ParseRegisterSequenceWithUnderscore(w);
            }
            throw new NotImplementedException();
        }

        private Argument_v1 ParseRegisterSequenceWithUnderscore(string w)
        {
            string[] subregs = w.Split('_');
            var regs = new List<Register_v1>();
            foreach (string subReg in subregs)
            {
                RegisterStorage r;
                if (!arch.TryGetRegister(subReg, out r))
                    return null;
                regs.Add(new Register_v1(r.Name));
            }
            var seq = new SerializedSequence();
            seq.Registers = regs.ToArray();
            return new Argument_v1(
                w,
                null,
                seq,
                true);
        }

        private string ParseProcedureName()
        {
            return GetNextWord();
        }

        private Argument_v1[] ParseProcedureArgs()
        {
            EatWhiteSpace();
            if (idx >= str.Length || str[idx] != '(')
                return null;
            ++idx;
            var args = new List<Argument_v1>();
            for (; ; )
            {
                EatWhiteSpace();
                if (idx >= str.Length)
                    return null;
                if (str[idx] == ')')
                    return args.ToArray();
                if (args.Count > 0)
                {
                    if (str[idx] != ',')
                        return null;
                    ++idx;
                }
                var arg = ParseArg();
                if (arg == null)
                    return null;
                args.Add(arg);
            }
        }

        private Argument_v1 ParseArg()
        {
            var w = GetNextWord();
            if (w == null)
                return null;

            RegisterStorage reg;
            string type = null;
            if (!arch.TryGetRegister(w, out reg))
            {
                type = w;
                w = GetNextWord();
                if (w == null)
                    return null;
                if (w.Contains("_"))
                {
                    var retval = ParseRegisterSequenceWithUnderscore(w);
                    if (retval != null)
                        return retval;
                }
                if (!arch.TryGetRegister(w, out reg))
                {
                    return ParseStackArgument(type, w);
                }
            }

            if (PeekChar(':') || PeekChar('_'))
            {
                return ParseRegisterSequence(reg, type);
            }

            var arg = new Argument_v1()
            {
                Name = reg.Name,
                Kind = new Register_v1(reg.Name),
                OutParameter = false,
                Type = new SerializedTypeReference { TypeName = type },
            };
            return arg;
        }

        private Argument_v1 ParseRegisterSequence(RegisterStorage reg, string type)
        {
            ++idx;
            string w2 = GetNextWord();
            if (w2 == null)
                return null;
            RegisterStorage reg2;
            if (!arch.TryGetRegister(w2, out reg2))
                return null;
            var seqArgName = reg.Name + "_" + reg2.Name;
            var seqArgType = new SerializedTypeReference { TypeName = type };
            var seqKind = new SerializedSequence();
            seqKind.Registers = new Register_v1[]{ 
                    new Register_v1(reg.Name), 
                    new Register_v1(reg2.Name)
                };
            return new Argument_v1(seqArgName, seqArgType, seqKind, false);
        }

        private bool PeekChar(char cha)
        {
            return idx < str.Length && str[idx] == cha;
        }

		private Argument_v1 ParseStackArgument(string typeName, string argName)
		{
			PrimitiveType p;
			int sizeInWords;
			int wordSize = arch.WordWidth.Size;
			if (PrimitiveType.TryParse(typeName, out p))
			{
				sizeInWords = (p.Size + (wordSize - 1))/wordSize;
			}
			else
			{
				sizeInWords = 1;      // A reasonable guess, but is it a good one?
			}

			Argument_v1 arg = new Argument_v1();
			arg.Name = argName;
			arg.Type = new SerializedTypeReference { TypeName = typeName };
			arg.Kind = new StackVariable_v1(); //  (sizeInWords * wordSize);
			return arg;
		}

        private string GetNextWord()
        {
            EatWhiteSpace();
            if (idx >= str.Length)
                return null;

            int i = idx;
            char c = str[i];
            while (i < str.Length && (Char.IsLetterOrDigit(c) || c == '_'))
            {
                ++i;
                c = str[i];
            }
            string s = str.Substring(idx, i - idx);
            idx = i;
            return s;
        }

        private void EatWhiteSpace()
        {
            while (idx < str.Length && Char.IsWhiteSpace(str[idx]))
                ++idx;
        }
    }
}
