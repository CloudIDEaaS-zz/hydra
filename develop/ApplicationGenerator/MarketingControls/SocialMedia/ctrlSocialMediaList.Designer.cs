
using System.Collections.Generic;

namespace AbstraX.MarketingControls.SocialMedia
{
    partial class ctrlSocialMediaList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctrlSocialMediaList));
            this.socialMediaGridView = new System.Windows.Forms.DataGridView();
            this.smallLogoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.enableDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.accountUrlDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.shareUrlDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.visitCallToActionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.shareCallToActionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.siteUrlDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewLinkColumn();
            this.socialMediaListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ctrlSplitCheckboxAccountUrl = new Utils.ctrlSplitCheckbox();
            this.ctrlSplitCheckboxShareUrl = new Utils.ctrlSplitCheckbox();
            ((System.ComponentModel.ISupportInitialize)(this.socialMediaGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.socialMediaListBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // socialMediaGridView
            // 
            this.socialMediaGridView.AllowUserToAddRows = false;
            this.socialMediaGridView.AllowUserToDeleteRows = false;
            this.socialMediaGridView.AutoGenerateColumns = false;
            this.socialMediaGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.socialMediaGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.socialMediaGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.socialMediaGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.smallLogoDataGridViewTextBoxColumn,
            this.enableDataGridViewCheckBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.accountUrlDataGridViewTextBoxColumn,
            this.shareUrlDataGridViewTextBoxColumn,
            this.visitCallToActionDataGridViewTextBoxColumn,
            this.shareCallToActionDataGridViewTextBoxColumn,
            this.siteUrlDataGridViewTextBoxColumn});
            this.socialMediaGridView.DataSource = this.socialMediaListBindingSource;
            this.socialMediaGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.socialMediaGridView.Location = new System.Drawing.Point(0, 0);
            this.socialMediaGridView.Name = "socialMediaGridView";
            this.socialMediaGridView.RowHeadersVisible = false;
            this.socialMediaGridView.RowTemplate.Height = 28;
            this.socialMediaGridView.Size = new System.Drawing.Size(1106, 413);
            this.socialMediaGridView.TabIndex = 0;
            this.socialMediaGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.socialMediaGridView_CellContentClick);
            this.socialMediaGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.socialMediaGridView_CellEndEdit);
            this.socialMediaGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.socialMediaGridView_CellFormatting);
            this.socialMediaGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.socialMediaGridView_CellValueChanged);
            this.socialMediaGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.socialMediaGridView_RowEnter);
            this.socialMediaGridView.Click += new System.EventHandler(this.socialMediaGridView_Click);
            this.socialMediaGridView.Leave += new System.EventHandler(this.socialMediaGridView_Leave);
            // 
            // smallLogoDataGridViewTextBoxColumn
            // 
            this.smallLogoDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.smallLogoDataGridViewTextBoxColumn.DataPropertyName = "SmallLogoImage";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle1.NullValue")));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
            this.smallLogoDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.smallLogoDataGridViewTextBoxColumn.HeaderText = "";
            this.smallLogoDataGridViewTextBoxColumn.Name = "smallLogoDataGridViewTextBoxColumn";
            this.smallLogoDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.smallLogoDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.smallLogoDataGridViewTextBoxColumn.Width = 25;
            // 
            // enableDataGridViewCheckBoxColumn
            // 
            this.enableDataGridViewCheckBoxColumn.DataPropertyName = "Enable";
            this.enableDataGridViewCheckBoxColumn.HeaderText = "";
            this.enableDataGridViewCheckBoxColumn.Name = "enableDataGridViewCheckBoxColumn";
            this.enableDataGridViewCheckBoxColumn.Width = 25;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // accountUrlDataGridViewTextBoxColumn
            // 
            this.accountUrlDataGridViewTextBoxColumn.DataPropertyName = "AccountUrl";
            this.accountUrlDataGridViewTextBoxColumn.HeaderText = "AccountUrl";
            this.accountUrlDataGridViewTextBoxColumn.Name = "accountUrlDataGridViewTextBoxColumn";
            this.accountUrlDataGridViewTextBoxColumn.ReadOnly = true;
            this.accountUrlDataGridViewTextBoxColumn.Width = 200;
            // 
            // shareUrlDataGridViewTextBoxColumn
            // 
            this.shareUrlDataGridViewTextBoxColumn.DataPropertyName = "ShareUrl";
            this.shareUrlDataGridViewTextBoxColumn.HeaderText = "ShareUrl";
            this.shareUrlDataGridViewTextBoxColumn.Name = "shareUrlDataGridViewTextBoxColumn";
            this.shareUrlDataGridViewTextBoxColumn.ReadOnly = true;
            this.shareUrlDataGridViewTextBoxColumn.Width = 250;
            // 
            // visitCallToActionDataGridViewTextBoxColumn
            // 
            this.visitCallToActionDataGridViewTextBoxColumn.DataPropertyName = "VisitCallToAction";
            this.visitCallToActionDataGridViewTextBoxColumn.HeaderText = "Visit Call To Action";
            this.visitCallToActionDataGridViewTextBoxColumn.Name = "visitCallToActionDataGridViewTextBoxColumn";
            this.visitCallToActionDataGridViewTextBoxColumn.Width = 150;
            // 
            // shareCallToActionDataGridViewTextBoxColumn
            // 
            this.shareCallToActionDataGridViewTextBoxColumn.DataPropertyName = "ShareCallToAction";
            this.shareCallToActionDataGridViewTextBoxColumn.HeaderText = "Share Call To Action";
            this.shareCallToActionDataGridViewTextBoxColumn.Name = "shareCallToActionDataGridViewTextBoxColumn";
            this.shareCallToActionDataGridViewTextBoxColumn.Width = 150;
            // 
            // siteUrlDataGridViewTextBoxColumn
            // 
            this.siteUrlDataGridViewTextBoxColumn.DataPropertyName = "SiteUrl";
            this.siteUrlDataGridViewTextBoxColumn.HeaderText = "SiteUrl";
            this.siteUrlDataGridViewTextBoxColumn.Name = "siteUrlDataGridViewTextBoxColumn";
            this.siteUrlDataGridViewTextBoxColumn.ReadOnly = true;
            this.siteUrlDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.siteUrlDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.siteUrlDataGridViewTextBoxColumn.Width = 200;
            // 
            // socialMediaListBindingSource
            // 
            this.socialMediaListBindingSource.DataSource = typeof(System.Collections.Generic.List<AbstraX.MarketingControls.SocialMedia.SocialMediaEntryViewable>);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Name";
            this.dataGridViewTextBoxColumn1.HeaderText = "Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "AccountUrl";
            this.dataGridViewTextBoxColumn2.HeaderText = "AccountUrl";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 200;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "ShareUrl";
            this.dataGridViewTextBoxColumn3.HeaderText = "ShareUrl";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 250;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "VisitCallToAction";
            this.dataGridViewTextBoxColumn4.HeaderText = "Visit Call To Action";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 150;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "ShareCallToAction";
            this.dataGridViewTextBoxColumn5.HeaderText = "Share Call To Action";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Width = 150;
            // 
            // ctrlSplitCheckboxAccountUrl
            // 
            this.ctrlSplitCheckboxAccountUrl.Appearance = System.Windows.Forms.Appearance.Button;
            this.ctrlSplitCheckboxAccountUrl.DropDownButton = true;
            this.ctrlSplitCheckboxAccountUrl.Location = new System.Drawing.Point(182, 251);
            this.ctrlSplitCheckboxAccountUrl.Name = "ctrlSplitCheckboxAccountUrl";
            this.ctrlSplitCheckboxAccountUrl.Size = new System.Drawing.Size(15, 24);
            this.ctrlSplitCheckboxAccountUrl.TabIndex = 1;
            this.ctrlSplitCheckboxAccountUrl.UseVisualStyleBackColor = true;
            this.ctrlSplitCheckboxAccountUrl.Visible = false;
            this.ctrlSplitCheckboxAccountUrl.CheckedChanged += new System.EventHandler(this.ctrlSplitCheckboxAccountUrl_CheckedChanged);
            // 
            // ctrlSplitCheckboxShareUrl
            // 
            this.ctrlSplitCheckboxShareUrl.Appearance = System.Windows.Forms.Appearance.Button;
            this.ctrlSplitCheckboxShareUrl.DropDownButton = true;
            this.ctrlSplitCheckboxShareUrl.Location = new System.Drawing.Point(203, 251);
            this.ctrlSplitCheckboxShareUrl.Name = "ctrlSplitCheckboxShareUrl";
            this.ctrlSplitCheckboxShareUrl.Size = new System.Drawing.Size(15, 24);
            this.ctrlSplitCheckboxShareUrl.TabIndex = 1;
            this.ctrlSplitCheckboxShareUrl.UseVisualStyleBackColor = true;
            this.ctrlSplitCheckboxShareUrl.Visible = false;
            this.ctrlSplitCheckboxShareUrl.CheckedChanged += new System.EventHandler(this.ctrlSplitCheckboxShareUrl_CheckedChanged);
            // 
            // ctrlSocialMediaList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ctrlSplitCheckboxShareUrl);
            this.Controls.Add(this.ctrlSplitCheckboxAccountUrl);
            this.Controls.Add(this.socialMediaGridView);
            this.Name = "ctrlSocialMediaList";
            this.Size = new System.Drawing.Size(1106, 413);
            ((System.ComponentModel.ISupportInitialize)(this.socialMediaGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.socialMediaListBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView socialMediaGridView;
        private System.Windows.Forms.BindingSource socialMediaListBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private Utils.ctrlSplitCheckbox ctrlSplitCheckboxAccountUrl;
        private Utils.ctrlSplitCheckbox ctrlSplitCheckboxShareUrl;
        private System.Windows.Forms.DataGridViewImageColumn smallLogoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enableDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn accountUrlDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn shareUrlDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn visitCallToActionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn shareCallToActionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewLinkColumn siteUrlDataGridViewTextBoxColumn;
    }
}
