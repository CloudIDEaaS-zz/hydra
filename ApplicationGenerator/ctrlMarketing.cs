// file:	ctrlMarketing.cs
//
// summary:	Implements the control marketing class

using AbstraX.MarketingControls;
using AbstraX.MarketingControls.SocialMedia;
using AbstraX.ObjectProperties;
using AbstraX.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;


// namespace: AbstraX
//
// summary:	.

namespace AbstraX
{
    /// <summary>   A control marketing. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/7/2021. </remarks>

    public partial class ctrlMarketing : ctrlResourceManagementBase, IPropertyOwner
    {
        /// <summary>   The form URL edit popup. </summary>
        private frmUrlEditPopup frmUrlEditPopup;
        /// <summary>   The object property grid leave. </summary>
        private EventHandler objectPropertyGridLeave;
        private List<LinkData> linkDataList;

        /// <summary>   Gets or sets the pathname of the working directory. </summary>
        ///
        /// <value> The pathname of the working directory. </value>

        public string WorkingDirectory { get; set; }

        /// <summary>   Gets or sets a list of social medias. </summary>
        ///
        /// <value> A list of social medias. </value>

        public SocialMediaList SocialMediaList { get; set; }

        /// <summary>   Gets or sets the social media list original. </summary>
        ///
        /// <value> The social media list original. </value>

        public SocialMediaList SocialMediaListOriginal { get; set; }

        /// <summary>   Gets or sets a list of application stores. </summary>
        ///
        /// <value> A list of application stores. </value>

        public AppStoreList AppStoreListOriginal { get; set; }

        /// <summary>   Gets or sets the application store list processed. </summary>
        ///
        /// <value> The application store list processed. </value>

        public AppStoreList AppStoreList { get; set; }

        /// <summary>   Gets or sets the tell others processed. </summary>
        ///
        /// <value> The tell others processed. </value>

        public TellOthers TellOthers { get; set; }

        /// <summary>   Gets or sets the advertising link. </summary>
        ///
        /// <value> The advertising link. </value>

        public string AdvertisingLink { get; set; }

        /// <summary>   Gets or sets the connect with us link. </summary>
        ///
        /// <value> The connect with us link. </value>

        public string ConnectWithUsLink { get; set; }

        /// <summary>   Gets or sets the email us link. </summary>
        ///
        /// <value> The email us link. </value>

