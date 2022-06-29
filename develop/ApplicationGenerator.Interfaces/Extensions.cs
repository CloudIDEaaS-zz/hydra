// file:	Extensions.cs
//
// summary:	Implements the extensions class

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Utils;
using AbstraX.DataAnnotations;
using CodePlex.XPathParser;
using AbstraX.XPathBuilder;
using AbstraX.ServerInterfaces;
using System.Diagnostics;
using EntityProvider.Web.Entities;
using System.ComponentModel;
using System.Linq.Expressions;
using Assembly = System.Reflection.Assembly;
using System.ComponentModel.DataAnnotations;
using AbstraX.TypeMappings;
using System.IO;
using AbstraX.FolderStructure;
using Utils.Hierarchies;
using AbstraX.QueryPath;
using AbstraX.PackageCache;
using AbstraX.Models.Interfaces;
using System.Reflection;
using AbstraX.TemplateObjects;
using System.Reflection.Emit;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Text.RegularExpressions;
using System.Reflection.Metadata.Ecma335;
using CustomAttributeData = System.Reflection.CustomAttributeData;
using NetCoreReflectionShim.Agent;
using CoreShim.Reflection;
using System.Configuration;
using System.Collections;
using AbstraX.CommandHandlers;
using System.Threading;
using System.Drawing;
using AbstraX.Resources;
using AbstraX.ObjectProperties;
using System.Drawing.Imaging;
using Newtonsoft.Json.Serialization;
using MailSlot;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using WizardBase;

namespace AbstraX
{
    /// <summary>   An extensions. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>

    public static partial class Extensions 
    {
        /// <summary>   A System.Windows.Forms.Label extension method that draw gradient. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/8/2021. </remarks>
        ///
        /// <param name="labelCaption"> The label caption control. </param>
        /// <param name="e">            Paint event information. </param>

        public static void DrawGradient(this System.Windows.Forms.Label labelCaption, PaintEventArgs e)
        {
            var rect = labelCaption.ClientRectangle;
            var color1 = Color.Transparent;
            var color2 = ColorTranslator.FromHtml("#7b8989").Lighten(.8);

            using (var brush = new LinearGradientBrush(rect, color1, color2, 15f, false))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            using (var pen = new Pen(Color.DarkGray))
            {
                e.Graphics.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
            }
        }

        /// <summary>   An IResourceManager extension method that normalize path. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        ///
        /// <param name="resourceManager">  The resourceManager to act on. </param>
        /// <param name="path">             Full pathname of the file. </param>
        ///
        /// <returns>   A string. </returns>

