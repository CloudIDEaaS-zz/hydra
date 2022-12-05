// file:	Handlers\TemplateHandlers\EntityContainerHandler.cs
//
// summary:	Implements the entity container handler class

using AbstraX.TemplateObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.TemplateHandlers
{
    /// <summary>   An entity container handler. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public class EntityContainerHandler : IEntityContainerHandler
    {
        /// <summary>   Gets the priority. </summary>
        ///
        /// <value> The priority. </value>

        public float Priority => 1.0f;

        /// <summary>   Process this. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="appUIHierarchyNodeObject"> The application user interface hierarchy node object. </param>
        /// <param name="projectType">              Type of the project. </param>
        /// <param name="projectFolderRoot">        The project folder root. </param>
        /// <param name="entitiesProject">          The entities project. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="entityContainerObject"></param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool Process(AppUIHierarchyNodeObject appUIHierarchyNodeObject, Guid projectType, string projectFolderRoot, CodeInterfaces.IVSProject entitiesProject, IGeneratorConfiguration generatorConfiguration, out EntityObject entityContainerObject)
        {
            var userEntity = appUIHierarchyNodeObject.AllEntities.SingleOrDefault(e => e.HasIdentityEntityKind(DataAnnotations.IdentityEntityKind.User));
            var roleEntity = appUIHierarchyNodeObject.AllEntities.SingleOrDefault(e => e.HasIdentityEntityKind(DataAnnotations.IdentityEntityKind.Role));
            var userToRoleEntity = appUIHierarchyNodeObject.AllEntities.SingleOrDefault(e => e.HasIdentityEntityKind(DataAnnotations.IdentityEntityKind.UserToRole));

            entityContainerObject = new EntityObject(generatorConfiguration.AppName + "Context", "*");

            foreach (var entity in appUIHierarchyNodeObject.AllEntities)
            {
                entityContainerObject.Attributes.Add(new AttributeObject(entity.Name.Pluralize(), string.Format("EntitySet[@Entity='{0}']", entity.Name)));
            }

            entityContainerObject.Properties.AddRange(new List<EntityPropertyItem>
            {
                new EntityMarkerUIGroup(DataAnnotations.UIKind.WelcomePage, DataAnnotations.UILoadKind.HomePage),
                new EntityMarkerUIGroup(DataAnnotations.UIKind.TabsPage, DataAnnotations.UILoadKind.MainPage),
                new EntityPropertyItem
                {
                    PropertyName = "AppName",
                    ChildProperties = new List<EntityPropertyItem>
                    {
                        new EntityPropertyItem { PropertyName = "Name", PropertyValue = generatorConfiguration.AppName},
                        new EntityPropertyItem { PropertyName = "Description", PropertyValue = generatorConfiguration.AppDescription}
                    }
                },
                new EntityPropertyItem
                {
                    PropertyName = "Resources",
                    PropertyValue = "AppResources"
                },
                new EntityPropertyItem
                {
                    PropertyName = "UI",
                    ChildProperties = new List<EntityPropertyItem>
                    {
                        new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "WelcomePage"},
                        new EntityPropertyItem { PropertyName = "UILoadKind", PropertyValue = "HomePage"}
                    }
                },
                new EntityPropertyItem
                {
                    PropertyName = "UINavigationName",
                    ChildProperties = new List<EntityPropertyItem>
                    {
                        new EntityPropertyItem { PropertyName = "Name", PropertyValue = "Welcome"},
                        new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "WelcomePage"},
                        new EntityPropertyItem { PropertyName = "UILoadKind", PropertyValue = "HomePage"}
                    }
                },
                new EntityPropertyItem
                {
                    PropertyName = "UI",
                    ChildProperties = new List<EntityPropertyItem>
                    {
                        new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "TabsPage"},
                        new EntityPropertyItem { PropertyName = "UILoadKind", PropertyValue = "MainPage"}
                    }
                },
                new EntityPropertyItem
                {
                    PropertyName = "UINavigationName",
                    ChildProperties = new List<EntityPropertyItem>
                    {
                        new EntityPropertyItem { PropertyName = "Name", PropertyValue = "Main"},
                        new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "TabsPage"},
                        new EntityPropertyItem { PropertyName = "UILoadKind", PropertyValue = "MainPage"}
                    }
                }
            });

            if (CompareExtensions.AllAreNotNull(userEntity, roleEntity, userToRoleEntity))
            {
                entityContainerObject.Properties.AddRange(new List<EntityPropertyItem>
                {
                    new EntityPropertyItem
                    {
                        PropertyName = "Identity",
                        ChildProperties = new List<EntityPropertyItem>()
                        {
                            new EntityPropertyItem { PropertyName = "IdentitySet", PropertyValue = userEntity.Name.Pluralize().RemoveSpaces()},
                            new EntityPropertyItem { PropertyName = "RoleSet", PropertyValue = roleEntity.Name.Pluralize().RemoveSpaces()},
                            new EntityPropertyItem { PropertyName = "UserToRolesSet", PropertyValue = userToRoleEntity.Name.Pluralize().RemoveSpaces()}
                        }
                    },
                    new EntityPropertyItem
                    {
                        PropertyName = "Authorize",
                        ChildProperties = new List<EntityPropertyItem>
                        {
                            new EntityPropertyItem { PropertyName = "UIKind", PropertyValue = "TabsPage"},
                            new EntityPropertyItem { PropertyName = "UILoadKind", PropertyValue = "MainPage"},
                            new EntityPropertyItem { PropertyName = "Roles", PropertyValue = "User"}
                        }
                    }
                });
            }

            return true;
        }
    }
}
