// file:	frmAppDesigner.cs
//
// summary:	Implements the form application designer class

using AbstraX.Resources;
using ApplicationGenerator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using WizardBase;

namespace AbstraX
{
    /// <summary>   Designer for Form Application. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>

    public partial class ctrlAppDesigner : UserControl, IWizardStepDesign
    {
        private PrivateFontCollection fontCollection;
        private Dictionary<string, Bitmap> imageCollection;
        private ActionQueueService actionQueueService;
        private IManagedLockObject lockObject;
        private bool inRefresh;
        private bool initialized;
        private ResourceDefaults resourceDefaults;

        /// <summary>   Event queue for all listeners interested in DisableNext events. </summary>
        public event EventHandler DisableNext;
        /// <summary>   Event queue for all listeners interested in EnableNext events. </summary>
        public event EventHandler EnableNext;
        /// <summary>   Event queue for all listeners interested in OnPageValidated events. </summary>
        public event EventHandler OnPageValid;
        /// <summary>   Event queue for all listeners interested in OnPageInvalid events. </summary>
        public event EventHandlerT<Exception> OnPageInvalid;

        /// <summary>   Gets or sets information describing the resource. </summary>
        ///
        /// <value> Information describing the resource. </value>

        public IResourceData ResourceData { get; set; }

        /// <summary>   Gets or sets the pathname of the local theme folder. </summary>
        ///
        /// <value> The pathname of the local theme folder. </value>

        public string LocalThemeFolder { get; set; }

        /// <summary>   Gets or sets the pathname of the working directory. </summary>
        ///
        /// <value> The pathname of the working directory. </value>

