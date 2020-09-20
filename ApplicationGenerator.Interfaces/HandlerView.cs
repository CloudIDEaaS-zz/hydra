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
    public class HandlerView
    {
        public IModuleHandler Handler { get; }
        public IView View { get; }

        public HandlerView(IModuleHandler handler, IView view)
        {
            this.Handler = handler;
            this.View = view;
        }

        public string DebugInfo
        {
            get
            {
                return string.Format("Handler: {0}, "
                    + "View: {1}",
                    this.Handler,
                    this.View.File
                );
            }
        }
    }
}
