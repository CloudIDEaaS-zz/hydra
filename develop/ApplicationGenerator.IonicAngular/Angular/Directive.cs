using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Angular
{
    public class Directive : ESModule, IDeclarable
    {
        public Directive(string name) : base(name, "Directive")
        {
        }
    }
}