        public string EmailUsLink { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/7/2021. </remarks>
        ///
        /// <value> The object properties. </value>

        /// <summary>   Gets or sets the link object properties. </summary>
        ///
        /// <value> The link object properties. </value>

        [Browsable(false)]
        public ObjectPropertiesDictionary LinkObjectProperties { get; set; }

        /// <summary>   Gets or sets the other links. </summary>
        ///
        /// <value> The other links. </value>

        public List<string> OtherLinks { get; set; }

        /// <summary>   Gets or sets the other properties. </summary>
        ///
        /// <value> The other properties. </value>

        public ObjectPropertiesDictionary OtherProperties { get; set; }

        /// <summary>
        /// Event queue for all listeners interested in ObjectPropertiesChanged events.
        /// </summary>

        private Queue<Control> validationQueue;
        /// <summary>   Event queue for all listeners interested in TextPropertiesChanged events. </summary>

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/9/2021. </remarks>

        public ctrlMarketing()
        {
            linkDataList = new List<LinkData>();

            this.ObjectProperties = new ObjectPropertiesDictionary();
            this.OtherLinks = new List<string>();

            validationQueue = new Queue<Control>();

            InitializeComponent();
        }

        /// <summary>   Initializes the control. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/14/2021. </remarks>
        ///
        /// <param name="workingDirectory"> The pathname of the working directory. </param>

        public void InitializeControl(string workingDirectory)
        {
            this.DelayInvoke(100, () =>
            {
                using (this.SetState(DocumentManagementState.InitializingFromState))
                {
                    try
                    {
                        var assemblyLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                        var localMarketingFolder = System.IO.Path.Combine(assemblyLocation, @"Marketing");
                        var localSocialMediaPath = System.IO.Path.Combine(localMarketingFolder, @"SocialMedia.json");
                        var localAppStoresPath = System.IO.Path.Combine(localMarketingFolder, @"AppStores.json");
                        string jsonText;

                        this.WorkingDirectory = workingDirectory;

                        ReadProperties();

                        jsonText = System.IO.File.ReadAllText(localSocialMediaPath);
                        this.SocialMediaListOriginal = JsonExtensions.ReadJson<SocialMediaList>(jsonText);

                        if (this.SocialMediaList.SocialMedia.Length == 0)
                        {
                            this.SocialMediaList.SocialMedia = this.SocialMediaListOriginal.SocialMedia.ToArray();
                        }

                        this.SocialMediaList.WorkingDirectory = this.WorkingDirectory;

                        jsonText = System.IO.File.ReadAllText(localAppStoresPath);
                        this.AppStoreListOriginal = JsonExtensions.ReadJson<AppStoreList>(jsonText);

                        ctrlSocialMediaList.SocialMediaList = this.SocialMediaList;
                        ctrlSocialMediaList.SocialMediaListOriginal = this.SocialMediaListOriginal;

                        checkedListBoxRateUs.DisplayMember = "Name";
                        checkedListBoxRateUs.ValueMember = "AllowRating";

                        if (this.AdvertisingLink != null)
                        {
                            txtAdvertisingLink.Text = this.AdvertisingLink;
                        }

                        if (this.ConnectWithUsLink != null)
                        {
                            txtConnectWithUsLink.Text = this.ConnectWithUsLink;
                        }

                        if (this.EmailUsLink != null)
                        {
                            txtEmailUsLink.Text = this.EmailUsLink.RemoveStartIfMatches("mailto:");
                        }

                        if (this.AppStoreList.AppStores.Length == 0)
                        {
                            this.AppStoreList.AppStores = this.AppStoreListOriginal.AppStores.ToArray();
                        }

                        this.AppStoreList.WorkingDirectory = this.WorkingDirectory;

                        checkedListBoxRateUs.DataSource = new ItemTrackingBindingList<AppStoreEntry>(this.AppStoreList.AppStores);
                        checkedListBoxRateUs.SelectedIndex = -1;

                        if (this.OtherLinks.Count > 0)
                        {
                            lstOtherLinks.DataSource = new ItemTrackingBindingList<string>(this.OtherLinks);
                        }

                        lstOtherLinks.SelectedIndex = -1;
                        txtTellOthersLink.Text = this.TellOthers.Url;

                        ObjectExtensions.MultiAct<Control>(c => c.GetMessages((m) => true, (m) => GetPostPaintMessage(c, m)), txtTellOthersLink, txtAdvertisingLink, txtConnectWithUsLink, txtEmailUsLink, txtOtherLinks);
                    }
                    catch (Exception ex)
                    {
                        DebugUtils.Break();
                    }
                }
            });
        }

        /// <summary>   Reads the properties. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>

        private void ReadProperties()
        {
            if (this.ObjectProperties.ContainsKey("Links"))
            {
                this.LinkObjectProperties = this.ObjectProperties["Links"];
            }
            else
            {
                this.LinkObjectProperties = new ObjectPropertiesDictionary();
                this.ObjectProperties["Links"] = this.LinkObjectProperties;
            }

            if (this.ObjectProperties.ContainsKey("SocialMedia"))
            {
                if (this.ObjectProperties["SocialMedia"] is ObjectPropertiesDictionary socialMediaList)
                {
                    this.SocialMediaList = new SocialMediaList();

                    socialMediaList.CopyTo(this.SocialMediaList);

                    this.ObjectProperties["SocialMedia"] = this.SocialMediaList;
                }
            }
            else
            {
                this.SocialMediaList = new SocialMediaList();
                this.ObjectProperties["SocialMedia"] = this.SocialMediaList;
            }

            if (this.ObjectProperties.ContainsKey("RateUs"))
            {
                if (this.ObjectProperties["RateUs"] is ObjectPropertiesDictionary appStoreList)
                {
                    this.AppStoreList = new AppStoreList();

                    appStoreList.CopyTo(this.AppStoreList);

                    this.ObjectProperties["RateUs"] = this.AppStoreList;
                }
            }
            else
            {
                this.AppStoreList = new AppStoreList();
                this.ObjectProperties["RateUs"] = this.AppStoreList;
            }

            if (this.ObjectProperties.ContainsKey("OtherLinks"))
            {
                if (this.ObjectProperties["OtherLinks"] is JArray otherLinksJArray)
                {
                    var otherLinks = otherLinksJArray.ToObject<string[]>();

                    this.OtherLinks = new List<string>();
                    this.OtherLinks.AddRange(otherLinks);

                    this.ObjectProperties["OtherLinks"] = this.OtherLinks;
                }
            }
            else
            {
                this.OtherLinks = new List<string>();
                this.ObjectProperties["OtherLinks"] = this.OtherLinks;
            }

            if (this.ObjectProperties.ContainsKey("Other"))
            {
                if (this.ObjectProperties["Other"] is ObjectPropertiesDictionary otherProperties)
                {
                    if (otherProperties.ContainsKey("TellOthers") && otherProperties["TellOthers"] is ObjectPropertiesDictionary tellOthers)
                    {
                        this.TellOthers = new TellOthers();

                        tellOthers.CopyTo(this.TellOthers);

                        this.TellOthers.WorkingDirectory = this.WorkingDirectory;

                        otherProperties["TellOthers"] = this.TellOthers;
                    }

                    if (otherProperties.ContainsKey("AdvertisingLink"))
                    {
                        this.AdvertisingLink = otherProperties["AdvertisingLink"];
                    }

                    if (otherProperties.ContainsKey("ConnectWithUsLink"))
                    {
                        this.ConnectWithUsLink = otherProperties["ConnectWithUsLink"];
                    }

                    if (otherProperties.ContainsKey("EmailUsLink"))
                    {
                        this.EmailUsLink = otherProperties["EmailUsLink"];
                    }

                    this.OtherProperties = otherProperties;
                }
            }
            else
            {
                this.OtherProperties = new ObjectPropertiesDictionary();

                this.ObjectProperties["Other"] = this.OtherProperties;
                this.TellOthers = new TellOthers();

                this.TellOthers.WorkingDirectory = this.WorkingDirectory;

                this.OtherProperties.Add("TellOthers", this.TellOthers);
            }
        }

        /// <summary>   Gets post paint message. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="ctrl"> The control. </param>
        /// <param name="m">    A Message to process. </param>

        private void GetPostPaintMessage(Control ctrl, Message m)
        {
            if (m.Msg == (int) ControlExtensions.WindowsMessage.PAINT)
            {
                if (ctrl.Tag is string && ((string) ctrl.Tag) == "Invalid")
                {
                    if (ctrl is TextBoxBase textBoxBase)
                    {
                        var rect = textBoxBase.ClientRectangle;
                        var url = textBoxBase.Text;
                        var size = TextRenderer.MeasureText(url, textBoxBase.Font);

                        size.Height -= 2;

                        rect.Size = size;

                        using (var graphics = textBoxBase.CreateGraphics())
                        {
                            graphics.DrawErrorSquiggly(url, textBoxBase.Font, rect);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event handler. Called by ctrlSplitCheckboxTellOthersUrl for checked changed events.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/9/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void ctrlSplitCheckboxTellOthersUrl_CheckedChanged(object sender, EventArgs e)
        {
            if (ctrlSplitCheckboxTellOthersUrl.Checked)
            {
                var entryOriginal = new TellOthers();
                var entry = this.TellOthers;
                var propertyInfo = entry.GetProperty("Url");
                var textBoxRect = txtTellOthersLink.Bounds;

                if (frmUrlEditPopup != null)
                {
                    frmUrlEditPopup.Dispose();
                }

                frmUrlEditPopup = new frmUrlEditPopup(this, entry, propertyInfo, entryOriginal.Url, entry.Url);

                frmUrlEditPopup.Deactivate += (sender2, e2) =>
                {
                    frmUrlEditPopup.Dispose();
                };

                frmUrlEditPopup.Disposed += (sender2, e2) =>
                {
                    if (ctrlSplitCheckboxTellOthersUrl.Checked)
                    {
                        var focusControl = this.GetFocus();
                        var clickControl = this.GetControlAtCursor();

                        if (focusControl != ctrlSplitCheckboxTellOthersUrl || clickControl != ctrlSplitCheckboxTellOthersUrl)
                        {
                            Debug.WriteLine("Unchecking in code");

                            ctrlSplitCheckboxTellOthersUrl.Checked = false;
                        }
                    }
                };

                textBoxRect = this.RectangleToScreen(textBoxRect);
                textBoxRect = this.Parent.RectangleToClient(textBoxRect);

                frmUrlEditPopup.Location = new Point(textBoxRect.Left, textBoxRect.Bottom);
                frmUrlEditPopup.Width = textBoxRect.Width + 50;

                frmUrlEditPopup.BringToFront();

                frmUrlEditPopup.Show();
            }
            else
            {
                frmUrlEditPopup.Dispose();
            }
        }

        /// <summary>
        /// Event handler. Called by checkedListBoxRateUs for selected index changed events.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/9/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void checkedListBoxRateUs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var entry = (AppStoreEntry) checkedListBoxRateUs.SelectedItem;
            Guid itemGuid;

            if (entry == null)
            {
                objectPropertyGrid.SelectedObject = null;
                return;
            }

            itemGuid = checkedListBoxRateUs.GetItemGuid(entry);

            if (entry.AllowRating)
            {
                if (!ShowLinkProperties(checkedListBoxRateUs, itemGuid, entry.SiteUrl))
                {
                    objectPropertyGrid.SelectedObject = null;
                }
            }
            else
            {
                objectPropertyGrid.SelectedObject = null;
            }
        }

        /// <summary>   Event handler. Called by checkedListBoxRateUs for item check events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Item check event information. </param>

        private void checkedListBoxRateUs_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var entry = (AppStoreEntry)checkedListBoxRateUs.SelectedItem;
            var itemGuid = checkedListBoxRateUs.GetItemGuid(entry);
            var currentValue = e.CurrentValue;

            if (entry.AllowRating)
            {
                if (!ShowLinkProperties(checkedListBoxRateUs, itemGuid, entry.SiteUrl))
                {
                    objectPropertyGrid.SelectedObject = null;
                }
            }
            else
            {
                objectPropertyGrid.SelectedObject = null;
            }

            if (this.DocumentManagementState != DocumentManagementState.InitializingFromState)
            {
                this.RaiseObjectPropertiesChanged(sender, entry, currentValue);
            }
        }

        /// <summary>   Event handler. Called by checkedListBoxRateUs for leave events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void checkedListBoxRateUs_Leave(object sender, EventArgs e)
        {
            CheckObjectPropertiesFocus();
        }

        /// <summary>   Event handler. Called by txtTellOthersLink for enter events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/9/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void txtTellOthersLink_Enter(object sender, EventArgs e)
        {
            if (!ShowLinkProperties(txtTellOthersLink, txtTellOthersLink.Text))
            {
                objectPropertyGrid.Title = "Invalid Link";
                objectPropertyGrid.SelectedObject = null;
            }
        }

        /// <summary>   Event handler. Called by txtOtherLinks for enter events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/9/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void txtOtherLinks_Enter(object sender, EventArgs e)
        {
            if (!ShowLinkProperties(txtOtherLinks, txtOtherLinks.Text))
            {
                objectPropertyGrid.Title = "Invalid Link";
                objectPropertyGrid.SelectedObject = null;
            }
        }

        /// <summary>   Event handler. Called by lstOtherLinks for selected index changed events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/9/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void lstOtherLinks_SelectedIndexChanged(object sender, EventArgs e)
        {
            var url = (string)lstOtherLinks.SelectedItem;
            Guid itemGuid;

            if (url != null && url.Length > 0)
            {
                itemGuid = lstOtherLinks.GetItemGuid(url);

                if (!ShowLinkProperties(lstOtherLinks, itemGuid, url))
                {
                    objectPropertyGrid.SelectedObject = null;
                }

                linkLabelRemoveSelected.Enabled = true;
            }
            else
            {
                objectPropertyGrid.SelectedObject = null;
            }
        }

        /// <summary>   Event handler. Called by txtTellOthersLink for leave events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/9/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void txtTellOthersLink_Leave(object sender, EventArgs e)
        {
            CheckObjectPropertiesFocus();
        }

        /// <summary>   Check object properties focus. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>

        private void CheckObjectPropertiesFocus()
        {
            var hwndFocus = ControlExtensions.GetFocus();

            if (objectPropertyGrid.Handle == hwndFocus)
            {
                return;
            }
            else
            {
                var childWindows = ControlExtensions.GetChildWindows(objectPropertyGrid.Handle);

                if (!childWindows.Any(h => h == hwndFocus))
                {
                    objectPropertyGrid.SelectedObject = null;
                }

                objectPropertyGridLeave = new EventHandler((sender3, e3) =>
                {
                    objectPropertyGrid.LostFocus -= objectPropertyGridLeave;
                    objectPropertyGrid.SelectedObject = null;
                });

                objectPropertyGrid.Leave += objectPropertyGridLeave;
            }
        }

        private bool ShowLinkProperties(Control control, string linkText)
        {
            return ShowLinkProperties(control, Guid.Empty, linkText);
        }

        /// <summary>   Shows the link properties. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/9/2021. </remarks>
        ///
        /// <param name="control">  The control. </param>
        /// <param name="itemGuid"> Unique identifier for the item. </param>
        /// <param name="linkText"> The link text. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        private bool ShowLinkProperties(Control control, Guid itemGuid, string linkText)
        {
            LinkProperties properties;
            LinkData newLinkData;

            if (!linkText.IsValidEmailAddress() && !linkText.IsValidUrl())
            {
                return false;
            }

            newLinkData = new LinkData(linkText, control, itemGuid);

            if (!this.LinkObjectProperties.ContainsKey(linkText))
            {
                properties = new LinkProperties(linkText);

                this.LinkObjectProperties.Add(linkText, properties);
            }
            else if (LinkObjectProperties[linkText] is LinkProperties)
            {
                properties = this.LinkObjectProperties[linkText];
            }
            else
            {
                var objectProperties = (ObjectPropertiesDictionary)this.LinkObjectProperties[linkText];

                properties = new LinkProperties(linkText);

                foreach (var pair in objectProperties)
                {
                    var key = pair.Key;
                    var value = (object)pair.Value;
                    var property = properties.GetProperty(key);

                    if (value != null)
                    {
                        if (property.PropertyType.IsArray && value is JArray jArray)
                        {
                            var array = jArray.ToObject(property.PropertyType);

                            properties.SetPropertyValue(key, array);
                        }
                        else
                        {
                            properties.SetPropertyValue(key, Convert.ChangeType(value, property.PropertyType));
                        }
                    }
                }

                this.LinkObjectProperties[linkText] = properties;
            }

            if (this.linkDataList.Any(d => d.Word != newLinkData.Word && d.Intersects(newLinkData)))
            {
                var linkDataIntersections = this.linkDataList.Where(d => d.Word != newLinkData.Word && d.Intersects(newLinkData)).ToList();

                foreach (var linkData in linkDataIntersections)
                {
                    var newLinkWord = newLinkData.Word;
                    var oldLinkWord = linkData.Word;
                    var deleteOld = false;

                    if (this.LinkObjectProperties.ContainsKey(oldLinkWord))
                    {
                        this.LinkObjectProperties.Remove(oldLinkWord);
                        deleteOld = true;
                    }

                    if (deleteOld)
                    {
                        this.linkDataList.Remove(linkData);
                    }
                }
            }

            if (!linkDataList.Any(d => d.Word == newLinkData.Word))
            {
                linkDataList.Add(newLinkData);
            }

            if (properties.Name == null)
            {
                objectPropertyGrid.Title = linkText;
            }
            else
            {
                objectPropertyGrid.Title = properties.Name;
            }

            objectPropertyGrid.SelectedObject = properties;

            return true;
        }

        /// <summary>   Event handler. Called by txtAdvertisingLink for enter events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void txtAdvertisingLink_Enter(object sender, EventArgs e)
        {
            if (!ShowLinkProperties(txtAdvertisingLink, txtAdvertisingLink.Text))
            {
                objectPropertyGrid.Title = "Invalid Link";
                objectPropertyGrid.SelectedObject = null;
            }
        }

        /// <summary>   Event handler. Called by txtAdvertisingLink for leave events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void txtAdvertisingLink_Leave(object sender, EventArgs e)
        {
            CheckObjectPropertiesFocus();
        }

        /// <summary>   Event handler. Called by txtConnectWithUsLink for enter events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void txtConnectWithUsLink_Enter(object sender, EventArgs e)
        {
            if (!ShowLinkProperties(txtConnectWithUsLink, txtConnectWithUsLink.Text))
            {
                objectPropertyGrid.Title = "Invalid Link";
                objectPropertyGrid.SelectedObject = null;
            }
        }

        /// <summary>   Event handler. Called by txtConnectWithUsLink for leave events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void txtConnectWithUsLink_Leave(object sender, EventArgs e)
        {
            CheckObjectPropertiesFocus();
        }

        /// <summary>   Resets this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>

        public void Reset()
        {
            linkDataList = new List<LinkData>();

            this.ObjectProperties = new ObjectPropertiesDictionary();
            this.OtherLinks = new List<string>();

            validationQueue = new Queue<Control>();

            using (this.SetState(DocumentManagementState.InitializingFromState))
            {
                try
                {
                    var assemblyLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    var localMarketingFolder = System.IO.Path.Combine(assemblyLocation, @"Marketing");
                    var localSocialMediaPath = System.IO.Path.Combine(localMarketingFolder, @"SocialMedia.json");
                    var localAppStoresPath = System.IO.Path.Combine(localMarketingFolder, @"AppStores.json");
                    string jsonText;

                    ReadProperties();

                    jsonText = System.IO.File.ReadAllText(localSocialMediaPath);
                    this.SocialMediaListOriginal = JsonExtensions.ReadJson<SocialMediaList>(jsonText);

                    if (this.SocialMediaList.SocialMedia.Length == 0)
                    {
                        this.SocialMediaList.SocialMedia = this.SocialMediaListOriginal.SocialMedia.ToArray();
                    }

                    this.SocialMediaList.WorkingDirectory = this.WorkingDirectory;

                    jsonText = System.IO.File.ReadAllText(localAppStoresPath);
                    this.AppStoreListOriginal = JsonExtensions.ReadJson<AppStoreList>(jsonText);

                    ctrlSocialMediaList.SocialMediaList = this.SocialMediaList;
                    ctrlSocialMediaList.SocialMediaListOriginal = this.SocialMediaListOriginal;

                    checkedListBoxRateUs.DisplayMember = "Name";
                    checkedListBoxRateUs.ValueMember = "AllowRating";

                    if (this.AdvertisingLink != null)
                    {
                        txtAdvertisingLink.Text = this.AdvertisingLink;
                    }

                    if (this.ConnectWithUsLink != null)
                    {
                        txtConnectWithUsLink.Text = this.ConnectWithUsLink;
                    }

                    if (this.EmailUsLink != null)
                    {
                        txtEmailUsLink.Text = this.EmailUsLink.RemoveStartIfMatches("mailto:");
                    }

                    if (this.AppStoreList.AppStores.Length == 0)
                    {
                        this.AppStoreList.AppStores = this.AppStoreListOriginal.AppStores.ToArray();
                    }

                    this.AppStoreList.WorkingDirectory = this.WorkingDirectory;

                    checkedListBoxRateUs.DataSource = new ItemTrackingBindingList<AppStoreEntry>(this.AppStoreList.AppStores);
                    checkedListBoxRateUs.SelectedIndex = -1;

                    if (this.OtherLinks.Count > 0)
                    {
                        lstOtherLinks.DataSource = new ItemTrackingBindingList<string>(this.OtherLinks);
                    }

                    lstOtherLinks.SelectedIndex = -1;
                    txtTellOthersLink.Text = this.TellOthers.Url;

                    objectPropertyGrid.SelectedObject = null;
                }
                catch (Exception ex)
                {
                    DebugUtils.Break();
                }
            }
        }

        /// <summary>   Event handler. Called by txtOtherLinks for leave events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void txtOtherLinks_Leave(object sender, EventArgs e)
        {
            CheckObjectPropertiesFocus();
        }

        /// <summary>   Event handler. Called by lstOtherLinks for leave events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void lstOtherLinks_Leave(object sender, EventArgs e)
        {
            CheckObjectPropertiesFocus();
        }

        /// <summary>   Event handler. Called by linkLabelAdd for link clicked events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Link label link clicked event information. </param>

        private void linkLabelAdd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AddToOtherLinks(sender);
        }

        /// <summary>   Adds to the other links. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>

        private void AddToOtherLinks(object sender)
        {
            var url = txtOtherLinks.Text;

            if (!this.OtherLinks.Contains(txtOtherLinks.Text))
            {
                var dataSource = (BindingList<string>)lstOtherLinks.DataSource;

                this.OtherLinks.Add(url);

                if (dataSource == null)
                {
                    lstOtherLinks.DataSource = new ItemTrackingBindingList<string>(this.OtherLinks);
                }
                else
                {
                    dataSource.ResetBindings();
                    lstOtherLinks.Update();
                }

                txtOtherLinks.Clear();

                this.RaiseObjectPropertiesChanged(sender, url, null);
            }
        }

        /// <summary>
        /// Event handler. Called by linkLabelRemoveSelected for link clicked events.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Link label link clicked event information. </param>

        private void linkLabelRemoveSelected_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RemoveFromOtherLinks(sender);
        }

        /// <summary>   Removes from other links described by sender. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>

        private void RemoveFromOtherLinks(object sender)
        {
            var dataSource = (BindingList<string>)lstOtherLinks.DataSource;
            var removedItems = new List<string>();

            foreach (var item in lstOtherLinks.SelectedItems.Cast<string>().ToList())
            {
                var index = lstOtherLinks.SelectedItems.IndexOf(item);

                removedItems.Add(item);
                dataSource.RemoveAt(index);
            }

            if (dataSource.Count == 0)
            {
                linkLabelRemoveSelected.Enabled = false;
            }

            this.RaiseObjectPropertiesChanged(sender, null, removedItems);
        }

        /// <summary>   Event handler. Called by lstOtherLinks for key down events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Key event information. </param>

        private void lstOtherLinks_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveFromOtherLinks(sender);
            }
        }

