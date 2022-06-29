using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Interface for iui load kind augmenter handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 2/27/2021. </remarks>

    public interface IUILoadKindAugmenterHandler : IFacetHandler
    {
        /// <summary>   Gets the additional facets. </summary>
        ///
        /// <value> The additional facets. </value>

        List<Facet> AdditionalFacets { get; }
    }
}
