// file:	MarketingControls\SocialMedia\frmUrlEditPopup.cs
//
// summary:	Implements the form URL edit popup class

using AbstraX.MarketingControls.SocialMedia;
using AbstraX.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace AbstraX.MarketingControls
{
    /// <summary>   Popup for displayng the form URL edit. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>

    public partial class frmUrlEditPopup : ChildPopupForm
    {
        private IDisposable wait;

        /// <summary>   Gets the value. </summary>
        ///
        /// <value> The value. </value>

        public string Value { get; }

        /// <summary>   Gets the parts. </summary>
        ///
        /// <value> The parts. </value>

        public UrlParts Parts { get; }

        /// <summary>   Gets the tell others entry. </summary>
        ///
        /// <value> The tell others entry. </value>

        public TellOthers TellOthersEntry { get; }

        /// <summary>   Gets a list of social medias. </summary>
        ///
        /// <value> A list of social medias. </value>

        public SocialMediaEntry SocialMediaEntry { get; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/15/2021. </remarks>

        public frmUrlEditPopup()
        {
            InitializeComponent();
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="parentControl">    The parent control. </param>
        /// <param name="socialMediaEntry"> A list of social medias. </param>
        /// <param name="propertyInfo">     Information describing the property. </param>
        /// <param name="originalValue">    The original value. </param>
        /// <param name="value">            The value. </param>

        public frmUrlEditPopup(Control parentControl, SocialMediaEntry socialMediaEntry, PropertyInfo propertyInfo, string originalValue, object value) : base(parentControl.Parent, true, false)
        {
            var pattern = @"\[(?<part>.+?)\]";
            var regex = new Regex(pattern);
            var propertyOwner = (IPropertyOwner) parentControl;

            wait = parentControl.Wait();

            this.Value = (string)value;
            this.Parts = new UrlParts(propertyOwner);
            this.SocialMediaEntry = socialMediaEntry;

            InitializeComponent();

            labelLoading.BringToFront();

            this.DelayInvoke(100, () =>
            {
                if (regex.IsMatch(originalValue))
                {
                    var matches = regex.Matches(originalValue);

                    foreach (var match in matches.Cast<Match>())
                    {
                        var group = match.Groups["part"];
                        var partValue = group.Value;

                        this.Parts.Add(group, new UrlPart(partValue, this.Parts, socialMediaEntry, originalValue, propertyInfo, socialMediaEntry.GetUrlPartAttributes(partValue)));
                    }

                    propertyGrid.SelectedObject = this.Parts;

                    labelLoading.Visible = false;
                    wait.Dispose();
                }
            });
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <param name="parentControl">    The parent control. </param>
        /// <param name="tellOthers">       The tell others. </param>
        /// <param name="propertyInfo">     Information describing the property. </param>
        /// <param name="originalValue">    The original value. </param>
        /// <param name="value">            The value. </param>

        public frmUrlEditPopup(Control parentControl, TellOthers tellOthers, PropertyInfo propertyInfo, string originalValue, object value) : base(parentControl, true, false)
        {
            var pattern = @"\[(?<part>.+?)\]";
            var regex = new Regex(pattern);
            var propertyOwner = parentControl.GetAncestors().Concat(new[] { parentControl }).OfType<IPropertyOwner>().Single();

            wait = parentControl.Wait();

            this.Value = (string)value;
            this.Parts = new UrlParts(propertyOwner);
            this.TellOthersEntry = tellOthers;

            InitializeComponent();

            this.DelayInvoke(100, () =>
            {
                if (regex.IsMatch(originalValue))
                {
                    var matches = regex.Matches(originalValue);

                    foreach (var match in matches.Cast<Match>())
                    {
                        var group = match.Groups["part"];
                        var partValue = group.Value;

                        this.Parts.Add(group, new UrlPart(partValue, this.Parts, tellOthers, originalValue, propertyInfo, tellOthers.GetUrlPartAttributes(partValue)));
                    }

                    propertyGrid.SelectedObject = this.Parts;

                    labelLoading.Visible = false;
                    wait.Dispose();
                }
            });
        }

        /// <summary>   Raises the <see cref="E:System.Windows.Forms.Form.Closed" /> event. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/15/2021. </remarks>
        ///
        /// <param name="e">    The <see cref="T:System.EventArgs" /> that contains the event data. </param>

        protected override void OnClosed(EventArgs e)
        {
            wait.Dispose();
            base.OnClosed(e);
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {

        }

        private void frmUrlEditPopup_Resize(object sender, EventArgs e)
        {
            var clientRectangle = this.ClientRectangle;
            var midPoint = new Point(clientRectangle.Width / 2, clientRectangle.Height / 2);

            labelLoading.Top = this.Top + 40;
            labelLoading.Left = midPoint.X - (labelLoading.Width / 2);
        }
    }
}