        public static string NormalizePath(this IResourceManager resourceManager, string path)
        {
            if (!path.IsNullOrEmpty())
            {
                var rootPath = resourceManager.RootPath;
                var file = new FileInfo(path);

                if (path.AsCaseless().StartsWith(rootPath))
                {
                    path = path.RemoveStart(rootPath).RemoveStartIfMatches(@"\");
                }
            }

            return path;
        }

        /// <summary>   An IResourceManager extension method that denormalize path. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        ///
        /// <param name="resourceManager">  The resourceManager to act on. </param>
        /// <param name="path">             Full pathname of the file. </param>
        ///
        /// <returns>   A string. </returns>

        public static string DenormalizePath(this IResourceManager resourceManager, string path)
        {
            if (!path.IsNullOrEmpty())
            {
                var rootPath = resourceManager.RootPath;
                var file = new FileInfo(path);
                var directory = new DirectoryInfo(rootPath);

                if (!Path.IsPathRooted(path))
                {
                    path = Path.Combine(rootPath, path);
                }
                else if (!file.Exists)
                {
                    var files = directory.GetFiles(file.Name).ToList();

                    if (files.Count == 1)
                    {
                        file = files.Single();
                        path = file.FullName;
                    }
                }
            }
            
            return path;
        }

        /// <summary>   An IHandlerAssemblyCache extension method that gets wizard page. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/15/2021. </remarks>
        ///
        /// <param name="handlerAssemblyCache"> The handlerAssemblyCache to act on. </param>
        /// <param name="generatorHandlerType"> Type of the generator handler. </param>
        /// <param name="pageName">             Name of the page. </param>
        ///
        /// <returns>   The wizard page. </returns>

        public static IWizardPage GetWizardPage(this IHandlerAssemblyCache handlerAssemblyCache, string generatorHandlerType, string pageName)
        {
            Assembly assembly;
            Type type;
            IWizardPage wizardPage;
            var handlerAssemblies = handlerAssemblyCache.HandlerAssemblies;
            var handlerElements = handlerAssemblyCache.HandlerElements;

            if (!handlerAssemblies.ContainsKey(generatorHandlerType))
            {
                var handlerElement = handlerElements.Single(e => e.Attribute("handlerType").Value == generatorHandlerType);
                var assemblyElement = handlerElement.Elements().Single();
                var name = assemblyElement.Attribute("name").Value;
                var version = assemblyElement.Attribute("version").Value;
                var publicKeyToken = assemblyElement.Attribute("publicKeyToken").Value;
                var culture = assemblyElement.Attribute("culture").Value;
                var assemblyNameString = $"{ name }, Version={ version }, Culture={ culture }, PublicKeyToken={ publicKeyToken }";
                var assemblyName = new AssemblyName(assemblyNameString);

                assembly = Assembly.Load(assemblyName);
            }
            else
            {
                assembly = handlerAssemblies[generatorHandlerType];
            }

            type = assembly.GetTypes().Single(t => t.HasCustomAttribute<WizardPageAttribute>() && t.GetCustomAttribute<WizardPageAttribute>().Name == pageName);
            wizardPage = (IWizardPage)Activator.CreateInstance(type);

            return wizardPage;
        }

        /// <summary>   A List&lt;Type&gt; extension method that handler, called when the get. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 5/31/2022. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="handlerAssemblyCache"> The handlerAssemblyCache to act on. </param>
        /// <param name="args">                 A variable-length parameters list containing arguments. </param>
        ///
        /// <returns>   The handler. </returns>

        public static T GetHandler<T>(this IHandlerAssemblyCache handlerAssemblyCache, params object[] args) where T : IHandler
        {
            var handlers = new List<T>();
            var assembly = Assembly.GetEntryAssembly();
            var handlerAssemblies = handlerAssemblyCache.HandlerAssemblies;
            var allTypes = assembly.GetMinimumTypes().ToList();

            foreach (var handlerElement in handlerAssemblyCache.HandlerElements)
            {
                var assemblyElement = handlerElement.Elements().Single();
                var name = assemblyElement.Attribute("name").Value;
                var version = assemblyElement.Attribute("version").Value;
                var publicKeyToken = assemblyElement.Attribute("publicKeyToken").Value;
                var culture = assemblyElement.Attribute("culture").Value;
                var assemblyNameString = $"{ name }, Version={ version }, Culture={ culture }, PublicKeyToken={ publicKeyToken }";
                var assemblyName = new AssemblyName(assemblyNameString);

                assembly = Assembly.Load(assemblyName);

                allTypes.AddRange(assembly.GetTypes());
            }

            allTypes.AddRange(handlerAssemblies.Values.SelectMany(a => a.GetTypes()));

            foreach (var type in allTypes.Where(t => !t.IsInterface && t.Implements<T>()))
            {
                var handler = (T)Activator.CreateInstance(type, args);

                handlers.Add(handler);
            }

            return handlers.OrderBy(h => h.Priority).FirstOrDefault();
        }

        /// <summary>
        /// An IHandlerAssemblyCache extension method that handler, called when the get process.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/15/2021. </remarks>
        ///
        /// <param name="handlerAssemblyCache"> The handlerAssemblyCache to act on. </param>
        /// <param name="generatorHandlerType"> Type of the generator handler. </param>
        /// <param name="processHandlerKind">   The process handler kind. </param>
        ///
        /// <returns>   The process handler. </returns>

        public static IProcessHandler GetProcessHandler(this IHandlerAssemblyCache handlerAssemblyCache, string generatorHandlerType, ProcessHandlerKind processHandlerKind)
        {
            Assembly assembly;
            Type type;
            IProcessHandler processHandler;
            var handlerAssemblies = handlerAssemblyCache.HandlerAssemblies;
            var handlerElements = handlerAssemblyCache.HandlerElements;

            if (!handlerAssemblies.ContainsKey(generatorHandlerType))
            {
                var handlerElement = handlerElements.Single(e => e.Attribute("handlerType").Value == generatorHandlerType);
                var assemblyElement = handlerElement.Elements().Single();
                var name = assemblyElement.Attribute("name").Value;
                var version = assemblyElement.Attribute("version").Value;
                var publicKeyToken = assemblyElement.Attribute("publicKeyToken").Value;
                var culture = assemblyElement.Attribute("culture").Value;
                var assemblyNameString = $"{ name }, Version={ version }, Culture={ culture }, PublicKeyToken={ publicKeyToken }";
                var assemblyName = new AssemblyName(assemblyNameString);

                assembly = Assembly.Load(assemblyName);
            }
            else
            {
                assembly = handlerAssemblies[generatorHandlerType];
            }

            type = assembly.GetTypes().Single(t => t.HasCustomAttribute<ProcessHandlerAttribute>() && t.GetCustomAttribute<ProcessHandlerAttribute>().ProcessHandlerKind.HasFlag(processHandlerKind));
            processHandler = (IProcessHandler)Activator.CreateInstance(type);

            return processHandler;
        }

        /// <summary>
        /// An IHandlerAssemblyCache extension method that gets application layout surveyor.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/15/2021. </remarks>
        ///
        /// <param name="handlerAssemblyCache"> The handlerAssemblyCache to act on. </param>
        /// <param name="generatorHandlerType"> Type of the generator handler. </param>
        ///
        /// <returns>   The application layout surveyor. </returns>

        public static IAppFolderStructureSurveyor GetAppFolderStructureSurveyor(this IHandlerAssemblyCache handlerAssemblyCache, string generatorHandlerType)
        {
            Assembly assembly;
            Type type;
            IAppFolderStructureSurveyor appFolderStructureSurveyor;
            var handlerAssemblies = handlerAssemblyCache.HandlerAssemblies;
            var handlerElements = handlerAssemblyCache.HandlerElements;

            if (!handlerAssemblies.ContainsKey(generatorHandlerType))
            {
                var handlerElement = handlerElements.Single(e => e.Attribute("handlerType").Value == generatorHandlerType);
                var assemblyElement = handlerElement.Elements().Single();
                var name = assemblyElement.Attribute("name").Value;
                var version = assemblyElement.Attribute("version").Value;
                var publicKeyToken = assemblyElement.Attribute("publicKeyToken").Value;
                var culture = assemblyElement.Attribute("culture").Value;
                var assemblyNameString = $"{ name }, Version={ version }, Culture={ culture }, PublicKeyToken={ publicKeyToken }";
                var assemblyName = new AssemblyName(assemblyNameString);

                assembly = Assembly.Load(assemblyName);
            }
            else
            {
                assembly = handlerAssemblies[generatorHandlerType];
            }

            type = assembly.GetTypes().Single(t => t.Implements<IAppFolderStructureSurveyor>());
            appFolderStructureSurveyor = (IAppFolderStructureSurveyor)Activator.CreateInstance(type);

            return appFolderStructureSurveyor;
        }

        /// <summary>   A MailslotClient extension method that reports callback status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/24/2021. </remarks>
        ///
        /// <param name="generatorEngine">  The mailslotClient to act on. </param>
        /// <param name="name">             The name. </param>
        /// <param name="status">           The status. </param>
        /// <param name="percentComplete">  The percent complete. </param>
        /// <param name="percentOfStart">   The percent of start. </param>
        /// <param name="percentOf">        The percent of. </param>

        public static void SendCallbackStatus(this IGeneratorEngine generatorEngine, string name, string status, int percentComplete, int percentOfStart, int percentOf)
        {
            var mailslotClient = generatorEngine.MailslotClient;
            var config = generatorEngine.GeneratorConfiguration;
            var range = percentOf - percentOfStart;
            var percentCompleteRevised = (int)(percentOfStart + ((percentComplete.As<float>() / 100f) * range));
            var message = string.Format("callback: name={0}, status={1}, percentComplete={2}", name, status, percentCompleteRevised);

            if (config == null)
            {
                generatorEngine.LogMessageQueue.Enqueue(status);
            }
            else
            {
                while (generatorEngine.LogMessageQueue.Count > 0)
                {
                    var queuedMessage = generatorEngine.LogMessageQueue.Dequeue();

                    config.LogStatusInformation(queuedMessage);
                }

                config.LogStatusInformation(message);
            }

            if (mailslotClient != null)
            {
                mailslotClient.SendMessage(message);
                mailslotClient.Flush();
            }
        }

        /// <summary>   A MailslotClient extension method that reports callback status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/24/2021. </remarks>
        ///
        /// <param name="generatorEngine">  The mailslotClient to act on. </param>
        /// <param name="currentPass">      The current pass. </param>
        /// <param name="name">             The name. </param>
        /// <param name="status">           The status. </param>
        /// <param name="percentComplete">  The percent complete. </param>
        /// <param name="percentOfStart">   The percent of start. </param>
        /// <param name="percentOf">        The percent of. </param>

        public static void SendCallbackStatus(this IGeneratorEngine generatorEngine, GeneratorPass currentPass, string name, string status, int percentComplete, int percentOfStart, int percentOf)
        {
            var mailslotClient = generatorEngine.MailslotClient;
            var config = generatorEngine.GeneratorConfiguration;
            var message = string.Format("callback: name={0}, status={1}, percentComplete={2}", name, status, percentComplete);

            if (config == null)
            {
                generatorEngine.LogMessageQueue.Enqueue(status);
            }
            else
            {
                while (generatorEngine.LogMessageQueue.Count > 0)
                {
                    var queuedMessage = generatorEngine.LogMessageQueue.Dequeue();

                    config.LogStatusInformation(queuedMessage);
                }

                config.LogStatusInformation(message);
            }

            if (mailslotClient != null)
            {
                if (currentPass == GeneratorPass.Files)
                {
                    var range = percentOf - percentOfStart;

                    percentComplete = (int)(percentOfStart + ((percentComplete.As<float>() / 100f) * range));

                    mailslotClient.SendMessage(message);
                    mailslotClient.Flush();
                }
            }
        }

        /// <summary>   A MailslotClient extension method that reports callback status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        ///
        /// <param name="generatorEngine">  The mailslotClient to act on. </param>
        /// <param name="name">             The name. </param>
        /// <param name="status">           The status. </param>
        /// <param name="percentComplete">  The percent complete. </param>

        public static void SendCallbackStatus(this IGeneratorEngine generatorEngine, string name, string status, int percentComplete)
        {
            var mailslotClient = generatorEngine.MailslotClient;
            var config = generatorEngine.GeneratorConfiguration;
            var message = string.Format("callback: name={0}, status={1}, percentComplete={2}", name, status, percentComplete);

            if (config == null)
            {
                generatorEngine.LogMessageQueue.Enqueue(status);
            }
            else
            {
                while (generatorEngine.LogMessageQueue.Count > 0)
                {
                    var queuedMessage = generatorEngine.LogMessageQueue.Dequeue();

                    config.LogStatusInformation(queuedMessage);
                }

                config.LogStatusInformation(message);
            }

            if (mailslotClient != null)
            {
                mailslotClient.SendMessage(message);
                mailslotClient.Flush();
            }
        }

        /// <summary>   A MailslotClient extension method that reports callback status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>
        ///
        /// <param name="generatorEngine">  The mailslotClient to act on. </param>
        /// <param name="currentPass">      The current pass. </param>
        /// <param name="name">             The name. </param>
        /// <param name="status">           The status. </param>
        /// <param name="percentComplete">  The percent complete. </param>

        public static void SendCallbackStatus(this IGeneratorEngine generatorEngine, GeneratorPass currentPass, string name, string status, int percentComplete)
        {
            var mailslotClient = generatorEngine.MailslotClient;
            var config = generatorEngine.GeneratorConfiguration;
            var message = string.Format("callback: name={0}, status={1}, percentComplete={2}", name, status, percentComplete);

            if (config == null)
            {
                generatorEngine.LogMessageQueue.Enqueue(status);
            }
            else
            {
                while (generatorEngine.LogMessageQueue.Count > 0)
                {
                    var queuedMessage = generatorEngine.LogMessageQueue.Dequeue();

                    config.LogStatusInformation(queuedMessage);
                }

                config.LogStatusInformation(message);
            }

            if (mailslotClient != null)
            {
                if (currentPass == GeneratorPass.Files)
                {
                    mailslotClient.SendMessage(message);
                    mailslotClient.Flush();
                }
            }
        }

        /// <summary>   A MailslotClient extension method that sends a hydra status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/24/2021. </remarks>
        ///
        /// <param name="generatorEngine">  The mailslotClient to act on. </param>
        /// <param name="status">           The status. </param>

        public static void SendHydraStatus(this IGeneratorEngine generatorEngine, string status)
        {
            var mailslotClient = generatorEngine.MailslotClient;
            var config = generatorEngine.GeneratorConfiguration;

            if (config == null)
            {
                generatorEngine.LogMessageQueue.Enqueue(status);
            }
            else
            {
                while (generatorEngine.LogMessageQueue.Count > 0)
                {
                    var queueStatus = generatorEngine.LogMessageQueue.Dequeue();

                    config.LogStatusInformation(queueStatus);
                }

                config.LogStatusInformation(status);
            }

            if (mailslotClient != null)
            {
                mailslotClient.SendMessage(status);
                mailslotClient.Flush();
            }
        }

        /// <summary>
        /// An ObjectPropertiesDictionary extension method that converts the properties to an object.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/20/2021. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="properties">   The properties to act on. </param>
        ///
        /// <returns>   Properties as a T. </returns>

        public static T ToObject<T>(this ObjectPropertiesDictionary properties)
        {
            var json = properties.ToJsonText(Newtonsoft.Json.Formatting.Indented, new CamelCaseNamingStrategy());

            return JsonExtensions.ReadJson<T>(json);
        }

        /// <summary>   The MarketingObjectProperties extension method that gets link properties. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/20/2021. </remarks>
        ///
        /// <param name="marketingObjectProperties">    The marketingObjectProperties to act on. </param>
        /// <param name="linkType">                     Type of the link. </param>
        /// <param name="link">                         The link. </param>
        ///
        /// <returns>   The link properties. </returns>

        public static LinkProperties GetLinkProperties(this MarketingObjectProperties marketingObjectProperties, string linkType, string link)
        {
            LinkProperties properties;

            if (marketingObjectProperties.Links.ContainsKey(link))
            {
                properties = marketingObjectProperties.Links[link];
            }
            else
            {
                properties = new LinkProperties(Guid.NewGuid().ToString().Left(8));
            }

            if (properties.LinkCallToAction == null)
            {
                switch (linkType)
                {
                    case "AdvertisingLink":
                        properties.LinkCallToAction = "Advertise with us";
                        break;
                    case "ConnectWithUsLink":
                        properties.LinkCallToAction = "Connect with us";
                        break;
                    case "EmailUsLink":
                        properties.LinkCallToAction = "Contact us";
                        break;
                    case "TellOthers":
                        properties.LinkCallToAction = "Tell others about us";
                        break;
                    case "Other":
                        properties.LinkCallToAction = link;
                        break;
                }
            }

            return properties;
        }

        /// <summary>   A FileInfo extension method that sets image file details. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/19/2021. </remarks>
        ///
        /// <param name="resourceData">     The generator configuration. </param>
        /// <param name="imageInfo"></param>
        /// <param name="fileInfoImage">    The fileInfoImage to act on. </param>

        public static void SetImageFileDetails(this IResourceData resourceData, IImageInfo imageInfo, FileInfo fileInfoImage, TextWriter textWriter)
        {
            var assembly = Assembly.GetEntryAssembly();
            var assemblyAttributes = assembly.GetAttributes();
            var programName = string.Format("{0} v{1}, {2}, {3}", assemblyAttributes.Product, assemblyAttributes.Version, assemblyAttributes.Company, assemblyAttributes.Copyright);
            var objectProperties = imageInfo.ObjectProperties;

            if (objectProperties != null)
            {
                var baseObjectProperties = objectProperties.Where(p => p.Key.IsOneOf(TypeExtensions.GetPublicPropertyNames<ImageProperties>())).ToDictionary(p => p.Key, p => p.Value);
                FileDetails fileDetails;
                Bitmap bitmapImage;
                string copyImage;

                copyImage = Path.Combine(fileInfoImage.DirectoryName, "~" + fileInfoImage.Name);

                fileInfoImage.CopyTo(copyImage);

                // following adds Exif info to the file to further processing
                // see https://en.wikipedia.org/wiki/Exif

                using (bitmapImage = (Bitmap)Image.FromFile(copyImage))
                {
                    bitmapImage.SetMetaValue(MetaProperty.Software, programName);
                    bitmapImage.Save(fileInfoImage.FullName, ImageFormat.Jpeg);
                }

                System.IO.File.Delete(copyImage);

                fileDetails = new FileDetails(fileInfoImage);

                foreach (var pair in baseObjectProperties)
                {
                    switch (pair.Key)
                    {
                        case "Author":
                            {
                                if (pair.Value is string stringValue && !stringValue.IsNullOrEmpty())
                                {
                                    fileDetails.SetGeneralAuthorProperty(stringValue);
                                }
                                else
                                {
                                    fileDetails.SetGeneralAuthorProperty(resourceData.OrganizationName);
                                }
                            }
                            break;
                        case "Rating":
                            {
                                if (pair.Value is int intValue)
                                {
                                    fileDetails.SetGeneralRatingProperty((uint)intValue);
                                }
                                else
                                {
                                    fileDetails.SetGeneralRatingProperty((uint)90);
                                }
                            }
                            break;
                        case "Title":
                            {
                                if (pair.Value is string stringValue && !stringValue.IsNullOrEmpty())
                                {
                                    fileDetails.SetGeneralTitleProperty(stringValue);
                                }
                                else
                                {
                                    fileDetails.SetGeneralTitleProperty(imageInfo.ResourceName);
                                }
                            }
                            break;
                        case "Comment":
                            {
                                if (pair.Value is string stringValue && !stringValue.IsNullOrEmpty())
                                {
                                    fileDetails.SetGeneralCommentProperty(stringValue);
                                }
                                else if (!imageInfo.Description.IsNullOrEmpty())
                                {
                                    fileDetails.SetGeneralCommentProperty(imageInfo.Description);
                                }
                            }
                            break;
                        case "Keywords":
                            {
                                if (pair.Value is string[] stringValues && stringValues.Length > 0)
                                {
                                    fileDetails.SetGeneralKeywordsProperty(stringValues);
                                }
                            }
                            break;
                        case "ItemUrl":
                            {
                                if (pair.Value is string stringValue && !stringValue.IsNullOrEmpty())
                                {
                                    fileDetails.SetGeneralItemUrlProperty(stringValue);
                                }
                            }
                            break;
                    }
                }

                fileDetails.SetGeneralCopyrightProperty($"Copyright © { resourceData.OrganizationName } { DateTime.Now.ToString("yyyy") }");
                fileDetails.SetGeneralDateAcquiredProperty(DateTime.Now);
            }
            else
            {
                textWriter.WriteLine("The following image has no object properties and will deploy with minimal properties, image file {0}", fileInfoImage.FullName);
            }
        }

        /// <summary>
        /// An IMarketingObject extension method that gets marketing object attribute provider.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/13/2021. </remarks>
        ///
        /// <param name="marketingObject">  The marketingObject to act on. </param>
        ///
        /// <returns>   The marketing object attribute provider. </returns>

        public static IMarketingObjectAttributeProvider GetMarketingObjectAttributeProvider(this IMarketingObject marketingObject)
        {
            var packageCachePath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\hydra\cache");
            var keyValuePair = AbstraXExtensions.GetOverrides(false, marketingObject.WorkingDirectory).LastOrDefault();
            var argumentsKind = keyValuePair.Key;
            var generatorOverrides = keyValuePair.Value;
            var arguments = generatorOverrides.GetHandlerArguments(packageCachePath, argumentsKind, marketingObject.WorkingDirectory);
            var generatorHandlerType = (string)arguments.Single(a => a.Key == "GeneratorHandlerType").Value;
            var generatorHandler = AbstraXExtensions.GetGeneratorHandler(generatorHandlerType);
            var assembly = generatorHandler.GetType().Assembly;
            var type = assembly.GetTypes().Where(t => t.HasCustomAttribute<MarketingObjectAttribute>()).SingleOrDefault(t => t.GetCustomAttribute<MarketingObjectAttribute>().ObjectName == marketingObject.Name);
            var supportedMarketingVariables = ConfigurationSettings.AppSettings["SupportedMarketingVariables"];

            if (type == null)
            {
                type = assembly.GetTypes().Where(t => t.HasCustomAttribute<MarketingObjectAttribute>()).SingleOrDefault(t => t.GetCustomAttribute<MarketingObjectAttribute>().ObjectName == "*");
            }

            if (type != null)
            {
                var provider = (IMarketingObjectAttributeProvider) Activator.CreateInstance(type);
                var variablesDictionary = new Dictionary<string, List<string>>();
                var groups = supportedMarketingVariables.Split(";");
                var regex = new Regex("(?<key>.*?):(?<variables>.*)");

                foreach (var group in groups)
                {
                    var match = regex.Match(group);
                    var key = match.GetGroupValue("key").Trim();
                    var variables = match.GetGroupValue("variables").Split(",").Select(v => v.Trim()).ToList();

                    variablesDictionary.Add(key, variables);
                }

                provider.SupportedVariables = variablesDictionary;

                return provider;
            }

            return null;
        }

        /// <summary>   An IMarketingEntry extension method that gets small logo image. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/7/2021. </remarks>
        ///
        /// <param name="marketingEntry">   The marketingEntry to act on. </param>
        ///
        /// <returns>   The small logo image. </returns>

        public static Image GetSmallLogoImage(this IMarketingEntry marketingEntry)
        {
            var imageText = marketingEntry.SmallLogo;
            var regex = new Regex(@"^data:image/(?<imageType>.*?);base64,(?<data>.*)$");
            var match = regex.Match(imageText);
            var imageType = match.GetGroupValue("imageType");
            var dataString = match.GetGroupValue("data");
            var bytes = dataString.FromBase64();
            var image = Image.FromStream(bytes.ToMemory());

            image = image.ResizeImage(20, 20);

            return image;
        }


        /// <summary>   A FileInfo extension method that creates a key. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/15/2021. </remarks>
        ///
        /// <param name="file">   The configFile to act on. </param>
        ///
        /// <returns>   The new key. </returns>

        public static string GenerateKey(this FileInfo file)
        {
            return string.Format("{0}:{1}", file.FullName, file.LastWriteTime);
        }

        /// <summary>   A HtmlAgilityPack.HtmlNode extension method that creates a box. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/25/2020. </remarks>
        ///
        /// <param name="parentNode">   The parent node. </param>
        /// <param name="name">         The name. </param>

        public static HtmlAgilityPack.HtmlNode CreateBox(this HtmlAgilityPack.HtmlNode parentNode, string name)
        {
            var sectionBoxElement = HtmlAgilityPack.HtmlNode.CreateNode($"<div name=\"{ name }Box\" class=\"box\">");

            sectionBoxElement.AppendChild(HtmlAgilityPack.HtmlNode.CreateNode($"<h4>{ name }</h4>"));
            sectionBoxElement.AppendChild(HtmlAgilityPack.HtmlNode.CreateNode($"<p style=\"white-space: pre-line\" name=\"{ name }\">"));

            parentNode.AppendChild(sectionBoxElement);

            return sectionBoxElement;
        }

        /// <summary>   A HtmlAgilityPack.HtmlNode extension method that creates small box. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/31/2021. </remarks>
        ///
        /// <param name="parentNode">   The parent node. </param>
        /// <param name="name">         The name. </param>
        ///
        /// <returns>   The new small box. </returns>

        public static HtmlAgilityPack.HtmlNode CreateSmallBox(this HtmlAgilityPack.HtmlNode parentNode, string name)
        {
            var sectionBoxElement = HtmlAgilityPack.HtmlNode.CreateNode($"<div name=\"{ name }Box\" class=\"smallbox\">");

            sectionBoxElement.AppendChild(HtmlAgilityPack.HtmlNode.CreateNode($"<h4>{ name }</h4>"));
            sectionBoxElement.AppendChild(HtmlAgilityPack.HtmlNode.CreateNode($"<p style=\"white-space: pre-line\" name=\"{ name }\">"));

            parentNode.AppendChild(sectionBoxElement);

            return sectionBoxElement;
        }

        /// <summary>
        /// A List&lt;Type&gt; extension method that searches for the first proxy type.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/9/2020. </remarks>
        ///
        /// <param name="types">    A variable-length parameters list containing types. </param>
        /// <param name="type">     The type to act on. </param>
        ///
        /// <returns>   The found proxy type. </returns>

        public static Type FindProxyType(this List<Type> types, Type type)
        {
            var proxyType = types.SingleOrDefault(t => t.HasCustomAttribute<TypeProxyAttribute>() && t.GetCustomAttribute<TypeProxyAttribute>().ProxiedType == type);

            return proxyType;
        }

        /// <summary>
        /// A CreateInstanceEventHandler extension method that creates an instance.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/29/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="eventHandler"> The eventHandler to act on. </param>
        /// <param name="type">     Name of the type. </param>
        ///
        /// <returns>   The new instance. </returns>

        public static T Invoke<T>(this CreateInstanceEventHandler eventHandler, Type type)
        {
            var args = new CreateInstanceEventArgs(type);

            eventHandler(typeof(Extensions), args);

            return (T)args.Instance;
        }

        /// <summary>
        /// A ResourcesAttribute extension method that handler, called when the create.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>
        ///
        /// <param name="resourcesAttribute">   The resourcesAttribute to act on. </param>
        ///
        /// <returns>   The new handler. </returns>

        public static ResourcesHandler CreateHandler(this ResourcesAttribute resourcesAttribute)
        {
            if (resourcesAttribute.ResourcesType is TypeShim)
            {
                var activator = TypeShimExtensions.GetTypeShimActivator();

                return new ResourcesHandler(activator.CreateInstance<IAppResources>((TypeShim) resourcesAttribute.ResourcesType));
            }
            else
            {
                return new ResourcesHandler((IAppResources)Activator.CreateInstance(resourcesAttribute.ResourcesType));
            }
        }


        /// <summary>   An EntityBaseObject extension method that gets attribute code. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="entityBaseObject"> The entityBaseObject to act on. </param>
        /// <param name="tabIndent">        The tab indent. </param>
        ///
        /// <returns>   The attribute code. </returns>

        public static string GetAttributeCode(this EntityBaseObject entityBaseObject, int tabIndent)
        {
            var listOfAttributeLists = entityBaseObject.GetAttributeLists();

            return listOfAttributeLists.GetAttributeCode(tabIndent);
        }

        /// <summary>   An EntityBaseObject extension method that gets attribute lists. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="entityBaseObject"> The entityBaseObject to act on. </param>
        ///
        /// <returns>   The attribute lists. </returns>

        public static List<List<CustomAttributeData>> GetAttributeLists(this EntityBaseObject entityBaseObject)
        {
            MemberInfo memberInfo;
            var entityType = entityBaseObject.DynamicEntityType;

            switch (entityBaseObject)
            {
                case EntityObject entityObject:
                    memberInfo = entityType;
                    break;
                case AttributeObject attributeObject:
                    memberInfo = entityType.GetProperties().Single(p => p.Name == attributeObject.DynamicPropertyName);
                    break;
                default:
                    DebugUtils.Break();
                    memberInfo = null;
                    break;
            }

            return entityBaseObject.GetAttributeLists(memberInfo);
        }

        /// <summary>   An EntityBaseObject extension method that gets metadata attribute code. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="entityBaseObject"> The entityBaseObject to act on. </param>
        /// <param name="tabIndent">        The tab indent. </param>
        ///
        /// <returns>   The metadata attribute code. </returns>

        public static string GetMetadataAttributeCode(this EntityBaseObject entityBaseObject, int tabIndent)
        {
            var listOfAttributeLists = entityBaseObject.GetMetadataAttributeLists();

            return listOfAttributeLists.GetAttributeCode(tabIndent);
        }

        /// <summary>   An EntityBaseObject extension method that gets metadata attribute lists. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="entityBaseObject"> The entityBaseObject to act on. </param>
        ///
        /// <returns>   The metadata attribute lists. </returns>

        public static List<List<CustomAttributeData>> GetMetadataAttributeLists(this EntityBaseObject entityBaseObject)
        {
            MemberInfo memberInfo;
            var entityMetadataType = entityBaseObject.DynamicEntityMetadataType;

            switch (entityBaseObject)
            {
                case EntityObject entityObject:
                    memberInfo = entityMetadataType;
                    break;
                case AttributeObject attributeObject:
                    memberInfo = entityMetadataType.GetProperties().Single(p => p.Name == attributeObject.DynamicPropertyName);
                    break;
                default:
                    DebugUtils.Break();
                    memberInfo = null;
                    break;
            }

            return entityBaseObject.GetAttributeLists(memberInfo);
        }

        /// <summary>   An EntityBaseObject extension method that gets attribute code. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="listOfAttributeLists"> The listOfLists to act on. </param>
        /// <param name="tabIndent">            The tab indent. </param>
        ///
        /// <returns>   The attribute code. </returns>

        public static string GetAttributeCode(this List<List<CustomAttributeData>> listOfAttributeLists, int tabIndent)
        {
            var builder = new StringBuilder();
            var x = 0;
            var y = 0;
            var lastAttributeIndex = 0;
            var lastArgumentIndex = 0;

            foreach (var list in listOfAttributeLists)
            {
                if (builder.Length > 0)
                {
                    builder.AppendTabIndent(tabIndent, "[");
                }
                else
                {
                    builder.Append("[");
                }

                x = 0;
                lastAttributeIndex = list.Count - 1;

                foreach (var attribute in list)
                {
                    builder.AppendFormat("{0}(", attribute.AttributeType.Name.RemoveEndIfMatches("Attribute"));

                    y = 0;
                    lastArgumentIndex = attribute.ConstructorArguments.Count + attribute.NamedArguments.Count - 1;

                    foreach (var argument in attribute.ConstructorArguments)
                    {
                        var type = argument.ArgumentType;
                        string value = null;

                        switch (Type.GetTypeCode(type))
                        {
                            case TypeCode.String:

                                value = "\"" + argument.Value + "\"";
                                break;

                            case TypeCode.Boolean:

                                value = argument.Value.ToString().ToLower();
                                break;

                            case TypeCode.Int32:

                                if (type.IsEnum)
                                {
                                    value = string.Format("{0}.{1}", type.Name, EnumUtils.GetValue(type, (int) argument.Value));
                                }
                                else
                                {
                                    value = argument.Value.ToString();
                                }

                                break;

                            default:

                                if (type.IsAssignableFrom(typeof(Enum)))
                                {
                                    DebugUtils.Break();
                                }
                                else if (type == typeof(Type))
                                {
                                    value = string.Format("typeof({0})", ((Type) argument.Value).Name);
                                }
                                else
                                {
                                    DebugUtils.Break();
                                }

                                break;
                        }

                        builder.Append(value);

                        if (y != lastArgumentIndex)
                        {
                            builder.Append(", ");
                        }

                        y++;
                    }

                    foreach (var argument in attribute.NamedArguments)
                    {
                        var memberInfo = argument.MemberInfo;
                        Type type;
                        string value = null;

                        switch (memberInfo)
                        {
                            case PropertyInfo propertyInfo:
                                type = propertyInfo.PropertyType;
                                break;
                            case FieldInfo fieldInfo:
                                type = fieldInfo.FieldType;
                                break;
                            default:
                                DebugUtils.Break();
                                type = null;
                                break;
                        }

                        switch (Type.GetTypeCode(type))
                        {
                            case TypeCode.String:

                                value = string.Format("{0}={1}", argument.MemberName, "\"" + argument.TypedValue.Value + "\"");
                                break;

                            case TypeCode.Boolean:

                                value = string.Format("{0}={1}", argument.MemberName, argument.TypedValue.Value.ToString().ToLower());
                                break;

                            case TypeCode.Int32:

                                if (type.IsEnum)
                                {
                                    value = string.Format("{0}={1}.{2}", argument.MemberName, type.Name, EnumUtils.GetValue(type, (int)argument.TypedValue.Value));
                                }
                                else
                                {
                                    value = string.Format("{0}={1}", argument.MemberName, argument.TypedValue.Value.ToString());
                                }

                                break;

                            default:

                                if (type.IsAssignableFrom(typeof(Enum)))
                                {
                                    DebugUtils.Break();
                                }
                                else if (type == typeof(Type))
                                {
                                    DebugUtils.Break();
                                }
                                else
                                {
                                    DebugUtils.Break();
                                }

                                break;
                        }

                        builder.Append(value);

                        if (y != lastArgumentIndex)
                        {
                            builder.Append(", ");
                        }

                        y++;
                    }

                    builder.Append(")");

                    if (x != lastAttributeIndex)
                    {
                        builder.Append(", ");
                    }

                    x++;
                }

                builder.AppendLine("]");
            }

            if (builder.Length > 2)
            {
                builder.RemoveEnd(2);
            }

            return builder.ToString();
        }

        /// <summary>   An EntityBaseObject extension method that gets attribute lists. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="entityBaseObject"> The entityBaseObject to act on. </param>
        /// <param name="memberInfo">       The type to act on. </param>
        ///
        /// <returns>   The attribute lists. </returns>

        public static List<List<CustomAttributeData>> GetAttributeLists(this EntityBaseObject entityBaseObject, MemberInfo memberInfo)
        {
            var listOfLists = new List<List<CustomAttributeData>>();
            var descendantProperties = entityBaseObject.GetDescendantProperties().ToList();
            var entityMarkerUIGroups = descendantProperties.OfType<EntityMarkerUIGroup>();
            var uiHierarchyPathProperties = descendantProperties.Where(p => p.PropertyName == "UIHierarchyPath").ToList();
            var uiKindProperties = descendantProperties.Where(p => p.PropertyName.IsOneOf("UIKind", "UILoadKind")).ToList();
            var attributes = memberInfo.CustomAttributes;
            var listedAttributes = new List<CustomAttributeData>();
            var propertiesProcessed = new List<EntityPropertyItem>();
            var remainingList = new List<CustomAttributeData>();

            foreach (var entityMarkerUIGroup in entityMarkerUIGroups)
            {
                var matchingHierarchyPathProperties = uiHierarchyPathProperties.Where(p => p.PropertyValue == entityMarkerUIGroup.UIPath);
                var matchingUIKindProperties = uiKindProperties.Where(p => p.PropertyValue == entityMarkerUIGroup.UIKind.ToString());
                var list = new List<CustomAttributeData>();
                IEnumerable<CustomAttributeData> uiAttributes;

                foreach (var matchingHierarchyPathProperty in matchingHierarchyPathProperties.ToList())
                {
                    var topLevelProperty = matchingHierarchyPathProperty.GetAncestorProperties(entityBaseObject).Last();
                    var uiKindProperty = topLevelProperty.GetDescendantProperties().SingleOrDefault(p => p.PropertyName == "UIKind");
                    var uiLoadKindProperty = topLevelProperty.GetDescendantProperties().SingleOrDefault(p => p.PropertyName == "UILoadKind");

                    switch (topLevelProperty.PropertyName)
                    {
                        case "UI":

                            uiAttributes = attributes.Where(a => a.AttributeType == typeof(UIAttribute) && a.MatchesArguments(matchingHierarchyPathProperty, uiKindProperty, uiLoadKindProperty));

                            listedAttributes.AddRange(uiAttributes);
                            list.AddRange(uiAttributes);
                            break;

                        case "GridColumn":

                            uiAttributes = attributes.Where(a => a.AttributeType == typeof(GridColumnAttribute) && a.MatchesArguments(matchingHierarchyPathProperty));

                            listedAttributes.AddRange(uiAttributes);
                            list.AddRange(uiAttributes);
                            break;

                        case "FormField":

                            uiAttributes = attributes.Where(a => a.AttributeType == typeof(FormFieldAttribute) && a.MatchesArguments(matchingHierarchyPathProperty));

                            listedAttributes.AddRange(uiAttributes);
                            list.AddRange(uiAttributes);
                            break;

                        default:
                            DebugUtils.Break();
                            break;
                    }

                    propertiesProcessed.Add(topLevelProperty);
                    uiHierarchyPathProperties.Remove(matchingHierarchyPathProperty);
                }

                foreach (var matchingUIKindProperty in matchingUIKindProperties.ToList())
                {
                    var topLevelProperty = matchingUIKindProperty.GetAncestorProperties(entityBaseObject).Last();

                    if (!propertiesProcessed.Contains(topLevelProperty))
                    {
                        var uiLoadKindProperty = topLevelProperty.GetDescendantProperties().SingleOrDefault(p => p.PropertyName == "UILoadKind");

                        switch (topLevelProperty.PropertyName)
                        {
                            case "UI":

                                uiAttributes = attributes.Where(a => a.AttributeType == typeof(UIAttribute) && a.MatchesArguments(matchingUIKindProperty, uiLoadKindProperty));

                                listedAttributes.AddRange(uiAttributes);
                                list.AddRange(uiAttributes);
                                break;


                            case "UINavigationName":

                                var nameProperty = topLevelProperty.GetDescendantProperties().SingleOrDefault(p => p.PropertyName == "Name");
                                
                                uiAttributes = attributes.Where(a => a.AttributeType == typeof(UINavigationNameAttribute) && a.MatchesArguments(matchingUIKindProperty, nameProperty, uiLoadKindProperty));

                                listedAttributes.AddRange(uiAttributes);
                                list.AddRange(uiAttributes);
                                break;

                            case "Authorize":

                                var stateProperty = topLevelProperty.GetDescendantProperties().SingleOrDefault(p => p.PropertyName == "AuthorizationState");
                                
                                uiAttributes = attributes.Where(a => a.AttributeType == typeof(AuthorizeAttribute) && a.MatchesArguments(matchingUIKindProperty, stateProperty, uiLoadKindProperty));

                                listedAttributes.AddRange(uiAttributes);
                                list.AddRange(uiAttributes);
                                break;

                            default:
                                DebugUtils.Break();
                                break;
                        }
                    }
                }

                if (list.Count > 0)
                {
                    listOfLists.Add(list);
                }
            }

            foreach (var attribute in attributes.Where(a => !listedAttributes.Any(a2 => a2 == a)))
            {
                remainingList.Add(attribute);
            }

            if (remainingList.Count > 0)
            {
                listOfLists.Add(remainingList);
            }

            return listOfLists;
        }

        /// <summary>   A CustomAttributeData extension method that matches arguments. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="customAttributeData">  The customAttributeData to act on. </param>
        /// <param name="args">                 A variable-length parameters list containing arguments. </param>
        ///
        /// <returns>   True if matches arguments, false if not. </returns>

        public static bool MatchesArguments(this CustomAttributeData customAttributeData, params EntityPropertyItem[] args)
        {
            var constructor = customAttributeData.Constructor;
            var x = 0;

            args = args.OfType<EntityPropertyItem>().ToArray();

            foreach (var parm in constructor.GetParameters())
            {
                if (args.Any(a => a.PropertyName.Replace("UI", "ui").ToCamelCase() == parm.Name))
                {
                    var property = args.Single(a => a.PropertyName.Replace("UI", "ui").ToCamelCase() == parm.Name);
                    var propertyName = property.PropertyName;
                    var propertyValue = property.PropertyValue;
                    var argValue = customAttributeData.ConstructorArguments[x].Value;
                    var type = parm.ParameterType;
                    object value = null;

                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.String:

                            value = propertyValue;
                            break;

                        case TypeCode.Boolean:

                            value = bool.Parse(propertyValue);
                            break;

                        case TypeCode.Int32:

                            if (type.IsEnum)
                            {
                                if (propertyName == type.Name)
                                {
                                    value = (int) EnumUtils.GetValue(type, propertyValue);
                                }
                                else
                                {
                                    DebugUtils.Break();
                                }
                            }
                            else
                            {
                                value = int.Parse(propertyValue);
                            }

                            break;

                        default:

                            if (type.IsAssignableFrom(typeof(Enum)))
                            {
                                if (propertyName == type.Name)
                                {
                                    value = EnumUtils.GetValue(type, propertyValue);
                                }
                                else
                                {
                                    DebugUtils.Break();
                                }
                            }
                            else if (type == typeof(Type))
                            {
                            }
                            else
                            {
                                DebugUtils.Break();
                            }

                            break;
                    }

                    if (!argValue.Equals(value))
                    {
                        return false;
                    }
                }
                else if (!parm.HasDefaultValue)
                {
                    return false;
                }

                x++;
            }

            return true;
        }

