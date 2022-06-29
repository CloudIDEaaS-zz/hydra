using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Angular
{
    public class ExportedFunction : Module
    {
        public override string Name { get; set; }

        public ExportedFunction(string name) : base(name)
        {
        }
    }
}
