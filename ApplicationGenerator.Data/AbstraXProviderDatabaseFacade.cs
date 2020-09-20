using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ApplicationGenerator.Data
{
    internal class AbstraXProviderDatabaseFacade : DatabaseFacade
    {
        public AbstraXProviderDatabaseFacade(DbContext context) : base(context)
        {
        }

        public override string ProviderName
        {
            get
            {
                return typeof(AbstraXProviderDataProvider).FullName;
            }
        }
    }
}