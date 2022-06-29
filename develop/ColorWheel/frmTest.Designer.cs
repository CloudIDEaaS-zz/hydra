
namespace ColorWheel
{
    partial class frmTest
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTest));
            this.cmdColorChooserBackground = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.cmdColorChooserTertiary = new System.Windows.Forms.Button();
            this.cmdColorChooserSecondary = new System.Windows.Forms.Button();
            this.cmdColorChooserPrimary = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblBackground = new System.Windows.Forms.Label();
            this.lblTertiary = new System.Windows.Forms.Label();
            this.lblSecondary = new System.Windows.Forms.Label();
            this.lblTint = new System.Windows.Forms.Label();
            this.lblShade = new System.Windows.Forms.Label();
            this.lblPrimary = new System.Windows.Forms.Label();
            this.cboColorGeneration = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdGenerate = new System.Windows.Forms.LinkLabel();
            this.cboPersonality = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdGenerateColors = new System.Windows.Forms.LinkLabel();
            this.panelColorWheelColors = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panelPastels = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdApplyFilterToPrimary = new System.Windows.Forms.LinkLabel();
            this.cboFilters = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.colorPickerPrimary = new Utils.ColorPicker();
            this.colorPickerBackground = new Utils.ColorPicker();
            this.colorPickerTertiary = new Utils.ColorPicker();
            this.colorPickerSecondary = new Utils.ColorPicker();
            this.tableLayoutPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdColorChooserBackground
            // 
            this.cmdColorChooserBackground.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cmdColorChooserBackground.BackgroundImage")));
            this.cmdColorChooserBackground.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.cmdColorChooserBackground.Location = new System.Drawing.Point(262, 212);
            this.cmdColorChooserBackground.Name = "cmdColorChooserBackground";
            this.cmdColorChooserBackground.Size = new System.Drawing.Size(21, 21);
            this.cmdColorChooserBackground.TabIndex = 32;
            this.cmdColorChooserBackground.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cmdColorChooserBackground.UseVisualStyleBackColor = true;
            this.cmdColorChooserBackground.Click += new System.EventHandler(this.cmdColorChooser_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 215);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 13);
            this.label8.TabIndex = 31;
            this.label8.Text = "Background Color";
            // 
            // cmdColorChooserTertiary
            // 
            this.cmdColorChooserTertiary.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cmdColorChooserTertiary.BackgroundImage")));
            this.cmdColorChooserTertiary.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.cmdColorChooserTertiary.Location = new System.Drawing.Point(262, 186);
            this.cmdColorChooserTertiary.Name = "cmdColorChooserTertiary";
            this.cmdColorChooserTertiary.Size = new System.Drawing.Size(21, 21);
            this.cmdColorChooserTertiary.TabIndex = 29;
            this.cmdColorChooserTertiary.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cmdColorChooserTertiary.UseVisualStyleBackColor = true;
            this.cmdColorChooserTertiary.Click += new System.EventHandler(this.cmdColorChooser_Click);
            // 
            // cmdColorChooserSecondary
            // 
            this.cmdColorChooserSecondary.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cmdColorChooserSecondary.BackgroundImage")));
            this.cmdColorChooserSecondary.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.cmdColorChooserSecondary.Location = new System.Drawing.Point(262, 160);
            this.cmdColorChooserSecondary.Name = "cmdColorChooserSecondary";
            this.cmdColorChooserSecondary.Size = new System.Drawing.Size(21, 21);
            this.cmdColorChooserSecondary.TabIndex = 28;
            this.cmdColorChooserSecondary.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cmdColorChooserSecondary.UseVisualStyleBackColor = true;
            this.cmdColorChooserSecondary.Click += new System.EventHandler(this.cmdColorChooser_Click);
            // 
            // cmdColorChooserPrimary
            // 
            this.cmdColorChooserPrimary.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cmdColorChooserPrimary.BackgroundImage")));
            this.cmdColorChooserPrimary.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.cmdColorChooserPrimary.Location = new System.Drawing.Point(254, 30);
            this.cmdColorChooserPrimary.Name = "cmdColorChooserPrimary";
            this.cmdColorChooserPrimary.Size = new System.Drawing.Size(21, 21);
            this.cmdColorChooserPrimary.TabIndex = 27;
            this.cmdColorChooserPrimary.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cmdColorChooserPrimary.UseVisualStyleBackColor = true;
            this.cmdColorChooserPrimary.Click += new System.EventHandler(this.cmdColorChooser_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 190);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Tertiary Color";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 164);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 25;
            this.label6.Text = "Secondary Color";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Color";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel.ColumnCount = 6;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel.Controls.Add(this.lblBackground, 5, 0);
            this.tableLayoutPanel.Controls.Add(this.lblTertiary, 4, 0);
            this.tableLayoutPanel.Controls.Add(this.lblSecondary, 3, 0);
            this.tableLayoutPanel.Controls.Add(this.lblTint, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.lblShade, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.lblPrimary, 0, 0);
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 338);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(931, 100);
            this.tableLayoutPanel.TabIndex = 33;
            // 
            // lblBackground
            // 
            this.lblBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBackground.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBackground.Location = new System.Drawing.Point(774, 1);
            this.lblBackground.Name = "lblBackground";
            this.lblBackground.Size = new System.Drawing.Size(153, 98);
            this.lblBackground.TabIndex = 5;
            this.lblBackground.Text = "Background";
            this.lblBackground.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTertiary
            // 
            this.lblTertiary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTertiary.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTertiary.Location = new System.Drawing.Point(620, 1);
            this.lblTertiary.Name = "lblTertiary";
            this.lblTertiary.Size = new System.Drawing.Size(147, 98);
            this.lblTertiary.TabIndex = 4;
            this.lblTertiary.Text = "Tertiary";
            this.lblTertiary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSecondary
            // 
            this.lblSecondary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSecondary.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSecondary.Location = new System.Drawing.Point(466, 1);
            this.lblSecondary.Name = "lblSecondary";
            this.lblSecondary.Size = new System.Drawing.Size(147, 98);
            this.lblSecondary.TabIndex = 3;
            this.lblSecondary.Text = "Secondary";
            this.lblSecondary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTint
            // 
            this.lblTint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTint.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTint.Location = new System.Drawing.Point(312, 1);
            this.lblTint.Name = "lblTint";
            this.lblTint.Size = new System.Drawing.Size(147, 98);
            this.lblTint.TabIndex = 2;
            this.lblTint.Text = "Tint";
            this.lblTint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblShade
            // 
            this.lblShade.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblShade.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShade.Location = new System.Drawing.Point(158, 1);
            this.lblShade.Name = "lblShade";
            this.lblShade.Size = new System.Drawing.Size(147, 98);
            this.lblShade.TabIndex = 1;
            this.lblShade.Text = "Shade";
            this.lblShade.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPrimary
            // 
            this.lblPrimary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPrimary.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrimary.Location = new System.Drawing.Point(4, 1);
            this.lblPrimary.Name = "lblPrimary";
            this.lblPrimary.Size = new System.Drawing.Size(147, 98);
            this.lblPrimary.TabIndex = 0;
            this.lblPrimary.Text = "Primary";
            this.lblPrimary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cboColorGeneration
            // 
            this.cboColorGeneration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboColorGeneration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboColorGeneration.FormattingEnabled = true;
            this.cboColorGeneration.Items.AddRange(new object[] {
            "Complements",
            "Split Complements",
            "Triads",
            "Tetrads",
            "Quintads",
            "Analogous Left",
            "Analogous Right",
            "Monochromatics"});
            this.cboColorGeneration.Location = new System.Drawing.Point(629, 71);
            this.cboColorGeneration.Name = "cboColorGeneration";
            this.cboColorGeneration.Size = new System.Drawing.Size(144, 21);
            this.cboColorGeneration.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(484, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Remaining Color Generation";
            // 
            // cmdGenerate
            // 
            this.cmdGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdGenerate.AutoSize = true;
            this.cmdGenerate.Location = new System.Drawing.Point(779, 75);
            this.cmdGenerate.Name = "cmdGenerate";
            this.cmdGenerate.Size = new System.Drawing.Size(51, 13);
            this.cmdGenerate.TabIndex = 34;
            this.cmdGenerate.TabStop = true;
            this.cmdGenerate.Text = "Generate";
            this.cmdGenerate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.cmdGenerate_LinkClicked);
            // 
            // cboPersonality
            // 
            this.cboPersonality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPersonality.FormattingEnabled = true;
            this.cboPersonality.Items.AddRange(new object[] {
            "Default",
            "SeriousElegant",
            "MinimalistSimple",
            "PlainNeutral",
            "BoldConfident",
            "CalmPeaceful",
            "StartupUpbeat",
            "PlayfulFun"});
            this.cboPersonality.Location = new System.Drawing.Point(110, 6);
            this.cboPersonality.Name = "cboPersonality";
            this.cboPersonality.Size = new System.Drawing.Size(147, 21);
            this.cboPersonality.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "Personality";
            // 
            // cmdGenerateColors
            // 
            this.cmdGenerateColors.AutoSize = true;
            this.cmdGenerateColors.Location = new System.Drawing.Point(268, 9);
            this.cmdGenerateColors.Name = "cmdGenerateColors";
            this.cmdGenerateColors.Size = new System.Drawing.Size(83, 13);
            this.cmdGenerateColors.TabIndex = 34;
            this.cmdGenerateColors.TabStop = true;
            this.cmdGenerateColors.Text = "Generate Colors";
            this.cmdGenerateColors.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.cmdGenerateColors_LinkClicked);
            // 
            // panelColorWheelColors
            // 
            this.panelColorWheelColors.Location = new System.Drawing.Point(12, 467);
            this.panelColorWheelColors.Name = "panelColorWheelColors";
            this.panelColorWheelColors.Size = new System.Drawing.Size(931, 36);
            this.panelColorWheelColors.TabIndex = 35;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 451);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(166, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Color Wheel Colors (warm to cool)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 515);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Pastels";
            // 
            // panelPastels
            // 
            this.panelPastels.Location = new System.Drawing.Point(12, 531);
            this.panelPastels.Name = "panelPastels";
            this.panelPastels.Size = new System.Drawing.Size(931, 36);
            this.panelPastels.TabIndex = 35;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmdApplyFilterToPrimary);
            this.groupBox1.Controls.Add(this.cmdGenerate);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.colorPickerPrimary);
            this.groupBox1.Controls.Add(this.cboFilters);
            this.groupBox1.Controls.Add(this.cmdColorChooserPrimary);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.cboColorGeneration);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(9, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(839, 121);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Primary Color";
            // 
            // cmdApplyFilterToPrimary
            // 
            this.cmdApplyFilterToPrimary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdApplyFilterToPrimary.AutoSize = true;
            this.cmdApplyFilterToPrimary.Location = new System.Drawing.Point(259, 65);
            this.cmdApplyFilterToPrimary.Name = "cmdApplyFilterToPrimary";
            this.cmdApplyFilterToPrimary.Size = new System.Drawing.Size(82, 13);
            this.cmdApplyFilterToPrimary.TabIndex = 34;
            this.cmdApplyFilterToPrimary.TabStop = true;
            this.cmdApplyFilterToPrimary.Text = "Apply to Primary";
            this.cmdApplyFilterToPrimary.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.cmdApplyFilterToPrimary_LinkClicked);
            // 
            // cboFilters
            // 
            this.cboFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFilters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilters.FormattingEnabled = true;
            this.cboFilters.Items.AddRange(new object[] {
            "Royalize",
            "Pastelize",
            "Darkerize",
            "Warmerize",
            "Coolerize",
            "Bolderize",
            "Neutralize",
            "Feminize",
            "Masculinize"});
            this.cboFilters.Location = new System.Drawing.Point(101, 62);
            this.cboFilters.Name = "cboFilters";
            this.cboFilters.Size = new System.Drawing.Size(147, 21);
            this.cboFilters.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 65);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(58, 13);
            this.label9.TabIndex = 31;
            this.label9.Text = "Apply Filter";
            // 
            // colorPickerPrimary
            // 
            this.colorPickerPrimary.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.colorPickerPrimary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorPickerPrimary.FormattingEnabled = true;
            this.colorPickerPrimary.Items.AddRange(new object[] {
            "",
            "Blue",
            "Green",
            "Orange",
            "Purple",
            "Red",
            "AliceBlue",
            "AntiqueWhite",
            "Aqua",
            "Aquamarine",
            "Azure",
            "Beige",
            "Bisque",
            "Black",
            "BlanchedAlmond",
            "BlueViolet",
            "Brown",
            "BurlyWood",
            "CadetBlue",
            "Chartreuse",
            "Chocolate",
            "Coral",
            "CornflowerBlue",
            "Cornsilk",
            "Crimson",
            "Cyan",
            "DarkBlue",
            "DarkCyan",
            "DarkGoldenrod",
            "DarkGray",
            "DarkGreen",
            "DarkKhaki",
            "DarkMagenta",
            "DarkOliveGreen",
            "DarkOrange",
            "DarkOrchid",
            "DarkRed",
            "DarkSalmon",
            "DarkSeaGreen",
            "DarkSlateBlue",
            "DarkSlateGray",
            "DarkTurquoise",
            "DarkViolet",
            "DeepPink",
            "DeepSkyBlue",
            "DimGray",
            "DodgerBlue",
            "Firebrick",
            "FloralWhite",
            "ForestGreen",
            "Fuchsia",
            "Gainsboro",
            "GhostWhite",
            "Gold",
            "Goldenrod",
            "Gray",
            "GreenYellow",
            "Honeydew",
            "HotPink",
            "IndianRed",
            "Indigo",
            "Ivory",
            "Khaki",
            "Lavender",
            "LavenderBlush",
            "LawnGreen",
            "LemonChiffon",
            "LightBlue",
            "LightCoral",
            "LightCyan",
            "LightGoldenrodYellow",
            "LightGray",
            "LightGreen",
            "LightPink",
            "LightSalmon",
            "LightSeaGreen",
            "LightSkyBlue",
            "LightSlateGray",
            "LightSteelBlue",
            "LightYellow",
            "Lime",
            "LimeGreen",
            "Linen",
            "Magenta",
            "Maroon",
            "MediumAquamarine",
            "MediumBlue",
            "MediumOrchid",
            "MediumPurple",
            "MediumSeaGreen",
            "MediumSlateBlue",
            "MediumSpringGreen",
            "MediumTurquoise",
            "MediumVioletRed",
            "MidnightBlue",
            "MintCream",
            "MistyRose",
            "Moccasin",
            "NavajoWhite",
            "Navy",
            "OldLace",
            "Olive",
            "OliveDrab",
            "OrangeRed",
            "Orchid",
            "PaleGoldenrod",
            "PaleGreen",
            "PaleTurquoise",
            "PaleVioletRed",
            "PapayaWhip",
            "PeachPuff",
            "Peru",
            "Pink",
            "Plum",
            "PowderBlue",
            "RosyBrown",
            "RoyalBlue",
            "SaddleBrown",
            "Salmon",
            "SandyBrown",
            "SeaGreen",
            "SeaShell",
            "Sienna",
            "Silver",
            "SkyBlue",
            "SlateBlue",
            "SlateGray",
            "Snow",
            "SpringGreen",
            "SteelBlue",
            "Tan",
            "Teal",
            "Thistle",
            "Tomato",
            "Transparent",
            "Turquoise",
            "Violet",
            "Wheat",
            "White",
            "WhiteSmoke",
            "Yellow",
            "YellowGreen"});
            this.colorPickerPrimary.Location = new System.Drawing.Point(101, 30);
            this.colorPickerPrimary.Name = "colorPickerPrimary";
            this.colorPickerPrimary.Size = new System.Drawing.Size(147, 21);
            this.colorPickerPrimary.TabIndex = 21;
            this.colorPickerPrimary.SelectedIndexChanged += new System.EventHandler(this.colorPicker_SelectedIndexChanged);
            // 
            // colorPickerBackground
            // 
            this.colorPickerBackground.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.colorPickerBackground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorPickerBackground.FormattingEnabled = true;
            this.colorPickerBackground.Items.AddRange(new object[] {
            "",
            "Blue",
            "Green",
            "Orange",
            "Purple",
            "Red",
            "AliceBlue",
            "AntiqueWhite",
            "Aqua",
            "Aquamarine",
            "Azure",
            "Beige",
            "Bisque",
            "Black",
            "BlanchedAlmond",
            "BlueViolet",
            "Brown",
            "BurlyWood",
            "CadetBlue",
            "Chartreuse",
            "Chocolate",
            "Coral",
            "CornflowerBlue",
            "Cornsilk",
            "Crimson",
            "Cyan",
            "DarkBlue",
            "DarkCyan",
            "DarkGoldenrod",
            "DarkGray",
            "DarkGreen",
            "DarkKhaki",
            "DarkMagenta",
            "DarkOliveGreen",
            "DarkOrange",
            "DarkOrchid",
            "DarkRed",
            "DarkSalmon",
            "DarkSeaGreen",
            "DarkSlateBlue",
            "DarkSlateGray",
            "DarkTurquoise",
            "DarkViolet",
            "DeepPink",
            "DeepSkyBlue",
            "DimGray",
            "DodgerBlue",
            "Firebrick",
            "FloralWhite",
            "ForestGreen",
            "Fuchsia",
            "Gainsboro",
            "GhostWhite",
            "Gold",
            "Goldenrod",
            "Gray",
            "GreenYellow",
            "Honeydew",
            "HotPink",
            "IndianRed",
            "Indigo",
            "Ivory",
            "Khaki",
            "Lavender",
            "LavenderBlush",
            "LawnGreen",
            "LemonChiffon",
            "LightBlue",
            "LightCoral",
            "LightCyan",
            "LightGoldenrodYellow",
            "LightGray",
            "LightGreen",
            "LightPink",
            "LightSalmon",
            "LightSeaGreen",
            "LightSkyBlue",
            "LightSlateGray",
            "LightSteelBlue",
            "LightYellow",
            "Lime",
            "LimeGreen",
            "Linen",
            "Magenta",
            "Maroon",
            "MediumAquamarine",
            "MediumBlue",
            "MediumOrchid",
            "MediumPurple",
            "MediumSeaGreen",
            "MediumSlateBlue",
            "MediumSpringGreen",
            "MediumTurquoise",
            "MediumVioletRed",
            "MidnightBlue",
            "MintCream",
            "MistyRose",
            "Moccasin",
            "NavajoWhite",
            "Navy",
            "OldLace",
            "Olive",
            "OliveDrab",
            "OrangeRed",
            "Orchid",
            "PaleGoldenrod",
            "PaleGreen",
            "PaleTurquoise",
            "PaleVioletRed",
            "PapayaWhip",
            "PeachPuff",
            "Peru",
            "Pink",
            "Plum",
            "PowderBlue",
            "RosyBrown",
            "RoyalBlue",
            "SaddleBrown",
            "Salmon",
            "SandyBrown",
            "SeaGreen",
            "SeaShell",
            "Sienna",
            "Silver",
            "SkyBlue",
            "SlateBlue",
            "SlateGray",
            "Snow",
            "SpringGreen",
            "SteelBlue",
            "Tan",
            "Teal",
            "Thistle",
            "Tomato",
            "Transparent",
            "Turquoise",
            "Violet",
            "Wheat",
            "White",
            "WhiteSmoke",
            "Yellow",
            "YellowGreen"});
            this.colorPickerBackground.Location = new System.Drawing.Point(109, 212);
            this.colorPickerBackground.Name = "colorPickerBackground";
            this.colorPickerBackground.Size = new System.Drawing.Size(147, 21);
            this.colorPickerBackground.TabIndex = 30;
            this.colorPickerBackground.SelectedIndexChanged += new System.EventHandler(this.colorPicker_SelectedIndexChanged);
            // 
            // colorPickerTertiary
            // 
            this.colorPickerTertiary.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.colorPickerTertiary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorPickerTertiary.FormattingEnabled = true;
            this.colorPickerTertiary.Items.AddRange(new object[] {
            "",
            "Blue",
            "Green",
            "Orange",
            "Purple",
            "Red",
            "AliceBlue",
            "AntiqueWhite",
            "Aqua",
            "Aquamarine",
            "Azure",
            "Beige",
            "Bisque",
            "Black",
            "BlanchedAlmond",
            "BlueViolet",
            "Brown",
            "BurlyWood",
            "CadetBlue",
            "Chartreuse",
            "Chocolate",
            "Coral",
            "CornflowerBlue",
            "Cornsilk",
            "Crimson",
            "Cyan",
            "DarkBlue",
            "DarkCyan",
            "DarkGoldenrod",
            "DarkGray",
            "DarkGreen",
            "DarkKhaki",
            "DarkMagenta",
            "DarkOliveGreen",
            "DarkOrange",
            "DarkOrchid",
            "DarkRed",
            "DarkSalmon",
            "DarkSeaGreen",
            "DarkSlateBlue",
            "DarkSlateGray",
            "DarkTurquoise",
            "DarkViolet",
            "DeepPink",
            "DeepSkyBlue",
            "DimGray",
            "DodgerBlue",
            "Firebrick",
            "FloralWhite",
            "ForestGreen",
            "Fuchsia",
            "Gainsboro",
            "GhostWhite",
            "Gold",
            "Goldenrod",
            "Gray",
            "GreenYellow",
            "Honeydew",
            "HotPink",
            "IndianRed",
            "Indigo",
            "Ivory",
            "Khaki",
            "Lavender",
            "LavenderBlush",
            "LawnGreen",
            "LemonChiffon",
            "LightBlue",
            "LightCoral",
            "LightCyan",
            "LightGoldenrodYellow",
            "LightGray",
            "LightGreen",
            "LightPink",
            "LightSalmon",
            "LightSeaGreen",
            "LightSkyBlue",
            "LightSlateGray",
            "LightSteelBlue",
            "LightYellow",
            "Lime",
            "LimeGreen",
            "Linen",
            "Magenta",
            "Maroon",
            "MediumAquamarine",
            "MediumBlue",
            "MediumOrchid",
            "MediumPurple",
            "MediumSeaGreen",
            "MediumSlateBlue",
            "MediumSpringGreen",
            "MediumTurquoise",
            "MediumVioletRed",
            "MidnightBlue",
            "MintCream",
            "MistyRose",
            "Moccasin",
            "NavajoWhite",
            "Navy",
            "OldLace",
            "Olive",
            "OliveDrab",
            "OrangeRed",
            "Orchid",
            "PaleGoldenrod",
            "PaleGreen",
            "PaleTurquoise",
            "PaleVioletRed",
            "PapayaWhip",
            "PeachPuff",
            "Peru",
            "Pink",
            "Plum",
            "PowderBlue",
            "RosyBrown",
            "RoyalBlue",
            "SaddleBrown",
            "Salmon",
            "SandyBrown",
            "SeaGreen",
            "SeaShell",
            "Sienna",
            "Silver",
            "SkyBlue",
            "SlateBlue",
            "SlateGray",
            "Snow",
            "SpringGreen",
            "SteelBlue",
            "Tan",
            "Teal",
            "Thistle",
            "Tomato",
            "Transparent",
            "Turquoise",
            "Violet",
            "Wheat",
            "White",
            "WhiteSmoke",
            "Yellow",
            "YellowGreen"});
            this.colorPickerTertiary.Location = new System.Drawing.Point(109, 187);
            this.colorPickerTertiary.Name = "colorPickerTertiary";
            this.colorPickerTertiary.Size = new System.Drawing.Size(147, 21);
            this.colorPickerTertiary.TabIndex = 23;
            this.colorPickerTertiary.SelectedIndexChanged += new System.EventHandler(this.colorPicker_SelectedIndexChanged);
            // 
            // colorPickerSecondary
            // 
            this.colorPickerSecondary.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.colorPickerSecondary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colorPickerSecondary.FormattingEnabled = true;
            this.colorPickerSecondary.Items.AddRange(new object[] {
            "",
            "Blue",
            "Green",
            "Orange",
            "Purple",
            "Red",
            "AliceBlue",
            "AntiqueWhite",
            "Aqua",
            "Aquamarine",
            "Azure",
            "Beige",
            "Bisque",
            "Black",
            "BlanchedAlmond",
            "BlueViolet",
            "Brown",
            "BurlyWood",
            "CadetBlue",
            "Chartreuse",
            "Chocolate",
            "Coral",
            "CornflowerBlue",
            "Cornsilk",
            "Crimson",
            "Cyan",
            "DarkBlue",
            "DarkCyan",
            "DarkGoldenrod",
            "DarkGray",
            "DarkGreen",
            "DarkKhaki",
            "DarkMagenta",
            "DarkOliveGreen",
            "DarkOrange",
            "DarkOrchid",
            "DarkRed",
            "DarkSalmon",
            "DarkSeaGreen",
            "DarkSlateBlue",
            "DarkSlateGray",
            "DarkTurquoise",
            "DarkViolet",
            "DeepPink",
            "DeepSkyBlue",
            "DimGray",
            "DodgerBlue",
            "Firebrick",
            "FloralWhite",
            "ForestGreen",
            "Fuchsia",
            "Gainsboro",
            "GhostWhite",
            "Gold",
            "Goldenrod",
            "Gray",
            "GreenYellow",
            "Honeydew",
            "HotPink",
            "IndianRed",
            "Indigo",
            "Ivory",
            "Khaki",
            "Lavender",
            "LavenderBlush",
            "LawnGreen",
            "LemonChiffon",
            "LightBlue",
            "LightCoral",
            "LightCyan",
            "LightGoldenrodYellow",
            "LightGray",
            "LightGreen",
            "LightPink",
            "LightSalmon",
            "LightSeaGreen",
            "LightSkyBlue",
            "LightSlateGray",
            "LightSteelBlue",
            "LightYellow",
            "Lime",
            "LimeGreen",
            "Linen",
            "Magenta",
            "Maroon",
            "MediumAquamarine",
            "MediumBlue",
            "MediumOrchid",
            "MediumPurple",
            "MediumSeaGreen",
            "MediumSlateBlue",
            "MediumSpringGreen",
            "MediumTurquoise",
            "MediumVioletRed",
            "MidnightBlue",
            "MintCream",
            "MistyRose",
            "Moccasin",
            "NavajoWhite",
            "Navy",
            "OldLace",
            "Olive",
            "OliveDrab",
            "OrangeRed",
            "Orchid",
            "PaleGoldenrod",
            "PaleGreen",
            "PaleTurquoise",
            "PaleVioletRed",
            "PapayaWhip",
            "PeachPuff",
            "Peru",
            "Pink",
            "Plum",
            "PowderBlue",
            "RosyBrown",
            "RoyalBlue",
            "SaddleBrown",
            "Salmon",
            "SandyBrown",
            "SeaGreen",
            "SeaShell",
            "Sienna",
            "Silver",
            "SkyBlue",
            "SlateBlue",
            "SlateGray",
            "Snow",
            "SpringGreen",
            "SteelBlue",
            "Tan",
            "Teal",
            "Thistle",
            "Tomato",
            "Transparent",
            "Turquoise",
            "Violet",
            "Wheat",
            "White",
            "WhiteSmoke",
            "Yellow",
            "YellowGreen"});
            this.colorPickerSecondary.Location = new System.Drawing.Point(109, 161);
            this.colorPickerSecondary.Name = "colorPickerSecondary";
            this.colorPickerSecondary.Size = new System.Drawing.Size(147, 21);
            this.colorPickerSecondary.TabIndex = 22;
            this.colorPickerSecondary.SelectedIndexChanged += new System.EventHandler(this.colorPicker_SelectedIndexChanged);
            // 
            // frmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(956, 667);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panelPastels);
            this.Controls.Add(this.panelColorWheelColors);
            this.Controls.Add(this.cmdGenerateColors);
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.cmdColorChooserBackground);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.colorPickerBackground);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cboPersonality);
            this.Controls.Add(this.cmdColorChooserTertiary);
            this.Controls.Add(this.cmdColorChooserSecondary);
            this.Controls.Add(this.colorPickerTertiary);
            this.Controls.Add(this.colorPickerSecondary);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTest";
            this.Text = "Test Color Wheel";
            this.Load += new System.EventHandler(this.frmTest_Load);
            this.tableLayoutPanel.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdColorChooserBackground;
        private Utils.ColorPicker colorPickerBackground;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button cmdColorChooserTertiary;
        private System.Windows.Forms.Button cmdColorChooserSecondary;
        private System.Windows.Forms.Button cmdColorChooserPrimary;
        private Utils.ColorPicker colorPickerTertiary;
        private Utils.ColorPicker colorPickerSecondary;
        private Utils.ColorPicker colorPickerPrimary;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label lblTertiary;
        private System.Windows.Forms.Label lblSecondary;
        private System.Windows.Forms.Label lblTint;
        private System.Windows.Forms.Label lblShade;
        private System.Windows.Forms.Label lblPrimary;
        private System.Windows.Forms.ComboBox cboColorGeneration;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel cmdGenerate;
        private System.Windows.Forms.ComboBox cboPersonality;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel cmdGenerateColors;
        private System.Windows.Forms.FlowLayoutPanel panelColorWheelColors;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.FlowLayoutPanel panelPastels;
        private System.Windows.Forms.Label lblBackground;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel cmdApplyFilterToPrimary;
        private System.Windows.Forms.ComboBox cboFilters;
        private System.Windows.Forms.Label label9;
    }
}

