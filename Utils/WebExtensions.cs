using HtmlAgilityPack;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Windows.Forms;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace Utils
{
    public static class WebExtensions
    {
        public static void RegisterIEEmulation(this Assembly assembly, int preferredVersionValue = 10000)
        {
            var processName = Path.GetFileName(assembly.Location);
            var emulationKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            var versionValue = 0;

            try
            {
                versionValue = (int)emulationKey.GetValue(processName);
            }
            catch
            {
            }

            if (versionValue != preferredVersionValue)
            {
                versionValue = preferredVersionValue;
                emulationKey.SetValue(processName, versionValue, Microsoft.Win32.RegistryValueKind.DWord);
            }
        }

        public static string Unescape(this Uri uri)
        {
            return UnescapeUri(uri.AbsoluteUri);
        }

        public static string EscapeUri(string url)
        {
            return Uri.EscapeDataString(url);
        }

        public static string UnescapeUri(string url)
        {
            return Uri.UnescapeDataString(url);
        }

        public static dynamic JsonDecode(string json)
        {
            return Json.Decode(json);
        }

        public static string JsonEncode(object value)
        {
            return Json.Encode(value);
        }

        public static IEnumerable<KeyValuePair<string, T>> GetJsonMembers<T>(this object obj)
        {
            var dynamicJsonObject = (DynamicJsonObject)obj;
            
            foreach (var memberName in dynamicJsonObject.GetDynamicMemberNames())
            {
                var value = dynamicJsonObject.GetDynamicMember<T>(memberName);

                yield return new KeyValuePair<string, T>(memberName, value);
            }
        }

        public static T GetDynamicMember<T>(this object obj, string memberName)
        {
            CallSite<Func<CallSite, object, object>> callsite = null;

            try
            {
                var binder = Binder.GetMember(CSharpBinderFlags.None, memberName, obj.GetType(), new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });

                callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
            }
            catch (Exception ex)
            {
                return default(T);
            }

            return (T) callsite.Target(callsite, obj);
        }

        public static dynamic CallRestServiceGet(string url, out string rawResults, KeyValuePair<string, string>[] requestProperties, params KeyValuePair<string, object>[] parms)
        {
            HttpWebRequest request;
            string fullUrl;

            if (parms.Length > 0)
            {
                var queryParms = string.Join("&", parms.Select(p => string.Format("{0}={1}", p.Key, p.Value)));

                fullUrl = string.Format("{0}?{1}", url, queryParms);
            }
            else
            {
                fullUrl = url;
            }

            request = (HttpWebRequest) HttpWebRequest.Create(fullUrl);
            request.Method = "GET";
            request.Headers = new WebHeaderCollection();
            request.SetHeaders(requestProperties);

            using (var resp = request.GetResponse())
            {
                var results = new StreamReader(resp.GetResponseStream()).ReadToEnd();

                rawResults = results;

                return JsonDecode(results);
            }
        }

        public static dynamic CallRestServiceGet(string url, out string rawResults, params KeyValuePair<string, object>[] parms)
        {
            WebRequest request;
            string fullUrl;

            if (parms.Length > 0)
            {
                var queryParms = string.Join("&", parms.Select(p => string.Format("{0}={1}", p.Key, p.Value)));

                fullUrl = string.Format("{0}?{1}", url, queryParms);
            }
            else
            {
                fullUrl = url;
            }

            request = HttpWebRequest.Create(fullUrl);
            request.Method = "GET";

            using (var resp = request.GetResponse())
            {
                var results = new StreamReader(resp.GetResponseStream()).ReadToEnd();

                rawResults = results;

                return JsonDecode(results);
            }
        }

        public static void SetHeaders(this HttpWebRequest request, KeyValuePair<string, string>[] requestProperties)
        {
            foreach (var requestProperty in requestProperties)
            {
                switch (requestProperty.Key)
                {
                    case "Content-Type":
                        request.ContentType = requestProperty.Value;
                        break;
                    case "Accept":
                        request.Accept = requestProperty.Value;
                        break;
                    default:
                        request.Headers.Add(requestProperty.Key, requestProperty.Value);
                        break;
                }
            }
        }

        public static dynamic CallRestServiceGet(string url, KeyValuePair<string, string>[] requestProperties, params KeyValuePair<string, object>[] parms)
        {
            HttpWebRequest request;
            string fullUrl;

            if (parms.Length > 0)
            {
                var queryParms = string.Join("&", parms.Select(p => string.Format("{0}={1}", p.Key, p.Value)));

                fullUrl = string.Format("{0}?{1}", url, queryParms);
            }
            else
            {
                fullUrl = url;
            }

            request = (HttpWebRequest) HttpWebRequest.Create(fullUrl);
            request.Method = "GET";
            request.Headers = new WebHeaderCollection();
            request.SetHeaders(requestProperties);

            using (var resp = request.GetResponse())
            {
                var results = new StreamReader(resp.GetResponseStream()).ReadToEnd();

                return JsonDecode(results);
            }
        }

        public static T CallRestServiceGet<T>(string url, KeyValuePair<string, string>[] requestProperties, params KeyValuePair<string, object>[] parms)
        {
            HttpWebRequest request;
            string fullUrl;

            if (parms.Length > 0)
            {
                var queryParms = string.Join("&", parms.Select(p => string.Format("{0}={1}", p.Key, p.Value)));

                fullUrl = string.Format("{0}?{1}", url, queryParms);
            }
            else
            {
                fullUrl = url;
            }

            request = (HttpWebRequest)HttpWebRequest.Create(fullUrl);
            request.Method = "GET";
            request.Headers = new WebHeaderCollection();
            request.SetHeaders(requestProperties);

            using (var resp = request.GetResponse())
            {
                var results = new StreamReader(resp.GetResponseStream()).ReadToEnd();

                return JsonExtensions.ReadJson<T>(results);
            }
        }

        public static dynamic CallRestServicePost(string url, KeyValuePair<string, string>[] requestProperties, params KeyValuePair<string, object>[] bodyParms)
        {
            HttpWebRequest request;
            Stream requestStream;
            StreamWriter streamWriter;
            string body;

            request = (HttpWebRequest) HttpWebRequest.Create(url);
            request.Method = "POST";
            request.Headers = new WebHeaderCollection();
            request.SetHeaders(requestProperties);

            using (requestStream = request.GetRequestStream())
            {
                streamWriter = new StreamWriter(requestStream);

                body = string.Join("&", bodyParms.Select(p => string.Format("{0}={1}", p.Key, p.Value)));

                streamWriter.Write(ASCIIEncoding.ASCII.GetBytes(body));
                streamWriter.Flush();
            }

            using (var resp = request.GetResponse())
            {
                var results = new StreamReader(resp.GetResponseStream()).ReadToEnd();

                return JsonDecode(results);
            }
        }

        public static dynamic CallRestServiceGet(string url, params KeyValuePair<string, object>[] parms)
        {
            WebRequest request;
            string fullUrl;

            if (parms.Length > 0)
            {
                var queryParms = string.Join("&", parms.Select(p => string.Format("{0}={1}", p.Key, p.Value)));

                fullUrl = string.Format("{0}?{1}", url, queryParms);
            }
            else
            {
                fullUrl = url;
            }

            request = HttpWebRequest.Create(fullUrl);
            request.Method = "GET";

            using (var resp = request.GetResponse())
            {
                var results = new StreamReader(resp.GetResponseStream()).ReadToEnd();

                return JsonDecode(results);
            }
        }

        public static HtmlNode NextElement(this HtmlNode htmlNode)
        {
            var nextSibling = htmlNode.NextSibling;

            while (nextSibling != null)
            {
                if (nextSibling.NodeType == HtmlNodeType.Element)
                {
                    return nextSibling;
                }

                nextSibling = nextSibling.NextSibling;
            }

            return null;
        }

        public static bool DownloadPage(this Uri uri, out HtmlDocument document)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            Stream responseStream;

            request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = "GET";
            request.Headers.Clear();

            request.UserAgent = Assembly.GetExecutingAssembly().FullName;

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                responseStream = response.GetResponseStream();

                using (var reader = new StreamReader(responseStream))
                {
                    var source = reader.ReadToEnd();

                    document = new HtmlDocument();
                    document.LoadHtml(source);
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    throw new HttpException((int)response.StatusCode, response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<(bool, Exception, HtmlDocument)> DownloadPageAsync(this Uri uri)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            Stream responseStream;
            HtmlDocument document;

            request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = "GET";
            request.Headers.Clear();

            request.UserAgent = Assembly.GetExecutingAssembly().FullName;

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                responseStream = response.GetResponseStream();

                using (var reader = new StreamReader(responseStream))
                {
                    var source = await reader.ReadToEndAsync();

                    document = new HtmlDocument();
                    document.LoadHtml(source);
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return (response.StatusCode == HttpStatusCode.OK, null, document);
                }
                else
                {
                    throw new HttpException((int) response.StatusCode, response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                document = null;
                return (false, ex, document);
            }
        }
    }
}
