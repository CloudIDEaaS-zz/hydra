using CognitiveServices;
using CognitiveServices.Services;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace AnalyzePhotoTestHarness
{
    public partial class frmAnalyzer : Form
    {
        private ImageAnalyzer imageAnalyzer;

        public frmAnalyzer()
        {
            imageAnalyzer = new ImageAnalyzer();

            InitializeComponent();
        }

        private void cmdFetchImage_Click(object sender, EventArgs e)
        {
            Image image;
            string url;
            GeneratePhotoResponse generatePhotoResponse;
            ImageAnalysis imageAnalysis;
            List<string> tagNames;

            statusStrip.SetStatus("Fetching photo");

            generatePhotoResponse = GeneratePhotoService.FetchRandomPhoto();
            image = generatePhotoResponse.Image;
            url = generatePhotoResponse.Url;

            pictureBox.Image = image;
            this.DoEvents();

            statusStrip.SetStatus("Analyzing photo");

            imageAnalysis = imageAnalyzer.AnalyzeImage(image).Result;

            tagNames = imageAnalysis.Tags.Select(t => t.Name).ToList();
        }

        private void frmAnalyzer_Load(object sender, EventArgs e)
        {
            this.ShowInSecondaryMonitor();
        }
    }
}
