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
    [UILoadKindAugmenter(DataAnnotations.UILoadKind.MainPage)]
    public class MainPageAugmenter : BaseNonPageFacetHandler, IUILoadKindAugmenterHandler
    {
        public List<Facet> AdditionalFacets { get; private set; }

        public override FacetHandlerLayer FacetHandlerLayer => FacetHandlerLayer.Client;

        public override bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var aboutAttribute = new UIAttribute("/app/main/about", UIKind.AboutPage);
            var aboutFacet = new Facet(aboutAttribute);

            this.AdditionalFacets = new List<Facet>();

            this.AdditionalFacets.Add(aboutFacet);

            return true;
        }
    }
}