        /// <summary>
        /// An EntityPropertyItem extension method that query if 'entityPropertyItem' is marker property.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="entityPropertyItem">   The entityPropertyItem to act on. </param>
        ///
        /// <returns>   True if marker property, false if not. </returns>

        public static bool IsMarkerProperty(this EntityPropertyItem entityPropertyItem)
        {
            return typeof(EntityMarkerPropertyItem).IsAssignableFrom(entityPropertyItem.GetType());
        }

        /// <summary>
        /// An UIHierarchyNodeObject extension method that query if 'hierarchyNodeObject' contains entity.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="hierarchyNodeObject">  The hierarchyNodeObject to act on. </param>
        /// <param name="entityObject">         The entityObject to act on. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool ContainsEntity(this UIHierarchyNodeObject hierarchyNodeObject, EntityObject entityObject)
        {
            if (hierarchyNodeObject.Entities.Contains(entityObject))
            {
                return true;
            }
            else if (entityObject.ShadowOfEntity != null)
            {
                entityObject = entityObject.ShadowOfEntity;

                return hierarchyNodeObject.ContainsEntity(entityObject);
            }

            return false;
        }

        /// <summary>
        /// An UIHierarchyNodeObject extension method that query if 'hierarchyNodeObject' contains entity.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="hierarchyNodeObject">      The hierarchyNodeObject to act on. </param>
        /// <param name="entityObject">             The entityObject to act on. </param>
        /// <param name="appUIHierarchyNodeObject"> . </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool ContainsEntity(this UIHierarchyNodeObject hierarchyNodeObject, EntityObject entityObject, AppUIHierarchyNodeObject appUIHierarchyNodeObject)
        {
            if (hierarchyNodeObject.Entities.Contains(entityObject))
            {
                return true;
            }
            else if (entityObject.ShadowOfEntity != null)
            {
                entityObject = entityObject.ShadowOfEntity;

                return hierarchyNodeObject.ContainsEntity(entityObject);
            }
            else
            {
                if (hierarchyNodeObject.Entities.Any(e => appUIHierarchyNodeObject.AllEntities.Any(e2 => e.ShadowOfEntity == e2)))
                {
                    foreach (var shadowOfEntity in hierarchyNodeObject.Entities.Where(e => appUIHierarchyNodeObject.AllEntities.Any(e2 => e.ShadowOfEntity == e2)))
                    {
                        if (hierarchyNodeObject.ContainsEntity(shadowOfEntity))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// An EntityPropertyItem extension method that query if 'entityPropertyItem' is matching
        /// property.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="entityPropertyItem">       The entityPropertyItem to act on. </param>
        /// <param name="identityFieldKind">        The identity field kind. </param>
        /// <param name="uIHierarchyNodeObject">    The UI hierarchy node object. </param>
        ///
        /// <returns>   True if matching property, false if not. </returns>

        public static bool IsMatchingProperty(this EntityPropertyItem entityPropertyItem, IdentityFieldKind identityFieldKind, UIHierarchyNodeObject uIHierarchyNodeObject)
        {
            if (entityPropertyItem.PropertyName == "UIHierarchyPath")
            {
                switch (entityPropertyItem.PropertyValue)
                {
                    case "/Login":
                        return  (identityFieldKind == IdentityFieldKind.UserName || identityFieldKind == IdentityFieldKind.PasswordHash) && uIHierarchyNodeObject.ShadowItem == 0;
                    case "/Register":
                        DebugUtils.Break();  // todo
                        return false;
                    default:
                        break;
                }

            }

            return false;
        }

        /// <summary>
        /// An UIHierarchyNodeObject extension method that searches for the first entity container
        /// attribute.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="appUIHierarchyNodeObject"> . </param>
        /// <param name="hierarchyNode">            The hierarchyNode to act on. </param>
        /// <param name="entityObject">             The entityObject to act on. </param>
        ///
        /// <returns>   The found entity container attribute. </returns>

        public static AttributeObject FindEntityContainerAttribute(this AppUIHierarchyNodeObject appUIHierarchyNodeObject, UIHierarchyNodeObject hierarchyNode, EntityObject entityObject)
        {
            var ancestors = hierarchyNode.GetAncestors();
            var regex = new Regex(string.Format(@"EntitySet\[@Entity='{0}'\]", entityObject.Name));

            foreach (var ancestor in ancestors)
            {
                foreach (var entity in ancestor.Entities)
                {
                    if (entity.Properties.Any(p => p.PropertyName == "RelatedEntity"))
                    {
                        // todo
                        DebugUtils.Break();
                    }
                }
            }

            foreach (var attribute in appUIHierarchyNodeObject.EntityContainer.Attributes)
            {
                if (regex.IsMatch(attribute.AttributeType))
                {
                    return attribute;
                }
            }

            return null;
        }

        /// <summary>
        /// A Dictionary&lt;string,string&gt; extension method that handles the text replacements.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/7/2020. </remarks>
        ///
        /// <param name="textReplacements"> The textReplacements to act on. </param>
        /// <param name="str">              The string. </param>
        ///
        /// <returns>   A string. </returns>

        public static string HandleTextReplacements(this Dictionary<string, string> textReplacements, string str)
        {
            foreach (var pair in textReplacements)
            {
                str = str.Replace(pair.Key, pair.Value);
            }

            return str;
        }

        /// <summary>   A Type extension method that generates a code. </summary>
        ///
        /// <remarks>   Ken, 10/6/2020. </remarks>
        ///
        /// <param name="entityType">       . </param>
        /// <param name="metadataType">     The type to act on. </param>
        /// <param name="moduleBuilder">    The module builder. </param>
        ///
        /// <returns>   The code. </returns>

        public static string GenerateCode(this Type entityType, Type metadataType, ModuleBuilder moduleBuilder)
        {
            CodeTypeDeclaration entityClass;
            CodeTypeDeclaration metaDataClass;
            ICodeGenerator codeGenerator;
            CodeCompileUnit unit;

            entityClass = entityType.GetCodeTypeDeclaration(moduleBuilder, metadataType.Name);
            metaDataClass = metadataType.GetCodeTypeDeclaration(moduleBuilder);

            unit = new CodeCompileUnit
            {
                Namespaces =
                {
                    new CodeNamespace(metadataType.Namespace)
                    {
                        Imports =
                        {
                            new CodeNamespaceImport("System"),
                            new CodeNamespaceImport("System.Collections.Generic"),
                            new CodeNamespaceImport("AbstraX.DataAnnotations"),
                            new CodeNamespaceImport("System.ComponentModel.DataAnnotations"),
                            new CodeNamespaceImport("System.ComponentModel")
                        },
                        Types =
                        {
                            entityClass,
                            metaDataClass
                        }
                    }
                }
            };

            codeGenerator = new CSharpCodeProvider().CreateGenerator();

            using (var writer = new StringWriter())
            {
                codeGenerator.GenerateCodeFromCompileUnit(unit, writer, new CodeGeneratorOptions
                {
                    BracingStyle = "C"
                });
                return writer.ToString().Replace("{ get; set; };", "{ get; set; }");
            }
        }

        /// <summary>   A Type extension method that gets code type declaration. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>
        ///
        /// <param name="type">             The type to act on. </param>
        /// <param name="moduleBuilder">    The module builder. </param>
        /// <param name="metadataType">     (Optional) The type to act on. </param>
        ///
        /// <returns>   The code type declaration. </returns>

        private static CodeTypeDeclaration GetCodeTypeDeclaration(this Type type, ModuleBuilder moduleBuilder, string metadataType = null)
        {
            var newClass = new CodeTypeDeclaration(type.Name)
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public,
                IsPartial = true
            };

            foreach (var attribute in type.GetCustomAttributesData())
            {
                if (attribute.AttributeType.Name == "MetadataTypeAttribute")
                {
                    newClass.CustomAttributes.Add(new CodeAttributeDeclaration(attribute.AttributeType.Name, new CodeAttributeArgument(new CodeSnippetExpression(string.Format("typeof({0})", metadataType)))));
                }
                else if (attribute.AttributeType.Name == "ResourcesAttribute")
                {
                    newClass.CustomAttributes.Add(new CodeAttributeDeclaration(attribute.AttributeType.Name, new CodeAttributeArgument(new CodeSnippetExpression("typeof(AppResources)"))));
                }
                else if (attribute.AttributeType.Name == "CustomQueryAttribute")
                {
                    var ctorParams = attribute.GetPrivateFieldValue<IEnumerable>("m_ctorParams").Cast<object>();
                    var typeParm = ctorParams.ElementAt(0);
                    var controllerMethodNameParm = ctorParams.ElementAt(1);
                    var queryKindParm = ctorParams.ElementAt(2);
                    var typeEncodedArgument = typeParm.GetPrivateFieldValue<object>("m_encodedArgument");
                    var controllerMethodNameEncodedArgument = controllerMethodNameParm.GetPrivateFieldValue<object>("m_encodedArgument");
                    var queryKindEncodedArgument = queryKindParm.GetPrivateFieldValue<object>("m_encodedArgument");
                    var typeName = typeEncodedArgument.GetPropertyValue<string>("StringValue");
                    var controllerMethodName = controllerMethodNameEncodedArgument.GetPropertyValue<string>("StringValue");
                    var queryKind = (QueryKind) queryKindEncodedArgument.GetPropertyValue<long>("PrimitiveValue");

                    newClass.CustomAttributes.Add(new CodeAttributeDeclaration(attribute.AttributeType.Name, new CodeAttributeArgument[]
                    {
                        new CodeAttributeArgument(new CodeSnippetExpression("typeof(AppResources)")),
                        new CodeAttributeArgument(new CodeSnippetExpression(controllerMethodName.SurroundWithQuotes())),
                        new CodeAttributeArgument(new CodeSnippetExpression("QueryKind." + queryKind.ToString()))
                    }));
                }
                else
                {
                    newClass.CustomAttributes.Add(new CodeAttributeDeclaration(attribute.AttributeType.Name, attribute.ConstructorArguments.Select(a => new CodeAttributeArgument(new CodeSnippetExpression(a.GetText()))).ToArray()));
                }
            }

            if (type.IsGenericType)
            {
                foreach (var genericArgumentType in type.GetGenericArguments())
                {
                    newClass.TypeParameters.Add(genericArgumentType.Name);
                }
            }

            foreach (var propertyInfo in type.GetProperties())
            {
                CodeMemberField property;

                // this is a hack to get auto properties

                try
                {
                    property = new CodeMemberField
                    {
                        Name = propertyInfo.Name + " { get; set; }",
                        Type = propertyInfo.PropertyType.MakeTypeReference(),
                        Attributes = MemberAttributes.Public,
                    };
                }
                catch (TypeLoadException ex)
                {
                    var typeName = ex.TypeName;
                    var propertyType = moduleBuilder.GetTypes().Single(t => t.FullName == typeName);

                    property = new CodeMemberField
                    {
                        Name = propertyInfo.Name + " { get; set; }",
                        Type = propertyType.MakeTypeReference(),
                        Attributes = MemberAttributes.Public,
                    };
                }

                foreach (var attribute in propertyInfo.GetCustomAttributesData())
                {
                    property.CustomAttributes.Add(new CodeAttributeDeclaration(attribute.AttributeType.Name, attribute.ConstructorArguments.Select(a => new CodeAttributeArgument(new CodeSnippetExpression(a.GetText()))).ToArray()));
                }

                newClass.Members.Add(property);
            }

            return newClass;
        }

        /// <summary>   An XPathOperator extension method that gets a text. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>
        ///
        /// <param name="argument"> The argument. </param>
        ///
        /// <returns>   The text. </returns>

        private static string GetText(this System.Reflection.CustomAttributeTypedArgument argument)
        {
            var type = argument.ArgumentType;
            var value = "null";

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:

                    if (argument.Value != null)
                    {
                        value = "\"" + argument.Value.ToString() + "\"";
                    }

                    break;

                case TypeCode.Boolean:

                    value = argument.Value.ToString().ToLower();
                    break;

                case TypeCode.Int32:

                    if (type.IsEnum)
                    {
                        value = type.Name + "." + Enum.GetName(type, argument.Value);
                    }
                    else
                    {
                        value = argument.Value.ToString();
                    }

                    break;

                default:

                    if (type.IsAssignableFrom(typeof(Enum)))
                    {
                        value = type.Name + "." + Enum.GetName(type, argument.Value);
                    }
                    else if (type == typeof(Type))
                    {

                    }
                    else
                    {
                        DebugUtils.Break();
                    }

                    break;
            }

            return value;
        }

        /// <summary>   A Type extension method that makes type reference. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>
        ///
        /// <param name="type"> The type to act on. </param>
        ///
        /// <returns>   A CodeTypeReference. </returns>

        private static CodeTypeReference MakeTypeReference(this Type type)
        {
            if (!type.IsGenericType)
            {
                return new CodeTypeReference(type);
            }
            else
            {
                return new CodeTypeReference(type.Name, type.GetGenericArguments().Select(MakeTypeReference).ToArray());
            }
        }

        /// <summary>   Gets the related entity attributes in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="entityObject"> The entityObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the related entity attributes in this
        /// collection.
        /// </returns>

        public static IEnumerable<AttributeObject> GetRelatedEntityAttributes(this EntityObject entityObject)
        {
            return entityObject.Attributes.Where(a => a.Properties.Any(p => p.PropertyName == "RelatedEntity"));
        }

        /// <summary>
        /// An EntityObject extension method that query if 'entityObject' has related entity attributes.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="entityObject"> The entityObject to act on. </param>
        ///
        /// <returns>   True if related entity attributes, false if not. </returns>

        public static bool HasRelatedEntityAttributes(this EntityObject entityObject)
        {
            return entityObject.Attributes.Any(a => a.Properties != null && a.Properties.Any(p => p.PropertyName == "RelatedEntity"));
        }

        /// <summary>
        /// An EntityObject extension method that query if 'entityObject' has identity kind.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="entityObject">         The entityObject to act on. </param>
        /// <param name="identityEntityKind">   The identity kind. </param>
        ///
        /// <returns>   True if identity kind, false if not. </returns>

        public static bool HasIdentityEntityKind(this EntityObject entityObject, IdentityEntityKind identityEntityKind)
        {
            return entityObject.Properties.Any(p => p.PropertyName == "IdentityEntity") && entityObject.Properties.Single(p => p.PropertyName == "IdentityEntity").ChildProperties.Any(p2 => p2.PropertyName == "IdentityEntityKind" && p2.PropertyValue == identityEntityKind.ToString());
        }

        /// <summary>
        /// An EntityObject extension method that query if 'entityObject' has identify field kind.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="attributeObject">      The entityObject to act on. </param>
        /// <param name="identityFieldKind">    The identity field kind. </param>
        ///
        /// <returns>   True if identify entity kind, false if not. </returns>

        public static bool HasIdentityFieldKind(this AttributeObject attributeObject, IdentityFieldKind identityFieldKind)
        {
            return attributeObject.Properties.Any(p => p.PropertyName == "IdentityEntity") && attributeObject.Properties.Single(p => p.PropertyName == "IdentityField").ChildProperties.Any(p2 => p2.PropertyName == "IdentityFieldKind" && p2.PropertyValue == identityFieldKind.ToString());
        }

        /// <summary>   An EntityObject extension method that shadows the given entity object. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="entityObject"> The entityObject to act on. </param>
        ///
        /// <returns>   An EntityObject. </returns>

        public static EntityObject Shadow(this EntityObject entityObject)
        {
            var shadow = entityObject.CreateCopy<EntityObject>();

            shadow.ShadowOfEntity = entityObject;

            return shadow;
        }

        /// <summary>   Gets the descendants in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="hierarchyNodeObject">  The hierarchyNodeObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendants in this collection.
        /// </returns>

        public static IEnumerable<UIHierarchyNodeObject> GetDescendants(this UIHierarchyNodeObject hierarchyNodeObject)
        {
            var descendants = new List<UIHierarchyNodeObject>();

            hierarchyNodeObject.GetDescendants(o => o.Children, o =>
            {
                descendants.Add(o);
            });

            return descendants;
        }

        /// <summary>   Gets the descendants in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/7/2020. </remarks>
        ///
        /// <param name="appUIHierarchyNodeObject"> The appUIHierarchyNodeObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendants in this collection.
        /// </returns>

        public static IEnumerable<UIHierarchyNodeObject> GetDescendants(this AppUIHierarchyNodeObject appUIHierarchyNodeObject)
        {
            var descendants = new List<UIHierarchyNodeObject>();

            foreach (var hierarchyNodeObject in appUIHierarchyNodeObject.TopUIHierarchyNodeObjects)
            {
                descendants.Add(hierarchyNodeObject);

                hierarchyNodeObject.GetDescendants(o => o.Children, o =>
                {
                    descendants.Add(o);
                });
            }

            return descendants;
        }

        /// <summary>
        /// An AppUIHierarchyNodeObject extension method that debug prints user interface hierarchy.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/7/2020. </remarks>
        ///
        /// <param name="appUIHierarchyNodeObject"> The appUIHierarchyNodeObject to act on. </param>
        ///
        /// <returns>   A string. </returns>

        public static string DebugPrintUIHierarchy(this AppUIHierarchyNodeObject appUIHierarchyNodeObject)
        {
            var builder = new StringBuilder();

            foreach (var hierarchyNodeObject in appUIHierarchyNodeObject.TopUIHierarchyNodeObjects)
            {
                builder.AppendLine(hierarchyNodeObject.Name);

                hierarchyNodeObject.GetDescendants(o => o.Children, (o, l) =>
                {
                    builder.AppendLineTabIndent(l, o.Name);

                    foreach (var entity in o.Entities)
                    {
                        builder.AppendLineFormatTabIndent(l + 1, "Entity: {0}", entity.Name);
                    }
                });
            }

            return builder.ToString();
        }

        /// <summary>   Gets the descendants and selfs in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="hierarchyNodeObject">  The hierarchyNodeObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendants and selfs in this
        /// collection.
        /// </returns>

        public static IEnumerable<UIHierarchyNodeObject> GetDescendantsAndSelf(this UIHierarchyNodeObject hierarchyNodeObject)
        {
            var descendants = new List<UIHierarchyNodeObject>();

            hierarchyNodeObject.GetDescendantsAndSelf(o => o.Children, o =>
            {
                descendants.Add(o);
            });

            return descendants;
        }

        /// <summary>   Gets the ancestors in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="hierarchyNodeObject">  The hierarchyNodeObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the ancestors in this collection.
        /// </returns>

        public static IEnumerable<UIHierarchyNodeObject> GetAncestors(this UIHierarchyNodeObject hierarchyNodeObject)
        {
            var ancestors = new List<UIHierarchyNodeObject>();

            hierarchyNodeObject = hierarchyNodeObject.Parent;

            while (hierarchyNodeObject != null)
            {
                ancestors.Add(hierarchyNodeObject);

                hierarchyNodeObject = hierarchyNodeObject.Parent;
            }

            return ancestors;
        }

        /// <summary>   Gets the ancestors and self in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="hierarchyNodeObject">  The hierarchyNodeObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the ancestors and selfs in this
        /// collection.
        /// </returns>

        public static IEnumerable<UIHierarchyNodeObject> GetAncestorsAndSelf(this UIHierarchyNodeObject hierarchyNodeObject)
        {
            var ancestorsAndSelf = new List<UIHierarchyNodeObject>();

            while (hierarchyNodeObject != null)
            {
                ancestorsAndSelf.Add(hierarchyNodeObject);

                hierarchyNodeObject = hierarchyNodeObject.Parent;
            }

            return ancestorsAndSelf;
        }

        /// <summary>   An UIHierarchyNodeObject extension method that gets user interface path. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="hierarchyNodeObject">      The hierarchyNodeObject to act on. </param>
        ///
        /// <returns>   The user interface path. </returns>

        public static string GetUIPath(this UIHierarchyNodeObject hierarchyNodeObject)
        {
            var ancestors = hierarchyNodeObject.GetAncestorsAndSelf();
            var path = string.Empty;
            var topLevel = ancestors.Last();

            if (hierarchyNodeObject.ConstructedUIPath != null)
            {
                return hierarchyNodeObject.ConstructedUIPath;
            }

            foreach (var ancestor in ancestors.Reverse())
            {
                var hasSpaces = false;
                var name = string.Empty;

                if (ancestor.Name.Contains(" "))
                {
                    hasSpaces = true;
                    name = ancestor.Name.RemoveText(" ");
                }
                else
                {
                    name = ancestor.Name;
                }

                path += "/" + name;

                if (ancestor.UserRoles != null && ancestor.UserRoles.Count > 0)
                {
                    StringBuilder builder;

                    if (hasSpaces)
                    {
                        builder = new StringBuilder($"[@Name='{ ancestor.Name }' and @RoleView='");
                    }
                    else
                    {
                        builder = new StringBuilder("[@RoleView='");
                    }

                    foreach (var userRole in ancestor.UserRoles)
                    {
                        builder.AppendFormat("{0},", userRole);
                    }

                    builder.RemoveEnd(1);
                    builder.Append("']");

                    path += builder.ToString();
                }
                else if (hasSpaces)
                {
                    path += $"[@Name='{ ancestor.Name }']";
                }
            }

            return path;
        }

        /// <summary>   An UIHierarchyNodeObject extension method that gets user interface path. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/15/2020. </remarks>
        ///
        /// <param name="attributeObject">  The entityObject to act on. </param>
        ///
        /// <returns>   The user interface path. </returns>

        public static string GetUIPathSegment(this AttributeObject attributeObject)
        {
            var hasSpaces = false;
            var name = string.Empty;
            var path = string.Empty;

            if (attributeObject.Name.Contains(" "))
            {
                hasSpaces = true;
                name = attributeObject.Name.RemoveText(" ");
            }
            else
            {
                name = attributeObject.Name;
            }

            path = name;

            if (hasSpaces)
            {
                path += $"[@Name='{ attributeObject.Name }']";
            }

            return path;
        }

        /// <summary>   Gets the descendants in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="businessModel">    The business model. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendants in this collection.
        /// </returns>

        public static IEnumerable<BusinessModelObject> GetDescendants(this BusinessModel businessModel)
        {
            var descendants = new List<BusinessModelObject>();
            var businessModelObject = businessModel.TopLevelObject;

            businessModelObject.GetDescendantsAndSelf(o => o.Children, o =>
            {
                descendants.Add(o);
            });

            return descendants;
        }

        /// <summary>   Gets the descendants in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="businessModelObject">  The businessModelObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendants in this collection.
        /// </returns>

        public static IEnumerable<BusinessModelObject> GetDescendants(this BusinessModelObject businessModelObject)
        {
            var descendants = new List<BusinessModelObject>();

            businessModelObject.GetDescendants(o => o.Children, o =>
            {
                descendants.Add(o);
            });

            return descendants;
        }

        /// <summary>   Gets the descendants and selfs in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="businessModelObject">  The businessModelObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendants and selfs in this
        /// collection.
        /// </returns>

        public static IEnumerable<BusinessModelObject> GetDescendantsAndSelf(this BusinessModelObject businessModelObject)
        {
            var descendants = new List<BusinessModelObject>();

            businessModelObject.GetDescendantsAndSelf(o => o.Children, o =>
            {
                descendants.Add(o);
            });

            return descendants;
        }

        /// <summary>   Gets the ancestors in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="businessModelObject">  The businessModelObject to act on. </param>
        /// <param name="businessModel">        The business model. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the ancestors in this collection.
        /// </returns>

        public static IEnumerable<BusinessModelObject> GetAncestors(this BusinessModelObject businessModelObject, BusinessModel businessModel)
        {
            var ancestors = new List<BusinessModelObject>();
            var parentId = businessModelObject.ParentId;
            var allBusinessModelObjects = businessModel.GetDescendants();

            while (parentId != 0)
            {
                businessModelObject = allBusinessModelObjects.Single(o => o.Id == parentId);

                ancestors.Add(businessModelObject);

                parentId = businessModelObject.ParentId;
            }

            return ancestors;
        }

        /// <summary>   Gets the descendant properties in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="entityBaseObject"> The entityBaseObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendant properties in this
        /// collection.
        /// </returns>

        public static IEnumerable<EntityPropertyItem> GetDescendantProperties(this EntityBaseObject entityBaseObject)
        {
            var descendants = new List<EntityPropertyItem>();
            var properties = entityBaseObject.Properties;

            foreach (var property in properties)
            {
                foreach (var property2 in property.GetDescendantPropertiesAndSelf())
                {
                    descendants.Add(property2);
                }
            }

            return descendants;
        }

        /// <summary>   Gets the descendant properties in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="entityPropertyItem">   The entityPropertyItem to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendant properties in this
        /// collection.
        /// </returns>

        public static IEnumerable<EntityPropertyItem> GetDescendantProperties(this EntityPropertyItem entityPropertyItem)
        {
            var descendants = new List<EntityPropertyItem>();

            entityPropertyItem.GetDescendants(p => p.ChildProperties, (p) =>
            {
                descendants.Add(p);
            });

            return descendants;
        }

        /// <summary>   Gets the descendant properties and selfs in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="entityPropertyItem">   The entityPropertyItem to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendant properties and selfs
        /// in this collection.
        /// </returns>

        public static IEnumerable<EntityPropertyItem> GetDescendantPropertiesAndSelf(this EntityPropertyItem entityPropertyItem)
        {
            var descendants = new List<EntityPropertyItem>();

            entityPropertyItem.GetDescendantsAndSelf(p => p.ChildProperties, (p) =>
            {
                descendants.Add(p);
            });

            return descendants;
        }

        /// <summary>   Gets the ancestor properties in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="entityPropertyItem">   The entityPropertyItem to act on. </param>
        /// <param name="entityBaseObject">     The entityBaseObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the ancestor properties in this
        /// collection.
        /// </returns>

        public static IEnumerable<EntityPropertyItem> GetAncestorProperties(this EntityPropertyItem entityPropertyItem, EntityBaseObject entityBaseObject)
        {
            var ancestors = new List<EntityPropertyItem>();
            var allEntityProperties = entityBaseObject.GetDescendantProperties();

            while (entityPropertyItem != null)
            {
                entityPropertyItem = allEntityProperties.SingleOrDefault(i => i.ChildProperties != null && i.ChildProperties.Contains(entityPropertyItem));

                if (entityPropertyItem != null)
                {
                    ancestors.Add(entityPropertyItem);
                }
            }

            return ancestors;
        }

        /// <summary>   A DirectoryInfo extension method that backups the given directory. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="directory">    The directory to act on. </param>

        public static void Backup(this DirectoryInfo directory)
        {
            var backupRetentionDays = int.Parse(ConfigurationSettings.AppSettings["BackupRetentionDays"]);
            var backupRetentionCount = int.Parse(ConfigurationSettings.AppSettings["BackupRetentionCount"]);
            var count = 1;
            var backupDirectory = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"HydraProjects\Backup"));
            var zipName = Path.Combine(backupDirectory.FullName, directory.Name + DateTime.Now.ToSortableDateTimeText() + ".zip");
            var exclusions = new List<string>
            {
                "debug",
                "debugPublic",
                "release",
                "releases",
                "x64",
                "x86",
                "arm",
                "arm64",
                "bld",
                "bin",
                "obj",
                "log",
                "logs",
                "node_modules",
                ".dll",
                ".exe",
                ".zip"
            };

            if (backupDirectory.Exists)
            {
                backupDirectory.GetFiles("*.zip", SearchOption.TopDirectoryOnly).OrderByDescending(f => f.Name).ToList().ForEach(f =>
                {
                    DateTime dateTime;

                    if (DateTimeExtensions.IsSortableDateTimeText(f.FullName, out dateTime))
                    {
                        if (count < backupRetentionCount)
                        {
                            if (DateTime.Now - dateTime > TimeSpan.FromDays(backupRetentionDays))
                            {
                                try
                                {
                                    f.MakeWritable();
                                    f.Delete();
                                }
                                catch
                                {
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                f.MakeWritable();
                                f.Delete();
                            }
                            catch
                            {
                            }
                        }
                    }

                    count++;
                });
            }
            else
            {
                backupDirectory.Create();
            }

            directory.ToZipFile(zipName, (f) =>
            {
                if (f.Directory.GetParts().Any(p => exclusions.Where(e => !e.StartsWith(".")).Any(e => e.AsCaseless() == p)))
                {
                    return false;
                }
                else if (exclusions.Where(e => e.StartsWith(".")).Any(e => e == f.Extension))
                {
                    return false;
                }

                return true;
            });
        }

        /// <summary>   A GetOverridesEventHandler extension method that gets override identifier. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handler">      The handler to act on. </param>
        /// <param name="baseObject">   The base object. </param>
        /// <param name="predicate">    The predicate. </param>
        /// <param name="generatedId">  Identifier for the generated. </param>
        ///
        /// <returns>   The override identifier. </returns>

        public static string GetOverrideId(this GetOverridesEventHandler handler, IBase baseObject, string predicate, string generatedId)
        {
            var overrideEventHandler = handler;
            var args = new GetOverridesEventArgs();
            IGeneratorOverrides generatorOverride;

            overrideEventHandler(baseObject, args);
            generatorOverride = args.Overrides.Select(p => p.Value).Last();

            return generatorOverride.GetOverrideId(baseObject, predicate, generatedId);
        }

        /// <summary>   A GetOverridesEventHandler extension method that skip process. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handler">                  The handler to act on. </param>
        /// <param name="facetHandler">             The facet handler. </param>
        /// <param name="baseObject">               The base object. </param>
        /// <param name="facet">                    The facet. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool SkipProcess(this GetOverridesEventHandler handler, IFacetHandler facetHandler, IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var overrideEventHandler = handler;
            var args = new GetOverridesEventArgs();
            IGeneratorOverrides generatorOverride;

            overrideEventHandler(baseObject, args);
            generatorOverride = args.Overrides.Select(p => p.Value).Last();

            return generatorOverride.SkipProcess(facetHandler, baseObject, facet, generatorConfiguration);
        }

