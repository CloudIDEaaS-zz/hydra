using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.Contracts;

namespace AbstraX.Contracts
{
    public interface IPipelineStep
    {
        string Name { get; set; }
        string StepAssemblyFile { get; set; }
        IProjectStructure ProjectStructure { get; set; }
    }
}
