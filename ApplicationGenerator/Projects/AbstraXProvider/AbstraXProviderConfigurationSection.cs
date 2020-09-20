using System;
using System.Configuration;

namespace AbstraX
{
    public class AbstraXConfigurationSection : ConfigurationSection
    {
        public AbstraXConfigurationSection()
        {
        }

        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers 
        {
            get 
            {
                return (ProviderSettingsCollection)base["providers"];
            }
        }

        [ConfigurationProperty("defaultProvider", DefaultValue = "DefaultAbstraXProvider")]
        [StringValidator(MinLength = 1)]
        public string DefaultProvider 
        {
            get 
            {
                return (string)base["defaultProvider"];
            }

            set 
            {
                base["defaultProvider"] = value;
            }
        }
    }
}