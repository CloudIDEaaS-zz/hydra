using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ApplicationGenerator.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;

    namespace ApplicationGenerator.Data
    {
        public class AbstraxProviderDbContextOptions : DbContextOptions
        {
            private static Dictionary<Type, IDbContextOptionsExtension> extensions = new Dictionary<Type, IDbContextOptionsExtension>();

            public AbstraxProviderDbContextOptions() : base(AbstraxProviderDbContextOptions.GetExtensions(new Dictionary<Type, IDbContextOptionsExtension>()))
            {
            }

            private static ReadOnlyDictionary<Type, IDbContextOptionsExtension> GetExtensions(Dictionary<Type, IDbContextOptionsExtension> dictionary)
            {
                var optionsType = typeof(AbstraXProviderContext);

                if (!extensions.ContainsKey(optionsType))
                {
                    extensions.Add(typeof(AbstraXProviderContext), new AbstraXProviderContextOptionsExtension());
                }

                return new ReadOnlyDictionary<Type, IDbContextOptionsExtension>(extensions);
            }

            public override Type ContextType
            {
                get
                {
                    return typeof(AbstraXProviderContext);
                }
            }

            public override DbContextOptions WithExtension<TExtension>(TExtension extension)
            {
                extensions.Add(typeof(TExtension), extension);

                return new AbstraxProviderDbContextOptions();
            }
        }
    }
}