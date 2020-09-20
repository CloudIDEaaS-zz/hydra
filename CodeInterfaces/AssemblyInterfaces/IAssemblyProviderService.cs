using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces.AssemblyInterfaces
{
    public interface IAssemblyProviderService
    {
        IAssembliesRoot GetAssembliesRoot(List<string> assemblies);
    }
}
