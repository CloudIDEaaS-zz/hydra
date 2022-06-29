// file:	Resources\SocialMediaList.cs
//
// summary:	Implements the social media list class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Resources
{
    /// <summary>   List of social medias. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/7/2021. </remarks>

    public class SocialMediaList
    {
        /// <summary>   Gets or sets the entries. </summary>
        ///
        /// <value> The entries. </value>

        public SocialMediaEntry[] SocialMedia { get; set; }

        /// <summary>   Gets or sets the pathname of the working directory. </summary>
        ///
        /// <value> The pathname of the working directory. </value>

        public string WorkingDirectory
        {
            set
            {
                foreach (var socialMedia in this.SocialMedia)
                {
                    socialMedia.WorkingDirectory = value;
                }
            }
        }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/7/2021. </remarks>

        public SocialMediaList()
        {
            this.SocialMedia = new SocialMediaEntry[0];
        }
    }
}
