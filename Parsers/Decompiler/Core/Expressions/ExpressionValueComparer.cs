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

using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Core.Expressions
{
    /// <summary>
    /// Deep-compares expressions.
    /// </summary>
    public class ExpressionValueComparer : IEqualityComparer<Expression>
    {
        private delegate bool EqualsFn(Expression x, Expression y);
        private delegate int HashFn(Expression obj);

        private static Dictionary<Type, EqualsFn> eqs = new Dictionary<Type, EqualsFn>();
        private static Dictionary<Type, HashFn> hashes = new Dictionary<Type, HashFn>();

        static ExpressionValueComparer()
        {
            Add(typeof(Application),
                delegate(Expression ea, Expression eb)
                {
                    Application a = (Application) ea, b = (Application) eb;
                    if (a.Arguments.Length != b.Arguments.Length)
                        return false;
                    if (!EqualsImpl(a.Procedure, b.Procedure))
                        return false;
                    for (int i = 0; i != a.Arguments.Length; ++i)
                    {
                        if (!EqualsImpl(a.Arguments[i], b.Arguments[i]))
                            return false;
                    }
                    return true;
                },
                delegate(Expression obj)
                {
                    Application a = (Application) obj;
                    int h = GetHashCodeImpl(a.Procedure);
                    h ^= a.Arguments.Length;
                    foreach (Expression e in a.Arguments)
                    {
                        h *= 47;
                        if (e != null)
                            h ^= GetHashCodeImpl(e);
                    }
                    return h;
                });
            Add(typeof(BinaryExpression),
                delegate(Expression ea, Expression eb)
                {
                    BinaryExpression a = (BinaryExpression) ea, b = (BinaryExpression) eb;
                    if (a.Operator != b.Operator)
                        return false;
                    return (EqualsImpl(a.Left, b.Left) && EqualsImpl(a.Right, b.Right));
                },
                delegate(Expression obj)
                {
                    BinaryExpression b = (BinaryExpression) obj;
                    return b.Operator.GetHashCode() ^ GetHashCodeImpl(b.Left) ^ 47 * GetHashCodeImpl(b.Right);
                });
            Add(typeof(Cast),
                delegate(Expression ea, Expression eb)
                {
                    Cast a = (Cast)ea, b = (Cast)eb;
                    return EqualsImpl(a.Expression, b.Expression);
                },
                delegate(Expression obj)
                {
                    Cast c = (Cast)obj;
                    return GetHashCodeImpl(c.Expression) * 43;
                });
            Add(typeof(ConditionOf),
                delegate(Expression ea, Expression eb)
                {
                    ConditionOf a = (ConditionOf) ea, b = (ConditionOf) eb;
                    return EqualsImpl(a.Expression, b.Expression);
                },
                delegate(Expression obj)
                {
                    return 0x10101010 * GetHashCodeImpl(((ConditionOf) obj).Expression);
                });
            Add(typeof(Address),
                addrComp,
                addrHash);
            Add(typeof(Address16),
                addrComp,
                addrHash);
            Add(typeof(Address32),
                addrComp,
                addrHash);
            Add(typeof(Address64),
                addrComp,
                addrHash);

            Add(typeof(Constant),
                delegate(Expression ea, Expression eb)
                {
                    Constant a = (Constant) ea, b = (Constant) eb;
                    return object.Equals(a.ToUInt64(), b.ToUInt64());
                },
                delegate(Expression obj)
                {
                    return ((Constant)obj).ToUInt64().GetHashCode();
                });

            Add(typeof(DepositBits),
                delegate(Expression ea, Expression eb)
                {
                    DepositBits a = (DepositBits) ea, b = (DepositBits) eb;
                    return a.BitCount == b.BitCount && a.BitPosition == b.BitPosition &&
                        EqualsImpl(a.Source, b.Source) && EqualsImpl(a.InsertedBits, b.InsertedBits);
                },
                delegate(Expression obj)
                {
                    DepositBits dpb = (DepositBits) obj;
                    return GetHashCodeImpl(dpb.Source) * 67 ^ GetHashCodeImpl(dpb.InsertedBits) * 43 ^ dpb.BitPosition * 7 ^ dpb.BitCount;
                });

            Add(typeof(Dereference),
                delegate(Expression ea, Expression eb)
                {
                    Dereference a = (Dereference) ea, b = (Dereference) eb;
                    return EqualsImpl(a.Expression, b.Expression);
                },
                delegate(Expression obj)
                {
                    return GetHashCodeImpl(((Dereference) obj).Expression) * 129;
                });

            Add(
                typeof(Identifier),
                delegate(Expression x, Expression y)
                {
                    return ((Identifier) x).Name == ((Identifier) y).Name;
                },
                delegate(Expression x)
                {
                    return ((Identifier) x).Name.GetHashCode();
                });
            Add(typeof(MemoryAccess),
                delegate(Expression ea, Expression eb)
                {
                    MemoryAccess a = (MemoryAccess) ea, b = (MemoryAccess) eb;
                    return EqualsImpl(a.MemoryId, b.MemoryId) &&
                        a.DataType == b.DataType &&
                        EqualsImpl(a.EffectiveAddress, b.EffectiveAddress);
                },
                delegate(Expression obj)
                {
                    MemoryAccess m = (MemoryAccess) obj;
                    return GetHashCodeImpl(m.MemoryId) ^ m.DataType.GetHashCode() ^ 47 * GetHashCodeImpl(m.EffectiveAddress);
                });

            Add(typeof(MemoryIdentifier),
                delegate(Expression ea, Expression eb)
                {
                    return ((MemoryIdentifier)ea).Name == ((Identifier)eb).Name;
                },
                delegate(Expression x)
                {
                    return ((Identifier)x).Name.GetHashCode();
                });
            Add(typeof(MkSequence),
                delegate(Expression ea, Expression eb)
                {
                    var a = (MkSequence)ea;
                    var b = (MkSequence)eb;
                    return EqualsImpl(a.Head, b.Tail) && EqualsImpl(a.Head, b.Tail);
                },
                delegate(Expression obj)
                {
                    var s = (MkSequence)obj;
                    return obj.GetType().GetHashCode() ^ 37 * GetHashCodeImpl(s.Head) ^
                        GetHashCodeImpl(s.Tail);
                });

            Add(typeof(PhiFunction),
                delegate(Expression ea, Expression eb)
                {
                    PhiFunction a = (PhiFunction) ea, b = (PhiFunction) eb;
                    if (a.Arguments.Length != b.Arguments.Length)
                        return false;
                    for (int i = 0; i != a.Arguments.Length; ++i)
                    {
                        if (!EqualsImpl(a.Arguments[i], b.Arguments[i]))
                            return false;
                    }
                    return true;
                },
                delegate(Expression obj)
                {
                    PhiFunction phi = (PhiFunction) obj;
                    int h = phi.Arguments.Length.GetHashCode();
                    for (int i = 0; i < phi.Arguments.Length; ++i)
                    {
                        h = h * 47 ^ GetHashCodeImpl(phi.Arguments[i]);
                    }
                    return h;
                });

            Add(typeof(ProcedureConstant),
                delegate(Expression ea, Expression eb)
                {
                    ProcedureConstant a = (ProcedureConstant) ea, b = (ProcedureConstant) eb;
                    return a.Procedure == b.Procedure;
                },
                delegate(Expression obj)
                {
                    return ((ProcedureConstant) obj).GetHashCode();
                });

            Add(typeof(SegmentedAccess),
                delegate(Expression ea, Expression eb)
                {
                    SegmentedAccess a = (SegmentedAccess) ea, b = (SegmentedAccess) eb;
                    return
                        EqualsImpl(a.BasePointer, b.BasePointer) &&
                        EqualsImpl(a.MemoryId, b.MemoryId) &&
                        a.DataType == b.DataType &&
                        EqualsImpl(a.EffectiveAddress, b.EffectiveAddress);
                },
                delegate(Expression obj)
                {
                    SegmentedAccess m = (SegmentedAccess) obj;
                    return GetHashCodeImpl(m.MemoryId) ^
                        m.DataType.GetHashCode() ^
                        47 * GetHashCodeImpl(m.EffectiveAddress) ^
                        GetHashCodeImpl(m.BasePointer);
                });

            Add(typeof(Slice),
                delegate(Expression ea, Expression eb)
                {
                    Slice a = (Slice) ea, b = (Slice) eb;
                    return EqualsImpl(a.Expression, b.Expression) &&
                        a.Offset == b.Offset && a.DataType == b.DataType;
                },
                delegate(Expression obj)
                {
                    Slice s = (Slice) obj;
                    return GetHashCodeImpl(s.Expression) ^ s.Offset * 47 ^ s.DataType.GetHashCode() * 23;
                }); 


            Add(
                typeof(TestCondition),
                delegate(Expression x, Expression y)
                {
                    TestCondition tx = (TestCondition) x, ty = (TestCondition) y; 
                    return Equals(tx.ConditionCode, ty.ConditionCode) && EqualsImpl(tx.Expression, ty.Expression);
                },
                delegate(Expression x)
                {
                    TestCondition tx = (TestCondition) x;
                    return tx.ConditionCode.GetHashCode() ^ GetHashCodeImpl(tx.Expression) & 47;
                });

            Add(typeof (UnaryExpression),
                delegate(Expression x, Expression y)
                {
                    UnaryExpression a = (UnaryExpression) x, b = (UnaryExpression) y;
                    return a.Operator == b.Operator && 
                        EqualsImpl(a.Expression, b.Expression);
                },
                delegate(Expression obj)
                {
                    UnaryExpression u = (UnaryExpression) obj;
                    return GetHashCodeImpl(u.Expression) ^ u.Operator.GetHashCode();
                });
        }

        private static bool addrComp(Expression ea, Expression eb)
        {
            Address a = (Address)ea, b = (Address)eb;
            return a.ToLinear() == b.ToLinear();
        }

        private static int addrHash(Expression obj)
        {
            return ((Address)obj).ToLinear().GetHashCode();
        }

        private static void Add(Type t, EqualsFn eq, HashFn hash)
        {
            eqs.Add(t, eq);
            hashes.Add(t, hash);
        }

        #region IEqualityComparer Members

        public bool Equals(Expression x, Expression y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return EqualsImpl(x, y);
        }

        private static bool EqualsImpl(Expression x, Expression y)
        {
            Type tx = x.GetType();
            if (typeof(Constant).IsAssignableFrom(tx))
                tx = typeof(Constant);
            Type ty = y.GetType();
            if (typeof(Constant).IsAssignableFrom(ty))
                ty = typeof(Constant);
            if (tx != ty)
                return false;

            return eqs[tx](x, y);
        }

        private static int GetHashCodeImpl(Expression obj)
        {
            if (obj == null)
                throw new ArgumentNullException();
            var tc = typeof(Constant);
            Type t = obj.GetType();
            if (tc.IsAssignableFrom(t))
                t = tc;
            return hashes[t](obj);
        }

        public int GetHashCode(Expression obj)
        {
            return GetHashCodeImpl(obj);
        }
        #endregion
    }
}
