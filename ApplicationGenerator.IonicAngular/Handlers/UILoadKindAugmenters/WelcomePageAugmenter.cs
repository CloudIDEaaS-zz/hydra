using AbstraX.DataAnnotations;
using AbstraX.Handlers.FacetHandlers;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.UILoadKindAugmenters
{
    [UILoadKindAugmenter(DataAnnotations.UILoadKind.HomePage)]
    public class WelcomePageAugmenter : BaseNonPageFacetHandler, IUILoadKindAugmenterHandler
    {
        public List<Facet> AdditionalFacets { get; private set; }

        public override FacetHandlerLayer FacetHandlerLayer => FacetHandlerLayer.Client;

        public override bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var licenseAttribute = new UIAttribute("license", UIKind.LicensePage);
            var licenseFacet = new Facet(licenseAttribute);
            var supportAttribute = new UIAttribute("support", UIKind.SupportPage);
            var supportFacet = new Facet(supportAttribute);

            this.AdditionalFacets = new List<Facet>();

            this.AdditionalFacets.Add(licenseFacet);
            this.AdditionalFacets.Add(supportFacet);

            return true;
        }
    }
}
