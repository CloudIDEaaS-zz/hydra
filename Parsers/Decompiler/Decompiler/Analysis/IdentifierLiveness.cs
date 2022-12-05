#region License
/* 
 * Copyright (C) 1999-2015 John K�ll�n.
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
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Operators;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Analysis
{
	public class IdentifierLiveness : StorageVisitor<Storage>	
	{
		private Identifier id;
		private BitSet bits;
		private uint grf;
		private Dictionary<Storage,int> liveStackVars;
		private IProcessorArchitecture arch;
		private bool define;
		private int defOffset;
		private int defBitSize;
		private int useOffset;
		private int useBitSize;

		public IdentifierLiveness(IProcessorArchitecture arch)
		{
			this.arch = arch;
			liveStackVars = new Dictionary<Storage,int>();
		}

		public void Def(Identifier id)
		{
			this.id = id;
			define = true;
			id.Storage.Accept(this);
		}

        public void Def(Expression e)
        {
            var id = e as Identifier;
            if (id != null)
            {
                Def(id);
                return;
            }
            var bin = e as MemoryAccess;
            if (bin != null)
            {
                Def(bin);
                return;
            }
            throw new NotImplementedException(string.Format("Can't handle {0}.", e));
        }


        public bool Def(MemoryAccess mem)
        {
            var bin = mem.EffectiveAddress as BinaryExpression;
            if (bin == null ||(bin.Operator != Operator.IAdd && bin.Operator != Operator.ISub))
                return false;

            var idLeft = bin.Left as Identifier;
            if (idLeft == null || idLeft.Storage != arch.StackRegister)
                return false;
            var cRight = bin.Right as Constant;
            if (cRight == null || !cRight.IsValid)
                return false;
            int val = cRight.ToInt32();
            if (bin.Operator == Operator.ISub)
            {
                VisitStackLocalStorage(new StackLocalStorage(-val, bin.DataType));
            }
            else 
                VisitStackArgumentStorage(new StackArgumentStorage(val, bin.DataType));
            return true;
        }

		public int DefBitSize
		{
			get { return defBitSize; }
		}

		public int DefOffset
		{
			get { return defOffset; }
		}

		public void Use(Identifier id)
		{
			this.id = id;
			define = false;
			id.Storage.Accept(this);
		}

        public void Use(Expression ex)
        {
            var id = ex as Identifier;
            if (id != null)
            {
                Use(id);
                return;
            }
            throw new NotImplementedException("Use: " + ex);
        }

        /// <summary>
        /// Marks this identifier as being used.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bitOffset">Starting bit position in the identifier being used.</param>
        /// <param name="cbits">Number of bits being used.</param>
		public void Use(Identifier id, int bitOffset, int cbits)
		{
			define = false;
			useOffset = bitOffset;
			useBitSize = cbits;
			id.Storage.Accept(this);
		}

        public void Dump()
        {
            StringWriter sw = new StringWriter();
            Write(sw, "varLive");
            Debug.WriteLine(sw.ToString());
        }

		public BitSet BitSet
		{
			get { return bits; }
			set { bits = value; }
		}

		public virtual void DefinedRegister(RegisterStorage reg)
		{
			defOffset = reg.AliasOffset;
			defBitSize = reg.DataType.BitSize;
			var widestSub = reg.GetWidestSubregister(bits);
			if (widestSub != null)
			{
				defOffset = Math.Max(widestSub.AliasOffset, defOffset);
				defBitSize = Math.Min(widestSub.DataType.BitSize, defBitSize);
			}
			reg.SetAliases(bits, false);
		}

		public uint Grf
		{
			get { return grf; }
			set { grf = value; }
		}

		public Dictionary<Storage,int> LiveStorages
		{
			get { return liveStackVars; }
			set { liveStackVars = value; }
		}

		public Storage VisitFlagGroupStorage(FlagGroupStorage grf)
		{
			if (define)
			{
				this.grf &= ~grf.FlagGroupBits;
			}
			else
			{
				this.grf |= grf.FlagGroupBits;
			}
            return null;
		}
	

		public Storage VisitFpuStackStorage(FpuStackStorage fpu)
		{
            return null; 
		}

		public Storage VisitMemoryStorage(MemoryStorage global)
		{
            return null;
		}

		public Storage VisitOutArgumentStorage(OutArgumentStorage arg)
		{
			Def(arg.OriginalIdentifier);
            return null;
		}

		public Storage VisitRegisterStorage(RegisterStorage reg)
		{
			if (define)
			{
				DefinedRegister(reg);
			}
			else
			{
				RegisterStorage r = (useBitSize == 0)
					? reg
					: reg.GetSubregister(useOffset, useBitSize);
				if (r == null)
					r = reg;
				bits[r.Number] = true;
			}
            return null;
		}


		public Storage VisitSequenceStorage(SequenceStorage seq)
		{
			seq.Head.Storage.Accept(this);
			seq.Tail.Storage.Accept(this);
	  		if (define)
			{
				defBitSize = seq.Head.DataType.BitSize + seq.Tail.DataType.BitSize;
				defOffset = 0;
			}
			else
			{
				//$NOTimplemented: what happens in cases like es_bx = AAAABBBB
				// but only es is live out? AAAA but not BBBB should be live then.
			}
            return null;
		}

		public Storage VisitStackLocalStorage(StackLocalStorage local)
		{
			if (define)
			{
				if (liveStackVars.ContainsKey(local))
				{
					defBitSize = liveStackVars[local];
					liveStackVars.Remove(local);
				}
				defOffset = 0;
			}
			else
			{
				if (liveStackVars.ContainsKey(local))
				{
					liveStackVars[local] =
						Math.Max(useBitSize, liveStackVars[local]);
				}
				else
				{
					liveStackVars.Add(local, useBitSize);
				}
			}
            return null;
		}


		public Storage VisitStackArgumentStorage(StackArgumentStorage arg)
		{
			if (define)
			{
				if (liveStackVars.ContainsKey(arg))
				{
					defBitSize = liveStackVars[arg];
					liveStackVars.Remove(arg);
				}
				defOffset = 0;
			}
			else
			{
				if (liveStackVars.ContainsKey(arg))
				{
					liveStackVars[arg] = Math.Max(useBitSize, liveStackVars[arg]);
				}
				else
				{
					liveStackVars.Add(arg, useBitSize != 0 ? useBitSize : arg.DataType.BitSize);
				}
			}
            return null;
		}


		public Storage VisitTemporaryStorage(TemporaryStorage tmp)
		{
			if (define)
			{
				if (liveStackVars.ContainsKey(tmp))
				{
					defBitSize = liveStackVars[tmp];
					liveStackVars.Remove(tmp);
				}
				defOffset = 0;
			}
			else
			{
				if (liveStackVars.ContainsKey(tmp))
				{
					liveStackVars[tmp] = Math.Max(useBitSize, liveStackVars[tmp]);
				}
				else
					liveStackVars.Add(tmp, useBitSize != 0 ? useBitSize : id.DataType.BitSize);
			}
            return null;
		}

		public void Write(TextWriter writer, string format, params object [] args)
		{
			writer.Write(format, args);
			WriteFlagGroup(writer);
			WriteRegisters(writer);
		}

		private void WriteRegisters(TextWriter writer)
		{
			if (this.bits != null && !bits.IsEmpty)
			{
				for (int i = 0; i < bits.Count; ++i)
				{
					if (bits[i])
					{
						var reg = arch.GetRegister(i);
                        if (reg != null && reg.IsAluRegister)
						{
							writer.Write(' ');
							writer.Write(reg.ToString());
						}
					}
				}
			}
		}
			

		private void WriteFlagGroup(TextWriter writer)
		{
			if (grf != 0)
			{
				writer.Write(' ');
				writer.Write(arch.GrfToString(grf));
			}
		}
	}
}