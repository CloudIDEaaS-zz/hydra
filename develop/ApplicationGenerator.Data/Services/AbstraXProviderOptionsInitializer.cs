using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.Data
{
    public class AbstraXProviderOptionsInitializer : ISingletonOptionsInitializer
    {
        public void EnsureInitialized(IServiceProvider serviceProvider, IDbContextOptions options)
        {
        }
    }
}
