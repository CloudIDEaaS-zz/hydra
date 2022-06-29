using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator
{
    public class ReadOnlyList<T> : List<T>, IReadOnlyList<T>
    {
        public ReadOnlyList(IEnumerable<T> collection) : base(collection)
        {
        }
    }
}
