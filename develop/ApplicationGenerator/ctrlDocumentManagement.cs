// file:	DocumentManagement.cs
//
// summary:	Implements the document management class

using AbstraX.ObjectProperties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using Utils.Controls.ScreenCapture;

namespace AbstraX
{    
    /// <summary>   An document management. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/4/2020. </remarks>

    public partial class ctrlDocumentManagement : ctrlResourceManagementBase 
    {
        /// <summary>   Event queue for all listeners interested in DocumentChanged events. </summary>
        public event EventHandler DocumentChanged;  
        /// <summary>   Event queue for all listeners interested in DocumentUrlChanged events. </summary>
        public event EventHandler DocumentUrlChanged;

        /// <summary>
        /// Event queue for all listeners interested in DocumentEmailAddressChanged events.
        /// </summary>

        public event EventHandler DocumentEmailAddressChanged;

        /// <summary>
        /// Event queue for all listeners interested in DocumentMailingAddressChanged events.
        /// </summary>

        public event EventHandler DocumentMailingAddressChanged;

        /// <summary>
        /// Event queue for all listeners interested in DocumentDetailsRtfChanged events.
        /// </summary>

        public event EventHandler DocumentDetailsRtfChanged;
        private string documentFileName;
        private string documentTypeName;
        private bool editingUrlLink;
        private bool editingEmailAddressLink;
        private TextEdit textEditUrl;
        private TextEdit textEditEmailAddress;
        private string defaultLabelUrlText;
        private string defaultLabelEmailAddressText;
        private int defaultDocumentDetailsTop;
        private int defaultDocumentDetailsHeight;
        private string documentUrl;
        private string documentEmailAddress;
        private string inquiryTypeName;
        private bool isValidEmailAddress = true;
        private bool isValidUrl = true;
        private string documentDetailsRtf;
        private string documentMailingAddress;
        private EventHandler objectPropertyGridLeave;

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/12/2021. </remarks>

        public ctrlDocumentManagement()
        {
            InitializeComponent();

            defaultLabelUrlText = linkLabelUrl.Text;
            defaultLabelEmailAddressText = linkLabelEmailAddress.Text;
            defaultDocumentDetailsTop = panelDocumentDetails.Top;
            defaultDocumentDetailsHeight = panelDocumentDetails.Height;
        }

        /// <summary>   Sets the name of the document type. </summary>
        ///
        /// <value> The name of the document type. </value>

        public string DocumentTypeName
        {
            set
            {
                documentTypeName = value;
                cmdSelectDocument.Text = cmdSelectDocument.Text.Replace("$DocumentType$", documentTypeName);
                labelUrl.Text = labelUrl.Text.Replace("$DocumentType$", documentTypeName);
                labelDocumentDetails.Text = labelDocumentDetails.Text.Replace("$DocumentType$", documentTypeName);
            }

            get
            {
                return documentTypeName;
            }
        }

        /// <summary>   Gets or sets the name of the inquiry type. </summary>
        ///
        /// <value> The name of the inquiry type. </value>

        public string InquiryTypeName
        {
            set
            {
                inquiryTypeName = value;
                labelEmailAddress.Text = labelEmailAddress.Text.Replace("$InquiryType$", inquiryTypeName);
                labelMailingAddress.Text = labelMailingAddress.Text.Replace("$InquiryType$", inquiryTypeName);
            }

            get
            {
                return inquiryTypeName;
            }
        }

        /// <summary>   Gets or sets a value indicating whether the mailing address is shown. </summary>
        ///
        /// <value> True if show mailing address, false if not. </value>

        public bool ShowMailingAddress
        {
            get
            {
                return panelMailingAddress.Visible;
            }

            set
            {
                var show = value;

                if (show)
                {
                    panelMailingAddress.Visible = true;

                    if (panelDocumentDetails.Top != defaultDocumentDetailsTop)
                    {
                        panelDocumentDetails.Top = defaultDocumentDetailsTop;
                    }
                }
                else
                {
                    var diff = panelDocumentDetails.Top - panelMailingAddress.Top;

                    panelMailingAddress.Visible = false;
                    panelDocumentDetails.Top = panelMailingAddress.Top;
                    panelDocumentDetails.Height = defaultDocumentDetailsHeight + diff;
                }
            }
        }

