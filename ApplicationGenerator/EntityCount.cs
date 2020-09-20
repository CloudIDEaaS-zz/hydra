using AbstraX.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public class EntityCount
    {
        public IEntityType Entity { get; set; }
        public int Count { get; set; }

        public EntityCount(IEntityType entity)
        {
            this.Entity = entity;
            this.Count = 1;
        }
    }
}
