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
using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Decompiler.Core
{
	/// <summary>
	/// Describes the call structure of the program: what nodes call what others.
	/// </summary>
	public class CallGraph
	{
		private List<Procedure> entryPoints = new List<Procedure>();	
		private DirectedGraphImpl<Procedure> graphProcs = new DirectedGraphImpl<Procedure>();
		private DirectedGraphImpl<object> graphStms = new DirectedGraphImpl<object>();

		public void AddEdge(Statement stmCaller, Procedure callee)
		{
			graphProcs.AddNode(stmCaller.Block.Procedure);
			graphProcs.AddNode(callee);
			graphProcs.AddEdge(stmCaller.Block.Procedure, callee);

			graphStms.AddNode(stmCaller);
			graphStms.AddNode(callee);
			graphStms.AddEdge(stmCaller, callee);
		}

		public void AddEntryPoint(Procedure proc)
		{
			AddProcedure(proc);
			if (!entryPoints.Contains(proc))
			{
				entryPoints.Add(proc);
			}
		}

		public void AddProcedure(Procedure proc)
		{
			graphProcs.AddNode(proc);
			graphStms.AddNode(proc);
		}

		public void AddStatement(Statement stm)
		{
			graphStms.AddNode(stm);
		}

		public IEnumerable<object> Callees(Statement stm)
		{
            return graphStms.Successors(stm);
		}

		public IEnumerable<Procedure> Callees(Procedure proc)
		{
			return graphProcs.Successors(proc);
		}

        public IEnumerable<Procedure> CallerProcedures(Procedure proc)
		{
			return graphProcs.Predecessors(proc);
		}

        public IEnumerable<object> CallerStatements(Procedure proc)
		{
			return graphStms.Predecessors(proc);
		}

		public void Write(TextWriter wri)
		{
            var sl = graphProcs.Nodes.OrderBy(n => n, new ProcedureComparer());
			foreach (Procedure proc in sl)
			{
				wri.WriteLine("Procedure {0} calls:", proc.Name);
				foreach (Procedure p in graphProcs.Successors(proc))
				{
					wri.WriteLine("\t{0}", p.Name);
				}
			}

            var st = graphStms.Nodes.OfType<Statement>().OrderBy(n => n, new StmComparer());
            foreach (var stm in st)
            {
                wri.WriteLine("Statement {0:X8} {1} calls:", stm.LinearAddress, stm.Instruction);
                foreach (Procedure p in graphStms.Successors(stm).OfType<Procedure>())
                {
                    wri.WriteLine("\t{0}", p.Name);
                }
            }
		}

		private class ProcedureComparer : IComparer<Procedure>
		{
			#region IComparer Members

			public int Compare(Procedure x, Procedure y)
			{
				return string.Compare(x.Name, y.Name);
			}

			#endregion
		}

        private class StmComparer : IComparer<Statement>
        {
            public int Compare(Statement a, Statement b)
            {
                return a.LinearAddress < b.LinearAddress
                    ? -1
                    : a.LinearAddress > b.LinearAddress
                        ? 1
                        : 0;
            }
        }

		public List<Procedure> EntryPoints
		{
			get { return entryPoints; }
		}
	}
}
