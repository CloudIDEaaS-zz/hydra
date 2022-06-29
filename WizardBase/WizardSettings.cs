using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace WizardBase
{
    public abstract class WizardSettingsBase : IEnumerable<KeyValuePair<string, object>>
    {
        public abstract IEnumerator<KeyValuePair<string, object>> GetEnumerator();

        public object this[string key]
        {
            get
            {
                return this.Where(p => p.Key == key).Select(p => p.Value).SingleOrDefault();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
