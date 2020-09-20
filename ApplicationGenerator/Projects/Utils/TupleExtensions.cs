using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace Utils
{
    public static class TupleExtensions
    {
        public static IEnumerable<object> GetTupleValues<T>(this T tuple) where T : IStructuralEquatable, IStructuralComparable, IComparable
        {
            var values = new List<object>();
            var tupleType = tuple.GetType();
            var type = Type.GetType("System.ITuple");
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty;
            var property = tupleType.GetProperty("Item1", flags);
            var x = 2;

            if (!tupleType.Implements(type))
            {
                e.Throw<ArgumentException>("Value of type {0} is not a Tuple", tupleType.Name);
                return null;
            }

            while (property != null)
            {
                var value = property.GetGetMethod().Invoke(tuple, null);

                values.Add(value);

                property = tupleType.GetProperty(string.Format("Item{0}", x), flags);
                x++;
            }

            return values;
        }
    }
}
