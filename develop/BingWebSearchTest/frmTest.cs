using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Reflection;
using Utils;
using System.Windows.Forms;
using Leadtools;
using Leadtools.Codecs;
using System.Drawing.Imaging;
using Leadtools.ImageProcessing.Color;

namespace BingWebSearchTest
{
    public partial class frmTest : Form
    {
        private static string _subscriptionKey = "d3fedd86c7c84fb39c36fa19898e0f68";
        private static string _baseUri = "https://api.bing.microsoft.com/v7.0/images/search";

        // The user's search string.

        // To page through the images, you'll need the next offset that Bing returns.

        private static long _nextOffset = 0;

        // To get additional insights about the image, you'll need the image's
        // insights token (see Visual Search API).

        private static string _insightsToken = null;

        // Bing uses the X-MSEdge-ClientID header to provide users with consistent
        // behavior across Bing API calls. See the reference documentation
        // for usage.

        private static string _clientIdHeader = null;
        private const string QUERY_PARAMETER = "?q=";  // Required
        private const string MKT_PARAMETER = "&mkt=";  // Strongly suggested
        private const string COUNT_PARAMETER = "&count=";
        private const string OFFSET_PARAMETER = "&offset=";
        private const string ID_PARAMETER = "&id=";
        private const string SAFE_SEARCH_PARAMETER = "&safeSearch=";
        private const string ASPECT_PARAMETER = "&aspect=";
        private const string COLOR_PARAMETER = "&color=";
        private const string FRESHNESS_PARAMETER = "&freshness=";
        private const string HEIGHT_PARAMETER = "&height=";
        private const string WIDTH_PARAMETER = "&width=";
        private const string IMAGE_CONTENT_PARAMETER = "&imageContent=";
        private const string IMAGE_TYPE_PARAMETER = "&imageType=";
        private const string LICENSE_PARAMETER = "&license=";
        private const string MAX_FILE_SIZE_PARAMETER = "&maxFileSize=";
        private const string MIN_FILE_SIZE_PARAMETER = "&minFileSize=";
        private const string MAX_HEIGHT_PARAMETER = "&maxHeight=";
        private const string MIN_HEIGHT_PARAMETER = "&minHeight=";
        private const string MAX_WIDTH_PARAMETER = "&maxWidth=";
        private const string MIN_WIDTH_PARAMETER = "&minWidth=";
        private const string SIZE_PARAMETER = "&size=";
        private IManagedLockObject lockObject;
        private List<ImageResult> imageResults;
        private Queue<ImageResult> displayQueue;
        private string searchString;
        private string tempFolder;

        public frmTest()
        {
            lockObject = LockManager.CreateObject();
            imageResults = new List<ImageResult>();
            displayQueue = new Queue<ImageResult>();

            InitializeComponent();
        }

        private void frmTest_Load(object sender, EventArgs e)
        {
            tempFolder = Path.Combine(Path.GetTempPath(), "BingWebSearch");

            this.ShowInSecondaryMonitor();
        }

