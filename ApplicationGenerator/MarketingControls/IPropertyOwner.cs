// file:	MarketingControls\IPropertyOwner.cs
//
// summary:	Declares the IPropertyOwner interface

using AbstraX.MarketingControls.SocialMedia;
using AbstraX.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.MarketingControls
{
    /// <summary>   Interface for property owner. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>

    public interface IPropertyOwner
    {
        /// <summary>   Property changed. </summary>
        ///
        /// <param name="sender">           Source of the event. </param>
        /// <param name="socialMediaEntry"> The social media entry. </param>

        void PropertyChanged(object sender, SocialMediaEntry socialMediaEntry);

        /// <summary>   Property changed. </summary>
        ///
        /// <param name="sender">       Source of the event. </param>
        /// <param name="tellOthers">   The tell others. </param>

        void PropertyChanged(object sender, TellOthers tellOthers);
    }
}
