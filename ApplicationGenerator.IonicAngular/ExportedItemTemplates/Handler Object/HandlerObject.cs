using AbstraX.DataAnnotations;
using AbstraX.Generators.Base;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $rootnamespace$
{
    [DebuggerDisplay(" { Title } ")]
    public class $safeitemname$ : HandlerObjectBase
    {
        public string Name { get; }
        public string Title { get; }

        public $safeitemname$(IBase baseObject, IGeneratorConfiguration generatorConfiguration) : base(baseObject, generatorConfiguration)
        {
            var displayName = baseObject.GetDisplayName();
            var name = baseObject.GetNavigationName();

            this.Title = displayName;
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
