namespace AbstraX
{
    partial class ctrlMarketing
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
            this.label1 = new System.Windows.Forms.Label();
            this.checkedListBoxRateUs = new Utils.ctrlValueBoundCheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTellOthersLink = new System.Windows.Forms.TextBox();
            this.panelScroll = new System.Windows.Forms.Panel();
            this.objectPropertyGrid = new Utils.TitlePropertyGrid();
            this.linkLabelRemoveSelected = new System.Windows.Forms.LinkLabel();
            this.linkLabelAdd = new System.Windows.Forms.LinkLabel();
            this.lstOtherLinks = new System.Windows.Forms.ListBox();
            this.txtOtherLinks = new System.Windows.Forms.TextBox();
            this.txtConnectWithUsLink = new System.Windows.Forms.TextBox();
            this.txtAdvertisingLink = new System.Windows.Forms.TextBox();
            this.ctrlSocialMediaList = new AbstraX.MarketingControls.SocialMedia.ctrlSocialMediaList();
            this.ctrlSplitCheckboxTellOthersUrl = new Utils.ctrlSplitCheckbox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.timerUrlValidation = new System.Windows.Forms.Timer(this.components);
            this.timerSaveEntries = new System.Windows.Forms.Timer(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.txtEmailUsLink = new System.Windows.Forms.TextBox();
            this.panelScroll.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 193);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Allow Rate Us";
            // 
            // checkedListBoxRateUs
            // 
            this.checkedListBoxRateUs.FormattingEnabled = true;
            this.checkedListBoxRateUs.Location = new System.Drawing.Point(1, 211);
            this.checkedListBoxRateUs.Name = "checkedListBoxRateUs";
            this.checkedListBoxRateUs.Size = new System.Drawing.Size(331, 94);
            this.checkedListBoxRateUs.TabIndex = 1;
            this.checkedListBoxRateUs.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxRateUs_ItemCheck);
            this.checkedListBoxRateUs.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxRateUs_SelectedIndexChanged);
            this.checkedListBoxRateUs.Leave += new System.EventHandler(this.checkedListBoxRateUs_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 308);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Tell Others";
            // 
            // txtTellOthersLink
            // 
            this.txtTellOthersLink.BackColor = System.Drawing.SystemColors.Window;
            this.txtTellOthersLink.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtTellOthersLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTellOthersLink.ForeColor = System.Drawing.Color.Blue;
            this.txtTellOthersLink.Location = new System.Drawing.Point(1, 325);
            this.txtTellOthersLink.Name = "txtTellOthersLink";
            this.txtTellOthersLink.ReadOnly = true;
            this.txtTellOthersLink.Size = new System.Drawing.Size(331, 20);
            this.txtTellOthersLink.TabIndex = 2;
            this.txtTellOthersLink.Enter += new System.EventHandler(this.txtTellOthersLink_Enter);
            this.txtTellOthersLink.Leave += new System.EventHandler(this.txtTellOthersLink_Leave);
            // 
            // panelScroll
            // 
            this.panelScroll.AutoScroll = true;
            this.panelScroll.Controls.Add(this.objectPropertyGrid);
            this.panelScroll.Controls.Add(this.linkLabelRemoveSelected);
            this.panelScroll.Controls.Add(this.linkLabelAdd);
            this.panelScroll.Controls.Add(this.lstOtherLinks);
            this.panelScroll.Controls.Add(this.txtOtherLinks);
            this.panelScroll.Controls.Add(this.txtEmailUsLink);
            this.panelScroll.Controls.Add(this.txtConnectWithUsLink);
            this.panelScroll.Controls.Add(this.txtAdvertisingLink);
            this.panelScroll.Controls.Add(this.ctrlSocialMediaList);
            this.panelScroll.Controls.Add(this.ctrlSplitCheckboxTellOthersUrl);
            this.panelScroll.Controls.Add(this.label3);
            this.panelScroll.Controls.Add(this.label7);
            this.panelScroll.Controls.Add(this.label1);
            this.panelScroll.Controls.Add(this.label5);
            this.panelScroll.Controls.Add(this.txtTellOthersLink);
            this.panelScroll.Controls.Add(this.label6);
            this.panelScroll.Controls.Add(this.label4);
            this.panelScroll.Controls.Add(this.label2);
            this.panelScroll.Controls.Add(this.checkedListBoxRateUs);
            this.panelScroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelScroll.Location = new System.Drawing.Point(0, 0);
            this.panelScroll.Name = "panelScroll";
            this.panelScroll.Size = new System.Drawing.Size(1116, 557);
            this.panelScroll.TabIndex = 5;
            this.panelScroll.TabStop = true;
            // 
            // objectPropertyGrid
            // 
            this.objectPropertyGrid.Location = new System.Drawing.Point(769, 208);
            this.objectPropertyGrid.Name = "objectPropertyGrid";
            this.objectPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.CategorizedAlphabetical;
            this.objectPropertyGrid.SelectedObject = null;
            this.objectPropertyGrid.Size = new System.Drawing.Size(336, 324);
            this.objectPropertyGrid.TabIndex = 7;
            this.objectPropertyGrid.Title = "Link Properties";
            this.objectPropertyGrid.ToolbarVisible = true;
            this.objectPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.objectPropertyGrid_PropertyValueChanged);
            // 
            // linkLabelRemoveSelected
            // 
            this.linkLabelRemoveSelected.AutoSize = true;
            this.linkLabelRemoveSelected.Enabled = false;
            this.linkLabelRemoveSelected.Location = new System.Drawing.Point(617, 412);
            this.linkLabelRemoveSelected.Name = "linkLabelRemoveSelected";
            this.linkLabelRemoveSelected.Size = new System.Drawing.Size(92, 13);
            this.linkLabelRemoveSelected.TabIndex = 7;
            this.linkLabelRemoveSelected.TabStop = true;
            this.linkLabelRemoveSelected.Text = "Remove Selected";
            this.linkLabelRemoveSelected.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelRemoveSelected_LinkClicked);
            // 
            // linkLabelAdd
            // 
            this.linkLabelAdd.AutoSize = true;
            this.linkLabelAdd.Enabled = false;
            this.linkLabelAdd.Location = new System.Drawing.Point(716, 289);
            this.linkLabelAdd.Name = "linkLabelAdd";
            this.linkLabelAdd.Size = new System.Drawing.Size(26, 13);
            this.linkLabelAdd.TabIndex = 7;
            this.linkLabelAdd.TabStop = true;
            this.linkLabelAdd.Text = "Add";
            this.linkLabelAdd.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAdd_LinkClicked);
            // 
            // lstOtherLinks
            // 
            this.lstOtherLinks.FormattingEnabled = true;
            this.lstOtherLinks.Location = new System.Drawing.Point(463, 314);
            this.lstOtherLinks.Name = "lstOtherLinks";
            this.lstOtherLinks.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstOtherLinks.Size = new System.Drawing.Size(246, 95);
            this.lstOtherLinks.TabIndex = 6;
            this.lstOtherLinks.SelectedIndexChanged += new System.EventHandler(this.lstOtherLinks_SelectedIndexChanged);
            this.lstOtherLinks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstOtherLinks_KeyDown);
            this.lstOtherLinks.Leave += new System.EventHandler(this.lstOtherLinks_Leave);
            // 
            // txtOtherLinks
            // 
            this.txtOtherLinks.Location = new System.Drawing.Point(464, 286);
            this.txtOtherLinks.Name = "txtOtherLinks";
            this.txtOtherLinks.Size = new System.Drawing.Size(246, 20);
            this.txtOtherLinks.TabIndex = 5;
            this.txtOtherLinks.Enter += new System.EventHandler(this.txtOtherLinks_Enter);
            this.txtOtherLinks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtOtherLinks_KeyDown);
            this.txtOtherLinks.Leave += new System.EventHandler(this.txtOtherLinks_Leave);
            // 
            // txtConnectWithUsLink
            // 
            this.txtConnectWithUsLink.Location = new System.Drawing.Point(463, 234);
            this.txtConnectWithUsLink.Name = "txtConnectWithUsLink";
            this.txtConnectWithUsLink.Size = new System.Drawing.Size(246, 20);
            this.txtConnectWithUsLink.TabIndex = 4;
            this.txtConnectWithUsLink.TextChanged += new System.EventHandler(this.txtConnectWithUsLink_TextChanged);
            this.txtConnectWithUsLink.Enter += new System.EventHandler(this.txtConnectWithUsLink_Enter);
            this.txtConnectWithUsLink.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtConnectWithUsLink_KeyDown);
            this.txtConnectWithUsLink.Leave += new System.EventHandler(this.txtConnectWithUsLink_Leave);
            // 
            // txtAdvertisingLink
            // 
            this.txtAdvertisingLink.Location = new System.Drawing.Point(463, 208);
            this.txtAdvertisingLink.Name = "txtAdvertisingLink";
            this.txtAdvertisingLink.Size = new System.Drawing.Size(246, 20);
            this.txtAdvertisingLink.TabIndex = 3;
            this.txtAdvertisingLink.TextChanged += new System.EventHandler(this.txtAdvertisingLink_TextChanged);
            this.txtAdvertisingLink.Enter += new System.EventHandler(this.txtAdvertisingLink_Enter);
            this.txtAdvertisingLink.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAdvertisingLink_KeyDown);
            this.txtAdvertisingLink.Leave += new System.EventHandler(this.txtAdvertisingLink_Leave);
            // 
            // ctrlSocialMediaList
            // 
            this.ctrlSocialMediaList.Location = new System.Drawing.Point(0, 16);
            this.ctrlSocialMediaList.Name = "ctrlSocialMediaList";
            this.ctrlSocialMediaList.Size = new System.Drawing.Size(1105, 174);
            this.ctrlSocialMediaList.SocialMediaList = null;
            this.ctrlSocialMediaList.SocialMediaListOriginal = null;
            this.ctrlSocialMediaList.TabIndex = 0;
            this.ctrlSocialMediaList.UpdateEntry += new Utils.EventHandlerT<AbstraX.Resources.SocialMediaEntry>(this.ctrlSocialMediaList_UpdateEntry);
            // 
            // ctrlSplitCheckboxTellOthersUrl
            // 
            this.ctrlSplitCheckboxTellOthersUrl.Appearance = System.Windows.Forms.Appearance.Button;
            this.ctrlSplitCheckboxTellOthersUrl.DropDownButton = true;
            this.ctrlSplitCheckboxTellOthersUrl.Location = new System.Drawing.Point(331, 324);
            this.ctrlSplitCheckboxTellOthersUrl.Name = "ctrlSplitCheckboxTellOthersUrl";
            this.ctrlSplitCheckboxTellOthersUrl.Size = new System.Drawing.Size(15, 22);
            this.ctrlSplitCheckboxTellOthersUrl.TabIndex = 4;
            this.ctrlSplitCheckboxTellOthersUrl.UseVisualStyleBackColor = true;
            this.ctrlSplitCheckboxTellOthersUrl.CheckedChanged += new System.EventHandler(this.ctrlSplitCheckboxTellOthersUrl_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Social Media";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(341, 237);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Connect with Us Link";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(341, 291);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Other Links";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(341, 211);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Advertising Link";
            // 
            // timerUrlValidation
            // 
            this.timerUrlValidation.Interval = 2000;
            this.timerUrlValidation.Tick += new System.EventHandler(this.timerUrlValidation_Tick);
            // 
            // timerSaveEntries
            // 
            this.timerSaveEntries.Interval = 2000;
            this.timerSaveEntries.Tick += new System.EventHandler(this.timerSaveEntries_Tick);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(341, 261);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Email Us Address";
            // 
            // txtEmailUsLink
            // 
            this.txtEmailUsLink.Location = new System.Drawing.Point(463, 260);
            this.txtEmailUsLink.Name = "txtEmailUsLink";
            this.txtEmailUsLink.Size = new System.Drawing.Size(246, 20);
            this.txtEmailUsLink.TabIndex = 4;
            this.txtEmailUsLink.TextChanged += new System.EventHandler(this.txtEmailUsLink_TextChanged);
            this.txtEmailUsLink.Enter += new System.EventHandler(this.txtEmailUsLink_Enter);
            this.txtEmailUsLink.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtEmailUsLink_KeyDown);
            this.txtEmailUsLink.Leave += new System.EventHandler(this.txtEmailUsLink_Leave);
            // 
            // ctrlMarketing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelScroll);
            this.Name = "ctrlMarketing";
            this.Size = new System.Drawing.Size(1116, 557);
            this.Tag = "";
            this.panelScroll.ResumeLayout(false);
            this.panelScroll.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private MarketingControls.SocialMedia.ctrlSocialMediaList ctrlSocialMediaList;
        private Utils.ctrlValueBoundCheckedListBox checkedListBoxRateUs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTellOthersLink;
        private Utils.ctrlSplitCheckbox ctrlSplitCheckboxTellOthersUrl;
        private System.Windows.Forms.Panel panelScroll;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkLabelAdd;
        private System.Windows.Forms.ListBox lstOtherLinks;
        private System.Windows.Forms.TextBox txtOtherLinks;
        private System.Windows.Forms.TextBox txtConnectWithUsLink;
        private System.Windows.Forms.TextBox txtAdvertisingLink;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private Utils.TitlePropertyGrid objectPropertyGrid;
        private System.Windows.Forms.Timer timerUrlValidation;
        private System.Windows.Forms.Timer timerSaveEntries;
        private System.Windows.Forms.LinkLabel linkLabelRemoveSelected;
        private System.Windows.Forms.TextBox txtEmailUsLink;
        private System.Windows.Forms.Label label7;
    }
}
