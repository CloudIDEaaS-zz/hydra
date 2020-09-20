using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.DataAnnotations;

namespace AbstraX.Angular
{
    public class Page : ESModule, IDeclarable, IPageOrComponent
    {
        public UILoadKind UILoadKind { get; }

        public Page(string name, UILoadKind loadKind) : base(name, "Page")
        {
            this.UILoadKind = loadKind;
        }

        public Page(string name) : base(name, "Page")
        {
        }
    }
}
