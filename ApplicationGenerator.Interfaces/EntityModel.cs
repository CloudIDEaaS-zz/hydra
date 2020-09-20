using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public class EntityModel<TEntity>
    {
        public TEntity Entity { get;  }
        public TEntity CreateScriptObject<TEntity>() { throw new Exception(); }
        public TEntity GetLoggedInUser<TEntity>() { throw new Exception(); }
        public object Create(object entity) { throw new Exception(); }
    }
}