        /// <summary>   An IEntityWithDataType extension method that gets original data type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="entityWithDataType">   The entityWithDataType to act on. </param>
        ///
        /// <returns>   The original data type. </returns>

        public static BaseType GetOriginalDataType(this IEntityObjectWithDataType entityWithDataType)
        {
            var args = new GetOverridesEventArgs();
            var overrideEventHandler = entityWithDataType.OverrideEventHandler;
            var entityDataType = entityWithDataType.DataType;
            IGeneratorOverrides generatorOverride;
            BaseType type;
            string _namespace;

            overrideEventHandler(entityWithDataType, args);
            generatorOverride = args.Overrides.Select(p => p.Value).Last();

            if (generatorOverride.OverridesNamespace)
            {
                _namespace = generatorOverride.OriginalNamespace;

                type = new BaseType()
                {
                    FullyQualifiedName = _namespace + "." + entityWithDataType.DataType.Name,
                    Name = entityDataType.Name,
                    ID = entityDataType.ID,
                    ParentID = entityDataType.ParentID
                };
            }
            else
            {
                type = entityWithDataType.DataType;
            }

            return type;
        }

        /// <summary>
        /// A Dictionary&lt;string,PackageWorkingInstallFromCache&gt; extension method that query if
        /// 'dependencies' contains install key.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="dependencies"> The dependencies to act on. </param>
        /// <param name="root">         The root. </param>
        /// <param name="install">      The install. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool ContainsInstallKey(this Dictionary<string, PackageWorkingInstallFromCache> dependencies, PackageWorkingInstallFromCache root, string install)
        {
            return dependencies.Concat(new List<KeyValuePair<string, PackageWorkingInstallFromCache>>() { new KeyValuePair<string, PackageWorkingInstallFromCache>(root.Install, root) }).Where(p => !p.Value.InstallToLocalModules).ToDictionary().ContainsKey(install);
        }

