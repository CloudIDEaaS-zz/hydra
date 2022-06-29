using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Utils;

namespace ApplicationGenerator.Data
{
    public class AbstraXProviderLoggingOptions : ILoggingOptions
    {
        public bool IsSensitiveDataLoggingWarned { get; set; }
        public bool IsSensitiveDataLoggingEnabled => true;

        public WarningsConfiguration WarningsConfiguration
        {
            get
            {
                return new WarningsConfiguration();
            }
        }

        public void Initialize(IDbContextOptions options)
        {
        }

        public void Validate(IDbContextOptions options)
        {
        }
    }
}