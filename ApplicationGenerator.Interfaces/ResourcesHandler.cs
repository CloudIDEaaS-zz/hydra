using AbstraX.DataAnnotations;
using AbstraX.Generators.Base;
using AbstraX.ServerInterfaces;
using System;
using System.Linq.Expressions;
using Utils;

namespace AbstraX
{
    public class ResourceHelper
    {
        private ResourcesHandler resourcesHandler;
        private dynamic resources;
        private IBase baseObject;

        public ResourceHelper(ResourcesHandler resourcesHandler, IBase baseObject, dynamic resources)
        {
            this.resourcesHandler = resourcesHandler;
            this.resources = resources;
            this.baseObject = baseObject;
        }

        internal string Get(Expression<Func<string>> expresson)
        {
            return resourcesHandler.CreateTranslationKey(baseObject, expresson);
        }
    }

    public class ResourcesHandler : HandlerObjectBase
    {
        private IAppResources appResources;
        private dynamic resources;

        public ResourcesHandler(IAppResources appResources) : base(null, null)
        {
            this.appResources = appResources;
        }

        public override object This => resources;

        public ResourceHelper CreateHelper(UIKind componentKind, IBase baseObject)
        {
            resources = appResources.GetResources(componentKind);

            return new ResourceHelper(this, baseObject, resources);
        }

        internal new IGeneratorConfiguration GeneratorConfiguration
        {
            set
            {
                base.GeneratorConfiguration = value;
            }
        }

    }
}