        /// <summary>   Event handler. Called by timerUrlValidation for tick events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void timerUrlValidation_Tick(object sender, EventArgs e)
        {
            while (validationQueue.Count > 0)
            {
                var ctrl = validationQueue.Dequeue();

                if (ctrl is TextBoxBase textBoxBase)
                {
                    var url = textBoxBase.Text;

                    if (textBoxBase == txtEmailUsLink)
                    {
                        url = "mailto:" + url;
                    }

                    if (url.IsValidUrl())
                    {
                        textBoxBase.Tag = "Valid";

                        ShowLinkProperties(ctrl, url);
                    }
                    else
                    {
                        textBoxBase.Tag = "Invalid";
                    }

                    textBoxBase.Invalidate();
                    textBoxBase.Refresh();
                }
            }

            timerUrlValidation.Stop();
        }

        /// <summary>   Event handler. Called by txtOtherLinks for key down events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Key event information. </param>

        private void txtOtherLinks_KeyDown(object sender, KeyEventArgs e)
        {
            var rect = txtOtherLinks.ClientRectangle;
            var url = txtOtherLinks.Text;

            if (e.KeyCode == Keys.Enter)
            {
                AddToOtherLinks(sender);
                return;
            }

            if (txtOtherLinks.Tag is string && ((string)txtOtherLinks.Tag) == "Invalid")
            {
                this.DelayInvoke(1, () =>
                {
                    rect.Y = rect.Height - 5;
                    rect.Height = 5;

                    txtOtherLinks.Invalidate(rect);
                    txtOtherLinks.Update();
                });
            }

            timerUrlValidation.Start();
            validationQueue.Enqueue(txtOtherLinks);

            if (url.IsValidUrl())
            {
                if (!this.OtherLinks.Contains(url))
                {
                    linkLabelAdd.Enabled = true;
                }
            }
            else
            {
                linkLabelAdd.Enabled = false;
            }
        }

