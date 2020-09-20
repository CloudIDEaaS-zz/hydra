using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Angular
{
    public class Provider : ESModule, IDeclarable
    {
        public Provider(string name) : base(name, "Injectable")
        {
        }
    }
}
