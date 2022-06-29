// file:	Resources\Dimension.cs
//
// summary:	Implements the dimension class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Resources
{
    /// <summary>   A dimension. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/11/2021. </remarks>

    public class Dimension
    {
        /// <summary>   Gets or sets the width. </summary>
        ///
        /// <value> The width. </value>

        public string Width { get; set; }

        /// <summary>   Gets or sets the height. </summary>
        ///
        /// <value> The height. </value>

        public string Height { get; set; }

        /// <summary>   Gets or sets the aspect ratio. </summary>
        ///
        /// <value> The aspect ratio. </value>

        public string AspectRatio { get; set; }

        /// <summary>   Gets or sets the format to use. </summary>
        ///
        /// <value> The format. </value>

        public string Format { get; set; }

        /// <summary>   Gets the width pixels. </summary>
        ///
        /// <value> The width pixels. </value>

        public int WidthPixels
        {
            get
            {
                var regex = new Regex(@"^(?<dimension>\d+?)(?<unitType>[a-zA-Z]+?$)");

                if (regex.IsMatch(this.Width))
                {
                    var match = regex.Match(this.Width);
                    var dimension = int.Parse(match.GetGroupValue("dimension"));
                    var unitType = match.GetGroupValue("unitType");

                    if (unitType != "px")
                    {
                        DebugUtils.Break();
                    }

                    return dimension;
                }

                return -1;
            }
        }

        /// <summary>   Gets the height pixels. </summary>
        ///
        /// <value> The height pixels. </value>

        public int HeightPixels
        {
            get
            {
                var regex = new Regex(@"^(?<dimension>\d+?)(?<unitType>[a-zA-Z]+?$)");

                if (regex.IsMatch(this.Height))
                {
                    var match = regex.Match(this.Height);
                    var dimension = int.Parse(match.GetGroupValue("dimension"));
                    var unitType = match.GetGroupValue("unitType");

                    if (unitType != "px")
                    {
                        DebugUtils.Break();
                    }

                    return dimension;
                }

                return -1;
            }
        }
    }
}