        /// <summary>
        /// Event handler. Called by objectPropertyGrid for property value changed events.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="s">    An object to process. </param>
        /// <param name="e">    Property value changed event information. </param>

        private void objectPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (this.DocumentManagementState != DocumentManagementState.InitializingFromState)
            {
                this.RaiseObjectPropertiesChanged(s, e);
            }
        }

        /// <summary>   Event handler. Called by ctrlSocialMediaList for update entry events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        An EventArgs&lt;SocialMediaEntry&gt; to process. </param>

        private void ctrlSocialMediaList_UpdateEntry(object sender, EventArgs<SocialMediaEntry> e)
        {
            var socialMediaEntryViewable = e.Value;

            if (this.SocialMediaList.SocialMedia.Any(m => m.Name == socialMediaEntryViewable.Name))
            {
                var entry = this.SocialMediaList.SocialMedia.Single(m => m.Name == socialMediaEntryViewable.Name);
                var index = this.SocialMediaList.SocialMedia.IndexOf(entry);
                var socialMediaEntry = this.SocialMediaList.SocialMedia[index];
                var oldValue = new SocialMediaEntry();

                socialMediaEntry.CopyTo(oldValue);
                socialMediaEntryViewable.CopyTo(socialMediaEntry);

                this.RaiseObjectPropertiesChanged(sender, socialMediaEntry, oldValue);
            }
        }

