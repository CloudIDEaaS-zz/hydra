using AbstraX;
using AbstraX.XPathBuilder;
using CodePlex.XPathParser;
using CoreShim.Reflection.JsonTypes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Utils;

namespace NetCoreReflectionShim.Service
{
    public static class Extensions
    {
        public static string GetAssemblyName(this KeyValuePair<string, object>[] arguments)
        {
            var path = (string) arguments.Single(a => a.Key == "Name").Value;
            var uri = new Uri(path);
            var nameValues = HttpUtility.ParseQueryString(uri.Query).GetKeyValues();
            var builder = new StringBuilder();

            foreach (var nameValue in nameValues)
            {
                switch (nameValue.Key)
                {
                    case "Name":
                        builder.Append(nameValue.Value + ", ");
                        break;
                    default:
                        builder.Append(string.Format("{0}={1}, ", nameValue.Key, nameValue.Value));
                        break;
                }
            }

            builder.RemoveEnd(2);

            return builder.ToString();
        }

        public static IEnumerable<KeyValuePair<string, string>> GetKeyValues(this NameValueCollection collection)
        {
            var items = collection.AllKeys.SelectMany(collection.GetValues, (k, v) => new {key = k, value = v});

            foreach (var item in items)
            {
                yield return new KeyValuePair<string, string>(item.key, item.value);
            }
        }


        /// <summary>   An Exception extension method that handles the exit exception. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/21/2021. </remarks>
        ///
        /// <param name="exception">        The exception to act on. </param>
        /// <param name="workingDirectory"> Pathname of the working directory. </param>

        public static void HandleExitException(this Exception exception, string workingDirectory, bool runAsAutomated)
        {
            var process = Process.GetCurrentProcess();
            var thread = Thread.CurrentThread;
            var debugLogDirectory = new DirectoryInfo(Path.Combine(workingDirectory, string.Format(@"Logs\{0}", DateTime.Now.ToSortableDateTimeText())));
            var debugLogFile = new FileInfo(Path.Combine(debugLogDirectory.FullName, string.Format("{0}-Debug.log", process.ProcessName)));
            var dumpLogFile = new FileInfo(Path.Combine(debugLogDirectory.FullName, string.Format("{0}.dmp", process.ProcessName)));
            var assembly = Assembly.GetEntryAssembly();
            var attributes = assembly.GetAttributes();
            var analyzerClient = new CrashAnalyzerClient();
            string message;

            if (!debugLogDirectory.Exists)
            {
                debugLogFile.Directory.Create();
            }

            using (var logWriter = System.IO.File.AppendText(debugLogFile.FullName))
            {
                logWriter.Write("\r\nLog Entry : ");
                logWriter.WriteLine($"{ DateTime.Now.ToLongTimeString() } { DateTime.Now.ToLongDateString() }");
                logWriter.WriteLine("  :");
                logWriter.WriteLine($"  :{ exception.ToString() }");
                logWriter.WriteLine("-------------------------------");
            }

            message = exception.Message;

            analyzerClient.RunAnalysis(process.Id, thread.ManagedThreadId, DebugUtils.GetCurrentThreadId(), dumpLogFile, runAsAutomated);

            MessageBox.Show($"Hydra has run into an unexpected error.  A crash analysis will be performed. Error message: { message }", attributes.Product);

            analyzerClient.Wait();
        }

