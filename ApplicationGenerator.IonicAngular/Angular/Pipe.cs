using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Angular
{
    public class Pipe : ESModule, IDeclarable
    {
        public Pipe(string name) : base(name, "Pipe")
        {
        }
    }
}
