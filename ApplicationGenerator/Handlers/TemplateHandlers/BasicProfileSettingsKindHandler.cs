// file:	Handlers\TemplateHandlers\BasicProfileSettingsKindHandler.cs
//
// summary:	Implements the basic profile settings kind handler class

using AbstraX.DataAnnotations;
using AbstraX.TemplateObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.TemplateHandlers
{
    /// <summary>   A basic profile settings kind handler. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    [AppSettingsKindHandler(AppSettingsKind.ProfilePreferences)]
    public class BasicProfileSettingsKindHandler : IAppSettingsKindHandler
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
        /// <param name="appUIHierarchyNodeObject"></param>
        /// <param name="appSettingsObjects">       The application settings objects. </param>
        /// <param name="projectType">              Type of the project. </param>
        /// <param name="projectFolderRoot">        The project folder root. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool Process(EntityDomainModel entityDomainModel, BusinessModel businessModel, AppUIHierarchyNodeObject appUIHierarchyNodeObject, Dictionary<AppSettingsKind, BusinessModelObject> appSettingsObjects, Guid projectType, string projectFolderRoot, IGeneratorConfiguration generatorConfiguration)
        {
            var profilePreferencesObject = appSettingsObjects[AppSettingsKind.ProfilePreferences];
            var userObject = businessModel.GetDescendants().Single(o => o.Id == profilePreferencesObject.ShadowItem);
            var userEntity = userObject.UIHierarchyNodeObject.Entities.Single(e => e.HasIdentityEntityKind(IdentityEntityKind.User));

            foreach (var attribute in userEntity.Attributes.Where(a => a.HasIdentityFieldKind(IdentityFieldKind.Client) || a.HasIdentityFieldKind(IdentityFieldKind.PasswordHash)))
            {
                attribute.Properties.Add(new EntityPropertyItem
                {
                    PropertyName = "AllowUserEdit",
                    PropertyValue = "true"
                });
            }

            return true;
        }
    }
}
