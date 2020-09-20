using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX.Contracts.Views
{
    public interface IViewsBase
    {
        string ID { get; }
        string Name { get; }
        string Description { get; }
    }
}
