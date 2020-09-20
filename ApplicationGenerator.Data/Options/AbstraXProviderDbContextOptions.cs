using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ApplicationGenerator.Data
{
    public class AbstraXProviderDbContextOptions : IDbContextOptions
    {
        public IEnumerable<IDbContextOptionsExtension> Extensions
        {
            get
            {
                return null;
            }
        }

        TExtension IDbContextOptions.FindExtension<TExtension>()
        {
            return null;
        }
    }
}