        /// <summary>   Gets or sets the filename of the document file. </summary>
        ///
        /// <value> The filename of the document file. </value>

        public string DocumentFileName 
        { 
            get
            {
                return documentFileName;
            }

            set
            {
                documentFileName = value;

                if (!documentFileName.IsNullOrEmpty() && this.DocumentManagementState != DocumentManagementState.SavingDetails)
                {
                    LoadDocumentFile(documentFileName);
                }
            }
        }

        /// <summary>   Gets or sets URL of the document. </summary>
        ///
        /// <value> The document URL. </value>

        public string DocumentUrl
        {
            get
            {
                return documentUrl;
            }

            set
            {
                documentUrl = value;

                if (!documentUrl.IsNullOrEmpty())
                {
                    linkLabelUrl.Text = documentUrl;

                    timerUrlValidation.Start();
                }
            }
        }

        /// <summary>   Gets or sets the document email address. </summary>
        ///
        /// <value> The document email address. </value>

        public string DocumentEmailAddress
        {
            get
            {
                return documentEmailAddress;
            }

            set
            {
                documentEmailAddress = value;

                if (!documentEmailAddress.IsNullOrEmpty())
                {
                    linkLabelEmailAddress.Text = documentEmailAddress;

                    timerEmailAddressValidation.Start();
                }
            }
        }

        /// <summary>   Gets or sets the document mailing address. </summary>
        ///
        /// <value> The document mailing address. </value>

        public string DocumentMailingAddress
        {
            get
            {
                return documentMailingAddress;
            }

            set
            {
                documentMailingAddress = value;
                txtMailingAddress.Text = documentMailingAddress;
            }
        }

        /// <summary>   Gets the document details RTF. </summary>
        ///
        /// <value> The document details RTF. </value>

        public string DocumentDetailsRtf
        {
            get
            {
                return documentDetailsRtf;
            }
        }

        /// <summary>   Creates a handle for the control. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/5/2020. </remarks>

        protected override void CreateHandle()
        {
            base.CreateHandle();

            if (this.DocumentFileName != null)
            {
                ctrlEditor.LoadFile(documentFileName, RichTextBoxStreamType.RichText);

                txtDocumentName.Text = this.DocumentFileName;
            }
        }