        private void cmdSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Search();
            }
        }

        private void Search()
        {
            searchString = txtSearch.Text;

            CleanTempFolders();

            cmdSearch.Enabled = false;
            imagePanel.Controls.Clear();

            timer.Start();

            RunAsync().Wait();

            using (lockObject.Lock())
            {
                while (displayQueue.Count > 0)
                {
                    Application.DoEvents();
                }
            }

            timer.Stop();
            cmdSearch.Enabled = true;
        }

        private void CleanTempFolders()
        {
            var tempPath = Path.GetTempPath();
            var directory = new DirectoryInfo(tempFolder);

            if (!directory.Exists)
            {
                return;
            }

            directory.ForceDeleteFiles();

            directory = new DirectoryInfo(tempPath);

            foreach (var file in directory.GetFiles())
            {
                if (file.Name.RegexIsMatch(@"(?im)^TempImage[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]"))
                {
                    try
                    {
                        if (file.IsReadOnly)
                        {
                            file.MakeWritable();
                        }

                        file.Delete();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            using (lockObject.Lock())
            {
                while (displayQueue.Count > 0)
                {
                    var imageResult = displayQueue.Dequeue();
                    var image = Bitmap.FromFile(imageResult.FileName);

                    var pictureBox = new PictureBox
                    {
                        Image = image,
                        Tag = imageResult,
                        Width = 200,
                        Height = 200,
                        SizeMode = PictureBoxSizeMode.Zoom
                    };

                    imagePanel.Controls.Add(pictureBox);
                }
            }
        }

        private async Task RunAsync()
        {
            try
            {
                // Remember to encode the q query parameter.

                var queryString = QUERY_PARAMETER + Uri.EscapeDataString(searchString);
                queryString += MKT_PARAMETER + "en-us";
                queryString += COUNT_PARAMETER + "50";

                HttpResponseMessage response = await MakeRequestAsync(queryString).ConfigureAwait(false);

                _clientIdHeader = response.Headers.GetValues("X-MSEdge-ClientID").FirstOrDefault();

                // This example uses dictionaries instead of objects to access the response data.

                var contentString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                Dictionary<string, object> searchResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentString);

                if (response.IsSuccessStatusCode)
                {
                    SaveImages(searchResponse);
                }
                else
                {
                    PrintErrors(response.Headers, searchResponse);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("\nPress ENTER to exit...");
            Console.ReadLine();
        }

        private void SaveImages(Dictionary<string, object> response)
        {
            Console.WriteLine("The response contains the following images:\n");

            // This example prints the first page of images but if you want to page
            // through the images, you need to capture the next offset that Bing returns.

            _nextOffset = (long)response["nextOffset"];

            var images = response["value"] as Newtonsoft.Json.Linq.JToken;

            foreach (Newtonsoft.Json.Linq.JToken image in images)
            {
                var thumbnailUrl = (string) image["thumbnailUrl"];
                var encodingFormat = (string)image["encodingFormat"];
                string fileName;
                var imageResult = new ImageResult
                {
                    ThumbnailUrl = thumbnailUrl,
                    EncodingFormat = encodingFormat,
                    ContentUrl = (string)image["contentUrl"],
                    HostPageDomainFriendlyName = (string)image["hostPageDomainFriendlyName"],
                    HostPageDisplayUrl = (string)image["hostPageDisplayUrl"],
                    AccentColor = ColorTranslator.FromHtml("#" + (string)image["accentColor"])
                };

                try
                {
                    if (DownloadFile(new Uri(thumbnailUrl), "." + encodingFormat, out fileName))
                    {
                        imageResult.FileName = fileName;
                    }

                    imageResult.FileName = fileName;
                }
                catch (Exception ex)
                {
                    imageResult.DownloadException = ex;
                }

                using (lockObject.Lock())
                {
                    imageResults.Add(imageResult);
                    displayQueue.Enqueue(imageResult);
                }

                // If you want to get additional insights about the image, capture the
                // image token that you use when calling Visual Search API.

                _insightsToken = (string)image["imageInsightsToken"];
            }
        }

        public string GetTempPrefix()
        {
            var tempFilePrefix = Path.Combine(Path.GetTempPath(), "BingWebSearch", Guid.NewGuid().ToString());

            return tempFilePrefix;
        }

        private bool DownloadFile(Uri uri, string extension, out string fileName)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            Stream responseStream;
            var tempFilePrefix = GetTempPrefix();

            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(uri);
                request.Method = "GET";
                request.Headers.Clear();

                request.UserAgent = Assembly.GetExecutingAssembly().FullName;

                response = (HttpWebResponse)request.GetResponse();

                responseStream = response.GetResponseStream();

                using (responseStream)
                {
                    FileInfo file;
                    var bytes = responseStream.ReadToEnd();

                    fileName = tempFilePrefix + extension;
                    file = new FileInfo(fileName);

                    if (!file.Directory.Exists)
                    {
                        file.Directory.Create();
                    }

                    using (var fileStream = File.OpenWrite(fileName))
                    using (var writer = new BinaryWriter(fileStream))
                    {
                        writer.Write(bytes);
                        writer.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                fileName = null;
                return false;
            }

            return true;
        }

        private async Task<HttpResponseMessage> MakeRequestAsync(string queryString)
        {
            var client = new HttpClient();

            // Request headers. The subscription key is the only required header but you should
            // include User-Agent (especially for mobile), X-MSEdge-ClientID, X-Search-Location
            // and X-MSEdge-ClientIP (especially for local aware queries).

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

            return (await client.GetAsync(_baseUri + queryString).ConfigureAwait(false));
        }

        private void PrintErrors(HttpResponseHeaders headers, Dictionary<String, object> response)
        {
            Console.WriteLine("The response contains the following errors:\n");

            object value;

            if (response.TryGetValue("error", out value))  // typically 401, 403
            {
                PrintError(response["error"] as Newtonsoft.Json.Linq.JToken);
            }
            else if (response.TryGetValue("errors", out value))
            {
                // Bing API error

                foreach (Newtonsoft.Json.Linq.JToken error in response["errors"] as Newtonsoft.Json.Linq.JToken)
                {
                    PrintError(error);
                }

                // Included only when HTTP status code is 400; not included with 401 or 403.

                IEnumerable<string> headerValues;
                if (headers.TryGetValues("BingAPIs-TraceId", out headerValues))
                {
                    Console.WriteLine("\nTrace ID: " + headerValues.FirstOrDefault());
                }
            }

        }

        private void PrintError(Newtonsoft.Json.Linq.JToken error)
        {
            string value = null;

            Console.WriteLine("Code: " + error["code"]);
            Console.WriteLine("Message: " + error["message"]);

            if ((value = (string)error["parameter"]) != null)
            {
                Console.WriteLine("Parameter: " + value);
            }

            if ((value = (string)error["value"]) != null)
            {
                Console.WriteLine("Value: " + value);
            }
        }

        private void cmdChangeColor_Click(object sender, EventArgs e)
        {
            foreach (var pictureBox in imagePanel.Controls.Cast<PictureBox>())
            {
                var image = (Bitmap)pictureBox.Image;
                var imageResult = (ImageResult)pictureBox.Tag;
                var rasterImage = image.ToRasterImage();
                var dominantColors = image.GetDominantColors();
                var nonGrays = dominantColors.FilterGrays().ToList();
                Color primaryDominantColor;

                if (nonGrays.Count > 0)
                {
                    primaryDominantColor = dominantColors.FilterGrays().First();
                }
                else
                {
                    primaryDominantColor = dominantColors.First();
                }

                //rasterImage.ChangeColorIntensityBalance(0, 15, 0);
                rasterImage.ReplaceColor(primaryDominantColor, Color.Green);

                pictureBox.ChangeImage(rasterImage, RasterImageFormat.Bmp);
            }
        }
    }
}
