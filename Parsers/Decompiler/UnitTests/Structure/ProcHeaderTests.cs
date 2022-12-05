using Decompiler.Core;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Structure
{
    [TestFixture]
    public class ProcHeaderTests
    {
        ProcedureStructure proc;

        [Test]
        public void ProcHeader_Create()
        {
            RunTest(delegate(ProcedureBuilder m)
            {
                m.Return();
            });
            Assert.AreEqual(3, proc.Nodes.Count);
            Assert.AreSame(proc.Nodes[1], proc.Nodes[0].OutEdges[0]);
            Assert.AreEqual(proc.Nodes[0], proc.Nodes[1].InEdges[0]);
        }

        protected virtual void RunTest(Action<ProcedureBuilder> pg)
        {
            ProcedureBuilder pm = new ProcedureBuilder();
            pg(pm);
            RunTest(pm);
        }

        protected virtual void RunTest(ProcedureBuilder pm)
        {
            ProcedureStructureBuilder g = new ProcedureStructureBuilder(pm.Procedure);
            g.BuildNodes();
            g.DefineEdges();
            proc = g.CreateProcedureStructure();
            g.SetTimeStamps();
        }
    }
}