        /// <summary>   A PackageWorkingInstallFromCache extension method that creates the status. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="installFromCache"> The installFromCache to act on. </param>
        /// <param name="status">           The status. </param>
        /// <param name="mode">             The mode. </param>
        ///
        /// <returns>   The new status. </returns>

        public static PackageInstallFromCacheStatus CreateStatus(this PackageWorkingInstallFromCache installFromCache, string status, StatusMode mode)
        {
            return new PackageInstallFromCacheStatus(installFromCache.Install, installFromCache.CachePath, installFromCache.PackagePath, status, mode);
        }

        /// <summary>   A PackageWorkingInstallFromCache extension method that creates the status. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="installFromCache"> The installFromCache to act on. </param>
        /// <param name="ex">               The exception. </param>
        ///
        /// <returns>   The new status. </returns>

        public static PackageInstallFromCacheStatus CreateStatus(this PackageWorkingInstallFromCache installFromCache, Exception ex)
        {
            return new PackageInstallFromCacheStatus(installFromCache.Install, installFromCache.CachePath, installFromCache.PackagePath, ex.Message, StatusMode.Error);
        }

        /// <summary>   A string extension method that fix dots. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   A string. </returns>

        public static string FixDots(this string path)
        {
            string newPath = path.RegexReplace(@"^\.\./", "./");

            if (newPath.StartsWith("./../"))
            {
                newPath = newPath.RemoveStart("./");
            }

            return newPath;
        }

        /// <summary>   A TimeSpan extension method that converts a timeSpan to a decimal seconds. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="timeSpan"> The timeSpan to act on. </param>
        ///
        /// <returns>   TimeSpan as a float. </returns>

        public static float ToDecimalSeconds(this TimeSpan timeSpan)
        {
            return timeSpan.GetDecimalTimeComponent((t) => t.Milliseconds, 2);
        }

        /// <summary>   An UIAttribute extension method that gets feature kind. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="uIAttribute">  The uIAttribute to act on. </param>
        ///
        /// <returns>   The feature kind. </returns>

        public static UIFeatureKind GetFeatureKind(this UIAttribute uIAttribute)
        {
            var uiKind = uIAttribute.UIKind;
            var field = EnumUtils.GetField<UIKind>(uiKind);
            var kindGuidAttribute = field.GetCustomAttribute<KindGuidAttribute>();

            return kindGuidAttribute.FeatureKind;
        }

        /// <summary>   A DefinitionKind extension method that matches. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/1/2021. </remarks>
        ///
        /// <param name="uiAttribute">      The uiAttribute to act on. </param>
        /// <param name="testAttribute">    The test attribute. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool Matches(this UIAttribute uiAttribute, UIAttribute testAttribute)
        {
            return testAttribute.UIHierarchyPath == uiAttribute.UIHierarchyPath && testAttribute.UIKind == uiAttribute.UIKind && testAttribute.UILoadKind == testAttribute.UILoadKind;
        }

        /// <summary>   A DefinitionKind extension method that matches. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/1/2021. </remarks>
        ///
        /// <param name="moduleAssembly">   The moduleAssembly to act on. </param>
        /// <param name="testAttribute">    The test attribute. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool Matches(this IModuleAssembly moduleAssembly, UIAttribute testAttribute)
        {
            return testAttribute.UIHierarchyPath == moduleAssembly.UIHierarchyPath && testAttribute.UIKind == moduleAssembly.UIKind && testAttribute.UILoadKind == moduleAssembly.UILoadKind;
        }

        /// <summary>
        /// An UIFeatureKind extension method that query if 'uiFeatureKind' is component.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="uiFeatureKind">    The uiFeatureKind to act on. </param>
        ///
        /// <returns>   True if component, false if not. </returns>

        public static bool IsComponent(this UIFeatureKind uiFeatureKind)
        {
            return uiFeatureKind.IsOneOf(UIFeatureKind.CustomComponent, UIFeatureKind.StandardComponent);
        }

        /// <summary>   An XPathOperator extension method that gets a text. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="_operator">    The _operator to act on. </param>
        ///
        /// <returns>   The text. </returns>

        public static string GetText(this XPathOperator _operator)
        {
            return XPathElement.GetCSharpOperatorString(_operator);
        }

        /// <summary>
        /// A QueryDictionary extension method that adds to the dictionary list create if not exist.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="dictionary">   The dictionary to act on. </param>
        /// <param name="key">          The key. </param>
        /// <param name="queryInfo">    Information describing the query. </param>

        public static void AddToDictionaryListCreateIfNotExist(this QueryDictionary dictionary, IBase key, QueryInfo queryInfo)
        {
            List<QueryInfo> items;

            if (dictionary.ContainsKey(key))
            {
                items = dictionary[key];
            }
            else
            {
                items = new List<QueryInfo>();

                dictionary.Add(key, items);
            }

            if (!items.Contains(queryInfo))
            {
                items.Add(queryInfo);
            }
        }

        /// <summary>   An IElement extension method that gets a key. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="element">  The element to act on. </param>
        ///
        /// <returns>   The key. </returns>

        public static IAttribute GetKey(this IElement element)
        {
            if (element is IElementWithKeyAttribute)
            {
                var elementWithKeyAttribute = (IElementWithKeyAttribute)element;

                return elementWithKeyAttribute.GetKeyAttribute();
            }

            return element.Attributes.Single(a => a.HasFacetAttribute<KeyAttribute>());
        }

        /// <summary>
        /// A List&lt;IBuiltInImportHandler&gt; extension method that clears the declarations described
        /// by builtInImportHandlerList.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="builtInImportHandlerList"> The builtInImportHandlerList to act on. </param>

        public static void ClearDeclarations(this List<IBuiltInImportHandler> builtInImportHandlerList)
        {
            builtInImportHandlerList.ForEach(b => b.ClearDeclarations());
        }

        /// <summary>   Enumerates exclude in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="importGroups"> The importGroups to act on. </param>
        /// <param name="moduleName">   Name of the module. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process exclude in this collection.
        /// </returns>

        public static IDictionary<string, IEnumerable<ModuleImportDeclaration>> Exclude(this IDictionary<string, IEnumerable<ModuleImportDeclaration>> importGroups, string moduleName)
        {
            return importGroups.Where(g => !g.Value.Any(m => m.ModuleNames.Any(n => n == moduleName))).ToDictionary(g => g.Key, g => g.Value);
        }

        /// <summary>   Enumerates exclude in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="importDeclarations">   The importDeclarations to act on. </param>
        /// <param name="moduleName">           Name of the module. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process exclude in this collection.
        /// </returns>

        public static IEnumerable<ModuleImportDeclaration> Exclude(this IEnumerable<ModuleImportDeclaration> importDeclarations, string moduleName)
        {
            return importDeclarations.Where(m => !m.ModuleNames.Any(n => n == moduleName));
        }

        /// <summary>
        /// An IDictionary&lt;string,IEnumerable&lt;ModuleImportDeclaration&gt;&gt; extension method that
        /// adds an index import.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="importGroups">     The importGroups to act on. </param>
        /// <param name="groupName">        Name of the group. </param>
        /// <param name="moduleName">       Name of the module. </param>
        /// <param name="subFolderCount">   Number of sub folders. </param>

        public static void AddIndexImport(this IDictionary<string, IEnumerable<ModuleImportDeclaration>> importGroups, string groupName, string moduleName, int subFolderCount)
        {
            var folderBuilder = new StringBuilder();
            var declarations = (List<ModuleImportDeclaration>) importGroups[groupName];
            ModuleImportDeclaration declaration;
            string path;

            subFolderCount.Countdown((n) =>
            {
                folderBuilder.Append(@"../");
            });

            folderBuilder.Append("..");
            path = folderBuilder.ToString();

            declaration = new ModuleImportDeclaration(path, moduleName);

            declarations.Add(declaration);
        }

        /// <summary>   An IModuleAssembly extension method that creates import declaration. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="module">           The module to act on. </param>
        /// <param name="folder">           Pathname of the folder. </param>
        /// <param name="subFolderCount">   Number of sub folders. </param>
        ///
        /// <returns>   The new import declaration. </returns>

        public static ModuleImportDeclaration CreateImportDeclaration(this Module module, Folder folder, int subFolderCount)
        {
            var file = module.File;
            var fileName = FileSystemObject.PathCombine(file.FolderName, Path.GetFileNameWithoutExtension(file.Name));
            var fileInfo = new FileInfo(fileName);
            var folderBuilder = new StringBuilder(folder.FullName.BackSlashes());
            string path;
            ModuleImportDeclaration declaration;

            subFolderCount.Countdown((n) =>
            {
                folderBuilder.AppendFormat(@"\subFolder{0}", n);
            });

            if (module.ReferencedByIndex)
            {
                path = Path.GetDirectoryName(fileInfo.GetRelativePath(folderBuilder.ToString()).ForwardSlashes());
            }
            else
            {
                path = fileInfo.GetRelativePath(folderBuilder.ToString()).ForwardSlashes();
            }

            if (!path.StartsWith(@"../"))
            {
                path = path.Prepend("./");
            }

            declaration = new ModuleImportDeclaration(path, module, module.Name);

            return declaration;
        }

        /// <summary>   An IModuleAssembly extension method that creates import declaration. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="moduleAssembly">   The moduleAssembly to act on. </param>
        /// <param name="folder">           Pathname of the folder. </param>
        /// <param name="subFolderCount">   Number of sub folders. </param>
        ///
        /// <returns>   The new import declaration. </returns>

        public static ModuleImportDeclaration CreateImportDeclaration(this IModuleAssembly moduleAssembly, Folder folder, int subFolderCount)
        {
            var file = moduleAssembly.File;
            var fileName = FileSystemObject.PathCombine(file.FolderName, Path.GetFileNameWithoutExtension(file.Name));
            var fileInfo = new FileInfo(fileName);
            var folderBuilder = new StringBuilder(folder.FullName.BackSlashes());
            string path;
            ModuleImportDeclaration declaration;

            subFolderCount.Countdown((n) =>
            {
                folderBuilder.AppendFormat(@"\subFolder{0}", n);
            });

            path = fileInfo.GetRelativePath(folderBuilder.ToString()).ForwardSlashes();

            if (!path.StartsWith(@"../"))
            {
                path = path.Prepend("./");
            }

            declaration = new ModuleImportDeclaration(path, moduleAssembly, moduleAssembly.Name);

            return declaration;
        }

        /// <summary>   An IAttribute extension method that query if 'attribute' is key. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="attribute">    The attribute to act on. </param>
        ///
        /// <returns>   True if key, false if not. </returns>

        public static bool IsKey(this IAttribute attribute)
        {
            return attribute.HasFacetAttribute<KeyAttribute>();
        }

        /// <summary>   An Enum extension method that gets module import declaration attribute. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="moduleId"> The moduleId to act on. </param>
        ///
        /// <returns>   The module import declaration attribute. </returns>

        public static ModuleImportDeclarationAttribute GetModuleImportDeclarationAttribute(this Enum moduleId)
        {
            var field = EnumUtils.GetField(moduleId);

            return field.GetCustomAttribute<ModuleImportDeclarationAttribute>();
        }

        /// <summary>
        /// An IdentityFieldKind extension method that query if 'userFieldKind' has identity field
        /// category attribute.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="userFieldKind">    The userFieldKind to act on. </param>
        ///
        /// <returns>   True if identity field category attribute, false if not. </returns>

        public static bool HasIdentityFieldCategoryAttribute(this IdentityFieldKind userFieldKind)
        {
            var field = EnumUtils.GetField(userFieldKind);

            return field.HasCustomAttribute<IdentityFieldCategoryAttribute>();
        }

        /// <summary>
        /// An IdentityFieldKind extension method that gets identity field category attribute.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="userFieldKind">    The userFieldKind to act on. </param>
        ///
        /// <returns>   The identity field category attribute. </returns>

        public static IdentityFieldCategoryAttribute GetIdentityFieldCategoryAttribute(this IdentityFieldKind userFieldKind)
        {
            var field = EnumUtils.GetField(userFieldKind);

            return field.GetCustomAttribute<IdentityFieldCategoryAttribute>();
        }

        /// <summary>   An IAttribute extension method that gets script type name. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="attribute">    The attribute to act on. </param>
        ///
        /// <returns>   The script type name. </returns>

        public static string GetScriptTypeName(this IAttribute attribute)
        {
            var shortTypeName = attribute.DataType.UnderlyingType.GetShortName();
            var mappings = TypeMapper.Mappings["ScriptTypes"];
            var entry = mappings.Map[shortTypeName];

            return entry.MappedToType;
        }

        /// <summary>   An IAttribute extension method that gets dot net type name. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="attribute">    The attribute to act on. </param>
        ///
        /// <returns>   The dot net type name. </returns>

        public static string GetDotNetTypeName(this IAttribute attribute)
        {
            var shortTypeName = attribute.DataType.UnderlyingType.GetShortName();

            return shortTypeName;
        }

        /// <summary>   An IAttribute extension method that gets short type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="attribute">    The attribute to act on. </param>
        ///
        /// <returns>   The short type. </returns>

        public static string GetShortType(this IAttribute attribute)
        {
            return attribute.DataType.UnderlyingType.GetShortName();
        }

        /// <summary>   A string[] extension method that splits the imports. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="importStrings">    The importStrings to act on. </param>
        ///
        /// <returns>   A Guid[]. </returns>

