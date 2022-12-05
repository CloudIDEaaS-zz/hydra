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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Services;
using Decompiler.Core.Types;
using System;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Typing
{
	/// <summary>
	/// Gathers type information, infers structure, union, and array types,
	/// then rewrites the program as appropriate to incorporate the inferred types.
	/// Much of the type inference code in this namespace was inspired by the master's thesis
	/// "Entwicklung eines Typanalysesystem f�r einen Decompiler", 2004, by Raimar Falke.
	/// </summary>
	public class TypeAnalyzer
	{
        private DecompilerEventListener eventListener;

		private TypeFactory factory;
		private TypeStore store;
		private ExpressionNormalizer aen;
		private EquivalenceClassBuilder eqb;
		private TraitCollector trco;
		private DataTypeBuilder dtb;
        //private DerivedPointerAnalysis dpa;
		private TypeVariableReplacer tvr;
		private TypeTransformer trans;
		private ComplexTypeNamer ctn;
		private TypedExpressionRewriter ter;

		public TypeAnalyzer(DecompilerEventListener eventListener)
		{
			this.eventListener = eventListener;
		}

		/// <summary>
		/// Performs type analysis and rewrites program based on the inferred information.
		/// </summary>
		/// <remarks>
		/// For instance, all MemoryAccesses will be converted to structure field
		/// accesses or array accesses as appropriate.
		/// </remarks>
		/// <param name="prog"></param>
		public void RewriteProgram(Program prog)
		{
            factory = prog.TypeFactory;
            store = prog.TypeStore;

            aen = new ExpressionNormalizer(prog.Platform.PointerType);
            eqb = new EquivalenceClassBuilder(factory, store);
            dtb = new DataTypeBuilder(factory, store, prog.Platform);
            trco = new TraitCollector(factory, store, dtb, prog);
            //dpa = new DerivedPointerAnalysis(factory, store, prog.Architecture);
            tvr = new TypeVariableReplacer(store);
            trans = new TypeTransformer(factory, store,prog, eventListener);
            ctn = new ComplexTypeNamer();
            ter = new TypedExpressionRewriter(prog);

            // RestrictProcedures(prog, 0, 1, true); //$DEBUG
            eventListener.ShowStatus("Gathering primitive datatypes from instructions.");
			aen.Transform(prog);
			eqb.Build(prog);
            eventListener.ShowStatus("Collecting datatype usage traits.");
			trco.CollectProgramTraits(prog);
            eventListener.ShowStatus("Building equivalence classes.");
			dtb.BuildEquivalenceClassDataTypes();
            //dpa.FollowConstantPointers(prog);
			tvr.ReplaceTypeVariables();

            eventListener.ShowStatus("Transforming datatypes.");
			var ppr = new PtrPrimitiveReplacer(factory, store, prog);
			ppr.ReplaceAll();

			trans.Transform();
			ctn.RenameAllTypes(store);
			store.Dump();
            eventListener.ShowStatus("Rewriting expressions.");
			ter.RewriteProgram(prog);
		}

        /// <summary>
        /// $DEBUG: for debugging only, only performs type analysis on the count procedures starting at
        /// procedure start.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        [Conditional("DEBUG")]
        private void RestrictProcedures(Program prog, int start, int count, bool dumpProcedures)
        {
            count = Math.Min(count, prog.Procedures.Values.Count);
            Procedure[] procs = new Procedure[count];
            for (int i = 0; i < count; ++i)
            {
                procs[i] = prog.Procedures.Values[i + start];
            }
            prog.Procedures.Clear();
            for (uint i = 0; i < procs.Length; ++i)
            {
                prog.Procedures[Address.Ptr32(i)] = procs[i];
                if (dumpProcedures)
                {
                    procs[i].Dump(true, false);
                }
                else
                {
                    Debug.WriteLine(procs[i].Name);
                }
            }
        }

		public void WriteTypes(TextWriter output)
		{
		}
	}
}
