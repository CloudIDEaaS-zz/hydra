
using Utils;

namespace AbstraX
{
    partial class ctrlDocumentManagement
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.txtDocumentName = new System.Windows.Forms.TextBox();
            this.cmdSelectDocument = new System.Windows.Forms.Button();
            this.toolTipCaptureDocument = new System.Windows.Forms.ToolTip(this.components);
            this.linkLabelUrl = new System.Windows.Forms.LinkLabel();
            this.labelUrl = new System.Windows.Forms.Label();
            this.timerUrlValidation = new System.Windows.Forms.Timer(this.components);
            this.labelEmailAddress = new System.Windows.Forms.Label();
            this.linkLabelEmailAddress = new System.Windows.Forms.LinkLabel();
            this.labelMailingAddress = new System.Windows.Forms.Label();
            this.txtMailingAddress = new System.Windows.Forms.TextBox();
            this.labelDocumentDetails = new System.Windows.Forms.Label();
            this.timerEmailAddressValidation = new System.Windows.Forms.Timer(this.components);
            this.timerSaveEntries = new System.Windows.Forms.Timer(this.components);
            this.panelMailingAddress = new System.Windows.Forms.Panel();
            this.panelDocumentDetails = new System.Windows.Forms.Panel();
            this.objectPropertyGrid = new Utils.TitlePropertyGrid();
            this.panelEditor = new System.Windows.Forms.Panel();
            this.ctrlEditor = new Utils.ctrlEditor();
            this.lblUrlNote = new System.Windows.Forms.Label();
            this.lblEmailAddressNote = new System.Windows.Forms.Label();
            this.panelMailingAddress.SuspendLayout();
            this.panelDocumentDetails.SuspendLayout();
            this.panelEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtDocumentName
            // 
            this.txtDocumentName.Location = new System.Drawing.Point(0, 5);
            this.txtDocumentName.Name = "txtDocumentName";
            this.txtDocumentName.Size = new System.Drawing.Size(558, 20);
            this.txtDocumentName.TabIndex = 0;
            this.txtDocumentName.TextChanged += new System.EventHandler(this.txtDocumentName_TextChanged);
            // 
            // cmdSelectDocument
            // 
            this.cmdSelectDocument.Location = new System.Drawing.Point(564, 3);
            this.cmdSelectDocument.Name = "cmdSelectDocument";
            this.cmdSelectDocument.Size = new System.Drawing.Size(149, 23);
            this.cmdSelectDocument.TabIndex = 1;
            this.cmdSelectDocument.Text = "Select $DocumentType$...";
            this.cmdSelectDocument.UseVisualStyleBackColor = true;
            this.cmdSelectDocument.Click += new System.EventHandler(this.cmdSelectDocument_Click);
            // 
            // linkLabelUrl
            // 
            this.linkLabelUrl.AutoSize = true;
            this.linkLabelUrl.Location = new System.Drawing.Point(157, 28);
            this.linkLabelUrl.Name = "linkLabelUrl";
            this.linkLabelUrl.Size = new System.Drawing.Size(39, 13);
            this.linkLabelUrl.TabIndex = 3;
            this.linkLabelUrl.TabStop = true;
            this.linkLabelUrl.Text = "[None]";
            this.linkLabelUrl.Paint += new System.Windows.Forms.PaintEventHandler(this.linkLabelUrl_Paint);
            this.linkLabelUrl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.linkLabelUrl_MouseClick);
            this.linkLabelUrl.MouseLeave += new System.EventHandler(this.linkLabelUrl_MouseLeave);
            this.linkLabelUrl.MouseHover += new System.EventHandler(this.linkLabelUrl_MouseHover);
            // 
            // labelUrl
            // 
            this.labelUrl.AutoSize = true;
            this.labelUrl.Location = new System.Drawing.Point(0, 28);
            this.labelUrl.Name = "labelUrl";
            this.labelUrl.Size = new System.Drawing.Size(109, 13);
            this.labelUrl.TabIndex = 4;
            this.labelUrl.Text = "$DocumentType$ url:";
            // 
            // timerUrlValidation
            // 
            this.timerUrlValidation.Interval = 2000;
            this.timerUrlValidation.Tick += new System.EventHandler(this.timerUrlValidation_Tick);
            // 
            // labelEmailAddress
            // 
            this.labelEmailAddress.Location = new System.Drawing.Point(0, 49);
            this.labelEmailAddress.Name = "labelEmailAddress";
            this.labelEmailAddress.Size = new System.Drawing.Size(151, 18);
            this.labelEmailAddress.TabIndex = 6;
            this.labelEmailAddress.Text = "$InquiryType$ email address:";
            // 
            // linkLabelEmailAddress
            // 
            this.linkLabelEmailAddress.AutoSize = true;
            this.linkLabelEmailAddress.Location = new System.Drawing.Point(157, 49);
            this.linkLabelEmailAddress.Name = "linkLabelEmailAddress";
            this.linkLabelEmailAddress.Size = new System.Drawing.Size(39, 13);
            this.linkLabelEmailAddress.TabIndex = 5;
            this.linkLabelEmailAddress.TabStop = true;
            this.linkLabelEmailAddress.Text = "[None]";
            this.linkLabelEmailAddress.Paint += new System.Windows.Forms.PaintEventHandler(this.linkLabelEmailAddress_Paint);
            this.linkLabelEmailAddress.MouseClick += new System.Windows.Forms.MouseEventHandler(this.linkLabelEmailAddress_MouseClick);
            this.linkLabelEmailAddress.MouseLeave += new System.EventHandler(this.linkLabelEmailAddress_MouseLeave);
            this.linkLabelEmailAddress.MouseHover += new System.EventHandler(this.linkLabelEmailAddress_MouseHover);
            // 
            // labelMailingAddress
            // 
            this.labelMailingAddress.Location = new System.Drawing.Point(-2, 3);
            this.labelMailingAddress.Name = "labelMailingAddress";
            this.labelMailingAddress.Size = new System.Drawing.Size(154, 18);
            this.labelMailingAddress.TabIndex = 6;
            this.labelMailingAddress.Text = "$InquiryType$ mailing address:";
            // 
            // textBoxMailingAddress
            // 
            this.txtMailingAddress.Location = new System.Drawing.Point(158, 0);
            this.txtMailingAddress.Multiline = true;
            this.txtMailingAddress.Name = "textBoxMailingAddress";
            this.txtMailingAddress.Size = new System.Drawing.Size(243, 51);
            this.txtMailingAddress.TabIndex = 7;
            this.txtMailingAddress.TextChanged += new System.EventHandler(this.textBoxMailingAddress_TextChanged);
            this.txtMailingAddress.Leave += new System.EventHandler(this.textBoxMailingAddress_Leave);
            // 
            // labelDocumentDetails
            // 
            this.labelDocumentDetails.AutoSize = true;
            this.labelDocumentDetails.Location = new System.Drawing.Point(3, 3);
            this.labelDocumentDetails.Name = "labelDocumentDetails";
            this.labelDocumentDetails.Size = new System.Drawing.Size(125, 13);
            this.labelDocumentDetails.TabIndex = 4;
            this.labelDocumentDetails.Text = "$DocumentType$ details";
            // 
            // timerEmailAddressValidation
            // 
            this.timerEmailAddressValidation.Interval = 2000;
            this.timerEmailAddressValidation.Tick += new System.EventHandler(this.timerEmailAddressValidation_Tick);
            // 
            // timerSaveEntries
            // 
            this.timerSaveEntries.Interval = 2000;
            this.timerSaveEntries.Tick += new System.EventHandler(this.timerSaveEntries_Tick);
            // 
            // panelMailingAddress
            // 
            this.panelMailingAddress.Controls.Add(this.txtMailingAddress);
            this.panelMailingAddress.Controls.Add(this.labelMailingAddress);
            this.panelMailingAddress.Location = new System.Drawing.Point(3, 70);
            this.panelMailingAddress.Name = "panelMailingAddress";
            this.panelMailingAddress.Size = new System.Drawing.Size(427, 52);
            this.panelMailingAddress.TabIndex = 9;
            // 
            // panelDocumentDetails
            // 
            this.panelDocumentDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDocumentDetails.Controls.Add(this.objectPropertyGrid);
            this.panelDocumentDetails.Controls.Add(this.panelEditor);
            this.panelDocumentDetails.Controls.Add(this.labelDocumentDetails);
            this.panelDocumentDetails.Location = new System.Drawing.Point(-2, 127);
            this.panelDocumentDetails.Name = "panelDocumentDetails";
            this.panelDocumentDetails.Size = new System.Drawing.Size(1099, 344);
            this.panelDocumentDetails.TabIndex = 10;
            // 
            // objectPropertyGrid
            // 
            this.objectPropertyGrid.Location = new System.Drawing.Point(722, 19);
            this.objectPropertyGrid.Name = "objectPropertyGrid";
            this.objectPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.CategorizedAlphabetical;
            this.objectPropertyGrid.SelectedObject = null;
            this.objectPropertyGrid.Size = new System.Drawing.Size(336, 324);
            this.objectPropertyGrid.TabIndex = 6;
            this.objectPropertyGrid.Title = "Object Properties";
            this.objectPropertyGrid.ToolbarVisible = true;
            this.objectPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.objectPropertyGrid_PropertyValueChanged);
            // 
            // panelEditor
            // 
            this.panelEditor.Controls.Add(this.ctrlEditor);
            this.panelEditor.Location = new System.Drawing.Point(6, 19);
            this.panelEditor.Name = "panelEditor";
            this.panelEditor.Size = new System.Drawing.Size(709, 324);
            this.panelEditor.TabIndex = 5;
            // 
            // ctrlEditor
            // 
            this.ctrlEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlEditor.HasChanges = false;
            this.ctrlEditor.Location = new System.Drawing.Point(0, 0);
            this.ctrlEditor.Name = "ctrlEditor";
            this.ctrlEditor.Size = new System.Drawing.Size(709, 324);
            this.ctrlEditor.TabIndex = 11;
            this.ctrlEditor.TabNext = false;
            this.ctrlEditor.TabPrevious = false;
            this.ctrlEditor.OnImageSelected += new Utils.OnImageSelectedHandler(this.ctrlEditor_OnImageSelected);
            this.ctrlEditor.OnLinkSelected += new Utils.OnLinkSelectedHandler(this.ctrlEditor_OnLinkSelected);
            this.ctrlEditor.OnLinkChanged += new Utils.OnLinkChangedHandler(this.ctrlEditor_OnLinkChanged);
            this.ctrlEditor.OnSelectionChanged += new System.EventHandler(this.ctrlEditor_OnSelectionChanged);
            this.ctrlEditor.DocumentTextChanged += new System.EventHandler(this.ctrlEditor_DocumentTextChanged);
            this.ctrlEditor.DocumentLeave += new System.EventHandler(this.ctrlEditor_DocumentLeave);
            // 
            // lblUrlNote
            // 
            this.lblUrlNote.AutoSize = true;
            this.lblUrlNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUrlNote.Location = new System.Drawing.Point(395, 28);
            this.lblUrlNote.Name = "lblUrlNote";
            this.lblUrlNote.Size = new System.Drawing.Size(113, 13);
            this.lblUrlNote.TabIndex = 4;
            this.lblUrlNote.Text = "Hover hyperlink to edit";
            // 
            // lblEmailAddressNote
            // 
            this.lblEmailAddressNote.AutoSize = true;
            this.lblEmailAddressNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmailAddressNote.Location = new System.Drawing.Point(395, 49);
            this.lblEmailAddressNote.Name = "lblEmailAddressNote";
            this.lblEmailAddressNote.Size = new System.Drawing.Size(113, 13);
            this.lblEmailAddressNote.TabIndex = 4;
            this.lblEmailAddressNote.Text = "Hover hyperlink to edit";
            // 
            // ctrlDocumentManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.panelDocumentDetails);
            this.Controls.Add(this.panelMailingAddress);
            this.Controls.Add(this.labelEmailAddress);
            this.Controls.Add(this.linkLabelEmailAddress);
            this.Controls.Add(this.labelUrl);
            this.Controls.Add(this.linkLabelUrl);
            this.Controls.Add(this.cmdSelectDocument);
            this.Controls.Add(this.txtDocumentName);
            this.Controls.Add(this.lblEmailAddressNote);
            this.Controls.Add(this.lblUrlNote);
            this.Name = "ctrlDocumentManagement";
            this.Size = new System.Drawing.Size(1099, 475);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ctrlDocumentManagement_Paint);
            this.panelMailingAddress.ResumeLayout(false);
            this.panelMailingAddress.PerformLayout();
            this.panelDocumentDetails.ResumeLayout(false);
            this.panelDocumentDetails.PerformLayout();
            this.panelEditor.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox txtDocumentName;
        private System.Windows.Forms.Button cmdSelectDocument;
        private System.Windows.Forms.ToolTip toolTipCaptureDocument;
        private System.Windows.Forms.LinkLabel linkLabelUrl;
        private System.Windows.Forms.Label labelUrl;
        private System.Windows.Forms.Timer timerUrlValidation;
        private System.Windows.Forms.Label labelEmailAddress;
        private System.Windows.Forms.LinkLabel linkLabelEmailAddress;
        private System.Windows.Forms.Label labelMailingAddress;
        private System.Windows.Forms.TextBox txtMailingAddress;
        private System.Windows.Forms.Label labelDocumentDetails;
        private System.Windows.Forms.Timer timerEmailAddressValidation;
        private System.Windows.Forms.Timer timerSaveEntries;
        private System.Windows.Forms.Panel panelMailingAddress;
        private System.Windows.Forms.Panel panelDocumentDetails;
        private System.Windows.Forms.Panel panelEditor;
        private ctrlEditor ctrlEditor;
        private System.Windows.Forms.Label lblUrlNote;
        private System.Windows.Forms.Label lblEmailAddressNote;
        private TitlePropertyGrid objectPropertyGrid;
    }
}
