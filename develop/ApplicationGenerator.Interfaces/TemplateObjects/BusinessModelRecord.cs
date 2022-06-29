// file:	TemplateObjects\BusinessModelRecord.cs
//
// summary:	Implements the business model record class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AbstraX.TemplateObjects
{
    /// <summary>   Information about the business model. </summary>
    ///
    /// <remarks>   Ken, 10/1/2020. </remarks>

    public class BusinessModelRecord
    {
        /// <summary>   Gets or sets the identifier. </summary>
        ///
        /// <value> The identifier. </value>

        [JsonProperty(Order = 0)]
        public int Id { get; set; }

        /// <summary>   Gets or sets the identifier of the parent. </summary>
        ///
        /// <value> The identifier of the parent. </value>

        [JsonProperty(Order = 1)]
        public int ParentId { get; set; }

        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        [JsonProperty(Order = 2)]
        public string Name { get; set; }

        /// <summary>   Gets or sets the description. </summary>
        ///
        /// <value> The description. </value>

        [JsonProperty(Order = 3)]
        public string Description { get; set; }

        /// <summary>   Gets or sets the name of the singular. </summary>
        ///
        /// <value> The name of the singular. </value>

        [JsonProperty(Order = 4)]
        public string SingularName { get; set; }

        /// <summary>   Gets or sets the level. </summary>
        ///
        /// <value> The level. </value>

        [JsonProperty("title", Order = 5)]
        public string Level { get; set; }

        /// <summary>   Gets or sets the name of the class. </summary>
        ///
        /// <value> The name of the class. </value>

        [JsonProperty(Order = 6)]
        public string ClassName { get; set; }

        /// <summary>   Gets or sets the user roles. </summary>
        ///
        /// <value> The user roles. </value>

        [JsonProperty(Order = 7)]
        public string UserRoles { get; set; }

        /// <summary>   Gets or sets the stakeholder kind. </summary>
        ///
        /// <value> The stakeholder kind. </value>

        [JsonProperty(Order = 8)]
        public string StakeholderKind { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is system role. </summary>
        ///
        /// <value> True if this  is system role, false if not. </value>

        [JsonProperty(Order = 9)]
        public bool IsSystemRole { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is pseudo role. </summary>
        ///
        /// <value> True if this  is pseudo role, false if not. </value>

        [JsonProperty(Order = 10)]
        public bool IsPseudoRole { get; set; }

        /// <summary>   Gets or sets the pseudo roles. </summary>
        ///
        /// <value> The pseudo roles. </value>

        [JsonProperty(Order = 11)]
        public string PseudoRoles { get; set; }

        /// <summary>   Gets or sets the shadow item. </summary>
        ///
        /// <value> The shadow item. </value>

        [JsonProperty(Order = 12)]
        public int ShadowItem { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is application. </summary>
        ///
        /// <value> True if this  is application, false if not. </value>

        [JsonProperty(Order = 13)]
        public bool IsApp { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is data item. </summary>
        ///
        /// <value> True if this  is data item, false if not. </value>

        [JsonProperty(Order = 14)]
        public bool IsDataItem { get; set; }

        /// <summary>   Gets or sets the application settings kind. </summary>
        ///
        /// <value> The application settings kind. </value>

        [JsonProperty(Order = 15)]
        public string AppSettingsKind { get; set; }

        /// <summary>   Gets or sets the identity kind. </summary>
        ///
        /// <value> The identity kind. </value>

        [JsonProperty(Order = 16)]
        public string IdentityKind { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is system task. </summary>
        ///
        /// <value> True if this  is system task, false if not. </value>

        [JsonProperty(Order = 17)]
        public bool IsSystemTask { get; set; }

        /// <summary>   Gets or sets the task capabilities. </summary>
        ///
        /// <value> The task capabilities. </value>

        [JsonProperty(Order = 18)]
        public string TaskCapabilities { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the in user interface is shown.
        /// </summary>
        ///
        /// <value> True if show in user interface, false if not. </value>

        [JsonProperty(Order = 19)]
        public bool ShowInUI { get; set; }

        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The user interface name. </value>

        [JsonProperty(Order = 20)]
        public string UIName { get; set; }

        /// <summary>   Gets or sets the kind. </summary>
        ///
        /// <value> The user interface kind. </value>

        [JsonProperty(Order = 21)]
        public string UIKind { get; set; }

        /// <summary>   Gets or sets the load kind. </summary>
        ///
        /// <value> The user interface load kind. </value>

        [JsonProperty(Order = 22)]
        public string UILoadKind { get; set; }
    }
}
