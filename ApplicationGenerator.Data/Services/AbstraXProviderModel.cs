using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ApplicationGenerator.Data
{
    public class AbstraXProviderModel : IModel
    {
        public AbstraXProviderModel()
        { 
        }

        public object this[string name] 
        {
            get
            {
                return null;
            }
        }

        public IAnnotation FindAnnotation(string name)
        {
            return null;
        }

        public IEntityType FindEntityType(string name)
        {
            return null;
        }

        public IEntityType FindEntityType(string name, string definingNavigationName, IEntityType definingEntityType)
        {
            return null;
        }

        public IEnumerable<IAnnotation> GetAnnotations()
        {
            return null;
        }

        public IEnumerable<IEntityType> GetEntityTypes()
        {
            return null;
        }
    }
}