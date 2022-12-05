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
using Decompiler.Core.Expressions;
using Decompiler.Core.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Gui
{
    public class HtmlCodeFormatter : CodeFormatter
    {
        private HtmlFormatter formatter;
        private IDictionary<Address, Procedure> procedureMap;

        public HtmlCodeFormatter(TextWriter writer, IDictionary<Address, Procedure> procedureMap)
            : base(new HtmlFormatter(writer))
        {
            this.formatter = (HtmlFormatter)InnerFormatter;
            this.formatter.Terminator = "<br />" + Environment.NewLine;
            this.procedureMap = procedureMap;
        }

        public override void VisitProcedureConstant(ProcedureConstant pc)
        {
            //$REVIEW: this may be very slow, in which case we will need to add an Address 
            // field to the Procedure class.
            foreach (var de in procedureMap)
            {
                if (de.Value == pc.Procedure)
                {
                    formatter.WriteHyperlink(pc.Procedure.Name, de.Key.ToString());
                    return;
                }
            }
            base.VisitProcedureConstant(pc);
        }
    }
}
