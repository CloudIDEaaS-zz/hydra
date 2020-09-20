using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public interface IViewProject
    {
        string ProjectPath { get; }
        IEnumerable<IView> Views { get; }
    }
}
