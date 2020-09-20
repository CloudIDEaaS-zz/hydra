using ApplicationGenerator.Data;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace ApplicationGenerator.Data
{
    public class AbstraXProviderDataContext : DbContext
    {
        public IAbstraXProviderServices AbstraXProviderServices { get; set; }

        public AbstraXProviderDataContext(DbContextOptions options, IAbstraXProviderServices abstraXProviderServices) : base(options)
        {
            this.AbstraXProviderServices = abstraXProviderServices;
        }
    }
}
