using BTreeIndex.Collections.Generic.BTree;
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
using Utils.ColorWheel;
using WordAnalysis;

namespace ColorWheel
{
    public partial class frmTest : Form
    {
        private Color? primaryCalculatingColor;
        private BTreeDictionary<string, ConcreteRating> concreteRatingsIndex;
        private bool skipBackgroundAdjustment;

        public frmTest()
        {
            concreteRatingsIndex = ConcreteRatingsDictionary.Index;

            InitializeComponent();
        }

        private void cmdColorChooser_Click(object sender, EventArgs e)
        {
            var frmColorChooser = new frmColorChooser();
            var button = (Button)sender;
            ColorPicker colorPicker;
            string resourceName;
            string colorName;

            switch (button.Name)
            {
                case "cmdColorChooserPrimary":
                    colorPicker = colorPickerPrimary;
                    resourceName = "Primary Color";
                    break;
                case "cmdColorChooserSecondary":
                    colorPicker = colorPickerSecondary;
                    resourceName = "Secondary Color";
                    break;
                case "cmdColorChooserTertiary":
                    colorPicker = colorPickerTertiary;
                    resourceName = "Tertiary Color";
                    break;
                case "cmdColorChooserBackground":
                    colorPicker = colorPickerBackground;
                    resourceName = "Background Color";
                    break;
                default:
                    colorPicker = null;
                    resourceName = null;
                    DebugUtils.Break();
                    break;
            }

            frmColorChooser.InstanceRef = this.ParentForm;
            frmColorChooser.Color = colorPicker.SelectedColor;
            colorName = (string)colorPicker.SelectedItem;

            if (!colorName.IsNullOrEmpty())
            {
                frmColorChooser.Color = colorPicker.Colors[colorName];
            }

            if (frmColorChooser.ShowDialog() == DialogResult.OK)
            {
                colorPicker.SetColor(frmColorChooser.Color.Value, resourceName);

                switch (colorPicker.Name)
                {
                    case "colorPickerPrimary":
                        lblPrimary.BackColor = frmColorChooser.Color.Value;
                        break;
                    case "colorPickerSecondary":
                        lblSecondary.BackColor = frmColorChooser.Color.Value;
                        break;
                    case "colorPickerTertiary":
                        lblTertiary.BackColor = frmColorChooser.Color.Value;
                        break;
                    case "colorPickerBackground":
                        break;
                    default:
                        DebugUtils.Break();
                        break;
                }
            }
        }

        private void colorPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var colorPicker = (ColorPicker)sender;
            var selectedColor = colorPicker.SelectedColor;

            if (selectedColor == null)
            {
                return;
            }

            switch (colorPicker.Name)
            {
                case "colorPickerPrimary":
                    lblPrimary.BackColor = selectedColor.Value;

                    if (lblPrimary.AdjustForecolor() && !skipBackgroundAdjustment)
                    {
                        var color = selectedColor.Value.GetSplitComplements((short)NumberExtensions.GetRandomIntWithinRange(15, 30)).GetTupleValues().Cast<Color>().Randomize().First().Darkerize(NumberExtensions.GetRandomIntWithinRange(5, 15));

                        lblBackground.BackColor = color;
                    }
                    else
                    {
                        lblBackground.BackColor = Color.White;
                    }

                    lblBackground.AdjustForecolor();

                    break;
                case "colorPickerSecondary":
                    lblSecondary.BackColor = selectedColor.Value;
                    lblSecondary.AdjustForecolor();
                    break;
                case "colorPickerTertiary":
                    lblTertiary.BackColor = selectedColor.Value;
                    lblTertiary.AdjustForecolor();
                    break;
                case "colorPickerBackground":
                    lblBackground.BackColor = selectedColor.Value;
                    lblBackground.AdjustForecolor();
                    break;
                default:
                    DebugUtils.Break();
                    break;
            }
        }

        private Color Primary
        {
            get
            {
                if (primaryCalculatingColor.HasValue)
                {
                    return primaryCalculatingColor.Value;
                }
                else
                {
                    return colorPickerPrimary.SelectedColor.Value;
                }
            }
        }

        private void cmdGenerate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lblShade.BackColor = this.Primary.Shade();
            lblShade.AdjustForecolor();

            lblTint.BackColor = this.Primary.Tint();
            lblTint.AdjustForecolor();

