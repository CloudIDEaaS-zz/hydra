using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    public static class DynamicObjects
    {
        public static DynamicObject ToDynamic(this IDictionary<string, object> dictionary)
        {
            return new DynamicObjects.Dynamic(dictionary);
        }

        public class Dynamic : DynamicObject
        {
            private IDictionary<string, object> dictionary;
            private object obj;

            public Dynamic(object obj)
            {
                this.obj = obj;
            }

            public Dynamic(IDictionary<string, object> dictionary)
            {
                this.dictionary = dictionary;
            }

            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                return base.TryConvert(binder, out result);
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                string name = binder.Name;

                return dictionary.TryGetValue(name, out result);
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                dictionary[binder.Name] = value;
                return true;
            }
        }
    }
}
