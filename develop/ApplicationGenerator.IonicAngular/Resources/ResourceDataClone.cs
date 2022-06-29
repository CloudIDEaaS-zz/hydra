using AbstraX.Generators;
using AbstraX.ObjectProperties;
using HtmlAgilityPack;
using Itenso.Rtf;
using Itenso.Rtf.Converter.Html;
using Itenso.Rtf.Converter.Image;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    public class ResourceDataClone : IResourceDataProcessed<ImageInfo, LinkInfo>
    {
        public bool Dirty { get; set; }

        /// <summary>   Gets or sets the name of the application. </summary>
        ///
        /// <value> The name of the application. </value>
        
        public string AppName
        {
            get
            {
                return (string)this[nameof(AppName)];
            }

            set
            {
                this[nameof(AppName)] = value;
            }
        }

        /// <summary>   Gets or sets information describing the application. </summary>
        ///
        /// <value> Information describing the application. </value>

        public string AppDescription
        {
            get
            {
                return (string)this[nameof(AppDescription)];
            }

            set
            {
                this[nameof(AppDescription)] = value;
            }
        }

        /// <summary>   Gets or sets the name of the organization. </summary>
        ///
        /// <value> The name of the organization. </value>

        public string OrganizationName
        {
            get
            {
                return (string)this[nameof(OrganizationName)];
            }

            set
            {
                this[nameof(OrganizationName)] = value;
            }
        }

        /// <summary>   Gets or sets the color of the primary. </summary>
        ///
        /// <value> The color of the primary. </value>

        public string PrimaryColor
        {
            get
            {
                return (string)this[nameof(PrimaryColor)];
            }

            set
            {
                this[nameof(PrimaryColor)] = value;
            }
        }

        /// <summary>   Gets or sets the color of the secondary. </summary>
        ///
        /// <value> The color of the secondary. </value>

        public string SecondaryColor
        {
            get
            {
                return (string)this[nameof(SecondaryColor)];
            }

            set
            {
                this[nameof(SecondaryColor)] = value;
            }
        }

        /// <summary>   Gets or sets the color of the tertiary. </summary>
        ///
        /// <value> The color of the tertiary. </value>

        public string TertiaryColor
        {
            get
            {
                return (string)this[nameof(TertiaryColor)];
            }

            set
            {
                this[nameof(TertiaryColor)] = value;
            }
        }

        /// <summary>   Gets or sets the color of the background. </summary>
        ///
        /// <value> The color of the background. </value>

        public string BackgroundColor
        {
            get
            {
                return (string)this[nameof(BackgroundColor)];
            }

            set
            {
                this[nameof(BackgroundColor)] = value;
            }
        }

        /// <summary>   Gets or sets the splash screen. </summary>
        ///
        /// <value> The splash screen. </value>

        public string SplashScreen
        {
            get
            {
                return (string)this[nameof(SplashScreen)];
            }

            set
            {
                this[nameof(SplashScreen)] = value;
            }
        }

        /// <summary>   Gets or sets the about banner. </summary>
        ///
        /// <value> The about banner. </value>

        public string AboutBanner
        {
            get
            {
                return (string)this[nameof(AboutBanner)];
            }

            set
            {
                this[nameof(AboutBanner)] = value;
            }
        }

        /// <summary>   Gets or sets the logo. </summary>
        ///
        /// <value> The logo. </value>

        public string Logo
        {
            get
            {
                return (string)this[nameof(Logo)];
            }

            set
            {
                this[nameof(Logo)] = value;
            }
        }

        /// <summary>   Gets or sets the icon. </summary>
        ///
        /// <value> The icon. </value>

        public string Icon
        {
            get
            {
                return (string)this[nameof(Icon)];
            }

            set
            {
                this[nameof(Icon)] = value;
            }
        }

        /// <summary>   Gets or sets the end user license. </summary>
        ///
        /// <value> The end user license. </value>

        public string EndUserLicense
        {
            get
            {
                return (string)this[nameof(EndUserLicense)];
            }

            set
            {
                this[nameof(EndUserLicense)] = value;
            }
        }

        /// <summary>   Gets or sets the support details. </summary>
        ///
        /// <value> The support details. </value>

        public string SupportDetails
        {
            get
            {
                return (string)this[nameof(SupportDetails)];
            }

            set
            {
                this[nameof(SupportDetails)] = value;
            }
        }

        /// <summary>   Gets or sets URL of the end user license. </summary>
        ///
        /// <value> The end user license URL. </value>

        public string EndUserLicenseUrl
        {
            get
            {
                return (string)this[nameof(EndUserLicenseUrl)];
            }

            set
            {
                this[nameof(EndUserLicenseUrl)] = value;
            }
        }

        /// <summary>   Gets or sets the end user license email address. </summary>
        ///
        /// <value> The end user license email address. </value>

        public string EndUserLicenseEmailAddress
        {
            get
            {
                return (string)this[nameof(EndUserLicenseEmailAddress)];
            }

            set
            {
                this[nameof(EndUserLicenseEmailAddress)] = value;
            }
        }

        /// <summary>   Gets or sets the end user license mailing address. </summary>
        ///
        /// <value> The end user license mailing address. </value>

        public string EndUserLicenseMailingAddress
        {
            get
            {
                return (string)this[nameof(EndUserLicenseMailingAddress)];
            }

            set
            {
                this[nameof(EndUserLicenseMailingAddress)] = value;
            }
        }

        /// <summary>   Gets or sets URL of the support. </summary>
        ///
        /// <value> The support URL. </value>

        public string SupportUrl
        {
            get
            {
                return (string)this[nameof(SupportUrl)];
            }

            set
            {
                this[nameof(SupportUrl)] = value;
            }
        }

        /// <summary>   Gets or sets the support email address. </summary>
        ///
        /// <value> The support email address. </value>

        public string SupportEmailAddress
        {
            get
            {
                return (string)this[nameof(SupportEmailAddress)];
            }

            set
            {
                this[nameof(SupportEmailAddress)] = value;
            }
        }

        /// <summary>   Gets the full pathname of the root file. </summary>
        ///
        /// <value> The full pathname of the root file. </value>

        public string RootPath { get; }

        public Dictionary<string, List<ColorGroup>> ColorGroupsLookup { get; }

        public Dictionary<string, Dictionary<string, string>> PropertiesLookup { get; }

        public Dictionary<string, string> ThemeToSelectors { get; }
        public string CurrentImageFilePattern { get; set; }
        public string CurrentImagesPath { get; set; }

        public List<ImageInfo> ParsedImages { get; }
        public List<LinkInfo> ParsedLinks { get; }
        public GeneratorPass GeneratorPass { get; set; }

        public ObjectPropertiesDictionary ObjectProperties { get; }
        public string AboutThisImageHtmlTemplate { get; }
        public bool SuppressObjectPropertiesSave { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private Dictionary<string, object> keyValues;

        /// <summary>
        /// Indexer to get or set items within this collection using array index syntax.
        /// </summary>
        ///
        /// <param name="name"> The key. </param>
        ///
        /// <returns>   The indexed item. </returns>

        public object this[string name]
        {
            get
            {
                if (keyValues.ContainsKey(name))
                {
                    return keyValues[name];
                }
                else
                {
                    return null;
                }
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public ResourceDataClone(string rootPath, Dictionary<string, List<ColorGroup>> colorGroupsLookup, Dictionary<string, Dictionary<string, string>> propertiesLookup, Dictionary<string, string> themeToSelectors, Dictionary<string, object> keyValues)
        {
            this.RootPath = rootPath;
            this.ColorGroupsLookup = colorGroupsLookup;
            this.PropertiesLookup = propertiesLookup;
            this.ThemeToSelectors = themeToSelectors;
            this.keyValues = keyValues;
            this.ParsedImages = new List<ImageInfo>();
            this.ParsedLinks = new List<LinkInfo>();
            this.ObjectProperties = new ObjectPropertiesDictionary();
            this.AboutThisImageHtmlTemplate = typeof(ResourceDataClone).ReadResource<string>(@"Resources\AboutThisImage.html");

            if (keyValues.ContainsKey("ObjectProperties"))
            {
                var jObject = (JObject)keyValues["ObjectProperties"];

                this.ObjectProperties.AddRange(jObject.ToObject<Dictionary<string, dynamic>>(), true);
            }

            HandleRtf();
        }

        private void HandleRtf()
        {
            var endUserLicenseFile = this.EndUserLicense;
            var supportDetailsFile = this.SupportDetails;

            if (endUserLicenseFile != null)
            {
                var content = File.ReadAllText(endUserLicenseFile);

                this.keyValues["EndUserLicense"] = content;
            }

            if (supportDetailsFile != null)
            {
                var content = File.ReadAllText(supportDetailsFile);

                this.keyValues["SupportDetails"] = content;
            }
        }

        public void Dispose()
        {
        }

        public Color? FindColor(string resourceName)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<ColorGroup>> ApplyColorGroups()
        {
            var colorGroupsDictionary = new Dictionary<string, List<ColorGroup>>();

            foreach (var pair in this.ColorGroupsLookup)
            {
                var list = new List<ColorGroup>();

                foreach (var colorGroup in pair.Value)
                {
                    list.Add(colorGroup);
                }

                colorGroupsDictionary.Add(pair.Key, list);
            }

            return colorGroupsDictionary;
        }

        public string FindColor(string resourceName, string defaultColor)
        {
            var colorValue = (string) this[resourceName];

            if (colorValue != null)
            {
                var color = ColorTranslator.FromHtml(colorValue);

                return colorValue;
            }
            else
            {
                return defaultColor;
            }
        }

        public string FindColorRgb(string resourceName, string defaultRgb)
        {
            var colorValue = (string)this[resourceName];

            if (colorValue != null)
            {
                var color = ColorTranslator.FromHtml(colorValue);

                return string.Format("{0}, {1}, {2}", color.R, color.G, color.B);
            }
            else
            {
                return defaultRgb;
            }
        }

        public string GetHtmlContent(string resourceName)
        {
            string html = null;

            try
            {
                var resourceRtf = (string)this[resourceName];
                var cleanPattern = string.Format(this.CurrentImageFilePattern.RemoveStartAfterLastChar('/'), resourceName, @"(?<hex>[a-fA-F\d]{8})").RemoveEndAfterLastChar(')');
                var pattern = string.Format(this.CurrentImageFilePattern, resourceName, Guid.NewGuid().ToString().Left(8));
                var imageAdaptor = new RtfVisualImageAdapter(pattern, ImageFormat.Jpeg);
                var document = new HtmlDocument();
                var parsedImages = new List<ImageInfo>();
                var parsedLinks = new List<LinkInfo>();
                HtmlNode body;
                IEnumerable<HtmlNode> images;
                IEnumerable<HtmlNode> links;

                if (resourceRtf != null)
                {
                    var htmlConverterSettings = new RtfHtmlConvertSettings(imageAdaptor)
                    {
                        ImagesPath = this.CurrentImagesPath,
                        ConvertVisualHyperlinks = true
                    };
                    var imageConverterSettings = new RtfImageConvertSettings(imageAdaptor)
                    {
                        ImagesPath = this.CurrentImagesPath
                    };

                    html = resourceRtf.RtfToHtml(htmlConverterSettings, true, null, imageAdaptor, null, imageConverterSettings);

                    document.LoadHtml(html);

                    body = document.DocumentNode.Descendants("body").Single();
                    images = document.DocumentNode.Descendants("img");
                    links = document.DocumentNode.Descendants("a");

                    foreach (var image in images)
                    {
                        var imageInfo = new ImageInfo(image, resourceName, cleanPattern);

                        this.ParsedImages.Add(imageInfo);
                        parsedImages.Add(imageInfo);
                    }

                    foreach (var link in links)
                    {
                        var linkInfo = new LinkInfo(link, resourceName);

                        this.ParsedLinks.Add(linkInfo);
                        parsedLinks.Add(linkInfo);
                    }

                    ProcessLinksAndImages(resourceName, resourceRtf, parsedImages, parsedLinks);

                    return body.InnerHtml.ToBase64();
                }
                else
                {
                    return string.Empty;
                }

            }
            catch (Exception ex)
            {
                DebugUtils.Break();
            }

            return html;
        }

        public void ProcessLinksAndImages(string resourceName, string resourceRtf, List<ImageInfo> parsedImages, List<LinkInfo> parsedLinks)
        {
            var pattern = @"(?s)\{(?<image>\\pict\\.*?)\r\n.*?\}";
            var regex = new Regex(pattern);
            var matches = regex.Matches(resourceRtf).Cast<Match>().ToList();
            var x = 0;
            ObjectPropertiesDictionary objectProperties = null;

            if (resourceName == nameof(EndUserLicense) && this.ObjectProperties.ContainsKey("LicenseManagementObjectProperties"))
            {
                objectProperties = this.ObjectProperties["LicenseManagementObjectProperties"];
            }
            else if (resourceName == nameof(SupportDetails) && this.ObjectProperties.ContainsKey("SupportManagementObjectProperties"))
            {
                objectProperties = this.ObjectProperties["SupportManagementObjectProperties"];
            }

            foreach (var image in parsedImages)
            {
                var match = matches.ElementAt(x);
                var id = match.GetGroupValue("image");
                var imageNumber = string.Join(string.Empty, id.RegexGetMatches(@"\d+").Select(m => m.Value));
                var htmlImage = image.HtmlImage;

                ProcessImage(objectProperties, image, id, htmlImage);

                x++;
            }

            x = 0;

            foreach (var link in parsedLinks)
            {
                var htmlLink = link.HtmlLink;

                ProcessLink(objectProperties, link, htmlLink);

                x++;
            }
        }

        public void ProcessLink(ObjectPropertiesDictionary objectProperties, LinkInfo link, HtmlNode anchorNode)
        {
            if (objectProperties.ContainsKey(link.UrlPath))
            {
                var linkObjectProperties = (ObjectPropertiesDictionary)objectProperties[link.UrlPath];
                var baseObjectProperties = linkObjectProperties.Where(p => p.Key.IsOneOf(TypeExtensions.GetImmediatePublicPropertyNames<BaseObjectProperties>())).ToDictionary(p => p.Key, p => p.Value);
                var linkProperties = linkObjectProperties.Where(p => p.Key.IsOneOf(TypeExtensions.GetImmediatePublicPropertyNames<LinkProperties>()) && p.Value != null).ToList();

                foreach (var propertyPair in linkProperties)
                {
                    var propertyName = propertyPair.Key;
                    var propertyValue = ((object)propertyPair.Value).ToString();

                    switch (propertyName)
                    {
                        case "LinkCallToAction":
                            anchorNode.Attributes.Add("title", propertyValue);
                            anchorNode.Attributes.Add("target", "_blank");

                            break;
                    }
                }
            }
        }

        public ImageInfo CreateImage(string resourceName, string imageFilePath, string name, IGeneratorConfiguration generatorConfiguration)
        {
            var document = new HtmlDocument();
            var imageNode = document.CreateElement("img");
            var srcRoot = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Src];
            var assetImgs = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.AssetsImgs];
            var cleanPattern = string.Format(this.CurrentImageFilePattern, name, @"(?<hex>[a-fA-F\d]{8})");
            var fileName = string.Format(this.CurrentImageFilePattern, name, Guid.NewGuid().ToString().Left(8));
            var fileInfo = new FileInfo(Path.Combine(assetImgs.BackSlashes(), name, fileName + ".jpg"));
            string relativePath = null;
            ImageInfo imageInfo;
            HtmlNode parentNode;

            if (imageFilePath == null)
            {
                switch (resourceName)
                {
                    case "AboutBannerImage":
                        fileInfo = fileInfo.Directory.GetFiles().Where(f => f.Extension.IsOneOf(".jpg", ".png")).First();
                        break;

                    default:
                        DebugUtils.Break();
                        break;
                }
            }
            else
            {
                if (!Path.IsPathRooted(imageFilePath))
                {
                    imageFilePath = Path.Combine(this.RootPath, imageFilePath);
                }
            }

            parentNode = document.CreateElement("div");

            if (this.GeneratorPass == GeneratorPass.Files)
            {
                relativePath = fileInfo.GetRelativePath(srcRoot.BackSlashes());
                imageNode.Attributes.Add("src", "/" + relativePath.ReverseSlashes());
            }
            else
            {
                imageNode.Attributes.Add("src", "");
            }

            parentNode.ChildNodes.Add(imageNode);
            imageInfo = new ImageInfo(imageNode, resourceName, cleanPattern, imageFilePath, fileInfo.FullName);

            this.ParsedImages.Add(imageInfo);

            return imageInfo;
        }

        public void ProcessImage(ObjectPropertiesDictionary objectProperties, ImageInfo image, string id, HtmlNode htmlImage)
        {
            if (objectProperties.ContainsKey(id))
            {
                var imageObjectProperties = (ObjectPropertiesDictionary)objectProperties[id];
                var baseObjectProperties = imageObjectProperties.Where(p => p.Key.IsOneOf(TypeExtensions.GetImmediatePublicPropertyNames<BaseObjectProperties>())).ToDictionary(p => p.Key, p => p.Value);
                var imageProperties = imageObjectProperties.Where(p => p.Key.IsOneOf(TypeExtensions.GetImmediatePublicPropertyNames<ImageProperties>()) && p.Value != null).ToList();
                var name = string.Empty;
                var aboutThisImageTitle = string.Empty;

                image.ObjectProperties = imageObjectProperties;

                if (baseObjectProperties.ContainsKey("Name"))
                {
                    name = (string)baseObjectProperties["Name"];

                    if (name.IsNullOrEmpty())
                    {
                        name = Guid.NewGuid().ToString().Left(8);
                        aboutThisImageTitle = $"Click here to learn more about this image";
                    }
                    else
                    {
                        name = ((string)baseObjectProperties["Name"]).Replace(" ", "-");
                        aboutThisImageTitle = $"Click here to learn more about the { name } image";
                    }
                }
                else
                {
                    name = Guid.NewGuid().ToString().Left(8);
                    aboutThisImageTitle = $"Click here to learn more about this image";
                }

                foreach (var propertyPair in imageProperties)
                {
                    var propertyName = propertyPair.Key;
                    var propertyValue = ((object)propertyPair.Value).ToString();

                    switch (propertyName)
                    {
                        case "ImageId":
                            htmlImage.Attributes.Add("name", propertyValue);
                            break;
                        case "AlternativeText":
                            htmlImage.Attributes.Add("alt", propertyValue);
                            break;
                        case "LongDescription":

                            var parentNode = htmlImage.ParentNode;
                            var descriptionPageName = string.Format("AboutThisImage-{0}.html", name);
                            var anchorNode = parentNode.OwnerDocument.CreateElement("a");

                            anchorNode.Attributes.Add("href", descriptionPageName);
                            anchorNode.Attributes.Add("target", "_blank");
                            anchorNode.Attributes.Add("title", aboutThisImageTitle);

                            htmlImage.Remove();

                            parentNode.AppendChild(anchorNode);
                            anchorNode.AppendChild(htmlImage);

                            image.DescriptionPageName = descriptionPageName;
                            image.Description = propertyValue;
                            image.DescriptionHtml = this.CreateDescriptionHtml(image, name, baseObjectProperties, imageProperties);

                            break;
                    }
                }
            }
        }

        private string CreateDescriptionHtml(ImageInfo image, string name, Dictionary<string, dynamic> baseObjectProperties, List<KeyValuePair<string, dynamic>> imageProperties)
        {
            var html = this.AboutThisImageHtmlTemplate;

            html = html.Replace("$title", string.Format("Description about the {0} image", name));
            html = html.Replace("$themeColor", this.PrimaryColor);
            html = html.Replace("$metaDesc", string.Format("Description about the {0} image for the {1} product from {2}. {3}", name, this.AppName, this.OrganizationName, this.AppDescription));
            html = html.Replace("$metaKeyWords", "todo");
            html = html.Replace("$metaAuthor", this.OrganizationName);
            html = html.Replace("$imageUrl", image.UrlPath);
            html = html.Replace("$imageAlt", image.Alt);
            html = html.Replace("$longDesc", image.Description);

            return html;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public bool AskClose()
        {
            throw new NotImplementedException();
        }

        public void InitiateSaveBackup(Action<string, IResourceData> beforeExecuteCallback)
        {
            throw new NotImplementedException();
        }

        public void InitiateRestoreBackup(Action<string, IResourceData> beforeExecuteCallback)
        {
            throw new NotImplementedException();
        }

        public void InitiateNew(Action<string, IResourceData> beforeExecuteCallback)
        {
            throw new NotImplementedException();
        }
    }
}
