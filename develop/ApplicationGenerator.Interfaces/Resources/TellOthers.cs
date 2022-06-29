// file:	MarketingControls\TellOthers\TellOthers.cs
//
// summary:	Implements the tell others class

using AbstraX.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Resources
{
    /// <summary>   A tell others. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>

    public class TellOthers : IEmailLink
    {
        private IMarketingObjectAttributeProvider provider;
        private string workingDirectory;

        /// <summary>   Gets or sets URL of the document. </summary>
        ///
        /// <value> The URL. </value>

        public string Url { get; set; }

        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; set; }

        /// <summary>   Gets or sets the pathname of the working directory. </summary>
        ///
        /// <value> The pathname of the working directory. </value>

        [JsonIgnore]
        public string WorkingDirectory
        {
            get
            {
                return workingDirectory;
            }

            set
            {
                workingDirectory = value;
            }
        }

        /// <summary>   Gets the provider. </summary>
        ///
        /// <value> The provider. </value>

        [JsonIgnore]
        public IMarketingObjectAttributeProvider Provider
        {
            get
            {
                if (provider == null)
                {
                    provider = this.GetMarketingObjectAttributeProvider();
                }

                return provider;
            }
        }
        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>

        public TellOthers()
        {
            this.Url = "mailto:subject=[email-title]&body=[email-body]";
            this.Name = "Email";
        }
    }
}
