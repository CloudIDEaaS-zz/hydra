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

namespace AbstraX.Generators.Pages.NavigationGridPage
{
    [DebuggerDisplay(" { Title } ")]
    public class ManagedList : HandlerObjectBase
    {
        public string Title { get; }
        public string Name { get; }

        public ManagedList(IBase baseObject, IGeneratorConfiguration generatorConfiguration) : base(baseObject, generatorConfiguration)
        {
            var title = baseObject.GetDisplayName();
            var name = baseObject.GetNavigationName();

            this.Title = title;
            this.Name = name;
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
