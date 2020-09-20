using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;

namespace AbstraX
{
    public delegate void GetAddInEntitiesEventHandler(object sender, GetAddInEntitiesEventArgs e);

    public class GetAddInEntitiesEventArgs : EventArgs
    {
        public List<IBase> Entities { get; set; }
        public EntityFactory EntityFactory { get; }
        public string PropertyName { get; }
        public IBase BaseObject { get;  }
        public DefinitionKind DefinitionKind { get; }
        public BaseType Type { get; }
        public bool NoMetadata { get; internal set; }
        public Func<IEnumerable<IBase>> GetCurrentChildren { get; set; }

        public IEnumerable<T> GetEntities<T>() where T : IBase
        {
            return this.Entities.Cast<T>();
        }

        public GetAddInEntitiesEventArgs(IBase baseObject, DefinitionKind definitionKind, BaseType dataType, EntityFactory entityFactory, Func<IEnumerable<IBase>> getCurrentChildren, string propertyName = null)
        {
            this.BaseObject = baseObject;
            this.DefinitionKind = definitionKind;
            this.Entities = new List<IBase>();
            this.EntityFactory = entityFactory;
            this.PropertyName = propertyName;
            this.Type = dataType;
            this.GetCurrentChildren = getCurrentChildren;
        }
    }
}
