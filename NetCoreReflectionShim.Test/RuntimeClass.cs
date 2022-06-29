using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreReflectionShim.Test
{
    public interface IRuntime
    {
        public string Name { get; set; }
        void Test();
    }
}
