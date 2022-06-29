// file:	ImageManagement.cs
//
// summary:	Implements the image management class

using AbstraX.ObjectProperties;
using AbstraX.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using Utils.Controls.ScreenCapture;

namespace AbstraX
{
    /// <summary>   An image management. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/4/2020. </remarks>

    public partial class ctrlImageManagement : ctrlResourceManagementBase
    {
        /// <summary>   Gets or sets the image. </summary>
        ///
        /// <value> The image. </value>

        public Image Image { get; set; }

        /// <summary>   Event queue for all listeners interested in ImageChanged events. </summary>
        public event EventHandler ImageChanged;
        private string imageFileName;
        private string imageId;
        private bool imageHasTag;
        private Size imagePreferredSize;
        private Dimension recommendedDimensions;
        private bool dragImageValidData;
        private string dragImagePath;
        private Image dragImage;
        private Thread dragImageThread;
        private bool pictureBoxPainted;
        private Rectangle possibleMouseDragStart;

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/4/2020. </remarks>

        public ctrlImageManagement()
        {
            this.ObjectProperties = new ObjectPropertiesDictionary();

            InitializeComponent();

            pictureBox.AllowDrop = true;
            pictureBox.GetAncestors().ToList().ForEach(c => c.AllowDrop = true);
        }

        /// <summary>   Gets or sets the id of the image. </summary>
        ///
        /// <value> The name of the image. </value>

        public string ImageId
        {
            get
            {
                return imageId;
            }

            set
            {
                imageId = value;
            }
        }

        /// <summary>   Gets or sets a value indicating whether the image has tag. </summary>
        ///
        /// <value> True if image has tag, false if not. </value>

        public bool ImageHasTag
        {
            get
            {
                return imageHasTag;
            }

            set
            {
                imageHasTag = value;
            }
        }

        /// <summary>   Gets or sets the size of the image preferred. </summary>
        ///
        /// <value> The size of the image preferred. </value>

        public Size ImagePreferredSize
        {
            get
            {
                return imagePreferredSize;
            }

            set
            {
                imagePreferredSize = value;
            }
        }

        /// <summary>   Gets or sets the filename of the image file. </summary>
        ///
        /// <value> The filename of the image file. </value>

        public string ImageFileName 
        { 
            get
            {
                return imageFileName;
            }

            set
            {
                imageFileName = value;
                SetImage();
            }
        }

        private void SetImage()
        {
            if (!imageFileName.IsNullOrEmpty())
            {
                if ((recommendedDimensions != null && recommendedDimensions.Format == "ico") || Path.GetExtension(ImageFileName) == ".ico")
                {
                    var icon = new Icon(this.ImageFileName);

                    using (var imageOriginal = icon.ToBitmap())
                    {
                        this.Image = imageOriginal.Clone(new Rectangle(0, 0, imageOriginal.Width, imageOriginal.Height), imageOriginal.PixelFormat);
                    }
                }
                else
                {
                    using (var imageOriginal = (Bitmap)Bitmap.FromFile(this.ImageFileName))
                    {
                        this.Image = imageOriginal.Clone(new Rectangle(0, 0, imageOriginal.Width, imageOriginal.Height), imageOriginal.PixelFormat);
                    }
                }

                pictureBox.Image = this.Image;

                ShowImageProperties();
            }
        }


        /// <summary>   Gets or sets the recommended dimensions. </summary>
        ///
        /// <value> The recommended dimensions. </value>

        public Dimension RecommendedDimensions 
        {
            get
            {
                return recommendedDimensions;
            }

            set
            {
                recommendedDimensions = value;

                if (recommendedDimensions != null)
                {
                    UpdateDimensions();
                }
            }
        }

