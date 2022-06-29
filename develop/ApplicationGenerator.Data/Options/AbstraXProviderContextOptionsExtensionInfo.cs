using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Utils;

namespace ApplicationGenerator.Data
{
    public partial class AbstraXProviderContextOptionsExtension
    {
        private class AbstraXProviderContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
        {
            public override bool IsDatabaseProvider => true;

            public AbstraXProviderContextOptionsExtensionInfo(AbstraXProviderContextOptionsExtension extension) : base(extension)
            {
            }

            public override string LogFragment
            {
                get
                {
                    return "AbstraXProviderOptions";
                }
            }

            public override long GetServiceProviderHashCode()
            {
                return typeof(AbstraXProviderContextOptionsExtensionInfo).FullName.GetHashCode();
            }

            public override void PopulateDebugInfo([NotNull] IDictionary<string, string> debugInfo)
            {
            }
        }
    }
}