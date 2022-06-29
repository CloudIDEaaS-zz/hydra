using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PackageAttribute : Attribute
    {
        public string PackageConfigPath { get; private set; }

        public PackageAttribute(string packageConfigPath)
        {
            this.PackageConfigPath = packageConfigPath;
        }
    }
}
