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

namespace AbstraX
{
    [DebuggerDisplay(" { Title } ")]
    public class Response : HandlerObjectBase
    {
        public string Name { get; }
        public string Title { get; }
        public bool IsTextIdentifier { get; }
        public bool IsKey { get; }
        public GridColumnKind GridColumnKind { get; }

        public Response(IBase baseObject, IGeneratorConfiguration generatorConfiguration) : base(baseObject, generatorConfiguration)
        {
            var gridColumnAttribute = baseObject.GetFacetAttribute<GridColumnAttribute>();
            var displayName = baseObject.GetDisplayName();
            var name = baseObject.GetNavigationName();

            this.Title = displayName;
            this.Name = name;
            this.GridColumnKind = gridColumnAttribute.GridColumnKind;
            this.IsTextIdentifier = gridColumnAttribute.IsTextIdentifier;
            this.IsKey = baseObject.HasFacetAttribute<KeyAttribute>();
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
