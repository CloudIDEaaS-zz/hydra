using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace CognitiveServices.Services
{
    public static class GeneratePhotoService
    {
        private static string apiKey = "ZsikKfZD_Rbttlb6FrZZ6g";
        private static string urlPattern = "https://api.generated.photos/api/v1/faces?api_key={0}&per_page=1&order_by=random";
        private static bool TEST_MODE = true;

        public static GeneratePhotoResponse FetchRandomPhoto()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(string.Format(urlPattern, apiKey))
            };

            if (!TEST_MODE)
            {
                using (var response = client.SendAsync(request).Result)
                {
                    response.EnsureSuccessStatusCode();

                    var body = response.Content.ReadAsStringAsync();
                    var result = JsonExtensions.ReadJson<GeneratePhotoResponse>(body.Result);
                    var imageUrl = (string)result.faces.First().urls.Last().Value;
                    var image = DownloadImage(imageUrl);

                    result.Image = image;
                    result.Url = imageUrl;

                    return result;
                }
            }
            else
            {
                var result = new GeneratePhotoResponse();
                var imageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRRc5hKlhPp3Ygl3ZLiwQkTGRUPUBHw-ghuN3K4X6sfkz0qNTkwAeUAgPu5x7NjFzvG5GI&usqp=CAU";
                var image = DownloadImage(imageUrl);

                result.Image = image;
                result.Url = imageUrl;

                return result;
            }
        }

        private static Image DownloadImage(string url)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            Stream responseStream;

            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                request.Headers.Clear();

                request.UserAgent = Assembly.GetExecutingAssembly().FullName;

                response = (HttpWebResponse)request.GetResponse();

                responseStream = response.GetResponseStream();

                using (responseStream)
                {
                    Bitmap image;

                    try
                    {
                        image = (Bitmap)Bitmap.FromStream(responseStream);

                        return image;
                    }
                    catch
                    {
                        DebugUtils.Break();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugUtils.Break();
                throw;
            }
        }
    }
}
