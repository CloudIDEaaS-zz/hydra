using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public class ImportGroupAttribute : Attribute
    {
        public string[] Imports { get; private set; }
        public string GroupName { get; private set; }
        public int SubFolderCount { get; private set; }

        public ImportGroupAttribute(string groupName, int subFolderCount, params string[] imports)
        {
            this.GroupName = groupName;
            this.SubFolderCount = subFolderCount;
            this.Imports = imports;
        }

        public ImportGroupAttribute(string groupName, params string[] imports)
        {
            this.GroupName = groupName;
            this.SubFolderCount = 0;
            this.Imports = imports;
        }
    }
}
