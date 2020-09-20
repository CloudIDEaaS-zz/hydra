using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;

namespace AbstraX
{
    public static class AbstraXMainEntryPoint
    {
        private static bool isInitialized = false;
        private static Exception initializationException;
        private static AbstraXProviderBase defaultProvider;
        private static AbstraXProviderCollection providerCollection;
        private static object initializationLock = new object();

        static AbstraXMainEntryPoint()
        {
            Initialize();
        }

        private static void Initialize()
        {
            try
            {
                //Get the feature's configuration info

                AbstraXConfigurationSection configSection = (AbstraXConfigurationSection) ConfigurationManager.GetSection("AbstraX");

                if (configSection.DefaultProvider == null || configSection.Providers == null || configSection.Providers.Count < 1)
                {
                    throw new ProviderException("The feature requires that you specify a default " +
                                                "feature provider as well as at least one " +
                                                "provider definition.");
                }

                //Instantiate the feature's providers

                providerCollection = new AbstraXProviderCollection();
                ProvidersHelper.InstantiateProviders(configSection.Providers, providerCollection, typeof(AbstraXProviderBase));

                providerCollection.SetReadOnly();
                defaultProvider = providerCollection[configSection.DefaultProvider];

                if (defaultProvider == null)
                {
                    throw new ConfigurationErrorsException(
                        "The default feature provider was not specified.",
                        configSection.ElementInformation.Properties["defaultProvider"].Source,
                        configSection.ElementInformation.Properties["defaultProvider"].LineNumber);
                }
            }
            catch (Exception ex)
            {
                initializationException = ex;
                isInitialized = true;
                throw ex;
            }

            //error-free initialization

            isInitialized = true; 
        }

        public static AbstraXProviderBase Provider
        {
            get 
            {
                return defaultProvider; 
            }
        }

        public static AbstraXProviderCollection Providers
        {
            get
            {
                return providerCollection;
            }
        }
    }
}