        public static (string, int, string[]) GetParts(this KeyValuePair<string, object>[] arguments)
        {
            var identifier = (string)arguments.Single(a => a.Key == "Identifier").Value;
            var args = ((JArray)arguments.SingleOrDefault(a => a.Key == "Arguments").Value).ToObject<string[]>();
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();
            var id = string.Empty;
            var metadataToken = 0;
            IEnumerable<XPathElement> elements;
            IEnumerable<XPathElement> nonPredicatedElements;
            XPathElement metadataElement;
            XPathElement firstElement;
            XPathPredicate metadataPredicate;
            Uri uri;
            string location;

            if (Uri.TryCreate(identifier, UriKind.Absolute, out uri))
            {
                switch (uri.Scheme)
                {
                    case "assembly":
                        location = identifier;
                        break;
                    default:
                        DebugUtils.Break();
                        location = null;
                        break;
                }
            }
            else
            {
                parser.Parse(identifier, builder);

                elements = builder.PartQueue.OfType<XPathElement>();
                nonPredicatedElements = elements.Where(e => e.Predicates.Count == 0);
                metadataElement = builder.PartQueue.OfType<XPathElement>().SingleOrDefault(e => e.Predicates.Count > 0);
                firstElement = nonPredicatedElements.First();

                if (metadataElement != null)
                {
                    location = nonPredicatedElements.Skip(1).Select(e => e.Text).Concat(new List<string> { metadataElement.Text }).ToDelimitedList(@"\");
                }
                else
                {
                    location = nonPredicatedElements.Skip(1).Select(e => e.Text).ToDelimitedList(@"\");
                }

                if (DriveInfo.GetDrives().Any(d => d.Name.RemoveEndIfMatches(@":\").AsCaseless() == firstElement.Text))
                {
                    var drive = DriveInfo.GetDrives().Single(d => d.Name.RemoveEndIfMatches(@":\").AsCaseless() == firstElement.Text);

                    location = drive.Name + location;
                }

                if (metadataElement != null)
                {
                    metadataPredicate = metadataElement.Predicates.OfType<XPathPredicate>().Single();
                    metadataToken = int.Parse(metadataPredicate.RightFormatted);
                }
            }

            return (location, metadataToken, args);
        }

        public static (string, int, int, string, string[]) GetObjectParts(this KeyValuePair<string, object>[] arguments)
        {
            var identifier = (string)arguments.Single(a => a.Key == "Identifier").Value;
            var args = ((JArray)arguments.SingleOrDefault(a => a.Key == "Arguments").Value).ToObject<string[]>();
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();
            var id = string.Empty;
            var metadataToken = 0;
            var hashCode = 0;
            var member = string.Empty;
            IEnumerable<XPathElement> elements;
            IEnumerable<XPathElement> nonPredicatedElements;
            IEnumerable<XPathElement> predicatedElements;
            XPathElement metadataElement;
            XPathElement hashCodeElement;
            XPathElement memberElement;
            XPathElement firstElement;
            XPathPredicate metadataPredicate;
            XPathPredicate hashCodePredicate;
            XPathPredicate memberPredicate;
            Uri uri;
            string location;

            if (Uri.TryCreate(identifier, UriKind.Absolute, out uri))
            {
                switch (uri.Scheme)
                {
                    case "assembly":
                        location = identifier;
                        break;
                    default:
                        DebugUtils.Break();
                        location = null;
                        break;
                }
            }
            else
            {
                parser.Parse(identifier, builder);

                elements = builder.PartQueue.OfType<XPathElement>();
                nonPredicatedElements = elements.Where(e => e.Predicates.Count == 0);
                predicatedElements = elements.Where(e => e.Predicates.Count > 0);

                metadataElement = predicatedElements.ElementAt(0);
                hashCodeElement = predicatedElements.Single(e => e.Text == "ObjectInstance");
                memberElement = predicatedElements.Single(e => e.Text == "Member");
                firstElement = nonPredicatedElements.First();

                if (metadataElement != null)
                {
                    location = nonPredicatedElements.Skip(1).Select(e => e.Text).Concat(new List<string> { metadataElement.Text }).ToDelimitedList(@"\");
                }
                else
                {
                    location = nonPredicatedElements.Skip(1).Select(e => e.Text).ToDelimitedList(@"\");
                }

                if (DriveInfo.GetDrives().Any(d => d.Name.RemoveEndIfMatches(@":\") == firstElement.Text))
                {
                    var drive = DriveInfo.GetDrives().Single(d => d.Name.RemoveEndIfMatches(@":\") == firstElement.Text);

                    location = drive.Name + location;
                }

                metadataPredicate = metadataElement.Predicates.OfType<XPathPredicate>().Single();
                metadataToken = int.Parse(metadataPredicate.RightFormatted);

                hashCodePredicate = hashCodeElement.Predicates.OfType<XPathPredicate>().Single();
                hashCode = int.Parse(hashCodePredicate.RightFormatted);

                memberPredicate = memberElement.Predicates.OfType<XPathPredicate>().Single(p => p.Left.Name == "Name");
                member = memberPredicate.RightFormatted.RemoveQuotes();
            }

            return (location, metadataToken, hashCode, member, args);
        }
    }
}
