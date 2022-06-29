using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace CognitiveServices.Services
{
    public static class GeneratePhotoService
    {
        private static string apiKey = "ZsikKfZD_Rbttlb6FrZZ6g";
        private static string url = "https://api.generated.photos/api/v1/faces?per_page=1&order_by=random";
        private static bool TEST_MODE = false;

        public static GeneratePhotoResponse FetchRandomPhoto(params KeyValuePair<string, string>[] parms)
        {
            HttpClient client;
            HttpRequestMessage request;

            foreach (var parm in parms)
            {
                url += string.Format("&{0}={1}", parm.Key, parm.Value); 
            }

            client = new HttpClient();
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("API-Key", apiKey);

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

        public static async Task<GeneratePhotoResponse> FetchRandomPhotoAsync(params KeyValuePair<string, string>[] parms)
        {
            HttpClient client;
            HttpRequestMessage request;

            foreach (var parm in parms)
            {
                url += string.Format("&{0}={1}", parm.Key, parm.Value);
            }

            client = new HttpClient();
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("API-Key", apiKey);

            if (!TEST_MODE)
            {
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();

                    var body = await response.Content.ReadAsStringAsync();
                    var result = JsonExtensions.ReadJson<GeneratePhotoResponse>(body);
                    var imageUrl = (string)result.faces.First().urls.Last().Value;
                    var image = await DownloadImageAsync(imageUrl);

                    result.Image = image;
                    result.Url = imageUrl;

                    return result;
                }
            }
            else
            {
                var result = new GeneratePhotoResponse();
                var imageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRRc5hKlhPp3Ygl3ZLiwQkTGRUPUBHw-ghuN3K4X6sfkz0qNTkwAeUAgPu5x7NjFzvG5GI&usqp=CAU";
                var image = await DownloadImageAsync(imageUrl);

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

        private async static Task<Image> DownloadImageAsync(string url)
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

                response = (HttpWebResponse) await request.GetResponseAsync();

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
