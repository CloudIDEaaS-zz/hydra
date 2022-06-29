// file:	ResourceData.cs
//
// summary:	Implements the resource data class

using AbstraX.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AbstraX
{
    /// <summary>   Interface for resource data processed. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/2/2021. </remarks>
    ///
    /// <typeparam name="TImage">   Generic type parameter. </typeparam>
    /// <typeparam name="TLink">    Type of the link. </typeparam>

    public interface IResourceDataProcessed<TImage, TLink> : IResourceData
    {
        /// <summary>   Gets or sets the generator pass. </summary>
        ///
        /// <value> The generator pass. </value>

        GeneratorPass GeneratorPass { get; set; }
        /// <summary>   Gets the parsed images. </summary>
        ///
        /// <value> The parsed images. </value>

        List<TImage> ParsedImages { get; }

        /// <summary>   Gets the parsed links. </summary>
        ///
        /// <value> The parsed links. </value>

        List<TLink> ParsedLinks { get; }
    }

    /// <summary>   Interface for resource data. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 2/28/2021. </remarks>

    public interface IResourceData : IDisposable
    {
        /// <summary>   Saves the backup. </summary>
        void InitiateSaveBackup(Action<string, IResourceData> beforeExecuteCallback);
        /// <summary>   Restore backup. </summary>
        void InitiateRestoreBackup(Action<string, IResourceData> beforeExecuteCallback);

        /// <summary>   Initiate new. </summary>
        ///
        /// <param name="beforeExecuteCallback">    The before execute callback. </param>

        void InitiateNew(Action<string, IResourceData> beforeExecuteCallback);

        /// <summary>
        /// Gets or sets a value indicating whether the suppress object properties save.
        /// </summary>
        ///
        /// <value> True if suppress object properties save, false if not. </value>

        bool SuppressObjectPropertiesSave { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is dirty. </summary>
        ///
        /// <value> True if dirty, false if not. </value>

        bool Dirty { get; set; }

        /// <summary>   Determines if we can ask close. </summary>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool AskClose();
        /// <summary>
        /// Indexer to get or set items within this collection using array index syntax.
        /// </summary>
        ///
        /// <param name="name"> The name. </param>
        ///
        /// <returns>   The indexed item. </returns>

        object this[string name] { get; set; }

        /// <summary>   Gets or sets the about banner. </summary>
        ///
        /// <value> The about banner. </value>

        string AboutBanner { get; set; }

        /// <summary>   Gets or sets information describing the application. </summary>
        ///
        /// <value> Information describing the application. </value>

        string AppDescription { get; set; }

        /// <summary>   Gets or sets the name of the application. </summary>
        ///
        /// <value> The name of the application. </value>

        string AppName { get; set; }

        /// <summary>   Gets or sets the color of the background. </summary>
        ///
        /// <value> The color of the background. </value>

        string BackgroundColor { get; set; }

        /// <summary>   Gets the color groups lookup. </summary>
        ///
        /// <value> The color groups lookup. </value>

        Dictionary<string, List<ColorGroup>> ColorGroupsLookup { get; }

        /// <summary>   Gets or sets the end user license. </summary>
        ///
        /// <value> The end user license. </value>

        string EndUserLicense { get; set; }

        /// <summary>   Gets or sets the end user license email address. </summary>
        ///
        /// <value> The end user license email address. </value>

        string EndUserLicenseEmailAddress { get; set; }

        /// <summary>   Gets or sets the end user license mailing address. </summary>
        ///
        /// <value> The end user license mailing address. </value>

        string EndUserLicenseMailingAddress { get; set; }

        /// <summary>   Gets or sets URL of the end user license. </summary>
        ///
        /// <value> The end user license URL. </value>

        string EndUserLicenseUrl { get; set; }

        /// <summary>   Gets or sets the icon. </summary>
        ///
        /// <value> The icon. </value>

        string Icon { get; set; }

        /// <summary>   Gets or sets the logo. </summary>
        ///
        /// <value> The logo. </value>

        string Logo { get; set; }

        /// <summary>   Gets or sets the name of the organization. </summary>
        ///
        /// <value> The name of the organization. </value>

        string OrganizationName { get; set; }

        /// <summary>   Gets or sets the color of the primary. </summary>
        ///
        /// <value> The color of the primary. </value>

        string PrimaryColor { get; set; }

        /// <summary>   Gets the properties lookup. </summary>
        ///
        /// <value> The properties lookup. </value>

        Dictionary<string, Dictionary<string, string>> PropertiesLookup { get; }

        /// <summary>   Gets the object properties. </summary>
        ///
        /// <value> The object properties. </value>

        ObjectPropertiesDictionary ObjectProperties { get; }

        /// <summary>   Gets the full pathname of the root file. </summary>
        ///
        /// <value> The full pathname of the root file. </value>

        string RootPath { get; }

        /// <summary>   Gets or sets the color of the secondary. </summary>
        ///
        /// <value> The color of the secondary. </value>

        string SecondaryColor { get; set; }

        /// <summary>   Gets or sets the splash screen. </summary>
        ///
        /// <value> The splash screen. </value>

        string SplashScreen { get; set; }

        /// <summary>   Gets or sets the support details. </summary>
        ///
        /// <value> The support details. </value>

        string SupportDetails { get; set; }

        /// <summary>   Gets or sets the support email address. </summary>
        ///
        /// <value> The support email address. </value>

        string SupportEmailAddress { get; set; }

        /// <summary>   Gets or sets URL of the support. </summary>
        ///
        /// <value> The support URL. </value>

        string SupportUrl { get; set; }

        /// <summary>   Gets or sets the color of the tertiary. </summary>
        ///
        /// <value> The color of the tertiary. </value>

        string TertiaryColor { get; set; }

        /// <summary>   Gets or sets the current image file pattern. </summary>
        ///
        /// <value> The current image file pattern. </value>

        string CurrentImageFilePattern { get; set; }

        /// <summary>   Gets or sets the full pathname of the current RTF images file. </summary>
        ///
        /// <value> The full pathname of the current RTF images file. </value>

        string CurrentImagesPath { get; set; }

        /// <summary>   Gets the theme to selectors. </summary>
        ///
        /// <value> The theme to selectors. </value>

        Dictionary<string, string> ThemeToSelectors { get; }

        /// <summary>   Applies the color groups. </summary>
        ///
        /// <returns>   A list of. </returns>

        Dictionary<string, List<ColorGroup>> ApplyColorGroups();

        /// <summary>   Searches for the first color. </summary>
        ///
        /// <param name="resourceName"> Name of the resource. </param>
        ///
        /// <returns>   The found color. </returns>

        Color? FindColor(string resourceName);

        /// <summary>   Searches for the first color. </summary>
        ///
        /// <param name="resourceName"> Name of the resource. </param>
        /// <param name="defaultColor"> The default color. </param>
        ///
        /// <returns>   The found color. </returns>

        string FindColor(string resourceName, string defaultColor);

        /// <summary>   Searches for the first color RGB. </summary>
        ///
        /// <param name="resourceName"> Name of the resource. </param>
        /// <param name="defaultColor"> The default color. </param>
        ///
        /// <returns>   The found color RGB. </returns>

        string FindColorRgb(string resourceName, string defaultColor);

        /// <summary>   Gets HTML content. </summary>
        ///
        /// <param name="resourceName"> Name of the resource. </param>
        ///
        /// <returns>   The HTML content. </returns>

        string GetHtmlContent(string resourceName);
        /// <summary>   Saves this.  </summary>
        void Save();
    }
}