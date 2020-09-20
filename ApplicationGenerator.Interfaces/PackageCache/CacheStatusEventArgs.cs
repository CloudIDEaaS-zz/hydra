using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.PackageCache
{
    public enum IncrementKind
    {
        IncrementTotal,
        IncrementAll,
        Total,
        TotalRemaining,
        Requested,
        RequestedRemaining
    }

    public delegate void UpdateCacheStatusEventHandler(object sender, CacheStatusEventArgs e);

    public class CacheStatusEventArgs
    {
        public IncrementKind IncrementKind { get; }
        public int Increment { get; }

        public CacheStatusEventArgs(IncrementKind incrementKind, int increment = 1)
        {
            this.IncrementKind = incrementKind;
            this.Increment = increment;
        }
    }
}