        /// <summary>   Event handler. Called by cmdSelectDocument for click events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/4/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void cmdSelectDocument_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Document files|*.txt;*.rtf;|All files|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadDocumentFile(openFileDialog.FileName);
            }
        }

        private void LoadDocumentFile(string fileName)
        {
            string extension;

            extension = Path.GetExtension(fileName);

            documentFileName = fileName;

            switch (extension)
            {
                case ".rtf":
                    ctrlEditor.LoadFile(documentFileName, RichTextBoxStreamType.RichText);
                    break;
                case ".txt":
                    ctrlEditor.LoadFile(documentFileName, RichTextBoxStreamType.PlainText);
                    break;
            }

            txtDocumentName.Text = this.DocumentFileName;

            if (this.DocumentManagementState != DocumentManagementState.InitializingFromState)
            {
                using (this.SetState(DocumentManagementState.SavingDetails))
                {
                    DocumentChanged(this, EventArgs.Empty);
                }
            }
        }

        private void txtDocumentName_TextChanged(object sender, EventArgs e)
        {
            var fileName = txtDocumentName.Text;

            if (File.Exists(fileName))
            {
                this.LoadDocumentFile(fileName);
            }
            else
            {
                this.DocumentFileName = null;
                ctrlEditor.Clear();
            }

            if (this.DocumentManagementState != DocumentManagementState.InitializingFromState)
            {
                using (this.SetState(DocumentManagementState.SavingDetails))
                {
                    DocumentChanged(this, EventArgs.Empty);
                }
            }
        }

        private void linkLabelUrl_MouseHover(object sender, EventArgs e)
        {
            textEditUrl = linkLabelUrl.Parent.GetTextEdit();

            if (Keys.ControlKey.IsPressed())
            {
                if (textEditUrl != null)
                {
                    textEditUrl.Visible = false;
                }
            }
            else
            {
                if (textEditUrl != null)
                {
                    textEditUrl.Visible = true;
                }
                else
                {
                    ShowUrlTextEdit();
                }
            }
        }

        private void linkLabelUrl_MouseClick(object sender, MouseEventArgs e)
        {
            if (Keys.ControlKey.IsPressed())
            {
                if (linkLabelUrl.Text != defaultLabelUrlText)
                {
                    linkLabelUrl.LinkVisited = true;

                    Process.Start(linkLabelUrl.Text);
                }
            }
            else
            {
                ShowUrlTextEdit();
            }
        }

        private void ShowUrlTextEdit()
        {
            var destroying = false;
            var loading = true;
            Action<bool> destroyEdit;
            Rectangle linkRect;
            Rectangle invalid;

            if (this.editingUrlLink)
            {
                return;
            }

            if (linkLabelUrl.Text == defaultLabelUrlText)
            {
                textEditUrl = new TextEdit();
            }
            else
            {
                textEditUrl = new TextEdit(linkLabelUrl.Text);
            }

            linkRect = linkLabelUrl.Bounds;

            textEditUrl.Font = new Font(textEditUrl.Font, FontStyle.Underline);
            textEditUrl.ForeColor = ColorTranslator.FromHtml("#0003fe");
            textEditUrl.BackColor = SystemColors.Control;

            lblUrlNote.FadeOut(100);
            this.DoEventsSleep(100);

            if (textEditUrl == null)
            {
                DebugUtils.Break();
            }

            textEditUrl.CreateControl();
            textEditUrl.SetAsChildOf(linkLabelUrl.Parent);

            linkRect.Width += 200;

            textEditUrl.SetWindowPos(linkRect, ControlExtensions.SetWindowPosFlags.ShowWindow);
            textEditUrl.BringToFront();
            textEditUrl.Focus();

            ShowLinkProperties(textEditUrl);

            this.editingUrlLink = true;

            invalid = textEditUrl.Bounds;
            invalid.Inflate(6, 6);

            this.Invalidate(invalid);

            loading = false;

            destroyEdit = (restoreOnEmpty) =>
            {
                if (destroying || loading)
                {
                    return;
                }

                destroying = true;

                HideLinkProperties(textEditUrl);

                textEditUrl = linkLabelUrl.Parent.GetTextEdit();

                if (textEditUrl != null)
                {
                    if (textEditUrl.Text.IsNullOrEmpty() && restoreOnEmpty)
                    {
                        linkLabelUrl.Text = defaultLabelUrlText;
                    }
                    else
                    {
                        documentUrl = textEditUrl.Text;

                        linkLabelUrl.Text = documentUrl;
                        DocumentUrlChanged(this, EventArgs.Empty);
                    }
                }

                // Guess destroy doesn't always mean destroy

                while (textEditUrl != null)
                {
                    textEditUrl.SetAsChildOf(null);
                    textEditUrl.Destroy();

                    textEditUrl = linkLabelUrl.GetTextEdit();
                }

                timerUrlValidation.Start();

                this.editingUrlLink = false;

                linkLabelUrl.Parent.Invalidate();
                linkLabelUrl.Parent.GetParentForm().Refresh();

                lblUrlNote.FadeIn(200);
            };

            textEditUrl.MouseHover += (sender2, e2) =>
            {
                if (Keys.ControlKey.IsPressed())
                {
                    textEditUrl.Visible = false;
                }
                else
                {
                    textEditUrl.Visible = true;
                }
            };

            textEditUrl.MouseLeave += (sender2, e2) =>
            {
                var hwndFocus = ControlExtensions.GetFocus();

                if (textEditUrl.Handle != hwndFocus && objectPropertyGrid.Handle != hwndFocus)
                {
                    destroyEdit(true);
                }
            };


            textEditUrl.LostFocus += (sender2, e2) =>
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
                        destroyEdit(true);
                    }

                    objectPropertyGridLeave = new EventHandler((sender3, e3) =>
                    {
                        destroyEdit(true);

                        objectPropertyGrid.LostFocus -= objectPropertyGridLeave;

                    });

                    objectPropertyGrid.Leave += objectPropertyGridLeave;
                }
            };

            textEditUrl.KeyDown += (sender2, e2) =>
            {
                if (e2.KeyCode == Keys.Enter)
                {
                    destroyEdit(false);
                }
                else if (e2.KeyCode == Keys.Escape)
                {
                    destroyEdit(true);
                }
                else
                {
                    if (!timerUrlValidation.Enabled)
                    {
                        timerUrlValidation.Start();
                    }
                }
            };
        }

        private void ShowEmailTextEdit()
        {
            var destroying = false;
            var loading = true;
            Action<bool> destroyEdit;
            Rectangle linkRect;
            Rectangle invalid;

            if (this.editingEmailAddressLink)
            {
                return;
            }

            if (linkLabelEmailAddress.Text == defaultLabelEmailAddressText)
            {
                textEditEmailAddress = new TextEdit();
            }
            else
            {
                textEditEmailAddress = new TextEdit(linkLabelEmailAddress.Text);
            }

            linkRect = linkLabelEmailAddress.Bounds;

            textEditEmailAddress.Font = new Font(textEditEmailAddress.Font, FontStyle.Underline);
            textEditEmailAddress.ForeColor = ColorTranslator.FromHtml("#0003fe");
            textEditEmailAddress.BackColor = SystemColors.Control;

            lblEmailAddressNote.Visible = false;
            lblEmailAddressNote.FadeOut(100);
            this.DoEventsSleep(100);

            textEditEmailAddress.CreateControl();
            textEditEmailAddress.SetAsChildOf(linkLabelEmailAddress.Parent);

            linkRect.Width += 200;

            textEditEmailAddress.SetWindowPos(linkRect, ControlExtensions.SetWindowPosFlags.ShowWindow);
            textEditEmailAddress.BringToFront();
            textEditEmailAddress.Focus();

            ShowLinkProperties(textEditEmailAddress);

            this.editingEmailAddressLink = true;

            invalid = textEditEmailAddress.Bounds;
            invalid.Inflate(6, 6);

            this.Invalidate(invalid);

            loading = false;

            destroyEdit = (restoreOnEmpty) =>
            {
                if (destroying || loading)
                {
                    return;
                }

                destroying = true;

                HideLinkProperties(textEditEmailAddress);
                textEditEmailAddress = linkLabelEmailAddress.Parent.GetTextEdit();

                if (textEditEmailAddress != null)
                {
                    if (textEditEmailAddress.Text.IsNullOrEmpty() && restoreOnEmpty)
                    {
                        linkLabelEmailAddress.Text = defaultLabelEmailAddressText;
                    }
                    else
                    {
                        documentEmailAddress = textEditEmailAddress.Text;

                        linkLabelEmailAddress.Text = documentEmailAddress;
                        DocumentEmailAddressChanged(this, EventArgs.Empty);
                    }
                }

                // Guess destroy doesn't always mean destroy

                while (textEditEmailAddress != null)
                {
                    textEditEmailAddress.SetAsChildOf(null);
                    textEditEmailAddress.Destroy();

                    textEditEmailAddress = linkLabelEmailAddress.GetTextEdit();
                }

                this.editingEmailAddressLink = false;

                timerEmailAddressValidation.Start();

                linkLabelEmailAddress.Parent.Invalidate();
                linkLabelEmailAddress.Parent.GetParentForm().Refresh();

                lblEmailAddressNote.FadeIn(200);
            };

            textEditEmailAddress.MouseHover += (sender2, e2) =>
            {
                if (Keys.ControlKey.IsPressed())
                {
                    textEditEmailAddress.Visible = false;
                }
                else
                {
                    textEditEmailAddress.Visible = true;
                }
            };

            textEditEmailAddress.MouseLeave += (sender2, e2) =>
            {
                var hwndFocus = ControlExtensions.GetFocus();

                if (textEditEmailAddress.Handle != hwndFocus && objectPropertyGrid.Handle != hwndFocus)
                {
                    destroyEdit(true);
                }
            };

            textEditEmailAddress.LostFocus += (sender2, e2) =>
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
                        destroyEdit(true);
                    }

                    objectPropertyGridLeave = new EventHandler((sender3, e3) =>
                    {
                        destroyEdit(true);

                        objectPropertyGrid.LostFocus -= objectPropertyGridLeave;

                    });

                    objectPropertyGrid.Leave += objectPropertyGridLeave;
                }
            };

            textEditEmailAddress.KeyDown += (sender2, e2) =>
            {
                if (e2.KeyCode == Keys.Enter)
                {
                    destroyEdit(false);
                }
                else if (e2.KeyCode == Keys.Escape)
                {
                    destroyEdit(true);
                }
                else
                {
                    if (!timerEmailAddressValidation.Enabled)
                    {
                        timerEmailAddressValidation.Start();
                    }
                }
            };
        }

        private void HideLinkProperties(TextEdit textEditEmailAddress)
        {
            objectPropertyGrid.Title = "Object Properties";
            objectPropertyGrid.SelectedObject = null;
        }

        private void ShowLinkProperties(TextEdit textEdit)
        {
            LinkProperties properties;
            var linkText = textEdit.Text;

            if (!linkText.IsValidEmailAddress() && !linkText.IsValidUrl())
            {
                return;
            }

            if (!this.ObjectProperties.ContainsKey(linkText))
            {
                properties = new LinkProperties(linkText);

                 this.ObjectProperties.Add(linkText, properties);
            }
            else if (ObjectProperties[linkText] is LinkProperties)
            {
                properties = this.ObjectProperties[linkText];
            }
            else
            {
                var objectProperties = (ObjectPropertiesDictionary)this.ObjectProperties[linkText];

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

                this.ObjectProperties[linkText] = properties;
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
        }

        /// <summary>   Resets this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>

        public void Reset()
        {
            linkLabelUrl.Text = "[None]";
            linkLabelEmailAddress.Text = "[None]";
            
            txtMailingAddress.Text = null;
            txtDocumentName.Text = null;

            objectPropertyGrid.SelectedObject = null;

            this.DocumentTypeName = documentTypeName;
            this.InquiryTypeName = inquiryTypeName;

            ctrlEditor.Clear();
        }

        private void linkLabelUrl_MouseLeave(object sender, EventArgs e)
        {
            if (textEditUrl == null)
            {
                editingUrlLink = false;
            }
        }

        private void linkLabelUrl_Paint(object sender, PaintEventArgs e)
        {
            PaintUrlIndicators(e.Graphics, e.ClipRectangle);
        }

        private void PaintUrlIndicators(Graphics graphics, Rectangle rectangle)
        {
            if (!isValidUrl)
            {
                graphics.DrawErrorSquiggly(linkLabelUrl.Text, linkLabelUrl.Font, rectangle);
            }
        }

        private void timerUrlValidation_Tick(object sender, EventArgs e)
        {
            if (textEditUrl != null && textEditUrl.IsHandleCreated)
            {
                var url = textEditUrl.Text;

                if (url.IsValidUrl())
                {
                    textEditUrl.IsValidText = true;
                    isValidUrl = true;

                    ShowLinkProperties(textEditUrl);
                }
                else
                {
                    textEditUrl.IsValidText = false;
                    isValidUrl = false;

                    HideLinkProperties(textEditUrl);
                }
            }
            else if (linkLabelUrl.Text != defaultLabelUrlText)
            {
                var url = linkLabelUrl.Text;

                if (url.IsValidUrl())
                {
                    isValidUrl = true;
                    linkLabelUrl.Refresh();
                }
                else
                {
                    isValidUrl = false;

                    using (var graphics = linkLabelUrl.CreateGraphics())
                    {
                        PaintUrlIndicators(graphics, linkLabelUrl.Bounds);
                    }
                }
            }
            else
            {
                isValidUrl = true;
                linkLabelUrl.Refresh();
            }

            timerUrlValidation.Stop();
        }

        private void linkLabelEmailAddress_MouseHover(object sender, EventArgs e)
        {
            textEditEmailAddress = linkLabelEmailAddress.Parent.GetTextEdit();

            if (Keys.ControlKey.IsPressed())
            {
                if (textEditEmailAddress != null)
                {
                    textEditEmailAddress.Visible = false;
                }
            }
            else
            {
                if (textEditEmailAddress != null)
                {
                    textEditEmailAddress.Visible = true;
                }
                else
                {
                    ShowEmailTextEdit();
                }
            }
        }

        private void linkLabelEmailAddress_MouseClick(object sender, MouseEventArgs e)
        {
            if (Keys.ControlKey.IsPressed())
            {
                if (linkLabelEmailAddress.Text != defaultLabelUrlText)
                {
                    linkLabelEmailAddress.LinkVisited = true;

                    Process.Start(linkLabelEmailAddress.Text);
                }
            }
            else
            {
                ShowEmailTextEdit();
            }
        }

        private void linkLabelEmailAddress_MouseLeave(object sender, EventArgs e)
        {
            if (textEditEmailAddress == null)
            {
                editingEmailAddressLink = false;
            }
        }

        private void linkLabelEmailAddress_Paint(object sender, PaintEventArgs e)
        {
            PaintEmailAddressIndicators(e.Graphics, e.ClipRectangle);
        }

        private void PaintEmailAddressIndicators(Graphics graphics, Rectangle rectangle)
        {
            if (!isValidEmailAddress)
            {
                graphics.DrawErrorSquiggly(linkLabelEmailAddress.Text, linkLabelEmailAddress.Font, rectangle);
            }
        }

        private void timerEmailAddressValidation_Tick(object sender, EventArgs e)
        {
            if (textEditEmailAddress != null && textEditEmailAddress.IsHandleCreated)
            {
                var emailAddress = textEditEmailAddress.Text;

                if (emailAddress.IsValidEmailAddress())
                {
                    textEditEmailAddress.IsValidText = true;
                    isValidEmailAddress = true;

                    ShowLinkProperties(textEditEmailAddress);
                }
                else
                {
                    textEditEmailAddress.IsValidText = false;
                    isValidEmailAddress = false;

                    HideLinkProperties(textEditEmailAddress);
                }
            }
            else if (linkLabelEmailAddress.Text != defaultLabelEmailAddressText)
            {
                var url = linkLabelEmailAddress.Text;

                if (url.IsValidEmailAddress())
                {
                    isValidEmailAddress = true;
                    linkLabelEmailAddress.Refresh();
                }
                else
                {
                    isValidEmailAddress = false;

                    using (var graphics = linkLabelEmailAddress.CreateGraphics())
                    {
                        PaintEmailAddressIndicators(graphics, linkLabelEmailAddress.Bounds);
                    }
                }
            }
            else
            {
                isValidEmailAddress = true;
                linkLabelEmailAddress.Refresh();
            }

            timerEmailAddressValidation.Stop();
        }

        private void textBoxMailingAddress_TextChanged(object sender, EventArgs e)
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

        private void ctrlEditor_DocumentTextChanged(object sender, EventArgs e)
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

        private void ctrlEditor_DocumentLeave(object sender, EventArgs e)
        {
            if (timerSaveEntries.Enabled)
            {
                timerSaveEntries.Stop();
            }
        }

        private void textBoxMailingAddress_Leave(object sender, EventArgs e)
        {
            if (timerSaveEntries.Enabled)
            {
                timerSaveEntries.Stop();
            }
        }

        private void timerSaveEntries_Tick(object sender, EventArgs e)
        {
            SaveEntries();
        }

        /// <summary>   Saves the entries. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/17/2020. </remarks>

        public void SaveEntries()
        {
            timerSaveEntries.Stop();

            if (documentDetailsRtf != ctrlEditor.RichText && ctrlEditor.HasChanges)
            {
                documentDetailsRtf = ctrlEditor.RichText;

                using (this.SetState(DocumentManagementState.SavingDetails))
                {
                    DocumentDetailsRtfChanged(this, EventArgs.Empty);
                }
            }

            if (documentMailingAddress != txtMailingAddress.Text)
            {
                documentMailingAddress = txtMailingAddress.Text;

                DocumentMailingAddressChanged(this, EventArgs.Empty);
            }
        }

        private void ctrlDocumentManagement_Paint(object sender, PaintEventArgs e)
        {
            if (editingUrlLink)
            {
                var rect = textEditUrl.Bounds;
                var color = ColorTranslator.FromHtml("#e67700");
                var pen = new Pen(color);

                rect.Inflate(2, 2);

                e.Graphics.DrawRectangle(pen, rect);
                color = color.Lighten(.75);

                for (var x = 0; x < 4; x++)
                {
                    rect.Inflate(1, 1);
                    pen = new Pen(color);

                    e.Graphics.DrawRoundedRectangle(pen, rect, 2);

                    color = color.Lighten(.10);
                }
            }
            
            if (editingEmailAddressLink)
            {
                var rect = textEditEmailAddress.Bounds;
                var color = ColorTranslator.FromHtml("#e67700");
                var pen = new Pen(color);

                rect.Inflate(2, 2);

                e.Graphics.DrawRectangle(pen, rect);
                color = color.Lighten(.75);

                for (var x = 0; x < 4; x++)
                {
                    rect.Inflate(1, 1);
                    pen = new Pen(color);

                    e.Graphics.DrawRoundedRectangle(pen, rect, 2);

                    color = color.Lighten(.10);
                }
            }
        }

        private void ctrlEditor_OnImageSelected(object obj, OnImageSelectedEventArgs e)
        {
            var imageNumber = string.Join(string.Empty, e.Identifier.RegexGetMatches(@"\d+").Select(m => m.Value));
            ImageProperties properties;

            if (!this.ObjectProperties.ContainsKey(e.Identifier))
            {
                properties = new ImageProperties(e.Identifier);

                this.ObjectProperties.Add(e.Identifier, properties);
            }
            else if (ObjectProperties[e.Identifier] is ImageProperties)
            {
                properties = ObjectProperties[e.Identifier];
            }
            else
            {
                var objectProperties = (ObjectPropertiesDictionary)this.ObjectProperties[e.Identifier];

                properties = new ImageProperties(e.Identifier);

                foreach (var pair in objectProperties)
                {
                    var key = pair.Key;
                    var value = (object) pair.Value;
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

                this.ObjectProperties[e.Identifier] = properties;
            }


            if (properties.Name == null)
            {
                objectPropertyGrid.Title = "Image " + imageNumber;
            }
            else
            {
                objectPropertyGrid.Title = properties.Name;
            }

            objectPropertyGrid.SelectedObject = properties;
        }

        private void ctrlEditor_OnLinkSelected(object obj, OnLinkSelectedEventArgs e)
        {
            LinkProperties properties;
            var linkText = e.Link;

            if (!linkText.IsValidEmailAddress() && !linkText.IsValidUrl())
            {
                return;
            }

            if (!this.ObjectProperties.ContainsKey(e.Identifier))
            {
                properties = new LinkProperties(e.Identifier);

                this.ObjectProperties.Add(e.Identifier, properties);
            }
            else if (ObjectProperties[e.Identifier] is LinkProperties)
            {
                properties = this.ObjectProperties[e.Identifier];
            }
            else
            {
                var objectProperties = (ObjectPropertiesDictionary)this.ObjectProperties[e.Identifier];

                properties = new LinkProperties(e.Identifier);

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

                this.ObjectProperties[e.Identifier] = properties;
            }

            if (properties.Name == null)
            {
                objectPropertyGrid.Title = e.Identifier;
            }
            else
            {
                objectPropertyGrid.Title = properties.Name;
            }

            objectPropertyGrid.SelectedObject = properties;
        }

        private void ctrlEditor_OnSelectionChanged(object sender, EventArgs e)
        {
            objectPropertyGrid.Title = "Object Properties";
            objectPropertyGrid.SelectedObject = null;
        }

        private void objectPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            base.RaiseObjectPropertiesChanged(s, e);
        }

        /// <summary>   Destroys the handle associated with the control. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/12/2021. </remarks>

        public void OnClose()
        {
            foreach (var property in this.ObjectProperties.Where(p => p.Value is LinkProperties).ToList())
            {
                var key = property.Key;
                var value = (LinkProperties) property.Value;

                if (value.Name.IsNullOrEmpty() && value.LinkCallToAction.IsNullOrEmpty())
                {
                    this.ObjectProperties.Remove(key);
                }
            }
        }

        private void ctrlEditor_OnLinkChanged(object obj, OnLinkChangedEventArgs e)
        {
            if (this.ObjectProperties.ContainsKey(e.OldLink))
            {
                this.ObjectProperties.Remove(e.OldLink);
                e.DeleteOld = true;
            }
        }
    }
}