        /// <summary>   Property changed. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2021. </remarks>
        ///
        /// <param name="sender">           Source of the event. </param>
        /// <param name="socialMediaEntry"> The social media entry. </param>

        public void PropertyChanged(object sender, SocialMediaEntry socialMediaEntry)
        {
        }

        /// <summary>   Property changed. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2021. </remarks>
        ///
        /// <param name="sender">       Source of the event. </param>
        /// <param name="tellOthers">   The tell others. </param>

        public void PropertyChanged(object sender, TellOthers tellOthers)
        {
            var oldValue = txtTellOthersLink.Text;

            txtTellOthersLink.Text = tellOthers.Url;

            if (this.DocumentManagementState != DocumentManagementState.InitializingFromState)
            {
                this.RaiseObjectPropertiesChanged(sender, tellOthers, oldValue);
            }
        }

        /// <summary>   Event handler. Called by txtAdvertisingLink for key down events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Key event information. </param>

        private void txtAdvertisingLink_KeyDown(object sender, KeyEventArgs e)
        {
            var rect = txtAdvertisingLink.ClientRectangle;
            var url = txtAdvertisingLink.Text;

            if (txtAdvertisingLink.Tag is string && ((string)txtAdvertisingLink.Tag) == "Invalid")
            {
                this.DelayInvoke(1, () =>
                {
                    rect.Y = rect.Height - 5;
                    rect.Height = 5;

                    txtOtherLinks.Invalidate(rect);
                    txtOtherLinks.Update();
                });
            }

            timerUrlValidation.Start();
            validationQueue.Enqueue(txtAdvertisingLink);
        }

