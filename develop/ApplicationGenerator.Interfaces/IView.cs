using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public interface IView
    {
        Dictionary<string, object> ViewDataDictionary { get; }
        string Name { get; }
        string File { get; }
        bool Generated { get; set; }
        IViewProject ViewProject { get; }
        string GetLayout();
    }
}
