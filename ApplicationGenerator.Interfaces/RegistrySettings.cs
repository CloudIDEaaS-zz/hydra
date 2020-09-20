using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    public class RegistrySettings : RegistrySettingsBase
    {
        public string PackagePathCache { get; set; }
        public string CurrentWorkingDirectory { get; set; }

        public RegistrySettings() : base(@"Software\Hydra\ApplicationGenerator")
        {
        }
    }
}
