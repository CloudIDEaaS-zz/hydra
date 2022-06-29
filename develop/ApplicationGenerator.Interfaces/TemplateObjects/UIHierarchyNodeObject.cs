// file:	TemplateObjects\UIHierarchyNodeObject.cs
//
// summary:	Implements the hierarchy node object class

using AbstraX.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.TemplateObjects
{
    /// <summary>   A UI hierarchy node object. </summary>
    ///
    /// <remarks>   Ken, 10/3/2020. </remarks>

    [DebuggerDisplay(" { DebugInfo }")]
    public class UIHierarchyNodeObject
    {
        /// <summary>   A BusinessModelObject to process. </summary>
        private BusinessModelObject businessModelObject;

        /// <summary>   Gets or sets the identifier. </summary>
        ///
        /// <value> The identifier. </value>

        public int Id { get; set; }

        /// <summary>   Gets or sets the parent. </summary>
        ///
        /// <value> The parent. </value>

        public UIHierarchyNodeObject Parent { get; set; }

        /// <summary>   Gets the children. </summary>
        ///
        /// <value> The children. </value>

        public List<UIHierarchyNodeObject> Children { get; set; }

        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; set; }

        /// <summary>   Gets or sets the description. </summary>
        ///
        /// <value> The description. </value>

        public string Description { get; set; }

        /// <summary>   Gets or sets the name of the singular. </summary>
        ///
        /// <value> The name of the singular. </value>

        public string SingularName { get; set; }

        /// <summary>   Gets or sets the level. </summary>
        ///
        /// <value> The level. </value>

        public BusinessModelLevel Level { get; set; }

        /// <summary>   Gets or sets the user roles. </summary>
        ///
        /// <value> The user roles. </value>

        public List<string> UserRoles { get; set; }

        /// <summary>   Gets or sets the stakeholder kind. </summary>
        ///
        /// <value> The stakeholder kind. </value>

        public StakeholderKind StakeholderKind { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is system role. </summary>
        ///
        /// <value> True if this  is system role, false if not. </value>

        public bool IsSystemRole { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is pseudo role. </summary>
        ///
        /// <value> True if this  is pseudo role, false if not. </value>

        public bool IsPseudoRole { get; set; }

        /// <summary>   Gets or sets the pseudo roles. </summary>
        ///
        /// <value> The pseudo roles. </value>

        public List<string> PseudoRoles { get; set; }

        /// <summary>   Gets or sets the shadow item. </summary>
        ///
        /// <value> The shadow item. </value>

        public int ShadowItem { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is application. </summary>
        ///
        /// <value> True if this  is application, false if not. </value>

        public bool IsApp { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is data item. </summary>
        ///
        /// <value> True if this  is data item, false if not. </value>

        public bool IsDataItem { get; set; }

        /// <summary>   Gets or sets the application settings kind. </summary>
        ///
        /// <value> The application settings kind. </value>

        public AppSettingsKind AppSettingsKind { get; set; }

        /// <summary>   Gets or sets the identity kind. </summary>
        ///
        /// <value> The identity kind. </value>

        public IdentityKind IdentityKind { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is system task. </summary>
        ///
        /// <value> True if this  is system task, false if not. </value>

        public bool IsSystemTask { get; set; }

        /// <summary>   Gets or sets the task capabilities. </summary>
        ///
        /// <value> The task capabilities. </value>

        public TaskCapabilities TaskCapabilities { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the in user interface is shown.
        /// </summary>
        ///
        /// <value> True if show in user interface, false if not. </value>

        public bool ShowInUI { get; set; }

        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The user interface name. </value>

        public string UIName { get; set; }

        /// <summary>   Gets or sets the kind. </summary>
        ///
        /// <value> The user interface kind. </value>

        public UIKind UIKind { get; set; }

        /// <summary>   Gets or sets the load kind. </summary>
        ///
        /// <value> The user interface load kind. </value>

        public UILoadKind UILoadKind { get; set; }

        /// <summary>   Gets or sets the entities. </summary>
        ///
        /// <value> The entities. </value>
        /// 
        public List<EntityObject> Entities { get; set; }

        /// <summary>   Gets or sets the full pathname of the calculated user interface file. </summary>
        ///
        /// <value> The full pathname of the calculated user interface file. </value>

        public string ConstructedUIPath { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="businessModelObject">    A BusinessModelObject to process. </param>

        public UIHierarchyNodeObject(BusinessModelObject businessModelObject)
        {
            this.businessModelObject = businessModelObject;
            this.Id = businessModelObject.Id;
            this.Name = businessModelObject.Name;
            this.Description = businessModelObject.Description;
            this.SingularName = businessModelObject.SingularName;
            this.Level = EnumUtils.GetValue<BusinessModelLevel>(businessModelObject.Level.RemoveText(" "));
            this.UserRoles = businessModelObject.UserRoles == null ? new List<string>() : businessModelObject.UserRoles.Split(',').ToList();
            this.StakeholderKind = businessModelObject.StakeholderKind.IsNullOrEmpty() ? StakeholderKind.Normal : EnumUtils.GetValue<StakeholderKind>(businessModelObject.StakeholderKind);
            this.IsSystemRole = businessModelObject.IsSystemRole;
            this.IsPseudoRole = businessModelObject.IsPseudoRole;
            this.PseudoRoles = businessModelObject.PseudoRoles == null ? new List<string>() : businessModelObject.PseudoRoles.Split(',').ToList();
            this.ShadowItem = businessModelObject.ShadowItem;
            this.IsApp = businessModelObject.IsApp;
            this.IsDataItem = businessModelObject.IsDataItem;
            this.AppSettingsKind = businessModelObject.AppSettingsKind.IsNullOrEmpty() ? AppSettingsKind.None : EnumUtils.GetValue<AppSettingsKind>(businessModelObject.AppSettingsKind);
            this.IdentityKind = businessModelObject.IdentityKind.IsNullOrEmpty() ? IdentityKind.None : EnumUtils.GetValue<IdentityKind>(businessModelObject.IdentityKind);
            this.IsSystemTask = businessModelObject.IsSystemTask;
            
            if (businessModelObject.TaskCapabilities != null)
            {
                foreach (var taskCapability in businessModelObject.TaskCapabilities.Split(',').Select(c => EnumUtils.GetValue<TaskCapabilities>(c)))
                {
                    this.TaskCapabilities |= taskCapability;
                }
            }

            this.ShowInUI = businessModelObject.ShowInUI;
            this.UIName = businessModelObject.UIName;
            this.UIKind = businessModelObject.UIKind.IsNullOrEmpty() ? UIKind.NotSpecified : EnumUtils.GetValue<UIKind>(businessModelObject.UIKind);
            this.UILoadKind = businessModelObject.UILoadKind.IsNullOrEmpty() ? UILoadKind.Default : EnumUtils.GetValue<UILoadKind>(businessModelObject.UILoadKind);

            this.Children = new List<UIHierarchyNodeObject>();
            this.Entities = new List<EntityObject>();

            businessModelObject.UIHierarchyNodeObject = this;
        }

        /// <summary>   Specialized default constructor for use only by derived class. </summary>
        ///
        /// <remarks>   Ken, 10/7/2020. </remarks>

        protected UIHierarchyNodeObject()
        {
        }

        /// <summary>   Adds a child. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="hierarchyNodeObject">  The hierarchy node object. </param>

        public void AddChild(UIHierarchyNodeObject hierarchyNodeObject)
        {
            hierarchyNodeObject.Parent = this;
            this.Children.Add(hierarchyNodeObject);
        }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public string DebugInfo
        {
            get
            {
                return string.Format("Id: {0}, "
                    + "Name: {1}, "
                    + "Level: {2}, "
                    + "ShowInUI: {3}, "
                    + "UIKind: {4}, "
                    + "UILoadKind: {5}",
                    this.Id,
                    this.Name,
                    this.Level,
                    this.ShowInUI,
                    this.UIKind,
                    this.UILoadKind);
            }
        }
    }
}
