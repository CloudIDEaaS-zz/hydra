// file:	Handlers\TemplateHandlers\BasicAppSettingsKindHandler.cs
//
// summary:	Implements the basic application settings kind handler class

using AbstraX.DataAnnotations;
using AbstraX.TemplateObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.TemplateHandlers
{
    /// <summary>   A basic application settings kind handler. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    [AppSettingsKindHandler(AppSettingsKind.GlobalSettings)]
    public class BasicAppSettingsKindHandler : IAppSettingsKindHandler
    {
        /// <summary>   Gets the priority. </summary>
        ///
        /// <value> The priority. </value>

        public float Priority => 1.0f;

        /// <summary>   Process this.  </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="entityDomainModel">        The entity domain model. </param>
        /// <param name="businessModel">            The business model. </param>
        /// <param name="appUIHierarchyNodeObject">   The application UI hierarchy node object. </param>
        /// <param name="appSettingsObjects">       The application settings objects. </param>
        /// <param name="projectType">              Type of the project. </param>
        /// <param name="projectFolderRoot">        The project folder root. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool Process(EntityDomainModel entityDomainModel, BusinessModel businessModel, AppUIHierarchyNodeObject appUIHierarchyNodeObject, Dictionary<AppSettingsKind, BusinessModelObject> appSettingsObjects, Guid projectType, string projectFolderRoot, IGeneratorConfiguration generatorConfiguration)
        {
            var globalSettingsObject = appSettingsObjects[AppSettingsKind.GlobalSettings];
            var userPreferencesObject = appSettingsObjects[AppSettingsKind.UserPreferences];
            var appSettingsEntityObject = new EntityObject()
            {
                Name = appUIHierarchyNodeObject.Name + "Settings",
                ParentDataItem = globalSettingsObject.Id,
                Attributes = new List<AttributeObject>()
                {
                    new AttributeObject
                    {
                        Name = "Setting",
                        AttributeType = "string",
                        Properties = new List<EntityPropertyItem>()
                        {
                            new EntityPropertyItem 
                            {
                                PropertyName = "GlobalSettingsFieldKind",
                                ChildProperties = new List<EntityPropertyItem>()
                                {
                                    new EntityPropertyItem { PropertyName = "GlobalSettingsFieldKind", PropertyValue = "SettingName" }
                                }
                            }
                        }
                    },
                    new AttributeObject
                    {
                        Name = "Setting Type",
                        AttributeType = "string",
                        Properties = new List<EntityPropertyItem>()
                        {
                            new EntityPropertyItem
                            {
                                PropertyName = "GlobalSettingsFieldKind",
                                ChildProperties = new List<EntityPropertyItem>()
                                {
                                    new EntityPropertyItem { PropertyName = "GlobalSettingsFieldKind", PropertyValue = "SettingType" }
                                }
                            }
                        }
                    },
                    new AttributeObject
                    {
                        Name = "Setting Value",
                        AttributeType = "string",
                        Properties = new List<EntityPropertyItem>()
                        {
                            new EntityPropertyItem
                            {
                                PropertyName = "GlobalSettingsFieldKind",
                                ChildProperties = new List<EntityPropertyItem>()
                                {
                                    new EntityPropertyItem { PropertyName = "GlobalSettingsFieldKind", PropertyValue = "AllowUserCustomize" }
                                }
                            }
                        }
                    },
                    new AttributeObject
                    {
                        Name = "Allow User Customize",
                        AttributeType = "bool",
                        Properties = new List<EntityPropertyItem>()
                        {
                            new EntityPropertyItem
                            {
                                PropertyName = "GlobalSettingsFieldKind",
                                ChildProperties = new List<EntityPropertyItem>()
                                {
                                    new EntityPropertyItem { PropertyName = "GlobalSettingsFieldKind", PropertyValue = "ResetCustomize" }
                                }
                            }
                        }
                    },
                    new AttributeObject
                    {
                        Name = "Reset Customization",
                        AttributeType = "bool",
                        Properties = new List<EntityPropertyItem>()
                        {
                            new EntityPropertyItem
                            {
                                PropertyName = "GlobalSettingsFieldKind",
                                ChildProperties = new List<EntityPropertyItem>()
                                {
                                    new EntityPropertyItem { PropertyName = "GlobalSettingsFieldKind", PropertyValue = "SettingName" }
                                }
                            }
                        }
                    }
                },
                Properties = new List<EntityPropertyItem>()
                {
                    new EntityPropertyItem { PropertyName = "AppSettingsKind", PropertyValue = AppSettingsKind.GlobalSettings.ToString()}
                }
            };

            globalSettingsObject.UIHierarchyNodeObject.Entities.Add(appSettingsEntityObject);
            userPreferencesObject.UIHierarchyNodeObject.Entities.Add(appSettingsEntityObject.Shadow());
            appUIHierarchyNodeObject.AllEntities.Add(appSettingsEntityObject);

            return true;
        }
    }
}
