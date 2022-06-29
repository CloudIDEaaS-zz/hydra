// file:	ResourcesHandler.cs
//
// summary:	Implements the resources handler class

using AbstraX.DataAnnotations;
using AbstraX.Generators.Base;
using AbstraX.ServerInterfaces;
using System;
using System.Linq.Expressions;
using Utils;

namespace AbstraX
{
    /// <summary>   A resource helper. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>

    public class ResourceHelper
    {
        /// <summary>   The resources handler. </summary>
        private ResourcesHandler resourcesHandler;
        /// <summary>   The resources. </summary>
        private dynamic resources;
        /// <summary>   The base object. </summary>
        private IBase baseObject;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>
        ///
        /// <param name="resourcesHandler"> The resources handler. </param>
        /// <param name="baseObject ">       The base object. </param>
        /// <param name="resources">        The resources. </param>

        public ResourceHelper(ResourcesHandler resourcesHandler, IBase baseObject, dynamic resources)
        {
            this.resourcesHandler = resourcesHandler;
            this.resources = resources;
            this.baseObject = baseObject;
        }

        /// <summary>   Gets a string using the given expresson. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>
        ///
        /// <param name="expresson">    The expresson to get. </param>
        ///
        /// <returns>   A string. </returns>

        public string Get(Expression<Func<string>> expresson)
        {
            return resourcesHandler.CreateTranslationKey(baseObject, expresson);
        }
    }

    /// <summary>   The resources handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>

    public class ResourcesHandler : HandlerObjectBase
    {
        /// <summary>   The application resources. </summary>
        private IAppResources appResources;
        /// <summary>   The resources. </summary>
        private dynamic resources;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>
        ///
        /// <param name="appResources"> The application resources. </param>

        public ResourcesHandler(IAppResources appResources) : base(null, null)
        {
            this.appResources = appResources;
        }

        /// <summary>   Gets this. </summary>
        ///
        /// <value> this. </value>

        public override object This => resources;

        /// <summary>   Helper method that create. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>
        ///
        /// <param name="componentKind">    The component kind. </param>
        /// <param name="baseObject">       The base object. </param>
        ///
        /// <returns>   The new helper. </returns>

        public ResourceHelper CreateHelper(UIKind componentKind, IBase baseObject)
        {
            resources = appResources.GetResources(componentKind);

            if (resources is string && JsonExtensions.IsValidJson(((string) resources)))
            {
                resources = JsonExtensions.ReadJson<dynamic>(resources);
            }

            return new ResourceHelper(this, baseObject, resources);
        }

        /// <summary>   Sets the generator configuration. </summary>
        ///
        /// <value> The generator configuration. </value>

        internal new IGeneratorConfiguration GeneratorConfiguration
        {
            set
            {
                base.GeneratorConfiguration = value;
            }
        }

    }
}