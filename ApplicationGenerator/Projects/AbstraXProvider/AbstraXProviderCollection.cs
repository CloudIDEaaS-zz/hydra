using System;
using System.Configuration.Provider;

namespace AbstraX
{
    public class AbstraXProviderCollection : ProviderCollection
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("You must supply a provider reference");

            if (!(provider is AbstraXProviderBase))
                throw new ArgumentException("The supplied provider type must derive from AbstraXProvider");

            base.Add(provider);
        }

        new public AbstraXProviderBase this[string name]
        {
            get { return (AbstraXProviderBase)base[name]; }
        }

        public void CopyTo(AbstraXProviderBase[] array, int index)
        {
            base.CopyTo(array, index);
        }
    }
}
