using Hydra.ReleaseManagement.Services.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace ReleaseAgent
{
    public class Program
    {
        private static string pipelineWorkspace;
        private static string primaryProductBinaryName;
        private static string environment;
        private static string buildConfiguration;
        private static string releaseType;
        private static string commitId;
        private static string buildId;
        private static string repositoryUri;
        private static string additionalConfigFiles;
        private static string baseAddress;
        private static DirectoryInfo pipelineWorkspaceDirectory;
        private static FileInfo submissionIdFile;
        private static FileInfo primaryProductBinaryFile;
        private static DirectoryInfo installerDirectory;
        private static DirectoryInfo installerBundleDirectory;
        private static FileInfo installerWixFile;
        private static FileInfo installerBundleWixFile;
        private static FileInfo installerBundleFile;
        private static FileInfo installerFile;
        private static DirectoryInfo installerWixDirectory;
        private static DirectoryInfo installerBundleWixDirectory;
        private static DirectoryInfo primaryProjectDirectory;
        private static DirectoryInfo releaseInfoDirectory;
        private static FileInfo releaseInfoFile;
        private static FileInfo readMeFile;
        private static FileInfo solutionComponentsFile;
        private static FileInfo harvestedWixFile;
        private static FileInfo primaryProductBinaryConfigFile;

        public static void Main(string[] args)
        {
            var x = 0;
            Guid submissionId = Guid.Empty;
            ReleaseSubmission releaseSubmission;
            List<FileInfo> artifacts;

            pipelineWorkspace = args[x++];
            primaryProductBinaryName = args[x++];
            environment = args[x++];
            buildConfiguration = args[x++];
            releaseType = args[x++];
            commitId = args[x++];
            buildId = args[x++];
            repositoryUri = args[x++];
            additionalConfigFiles = args[x++];

            baseAddress = ConfigurationSettings.AppSettings["WebApiUrlBase"];

            Console.WriteLine("baseAddress: '{0}'", baseAddress);
            Console.WriteLine();

            Console.WriteLine($"pipelineWorkspace: { pipelineWorkspace }");
            Console.WriteLine($"primaryProductBinaryName: { primaryProductBinaryName }");
            Console.WriteLine($"environment: { environment }");
            Console.WriteLine($"buildConfiguration: { buildConfiguration }");
            Console.WriteLine($"releaseType: { releaseType }");
            Console.WriteLine($"commitId: { commitId }");
            Console.WriteLine($"buildId: { buildId }");
            Console.WriteLine($"repositoryUri: { repositoryUri }");
            Console.WriteLine($"additionalConfigFiles: { additionalConfigFiles }");

            Console.WriteLine("\r\nChecking artifact existence");

            try
            {
                pipelineWorkspaceDirectory = new DirectoryInfo(pipelineWorkspace);

                DebugUtils.AssertThrow(pipelineWorkspaceDirectory.Exists, "pipelineWorkspaceDirectory does not exist");

                primaryProjectDirectory = new DirectoryInfo(Path.Combine(pipelineWorkspace, "PrimaryProject"));
                DebugUtils.AssertThrow(primaryProjectDirectory.Exists, "primaryProjectDirectory does not exist");

                submissionIdFile = new FileInfo(Path.Combine(pipelineWorkspace, "SubmissionId.txt"));

                primaryProductBinaryFile = new FileInfo(Path.Combine(primaryProjectDirectory.FullName, primaryProductBinaryName));
                DebugUtils.AssertThrow(primaryProductBinaryFile.Exists, "primaryProductBinaryFile does not exist");

                primaryProductBinaryConfigFile = new FileInfo(Path.Combine(primaryProjectDirectory.FullName, primaryProductBinaryName + ".config"));
                DebugUtils.AssertThrow(primaryProductBinaryConfigFile.Exists, "primaryProductBinary Config File does not exist");

                readMeFile = primaryProjectDirectory.GetFiles().Where(f => f.Name.AsCaseless() == "README.md").SingleOrDefault();
                DebugUtils.AssertThrow(readMeFile != null, "readMeFile does not exist");

                releaseInfoDirectory = new DirectoryInfo(Path.Combine(pipelineWorkspace, "ReleaseInfo"));
                DebugUtils.AssertThrow(releaseInfoDirectory.Exists, "releaseInfoDirectory does not exist");

                releaseInfoFile = releaseInfoDirectory.GetFiles().Where(f => f.Name.AsCaseless() == "ReleaseInfo.json").Single();
                DebugUtils.AssertThrow(releaseInfoFile != null, "releaseInfoFile does not exist");

                installerBundleDirectory = new DirectoryInfo(Path.Combine(pipelineWorkspace, "InstallerBundle"));

                if (!installerBundleDirectory.Exists)
                {
                    Console.WriteLine("installerBundleDirectory does not exist");
                }
                else
                {
                    installerBundleFile = installerBundleDirectory.GetFiles().Where(f => f.Extension.IsOneOf(".exe", ".msi")).SingleOrDefault();
                    DebugUtils.AssertThrow(installerBundleFile != null, "installerBundleFile does not exist");
                }

                installerDirectory = new DirectoryInfo(Path.Combine(pipelineWorkspace, "Installer"));
                DebugUtils.AssertThrow(installerDirectory.Exists, "installerDirectory does not exist");

                installerFile = installerDirectory.GetFiles().Where(f => f.Extension.IsOneOf(".exe", ".msi")).Single();
                DebugUtils.AssertThrow(installerFile != null, "installerFile does not exist");

                installerBundleWixDirectory = new DirectoryInfo(Path.Combine(pipelineWorkspace, "InstallerBundleWix"));
                DebugUtils.AssertThrow(installerBundleWixDirectory.Exists, "installerBundleWixDirectory does not exist");

                installerBundleWixFile = installerBundleWixDirectory.GetFiles().Where(f => f.Name.AsCaseless() == "Product.wxs").SingleOrDefault();
                DebugUtils.AssertThrow(installerBundleWixFile != null, "installerBundleWixFile does not exist");

                installerWixDirectory = new DirectoryInfo(Path.Combine(pipelineWorkspace, "InstallerWix"));
                DebugUtils.AssertThrow(installerWixDirectory.Exists, "installerWixDirectory does not exist");

                installerWixFile = installerWixDirectory.GetFiles().Where(f => f.Name.AsCaseless() == "Product.wxs").SingleOrDefault();
                DebugUtils.AssertThrow(installerWixFile != null, "installerWixFile does not exist");

                harvestedWixFile = installerWixDirectory.GetFiles().Where(f => f.Name.AsCaseless() == "_" + Path.GetFileNameWithoutExtension(primaryProductBinaryFile.Name) + ".wxs").SingleOrDefault();
                DebugUtils.AssertThrow(harvestedWixFile != null, "harvestedWixFile does not exist");

                solutionComponentsFile = new FileInfo(Path.Combine(pipelineWorkspaceDirectory.FullName, "SolutionComponents.json"));
                DebugUtils.AssertThrow(solutionComponentsFile.Exists, "solutionComponentsFile does not exist");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }

            releaseSubmission = new ReleaseSubmission
            {
                SubmissionDate = DateTime.UtcNow,
                Environment = environment,
                BuildConfiguration = buildConfiguration,
                PrimaryBinaryFileName = string.Join(@"\", primaryProductBinaryFile.Directory.GetAncestors().Take(3).Reverse().Select(d => d.Name).Append(primaryProductBinaryFile.Name)),
                ReleaseInfoJson = File.ReadAllText(releaseInfoFile.FullName).ToBase64(),
                ReleaseType = releaseType,
                CommitId = commitId,
                BuildId = buildId,
                RepositoryUri = repositoryUri,
                AdditionalConfigFiles = additionalConfigFiles
            };

            Console.WriteLine("\r\nPinging Release Management server");

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage(new HttpMethod("GET"), baseAddress + "/releasemanagement/ping");
                    HttpResponseMessage response;
                    string returnValue;

                    Console.WriteLine("Calling {0}", request.RequestUri.ToString());

                    response = httpClient.SendAsync(request).Result;
                    returnValue = response.Content.ReadAsStringAsync().Result;

                    Console.WriteLine("Result: {0}", response);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("Response: {0}", returnValue);
                    }
                    else
                    {
                        Console.WriteLine("Response failed");
                        Environment.Exit(-1);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }

            Console.WriteLine("\r\nInitiating release");

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage(new HttpMethod("POST"), baseAddress + "/releasemanagement/initiaterelease");
                    var json = releaseSubmission.ToJsonText();
                    HttpResponseMessage response;
                    string returnValue;

                    Console.WriteLine("Calling {0}", request.RequestUri.ToString());

                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = httpClient.SendAsync(request).Result;
                    returnValue = response.Content.ReadAsStringAsync().Result;

                    Console.WriteLine("Result: {0}", returnValue);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("Response: {0}", returnValue);
                        submissionId = Guid.Parse(returnValue.RemoveQuotes());
                    }
                    else
                    {
                        Console.WriteLine("Response failed");
                        submissionId = Guid.Empty;

                        Environment.Exit(-1);
                    }
                }

                Console.WriteLine("\r\nPreparing and submitting artifacts");

                artifacts = PrepareArtifacts(submissionId);
                SubmitArtifacts(artifacts).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }

            Console.WriteLine("\r\nContinuing release");

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage(new HttpMethod("GET"), baseAddress + "/releasemanagement/continuerelease?submissionId=" + submissionId.ToString());
                    var json = releaseSubmission.ToJsonText();
                    HttpResponseMessage response;
                    string returnValue;

                    Console.WriteLine("Calling {0}", request.RequestUri.ToString());

                    response = httpClient.SendAsync(request).Result;
                    returnValue = response.Content.ReadAsStringAsync().Result;

                    Console.WriteLine("Result: {0}", returnValue);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("Response: {0}", returnValue);
                    }
                    else
                    {
                        Console.WriteLine("Response failed");

                        Environment.Exit(-1);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }
        }

        private static List<FileInfo> PrepareArtifacts(Guid submissionId)
        {
            var list = new List<FileInfo>();

            File.WriteAllText(submissionIdFile.FullName, submissionId.ToString());

            list.Add(submissionIdFile);
            list.Add(installerFile);
            list.Add(installerBundleFile);
            list.Add(installerBundleWixFile);
            list.Add(installerWixFile);
            list.Add(harvestedWixFile);
            list.Add(releaseInfoFile);
            list.Add(readMeFile);
            list.Add(solutionComponentsFile);
            list.Add(primaryProductBinaryConfigFile);

            if (!additionalConfigFiles.IsNullOrEmpty())
            {
                var files = additionalConfigFiles.Split(',');

                foreach (var filePath in files)
                {
                    var file = new FileInfo(filePath);

                    if (file.Exists)
                    {
                        list.Add(file);
                    }
                    else
                    {
                        throw new FileNotFoundException($"Additional config file not found, file: { filePath }");
                    }
                }
            }

            return list;
        }

        public static async Task SubmitArtifacts(List<FileInfo> artifacts)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(30);

                    using (var multipartFormDataContent = new MultipartFormDataContent())
                    {
                        HttpResponseMessage result;

                        foreach (var artifact in artifacts)
                        {
                            var file = new FileInfo(artifact.FullName);
                            var stream = new FileStream(artifact.FullName, FileMode.Open);
                            var reader = new BinaryReader(stream);
                            var fileName = file.Name;
                            var name = file.Name;
                            var length = (int)file.Length;
                            var fileContent = new ByteArrayContent(reader.ReadBytes(length));
                            var uniqueName = file.Directory.Name + "__" + fileName;

                            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                            {
                                FileName = uniqueName,
                                Name = uniqueName
                            };

                            multipartFormDataContent.Add(fileContent, uniqueName);
                        }

                        result = await httpClient.PostAsync(baseAddress + "/releasemanagement/submitartifacts", multipartFormDataContent).ConfigureAwait(false);

                        if (!result.IsSuccessStatusCode)
                        {
                            var body = result.Content.ReadAsStringAsync().Result;
                        }

                        result.EnsureSuccessStatusCode();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }
        }
    }
}
