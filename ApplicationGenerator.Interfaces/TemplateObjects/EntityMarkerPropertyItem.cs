// file:	TemplateObjects\EntityMarkerPropertyItem.cs
//
// summary:	Implements the entity marker property item class

using AbstraX.DataAnnotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.TemplateObjects
{
    /// <summary>   An entity marker property item. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public abstract class EntityMarkerPropertyItem : EntityPropertyItem
    {
        /// <summary>   Gets the entity marker kind. </summary>
        ///
        /// <value> The entity marker kind. </value>

        public abstract EntityMarkerKind EntityMarkerKind { get; }
    }

    /// <summary>   An entity marker application settings kind. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public class EntityMarkerAppSettingsKind : EntityMarkerPropertyItem
    {
        /// <summary>   Gets the path. </summary>
        ///
        /// <value> The user interface path. </value>

        public string UIPath { get; }

        /// <summary>   Gets the application settings kind. </summary>
        ///
        /// <value> The application settings kind. </value>

        public AppSettingsKind AppSettingsKind { get; }

        /// <summary>   Gets the entity marker kind. </summary>
        ///
        /// <value> The entity marker kind. </value>

        public override EntityMarkerKind EntityMarkerKind => EntityMarkerKind.AppSettingsKind;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="uiPathForAppSettings"> The path for application settings. </param>
        /// <param name="appSettingsKind">      The application settings kind. </param>

        public EntityMarkerAppSettingsKind(string uiPathForAppSettings, AppSettingsKind appSettingsKind)
        {
            UIPath = uiPathForAppSettings;
            AppSettingsKind = AppSettingsKind;
        }
    }

    /// <summary>   An entity marker identity user kind. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public class EntityMarkerIdentityEntityUserKind : EntityMarkerPropertyItem
    {
        /// <summary>   Gets the path. </summary>
        ///
        /// <value> The user interface path. </value>

        public string UIPath { get; }

        /// <summary>   Gets the task capability. </summary>
        ///
        /// <value> The task capability. </value>

        public TaskCapabilities TaskCapability { get; }

        /// <summary>   Gets the entity marker kind. </summary>
        ///
        /// <value> The entity marker kind. </value>

        public override EntityMarkerKind EntityMarkerKind => EntityMarkerKind.IdentityEntityUser;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="uiPathForIdentityEntityUser"> The path for identity entity user. </param>
        /// <param name="taskCapability"></param>

        public EntityMarkerIdentityEntityUserKind(string uiPathForIdentityEntityUser, TaskCapabilities taskCapability)
        {
            UIPath = uiPathForIdentityEntityUser;
            TaskCapability = taskCapability;
        }
    }

    /// <summary>   An entity marker user interface group. </summary>
    ///
    /// <remarks>   Ken, 10/9/2020. </remarks>

    public class EntityMarkerUIGroup : EntityMarkerPropertyItem
    {
        /// <summary>   Gets the path. </summary>
        ///
        /// <value> The user interface path. </value>

        public string UIPath { get; }

        /// <summary>   Gets the kind. </summary>
        ///
        /// <value> The user interface kind. </value>

        public UIKind UIKind { get; }

        /// <summary>   Gets the load kind. </summary>
        ///
        /// <value> The user interface load kind. </value>

        public UILoadKind UILoadKind { get; }

        /// <summary>   Gets the entity marker kind. </summary>
        ///
        /// <value> The entity marker kind. </value>

        public override EntityMarkerKind EntityMarkerKind => EntityMarkerKind.UIGroup;

        /// <summary>   Gets a unique identifier. </summary>
        ///
        /// <value> The identifier of the unique. </value>

        public Guid Guid { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/9/2020. </remarks>
        ///
        /// <param name="uiPath">       . </param>
        /// <param name="guid">         Unique identifier. </param>
        /// <param name="uiKind">       The kind. </param>
        /// <param name="uiLoadKind">   The load kind. </param>

        public EntityMarkerUIGroup(string uiPath, UIKind uiKind = UIKind.NotSpecified, UILoadKind uiLoadKind = UILoadKind.Default)
        {
            this.Guid = Guid.NewGuid();
            this.UIPath = uiPath;
            this.UIKind = uiKind;
            this.UILoadKind = uiLoadKind;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/12/2020. </remarks>
        ///
        /// <param name="uiKind">       (Optional) The kind. </param>
        /// <param name="uiLoadKind">   (Optional) The load kind. </param>

        public EntityMarkerUIGroup(UIKind uiKind = UIKind.NotSpecified, UILoadKind uiLoadKind = UILoadKind.Default)
        {
            this.Guid = Guid.NewGuid();
            this.UIKind = uiKind;
            this.UILoadKind = uiLoadKind;
        }
    }
}