        public static Guid[] SplitImports(this string[] importStrings)
        {
            var list = new List<Guid>();

            if (importStrings != null)
            {
                foreach (var str in importStrings)
                {
                    foreach (var str2 in str.Split(",").Select(s => s.Trim()))
                    {
                        list.Add(Guid.Parse(str2));
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>   A string extension method that converts this  to a constant name. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="value">    The value to act on. </param>
        /// <param name="suffix">   (Optional) The suffix. </param>
        ///
        /// <returns>   The given data converted to a string. </returns>

        public static string ToConstantName(this string value, string suffix = "")
        {
            return value.Replace(" ", "_").ToUpper() + suffix;
        }

        /// <summary>   An Expression extension method that visits the given node. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="node"> The node to act on. </param>
        ///
        /// <returns>   An AbstraXVisitor. </returns>

        public static AbstraXVisitor Visit(this Expression node)
        {
            var visitor = new AbstraXVisitor();

            visitor.Visit(node);

            return visitor;
        }

        /// <summary>   An IBase extension method that gets display name. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   The display name. </returns>

        public static string GetDisplayName(this IBase baseObject)
        {
            DisplayNameAttribute displayNameAttribute;
            string name;

            if (baseObject.NoUIConfigOrFollow())
            {
                return baseObject.Name.ToTitleCase();
            }

            displayNameAttribute = baseObject.GetFacetAttribute<DisplayNameAttribute>();

            if (displayNameAttribute != null)
            {
                name = displayNameAttribute.DisplayName;
            }
            else
            {
                name = baseObject.Name;
            }

            return name;
        }

        /// <summary>   An IBase extension method that gets navigation name. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        /// <param name="loadKind">     The load kind. </param>
        ///
        /// <returns>   The navigation name. </returns>

        public static string GetNavigationName(this IBase baseObject, UILoadKind loadKind)
        {
            int count;
            UINavigationNameAttribute nameAttribute;
            string name;

            if (baseObject.NoUIConfigOrFollow())
            {
                return baseObject.Name.ToTitleCase();
            }
            count = baseObject.GetFacetAttributes<UINavigationNameAttribute>().Count();

            if (count == 1)
            {
                nameAttribute = baseObject.GetFacetAttribute<UINavigationNameAttribute>();
            }
            else
            {
                nameAttribute = baseObject.GetFacetAttribute<UINavigationNameAttribute>(u => u.UILoadKind == loadKind);
            }

            if (nameAttribute != null)
            {
                name = nameAttribute.Name;
            }
            else if (baseObject is NavigationProperty)
            {
                name = baseObject.Name;
            }
            else
            {
                name = baseObject.Name;
            }

            return name;
        }

        /// <summary>   An IBase extension method that gets navigation name. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        /// <param name="kind">         The kind. </param>
        ///
        /// <returns>   The navigation name. </returns>

        public static string GetNavigationName(this IBase baseObject, UIKind kind)
        {
            int count;
            UINavigationNameAttribute nameAttribute;
            string name;

            if (baseObject.NoUIConfigOrFollow())
            {
                return baseObject.Name.ToTitleCase();
            }
            count = baseObject.GetFacetAttributes<UINavigationNameAttribute>().Count();

            if (count == 1)
            {
                nameAttribute = baseObject.GetFacetAttribute<UINavigationNameAttribute>();
            }
            else
            {
                nameAttribute = baseObject.GetFacetAttribute<UINavigationNameAttribute>(u => u.UIKind == kind);
            }

            if (nameAttribute != null)
            {
                name = nameAttribute.Name;
            }
            else if (baseObject is NavigationProperty)
            {
                name = baseObject.Name;
            }
            else
            {
                name = baseObject.Name;
            }

            return name;
        }

        /// <summary>   An IBase extension method that gets navigation names. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/31/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        /// <param name="kinds">        The kind. </param>
        ///
        /// <returns>   The navigation names. </returns>

        public static IEnumerable<string> GetNavigationNames(this IBase baseObject, params UIKind[] kinds)
        {
            UINavigationNameAttribute nameAttribute;

            foreach (var kind in kinds)
            {
                nameAttribute = baseObject.GetFacetAttribute<UINavigationNameAttribute>(u => u.UIKind == kind);

                if (nameAttribute != null)
                {
                    yield return nameAttribute.Name;
                }
            }
        }

        /// <summary>   An IBase extension method that gets navigation name. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   The navigation name. </returns>

        public static string GetNavigationName(this IBase baseObject)
        {
            UINavigationNameAttribute nameAttribute;
            string name;

            if (baseObject.NoUIConfigOrFollow())
            {
                return baseObject.Name.ToTitleCase();
            }

            nameAttribute = baseObject.GetFacetAttribute<UINavigationNameAttribute>();

            if (nameAttribute != null)
            {
                name = nameAttribute.Name;
            }
            else if (baseObject is NavigationProperty)
            {
                name = baseObject.Name;
            }
            else
            {
                name = baseObject.Name;
            }

            return name;
        }

        /// <summary>   Gets the following childrens in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">           The base object. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        /// <param name="noScalar">             (Optional) True to no scalar. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the following childrens in this
        /// collection.
        /// </returns>

        public static IEnumerable<IBase> GetFollowingChildren(this IBase baseObject, PartsAliasResolver partsAliasResolver, bool noScalar = false)
        {
            var parentObject = (IParentBase)baseObject;

            if (parentObject.Kind == DefinitionKind.Model)
            {
                foreach (var descendantObject in baseObject.GetDescendants())
                {
                    if (descendantObject.HasFacetAttribute<UIAttribute>())
                    {
                        yield return descendantObject;
                    }
                }
            }
            else
            {
                if (parentObject.ChildNodes != null)
                {
                    foreach (var childObject in parentObject.ChildNodes)
                    {
                        if (childObject.Follows(parentObject, partsAliasResolver, noScalar))
                        {
                            yield return childObject;
                        }
                    }
                }
            }
        }

        /// <summary>   Gets the ancestors in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        /// <param name="includeSelf">  (Optional) True to include, false to exclude the self. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the ancestors in this collection.
        /// </returns>

        public static IEnumerable<IBase> GetAncestors(this IBase baseObject, bool includeSelf = false)
        {
            var parent = baseObject.Parent;
            var ancestors = new List<IBase>();

            if (includeSelf)
            {
                ancestors.Add(baseObject);
            }

            while (parent != null)
            {
                ancestors.Add(parent);

                parent = parent.Parent;
            }

            return ancestors;
        }

        /// <summary>   Gets the following descendants in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">           The base object. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the following descendants in this
        /// collection.
        /// </returns>

        public static IEnumerable<IBase> GetFollowingDescendants(this IBase baseObject, PartsAliasResolver partsAliasResolver)
        {
            var descendants = new List<IBase>();

            baseObject.GetDescendants(obj => obj.GetFollowingChildren(partsAliasResolver), obj =>
            {
                descendants.Add(obj);
            });

            return descendants;
        }

        /// <summary>   Gets the descendants in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the descendants in this collection.
        /// </returns>

        public static IEnumerable<IBase> GetDescendants(this IBase baseObject)
        {
            var descendants = new Dictionary<string, IBase>();

            baseObject.GetDescendants(obj => (obj is IParentBase && ((IParentBase)obj).ChildNodes != null) ? ((IParentBase)obj).ChildNodes : new List<IBase>(), obj =>
            {
                var name = obj.Name;

                if (!descendants.ContainsKey(name))
                {
                    descendants.Add(name, obj);
                    return true;
                }
                else
                {
                    return false;
                }
            });

            return descendants.Values;
        }

        /// <summary>   Gets the following navigation childrens in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">           The base object. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the following navigation childrens in
        /// this collection.
        /// </returns>

        public static IEnumerable<IBase> GetFollowingNavigationChildren(this IBase baseObject, PartsAliasResolver partsAliasResolver)
        {
            var parentObject = (IParentBase)baseObject;

            foreach (var childObject in parentObject.ChildNodes.OfType<NavigationProperty>())
            {
                if (childObject.Follows(parentObject, partsAliasResolver))
                {
                    yield return childObject;
                }
            }
        }

        /// <summary>   Gets the grid columns in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="element">              The element to act on. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the grid columns in this collection.
        /// </returns>

        public static IEnumerable<IBase> GetGridColumns(this IElement element, PartsAliasResolver partsAliasResolver)
        {
            foreach (var attribute in element.Attributes.Where(a => a.HasFacetAttribute<GridColumnAttribute>()))
            {
                if (attribute.Follows(element, partsAliasResolver))
                {
                    yield return attribute;
                }
            }
        }

        /// <summary>   Gets the form fields in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="element">              The element to act on. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the form fields in this collection.
        /// </returns>

        public static IEnumerable<IBase> GetFormFields(this IElement element, PartsAliasResolver partsAliasResolver)
        {
            foreach (var attribute in element.Attributes.Where(a => a.HasFacetAttribute<FormFieldAttribute>()))
            {
                if (attribute.Follows(element, partsAliasResolver))
                {
                    yield return attribute;
                }
            }
        }

        /// <summary>   Gets the identity fields in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="element">  The element to act on. </param>
        /// <param name="category"> The category. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the identity fields in this
        /// collection.
        /// </returns>

        public static IEnumerable<IBase> GetIdentityFields(this IElement element, IdentityFieldCategory category)
        {
            foreach (var attribute in element.Attributes.Where(a => a.HasFacetAttribute<IdentityFieldAttribute>()))
            {
                var userFieldAttribute = attribute.GetFacetAttribute<IdentityFieldAttribute>();
                var userFieldKind = userFieldAttribute.IdentityFieldKind;

                if (userFieldKind.HasIdentityFieldCategoryAttribute())
                {
                    var categoryAttribute = userFieldKind.GetIdentityFieldCategoryAttribute();

                    if (categoryAttribute.IdentityFieldCategoryFlags.HasAnyFlag(category))
                    {
                        yield return attribute;
                    }
                }
            }
        }

        /// <summary>   Gets the identity fields in this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>
        ///
        /// <param name="element">  The element to act on. </param>
        /// <param name="kinds">     The kind. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the identity fields in this
        /// collection.
        /// </returns>

        public static IEnumerable<IBase> GetIdentityFields(this IElement element, params IdentityFieldKind[] kinds)
        {
            foreach (var attribute in element.Attributes.Where(a => a.HasFacetAttribute<IdentityFieldAttribute>()))
            {
                var identityFieldAttribute = attribute.GetFacetAttribute<IdentityFieldAttribute>();
                var identityFieldKind = identityFieldAttribute.IdentityFieldKind;

                if (kinds.Any(k => k == identityFieldKind))
                {
                    yield return attribute;
                }
            }
        }

        /// <summary>   Gets the parent navigation properties in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="element">              The element to act on. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the parent navigation properties in
        /// this collection.
        /// </returns>

        public static IEnumerable<IRelationProperty> GetParentNavigationProperties(this IElement element, PartsAliasResolver partsAliasResolver)
        {
            var grandParent = (IElement)element.Parent.Parent;

            if (grandParent.Kind == DefinitionKind.StaticContainer)
            {
                var entityContainer = (IEntityContainer)grandParent;

                foreach (var childElement in entityContainer.EntitySets)
                {
                    if (childElement.ChildElements.Single().ID == element.ID && childElement.Follows(element, partsAliasResolver))
                    {
                        yield return childElement;
                    }
                }
            }
            else
            {
                var container = grandParent.GetContainer();

                foreach (var childElement in container.ChildElements.OfType<IRelationProperty>())
                {
                    if (childElement.ChildElements.Single().Name == element.Name)
                    {
                        yield return childElement;
                    }
                }

                foreach (var childElement in grandParent.ChildElements.OfType<IRelationProperty>())
                {
                    if (childElement.ChildElements.Single().ID == element.ID && childElement.Follows(element, partsAliasResolver))
                    {
                        yield return childElement;
                    }
                }
            }
        }

        /// <summary>   An IBase extension method that gets a container. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   The container. </returns>

        public static IEntityContainer GetContainer(this IBase baseObject)
        {
            while (baseObject != null)
            {
                baseObject = baseObject.Parent;

                if (baseObject is IEntityContainer)
                {
                    return (IEntityContainer)baseObject;
                }
            }

            return null;
        }

        /// <summary>   A QueryPathFunctionKind extension method that gets expected cardinality. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="functionKind"> The functionKind to act on. </param>
        ///
        /// <returns>   The expected cardinality. </returns>

        public static QueryExpectedCardinality GetExpectedCardinality(this QueryPathFunctionKind functionKind)
        {
            var field = EnumUtils.GetField<QueryPathFunctionKind>(functionKind);

            if (field.HasCustomAttribute<QueryExpectedCardinalityAttribute>())
            {
                var expectedCardinalityAttribute = field.GetCustomAttribute<QueryExpectedCardinalityAttribute>();

                return expectedCardinalityAttribute.Cardinality;
            }

            return QueryExpectedCardinality.Unknown;
        }

        /// <summary>
        /// A QueryPathFunctionKind extension method that gets function code expression.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="functionKind"> The functionKind to act on. </param>
        ///
        /// <returns>   The function code expression. </returns>

        public static string GetFunctionCodeExpression(this QueryPathFunctionKind functionKind)
        {
            var field = EnumUtils.GetField<QueryPathFunctionKind>(functionKind);

            if (field.HasCustomAttribute<QueryFunctionCodeAttribute>())
            {
                var codeExpression = field.GetCustomAttribute<QueryFunctionCodeAttribute>();

                return codeExpression.CodeExpression;
            }

            return "<Node Code>";
        }

        /// <summary>   A QueryPathAttribute extension method that gets query path queue. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="loadParentPath">   The loadParentPath to act on. </param>
        /// <param name="baseObject">       The base object. </param>
        ///
        /// <returns>   The query path queue. </returns>

        public static QueryPathQueue GetQueryPathQueue(this QueryPathAttribute loadParentPath, IBase baseObject)
        {
            return new QueryPathQueue(loadParentPath, baseObject);
        }

        /// <summary>   An IBase extension method that gets container set. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   The container set. </returns>

        public static IEntitySet GetContainerSet(this IBase baseObject)
        {
            var container = baseObject.GetContainer();

            return container.EntitySets.Single(s => s.ChildElements.Single().Name == baseObject.Name);
        }

        /// <summary>   A ProcessFacetsHandler extension method that raises. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handler">  The handler to act on. </param>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="types">    A variable-length parameters list containing types. </param>

        public static void Raise(this ProcessFacetsHandler handler, object sender, params Type[] types)
        {
            if (handler != null)
            {
                handler.Invoke(sender, new ProcessFacetsEventArgs(types));
            }
        }

        /// <summary>   A ProcessFacetsHandler extension method that raises. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="handler">  The handler to act on. </param>
        /// <param name="sender">   Source of the event. </param>

        public static void Raise<T>(this ProcessFacetsHandler handler, object sender)
        {
            if (handler != null)
            {
                handler.Invoke(sender, new ProcessFacetsEventArgs(new Type[] { typeof(T) }));
            }
        }

        /// <summary>   An IBase extension method that gets navigation leaf. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   The navigation leaf. </returns>

        public static string GetNavigationLeaf(this IBase baseObject)
        {
            if (baseObject is Entity_Set)
            {
                return ((Entity_Set)baseObject).Name;
            }
            else
            {
                var paths = baseObject.GetUIHierarchyPaths();
                var queue = ParseHierarchyPath(paths.First());
                var parts = queue.SplitElementParts(baseObject);

                return parts.Last();
            }
        }

        /// <summary>   An IBase extension method that follows. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="childObject">          The childObject to act on. </param>
        /// <param name="parentObject">         The parent object. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        /// <param name="noScalar">             (Optional) True to no scalar. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool Follows(this IBase childObject, IBase parentObject, PartsAliasResolver partsAliasResolver, bool noScalar = false)
        {
            if (parentObject.Kind == DefinitionKind.Model)
            {
                return true;
            }
            else if (!noScalar || childObject is NavigationProperty)
            {
                var parentPaths = parentObject.GetUIHierarchyPaths();
                var childPaths = childObject.GetUIHierarchyPaths();

                if (parentPaths.Length == 0 && childPaths.Length == 0)
                {
                    if (parentObject is IEntityWithOptionalFacets)
                    {
                        return ((IEntityWithOptionalFacets)parentObject).FollowWithout;
                    }
                }
                else
                {
                    foreach (var parentPath in parentPaths)
                    {
                        var parentQueue = ParseHierarchyPath(parentPath, partsAliasResolver);
                        var parentParts = parentQueue.SplitElementParts(parentObject);

                        foreach (var childPath in childPaths)
                        {
                            var childQueue = ParseHierarchyPath(childPath, partsAliasResolver);
                            var childParts = childQueue.SplitElementParts(childObject);
                            var nextParentPath = false;
                            var x = 0;

                            if (childParts.Count() > 0)
                            {
                                foreach (var part in childParts.Take(childParts.Count() - 1))
                                {
                                    var part2 = parentParts.ElementAt(x);

                                    if (part != part2)
                                    {
                                        nextParentPath = true;
                                        break;
                                    }

                                    x++;
                                }

                                if (nextParentPath)
                                {
                                    continue;
                                }

                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>   An IBase extension method that follows. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>
        ///
        /// <param name="uiAttribute">          The uiAttribute to act on. </param>
        /// <param name="navigableAttribute">   The navigable attribute. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool Follows(this NavigableAttribute navigableAttribute, UIAttribute uiAttribute, PartsAliasResolver partsAliasResolver)
        {
            var parentPath = uiAttribute.UIHierarchyPath;
            var childPath = navigableAttribute.UIHierarchyPath;
            var parentQueue = ParseHierarchyPath(parentPath, partsAliasResolver);
            var parentParts = parentQueue.SplitElementParts();
            var childQueue = ParseHierarchyPath(childPath, partsAliasResolver);
            var childParts = childQueue.SplitElementParts();
            var x = 0;

            if (navigableAttribute is IdentityFieldAttribute identityFieldAttribute)
            {
                if (identityFieldAttribute.UIHierarchyPath == "/")
                {
                    if (!parentPath.IsOneOf("/Login", "/Register"))
                    {
                        return false;
                    }
                }
            }

            if (childParts.Count() > 0)
            {
                foreach (var part in childParts.Take(childParts.Count() - 1))
                {
                    var part2 = parentParts.ElementAt(x);

                    if (part != part2)
                    {
                        return false;
                    }

                    x++;
                }
            }

            return true;
        }

        /// <summary>   Enumerates with facets in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="baseObjects">  The baseObjects to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process with facets in this collection.
        /// </returns>

        public static IEnumerable<IBase> WithFacets<T>(this IEnumerable<IBase> baseObjects) where T : Attribute
        {
            return baseObjects.Where(o => o.HasFacetAttribute<T>());
        }

        /// <summary>   An IBase extension method that gets facet attribute. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   The facet attribute. </returns>

        public static T GetFacetAttribute<T>(this IBase baseObject) where T : Attribute
        {
            if (baseObject is IEntityObjectWithFacets)
            {
                var entityWithFacets = (IEntityObjectWithFacets)baseObject;
                var facet = entityWithFacets.Facets.SingleOrDefault(f => f.Attribute.IsOfType<T>());

                if (facet != null)
                {
                    return (T)facet.Attribute;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>   An IBase extension method that gets facet attribute. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="baseObject">   The base object. </param>
        /// <param name="filter">       Specifies the filter. </param>
        ///
        /// <returns>   The facet attribute. </returns>

        public static T GetFacetAttribute<T>(this IBase baseObject, Func<T, bool> filter) where T : Attribute
        {
            if (baseObject is IEntityObjectWithFacets)
            {
                var entityWithFacets = (IEntityObjectWithFacets)baseObject;
                var facet = entityWithFacets.Facets.SingleOrDefault(f => f.Attribute.IsOfType<T>() && filter((T) f.Attribute));

                if (facet != null)
                {
                    return (T)facet.Attribute;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>   An IBase extension method that gets facet attributes. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   An array of t. </returns>

        public static T[] GetFacetAttributes<T>(this IBase baseObject) where T : Attribute
        {
            if (baseObject is IEntityObjectWithFacets)
            {
                var entityWithFacets = (IEntityObjectWithFacets)baseObject;
                var facets = entityWithFacets.Facets.Where(f => f.Attribute.IsOfType<T>());

                if (facets.Count() > 0)
                {
                    var attributes = new List<T>();

                    foreach (var facet in facets)
                    {
                        attributes.Add((T)facet.Attribute);
                    }

                    return attributes.ToArray();
                }
                else
                {
                    return new T[0];
                }
            }

            return new T[0];
        }

        /// <summary>   An IBase extension method that gets validation attributes. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   An array of attribute. </returns>

        public static Attribute[] GetValidationAttributes(this IBase baseObject)
        {
            if (baseObject is IEntityObjectWithFacets)
            {
                var entityWithFacets = (IEntityObjectWithFacets)baseObject;
                return entityWithFacets.Facets.Where(f => f.Attribute.GetType().IsOneOf<ValidationAttribute, DataTypeAttribute>()).Select(f => f.Attribute).ToArray();
            }

            return new Attribute[0];
        }

        /// <summary>   An IBase extension method that query if 'baseObject' has facet attribute. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   True if facet attribute, false if not. </returns>

        public static bool HasFacetAttribute<T>(this IBase baseObject) where T : Attribute
        {
            if (baseObject is IEntityWithOptionalFacets && baseObject.Facets.Length == 0)
            {
                return false;
            }
            else if (baseObject is IEntityObjectWithFacets)
            {
                var entityWithFacets = (IEntityObjectWithFacets)baseObject;
                return entityWithFacets.Facets.Any(f => f.Attribute.IsOfType<T>());
            }

            return false;
        }

        /// <summary>   A Facet[] extension method that gets user interface hierarchy path list. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="facets">       The facets to act on. </param>
        /// <param name="printMode">    The print mode. </param>
        ///
        /// <returns>   The user interface hierarchy path list. </returns>

        public static string GetUIHierarchyPathList(this Facet[] facets, PrintMode printMode)
        {
            return facets.Select(f => f.Attribute).OfType<UIAttribute>().Select(a =>
            {
                if (a.UILoadKind.IsOneOf(UILoadKind.HomePage, UILoadKind.MainPage))
                {
                    var facetTypes = facets.Select(f => f.GetType());
                    var nameAttributes = facets.Select(f => f.Attribute).OfType<UINavigationNameAttribute>().ToList();

                    if (nameAttributes.Count == 1)
                    {
                        return nameAttributes.Single().Name;
                    }
                    else
                    {
                        return nameAttributes.Single(n => n.UIKind == a.UIKind).Name;
                    }
                }
                else if (printMode.HasFlag(PrintMode.PrintUIHierarchyPath) && a.PathRootAlias != null)
                {
                    return a.UIHierarchyPath + " = " + a.PathRootAlias;
                }
                else
                {
                    return a.UIHierarchyPath;
                }

            }).ToMultiLineList();
        }

        /// <summary>   An IBase extension method that gets user interface hierarchy paths. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   An array of string. </returns>

        public static string[] GetUIHierarchyPaths(this IBase baseObject)
        {
            var componentAttributes = baseObject.GetFacetAttributes<UIAttribute>();
            var navigableAttributes = baseObject.GetFacetAttributes<NavigableAttribute>();
            var paths = new List<string>();
            var facets = baseObject.Facets;

            if (componentAttributes.Count() > 0)
            {
                foreach (var componentAttribute in componentAttributes)
                {
                    if (componentAttribute.UILoadKind.IsOneOf(UILoadKind.HomePage, UILoadKind.MainPage))
                    {
                        var facetTypes = facets.Select(f => f.GetType());
                        var nameAttributes = facets.Select(f => f.Attribute).OfType<UINavigationNameAttribute>().ToList();

                        if (nameAttributes.Count == 1)
                        {
                            paths.Add(nameAttributes.Single().Name);
                        }
                        else
                        {
                            paths.Add(nameAttributes.Single(n => n.UIKind == componentAttribute.UIKind).Name);
                        }
                    }
                    else
                    {
                        paths.Add(componentAttribute.UIHierarchyPath);
                    }
                }

                return paths.ToArray();
            }
            else if (navigableAttributes.Count() > 0)
            {
                foreach (var navigableAttribute in navigableAttributes)
                {
                    paths.Add(navigableAttribute.UIHierarchyPath);
                }

                return paths.ToArray();
            }

            return new string[0];
        }

        /// <summary>   An IEnumerable&lt;HandlerStackItem&gt; extension method that pre process. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handlerStackItems">        The handlerStackItems to act on. </param>
        /// <param name="baseObject">               The base object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="currentHandler">           The current handler. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool PreProcess(this IEnumerable<HandlerStackItem> handlerStackItems, IBase baseObject, IGeneratorConfiguration generatorConfiguration, IViewLayoutHandler currentHandler)
        {
            foreach (var handlerStackItem in handlerStackItems)
            {
                foreach (var handler in handlerStackItem.ViewLayoutHandlers)
                {
                    if (!handler.PreProcess(baseObject, generatorConfiguration, currentHandler))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// An IEnumerable&lt;HandlerStackItem&gt; extension method that posts the process.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handlerStackItems">        The handlerStackItems to act on. </param>
        /// <param name="baseObject">               The base object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="currentHandler">           The current handler. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool PostProcess(this IEnumerable<HandlerStackItem> handlerStackItems, IBase baseObject, IGeneratorConfiguration generatorConfiguration, IViewLayoutHandler currentHandler)
        {
            foreach (var handlerStackItem in handlerStackItems)
            {
                foreach (var handler in handlerStackItem.ViewLayoutHandlers)
                {
                    if (!handler.PostProcess(baseObject, generatorConfiguration, currentHandler))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>   A HandlerStackItem extension method that logs a create. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="handlerStackItem"> The handlerStackItem to act on. </param>
        /// <param name="uiHierarchyPath">  Full pathname of the hierarchy file. </param>
        /// <param name="currentPass">      The current pass. </param>
        /// <param name="logger">           The logger. </param>
        ///
        /// <returns>   A HandlerStackItem. </returns>

        public static T LogCreate<T>(this HandlerStackItem handlerStackItem, string uiHierarchyPath, GeneratorPass currentPass, Serilog.ILogger logger) where T : HandlerStackItem
        {
            handlerStackItem.LogEvent(HandlerStackEvent.Created, uiHierarchyPath, currentPass, logger);

            return (T) handlerStackItem;
        }

        /// <summary>   A HandlerStackItem extension method that logs a create. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handlerStackItem"> The handlerStackItem to act on. </param>
        /// <param name="uiHierarchyPath">  Full pathname of the hierarchy file. </param>
        /// <param name="currentPass">      The current pass. </param>
        /// <param name="logger">           The logger. </param>
        ///
        /// <returns>   A HandlerStackItem. </returns>

        public static HandlerStackItem LogCreate(this HandlerStackItem handlerStackItem, string uiHierarchyPath, GeneratorPass currentPass, Serilog.ILogger logger)
        {
            handlerStackItem.LogEvent(HandlerStackEvent.Created, uiHierarchyPath, currentPass, logger);

            return handlerStackItem;
        }

        /// <summary>   An IEnumerable&lt;HandlerStackItem&gt; extension method that pre process. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handlerStackItems">        The handlerStackItems to act on. </param>
        /// <param name="baseObject">               The base object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="currentHandler">           The current handler. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool PreProcess(this IEnumerable<HandlerStackItem> handlerStackItems, IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            foreach (var handler in handlerStackItems.SelectMany(h => h.AsEnumerable()))
            {
                if (!handler.PreProcess(baseObject, generatorConfiguration, currentHandler))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>   An IBase extension method that no user interface configuration or follow. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool NoUIConfigOrFollow(this IBase baseObject)
        {
            var noUIConfigOrFollow = false;
             
            if (baseObject is IEntityWithOptionalFacets)
            {
                var entityWithOptionalFacets = (IEntityWithOptionalFacets)baseObject;

                noUIConfigOrFollow = entityWithOptionalFacets.NoUIOrConfig;

                if (entityWithOptionalFacets.FollowWithout)
                {
                    noUIConfigOrFollow = false;
                }
            }

            return noUIConfigOrFollow;
        }

        /// <summary>
        /// An IEnumerable&lt;HandlerStackItem&gt; extension method that posts the process.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="handlerStackItems">        The handlerStackItems to act on. </param>
        /// <param name="baseObject">               The base object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="currentHandler">           The current handler. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool PostProcess(this IEnumerable<HandlerStackItem> handlerStackItems, IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            foreach (var handler in handlerStackItems.SelectMany(h => h.AsEnumerable()))
            {
                if (!handler.PostProcess(baseObject, generatorConfiguration, currentHandler))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>   Parse hierarchy path. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="componentAttribute">   The componentAttribute to act on. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        ///
        /// <returns>   A Queue&lt;IXPathPart&gt; </returns>

        public static Queue<IXPathPart> ParseHierarchyPath(this UIAttribute componentAttribute, PartsAliasResolver partsAliasResolver)
        {
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();
            var id = string.Empty;
            var path = componentAttribute.UIHierarchyPath;

            path = partsAliasResolver.Resolve(path);

            parser.Parse(path, builder);

            return builder.PartQueue;
        }

        /// <summary>   Parse hierarchy path. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="componentAttribute">   The componentAttribute to act on. </param>
        ///
        /// <returns>   A Queue&lt;IXPathPart&gt; </returns>

        public static Queue<IXPathPart> ParseHierarchyPath(this UIAttribute componentAttribute)
        {
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();
            var id = string.Empty;
            var path = componentAttribute.UIHierarchyPath;

            parser.Parse(path, builder);

            return builder.PartQueue;
        }

        /// <summary>   Parse hierarchy path. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="path">                 Full pathname of the file. </param>
        /// <param name="partsAliasResolver">   The parts alias resolver. </param>
        ///
        /// <returns>   A Queue&lt;IXPathPart&gt; </returns>

        public static Queue<IXPathPart> ParseHierarchyPath(string path, PartsAliasResolver partsAliasResolver)
        {
            if (!path.IsNullWhiteSpaceOrEmpty())
            {
                var parser = new XPathParser<string>();
                var builder = new XPathStringBuilder();
                var id = string.Empty;

                path = partsAliasResolver.Resolve(path);

                parser.Parse(path, builder);

                return builder.PartQueue;
            }
            else
            {
                return new Queue<IXPathPart>();
            }
        }

        /// <summary>   Parse hierarchy path. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   A Queue&lt;IXPathPart&gt; </returns>

        public static Queue<IXPathPart> ParseHierarchyPath(string path)
        {
            if (!path.IsNullWhiteSpaceOrEmpty())
            {
                var parser = new XPathParser<string>();
                var builder = new XPathStringBuilder();
                var id = string.Empty;

                parser.Parse(path, builder);

                return builder.PartQueue;
            }
            else
            {
                return new Queue<IXPathPart>();
            }
        }

        /// <summary>   Enumerates split element parts in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="partsQueue">   The partsQueue to act on. </param>
        /// <param name="parentObject"> The parent object. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process split element parts in this
        /// collection.
        /// </returns>

        public static IEnumerable<string> SplitElementParts(this Queue<IXPathPart> partsQueue, IBase parentObject = null)
        {
            Dictionary<string, object> keyValuePairs = null;
            ObjectTree<UIPathItem> uiPathTree = null;
            ObjectTreeItem<UIPathItem> uiPathItem = null;
            int index = 0;

            if (parentObject != null)
            {
                var root = parentObject.Root;
                var generatorEngine = root.AppGeneratorEngine;
                var config = generatorEngine.GeneratorConfiguration;

                keyValuePairs = config.KeyValuePairs;

                if (keyValuePairs.ContainsKey("UIPathTree"))
                {
                    uiPathTree = (ObjectTree<UIPathItem>)keyValuePairs["UIPathTree"];
                    uiPathItem = (ObjectTreeItem<UIPathItem>)uiPathTree;
                }
                else
                {
                    uiPathTree = BuildUIPathTree(root);
                    uiPathItem = (ObjectTreeItem<UIPathItem>)uiPathTree;

                    keyValuePairs.Add("UIPathTree", uiPathTree);
                }
            }

            foreach (var part in partsQueue.OfType<XPathElement>())
            {
                if (part is XPathElement)
                {
                    var element = (XPathElement)part;

                    if (uiPathItem != null && uiPathItem.Children.Any(i => i.InternalObject.Name == element.Text))
                    {
                        uiPathItem = uiPathItem.Children.Single(i => i.InternalObject.Name == element.Text);

                        if (uiPathItem.InternalObject.TargetBaseObject != null)
                        {
                            var path = uiPathItem.InternalObject.Path.RemoveStartIfMatches("/").RightAt(index);

                            index += path.Length;

                            yield return path;
                        }
                    }
                    else
                    {
                        var path = element.Text;

                        yield return path;
                    }

                    foreach (var predicate in element.Predicates.OfType<XPathPredicate>())
                    {
                        if (predicate.Left is XPathAttribute attribute && predicate.Right == null)
                        {
                            if (uiPathItem != null && uiPathItem.Children.Any(i => i.InternalObject.Name == attribute.Name))
                            {
                                uiPathItem = uiPathItem.Children.Single(i => i.InternalObject.Name == attribute.Name);

                                if (uiPathItem.InternalObject.TargetBaseObject != null)
                                {
                                    var path = uiPathItem.InternalObject.Path.RemoveStartIfMatches("/").RightAt(index);

                                    index += path.Length;

                                    yield return path.RightAt(index);
                                }
                            }
                            else
                            {
                                var path = attribute.Name;

                                yield return path;
                            }
                        }
                    }

                    foreach (var predicate in element.Predicates.OfType<XPathBooleanPredicate>())
                    {
                        if (predicate.Left is XPathAttribute attributeLeft)
                        {
                            if (uiPathItem != null && uiPathItem.Children.Any(i => i.InternalObject.Name == attributeLeft.Name))
                            {
                                uiPathItem = uiPathItem.Children.Single(i => i.InternalObject.Name == attributeLeft.Name);

                                if (uiPathItem.InternalObject.TargetBaseObject != null)
                                {
                                    var path = uiPathItem.InternalObject.Path.RemoveStartIfMatches("/").RightAt(index);

                                    index += path.Length;

                                    yield return path;
                                }
                            }
                            else
                            {
                                var path = attributeLeft.Name;

                                yield return attributeLeft.Name;
                            }
                        }

                        if (predicate.Right is XPathAttribute attributeRight)
                        {
                            if (uiPathItem != null && uiPathItem.Children.Any(i => i.InternalObject.Name == attributeRight.Name))
                            {
                                uiPathItem = uiPathItem.Children.Single(i => i.InternalObject.Name == attributeRight.Name);

                                if (uiPathItem.InternalObject.TargetBaseObject != null)
                                {
                                    var path = uiPathItem.InternalObject.Path.RemoveStartIfMatches("/").RightAt(index);

                                    index += path.Length;

                                    yield return path;
                                }
                            }
                            else
                            {
                                yield return attributeRight.Name;
                            }
                        }
                    }
                }
                else
                {
                    DebugUtils.Break();
                }
            }
        }

        /// <summary>
        /// An IGeneratorConfiguration extension method that gets user interface path tree.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/19/2021. </remarks>
        ///
        /// <param name="config">   The config to act on. </param>
        ///
        /// <returns>   The user interface path tree. </returns>

        public static ObjectTree<UIPathItem> GetUIPathTree(this IGeneratorConfiguration config)
        {
            if (config.KeyValuePairs.ContainsKey("UIPathTree"))
            {
                return (ObjectTree<UIPathItem>) config.KeyValuePairs["UIPathTree"];
            }
            else
            {
                return null;
            }
        }

        /// <summary>   Builds user interface path tree. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/21/2020. </remarks>
        ///
        /// <param name="root"> The root. </param>
        ///
        /// <returns>   An ObjectTree&lt;UIPathItem&gt; </returns>

        private static ObjectTree<UIPathItem> BuildUIPathTree(IRoot root)
        {
            var uiPathTree = new ObjectTree<UIPathItem>();
            var baseObjectLookup = new Dictionary<string, IBase>();
            var generatorEngine = root.AppGeneratorEngine;
            var config = generatorEngine.GeneratorConfiguration;
            var partsAliasResolver = config.CreatePartsAliasResolverInstance();
            var actions = new Queue<Action>();
            Queue<IXPathPart> queue;

            foreach (var model in generatorEngine.AllModels)
            {
                foreach (var container in model.Containers)
                {
                    foreach (var facet in container.Facets.Where(n => n.FacetType.Name == "UIAttribute"))
                    {
                        var uiAttribute = (UIAttribute)facet.Attribute;
                        var path = uiAttribute.UIHierarchyPath;

                        if (uiAttribute.UILoadKind != UILoadKind.Default)
                        {
                            path = container.GetNavigationName(uiAttribute.UIKind);
                        }

                        if (uiAttribute.PathRootAlias != null)
                        {
                            partsAliasResolver.Add(uiAttribute);
                        }

                        actions.Enqueue(() =>
                        {
                            queue = ParseHierarchyPath(path, partsAliasResolver);

                            uiPathTree.AddPartsToTree(queue, path, container, partsAliasResolver);
                        });
                    }

                    foreach (var entitySet in container.EntitySets)
                    {
                        foreach (var facet in entitySet.Facets.Where(n => n.FacetType.Name == "UIAttribute"))
                        {
                            var uiAttribute = (UIAttribute)facet.Attribute;
                            var path = uiAttribute.UIHierarchyPath;

                            if (uiAttribute.PathRootAlias != null)
                            {
                                partsAliasResolver.Add(uiAttribute);
                            }

                            actions.Enqueue(() =>
                            {
                                queue = ParseHierarchyPath(path, partsAliasResolver);

                                uiPathTree.AddPartsToTree(queue, path, entitySet, partsAliasResolver);
                            });
                        }

                        foreach (var entity in entitySet.Entities)
                        {
                            foreach (var facet in entity.Facets.Where(n => n.FacetType.Name == "UIAttribute"))
                            {
                                var uiAttribute = (UIAttribute)facet.Attribute;
                                var path = uiAttribute.UIHierarchyPath;

                                if (uiAttribute.PathRootAlias != null)
                                {
                                    partsAliasResolver.Add(uiAttribute);
                                }

                                actions.Enqueue(() =>
                                {
                                    queue = ParseHierarchyPath(path, partsAliasResolver);

                                    uiPathTree.AddPartsToTree(queue, path, entity, partsAliasResolver);
                                });
                            }

                            foreach (var property in entity.NavigationProperties)
                            {
                                foreach (var facet in property.Facets.Where(n => n.FacetType.Name == "UIAttribute"))
                                {
                                    var uiAttribute = (UIAttribute)facet.Attribute;
                                    var path = uiAttribute.UIHierarchyPath;

                                    if (uiAttribute.PathRootAlias != null)
                                    {
                                        partsAliasResolver.Add(uiAttribute);
                                    }

                                    actions.Enqueue(() =>
                                    {
                                        queue = ParseHierarchyPath(path, partsAliasResolver);

                                        uiPathTree.AddPartsToTree(queue, path, property, partsAliasResolver);
                                    });
                                }
                            }
                        }
                    }
                }
            }

            while (actions.Count > 0)
            {
                var action = actions.Dequeue();

                action();
            }

            config.LogUIPathTree(uiPathTree);

            return uiPathTree;
        }

        /// <summary>   A StreamWriter extension method that logs user interface path item. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/24/2020. </remarks>
        ///
        /// <param name="logSet">       The writer to act on. </param>
        /// <param name="uiPathItem">   The path item. </param>
        /// <param name="indentLevel">  (Optional) The indent level. </param>

        public static void LogUIPathItem(this LogSet logSet, ObjectTreeItem<UIPathItem> uiPathItem, int indentLevel = 0)
        {
            logSet.LogLine("{0}{1}", " ".Repeat(indentLevel * 4), uiPathItem.DebugInfo);

            foreach (var child in uiPathItem.Children)
            {
                logSet.LogUIPathItem(child, indentLevel + 1);
            }
        }

        /// <summary>   A LogSet extension method that logs base object. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/26/2020. </remarks>
        ///
        /// <param name="logSet">       The writer to act on. </param>
        /// <param name="baseObject">   The base object. </param>

        public static void LogBaseObject(this LogSet logSet, IBase baseObject)
        {
            var paths = baseObject.GetUIHierarchyPaths();
            var builder = new StringBuilder();

            builder.AppendLine("UI Hierarchy Paths:\r\n");

            foreach (var path in paths)
            {
                builder.AppendLine(path);
            }

            logSet.LogLine(baseObject.GetDebugInfo(builder));

            
        }

        /// <summary>   A LogSet extension method that logs file system. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/26/2020. </remarks>
        ///
        /// <param name="logSet">       The writer to act on. </param>
        /// <param name="folder">       Pathname of the folder. </param>
        /// <param name="indentLevel">  (Optional) The indent level. </param>

        public static void LogFileSystem(this LogSet logSet, Folder folder, int indentLevel = 0)
        {
            logSet.LogLine("{0}+{1}", " ".Repeat(indentLevel * 4), folder.Name);

            foreach (var subFolder in folder.Folders)
            {
                logSet.LogFileSystem(subFolder, indentLevel + 1);
            }

            foreach (var file in folder.Files)
            {
                logSet.LogLine("{0}-{1}", " ".Repeat((indentLevel + 1) * 4), file.Name);
            }
        }

        private static void AddPartsToTree(this ObjectTree<UIPathItem> objectTree, Queue<IXPathPart> queue, string fullPath, IBase baseObject, PartsAliasResolver partsAliasResolver)
        {
            var x = 0;
            var parentParts = new List<string>();
            var lastItem = (ObjectTreeItem<UIPathItem>) objectTree;
            var pathElements = queue.OfType<XPathElement>().ToList();
            var count = pathElements.Count;

            foreach (var part in pathElements)
            {
                if (part is XPathElement)
                {
                    var element = (XPathElement)part;
                    var elementText = element.Text;
                    var predicates = new List<XPathPredicate>();
                    string elementPath;
                    string path;

                    if (lastItem.InternalObject != null)
                    {
                        path = lastItem.InternalObject.Path + part.ToString();
                        elementPath = lastItem.InternalObject.Path + "/" + elementText;
                    }
                    else
                    {
                        path = part.ToString();
                        elementPath = elementText;
                    }

                    lastItem = lastItem.GetOrAddPartToTree(elementText, elementPath, predicates);
                    x++;

                    foreach (var predicate in element.Predicates.OfType<XPathPredicate>())
                    {
                        if (predicate.Left is XPathAttribute attribute)
                        {
                            var attributeName = attribute.Name;

                            if (predicate.Right == null)
                            {
                                parentParts.Add(attributeName);
                                lastItem = lastItem.GetOrAddPartToTree(attributeName, path.Append("/" + attributeName), null);
                                x++;
                            }
                            else
                            {
                                predicates.Add(predicate);
                            }
                        }
                    }

                    foreach (var predicate in element.Predicates.OfType<XPathBooleanPredicate>())
                    {
                        if (predicate.Left is XPathAttribute attributeLeft)
                        {
                            var attributeName = attributeLeft.Name;

                            lastItem = lastItem.GetOrAddPartToTree(attributeName, path.Append("/" + attributeName), null);

                            x++;
                        }
                        else if (predicate.Left is XPathPredicate predicateLeft)
                        {
                            predicates.Add(predicateLeft);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }

                        if (predicate.Right is XPathAttribute attributeRight)
                        {
                            var attributeName = attributeRight.Name;

                            lastItem = lastItem.GetOrAddPartToTree(attributeName, path.Append("/" + attributeName), null);

                            x++;
                        }
                        else if (predicate.Right is XPathPredicate predicateRight)
                        {
                            predicates.Add(predicateRight);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }
                }
                else
                {
                    DebugUtils.Break();
                }
            }

            lastItem.InternalObject.TargetBaseObject = baseObject;
        }

        private static ObjectTreeItem<UIPathItem> GetOrAddPartToTree(this ObjectTreeItem<UIPathItem> objectTreeItemParent, string part, string path, List<XPathPredicate> predicates)
        {
            ObjectTreeItem<UIPathItem> item;
            var uiPathItem = new UIPathItem
            {
                Name = part,
                Path = path,
                Predicates = predicates
            };

            if (objectTreeItemParent.Children.Any(c => c.InternalObject.Path == path))
            {
                item = objectTreeItemParent.Children.Single(c => c.InternalObject.Path == path);
            }
            else
            {
                item = new ObjectTreeItem<UIPathItem>(uiPathItem);

                objectTreeItemParent.AddChild(item);
            }

            return item;
        }

        /// <summary>   Gets the minimum types in this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/17/2021. </remarks>
        ///
        /// <param name="generatorAssembly">    The assembly to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the minimum types in this collection.
        /// </returns>

        public static IEnumerable<Type> GetMinimumTypes(this Assembly generatorAssembly)
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyExtensions.AssemblyResolve;

            foreach (var type in generatorAssembly.GetTypes().Concat(generatorAssembly.GetExportedTypes()).Distinct())
            {
                yield return type;
            }

            foreach (var assemblyName in generatorAssembly.GetReferencedAssemblies().Where(a => a.Name == "ApplicationGenerator.Interfaces"))
            {
                var refAssembly = Assembly.Load(assemblyName);

                foreach (var assemblyName2 in generatorAssembly.GetReferencedAssemblies().Where(a => a.Name == "System.ComponentModel.DataAnnotations" || a.Name == "System"))
                {
                    var refAssembly2 = Assembly.Load(assemblyName2);

                    foreach (var type in refAssembly2.GetTypes().Concat(refAssembly2.GetExportedTypes()).Distinct())
                    {
                        yield return type;
                    }
                }

                foreach (var type in refAssembly.GetTypes().Concat(refAssembly.GetExportedTypes()).Distinct())
                {
                    yield return type;
                }
            }

        }

        /// <summary>   Gets all types in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="generatorAssembly">        The assembly to act on. </param>
        /// <param name="metadataAssembly">         The metadata assembly. </param>
        /// <param name="netCoreReflectionAgent">   . </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process all types in this collection.
        /// </returns>

        public static IEnumerable<Type> GetAllTypes(this Assembly generatorAssembly, NetCoreReflectionAgent netCoreReflectionAgent, Assembly metadataAssembly = null)
        {
            var isCore = false;

            if (metadataAssembly is AssemblyShim)
            {
                isCore = true;
            }

            AppDomain.CurrentDomain.AssemblyResolve += AssemblyExtensions.AssemblyResolve;

            if (metadataAssembly != null)
            {
                foreach (var type in metadataAssembly.GetTypes())
                {
                    yield return type;
                }

                foreach (var assemblyName in metadataAssembly.GetReferencedAssemblies())
                {
                    if (isCore)
                    {
                        var refAssembly = netCoreReflectionAgent.LoadCoreAssembly(assemblyName, netCoreReflectionAgent.RedirectedNamespaces);

                        if (refAssembly.HasCustomAttribute<EntityMetadataSourceAssemblyAttribute>())
                        {
                            foreach (var type in refAssembly.GetTypes())
                            {
                                yield return type;
                            }
                        }
                    }
                    else
                    {
                        var refAssembly = Assembly.Load(assemblyName);

                        if (refAssembly.HasCustomAttribute<EntityMetadataSourceAssemblyAttribute>())
                        {
                            foreach (var type in refAssembly.GetTypes())
                            {
                                yield return type;
                            }
                        }
                    }
                }
            }

            foreach (var type in generatorAssembly.GetTypes().Concat(generatorAssembly.GetExportedTypes()).Distinct())
            {
                yield return type;
            }

            foreach (var assemblyName in generatorAssembly.GetReferencedAssemblies().Where(a => a.Name == "ApplicationGenerator.Interfaces"))
            {
                var refAssembly = Assembly.Load(assemblyName);

                foreach (var assemblyName2 in generatorAssembly.GetReferencedAssemblies().Where(a => a.Name == "System.ComponentModel.DataAnnotations" || a.Name == "System"))
                {
                    var refAssembly2 = Assembly.Load(assemblyName2);

                    foreach (var type in refAssembly2.GetTypes().Concat(refAssembly2.GetExportedTypes()).Distinct())
                    {
                        yield return type;
                    }
                }

                foreach (var type in refAssembly.GetTypes().Concat(refAssembly.GetExportedTypes()).Distinct())
                {
                    yield return type;
                }
            }

            foreach (var assemblyName in generatorAssembly.GetReferencedAssemblies().Where(a => a.Name == "EntityFramework"))
            {
                var refAssembly = Assembly.Load(assemblyName);

                foreach (var type in refAssembly.GetTypes().Concat(refAssembly.GetExportedTypes()).Distinct())
                {
                    yield return type;
                }
            }
        }

        /// <summary>   Gets all types in this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/28/2022. </remarks>
        ///
        /// <param name="generatorAssembly">    The assembly to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process all types in this collection.
        /// </returns>

        public static IEnumerable<Type> GetAllTypes(this Assembly generatorAssembly)
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyExtensions.AssemblyResolve;

            foreach (var type in generatorAssembly.GetTypes().Concat(generatorAssembly.GetExportedTypes()).Distinct())
            {
                yield return type;
            }

            foreach (var assemblyName in generatorAssembly.GetReferencedAssemblies().Where(a => a.Name == "ApplicationGenerator.Interfaces"))
            {
                var refAssembly = Assembly.Load(assemblyName);

                foreach (var assemblyName2 in generatorAssembly.GetReferencedAssemblies().Where(a => a.Name == "System.ComponentModel.DataAnnotations" || a.Name == "System"))
                {
                    var refAssembly2 = Assembly.Load(assemblyName2);

                    foreach (var type in refAssembly2.GetTypes().Concat(refAssembly2.GetExportedTypes()).Distinct())
                    {
                        yield return type;
                    }
                }

                foreach (var type in refAssembly.GetTypes().Concat(refAssembly.GetExportedTypes()).Distinct())
                {
                    yield return type;
                }
            }

            foreach (var assemblyName in generatorAssembly.GetReferencedAssemblies().Where(a => a.Name == "EntityFramework"))
            {
                var refAssembly = Assembly.Load(assemblyName);

                foreach (var type in refAssembly.GetTypes().Concat(refAssembly.GetExportedTypes()).Distinct())
                {
                    yield return type;
                }
            }
        }

        /// <summary>   A Type extension method that query if 'type' is view layout handler type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="type">     The type to act on. </param>
        /// <param name="layout">   The layout. </param>
        ///
        /// <returns>   True if view layout handler type, false if not. </returns>

        public static bool IsViewLayoutHandlerType(this Type type, string layout)
        {
            var name = type.Name;

            if (type.HasCustomAttribute<ViewLayoutHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<ViewLayoutHandlerAttribute>();

                return Path.GetFileNameWithoutExtension(attribute.ViewLayout) == Path.GetFileNameWithoutExtension(layout);
            }

            return false;
        }

        /// <summary>   A Type extension method that query if this  is facet handler type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="type">                 The type to act on. </param>
        /// <param name="entityDefinitionKind"> The entityDefinitionKind to act on. </param>
        ///
        /// <returns>   True if facet handler type, false if not. </returns>

        public static bool IsFacetHandlerType(this Type type, DefinitionKind entityDefinitionKind)
        {
            var name = type.Name;

            if (type.HasCustomAttribute<FacetHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<FacetHandlerAttribute>();

                if (entityDefinitionKind.Matches(attribute.Kind))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>   A Type extension method that query if this  is facet handler type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="type">                 The type to act on. </param>
        /// <param name="facetType">            Type of the facet. </param>
        /// <param name="entityDefinitionKind"> The entityDefinitionKind to act on. </param>
        /// <param name="componentKind">        The component kind. </param>
        ///
        /// <returns>   True if facet handler type, false if not. </returns>

        public static bool IsFacetHandlerType(this Type type, Type facetType, DefinitionKind entityDefinitionKind, Guid componentKind)
        {
            var name = type.Name;

            if (type.HasCustomAttribute<FacetHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<FacetHandlerAttribute>();

                if (attribute.FacetType == facetType && (attribute.Kind == componentKind || entityDefinitionKind.Matches(attribute.Kind)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>   A Type extension method that query if this  is facet handler type. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/27/2021. </remarks>
        ///
        /// <param name="type">             The type to act on. </param>
        /// <param name="facetType">        Type of the facet. </param>
        /// <param name="componentKind">    The component kind. </param>
        ///
        /// <returns>   True if facet handler type, false if not. </returns>

        public static bool IsFacetHandlerType(this Type type, Type facetType, Guid componentKind)
        {
            var name = type.Name;

            if (type.HasCustomAttribute<FacetHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<FacetHandlerAttribute>();

                if (attribute.FacetType == facetType && attribute.Kind == componentKind)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>   A Type extension method that query if this  is module kind type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="type">             The type to act on. </param>
        /// <param name="moduleKind">       An enum constant representing the module kind option. </param>
        /// <param name="definitionKind">   The definition kind. </param>
        /// <param name="featureKind">      The feature kind. </param>
        ///
        /// <returns>   True if module kind type, false if not. </returns>

        public static bool IsModuleKindType(this Type type, Enum moduleKind, DefinitionKind definitionKind, UIFeatureKind featureKind)
        {
            var name = type.Name;

            if (type.HasCustomAttribute<ModuleKindHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<ModuleKindHandlerAttribute>();

                return attribute.ModuleKind.Equals(moduleKind) && attribute.DefinitionKind == definitionKind && attribute.FeatureKind == featureKind;
            }

            return false;
        }

        /// <summary>   A Type extension method that query if this  is module kind type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="type">             The type to act on. </param>
        /// <param name="moduleKind">       An enum constant representing the module kind option. </param>
        /// <param name="definitionKind">   The definition kind. </param>
        /// <param name="loadKind">         The load kind. </param>
        /// <param name="uiKind">           The kind. </param>
        /// <param name="featureKind">      The feature kind. </param>
        ///
        /// <returns>   True if module kind type, false if not. </returns>

        public static bool IsModuleKindType(this Type type, Enum moduleKind, DefinitionKind definitionKind, UILoadKind loadKind, Guid uiKind, UIFeatureKind featureKind)
        {
            var name = type.Name;

            if (type.HasCustomAttribute<ModuleKindHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<ModuleKindHandlerAttribute>();

                if (attribute.ModuleKind.Equals(moduleKind) && attribute.DefinitionKind == definitionKind && attribute.FeatureKind == featureKind)
                {
                    return true;
                }
                else if (attribute.ModuleKind.Equals(moduleKind) && (attribute.UIKind == uiKind || ((attribute.DefinitionKind == DefinitionKind.NotApplicable || attribute.DefinitionKind == definitionKind) && attribute.FeatureKind == featureKind)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>   A DefinitionKind extension method that matches. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="entityDefinitionKind"> The entityDefinitionKind to act on. </param>
        /// <param name="componentKind">        The component kind. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool Matches(this DefinitionKind entityDefinitionKind, Guid componentKind)
        {
            if (componentKind == Guid.Parse(UIKindGuids.Any))
            {
                return true;
            }
            else
            {
                switch (entityDefinitionKind)
                {
                    case DefinitionKind.Class:
                    case DefinitionKind.Interface:
                    case DefinitionKind.Structure:
                        return componentKind == Guid.Parse(UIKindGuids.Element);
                    case DefinitionKind.StaticContainer:
                        return componentKind == Guid.Parse(UIKindGuids.StaticContainer);
                }
            }

            return false;
        }

        /// <summary>   A Type extension method that query if 'type' is validation handler type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="type">             The type to act on. </param>
        /// <param name="validationType">   Type of the validation. </param>
        ///
        /// <returns>   True if validation handler type, false if not. </returns>

        public static bool IsValidationHandlerType(this Type type, Type validationType)
        {
            if (type.HasCustomAttribute<ValidationHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<ValidationHandlerAttribute>();

                return attribute.ValidationType == validationType;
            }

            return false;
        }

        /// <summary>   A Type extension method that query if 'type' is import handler type. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="type">         The type to act on. </param>
        /// <param name="handlerId">    Identifier for the handler. </param>
        ///
        /// <returns>   True if import handler type, false if not. </returns>

        public static bool IsImportHandlerType(this Type type, ulong handlerId)
        {
            if (type.HasCustomAttribute<ImportHandlerAttribute>())
            {
                var attribute = type.GetCustomAttribute<ImportHandlerAttribute>();

                return attribute.HandlerId == handlerId;
            }

            return false;
        }
    }
    /// <summary>   Extension methods. </summary>
    ///
    /// <remarks>   Ken, 10/3/2020. </remarks>

    public static class TypeShimExtensions
    {
        /// <summary>   Event queue for all listeners interested in getTypeShimActivator events. </summary>
        public static event GetTypeShimActivatorHandler GetTypeShimActivatorEvent;

        /// <summary>   Gets type shim activator. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>
        ///
        /// <returns>   The type shim activator. </returns>

        public static ITypeShimActivator GetTypeShimActivator()
        {
            var args = new GetTypeShimActivatorEventArgs();
            ITypeShimActivator activator;

            GetTypeShimActivatorEvent(typeof(Extensions), args);

            activator = args.Activator;

            return activator;
        }
    }

    /// <summary>   A logging extensions. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

    public static class LoggingExtensions
    {
        /// <summary>   Creates log folder. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="subFolder">    Pathname of the sub folder. </param>
        ///
        /// <returns>   The new log folder. </returns>

        public static System.IO.DirectoryInfo CreateLogFolder(string subFolder)
        {
            var directory = new System.IO.DirectoryInfo(System.IO.Path.Combine(Environment.CurrentDirectory, @"Logs\" + subFolder));
            DirectoryInfo logDirectory = null;

            if (!directory.Exists)
            {
                directory.Create();
            }

            logDirectory = new DirectoryInfo(System.IO.Path.Combine(directory.FullName, DateTime.Now.ToSortableDateTimeText()));

            logDirectory.Create();

            return logDirectory;
        }

        /// <summary>   Searches for the first log folder. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <returns>   The found log folder. </returns>

        public static System.IO.DirectoryInfo FindCurrentLogFolder(string subFolder, bool createIfNotFound = false)
        {
            var directory = new System.IO.DirectoryInfo(System.IO.Path.Combine(Environment.CurrentDirectory, @"Logs\" + subFolder));
            DirectoryInfo logDirectory = null;

            if (directory.Exists)
            {
                logDirectory = directory.GetDirectories().OrderBy(d => d.Name).LastOrDefault();
            }
            else if (createIfNotFound)
            {
                directory.Create();
            }

            if (logDirectory == null)
            {
                if (createIfNotFound)
                {
                    logDirectory = new DirectoryInfo(System.IO.Path.Combine(directory.FullName, DateTime.Now.ToSortableDateTimeText()));

                    logDirectory.Create();
                }
            }

            return logDirectory;
        }

        /// <summary>   Reads existing package. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <returns>   The existing package. </returns>

        public static NpmPackage ReadExistingPackage(this IGeneratorConfiguration generatorConfiguration)
        {
            var npmPackage = new NpmPackage(Environment.CurrentDirectory);

            npmPackage.Load();

            return npmPackage;
        }

        /// <summary>   Gets package records. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="existingNpmPackage">       The existing npm package. </param>
        /// <param name="packageReadEventArgs">     Package read event information. </param>
        ///
        /// <returns>   The package records. </returns>

        public static List<PackageRecord> GetPackageRecords(this IGeneratorConfiguration generatorConfiguration, NpmPackage existingNpmPackage, PackageReadEventArgs packageReadEventArgs)
        {
            var genPackage = packageReadEventArgs.GenPackage;
            var packageRecords = new List<PackageRecord>();
            var packageName = packageReadEventArgs.Package.GetType().Name;
            var importHandlerName = packageReadEventArgs.ImportHandler.GetType().Name;
            var lastOutput = string.Empty;
            Action<string, string> addRecord;

            addRecord = (dependency, dependencyType) =>
            {
                var regex = new Regex("(?<package>@?[^@]*?)($|@(?<version>.*$))");

                if (regex.IsMatch(dependency))
                {
                    var match = regex.Match(dependency);
                    var toInstallName = match.GetGroupValue("package");
                    NpmVersion toInstallVersion = match.GetGroupValue("version");
                    NpmVersion existingVersion = null;
                    NpmVersion latestVersion = null;
                    var existingPackagePair = existingNpmPackage.Dependencies.SingleOrDefault(p => p.Key.AsCaseless() == toInstallName);
                    var packageRecord = new PackageRecord(dependencyType, packageName, importHandlerName, toInstallName, toInstallVersion);

                    if (existingPackagePair.Key != null)
                    {
                        existingVersion = existingPackagePair.Value;

                        packageRecord.ExistingVersion = existingVersion.ToString();
                    }

                    using (var npmCommandHandler = generatorConfiguration.GetNpmCommandHandler())
                    {
                        npmCommandHandler.OutputWriteLine = (f, a) =>
                        {
                            lastOutput = string.Format(f, a);
                        };

                        npmCommandHandler.ErrorWriteLine = (f, a) =>
                        {
                            Console.WriteLine(string.Format(f, a));
                        };

                        lastOutput = string.Empty;
                        npmCommandHandler.ShowPackageVersion(toInstallName);
                    }

                    if (lastOutput != string.Empty)
                    {
                        latestVersion = lastOutput;

                        packageRecord.LatestVersion = latestVersion.ToString();
                    }

                    packageRecords.Add(packageRecord);
                }
                else
                {
                    DebugUtils.Break();
                }
            };

            foreach (var dependency in genPackage.dependencies)
            {
                addRecord(dependency, "Dependency");
            }

            foreach (var dependency in genPackage.devDependencies)
            {
                addRecord(dependency, "DevDependency");
            }

            return packageRecords;
        }

        /// <summary>   Writes to log. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="logDirectory"> The logDirectory to act on. </param>
        /// <param name="fileName">     Filename of the file. </param>
        /// <param name="text">     The JSON text. </param>

        public static void WriteToLog(this DirectoryInfo logDirectory, string fileName, string text)
        {
            var logPath = System.IO.Path.Combine(logDirectory.FullName, fileName);

            using (var writer = new System.IO.StreamWriter(logPath))
            {
                writer.Write(text);
            }
        }

        /// <summary>   A DirectoryInfo extension method that appends to log. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="logDirectory"> The logDirectory to act on. </param>
        /// <param name="fileName">     Filename of the file. </param>
        /// <param name="text">         The JSON text. </param>

        public static void AppendToLog(this DirectoryInfo logDirectory, string fileName, string text)
        {
            var logPath = System.IO.Path.Combine(logDirectory.FullName, fileName);

            using (var writer = new System.IO.StreamWriter(logPath, true))
            {
                writer.Write(text);
            }
        }

        /// <summary>   Writes to log. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="logDirectory"> The logDirectory to act on. </param>
        /// <param name="fileName">     Filename of the file. </param>
        /// <param name="jsonObject">   The JSON object. </param>

        public static void WriteToLog<T>(this DirectoryInfo logDirectory, string fileName, T jsonObject)
        {
            var logPath = System.IO.Path.Combine(logDirectory.FullName, fileName);
            var jsonText = jsonObject.ToJsonText();

            using (var writer = new System.IO.StreamWriter(logPath))
            {
                writer.Write(jsonText);
            }
        }
    }

}
