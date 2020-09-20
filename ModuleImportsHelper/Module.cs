using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace ModuleImportsHelper
{
    [DebuggerDisplay(" { ModuleName } ")]
    public class Module
    {
        public Package Package { get; set; }
        public string ModuleName { get; internal set; }
        public string ModuleIdString { get; internal set; }

        public Module()
        {
        }

        public string ModuleGuidId
        {
            get
            {
                return string.Format("{{\" + {0} + \"-\" + {1} + \"}}", this.Package.HandlerIdStringName, this.ModuleIdStringName);
            }
        }

        public string ModuleGuidIdName
        {
            get
            {
                return this.Package.PackageName.ToUpper() + "_" + this.ModuleName.RemoveSurrounding("[", "]");
            }
        }

        public string ModuleId
        {
            get
            {
                return "0x" + ((string)this.ModuleIdString).RemoveText("-");
            }
        }

        public string ModuleIdName
        {
            get
            {
                return this.Package.PackageName.ToUpper() + "_" + this.ModuleName.RemoveSurrounding("[", "]").ToUpper() + "_ID";
            }
        }

        public string ModuleIdStringName
        {
            get
            {
                return this.Package.PackageName.ToUpper() + "_" + this.ModuleName.RemoveSurrounding("[", "]").ToUpper() + "_ID_STRING";
            }
        }
    }
}
