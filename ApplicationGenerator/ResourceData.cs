// file:	ResourceData.cs
//
// summary:	Implements the resource data class

using Newtonsoft.Json.Linq;
using SassParser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   A resource data. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>

    public class ResourceData : IResourceData
    {
        /// <summary>   The sass content. </summary>
        private string sassContent;
        /// <summary>   Manager for resource. </summary>
        private ResourceManager resourceManager;

        /// <summary>   Gets or sets a value indicating whether this  is dirty. </summary>
        ///
        /// <value> True if dirty, false if not. </value>

        public bool Dirty { get; set; }

        /// <summary>   Gets the key values. </summary>
        ///
        /// <value> The key values. </value>

        public Dictionary<string, object> KeyValues { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the suppress object properties save.
        /// </summary>
        ///
        /// <value> True if suppress object properties save, false if not. </value>

        public bool SuppressObjectPropertiesSave { get; set; }

        /// <summary>   Gets the groups the color belongs to. </summary>
        ///
        /// <value> The color groups. </value>

        public Dictionary<string, List<ColorGroup>> ColorGroupsLookup { get; }
        /// <summary>   The selected theme. </summary>
        public string SelectedTheme = "Root";
        /// <summary>   The selected platform. </summary>
        public string SelectedPlatform = "Default";

        /// <summary>   Gets the theme to selectors. </summary>
        ///
        /// <value> The theme to selectors. </value>

        public Dictionary<string, string> ThemeToSelectors { get; }

        /// <summary>   Gets the properties lookup. </summary>
        ///
        /// <value> The properties lookup. </value>

        public Dictionary<string, Dictionary<string, string>> PropertiesLookup { get; }

        /// <summary>   Gets the object properties. </summary>
        ///
        /// <value> The object properties. </value>

        public ObjectPropertiesDictionary ObjectProperties { get; }

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
                return this.resourceManager.DenormalizePath((string)this[nameof(SplashScreen)]);
            }

            set
            {
                this[nameof(SplashScreen)] = this.resourceManager.NormalizePath(value);
            }
        }

        /// <summary>   Gets or sets the about banner. </summary>
        ///
        /// <value> The about banner. </value>

        public string AboutBanner
        {
            get
            {
                return this.resourceManager.DenormalizePath((string)this[nameof(AboutBanner)]);
            }

            set
            {
                this[nameof(AboutBanner)] = this.resourceManager.NormalizePath(value);
            }
        }

        /// <summary>   Gets or sets the logo. </summary>
        ///
        /// <value> The logo. </value>

        public string Logo
        {
            get
            {
                return this.resourceManager.DenormalizePath((string)this[nameof(Logo)]);
            }

            set
            {
                this[nameof(Logo)] = this.resourceManager.NormalizePath(value);
            }
        }

        /// <summary>   Gets or sets the icon. </summary>
        ///
        /// <value> The icon. </value>

        public string Icon
        {
            get
            {
                return this.resourceManager.DenormalizePath((string)this[nameof(Icon)]);
            }

            set
            {
                this[nameof(Icon)] = this.resourceManager.NormalizePath(value);
            }
        }

        /// <summary>   Gets or sets the end user license. </summary>
        ///
        /// <value> The end user license. </value>

        public string EndUserLicense
        {
            get
            {
                return this.resourceManager.DenormalizePath((string)this[nameof(EndUserLicense)]);
            }

            set
            {
                resourceManager.ReportResourceChange(value, "Setting file path to {0}", value);

                this[nameof(EndUserLicense)] = this.resourceManager.NormalizePath(value);
            }
        }

        /// <summary>   Gets or sets the support details. </summary>
        ///
        /// <value> The support details. </value>

        public string SupportDetails
        {
            get
            {
                return this.resourceManager.DenormalizePath((string)this[nameof(SupportDetails)]);
            }

            set
            {
                this[nameof(SupportDetails)] = this.resourceManager.NormalizePath(value);
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

        public string RootPath
        {
            get
            {
                return this.resourceManager.RootPath;
            }
        }

        /// <summary>   Gets or sets the current image file pattern. </summary>
        ///
        /// <value> The current image file pattern. </value>

        public string CurrentImageFilePattern { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>   Gets or sets the full pathname of the current RTF images file. </summary>
        ///
        /// <value> The full pathname of the current RTF images file. </value>

        public string CurrentImagesPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
                if (this.KeyValues.ContainsKey(name))
                {
                    return this.KeyValues[name];
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (this.KeyValues.ContainsKey(name))
                {
                    this.KeyValues[name] = value;
                }
                else
                {
                    this.KeyValues.Add(name, value);
                }

                resourceManager.Update(name, value);

                this.Dirty = true;
            }
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/30/2020. </remarks>
        ///
        /// <param name="sassContent">      The sass content. </param>
        /// <param name="resourceManager">  Manager for resource. </param>
        /// <param name="keyValues">        The key values. </param>

        public ResourceData(string sassContent, ResourceManager resourceManager, Dictionary<string, object> keyValues)
        {
            var parser = new SassParser.StylesheetParser(true, true, true, true, true, true);
            Stylesheet stylesheet;
            ColorGroup colorGroup = null;

            parser.Error += Parser_Error;

            stylesheet = parser.Parse(sassContent);

            this.sassContent = sassContent;
            this.resourceManager = resourceManager;
            this.KeyValues = keyValues;
            this.SelectedTheme = "Root";

            this.ColorGroupsLookup = new Dictionary<string, List<ColorGroup>>();
            this.PropertiesLookup = new Dictionary<string, Dictionary<string, string>>();
            this.ObjectProperties = new ObjectPropertiesDictionary();
            this.ThemeToSelectors = new Dictionary<string, string>();

            this.ObjectProperties.OnChanged += (sender, e) =>
            {
                if (!this.SuppressObjectPropertiesSave)
                {
                    this["ObjectProperties"] = this.ObjectProperties;
                }
            };

            if (keyValues.ContainsKey("ObjectProperties"))
            {
                var jObject = (JObject) keyValues["ObjectProperties"];

                this.SuppressObjectPropertiesSave = true;

                this.ObjectProperties.AddRange(jObject.ToObject<Dictionary<string, dynamic>>(), true);

                this.SuppressObjectPropertiesSave = false;
            }

            foreach (var styleRule in stylesheet.StyleRules.Cast<StyleRule>())
            {
                var selector = styleRule.Selector;
                var style = styleRule.Style;
                string theme;
                List<ColorGroup> colorGroups;
                Dictionary<string, string> properties;

                if (!styleRule.SelectorText.StartsWith(".") && !styleRule.SelectorText.StartsWith(":"))
                {
                    theme = string.Join(" ", styleRule.SelectorText.Split('-', '.').Select(s => s.ToTitleCase())).Trim();

                    if (this.PropertiesLookup.ContainsKey(theme))
                    {
                        properties = this.PropertiesLookup[theme];
                    }
                    else
                    {
                        properties = new Dictionary<string, string>();
                        this.PropertiesLookup.Add(theme, properties);
                        this.ThemeToSelectors.Add(styleRule.SelectorText, theme);
                    }

                    foreach (var node in style.Children)
                    {
                        switch (node)
                        {
                            case Property property:
                                {
                                    var expression = @"^(?<groupName>\w+)(-(?<offsetColorName>\w+))?$";
                                    var propertyName = property.Name.RemoveText("--ion").RemoveText("-default-");
                                    var pattern = @"var\((?<variable>.*?)\)";
                                    System.Drawing.Color? color = null;

                                    if (property.Value.RegexIsMatch(pattern))
                                    {
                                        var variable = property.Value.RegexGet(pattern, "variable");
                                        var varPropertyName = variable.RemoveText("--ion-color-");
                                        var varPropertyOffsetColorName = varPropertyName.RegexGet(expression, "offsetColorName").ToTitleCase();

                                        switch (varPropertyOffsetColorName)
                                        {
                                            case null:
                                            case "":
                                            case "base":
                                                color = this.ColorGroupsLookup[this.SelectedTheme].Single(c => c.BaseColorPropertyName == variable).BaseColor;
                                                break;
                                            case "Contrast":
                                                color = this.ColorGroupsLookup.SelectMany(l => l.Value).Single(c => c.ContrastPropertyName == variable).Contrast;
                                                break;
                                            case "Shade":
                                                color = this.ColorGroupsLookup.SelectMany(l => l.Value).Single(c => c.ShadePropertyName == variable).Shade;
                                                break;
                                            case "Tint":
                                                color = this.ColorGroupsLookup.SelectMany(l => l.Value).Single(c => c.TintPropertyName == variable).Tint;
                                                break;
                                            case "Rgb":
                                                break;
                                            default:
                                                DebugUtils.Break();
                                                break;
                                        }
                                    }

                                    properties.Add(propertyName, property.Value);
                                    break;
                                }
                            default:
                                break;
                                DebugUtils.Break();
                        }
                    }
                }
                else
                {
                    theme = string.Join(" ", styleRule.SelectorText.RemoveStart(1).RemoveStartIfMatches("ion-color-").Split('-', '.').Select(s => s.ToTitleCase())).Trim();

                    if (this.ColorGroupsLookup.ContainsKey(theme))
                    {
                        colorGroups = this.ColorGroupsLookup[theme];
                    }
                    else
                    {
                        colorGroups = new List<ColorGroup>();
                        this.ColorGroupsLookup.Add(theme, colorGroups);
                        this.ThemeToSelectors.Add(styleRule.SelectorText, theme);
                    }

                    foreach (var node in style.Children)
                    {
                        switch (node)
                        {
                            case Comment comment:

                                var commentText = comment.Data.RemoveText("*").Trim().ToTitleCase();

                                colorGroup = new ColorGroup(commentText + "Color");
                                colorGroups.Add(colorGroup);

                                break;

                            case Property property:

                                var expression = @"^(?<groupName>\w+)(-(?<offsetColorName>\w+))?$";
                                var propertyName = property.Name.RemoveText("--ion-color-");
                                var propertyOffsetColorName = propertyName.RegexGet(expression, "offsetColorName").ToTitleCase();
                                var pattern = @"var\((?<variable>.*?)\)";
                                string lookupVariable = null;
                                System.Drawing.Color? color = null;

                                if (propertyName.EndsWith("-rgb"))
                                {
                                    continue;
                                }

                                if (property.Value.RegexIsMatch(pattern))
                                {
                                    var variable = property.Value.RegexGet(pattern, "variable");
                                    var varPropertyName = variable.RemoveText("--ion-color-");
                                    var varPropertyOffsetColorName = varPropertyName.RegexGet(expression, "offsetColorName").ToTitleCase();

                                    switch (varPropertyOffsetColorName)
                                    {
                                        case null:
                                        case "":
                                        case "base":
                                            color = this.ColorGroupsLookup.SelectMany(l => l.Value).Single(c => c.BaseColorPropertyName == variable).BaseColor;
                                            break;
                                        case "Contrast":
                                            color = this.ColorGroupsLookup.SelectMany(l => l.Value).Single(c => c.ContrastPropertyName == variable).Contrast;
                                            break;
                                        case "Shade":
                                            color = this.ColorGroupsLookup.SelectMany(l => l.Value).Single(c => c.ShadePropertyName == variable).Shade;
                                            break;
                                        case "Tint":
                                            color = this.ColorGroupsLookup.SelectMany(l => l.Value).Single(c => c.TintPropertyName == variable).Tint;
                                            break;
                                        case "Rgb":
                                            break;
                                        default:
                                            DebugUtils.Break();
                                            break;
                                    }

                                    if (varPropertyName.EndsWith("-rgb"))
                                    {
                                        continue;
                                    }

                                    lookupVariable = variable;
                                }
                                else
                                {
                                    color = ColorTranslator.FromHtml(property.Value);
                                }

                                if (color == null)
                                {
                                    continue;
                                }

                                switch (propertyOffsetColorName)
                                {
                                    case null:
                                    case "":
                                    case "base":
                                        colorGroup.BaseColor = color.Value;
                                        colorGroup.BaseColorPropertyName = property.Name;
                                        colorGroup.BaseColorLookupVariable = lookupVariable;
                                        break;
                                    case "Contrast":
                                        colorGroup.Contrast = color.Value;
                                        colorGroup.ContrastPropertyName = property.Name;
                                        colorGroup.ContrastColorLookupVariable = lookupVariable;
                                        break;
                                    case "Shade":
                                        colorGroup.Shade = color.Value;
                                        colorGroup.ShadePropertyName = property.Name;
                                        colorGroup.ShadeColorLookupVariable = lookupVariable;
                                        break;
                                    case "Tint":
                                        colorGroup.Tint = color.Value;
                                        colorGroup.TintPropertyName = property.Name;
                                        colorGroup.TintColorVariable = lookupVariable;
                                        break;
                                    case "Rgb":
                                        break;
                                    default:
                                        colorGroup.Others.Add(propertyName, color.Value);
                                        break;
                                }

                                break;
                        }
                    }
                }
            }

            UpdateProperties();
        }

        private void Parser_Error(object sender, TokenizerError e)
        {
        }

        /// <summary>   Updates the properties. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>

        private void UpdateProperties()
        {
            var colorGroups = this.ColorGroupsLookup[this.SelectedTheme];
            var properties = this.PropertiesLookup[this.SelectedPlatform];

            foreach (var colorGroup in colorGroups)
            {
                if (!this.KeyValues.ContainsKey(colorGroup.Name))
                {
                    this.KeyValues.Add(colorGroup.Name, ColorTranslator.ToHtml(colorGroup.BaseColor));
                }
            }

            foreach (var property in properties)
            {
                if (!this.KeyValues.ContainsKey(property.Key))
                {
                    this.KeyValues.Add(property.Key, property.Value);
                }
            }
        }

        /// <summary>   Searches for the first color. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>
        ///
        /// <param name="resourceName"> Name of the resource. </param>
        ///
        /// <returns>   The found color. </returns>

        public System.Drawing.Color? FindColor(string resourceName)
        {
            var expression = @"^(?<groupName>\w+)(-(?<offsetColorName>\w+))?$";
            var groupName = resourceName.RegexGet(expression, "groupName");
            var offsetColorName = resourceName.RegexGet(expression, "offsetColorName");
            var colorGroups = this.ColorGroupsLookup[this.SelectedTheme];
            System.Drawing.Color? color = null;

            foreach (var colorGroup in colorGroups)
            {
                if (groupName == colorGroup.Name)
                {
                    switch (offsetColorName)
                    {
                        case "":
                            color = colorGroup.BaseColor;
                            break;
                        case "Contrast":
                            color = colorGroup.Contrast;
                            break;
                        case "Shade":
                            color = colorGroup.Shade;
                            break;
                        case "Tint":
                            color = colorGroup.Tint;
                            break;
                        default:
                            DebugUtils.Break();
                            color = System.Drawing.Color.Transparent;
                            break;
                    }
                }
            }

            return color;
        }

        /// <summary>   Determines if we can ask close. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/13/2021. </remarks>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool AskClose()
        {
            if (resourceManager != null)
            {
                return resourceManager.AskClose();
            }

            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/5/2020. </remarks>

        public void Dispose()
        {
            resourceManager.FreeResourceData();
            resourceManager.Dispose();
        }

        /// <summary>   Saves this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2021. </remarks>

        public void Save()
        {
            resourceManager.Save();
        }

        /// <summary>   Applies the color groups. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <returns>   A list of. </returns>

        public Dictionary<string, List<ColorGroup>> ApplyColorGroups()
        {
            throw new NotImplementedException();
        }

        /// <summary>   Searches for the first color. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <param name="resourceName"> Name of the resource. </param>
        /// <param name="defaultColor"> The default color. </param>
        ///
        /// <returns>   The found color. </returns>

        public string FindColor(string resourceName, string defaultColor)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Searches for the first color RGB. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <param name="resourceName"> Name of the resource. </param>
        /// <param name="defaultColor"> The default color. </param>
        ///
        /// <returns>   The found color RGB. </returns>

        public string FindColorRgb(string resourceName, string defaultColor)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Gets HTML content. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <param name="resourceName"> Name of the resource. </param>
        ///
        /// <returns>   The HTML content. </returns>

        public string GetHtmlContent(string resourceName)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Saves the backup. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>

        public void InitiateSaveBackup(Action<string, IResourceData> beforeExecuteCallback)
        {
            this.resourceManager.InitiateSaveBackup(beforeExecuteCallback);
        }

        /// <summary>   Restore backup. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>

        public void InitiateRestoreBackup(Action<string, IResourceData> beforeExecuteCallback)
        {
            this.resourceManager.InitiateRestoreBackup(beforeExecuteCallback);
        }

        /// <summary>   Initiate new. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>

        public void InitiateNew(Action<string, IResourceData> beforeExecuteCallback)
        {
            this.resourceManager.InitiateNew(beforeExecuteCallback);
        }
    }
}