            switch ((string)cboColorGeneration.SelectedItem)
            {
                case "Complements":
                    {
                        var colorComplement = this.Primary.GetComplement();
                        var tertiary = NumberExtensions.GetRandomIntWithinRange(0, 1) == 0 ? colorComplement.GetLeft((short)NumberExtensions.GetRandomIntWithinRange(15, 30)) : colorComplement.GetRight((short)NumberExtensions.GetRandomIntWithinRange(15, 30));

                        lblSecondary.BackColor = colorComplement;
                        lblTertiary.BackColor = tertiary;

                        colorPickerSecondary.SetColor(colorComplement, ColorTranslator.ToHtml(colorComplement));
                        colorPickerTertiary.SetColor(tertiary, ColorTranslator.ToHtml(tertiary));
                    }
                    break;
                case "Split Complements":
                    {
                        var splitComplements = this.Primary.GetSplitComplements((short)NumberExtensions.GetRandomIntWithinRange(15, 30));

                        lblSecondary.BackColor = splitComplements.Item1;
                        lblTertiary.BackColor = splitComplements.Item2;

                        colorPickerSecondary.SetColor(splitComplements.Item1, ColorTranslator.ToHtml(splitComplements.Item1));
                        colorPickerTertiary.SetColor(splitComplements.Item2, ColorTranslator.ToHtml(splitComplements.Item2));
                    }
                    break;
                case "Triads":
                    {
                        var triads = this.Primary.GetTriads();
                        var triadsRandomized = triads.GetTupleValues().Cast<Color>().Randomize().Take(2).ToArray();

                        lblSecondary.BackColor = triadsRandomized[0];
                        lblTertiary.BackColor = triadsRandomized[1];

                        colorPickerSecondary.SetColor(triadsRandomized[0], ColorTranslator.ToHtml(triadsRandomized[0]));
                        colorPickerTertiary.SetColor(triadsRandomized[1], ColorTranslator.ToHtml(triadsRandomized[1]));
                    }
                    break;
                case "Tetrads":
                    {
                        var tetrads = this.Primary.GetTetrads();
                        var tetradsRandomized = tetrads.GetTupleValues().Cast<Color>().Randomize().Take(2).ToArray();

                        lblSecondary.BackColor = tetradsRandomized[0];
                        lblTertiary.BackColor = tetradsRandomized[1];

                        colorPickerSecondary.SetColor(tetradsRandomized[0], ColorTranslator.ToHtml(tetradsRandomized[0]));
                        colorPickerTertiary.SetColor(tetradsRandomized[1], ColorTranslator.ToHtml(tetradsRandomized[1]));
                    }
                    break;
                case "Quintads":
                    {
                        var quintads = this.Primary.GetQuintads();
                        var quintadsRandomized = quintads.GetTupleValues().Cast<Color>().Randomize().Take(2).ToArray();

                        lblSecondary.BackColor = quintadsRandomized[0];
                        lblTertiary.BackColor = quintadsRandomized[1];

                        colorPickerSecondary.SetColor(quintadsRandomized[0], ColorTranslator.ToHtml(quintadsRandomized[0]));
                        colorPickerTertiary.SetColor(quintadsRandomized[1], ColorTranslator.ToHtml(quintadsRandomized[1]));
                    }
                    break;
                case "Analogous Left":
                    {
                        var analogousLeft = this.Primary.GetAnalogousLeft((short)NumberExtensions.GetRandomIntWithinRange(15, 30));
                        var analogousLeftRandomized = analogousLeft.GetTupleValues().Cast<Color>().Randomize().Take(2).ToArray();

                        lblSecondary.BackColor = analogousLeftRandomized[0];
                        lblTertiary.BackColor = analogousLeftRandomized[1];

                        colorPickerSecondary.SetColor(analogousLeftRandomized[0], ColorTranslator.ToHtml(analogousLeftRandomized[0]));
                        colorPickerTertiary.SetColor(analogousLeftRandomized[1], ColorTranslator.ToHtml(analogousLeftRandomized[1]));
                    }
                    break;
                case "Analogous Right":
                    {
                        var analogousRight = this.Primary.GetAnalogousRight((short)NumberExtensions.GetRandomIntWithinRange(15, 30));
                        var analogousRightRandomized = analogousRight.GetTupleValues().Cast<Color>().Randomize().Take(2).ToArray();

                        lblSecondary.BackColor = analogousRightRandomized[0];
                        lblTertiary.BackColor = analogousRightRandomized[1];

                        colorPickerSecondary.SetColor(analogousRightRandomized[0], ColorTranslator.ToHtml(analogousRightRandomized[0]));
                        colorPickerTertiary.SetColor(analogousRightRandomized[1], ColorTranslator.ToHtml(analogousRightRandomized[1]));
                    }
                    break;
                case "Monochromatics":
                    {
                        var gap = (double)(NumberExtensions.GetRandomIntWithinRange(10, 20) / 100f);
                        var monochromatics = this.Primary.GetMonochromatic(gap);
                        var monochromaticsRandomized = monochromatics.GetTupleValues().Cast<Color>().Randomize().Take(2).ToArray();

                        lblSecondary.BackColor = monochromaticsRandomized[0];
                        lblTertiary.BackColor = monochromaticsRandomized[1];

                        colorPickerSecondary.SetColor(monochromaticsRandomized[0], ColorTranslator.ToHtml(monochromaticsRandomized[0]));
                        colorPickerTertiary.SetColor(monochromaticsRandomized[1], ColorTranslator.ToHtml(monochromaticsRandomized[1]));
                    }
                    break;
                default:
                    {
                        DebugUtils.Break();
                    }
                    break;
            }
        }

        private void cmdGenerateColors_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            switch ((string)cboPersonality.SelectedItem)
            {
                case "Default":
                    {
                        var coolColors = Extensions.GetColorwheelColors().Select(c => c.AssureDark(.2f));
                        var colorPrimary = coolColors.Randomize().First();
                        var quintads = colorPrimary.GetSplitComplements().GetTupleValues().Cast<Color>().Randomize().Take(2).ToArray();
                        var lightGrayColors = Extensions.GetLightGrays();
                        var lightBackground = lightGrayColors.OrderByDescending(c => c.R).Take(3).Concat(new[] { Color.White }).Randomize().First();
                        Color colorSecondary;
                        Color colorTertiary;

                        colorPickerPrimary.SetColor(colorPrimary, ColorTranslator.ToHtml(colorPrimary));

                        lblShade.BackColor = colorPrimary.Shade();
                        lblShade.AdjustForecolor();

                        lblTint.BackColor = colorPrimary.Tint();
                        lblTint.AdjustForecolor();

                        colorSecondary = quintads[0];
                        colorTertiary = quintads[1];

                        colorPickerSecondary.SetColor(colorSecondary, ColorTranslator.ToHtml(colorSecondary));
                        colorPickerTertiary.SetColor(colorTertiary, ColorTranslator.ToHtml(colorTertiary));

                        colorPickerBackground.SetColor(lightBackground, ColorTranslator.ToHtml(lightBackground));
                    }
                    break;
                case "SeriousElegant":
                    {
                        var brightColors = colorPickerPrimary.Colors.GetBrightColors();
                        var colorsWithFrequencyUse = brightColors.Select(c => new KeyValuePair<KeyValuePair<string, Color>, float>(c, c.Key.SplitTitleCaseWords().Where(w => concreteRatingsIndex.ContainsKey(w)).Select(w => concreteRatingsIndex[w].SubtlexFrequencyCount).DefaultIfEmpty().Min())).ToList();
                        var rareColors = colorsWithFrequencyUse.Where(c => c.Value < 1000);
                        var colorPair = rareColors.Randomize().First().Key;
                        var colorPrimary = colorPair.Value.Royalize();
                        Color colorSecondary;
                        Color colorTertiary;
                        (Color, Color) analogous;
                        Color[] analogousRandomized;

                        if (colorPrimary.R > 125)
                        {
                            analogous = colorPrimary.GetAnalogousLeft((short)NumberExtensions.GetRandomIntWithinRange(15, 30));
                        }
                        else
                        {
                            analogous = colorPrimary.GetAnalogousRight((short)NumberExtensions.GetRandomIntWithinRange(15, 30));
                        }

                        analogousRandomized = analogous.GetTupleValues().Cast<Color>().Randomize().Take(2).ToArray();

                        colorPickerPrimary.SetColor(colorPrimary, ColorTranslator.ToHtml(colorPrimary));

                        lblShade.BackColor = colorPrimary.Shade();
                        lblShade.AdjustForecolor();

                        lblTint.BackColor = colorPrimary.Tint();
                        lblTint.AdjustForecolor();

                        colorSecondary = analogousRandomized[0];
                        colorTertiary = analogousRandomized[1];

                        colorPickerSecondary.SetColor(colorSecondary, ColorTranslator.ToHtml(colorSecondary));

                        if (colorTertiary.Compare(colorPrimary) > 3 || colorTertiary.Compare(colorSecondary) > 3)
                        {
                            var colorGray = Extensions.GetGrays().Randomize().First();
                            
                            colorPickerTertiary.SetColor(colorGray, ColorTranslator.ToHtml(colorGray));
                        }
                        else
                        {
                            colorPickerTertiary.SetColor(colorTertiary, ColorTranslator.ToHtml(colorTertiary));
                        }
                    }
                    break;
                case "MinimalistSimple":
                    {
                        var grayColors = Extensions.GetGrays();
                        var darkGrays = grayColors.OrderBy(c => c.R).Take(3);
                        var lightGrays = grayColors.Where(c => darkGrays.Any(c2 => c == c2)).Randomize().ToArray();
                        var colorPrimary = darkGrays.Randomize().First();
                        Color colorSecondary;
                        Color colorTertiary;

                        colorPickerPrimary.SetColor(colorPrimary, ColorTranslator.ToHtml(colorPrimary));

                        lblShade.BackColor = colorPrimary.Shade();
                        lblShade.AdjustForecolor();

                        lblTint.BackColor = colorPrimary.Tint();
                        lblTint.AdjustForecolor();

                        colorSecondary = lightGrays[0];
                        colorTertiary = lightGrays[1];

                        colorPickerSecondary.SetColor(colorSecondary, ColorTranslator.ToHtml(colorSecondary));
                        colorPickerTertiary.SetColor(colorTertiary, ColorTranslator.ToHtml(colorTertiary));
                    }
                    break;
                case "PlainNeutral":
                    {
                        var safeColors = Extensions.GetConservativeColors();
                        var colorPrimary = safeColors.Randomize().First();
                        var splitComplements = colorPrimary.GetSplitComplements((short)NumberExtensions.GetRandomIntWithinRange(15, 30));
                        Color colorSecondary;
                        Color colorTertiary;

                        colorPickerPrimary.SetColor(colorPrimary, ColorTranslator.ToHtml(colorPrimary));

                        lblShade.BackColor = colorPrimary.Shade();
                        lblShade.AdjustForecolor();

                        lblTint.BackColor = colorPrimary.Tint();
                        lblTint.AdjustForecolor();

                        colorSecondary = splitComplements.Item1;
                        colorTertiary = splitComplements.Item2;

                        colorPickerSecondary.SetColor(colorSecondary, ColorTranslator.ToHtml(colorSecondary));
                        colorPickerTertiary.SetColor(colorTertiary, ColorTranslator.ToHtml(colorTertiary));
                    }
                    break;
                case "BoldConfident":
                    {
                        var boldColors = Extensions.GetColorwheelColors().Select(c => c.Bolderize());
                        var colorPrimary = boldColors.Randomize().First();
                        var tetrads = colorPrimary.GetTetrads();
                        Color colorSecondary;
                        Color colorTertiary;

                        colorPickerPrimary.SetColor(colorPrimary, ColorTranslator.ToHtml(colorPrimary));

                        lblShade.BackColor = colorPrimary.Shade();
                        lblShade.AdjustForecolor();

                        lblTint.BackColor = colorPrimary.Tint();
                        lblTint.AdjustForecolor();

                        colorSecondary = tetrads.Item1;
                        colorTertiary = tetrads.Item2;

                        colorPickerSecondary.SetColor(colorSecondary, ColorTranslator.ToHtml(colorSecondary));
                        colorPickerTertiary.SetColor(colorTertiary, ColorTranslator.ToHtml(colorTertiary));
                    }
                    break;
                case "CalmPeaceful":
                    {
                        var pastelColors = Extensions.GetCalmColors();
                        var colorPrimary = pastelColors.Randomize().First();
                        var tetrads = colorPrimary.GetSplitComplements();
                        Color colorSecondary;
                        Color colorTertiary;

                        skipBackgroundAdjustment = true;

                        colorPickerPrimary.SetColor(colorPrimary, ColorTranslator.ToHtml(colorPrimary));

                        lblShade.BackColor = colorPrimary.Shade();
                        lblShade.AdjustForecolor();

                        lblTint.BackColor = colorPrimary.Tint();
                        lblTint.AdjustForecolor();

                        colorSecondary = tetrads.Item1;
                        colorTertiary = tetrads.Item2;

                        colorPickerSecondary.SetColor(colorSecondary, ColorTranslator.ToHtml(colorSecondary));
                        colorPickerTertiary.SetColor(colorTertiary, ColorTranslator.ToHtml(colorTertiary));

                        skipBackgroundAdjustment = false;
                    }
                    break;
                case "StartupUpbeat":
                    {
                        var coolColors = Extensions.GetCoolColors().Select(c => c.AssureDark(.2f));
                        var colorPrimary = coolColors.Randomize().First();
                        var quintads = colorPrimary.GetTriads().GetTupleValues().Cast<Color>().Randomize().Take(2).ToArray();
                        var lightGrayColors = Extensions.GetLightGrays();
                        var lightBackground = lightGrayColors.OrderByDescending(c => c.R).Take(3).Concat(new[] { Color.White }).Randomize().First();
                        Color colorSecondary;
                        Color colorTertiary;

                        colorPickerPrimary.SetColor(colorPrimary, ColorTranslator.ToHtml(colorPrimary));

                        lblShade.BackColor = colorPrimary.Shade();
                        lblShade.AdjustForecolor();

                        lblTint.BackColor = colorPrimary.Tint();
                        lblTint.AdjustForecolor();

                        colorSecondary = quintads[0];
                        colorTertiary = quintads[1];

                        colorPickerSecondary.SetColor(colorSecondary, ColorTranslator.ToHtml(colorSecondary));
                        colorPickerTertiary.SetColor(colorTertiary, ColorTranslator.ToHtml(colorTertiary));

                        colorPickerBackground.SetColor(lightBackground, ColorTranslator.ToHtml(lightBackground));
                    }
                    break;
                case "PlayfulFun":
                    {
                        var playfulColors = Extensions.GetColorwheelColors().Select(c => c.Neutralize());
                        var colorPrimary = playfulColors.Randomize().First();
                        var tetrads = colorPrimary.GetTetrads();
                        Color colorSecondary;
                        Color colorTertiary;

                        colorPickerPrimary.SetColor(colorPrimary, ColorTranslator.ToHtml(colorPrimary));

                        lblShade.BackColor = colorPrimary.Shade();
                        lblShade.AdjustForecolor();

                        lblTint.BackColor = colorPrimary.Tint();
                        lblTint.AdjustForecolor();

                        colorSecondary = tetrads.Item1;
                        colorTertiary = tetrads.Item2;

                        colorPickerSecondary.SetColor(colorSecondary, ColorTranslator.ToHtml(colorSecondary));
                        colorPickerTertiary.SetColor(colorTertiary, ColorTranslator.ToHtml(colorTertiary));
                    }
                    break;
                default:
                    {
                        DebugUtils.Break();
                    }
                    break;
            }
        }

        private void frmTest_Load(object sender, EventArgs e)
        {
            var colorWheelColors = Extensions.GetColorwheelColors().ToList();
            var pastelColors = Extensions.GetPastels().ToList();

            foreach (var color in colorWheelColors)
            {
                var colorPanel = new Panel { Height = panelColorWheelColors.Height, Width = panelColorWheelColors.Width / colorWheelColors.Count };

                colorPanel.BackColor = color;
                colorPanel.Margin = new Padding(0);
                colorPanel.Padding = new Padding(0);

                panelColorWheelColors.Controls.Add(colorPanel);
            }

            foreach (var color in pastelColors)
            {
                var colorPanel = new Panel { Height = panelPastels.Height, Width = panelPastels.Width / pastelColors.Count };

                colorPanel.BackColor = color;
                colorPanel.Margin = new Padding(0);
                colorPanel.Padding = new Padding(0);

                panelPastels.Controls.Add(colorPanel);
            }
        }

        private void cmdApplyFilterToPrimary_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var primaryColor = colorPickerPrimary.SelectedColor.Value;

            switch ((string)cboFilters.SelectedItem)
            {
                case "Royalize":
                    {
                        primaryColor = primaryColor.Royalize();
                        break;
                    }

                case "Pastelize":

                    primaryColor = primaryColor.Pastelize();
                    break;

                case "Darkerize":

                    primaryColor = primaryColor.Darkerize(NumberExtensions.GetRandomIntWithinRange(5, 15));
                    break;

                case "Bolderize":

                    primaryColor = primaryColor.Bolderize(NumberExtensions.GetRandomIntWithinRange(30, 50));
                    break;

                case "Neutralize":

                    primaryColor = primaryColor.Neutralize(NumberExtensions.GetRandomIntWithinRange(100, 120));
                    break;

                case "Warmerize":

                    primaryColor = primaryColor.Warmerize();
                    break;

                case "Coolerize":

                    primaryColor = primaryColor.Coolerize();
                    break;

                case "Feminize":

                    primaryColor = primaryColor.Feminize();
                    break;

                case "Masculinize":

                    primaryColor = primaryColor.Feminize();
                    break;


                default:
                    {
                        DebugUtils.Break();
                    }
                    break;
            }

            colorPickerPrimary.SetColor(primaryColor, ColorTranslator.ToHtml(primaryColor));
        }
    }
}