        private void UpdateDimensions()
        {
            var width = this.RecommendedDimensions.WidthPixels;
            var height = this.RecommendedDimensions.HeightPixels;

            pictureBox.MaximumSize = new Size(width, height);

            if (this.recommendedDimensions.WidthPixels == -1)
            {
                lblRecommendation.Text = string.Format("Size: {0} x {1}, Aspect ratio:{2}", this.RecommendedDimensions.Width, this.RecommendedDimensions.Height, this.RecommendedDimensions.AspectRatio);
            }
            else
            {
                lblRecommendation.Text = string.Format("Size: {0} x {1}, Aspect ratio:{2}", this.RecommendedDimensions.WidthPixels.ToString(), this.RecommendedDimensions.Height, this.RecommendedDimensions.AspectRatio);
            }
        }

        /// <summary>   Creates a handle for the control. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/5/2020. </remarks>

        protected override void CreateHandle()
        {
            base.CreateHandle();

            SetImage();
            ShowImageProperties();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!pictureBoxPainted)
            {
                this.DelayInvoke(1000, () =>
                {
                    PaintPictureBox();
                    pictureBoxPainted = true;
                });
            }
        }

        private void ShowImageProperties()
        {
            ImageProperties properties;

            if (this.ObjectProperties == null)
            {
                this.ObjectProperties = new ObjectPropertiesDictionary();
            }

            if (!this.ObjectProperties.ContainsKey(this.ImageId))
            {
                properties = new ImageProperties(this.ImageId);

                this.ObjectProperties.Add(this.ImageId, properties);
            }
            else if (ObjectProperties[this.ImageId] is ImageProperties)
            {
                properties = ObjectProperties[this.ImageId];
            }
            else
            {
                var objectProperties = (ObjectPropertiesDictionary)this.ObjectProperties[this.ImageId];

                properties = new ImageProperties(this.ImageId);

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

                this.ObjectProperties[this.ImageId] = properties;
            }


            if (properties.Name == null)
            {
                objectPropertyGrid.Title = this.imageId;
            }
            else
            {
                objectPropertyGrid.Title = properties.Name;
            }

            objectPropertyGrid.SelectedObject = properties;
        }

        /// <summary>   Dispose image. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/12/2020. </remarks>

        public void DisposeImage()
        {
            if (this.Image != null)
            {
                this.Image.Dispose();
            }
        }

        /// <summary>   Event handler. Called by cmdSelectImage for click events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/4/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void cmdSelectImage_Click(object sender, EventArgs e)
        {
            string fileName;

            if (recommendedDimensions.Format == "ico")
            {
                openFileDialog.Filter = "Icon files|*.ico|All files|*.*";
            }
            else
            {
                openFileDialog.Filter = "Image files|*.jpg;*.bmp;*.gif;*.png;*.tif|All files|*.*";
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;

                if (recommendedDimensions.Format == "ico")
                {
                    var icon = Icon.ExtractAssociatedIcon(fileName);

                    this.Image = icon.ToBitmap();
                }
                else
                {
                    this.Image = Bitmap.FromFile(fileName);
                }

                pictureBox.Image = this.Image;
                txtImageName.Text = fileName;
                this.ImageFileName = fileName;

                ImageChanged(this, EventArgs.Empty);

                ShowImageProperties();
            }
        }

        private void txtImageName_TextChanged(object sender, EventArgs e)
        {
            var fileName = txtImageName.Text;

            if (File.Exists(fileName))
            {
                this.Image = Bitmap.FromFile(fileName);

                pictureBox.Image = this.Image;
                txtImageName.Text = fileName;
                this.ImageFileName = fileName;
            }
            else
            {
                this.Image = null;
                this.ImageFileName = null;
                pictureBox.Image = null;
            }

            ImageChanged(this, EventArgs.Empty);

            ShowImageProperties();
        }

