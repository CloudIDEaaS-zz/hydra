using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public delegate void ProcessFacetsHandler(object sender, ProcessFacetsEventArgs e);

    public class ProcessFacetsEventArgs
    {
        public Type[] Types { get; }

        public ProcessFacetsEventArgs(params Type[] types)
        {
            this.Types = types;
        }
    }
}