        /// <summary>   Event handler. Called by txtConnectWithUsLink for key down events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Key event information. </param>

        private void txtConnectWithUsLink_KeyDown(object sender, KeyEventArgs e)
        {
            var rect = txtConnectWithUsLink.ClientRectangle;
            var url = txtConnectWithUsLink.Text;

            if (txtConnectWithUsLink.Tag is string && ((string)txtConnectWithUsLink.Tag) == "Invalid")
            {
                this.DelayInvoke(1, () =>
                {
                    rect.Y = rect.Height - 5;
                    rect.Height = 5;

                    txtOtherLinks.Invalidate(rect);
                    txtOtherLinks.Update();
                });
            }

            timerUrlValidation.Start();
            validationQueue.Enqueue(txtConnectWithUsLink);
        }

        /// <summary>   Event handler. Called by timerSaveEntries for tick events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void timerSaveEntries_Tick(object sender, EventArgs e)
        {
            SaveEntries();
        }

        /// <summary>   Event handler. Called by txtAdvertisingLink for text changed events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void txtAdvertisingLink_TextChanged(object sender, EventArgs e)
        {
            if (this.DocumentManagementState == DocumentManagementState.InitializingFromState)
            {
                return;
            }

            if (!timerSaveEntries.Enabled)
            {
                timerSaveEntries.Start();
            }
        }

