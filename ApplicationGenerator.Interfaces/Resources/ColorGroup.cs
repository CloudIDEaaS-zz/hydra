// file:	ColorGroup.cs
//
// summary:	Implements the color group class

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   A color group. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/25/2020. </remarks>

    [DebuggerDisplay(" { Name }, BaseColor: { BaseColor }")]
    public class ColorGroup
    {
        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; }

        /// <summary>   Gets or sets the main. </summary>
        ///
        /// <value> The main. </value>

        public Color BaseColor { get; set; }

        /// <summary>   Gets or sets the contrast. </summary>
        ///
        /// <value> The contrast. </value>

        public Color Contrast { get; set; }

        /// <summary>   Gets or sets the shade. </summary>
        ///
        /// <value> The shade. </value>

        public Color Shade { get; set; }

        /// <summary>   Gets or sets the tint. </summary>
        ///
        /// <value> The tint. </value>

        public Color Tint { get; set; }

        /// <summary>   Gets or sets the name of the base color property. </summary>
        ///
        /// <value> The name of the base color property. </value>

        public string BaseColorPropertyName { get; set; }

        /// <summary>   Gets or sets the name of the contrast property. </summary>
        ///
        /// <value> The name of the contrast property. </value>

        public string ContrastPropertyName { get; set; }

        /// <summary>   Gets or sets the name of the shade property. </summary>
        ///
        /// <value> The name of the shade property. </value>

        public string ShadePropertyName { get; set; }

        /// <summary>   Gets or sets the name of the tint property. </summary>
        ///
        /// <value> The name of the tint property. </value>

        public string TintPropertyName { get; set; }

        /// <summary>   Gets or sets the base color lookup variable. </summary>
        ///
        /// <value> The base color lookup variable. </value>

        public string BaseColorLookupVariable { get; set; }

        /// <summary>   Gets or sets the contrast color lookup variable. </summary>
        ///
        /// <value> The contrast color lookup variable. </value>

        public string ContrastColorLookupVariable { get; set; }

        /// <summary>   Gets or sets the shade color lookup variable. </summary>
        ///
        /// <value> The shade color lookup variable. </value>

        public string ShadeColorLookupVariable { get; set; }

        /// <summary>   Gets or sets the tint color variable. </summary>
        ///
        /// <value> The tint color variable. </value>

        public string TintColorVariable { get; set; }

        /// <summary>   Gets the others. </summary>
        ///
        /// <value> The others. </value>

        public Dictionary<string, Color> Others { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/25/2020. </remarks>
        ///
        /// <param name="name"> The name. </param>

        public ColorGroup(string name)
        {
            this.Name = name;
            this.Others = new Dictionary<string, Color>();
        }
    }
}