        public string WorkingDirectory { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is shown. </summary>
        ///
        /// <value> True if shown, false if not. </value>

        public bool Shown { get; set; }
        public bool UserValidated { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>
        public ctrlAppDesigner()
        {
            this.fontCollection = new PrivateFontCollection();
            this.imageCollection = new Dictionary<string, Bitmap>();
            this.actionQueueService = new ActionQueueService();

            lockObject = LockManager.CreateObject();

            InitializeComponent();
        }

        /// <summary>   Determine if we can close. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/13/2021. </remarks>
        ///
        /// <returns>   True if we can close, false if not. </returns>

        public bool CanClose()
        {
            if (this.ResourceData != null)
            {
                return this.ResourceData.AskClose();
            }

            return true;
        }

        /// <summary>   Initializes the control. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/23/2021. </remarks>

        public void InitializeControl(string workingDirectory)
        {
            if (this.initialized)
            {
                UpdateControl(workingDirectory);
            }
            else
            {
                this.WorkingDirectory = workingDirectory;

                actionQueueService.Start();
            }
        }

        /// <summary>   Updates the control described by workingDirectory. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>
        ///
        /// <param name="workingDirectory"> The pathname of the working directory. </param>

        public void UpdateControl(string workingDirectory)
        {
            var type = this.GetType();
            var assembly = type.Assembly;
            var directory = new DirectoryInfo(this.LocalThemeFolder);
            var fontDirectory = directory.GetDirectories("Fonts").Single();
            var primaryColor = ColorTranslator.FromHtml(this.ResourceData.PrimaryColor);
            var secondaryColor = ColorTranslator.FromHtml(this.ResourceData.SecondaryColor);
            var tertiaryColor = ColorTranslator.FromHtml(this.ResourceData.TertiaryColor);
            var backgroundColor = ColorTranslator.FromHtml(this.ResourceData.BackgroundColor);

            initialized = false;

            resourceDefaults = AbstraXExtensions.GetResourceDefaults();

            this.fontCollection = new PrivateFontCollection();
            this.imageCollection = new Dictionary<string, Bitmap>();

            resourceDataBindingSource.DataSource = this.ResourceData;

            foreach (var fontTypefolder in fontDirectory.GetDirectories())
            {
                foreach (var fontFamilyFolder in fontTypefolder.GetDirectories())
                {
                    foreach (var file in fontFamilyFolder.GetFiles("*.ttf"))
                    {
                        fontCollection.AddFontFile(file.FullName);
                    }
                }
            }

            foreach (var name in assembly.GetManifestResourceNames().Where(n => n.StartsWith("AbstraX.Themes.Images.")).OrderBy(n => n))
            {
                var image = type.ReadResource<Bitmap>(name);
                var pattern = @"^AbstraX\.Themes\.Images\.(?<name>.*)\.png$";
                var imageName = name.RegexGet(pattern, "name");

                imageCollection.Add(imageName, image);
            }

            colorPickerPrimary.ResetColors();
            colorPickerSecondary.ResetColors();
            colorPickerTertiary.ResetColors();
            colorPickerBackground.ResetColors();

            colorPickerPrimary.SetColor(primaryColor, "Primary Color");
            colorPickerSecondary.SetColor(secondaryColor, "Secondary Color");
            colorPickerTertiary.SetColor(tertiaryColor, "Tertiary Color");
            colorPickerBackground.SetColor(backgroundColor, "Background Color");

            ShowImageOrDocument(tabControlResources.SelectedIndex);
            this.ValidatePage();

            initialized = true;

            using (lockObject.Lock())
            {
                if (inRefresh)
                {
                    return;
                }

                inRefresh = true;
            }

            this.Invoke(() =>
            {
                foreach (var screenPanel in appScreensPanelDevice.Controls.Cast<AppScreenPanel>())
                {
                    screenPanel.ImageCollection = this.imageCollection;
                    screenPanel.ResourceData = this.ResourceData;

                    screenPanel.Refresh();
                }

                foreach (var screenPanel in appScreenPanelBrowser.Controls.Cast<AppScreenPanel>())
                {
                    screenPanel.ImageCollection = this.imageCollection;
                    screenPanel.ResourceData = this.ResourceData;

                    screenPanel.Refresh();
                }
            });

            using (lockObject.Lock())
            {
                inRefresh = false;
            }
        }

        /// <summary>   Event handler. Called by frmAppDesigner for load events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void ctrlAppDesigner_Load(object sender, EventArgs e)
        {
            var type = this.GetType();
            var assembly = type.Assembly;
            var directory = new DirectoryInfo(this.LocalThemeFolder);
            var fontDirectory = directory.GetDirectories("Fonts").Single();
            var primaryColor = ColorTranslator.FromHtml(this.ResourceData.PrimaryColor);
            var secondaryColor = ColorTranslator.FromHtml(this.ResourceData.SecondaryColor);
            var tertiaryColor = ColorTranslator.FromHtml(this.ResourceData.TertiaryColor);
            var backgroundColor = ColorTranslator.FromHtml(this.ResourceData.BackgroundColor);
            AppScreenPanel previousPanel = null;

            resourceDefaults = AbstraXExtensions.GetResourceDefaults();

            toolStrip.EnableClickOnActivate();

            resourceDataBindingSource.DataSource = this.ResourceData;

            foreach (var fontTypefolder in fontDirectory.GetDirectories())
            {
                foreach (var fontFamilyFolder in fontTypefolder.GetDirectories())
                {
                    foreach (var file in fontFamilyFolder.GetFiles("*.ttf"))
                    {
                        fontCollection.AddFontFile(file.FullName);
                    }
                }
            }

            foreach (var name in assembly.GetManifestResourceNames().Where(n => n.StartsWith("AbstraX.Themes.Images.")).OrderBy(n => n))
            {
                var image = type.ReadResource<Bitmap>(name);
                var pattern = @"^AbstraX\.Themes\.Images\.(?<name>.*)\.png$";
                var imageName = name.RegexGet(pattern, "name");

                if (!imageCollection.ContainsKey(imageName))
                {
                    imageCollection.Add(imageName, image);
                }
            }

            foreach (var name in assembly.GetManifestResourceNames().Where(n => n.StartsWith("AbstraX.Screens.")).OrderBy(n => n))
            {
                var image = type.ReadResource<Bitmap>(name);
                var isBrowser = name.EndsWith("Browser.png");
                var screenPanel = new AppScreenPanel(name, image, fontCollection, imageCollection, this.ResourceData, isBrowser);

                if (name.EndsWith("Browser.png"))
                {
                    screenPanel.IsBrowser = true;
                    appScreenPanelBrowser.Controls.Add(screenPanel);
                }
                else
                {
                    appScreensPanelDevice.Controls.Add(screenPanel);
                }

                if (previousPanel != null)
                {
                    screenPanel.PreviousPanel = previousPanel;
                }

                previousPanel = screenPanel;
            }

            colorPickerPrimary.SetColor(primaryColor, "Primary Color");
            colorPickerSecondary.SetColor(secondaryColor, "Secondary Color");
            colorPickerTertiary.SetColor(tertiaryColor, "Tertiary Color");
            colorPickerBackground.SetColor(backgroundColor, "Background Color");

            ShowImageOrDocument();

            initialized = true;

            this.ValidatePage();
        }

        /// <summary>   Text changed. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/1/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void ResourceTextChanged(object sender, EventArgs e)
        {
            if (!initialized)
            {
                return;
            }

            switch (sender)
            {
                case TextBox textBox:

                    actionQueueService.Run(() =>
                    {
                        using (lockObject.Lock())
                        {
                            if (inRefresh)
                            {
                                return;
                            }

                            inRefresh = true;
                        }

                        this.Invoke(() =>
                        {
                            foreach (var screenPanel in appScreensPanelDevice.Controls.Cast<AppScreenPanel>())
                            {
                                screenPanel.Refresh();
                            }

                            foreach (var screenPanel in appScreenPanelBrowser.Controls.Cast<AppScreenPanel>())
                            {
                                screenPanel.Refresh();
                            }
                        });

                        using (lockObject.Lock())
                        {
                            inRefresh = false;
                        }

                    }, textBox);

                    break;
            }

            this.ValidatePage();
        }

        private void ColorIndexChanged(object sender, EventArgs e)
        {
            if (!initialized)
            {
                return;
            }

            switch (sender)
            {
                case ColorPicker colorPicker:

                    switch (colorPicker.Name)
                    {
                        case "colorPickerPrimary":
                            ResourceData.PrimaryColor = ColorTranslator.ToHtml(colorPickerPrimary.SelectedColor.Value);
                            break;
                        case "colorPickerSecondary":
                            ResourceData.SecondaryColor = ColorTranslator.ToHtml(colorPickerSecondary.SelectedColor.Value);
                            break;
                        case "colorPickerTertiary":
                            ResourceData.TertiaryColor = ColorTranslator.ToHtml(colorPickerTertiary.SelectedColor.Value);
                            break;
                        case "colorPickerBackground":
                            ResourceData.BackgroundColor = ColorTranslator.ToHtml(colorPickerBackground.SelectedColor.Value);
                            break;
                    }

                    actionQueueService.Run(() =>
                    {
                        using (lockObject.Lock())
                        {
                            if (inRefresh)
                            {
                                return;
                            }

                            inRefresh = true;
                        }

                        this.Invoke(() =>
                        {
                            foreach (var screenPanel in appScreensPanelDevice.Controls.Cast<AppScreenPanel>())
                            {
                                screenPanel.Refresh();
                            }

                            foreach (var screenPanel in appScreenPanelBrowser.Controls.Cast<AppScreenPanel>())
                            {
                                screenPanel.Refresh();
                            }
                        });

                        using (lockObject.Lock())
                        {
                            inRefresh = false;
                        }

                    }, colorPicker);

                    break;
            }
        }

        /// <summary>   Form closing. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/23/2021. </remarks>

        public void FormClosing()
        {
            foreach (var image in this.imageCollection.Values)
            {
                image.Dispose();
            }

            if (this.ResourceData == null)
            {
                return;
            }

            foreach (var tabPage in tabControlResources.TabPages.Cast<TabPage>())
            {
                var imageManagement = tabPage.Controls.OfType<ctrlImageManagement>().SingleOrDefault();
                var documentManagement = tabPage.Controls.OfType<ctrlDocumentManagement>().SingleOrDefault();

                if (imageManagement != null)
                {
                    imageManagement.DisposeImage();
                }

                if (documentManagement != null)
                {
                    documentManagement.SaveEntries();
                    documentManagement.OnClose();
                }
            }

            this.ResourceData.Dispose();

            actionQueueService.Stop();
        }

        /// <summary>   Saves the data. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2021. </remarks>

        public void SaveData()
        {
            if (this.ResourceData != null && this.ResourceData.Dirty)
            {
                this.ResourceData.Save();
            }
        }

        private void cmdColorChooser_Click(object sender, EventArgs e)
        {
            var frmColorChooser = new frmColorChooser();
            var button = (Button)sender;
            ColorPicker colorPicker;
            string resourceName;
            string colorName;

            switch (button.Name)
            {
                case "cmdColorChooserPrimary":
                    colorPicker = colorPickerPrimary;
                    resourceName = "Primary Color";
                    break;
                case "cmdColorChooserSecondary":
                    colorPicker = colorPickerSecondary;
                    resourceName = "Secondary Color";
                    break;
                case "cmdColorChooserTertiary":
                    colorPicker = colorPickerTertiary;
                    resourceName = "Tertiary Color";
                    break;
                case "cmdColorChooserBackground":
                    colorPicker = colorPickerBackground;
                    resourceName = "Background Color";
                    break;
                default:
                    colorPicker = null;
                    resourceName = null;
                    DebugUtils.Break();
                    break;
            }

            frmColorChooser.InstanceRef = this.ParentForm;
            frmColorChooser.Color = colorPicker.SelectedColor;
            colorName = (string)colorPicker.SelectedItem;

            if (!colorName.IsNullOrEmpty())
            {
                frmColorChooser.Color = colorPicker.Colors[colorName];
            }

            if (frmColorChooser.ShowDialog() == DialogResult.OK)
            {
                colorPicker.SetColor(frmColorChooser.Color.Value, resourceName);

                switch (colorPicker.Name)
                {
                    case "colorPickerPrimary":
                        ResourceData.PrimaryColor = ColorTranslator.ToHtml(colorPickerPrimary.SelectedColor.Value);
                        break;
                    case "colorPickerSecondary":
                        ResourceData.SecondaryColor = ColorTranslator.ToHtml(colorPickerSecondary.SelectedColor.Value);
                        break;
                    case "colorPickerTertiary":
                        ResourceData.TertiaryColor = ColorTranslator.ToHtml(colorPickerTertiary.SelectedColor.Value);
                        break;
                    case "colorPickerBackground":
                        ResourceData.BackgroundColor = ColorTranslator.ToHtml(colorPickerBackground.SelectedColor.Value);
                        break;
                }

                actionQueueService.Run(() =>
                {
                    using (lockObject.Lock())
                    {
                        if (inRefresh)
                        {
                            return;
                        }

                        inRefresh = true;
                    }

                    this.Invoke(() =>
                    {
                        foreach (var screenPanel in appScreensPanelDevice.Controls.Cast<AppScreenPanel>())
                        {
                            screenPanel.Refresh();
                        }

                        foreach (var screenPanel in appScreenPanelBrowser.Controls.Cast<AppScreenPanel>())
                        {
                            screenPanel.Refresh();
                        }
                    });

                    using (lockObject.Lock())
                    {
                        inRefresh = false;
                    }

                }, colorPicker);
            }
        }

        private void ImageChanged(object sender, EventArgs e)
        {
            var imageManagement = (ctrlImageManagement)sender;
            var directory = new DirectoryInfo(this.ResourceData.RootPath);
            string filePath = null;
            string resourceName;
            string fileName;

            if (CompareExtensions.AllAreNull(imageManagement.ImageFileName, imageManagement.Image))
            {
                return;
            }

            switch (imageManagement.Name)
            {
                case "imageMgmtSplashScreen":
                    resourceName = "Splash Screen";
                    fileName = "SplashScreen.png";
                    break;
                case "imageMgmtAboutBanner":
                    resourceName = "About Banner";
                    fileName = "AboutBanner.png";
                    break;
                case "imageMgmtLogo":
                    resourceName = "Logo.png";
                    fileName = resourceName;
                    break;
                case "imageMgmtIcon":
                    resourceName = "Icon";
                    fileName = "Icon.png";
                    break;
                default:
                    fileName = null;
                    resourceName = null;
                    DebugUtils.Break();
                    break;
            }

            if (imageManagement.ImageFileName != null)
            {
                fileName = Path.GetFileName(imageManagement.ImageFileName);
                filePath = Path.Combine(directory.FullName, fileName);

                if (imageManagement.ImageFileName != filePath)
                {
                    File.Copy(imageManagement.ImageFileName, filePath, true);
                }
            }
            else if (imageManagement.Image != null)
            {
                var image = imageManagement.Image;

                filePath = Path.Combine(directory.FullName, fileName);

                image.Save(filePath);
            }

            switch (imageManagement.Name)
            {
                case "imageMgmtSplashScreen":
                    ResourceData.SplashScreen = filePath;
                    break;
                case "imageMgmtAboutBanner":
                    ResourceData.AboutBanner = filePath;
                    break;
                case "imageMgmtLogo":
                    ResourceData.Logo = filePath;
                    break;
                case "imageMgmtIcon":
                    ResourceData.Icon = filePath;
                    break;
                default:
                    DebugUtils.Break();
                    break;
            }

            actionQueueService.Run(() =>
            {
                using (lockObject.Lock())
                {
                    if (inRefresh)
                    {
                        return;
                    }

                    inRefresh = true;
                }

                this.Invoke(() =>
                {
                    foreach (var screenPanel in appScreensPanelDevice.Controls.Cast<AppScreenPanel>())
                    {
                        screenPanel.Refresh();
                    }

                    foreach (var screenPanel in appScreenPanelBrowser.Controls.Cast<AppScreenPanel>())
                    {
                        screenPanel.Refresh();
                    }
                });

                using (lockObject.Lock())
                {
                    inRefresh = false;
                }

            }, imageManagement);
        }

        private void tabControlImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowImageOrDocument(tabControlResources.SelectedIndex);
        }

        private void ShowImageOrDocument(int selectedIndex = 0)
        {
            var tabPage = tabControlResources.TabPages[selectedIndex];
            var imageManagement = tabPage.Controls.OfType<ctrlImageManagement>().SingleOrDefault();
            var documentManagement = tabPage.Controls.OfType<ctrlDocumentManagement>().SingleOrDefault();
            var marketing = tabPage.Controls.OfType<ctrlMarketing>().SingleOrDefault();

            if (documentManagement != null)
            {
                switch (documentManagement.Name)
                {
                    case "licenseManagement":

                        if (ResourceData.EndUserLicense.IsNullOrEmpty())
                        {
                            using (documentManagement.SetState(DocumentManagementState.InitializingFromState))
                            {
                                documentManagement.Reset();
                            }
                        }
                        else
                        {
                            using (documentManagement.SetState(DocumentManagementState.InitializingFromState))
                            {
                                documentManagement.ObjectProperties = this.ResourceData.ObjectProperties["LicenseManagementObjectProperties"];
                                documentManagement.DocumentFileName = ResourceData.EndUserLicense;
                                documentManagement.DocumentUrl = ResourceData.EndUserLicenseUrl;
                                documentManagement.DocumentEmailAddress = ResourceData.EndUserLicenseEmailAddress;
                                documentManagement.DocumentMailingAddress = ResourceData.EndUserLicenseMailingAddress;
                            }
                        }

                        break;

                    case "supportManagement":

                        if (ResourceData.SupportDetails.IsNullOrEmpty())
                        {
                            using (documentManagement.SetState(DocumentManagementState.InitializingFromState))
                            {
                                documentManagement.Reset();
                            }
                        }
                        else
                        {
                            using (documentManagement.SetState(DocumentManagementState.InitializingFromState))
                            {
                                documentManagement.ObjectProperties = this.ResourceData.ObjectProperties["SupportManagementObjectProperties"];
                                documentManagement.DocumentFileName = ResourceData.SupportDetails;
                                documentManagement.DocumentUrl = ResourceData.SupportUrl;
                                documentManagement.DocumentEmailAddress = ResourceData.SupportEmailAddress;
                            }
                        }

                        break;

                    default:
                        DebugUtils.Break();
                        break;
                }

            }
            else if (imageManagement != null)
            {
                switch (imageManagement.Name)
                {
                    case "imageMgmtSplashScreen":

                        if (ResourceData.SplashScreen.IsNullOrEmpty())
                        {
                            using (imageManagement.SetState(DocumentManagementState.InitializingFromState))
                            {
                                imageManagement.Reset();
                            }
                        }
                        else
                        {
                            using (imageManagement.SetState(DocumentManagementState.InitializingFromState))
                            {
                                imageManagement.ObjectProperties = this.ResourceData.ObjectProperties["SplashScreenObjectProperties"];
                                imageManagement.ImageFileName = this.ResourceData.SplashScreen;
                            }
                        }

                        imageManagement.RecommendedDimensions = resourceDefaults.Dimensions.SplashScreen;

                        break;

                    case "imageMgmtAboutBanner":

                        if (ResourceData.AboutBanner.IsNullOrEmpty())
                        {
                            using (imageManagement.SetState(DocumentManagementState.InitializingFromState))
                            {
                                imageManagement.Reset();
                            }
                        }
                        else
                        {
                            using (imageManagement.SetState(DocumentManagementState.InitializingFromState))
                            {
                                imageManagement.ObjectProperties = this.ResourceData.ObjectProperties["AboutBannerObjectProperties"];
                                imageManagement.ImageFileName = ResourceData.AboutBanner;
                            }
                        }

                        imageManagement.RecommendedDimensions = resourceDefaults.Dimensions.AboutBanner;

                        break;

                    case "imageMgmtLogo":

                        if (ResourceData.Logo.IsNullOrEmpty())
                        {
                            using (imageManagement.SetState(DocumentManagementState.InitializingFromState))
                            {
                                imageManagement.Reset();
                            }
                        }
                        else
                        {
                            using (imageManagement.SetState(DocumentManagementState.InitializingFromState))
                            {
                                imageManagement.ObjectProperties = this.ResourceData.ObjectProperties["LogoObjectProperties"];
                                imageManagement.ImageFileName = ResourceData.Logo;
                            }
                        }

                        imageManagement.RecommendedDimensions = resourceDefaults.Dimensions.Logo;

                        break;

                    case "imageMgmtIcon":

                        if (ResourceData.Icon.IsNullOrEmpty())
                        {
                            using (imageManagement.SetState(DocumentManagementState.InitializingFromState))
                            {
                                imageManagement.Reset();
                            }
                        }
                        else
                        {
                            using (imageManagement.SetState(DocumentManagementState.InitializingFromState))
                            {
                                imageManagement.ObjectProperties = this.ResourceData.ObjectProperties["IconObjectProperties"];
                                imageManagement.ImageFileName = ResourceData.Icon;
                            }
                        }

                        imageManagement.RecommendedDimensions = resourceDefaults.Dimensions.Icon;

                        break;

                    default:
                        DebugUtils.Break();
                        break;
                }
            }
            else if (marketing != null)
            {
                if (this.ResourceData.ObjectProperties.ContainsKey("MarketingObjectProperties"))
                {
                    var marketingObjectProperties = this.ResourceData.ObjectProperties["MarketingObjectProperties"];

                    using (marketing.SetState(DocumentManagementState.InitializingFromState))
                    {
                        marketing.ObjectProperties = marketingObjectProperties;

                        marketing.InitializeControl(this.WorkingDirectory);
                    }
                }
                else
                {
                    marketing.Reset();
                }
            }
            else
            {
                DebugUtils.Break();
            }
        }

        private void documentManagement_DocumentChanged(object sender, EventArgs e)
        {
            var documentManagement = (ctrlDocumentManagement)sender;
            var directory = new DirectoryInfo(this.ResourceData.RootPath);
            string filePath = null;
            string fileName;

            if (CompareExtensions.AllAreNull(documentManagement.DocumentFileName))
            {
                return;
            }

            switch (documentManagement.Name)
            {
                case "licenseManagement":
                    fileName = "EndUserLicense.rtf";
                    break;
                case "supportManagement":
                    fileName = "SupportStatement.rtf";
                    break;
                default:
                    fileName = null;
                    DebugUtils.Break();
                    break;
            }

            if (documentManagement.DocumentFileName != null)
            {
                fileName = Path.GetFileName(documentManagement.DocumentFileName);
                filePath = Path.Combine(directory.FullName, fileName);

                if (documentManagement.DocumentFileName != filePath)
                {
                    File.Copy(documentManagement.DocumentFileName, filePath, true);
                }
            }

            switch (documentManagement.Name)
            {
                case "licenseManagement":
                    ResourceData.EndUserLicense = filePath;
                    break;
                case "supportManagement":
                    ResourceData.SupportDetails = filePath;
                    break;
                default:
                    DebugUtils.Break();
                    break;
            }
        }

        private void documentManagement_DocumentUrlChanged(object sender, EventArgs e)
        {
            var documentManagement = (ctrlDocumentManagement)sender;
            var directory = new DirectoryInfo(this.ResourceData.RootPath);
            string resourceValue;

            if (CompareExtensions.AllAreNull(documentManagement.DocumentUrl))
            {
                return;
            }

            switch (documentManagement.Name)
            {
                case "licenseManagement":
                    resourceValue = documentManagement.DocumentUrl;
                    break;
                case "supportManagement":
                    resourceValue = documentManagement.DocumentUrl;
                    break;
                default:
                    resourceValue = null;
                    DebugUtils.Break();
                    break;
            }

            switch (documentManagement.Name)
            {
                case "licenseManagement":
                    ResourceData.EndUserLicenseUrl = resourceValue;
                    break;
                case "supportManagement":
                    ResourceData.SupportUrl = resourceValue;
                    break;
                default:
                    DebugUtils.Break();
                    break;
            }
        }

        private void documentManagement_DocumentEmailAddressChanged(object sender, EventArgs e)
        {
            var documentManagement = (ctrlDocumentManagement)sender;
            var directory = new DirectoryInfo(this.ResourceData.RootPath);
            string resourceValue;

            if (CompareExtensions.AllAreNull(documentManagement.DocumentUrl))
            {
                return;
            }

            switch (documentManagement.Name)
            {
                case "licenseManagement":
                    resourceValue = documentManagement.DocumentEmailAddress;
                    break;
                case "supportManagement":
                    resourceValue = documentManagement.DocumentEmailAddress;
                    break;
                default:
                    resourceValue = null;
                    DebugUtils.Break();
                    break;
            }

            switch (documentManagement.Name)
            {
                case "licenseManagement":
                    ResourceData.EndUserLicenseEmailAddress = resourceValue;
                    break;
                case "supportManagement":
                    ResourceData.SupportEmailAddress = resourceValue;
                    break;
                default:
                    DebugUtils.Break();
                    break;
            }
        }

        private void documentManagement_DocumentDetailsRtfChanged(object sender, EventArgs e)
        {
            var documentManagement = (ctrlDocumentManagement)sender;
            var directory = new DirectoryInfo(this.ResourceData.RootPath);
            string filePath = null;
            string fileName;

            switch (documentManagement.Name)
            {
                case "licenseManagement":
                    fileName = "EndUserLicense.rtf";
                    break;
                case "supportManagement":
                    fileName = "SupportStatement.rtf";
                    break;
                default:
                    fileName = null;
                    DebugUtils.Break();
                    break;
            }

            if (documentManagement.DocumentFileName == null)
            {
                filePath = Path.Combine(directory.FullName, fileName);
                documentManagement.DocumentFileName = filePath;
                string extension;

                extension = Path.GetExtension(fileName);

                using (var stream = File.OpenWrite(documentManagement.DocumentFileName))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        switch (extension)
                        {
                            case ".rtf":
                                writer.Write(documentManagement.DocumentDetailsRtf);
                                writer.Flush();

                                break;
                        }
                    }
                }
            }
            else
            {
                string extension;

                extension = Path.GetExtension(documentManagement.DocumentFileName);

                using (var stream = File.OpenWrite(documentManagement.DocumentFileName))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        switch (extension)
                        {
                            case ".rtf":
                                writer.Write(documentManagement.DocumentDetailsRtf);
                                writer.Flush();

                                break;
                        }
                    }
                }

