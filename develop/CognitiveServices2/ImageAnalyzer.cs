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
using System.Drawing;
using System.Drawing.Imaging;
using Utils;

namespace CognitiveServices
{
    public class ImageAnalyzer
    {
        const string subscriptionKey = "ddb49b1575a3444b973d64f928bacb4d";
        const string endpoint = "https://hydracomputervision.cognitiveservices.azure.com/";
        private ComputerVisionClient client;

        public ImageAnalyzer()
        {
            client = Authenticate(endpoint, subscriptionKey);
        }

        private ComputerVisionClient Authenticate(string endpoint, string key)
        {
            client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
            
            return client;
        }

        public async Task<ImageAnalysis> AnalyzeImageUrl(string imageUrl)
        {
            var features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Tags,
                VisualFeatureTypes.Categories,
                VisualFeatureTypes.Adult,
                VisualFeatureTypes.Brands,
                VisualFeatureTypes.Description,
                VisualFeatureTypes.Objects,
                VisualFeatureTypes.Faces,
                VisualFeatureTypes.Color
            };

            var results = await client.AnalyzeImageAsync(imageUrl, features);
            return results;
        }

        public Task<ImageAnalysis> AnalyzeImage(Image image)
        {
            ImageAnalysis results = null;

            var task = new Task<ImageAnalysis>(() =>
            {
                var resetEvent = new ManualResetEvent(false);
                Thread thread;

                thread = new Thread(async () =>
                {
                    var features = new List<VisualFeatureTypes?>()
                    {
                        VisualFeatureTypes.Tags,
                        VisualFeatureTypes.Categories,
                        VisualFeatureTypes.Adult,
                        VisualFeatureTypes.Brands,
                        VisualFeatureTypes.Description,
                        VisualFeatureTypes.Objects,
                        VisualFeatureTypes.Faces,
                        VisualFeatureTypes.Color
                    };

                    var details = new List<Details?>()
                    {
                        Details.Celebrities,
                        Details.Landmarks
                    };

                    using (var stream = new MemoryStream())
                    {
                        image.Save(stream, ImageFormat.Jpeg);
                        stream.Rewind();

                        results = await client.AnalyzeImageInStreamAsync(stream, features, details);
                    }

                    resetEvent.Set();
                });
                    

                thread.SetApartmentState(ApartmentState.MTA);
                thread.Start();

                resetEvent.WaitOne();

                return results;
            });

            task.Start();

            return task;
        }
    }
}
