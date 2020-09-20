using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public class BaseEntityModelPair<TBase, TEntity> where TBase : IBase
    {
        public TBase BaseObject { get; }
        public TEntity Entity { get;  }
    }
}