                fileName = Path.GetFileName(documentManagement.DocumentFileName);
                filePath = Path.Combine(directory.FullName, fileName);

                if (documentManagement.DocumentFileName != filePath)
                {
                    File.Copy(documentManagement.DocumentFileName, filePath, true);
                }
            }

            switch (documentManagement.Name)
            {
                case "licenseManagement":
                    ResourceData.EndUserLicense = filePath;
                    break;
                case "supportManagement":
                    ResourceData.SupportDetails = filePath;
                    break;
                default:
                    DebugUtils.Break();
                    break;
            }
        }

        private void documentManagement_DocumentMailingAddressChanged(object sender, EventArgs e)
        {
            if (this.ResourceData != null)
            {
                var documentManagement = (ctrlDocumentManagement)sender;
                var directory = new DirectoryInfo(this.ResourceData.RootPath);
                string resourceName;

                if (CompareExtensions.AllAreNull(documentManagement.DocumentUrl))
                {
                    return;
                }

                switch (documentManagement.Name)
                {
                    case "licenseManagement":
                        resourceName = "EndUserLicenseMailingAddress";
                        break;
                    case "supportManagement":
                        resourceName = "SupportMailingAddress";
                        break;
                    default:
                        resourceName = null;
                        DebugUtils.Break();
                        break;
                }

                this.ResourceData[resourceName] = documentManagement.DocumentMailingAddress;
            }
        }

        private void licenseManagement_ObjectPropertiesChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateObjectPropertiesResourceData();
        }

        private void supportManagement_ObjectPropertiesChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateObjectPropertiesResourceData();
        }

        private void imageMgmtSplashScreen_ObjectPropertiesChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateObjectPropertiesResourceData();
        }

        private void imageMgmtAboutBanner_ObjectPropertiesChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateObjectPropertiesResourceData();
        }

        private void imageMgmtLogo_ObjectPropertiesChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateObjectPropertiesResourceData();
        }

        private void imageMgmtIcon_ObjectPropertiesChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateObjectPropertiesResourceData();
        }

        private void ctrlMarketing_ObjectPropertiesChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateObjectPropertiesResourceData();
        }

        private void UpdateObjectPropertiesResourceData()
        {
            var licenseManagementObjectProperties = licenseManagement.ObjectProperties;
            var supportManagementObjectProperties = supportManagement.ObjectProperties;
            var imageSplashScreenObjectProperties = imageMgmtSplashScreen.ObjectProperties;
            var imageMgmtAboutBannerObjectProperties = imageMgmtAboutBanner.ObjectProperties;
            var imageMgmtLogoObjectProperties = imageMgmtLogo.ObjectProperties;
            var imageMgmtIconObjectProperties = imageMgmtIcon.ObjectProperties;
            var marketingObjectProperties = ctrlMarketing.ObjectProperties;

            this.ResourceData.SuppressObjectPropertiesSave = true;

            this.ResourceData.ObjectProperties.Remove("LicenseManagementObjectProperties");
            this.ResourceData.ObjectProperties.Remove("SupportManagementObjectProperties");
            this.ResourceData.ObjectProperties.Remove("SplashScreeenObjectProperties");
            this.ResourceData.ObjectProperties.Remove("AboutBannerObjectProperties");
            this.ResourceData.ObjectProperties.Remove("LogoObjectProperties");
            this.ResourceData.ObjectProperties.Remove("IconObjectProperties");
            this.ResourceData.ObjectProperties.Remove("MarketingObjectProperties");

            this.ResourceData.ObjectProperties.Add("LicenseManagementObjectProperties", licenseManagementObjectProperties);
            this.ResourceData.ObjectProperties.Add("SplashScreenObjectProperties", imageSplashScreenObjectProperties);
            this.ResourceData.ObjectProperties.Add("AboutBannerObjectProperties", imageMgmtAboutBannerObjectProperties);
            this.ResourceData.ObjectProperties.Add("LogoObjectProperties", imageMgmtLogoObjectProperties);
            this.ResourceData.ObjectProperties.Add("IconObjectProperties", imageMgmtIconObjectProperties);
            this.ResourceData.ObjectProperties.Add("MarketingObjectProperties", marketingObjectProperties);

            this.ResourceData.SuppressObjectPropertiesSave = false;

            this.ResourceData.ObjectProperties.Add("SupportManagementObjectProperties", supportManagementObjectProperties);
        }

        private void linkLabelVariations_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var frmVariations = new frmVariations(txtAppDescription.Text);

            frmVariations.ShowDialog();
        }

        /// <summary>   Initializes this. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        ///
        /// <param name="wizardSettingsBase">   Options for controlling the operation. </param>

        public void Initialize(WizardSettingsBase wizardSettingsBase)
        {
            var settings = (WizardSettings)wizardSettingsBase;
        }

        /// <summary>   Saves. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        ///
        /// <param name="wizardSettingsBase">   Options for controlling the operation. </param>
        /// <param name="isNext">               True if is next, false if not. </param>

        public void Save(WizardSettingsBase wizardSettingsBase, bool isNext)
        {
            var settings = (WizardSettings)wizardSettingsBase;

            settings.AppName = this.ResourceData.AppName;
            settings.AppDescription = this.ResourceData.AppDescription;
            settings.OrganizationName = this.ResourceData.OrganizationName;
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            this.ResourceData.InitiateSaveBackup(null);
        }

        private void BeforeExecute(string action, IResourceData resourceData)
        {
            foreach (var image in this.imageCollection.Values)
            {
                image.Dispose();
            }

            foreach (var tabPage in tabControlResources.TabPages.Cast<TabPage>())
            {
                var imageManagement = tabPage.Controls.OfType<ctrlImageManagement>().SingleOrDefault();
                var documentManagement = tabPage.Controls.OfType<ctrlDocumentManagement>().SingleOrDefault();

                if (imageManagement != null)
                {
                    imageManagement.DisposeImage();
                }

                if (documentManagement != null)
                {
                    documentManagement.SaveEntries();
                    documentManagement.OnClose();
                }
            }

            this.ResourceData.Dispose();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            this.ResourceData.InitiateRestoreBackup(this.BeforeExecute);
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            this.ResourceData.InitiateNew(this.BeforeExecute);
        }

        /// <summary>   Validates this. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>

        public void ValidatePage()
        {
            if (this.UserValidated && CompareExtensions.AllAreFalse(txtAppName.Text.IsNullOrEmpty(), txtAppDescription.Text.IsNullOrEmpty(), txtOrganizationName.Text.IsNullOrEmpty()))
            {
                this.errorProvider.Clear();

                this.OnPageValid(this, EventArgs.Empty);
            }
            else
            {
                var exception = new FormatException("The following fields are required: 'App Name', 'App Description', 'Organization Name'");

                if (txtAppName.Text.IsNullOrEmpty())
                {
                    this.errorProvider.SetError(txtAppName, "Required");
                }
                else
                {
                    this.errorProvider.SetError(txtAppName, string.Empty);
                }

                if (txtAppDescription.Text.IsNullOrEmpty())
                {
                    this.errorProvider.SetError(txtAppDescription, "Required");
                }
                else
                {
                    this.errorProvider.SetError(txtAppDescription, string.Empty);
                }

                if (txtOrganizationName.Text.IsNullOrEmpty())
                {
                    this.errorProvider.SetError(txtOrganizationName, "Required");
                }
                else
                {
                    this.errorProvider.SetError(txtOrganizationName, string.Empty);
                }

                if (this.OnPageInvalid != null)
                {
                    this.OnPageInvalid(this, new EventArgs<Exception>(exception));
                }
            }
        }
    }
}
