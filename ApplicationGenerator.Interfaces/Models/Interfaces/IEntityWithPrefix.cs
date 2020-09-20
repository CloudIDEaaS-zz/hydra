using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Models.Interfaces
{
    public interface IEntityWithPrefix
    {
        string PathPrefix { get; }
        string ControllerNamePrefix { get; }
        string ConfigPrefix { get; }
        string Namespace { get; }
    }
}