        private void cmdCaptureImage_Click(object sender, EventArgs e)
        {
            var controlPanel = new ScreenCaptureControlPanel();
            var currentWorkingDirectory = Environment.CurrentDirectory;

            controlPanel.InstanceRef = this.ParentForm;

            if (controlPanel.ShowDialog() == DialogResult.OK)
            {
                var image = controlPanel.Image;

                this.Image = image;
                pictureBox.Image = image;
                this.ImageFileName = null;

                ImageChanged(this, EventArgs.Empty);

                ShowImageProperties();
            }
        }

        private void objectPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            base.RaiseObjectPropertiesChanged(s, e);
        }

        /// <summary>   Resets this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/23/2021. </remarks>

        public void Reset()
        {
            this.ObjectProperties = new ObjectPropertiesDictionary();

            imageId = string.Empty;
            imageHasTag = false;
            imageFileName = string.Empty;
            this.Image = null;
            objectPropertyGrid.SelectedObject = null;

            pictureBox.Image = null;
            txtImageName.Text = string.Empty;
        }

        private void pictureBox_DragEnter(object sender, DragEventArgs e)
        {
            string filename;

            dragImageValidData = GetFilename(out filename, e);

            if (dragImageValidData)
            {
                dragImagePath = filename;

                dragImageThread = new Thread(new ThreadStart(LoadImage));
                dragImageThread.Start();

                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private bool GetFilename(out string filename, DragEventArgs e)
        {
            bool ret = false;
            filename = String.Empty;

            if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                Array data = ((System.Windows.Forms.IDataObject)e.Data).GetData("FileDrop") as Array;

                if (data != null)
                {
                    if ((data.Length == 1) && (data.GetValue(0) is String))
                    {
                        filename = ((string[])data)[0];
                        string ext = Path.GetExtension(filename).ToLower();

                        if ((ext == ".jpg") || (ext == ".png") || (ext == ".bmp"))
                        {
                            ret = true;
                        }
                    }
                }
            }

            return ret;
        }

        private void pictureBox_DragOver(object sender, DragEventArgs e)
        {

        }

        private void pictureBox_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }

        private void pictureBox_DragDrop(object sender, DragEventArgs e)
        {
            if (dragImageValidData)
            {
                while (dragImageThread.IsAlive)
                {
                    Application.DoEvents();
                    Thread.Sleep(0);
                }

                pictureBox.Image = dragImage;
                txtImageName.Text = dragImagePath;
                this.ImageFileName = dragImagePath;

                ImageChanged(this, EventArgs.Empty);

                ShowImageProperties();
            }
        }

        private void LoadImage()
        {
            dragImage = new Bitmap(dragImagePath);
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            PaintPictureBox();
        }

        private void PaintPictureBox()
        {
            var rect = new Rectangle(0, 0, 200, 25);
            var point = pictureBox.ClientRectangle.GetCenteredRectPosition(rect.Size);

            rect.X = point.X;
            rect.Y = point.Y - 200;

            using (var graphics = pictureBox.CreateGraphics())
            {
                using (var font = new Font(this.Font, FontStyle.Italic))
                {
                    graphics.DrawString("Drag drop area", font, Brushes.Gray, rect, StringAlignment.Center);
                }
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            var xDrag = ControlExtensions.GetSystemMetrics(SystemMetric.SM_CXDRAG);
            var yDrag = ControlExtensions.GetSystemMetrics(SystemMetric.SM_CYDRAG);

            possibleMouseDragStart = new Rectangle(0, 0, xDrag, yDrag);

            possibleMouseDragStart.X = e.X - (int) (xDrag.As<float>() / 2f);
            possibleMouseDragStart.Y = e.Y - (int)(yDrag.As<float>() / 2f);
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (possibleMouseDragStart != null && !possibleMouseDragStart.IsEmpty)
                {
                    if (!possibleMouseDragStart.Contains(e.Location))
                    {
                        pictureBox.DoDragDrop(pictureBox.Image, DragDropEffects.All);
                    }
                }
            }
            else
            {
                possibleMouseDragStart = Rectangle.Empty;
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            possibleMouseDragStart = Rectangle.Empty;
        }
    }
}
