using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.QueryProviders;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using EntityProvider.Web.Entities.DatabaseEntities;

namespace EntityProvider.Web
{
    public class EntityProviderHierarchy<T> : ExpressionTraceableHierarchy<T> where T : IBase
    {
        private IAbstraXService service;

        public EntityProviderHierarchy(IAbstraXService service)
        {
            this.service = service;
        }

        public EntityProviderHierarchy(IAbstraXService service, string parentID) : base(parentID)
        {
            this.service = service;
        }

        protected override IRoot Root
        {
	        get 
            {
                return new EntitiesRoot(service);
            }
        }
    }
}
