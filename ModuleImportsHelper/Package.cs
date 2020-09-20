using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace ModuleImportsHelper
{
    [DebuggerDisplay(" { PackageName } ")]
    public class Package
    {
        public string PackageName { get; internal set; }
        public string ImportPath { get; internal set; }
        public string InheritsFrom { get; internal set; }
        public string HandlerIdString { get; internal set; }
        public List<Module> Modules { get; set; }
        public List<Module> AddFromModules { get; set; }
        public string CombinedName { get; internal set; }

        public string CombinedNameValue
        {
            get
            {
                var builder = new StringBuilder();
                IEnumerable<string> moduleNames;

                if (this.IsPackageGroupOnly)
                {
                    moduleNames = this.AddFromModules.Select(m => m.ModuleGuidIdName);
                }
                else
                {
                    moduleNames = this.Modules.Select(m => m.ModuleGuidIdName).Concat(this.AddFromModules.Select(m => m.ModuleGuidIdName));
                }

                foreach (var moduleName in moduleNames)
                {
                    builder.AppendWithLeadingIfLength(" + \", \" + ", moduleName);
                }

                return builder.ToString();
            }
        }

        public bool IsPackageGroupOnly
        {
            get
            {
                return this.PackageName == null;
            }
        }
   

        public string HandlerId
        {
            get
            {
                if (this.IsPackageGroupOnly)
                {
                    return null;
                }
                else
                {
                    return "0x" + ((string)this.HandlerIdString).RemoveText("-");
                }
            }
        }

        public string HandlerIdName
        {
            get
            {
                if (this.IsPackageGroupOnly)
                {
                    return null;
                }
                else
                {
                    return this.PackageName.ToUpper() + "_HANDLER_ID";
                }
            }
        }

        public string HandlerIdStringName
        {
            get
            {
                if (this.IsPackageGroupOnly)
                {
                    return null;
                }
                else
                {
                    return this.PackageName.ToUpper() + "_HANDLER_ID_STRING";
                }
            }
        }
    }
}
