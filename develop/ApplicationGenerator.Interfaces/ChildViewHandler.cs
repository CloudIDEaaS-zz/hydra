using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.ServerInterfaces;

namespace AbstraX
{
    public delegate void ChildViewHandler(object sender, ChildViewHandlerEventArgs e);

    public class ChildViewHandlerEventArgs : EventArgs
    {
        public string PartialViewComponentName { get; set; }
        public string PartialViewName { get; }
        public List<IBase> ModelObjectGraph { get; }
        public Dictionary<string, object> ViewData;

        public ChildViewHandlerEventArgs(string partialViewName, List<IBase> modelObjectGraph, Dictionary<string, object> viewData)
        {
            this.PartialViewName = partialViewName;
            this.ModelObjectGraph = modelObjectGraph;
            this.ViewData = viewData;
        }
    }
}
