using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApplicationGenerator.State;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Utils;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ApplicationGenerator.Data
{
    public partial class AbstraXProviderDataProvider : IDbSetFinder
    {
        public IReadOnlyList<DbSetProperty> FindSets(Type contextType)
        {
            return new ReadOnlyList<DbSetProperty>(dbSets);
        }
    }
}