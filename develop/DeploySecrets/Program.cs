using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using Utils;

namespace DeploySecrets
{
    // Refer to:
    //  https://github.com/Azure/azure-sdk-for-net/blob/Azure.Security.KeyVault.Secrets_4.2.0/sdk/keyvault/Azure.Security.KeyVault.Secrets/README.md
    //  https://stackoverflow.com/questions/38558844/jcontainer-jobject-jtoken-and-linq-confusion

    public class Program
    {
        private static SecretClient client;

        public static void Main(string[] args)
        {
            var json = File.ReadAllText(args[0]);
            var jsonSecrets = (JObject) JsonExtensions.ReadJson(json);
            var keyVaultUrl = "https://hydracloudservices-kv.vault.azure.net/";
            Action<string, JToken> recurse = null;

            client = new SecretClient(vaultUri: new Uri(keyVaultUrl), credential: new DefaultAzureCredential());

            recurse = new Action<string, JToken>((parentKey, token) =>
            {
                if (token.Type.IsOneOf(JTokenType.Array))
                {
                    var jArray = (JArray)token;

                    StoreSecret(parentKey, token);
                }
                else if (token.Type.IsOneOf(JTokenType.Object))
                {
                    var jObject = (JObject)token;

                    foreach (var keyValuePair in jObject)
                    {
                        recurse(parentKey + "--" + keyValuePair.Key, keyValuePair.Value);
                    }
                }
                else
                {
                    StoreSecret(parentKey, token);
                }
            });

            foreach (var keyValuePair in jsonSecrets)
            {
                recurse(keyValuePair.Key, keyValuePair.Value);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void StoreSecret(string parentKey, JToken token)
        {
            KeyVaultSecret secret = null;

            try
            {
                Console.WriteLine("Checking existence of secret for {0}", parentKey);

                secret = client.GetSecret(parentKey).Value;

                Console.WriteLine("Secret exists");
            }
            catch (Azure.RequestFailedException ex)
            {
                if (ex.ErrorCode == "SecretNotFound")
                {
                    Console.WriteLine("Secret not found");
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (secret == null)
            {
                Console.WriteLine("Setting secret for {0}", parentKey);

                try
                {
                    secret = client.SetSecret(parentKey, token.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
