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
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.IO;

namespace Decompiler.Core.Expressions
{
	/// <summary>
	/// Represents an access to a named "register" or "variable".
	/// </summary>
	public class Identifier : Expression
	{
		public Identifier(string name, DataType type, Storage stg) : base(type)
		{
            if (name == "tLoc24")   //$DEBUG
                name.ToCharArray();
			this.Name = name;
			this.Storage = stg;
		}

        public string Name { get; private set; }
        public Storage Storage { get; private set; }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitIdentifier(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitIdentifier(this);
        }

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitIdentifier(this);
		}

		public override Expression CloneExpression()
		{
			return this;
		}

		public override Expression Invert()
		{
			return new UnaryExpression(Operator.Not, PrimitiveType.Bool, this);
		}

		public void Write(bool writeStorage, TextWriter writer)
		{
			WriteType(writeStorage, writer);
			writer.Write(' ');
			writer.Write(Name);
		}

		public void WriteType(bool writeStorage, TextWriter writer)
		{
			if (writeStorage)
			{
				OutArgumentStorage os = Storage as OutArgumentStorage;
				if (os != null)
				{
					writer.Write(os.OriginalIdentifier.Storage.Kind);
					writer.Write(" out ");
				}	
				else
				{
					writer.Write(Storage.Kind);
					writer.Write(' ');
				}
			}
			writer.Write(DataType);
		}
	}

	/// <summary>
	/// A special class that represents memory locations. Initially,
	/// all memory accesses can be considered to be made from one global
	/// variable, MEM0. Later, SSA analysis will break apart MEM references 
	/// after each store operation, giving rise to MEM1, MEM2 &c. 
	/// If ambitious, memory alias analysis can be done. In this case,
	/// we will have several MEMx variables before SSA, each MEMx variable
	/// will be an alias class. 
	/// </summary>
	public class MemoryIdentifier : Identifier
	{
		private static MemoryIdentifier g;

		public MemoryIdentifier(int i, DataType dt) : base("Mem" + i, dt, new MemoryStorage())
		{
		}

        public MemoryIdentifier(string name, DataType dt) : base(name, dt, new MemoryStorage())
        {

        }
		static MemoryIdentifier()
		{
			g = new MemoryIdentifier(0, new UnknownType());
		}

		public static MemoryIdentifier GlobalMemory
		{
			get { return g; }
		}
	}
}
