using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace ApplicationGeneratorBuildTasks
{
    public class FrameworkVersion
    {
        public string Framework { get; private set; }
        public Version Version { get; private set; }

        public FrameworkVersion(string framework, Version version)
        {
            this.Version = version;

            if (framework.IsOneOf("v", string.Empty))
            {
                this.Framework = "net";
            }
            else
            {
                this.Framework = framework;
            }
        }
    }
}
