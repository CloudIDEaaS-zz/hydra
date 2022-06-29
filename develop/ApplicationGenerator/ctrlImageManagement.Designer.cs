
namespace AbstraX
{
    partial class ctrlImageManagement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctrlImageManagement));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.txtImageName = new System.Windows.Forms.TextBox();
            this.cmdSelectImage = new System.Windows.Forms.Button();
            this.cmdCaptureImage = new System.Windows.Forms.Button();
            this.toolTipCaptureImage = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panelScroll = new System.Windows.Forms.Panel();
            this.editorPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.objectPropertyGrid = new Utils.TitlePropertyGrid();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblRecommendation = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panelScroll.SuspendLayout();
            this.editorPanel.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtImageName
            // 
            this.txtImageName.Location = new System.Drawing.Point(3, 4);
            this.txtImageName.Name = "txtImageName";
            this.txtImageName.Size = new System.Drawing.Size(574, 20);
            this.txtImageName.TabIndex = 0;
            this.txtImageName.TextChanged += new System.EventHandler(this.txtImageName_TextChanged);
            // 
            // cmdSelectImage
            // 
            this.cmdSelectImage.Location = new System.Drawing.Point(583, 2);
            this.cmdSelectImage.Name = "cmdSelectImage";
            this.cmdSelectImage.Size = new System.Drawing.Size(106, 23);
            this.cmdSelectImage.TabIndex = 1;
            this.cmdSelectImage.Text = "Select Image...";
            this.cmdSelectImage.UseVisualStyleBackColor = true;
            this.cmdSelectImage.Click += new System.EventHandler(this.cmdSelectImage_Click);
            // 
            // cmdCaptureImage
            // 
            this.cmdCaptureImage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cmdCaptureImage.BackgroundImage")));
            this.cmdCaptureImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.cmdCaptureImage.Location = new System.Drawing.Point(695, 3);
            this.cmdCaptureImage.Name = "cmdCaptureImage";
            this.cmdCaptureImage.Size = new System.Drawing.Size(21, 21);
            this.cmdCaptureImage.TabIndex = 16;
            this.toolTipCaptureImage.SetToolTip(this.cmdCaptureImage, "Capture image from screen");
            this.cmdCaptureImage.UseVisualStyleBackColor = true;
            this.cmdCaptureImage.Click += new System.EventHandler(this.cmdCaptureImage_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.White;
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point(3, 3);
            this.pictureBox.MinimumSize = new System.Drawing.Size(32, 32);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(713, 448);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            this.pictureBox.WaitOnLoad = true;
            this.pictureBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBox_DragDrop);
            this.pictureBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBox_DragEnter);
            this.pictureBox.DragOver += new System.Windows.Forms.DragEventHandler(this.pictureBox_DragOver);
            this.pictureBox.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.pictureBox_GiveFeedback);
            this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            // 
            // panelScroll
            // 
            this.panelScroll.AutoScroll = true;
            this.panelScroll.Controls.Add(this.editorPanel);
            this.panelScroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelScroll.Location = new System.Drawing.Point(0, 32);
            this.panelScroll.Name = "panelScroll";
            this.panelScroll.Size = new System.Drawing.Size(1064, 483);
            this.panelScroll.TabIndex = 3;
            // 
            // editorPanel
            // 
            this.editorPanel.Controls.Add(this.pictureBox);
            this.editorPanel.Controls.Add(this.objectPropertyGrid);
            this.editorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorPanel.Location = new System.Drawing.Point(0, 0);
            this.editorPanel.Name = "editorPanel";
            this.editorPanel.Size = new System.Drawing.Size(1064, 483);
            this.editorPanel.TabIndex = 8;
            // 
            // objectPropertyGrid
            // 
            this.objectPropertyGrid.Location = new System.Drawing.Point(722, 3);
            this.objectPropertyGrid.Name = "objectPropertyGrid";
            this.objectPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.CategorizedAlphabetical;
            this.objectPropertyGrid.SelectedObject = null;
            this.objectPropertyGrid.SelectedObjects = new object[0];
            this.objectPropertyGrid.Size = new System.Drawing.Size(336, 473);
            this.objectPropertyGrid.TabIndex = 8;
            this.objectPropertyGrid.Title = "Image Properties";
            this.objectPropertyGrid.ToolbarVisible = true;
            this.objectPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.objectPropertyGrid_PropertyValueChanged);
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.panel1);
            this.panelTop.Controls.Add(this.cmdCaptureImage);
            this.panelTop.Controls.Add(this.txtImageName);
            this.panelTop.Controls.Add(this.cmdSelectImage);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1064, 32);
            this.panelTop.TabIndex = 17;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.lblRecommendation);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(722, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(339, 32);
            this.panel1.TabIndex = 17;
            // 
            // lblRecommendation
            // 
            this.lblRecommendation.AutoSize = true;
            this.lblRecommendation.Location = new System.Drawing.Point(119, 4);
            this.lblRecommendation.Name = "lblRecommendation";
            this.lblRecommendation.Size = new System.Drawing.Size(0, 13);
            this.lblRecommendation.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Recommended:";
            // 
            // ctrlImageManagement
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.panelScroll);
            this.Controls.Add(this.panelTop);
            this.Name = "ctrlImageManagement";
            this.Size = new System.Drawing.Size(1064, 515);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panelScroll.ResumeLayout(false);
            this.editorPanel.ResumeLayout(false);
            this.editorPanel.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox txtImageName;
        private System.Windows.Forms.Button cmdSelectImage;
        private System.Windows.Forms.Button cmdCaptureImage;
        private System.Windows.Forms.ToolTip toolTipCaptureImage;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Panel panelScroll;
        private System.Windows.Forms.FlowLayoutPanel editorPanel;
        private Utils.TitlePropertyGrid objectPropertyGrid;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblRecommendation;
        private System.Windows.Forms.Label label1;
    }
}
