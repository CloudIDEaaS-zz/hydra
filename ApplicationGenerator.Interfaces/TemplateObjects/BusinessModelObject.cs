// file:	TemplateObjects\BusinessModelObject.cs
//
// summary:	Implements the business model object class

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AbstraX.TemplateObjects
{
    /// <summary>   The business model object. </summary>
    ///
    /// <remarks>   Ken, 10/1/2020. </remarks>

    [DebuggerDisplay(" { DebugInfo }")]
    public class BusinessModelObject : BusinessModelRecord
    {
        /// <summary>   Gets or sets the children. </summary>
        ///
        /// <value> The children. </value>

        [JsonProperty(Order = 22)]
        public List<BusinessModelObject> Children { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>

        [JsonIgnore]
        public List<UIHierarchyNodeObject> TopUIHierarchyNodeObjects { get; set; }

        /// <summary>   Gets or sets the hierarchy node object. </summary>
        ///
        /// <value> The user interface hierarchy node object. </value>

        [JsonIgnore]
        public UIHierarchyNodeObject UIHierarchyNodeObject { get; set; }

        public BusinessModelObject()
        {
            this.Children = new List<BusinessModelObject>();
            this.TopUIHierarchyNodeObjects = new List<UIHierarchyNodeObject>();
        }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public string DebugInfo
        {
            get
            {
                return string.Format("Name: {0}, "
                    + "Level: {1}, "
                    + "ClassName: {2}",
                    this.Name,
                    this.Level,
                    this.ClassName);
            }
        }
    }
}
