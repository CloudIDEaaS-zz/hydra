using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces
{
    public interface IProjectStructure
    {
        IProjectRoot ProjectRoot { get; set; }
        IArchitectureLayer ArchitectureLayer { get; set; }
    }
}