        /// <summary>   Event handler. Called by txtConnectWithUsLink for text changed events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void txtConnectWithUsLink_TextChanged(object sender, EventArgs e)
        {
            if (this.DocumentManagementState == DocumentManagementState.InitializingFromState)
            {
                return;
            }

            if (!timerSaveEntries.Enabled)
            {
                timerSaveEntries.Start();
            }
        }

        /// <summary>   Saves the entries. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/14/2021. </remarks>

        public void SaveEntries()
        {
            if (this.EmailUsLink != "mailto:" + txtEmailUsLink.Text)
            {
                var oldValue = this.EmailUsLink;

                this.EmailUsLink = "mailto:" + txtEmailUsLink.Text;

                if (this.OtherProperties.ContainsKey("EmailUsLink"))
                {
                    this.OtherProperties["EmailUsLink"] = this.EmailUsLink;
                }
                else
                {
                    this.OtherProperties.Add("EmailUsLink", this.EmailUsLink);
                }

                this.RaiseObjectPropertiesChanged(this, this.EmailUsLink, oldValue);
            }


            if (this.AdvertisingLink != txtAdvertisingLink.Text)
            {
                var oldValue = this.AdvertisingLink;

                this.AdvertisingLink = txtAdvertisingLink.Text;

                if (this.OtherProperties.ContainsKey("AdvertisingLink"))
                {
                    this.OtherProperties["AdvertisingLink"] = this.AdvertisingLink;
                }
                else
                {
                    this.OtherProperties.Add("AdvertisingLink", this.AdvertisingLink);
                }

                this.RaiseObjectPropertiesChanged(this, this.AdvertisingLink, oldValue);
            }

            if (this.ConnectWithUsLink != txtConnectWithUsLink.Text)
            {
                var oldValue = this.ConnectWithUsLink;

                this.ConnectWithUsLink = txtConnectWithUsLink.Text;

                if (this.OtherProperties.ContainsKey("ConnectWithUsLink"))
                {
                    this.OtherProperties["ConnectWithUsLink"] = this.ConnectWithUsLink;
                }
                else
                {
                    this.OtherProperties.Add("ConnectWithUsLink", this.ConnectWithUsLink);
                }

                this.RaiseObjectPropertiesChanged(this, this.AdvertisingLink, oldValue);
            }

            timerSaveEntries.Stop();
        }

