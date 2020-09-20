using AbstraX.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Angular
{
    public class Component : ESModule, IDeclarable, IPageOrComponent
    {
        public Component(string name) : base(name, "Component")
        {
        }
    }
}
