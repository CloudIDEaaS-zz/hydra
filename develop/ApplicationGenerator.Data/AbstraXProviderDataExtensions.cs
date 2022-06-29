using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Utils;

namespace ApplicationGenerator.Data
{
    public static class AbstraXProviderDataExtensions
    {
        public static void UseAbstraXProviderData(this DbContextOptionsBuilder optionsBuilder)
        {
            var extension = (AbstraXProviderContextOptionsExtension)GetOrCreateExtension(optionsBuilder);
            var infrastructureOptionsBuilder = (IDbContextOptionsBuilderInfrastructure)optionsBuilder;

            infrastructureOptionsBuilder.AddOrUpdateExtension(extension);
        }

        private static AbstraXProviderContextOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        {
            return optionsBuilder.Options.FindExtension<AbstraXProviderContextOptionsExtension>() ?? new AbstraXProviderContextOptionsExtension();
        }
    }
}