        /// <summary>   Event handler. Called by txtEmailUsLink for enter events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void txtEmailUsLink_Enter(object sender, EventArgs e)
        {
            if (!ShowLinkProperties(txtEmailUsLink, "mailto:" + txtEmailUsLink.Text))
            {
                objectPropertyGrid.Title = "Invalid Link";
                objectPropertyGrid.SelectedObject = null;
            }
        }

        /// <summary>   Event handler. Called by txtEmailUsLink for key down events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Key event information. </param>

        private void txtEmailUsLink_KeyDown(object sender, KeyEventArgs e)
        {
            var rect = txtEmailUsLink.ClientRectangle;
            var url = "mailto:" + txtEmailUsLink.Text;

            if (txtEmailUsLink.Tag is string && ((string)txtEmailUsLink.Tag) == "Invalid")
            {
                this.DelayInvoke(1, () =>
                {
                    rect.Y = rect.Height - 5;
                    rect.Height = 5;

                    txtOtherLinks.Invalidate(rect);
                    txtOtherLinks.Update();
                });
            }

            timerUrlValidation.Start();
            validationQueue.Enqueue(txtEmailUsLink);
        }

        /// <summary>   Event handler. Called by txtEmailUsLink for leave events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void txtEmailUsLink_Leave(object sender, EventArgs e)
        {
            CheckObjectPropertiesFocus();
        }

        /// <summary>   Event handler. Called by txtEmailUsLink for text changed events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void txtEmailUsLink_TextChanged(object sender, EventArgs e)
        {
            if (this.DocumentManagementState == DocumentManagementState.InitializingFromState)
            {
                return;
            }

            if (!timerSaveEntries.Enabled)
            {
                timerSaveEntries.Start();
            }
        }
    }

    /// <summary>   A link data. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>

    public class LinkData
    {
        /// <summary>   Gets or sets the control. </summary>
        ///
        /// <value> The control. </value>

        public Control Control { get; set; }

        /// <summary>   Gets or sets the word. </summary>
        ///
        /// <value> The word. </value>

        public string Word { get; set; }

        /// <summary>   Gets or sets a unique identifier of the item. </summary>
        ///
        /// <value> Unique identifier of the item. </value>

        public Guid ItemGuid { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="word">         The word. </param>
        /// <param name="control">      The control. </param>

        public LinkData(string word, Control control)
        {
            this.Word = word;
            this.Control = control;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/19/2021. </remarks>
        ///
        /// <param name="word">     The word. </param>
        /// <param name="control">  The control. </param>
        /// <param name="itemGuid"> Unique identifier of the item. </param>

        public LinkData(string word, Control control, Guid itemGuid)
        {
            this.Word = word;
            this.Control = control;
            this.ItemGuid = itemGuid;
        }

        /// <summary>   Query if this  intersects the given linkData. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/16/2021. </remarks>
        ///
        /// <param name="linkData"> Information describing the link. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool Intersects(LinkData linkData)
        {
            if (linkData.Control == this.Control && linkData.ItemGuid == this.ItemGuid)
            {
                return true;
            }

            return false;
        }
    }
}
