using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public class HandlerFacet
    {
        public IModuleHandler Handler { get; }
        public Facet Facet { get; }
        public IView View { get; }

        public HandlerFacet(IModuleHandler handler, Facet facet)
        {
            this.Handler = handler;
            this.Facet = facet;
        }

        public HandlerFacet(IModuleHandler handler, Facet facet, IView view)
        {
            this.Handler = handler;
            this.Facet = facet;
            this.View = view;
        }

        public string DebugInfo
        {
            get
            {
                var file = "<No File>";

                if (this.View != null)
                {
                    file = this.View.File;
                }

                return string.Format("Handler: {0}, "
                    + "Facet: {1}, "
                    + "View: {2}",
                    this.Handler,
                    this.Facet,
                    file
                );
            }
        }
    }
}
