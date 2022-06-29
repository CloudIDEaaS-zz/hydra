using Leadtools;
using Leadtools.Codecs;
using Leadtools.ImageProcessing.Color;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace BingWebSearchTest
{
    public static class Extensions
    {
        public static void Save(this RasterImage rasterImage, string fileName, RasterImageFormat rasterImageFormat)
        {
            using (var codecs = new RasterCodecs())
            {
                using (var stream = new MemoryStream())
                {
                    codecs.Save(rasterImage, fileName, rasterImageFormat, 24);
                }
            }
        }
        public static string GetExtension(this RasterImageFormat rasterImageFormat)
        {
            switch (rasterImageFormat)
            {
                case RasterImageFormat.Jpeg:
                    return ".jpg";
                case RasterImageFormat.Png:
                    return ".png";
                case RasterImageFormat.Gif:
                    return ".gif";
                case RasterImageFormat.Bmp:
                    return ".bmp";
                case RasterImageFormat.Tif:
                    return ".tif";
                case RasterImageFormat.WinIco:
                    return ".ico";
                case RasterImageFormat.Emf:
                    return ".emf";
                case RasterImageFormat.Wmf:
                    return ".wmf";
                case RasterImageFormat.Exif:
                    return ".exif";
            }

            return ".unknown";
        }

        public static void ChangeImage(this PictureBox pictureBox, RasterImage image, RasterImageFormat rasterImageFormat)
        {
            var tempPath = Path.GetTempPath();
            var extension = rasterImageFormat.GetExtension();
            var imageFile = Path.Combine(tempPath, string.Format("TempImage{0}{1}", Guid.NewGuid().ToString(), extension));
            var form = pictureBox.GetParentForm();

            if (form == null)
            {
                form = Application.OpenForms.Cast<Form>().FirstOrDefault();
            }

            form.FormClosing += (sender, e) =>
            {
                try
                {
                    pictureBox.Image.Dispose();
                }
                catch
                {
                }

                try
                {
                    File.Delete(imageFile);
                }
                catch
                {
                }
            };

            image.Save(imageFile, rasterImageFormat);

            pictureBox.Load(imageFile);
        }

        public static RasterImageChangedFlags ChangeColorIntensityBalance(this RasterImage rasterImage, int r, int g, int b)
        {
            var command = new ColorIntensityBalanceCommand();
            var data = new ColorIntensityBalanceCommandData[1];
            RasterImageChangedFlags flags;

            data[0] = new ColorIntensityBalanceCommandData();
            data[0].Red = r;
            data[0].Green = g;
            data[0].Blue = b;

            command.HighLight = data[0];

            flags = command.Run(rasterImage);

            return flags;
        }

        public static RasterColor ToRasterColor(this Color color)
        {
            return new RasterColor(color.R, color.G, color.B);
        }

        public static Color FromRasterColor(this RasterColor rasterColor)
        {
            return Color.FromArgb(rasterColor.R, rasterColor.G, rasterColor.B);
        }


        public static Color IntensityDetect(this RasterImage rasterImage)
        {
            var command = new IntensityDetectCommand();
            RasterImageChangedFlags flags;

            //Apply the filter. 
            // 
            command.LowThreshold = 0;
            command.HighThreshold = 255;
            command.InColor = new RasterColor(255, 255, 255);
            command.OutColor = new RasterColor(0, 0, 0);
            command.Channel = IntensityDetectCommandFlags.Master;

            flags = command.Run(rasterImage);

            return command.OutColor.FromRasterColor();
        }

        public static RasterImageChangedFlags ChangeSampleTarget(this RasterImage rasterImage, Color sampleColor, Color targetColor)
        {
            var command = new SampleTargetCommand(sampleColor.ToRasterColor(), targetColor.ToRasterColor(), SampleTargetCommandFlags.Rgb | SampleTargetCommandFlags.High);
            RasterImageChangedFlags flags;

            flags = command.Run(rasterImage);

            return flags;
        }

        public static RasterImageChangedFlags ReplaceColor(this RasterImage rasterImage, Color fromColor, Color toColor)
        {
            var command = new ColorReplaceCommand();
            var data = new ColorReplaceCommandColor[1];
            var hslColor = (HSLColor) toColor;
            RasterImageChangedFlags flags;

            data[0] = new ColorReplaceCommandColor();
            data[0].Color = fromColor.ToRasterColor();
            data[0].Fuzziness = 100;

            command.Colors = data;
            command.Hue = 9000;
            command.Saturation = 0;
            command.Brightness = 0;

            flags = command.Run(rasterImage);

            return flags;
        }

        public static RasterImageChangedFlags ChangeChannelMixerRedFactor(this RasterImage rasterImage, int red)
        {
            var command = new ChannelMixerCommand();
            var data = new ChannelMixerCommandFactor[1];
            RasterImageChangedFlags flags;

            data[0] = new ChannelMixerCommandFactor();
            data[0].Red = red;

            command.RedFactor = data[0];

            flags = command.Run(rasterImage);

            return flags;
        }

        public static RasterImageChangedFlags ChangeChannelMixerGreenFactor(this RasterImage rasterImage, int green)
        {
            var command = new ChannelMixerCommand();
            var data = new ChannelMixerCommandFactor[1];
            RasterImageChangedFlags flags;

            data[0] = new ChannelMixerCommandFactor();
            data[0].Green = green;

            command.GreenFactor = data[0];

            flags = command.Run(rasterImage);

            return flags;
        }

        public static RasterImageChangedFlags ChangeChannelMixerBlueFactor(this RasterImage rasterImage, int blue)
        {
            var command = new ChannelMixerCommand();
            var data = new ChannelMixerCommandFactor[1];
            RasterImageChangedFlags flags;

            data[0] = new ChannelMixerCommandFactor();
            data[0].Blue = blue;

            command.BlueFactor = data[0];

            flags = command.Run(rasterImage);

            return flags;
        }

        public static RasterImage ToRasterImage(this Bitmap image)
        {
            using (var codecs = new RasterCodecs())
            {
                using (var stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Bmp);
                    stream.Rewind();

                    return codecs.Load(stream);
                }
            }
        }

        public static Bitmap FromRasterImage(this RasterImage rasterImage)
        {
            using (var codecs = new RasterCodecs())
            {
                using (var stream = new MemoryStream())
                {
                    codecs.Save(rasterImage, stream, RasterImageFormat.Bmp, 24);
                    stream.Rewind();

                    return (Bitmap)Bitmap.FromStream(stream);
                }
            }
        }
    }
}
