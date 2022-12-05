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
using Decompiler.Core.Services;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Decompiler.Typing
{
	/// <summary>Performs transformations on the generated data types to make them 
	/// legal C-like data types.</summary>
	/// <remarks>
	/// Much of the type inference code in this namespace was inspired by the master's thesis
	/// "Entwicklung eines Typanalysesystem f�r einen Decompiler", 2004, by Raimar Falke.
	/// </remarks>
	public class TypeTransformer : IDataTypeVisitor<DataType>
	{
		private bool changed;
		private TypeFactory factory;
		private TypeStore store;
        private Program program;
		private Unifier unifier;
		private DataTypeComparer comparer;
		private TypeVariable tvCur;
        private DecompilerEventListener eventListener;

		private static TraceSwitch trace = new TraceSwitch("TypeTransformer", "Traces the transformation of types");
        private HashSet<DataType> visitedTypes;

        public TypeTransformer(TypeFactory factory, TypeStore store, Program prog)
            : this(factory, store, prog, new NullDecompilerEventListener())
        {
        }

		public TypeTransformer(TypeFactory factory, TypeStore store, Program program, DecompilerEventListener eventListener)
		{
			this.factory = factory;
			this.store = store;
            this.program = program;
			this.eventListener = eventListener;
			this.unifier = new Unifier(factory);
			this.comparer = new DataTypeComparer();
		}

		public bool Changed
		{
			get { return changed; }
			set { changed = value; }
		}

        /// <summary>
        /// Removes duplicate alternatives from a UnionType.
        /// </summary>
        /// <param name="u"></param>
        /// <returns>A (possibly) simplified UnionType.</returns>
		public UnionType FactorDuplicateAlternatives(UnionType u)
		{
			UnionType uNew = new UnionType(u.Name, u.PreferredType);
			foreach (UnionAlternative a in u.Alternatives.Values)
			{
				unifier.UnifyIntoUnion(uNew, a.DataType);
			}
			return uNew;
		}

        /// <summary>
        /// Returns true if a StructureType has fields that start at the same offset.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
		public bool HasCoincidentFields(StructureType s)
		{
			if (s.Fields.Count < 2)
				return false;
			int offset = s.Fields[0].Offset;
			for (int i = 1; i < s.Fields.Count; ++i)
			{
				int o = s.Fields[i].Offset;
				if (offset == o)
					return true;
				offset = o;
			}
			return false;
		}

		public StructureType MergeOffsetStructures(StructureType a, int aOffset, StructureType b, int bOffset)
		{
			int delta = bOffset - aOffset;
			foreach (StructureField f in b.Fields)
			{
				f.Offset += delta;
			}
			return (StructureType) unifier.UnifyStructures(a, b);
		}

		/// <summary>
		/// Merges staggered arrays in a structure into a single array.
		/// </summary>
		/// <param name="s"></param>
		public void MergeStaggeredArrays(StructureType s)
		{
			ArrayType arrMerged = null;
			StructureType strMerged = null;
            EquivalenceClass eqMerged = null;
			int offset = 0;
			for (int i = 0; i < s.Fields.Count; ++i)
			{
				ArrayType a = s.Fields[i].DataType as ArrayType;
				if (a == null)
					continue;
                EquivalenceClass eqElem = a.ElementType as EquivalenceClass;
                StructureType strElem;
                if (eqElem == null)
                    strElem = a.ElementType as StructureType;
                else
                    strElem = eqElem.DataType as StructureType;
				if (strElem == null)
					continue;

				if (StructuresOverlap(strMerged, offset, strElem, s.Fields[i].Offset))
				{
					strMerged = MergeOffsetStructures(strMerged, offset, strElem, s.Fields[i].Offset);
                    if (eqMerged != null)
                        eqMerged.DataType = strMerged;
                    else
                        arrMerged.ElementType = strMerged;
					s.Fields.RemoveAt(i);
                    Changed = true;
					--i;
				}
				else
				{
					arrMerged = a;
					strMerged = strElem;
                    eqMerged = eqElem;
					offset = s.Fields[i].Offset;
				}
			}
		}

        private T ResolveAs<T>(DataType dt) where T : DataType
        {
            for (; ; )
            {
                EquivalenceClass eq = dt as EquivalenceClass;
                if (eq == null)
                    return dt as T;
                dt = eq.DataType;
            }
        }

		public StructureType MergeStructureFields(StructureType str)
		{
			if (!HasCoincidentFields(str))
				return str;
			StructureType strNew = new StructureType(str.Name, str.Size);
            strNew.IsSegment = str.IsSegment;
			UnionType ut = new UnionType(null, null);
			int offset = 0;
			foreach (StructureField f in str.Fields)
			{
				//$REVIEW: what if multiple fields have differing names?
				if (ut.Alternatives.Count == 0)
				{
					offset = f.Offset;
					ut.Alternatives.Add(f.DataType);
				}
				else
				{
					if (f.Offset == offset)
					{
						UnionType uf = f.DataType as UnionType;
						if (uf != null)
						{
							ut = unifier.UnifyUnions(ut, uf);
						}
						else
						{
							unifier.UnifyIntoUnion(ut, f.DataType);
						}
					}
					else
					{
						strNew.Fields.Add(offset, ut.Simplify());
						offset = f.Offset;
						ut = new UnionType(null, null);
						ut.Alternatives.Add(f.DataType);
					}
				}
			}
			if (ut.Alternatives.Count > 0)
			{
				strNew.Fields.Add(offset, ut.Simplify());
			}

            StructureFieldMerger sfm = new StructureFieldMerger();
            strNew = sfm.Merge(strNew);
			return strNew;
		}

		public bool StructuresOverlap(StructureType a, int aOffset, StructureType b, int bOffset)
		{
			if (a == null || b == null)
				return false;
			if (a.Size != b.Size)
				return false;
			return (aOffset + a.Size > bOffset);
		}

		public void Transform()
		{
            var dpa = new DerivedPointerAnalysis(factory, store, program);
			var ppr = new PtrPrimitiveReplacer(factory, store, program);
            ppr.ReplaceAll();
            dpa.FollowDerivedPointers();
			int iteration = 0;
			do
			{
				++iteration;
                if (iteration > 50)
                {
                    eventListener.Warn(new NullCodeLocation(""),
                        string.Format("Type transformer has looped {0} times, quitting prematurely.", iteration));
                    return;
                }
				Changed = false;
                this.visitedTypes = new HashSet<DataType>();
				foreach (TypeVariable tv in store.TypeVariables)
				{
					tvCur = tv;
					EquivalenceClass eq = tv.Class;
					if (eq.DataType != null)
					{
						eq.DataType = eq.DataType.Accept(this);
					}
                    if (tv.DataType != null)
                    {
                        tv.DataType = tv.DataType.Accept(this);
                    }
                    // Debug.Print("Transformed {0}:{1}", tv, tv.Class.DataType);
				}
				if (ppr.ReplaceAll())
					Changed = true;
				if (NestedComplexTypeExtractor.ReplaceAll(factory, store))
					Changed = true;
			} while (Changed);
		}

		private void DumpStore(int iteration, System.IO.TextWriter writer)
		{
			if (writer == null)
				return;
			writer.WriteLine("// Store dump: iteration {0} ///////////////////////");
			store.Write(writer);
			writer.WriteLine();
			writer.Flush();
		}

		#region DataTypeTransformer methods  //////////////////////////////////////////

        public DataType VisitArray(ArrayType arr)
        {
            arr.ElementType = arr.ElementType.Accept(this);
            return arr;
        }

        public DataType VisitCode(CodeType c)
        {
            return c;
        }

        public DataType VisitEnum(EnumType e)
        {
            return e;
        }

        public DataType VisitEquivalenceClass(EquivalenceClass eq)
        {
            return eq;
        }

        public DataType VisitFunctionType(FunctionType fn)
        {
            if (fn.ReturnType != null)
            {
                fn.ReturnType = fn.ReturnType.Accept(this);
            }
            for (int i = 0; i < fn.ArgumentTypes.Length; ++i)
            {
                fn.ArgumentTypes[i] = fn.ArgumentTypes[i].Accept(this);
            }
            return fn;
        }

        public DataType VisitMemberPointer(MemberPointer mptr)
        {
            mptr.BasePointer = mptr.BasePointer.Accept(this);
            mptr.Pointee = mptr.Pointee.Accept(this);
            var array = mptr.Pointee as ArrayType;
            if (array != null)
            {
                Changed = true;
                return factory.CreateMemberPointer(
                    mptr.BasePointer,
                    array.ElementType,
                    mptr.Size);
            }
            return mptr;
        }

        public DataType VisitPointer(Pointer ptr)
        {
            ptr.Pointee = ptr.Pointee.Accept(this);
            return ptr;
        }

        public DataType VisitPrimitive(PrimitiveType pt)
        {
            return pt;
        }

        public DataType VisitString(StringType str)
        {
            return str;
        }

        public DataType VisitStructure(StructureType str)
		{
            if (visitedTypes.Contains(str))
                return str;
            visitedTypes.Add(str);
            foreach (var field in str.Fields)
            {
                field.DataType = field.DataType.Accept(this);
            }
			StructureType strNew = MergeStructureFields(str);
			if (strNew.Fields.Count != str.Fields.Count)
				Changed = true;
			MergeStaggeredArrays(strNew);
			DataType dt = strNew.Simplify();
			if (dt != strNew)
				Changed = true;
			return dt;
		}

        public DataType VisitTypeReference(TypeReference typeref)
        {
            return typeref;
        }
        public DataType VisitTypeVariable(TypeVariable tv)
        {
            return tv;
        }

		public DataType VisitUnion(UnionType ut)
		{
            foreach (var alt in ut.Alternatives.Values)
            {
                alt.DataType = alt.DataType.Accept(this);
            }

			var upsm = new UnionPointersStructuresMatcher();
			if (upsm.Match(ut))
			{
				StructureMerger sm = new StructureMerger(upsm.Structures, upsm.EquivalenceClasses);
				sm.Merge();
				Changed = true;
				return new Pointer(sm.MergedClass, upsm.PointerSize);
			}

			UnionType utNew = FactorDuplicateAlternatives(ut);
			if (utNew.Alternatives.Count != ut.Alternatives.Count)
				Changed = true;
			return utNew.Simplify();
		}

        public DataType VisitUnknownType(UnknownType unk)
        {
            return unk;
        }

        public DataType VisitVoidType(VoidType vt)
        {
            return vt;
        }

		#endregion
	}
}
