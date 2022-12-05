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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Decompiler.Tools.C2Xml.UnitTests
{
    [TestFixture]
    public class CConstantEvaluatorTests
    {
        private CConstantEvaluator eval;
        private Dictionary<string, int> constants;

        [SetUp]
        public void Setup()
        {
            this.constants = new Dictionary<string,int>();
            eval = new CConstantEvaluator(constants);
        }

        [Test]
        public void CCEval_Constant()
        {
            var exp = new ConstExp { Const = 3 };
            var result = exp.Accept(eval);
            Assert.AreEqual(3, result);
        }

        [Test]
        public void CCEval_Add()
        {
            var exp =
                new CBinaryExpression
                {
                    Operation = CTokenType.Plus,
                    Left = new ConstExp { Const = 1 },
                    Right = new ConstExp { Const = 2 },
                };
            var result = exp.Accept(eval);
            Assert.AreEqual(3, result);
        }

        [Test]
        public void CCEval_Shr()
        {
            var exp =
                new CBinaryExpression
                {
                    Operation = CTokenType.Shr,
                    Left = new ConstExp { Const = 4 },
                    Right = new ConstExp { Const = 2 },
                };
            var result = exp.Accept(eval);
            Assert.AreEqual(1, result);
        }

    }
}
