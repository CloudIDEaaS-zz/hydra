using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Linq;
using Utils;
using Un4seen.Bass;
using Newtonsoft.Json.Schema;

namespace CognitiveServices
{
    public class Program
    {
        // Add your Computer Vision subscription key and endpoint
        static string subscriptionKey = "ddb49b1575a3444b973d64f928bacb4d";
        static string endpoint = "https://hydracomputervision.cognitiveservices.azure.com/";

        // URL image used for analyzing an image (image of puppy)
        private const string ANALYZE_URL_IMAGE = "https://www.westend61.de/images/0001026023pw/closed-up-shot-of-a-young-pretty-girl-AURF01299.jpg";

        public static void Main(string[] args)
        {
            CreateEffectsSchema();
            //Test();
        }

        private static void CreateEffectsSchema()
        {
            var hydraSolutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
            var effectSchemaPath = Path.Combine(hydraSolutionPath, @"CognitiveServices\EffectSchema\Schema.json");
            var names = EnumUtils.GetNames<BASSFXType>();
            var assembly = typeof(BASSFXType).Assembly;
            var jsonSchema = new JsonSchema
            {
                Id = "https://schemas.cloudideaas.com/soundeffects.schema.json",
            };

            foreach (var name in names)
            {
                var typeName = name.RemoveStart("BASS_FX_");
                var effectType = assembly.GetType("Un4seen.Bass.BASS_" + typeName);
                //var parms = effectType.GetConstructors().SelectMany(c => c.GetParameters()).DistinctBy(p => p.Name);
                //var jObject = new JObject().AddThen(typeName, 


            }
        }

        private static void Test()
        {
            Console.WriteLine("Azure Cognitive Services Computer Vision - .NET quickstart example");
            Console.WriteLine();

            // Create a client
            ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);

            // Analyze an image to get features and other properties.
            AnalyzeImageUrl(client, ANALYZE_URL_IMAGE).Wait();
        }

        /*
         * AUTHENTICATE
         * Creates a Computer Vision client used by each example.
         */
        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        public static async Task AnalyzeImageUrl(ComputerVisionClient client, string imageUrl)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("ANALYZE IMAGE - URL");
            Console.WriteLine();

            // Creating a list that defines the features to be extracted from the image. 

            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Tags
            };

            Console.WriteLine($"Analyzing the image {Path.GetFileName(imageUrl)}...");
            Console.WriteLine();
            // Analyze the URL image 
            ImageAnalysis results = await client.AnalyzeImageAsync(imageUrl, visualFeatures: features);

            // Image tags and their confidence score
            Console.WriteLine("Tags:");
            foreach (var tag in results.Tags)
            {
                Console.WriteLine($"{tag.Name} {tag.Confidence}");
            }

            Console.WriteLine();
        }
    }
}