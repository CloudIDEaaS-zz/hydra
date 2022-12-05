using AbstraX.DataAnnotations;
using AbstraX.Generators.Base;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Generators.Pages.SlidingTabPage
{
    [DebuggerDisplay(" { Title } ")]
    public class SlidingTab : HandlerObjectBase
    {
        public string Root { get; }
        public string Title { get; }

        public SlidingTab(IBase baseObject, IGeneratorConfiguration generatorConfiguration) : base(baseObject, generatorConfiguration)
        {
            var navigationAttribute = baseObject.GetFacetAttribute<UINavigationAttribute>();
            var hasComponentAttribute = baseObject.HasFacetAttribute<UIAttribute>();
            var tabName = baseObject.GetDisplayName();
            string name;

            if (!hasComponentAttribute && baseObject.Kind == DefinitionKind.ComplexSetProperty)
            {
                var parentBase = (IParentBase)baseObject;
                var childElement = parentBase.ChildElements.Single();

                name = childElement.GetNavigationName() + "Page";
            }
            else
            {
                name = baseObject.GetNavigationName() + "Page";
            }

            this.Root = name;
            this.Title = tabName;
        }

        public string TranslationKey
        {
            get
            {
                return this.CreateTranslationKey(() => this.Title);
            }
        }
    }
}
