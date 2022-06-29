using AbstraX.FolderStructure;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Angular
{
    public class ESModule : Module
    {
        public override string Name { get; set; }

        public ESModule(string name, params string[] attributes) : base(name, attributes)
        {
        }
    }
}
