using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreReflectionShim.Test
{
    public interface IRuntime
    {
        string Name { get; set; }
        void Test(string p1, int p2, Type p3);
    }
}
