using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace AbstraX.ScreenProcessors
{
	/// <summary>   A welcome processor. </summary>
	///
	/// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>

	public class MenuProcessor : IPreviousScreenProcessor
	{
		/// <summary>   Gets or sets information describing the resource. </summary>
		///
		/// <value> Information describing the resource. </value>

		public IResourceData ResourceData { get; set; }

		/// <summary>   Gets the named blobs. </summary>
		///
		/// <value> The named blobs. </value>

		public List<NamedBlob> NamedBlobs { get; }

		/// <summary>   Gets or sets a collection of images. </summary>
		///
		/// <value> A collection of images. </value>

		public Dictionary<string, Bitmap> ImageCollection { get; set; }

		/// <summary>   Gets or sets a collection of fonts. </summary>
		///
		/// <value> A collection of fonts. </value>

		public PrivateFontCollection FontCollection { get; set; }

		/// <summary>   Default constructor. </summary>
		///
		/// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>

		public MenuProcessor()
		{
			this.NamedBlobs = new List<NamedBlob>();
		}

		/// <summary>   Check specified blob and decide if should be kept or not. </summary>
		///
		/// <remarks>   CloudIDEaaS, 11/28/2020. </remarks>
		///
		/// <param name="blob"> Blob to check. </param>
		///
		/// <returns>
		/// Return <see langword="true" /> if the blob should be kept or
		/// <see langword="false" /> if it should be removed.
		/// </returns>

		public bool Check(Blob blob)
		{
			var rect = blob.Rectangle;
			var x = rect.X;
			var y = rect.Y;
			NamedBlob namedBlob = null;

			switch (x)
			{
				case 0:

					switch (y)
					{
						case 55:
							namedBlob = new NamedBlob("SelectedMenu", blob);
							break;
						default:
							DebugUtils.Break();
							break;
					}

					break;

				case 13:

					switch (y)
					{
						case 93:
							namedBlob = new NamedBlob("UpperMenuIcons", blob);
							break;
						case 189:
							namedBlob = new NamedBlob("MiddleDivider", blob);
							break;
						case 240:
							namedBlob = new NamedBlob("LowerMenuIcons", blob);
							break;
						case 335:
							namedBlob = new NamedBlob("LowerDivider", blob);
							break;
						default:
							DebugUtils.Break();
							break;
					}

					break;

				case 44:

					switch (y)
					{
						case 93:
							namedBlob = new NamedBlob("UpperMenuItems", blob);
							break;
						case 239:
							namedBlob = new NamedBlob("LowerMenuItems", blob);
							break;
						default:
							DebugUtils.Break();
							break;
					}

					break;

				case 11:

					switch (y)
					{
						case 35:
							namedBlob = new NamedBlob("UpperMenuHeader", blob);
							break;
						case 213:
							namedBlob = new NamedBlob("LowerMenuHeader", blob);
							break;
						default:
							DebugUtils.Break();
							break;
					}

					break;

				case 4:
					namedBlob = new NamedBlob("TopMenu", blob);
					break;
				case 168:
					namedBlob = new NamedBlob("BackgroundView", blob);
					break;
				default:
					DebugUtils.Break();
					break;
			}

			if (namedBlob != null)
			{
				this.NamedBlobs.Add(namedBlob);
			}

			return true;
		}

		/// <summary>   Draw part. </summary>
		///
		/// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>
		///
		/// <param name="graphics">     The graphics. </param>
		/// <param name="boundingRect"> The bounding rectangle. </param>

		public void DrawParts(Graphics graphics, Rectangle boundingRect)
		{
			var colorInactive = Color.FromArgb(100, 0, 0, 0);
			var colorLightGray = Color.FromArgb(210, 210, 210);
			var colorGray = Color.FromArgb(75, 75, 75);
			var colorActive = Color.Black;
			var backgroundColor = ColorTranslator.FromHtml(this.ResourceData.BackgroundColor);
			var fontFamilyMedium = this.FontCollection.Families.Single(f => f.Name == "Roboto Medium");
			var fontFamilyLight = this.FontCollection.Families.Single(f => f.Name == "Roboto Light");

			foreach (var blob in this.NamedBlobs)
			{
				switch (blob.Name)
				{
					case "SelectedMenu":
						{
							var color = ColorTranslator.FromHtml(this.ResourceData.PrimaryColor);
							var colorActiveMenu = Color.FromArgb(45, color);
							var iconImage = this.ImageCollection["ScheduleIcon"];
							var iconDefaultColor = ColorTranslator.FromHtml("#5f6368");
							Bitmap activeIconImage;

							activeIconImage = iconImage.ChangeColor(iconDefaultColor, color, 30)
								.ChangeColor(ColorTranslator.FromHtml("#babcbe"), color.Lighten(.50).Saturate(-.40))
								.ChangeColor(ColorTranslator.FromHtml("#b5b7b9"), color.Lighten(.60).Saturate(-.50))
								.ChangeColor(ColorTranslator.FromHtml("#d8d9da"), color.Lighten(.70).Saturate(-.60))
								.ChangeColor(ColorTranslator.FromHtml("#e3e4e5"), color.Lighten(.80).Saturate(-.70))
								.ChangeColor(ColorTranslator.FromHtml("#fefefe"), color.Lighten(.90).Saturate(-.80));

							blob.DrawBackground(graphics, boundingRect, backgroundColor);
							blob.DrawRoundedRect(graphics, boundingRect, new Rectangle(-10, 2, 170, 30), colorActiveMenu, 12);
							blob.DrawImage(graphics, boundingRect, activeIconImage, 13, 10);

							blob.DrawText(graphics, boundingRect, fontFamilyLight, "Schedule", color, new Point(44, 13), 7F);
						}
						break;
					case "MiddleDivider":
						{
							blob.DrawBackground(graphics, boundingRect, backgroundColor);
							blob.DrawHorzontalLine(graphics, boundingRect, colorLightGray, 1);
						}
						break;
					case "LowerDivider":
						{
							blob.DrawBackground(graphics, boundingRect, backgroundColor);
							blob.DrawHorzontalLine(graphics, boundingRect, colorLightGray, 1);
						}
						break;
					case "UpperMenuIcons":
						{
							blob.DrawBackground(graphics, boundingRect, backgroundColor);

							blob.DrawImage(graphics, boundingRect, this.ImageCollection["SpeakersIcon"], 0, 2);
							blob.DrawImage(graphics, boundingRect, this.ImageCollection["MapIcon"], 0, 28);
							blob.DrawImage(graphics, boundingRect, this.ImageCollection["AboutIcon"], 0, 53);
						}
						break;
					case "LowerMenuIcons":
						{
							blob.DrawBackground(graphics, boundingRect, backgroundColor);

							blob.DrawBackground(graphics, boundingRect, backgroundColor);

							blob.DrawImage(graphics, boundingRect, this.ImageCollection["AccountIcon"], 0, 2);
							blob.DrawImage(graphics, boundingRect, this.ImageCollection["SupportIcon"], 0, 28);
							blob.DrawImage(graphics, boundingRect, this.ImageCollection["LogoutIcon"], 0, 53);
						}
						break;
					case "UpperMenuItems":
						{
							blob.DrawBackground(graphics, boundingRect, backgroundColor);

							blob.DrawText(graphics, boundingRect, fontFamilyLight, "Speakers", colorActive, new Point(0, 4), 7F);
							blob.DrawText(graphics, boundingRect, fontFamilyLight, "Map", colorActive, new Point(0, 30), 7F);
							blob.DrawText(graphics, boundingRect, fontFamilyLight, "About", colorActive, new Point(0, 55), 7F);
						}
						break;
					case "LowerMenuItems":
						{
							blob.DrawBackground(graphics, boundingRect, backgroundColor);

							blob.DrawText(graphics, boundingRect, fontFamilyLight, "Account", colorActive, new Point(0, 4), 7F);
							blob.DrawText(graphics, boundingRect, fontFamilyLight, "Support", colorActive, new Point(0, 30), 7F);
							blob.DrawText(graphics, boundingRect, fontFamilyLight, "Logout", colorActive, new Point(0, 55), 7F);
						}
						break;
					case "UpperMenuHeader":
						{
							blob.DrawText(graphics, boundingRect, fontFamilyMedium, "CONFERENCE", colorInactive, backgroundColor, 7F);
						}
						break;
					case "LowerMenuHeader":
						{
							blob.DrawText(graphics, boundingRect, fontFamilyMedium, "ACCOUNT", colorInactive, backgroundColor, 7F);
						}
						break;
					case "TopMenu":
						break;
					case "BackgroundView":
						{
							var color = Color.FromArgb(50, 0, 0, 0);

							blob.DrawVerticalLine(graphics, boundingRect, colorGray);
							blob.DrawVerticalShadowLine(graphics, boundingRect, color, 7, 1);
						}
						break;
					default:
						DebugUtils.Break();
						break;                
				}
			}
		}

		/// <summary>   Draw previous screen. </summary>
		///
		/// <remarks>   CloudIDEaaS, 11/29/2020. </remarks>
		///
		/// <param name="previousScreenProcessor">  The previous screen processor. </param>
		/// <param name="graphics">                 The graphics. </param>
		/// <param name="boundingRect">             The bounding rectangle. </param>
		/// <param name="previousScreenPainter">    The previous screen painter. </param>

		public void DrawPreviousScreen(IScreenProcessor previousScreenProcessor, Graphics graphics, Rectangle boundingRect, ScreenPainter previousScreenPainter)
		{
			var backgroundViewBlob = this.NamedBlobs.Single(b => b.Name == "BackgroundView");
			var clipRect = backgroundViewBlob.Blob.Rectangle;
			var color = Color.FromArgb(160, 136, 136, 136);

			clipRect.Offset(boundingRect.X, boundingRect.Y);

			graphics.SetClip(clipRect);

			previousScreenPainter(this, new PaintEventArgs(graphics, boundingRect));
			backgroundViewBlob.DrawBackground(graphics, boundingRect, color);

			graphics.ResetClip();
		}
	}
}
