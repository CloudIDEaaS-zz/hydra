using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX.Contracts
{
    public interface IProjectStructure
    {
        IProjectRoot ProjectRoot { get; set; }
        IArchitectureLayer ArchitectureLayer { get; set; }
    }
}
