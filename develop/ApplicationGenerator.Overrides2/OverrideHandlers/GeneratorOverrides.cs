using AbstraX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationGenerator.Overrides.OverrideHandlers
{
    public class GeneratorOverrides : IGeneratorOverrides
    {
        public bool OverridesNamespace => true;

        public void CopyFiles()
        {
        }

        public string GetNamespace()
        {
            return "HydraDevOps.Services";
        }
    }
}
