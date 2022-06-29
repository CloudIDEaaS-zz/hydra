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
        public UIKind UIKind { get; }

        public Page(string name, UILoadKind loadKind, UIKind uiKind) : base(name, "Page")
        {
            this.UILoadKind = loadKind;
            this.UIKind = uiKind;
        }

        public Page(string name, UIKind uiKind) : base(name, "Page")
        {
            this.UIKind = uiKind;
        }
    }
}
