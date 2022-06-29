using AbstraX.DataAnnotations;
using AbstraX.Models.Interfaces;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   A facet modules. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/30/2020. </remarks>

    public class FacetPartsModules
    {
        /// <summary>   Gets or sets the facet. </summary>
        ///
        /// <value> The facet. </value>

        public Facet Facet { get; private set; }

        /// <summary>   Gets the entity object. </summary>
        ///
        /// <value> The entity object. </value>

        public IEntityObjectWithFacets EntityObject { get; }

        /// <summary>   Gets the module assemblies. </summary>
        ///
        /// <value> The module assemblies. </value>

        public List<IModuleAssembly> ModuleAssemblies { get; set; }

        /// <summary>   Gets the full pathname of the hierarchy file. </summary>
        ///
        /// <value> The full pathname of the hierarchy file. </value>

        public string UIHierarchyPath { get; }

        /// <summary>   Gets the element parts. </summary>
        ///
        /// <value> The element parts. </value>

        public List<string> ElementParts { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/30/2020. </remarks>
        ///
        /// <param name="facet">        The facet. </param>
        /// <param name="entityObject"> The entity object. </param>
        /// <param name="resolver">     The resolver. </param>

        public FacetPartsModules(Facet facet, IEntityObjectWithFacets entityObject, PartsAliasResolver resolver)
        {
            var attribute = facet.Attribute;

            if (attribute is UIAttribute)
            {
                var componentAttribute = (UIAttribute)attribute;

                if (componentAttribute.UIHierarchyPath != null)
                {
                    var queue = componentAttribute.ParseHierarchyPath(resolver);

                    this.UIHierarchyPath = componentAttribute.UIHierarchyPath;
                    this.ElementParts = queue.SplitElementParts(entityObject).ToList();
                }
            }

            this.Facet = facet;
            this.EntityObject = entityObject;
            this.ModuleAssemblies = new List<IModuleAssembly>();
        }
    }
}
