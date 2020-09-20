using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Text;

namespace Utils
{
    public class PluginAssemblyLoadContext : AssemblyLoadContext
    {
        public PluginAssemblyLoadContext(string name) : base(name, true)
        {
        }
    }
}
