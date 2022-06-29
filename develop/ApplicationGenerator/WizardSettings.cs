using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBase;

namespace AbstraX
{
    /// <summary>   A wizard settings. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>

    public class WizardSettings : WizardSettingsBase
    {
        /// <summary>   Gets or sets the generator arguments kind. </summary>
        ///
        /// <value> The generator arguments kind. </value>

        public string GeneratorArgumentsKind { get; set; }

        /// <summary>   Gets or sets the type of the generator handler. </summary>
        ///
        /// <value> The type of the generator handler. </value>

        public string GeneratorHandlerType { get; set; }

        /// <summary>   Gets or sets the generator kind. </summary>
        ///
        /// <value> The generator kind. </value>

        public string GeneratorKind { get; set; }

        /// <summary>   Gets or sets the name of the application. </summary>
        ///
        /// <value> The name of the application. </value>

        public string AppName { get; set; }

        /// <summary>   Gets or sets information describing the resource. </summary>
        ///
        /// <value> Information describing the resource. </value>

        public IResourceData ResourceData { get; set; }

        /// <summary>   Gets or sets the command log. </summary>
        ///
        /// <value> The command log. </value>

        public IList<string> CommandLog { get; set; }

        /// <summary>   Gets or sets the wizard control. </summary>
        ///
        /// <value> The wizard control. </value>

        public WizardControl WizardControl { get; set; }

        /// <summary>   Gets or sets the description. </summary>
        ///
        /// <value> The description. </value>

        public string AppDescription { get; set; }

        /// <summary>   Gets or sets the name of the organization. </summary>
        ///
        /// <value> The name of the organization. </value>

        public string OrganizationName { get; set; }

        /// <summary>   Gets or sets options for controlling the additional. </summary>
        ///
        /// <value> Options that control the additional. </value>

        public Dictionary<string, object> AdditionalOptions { get; set; }

        /// <summary>   Gets or sets the pathname of the root working directory. </summary>
        ///
        /// <value> The pathname of the root working directory. </value>

        public string RootWorkingDirectory { get; set; }

        /// <summary>   Gets or sets the application layout surveyor. </summary>
        ///
        /// <value> The application layout surveyor. </value>

        public IAppFolderStructureSurveyor AppLayoutSurveyor { get; set; }

        /// <summary>   Gets or sets the hydra apps admin services client configuration. </summary>
        ///
        /// <value> The hydra apps admin services client configuration. </value>

        public IHydraAppsAdminServicesClientConfig HydraAppsAdminServicesClientConfig { get; set; }

        /// <summary>   Returns an enumerator that iterates through the collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        ///
        /// <returns>   An enumerator that can be used to iterate through the collection. </returns>

        public override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            if (this.GeneratorArgumentsKind != null)
            {
                yield return new KeyValuePair<string, object>("GeneratorArgumentsKind", this.GeneratorArgumentsKind);
            }

            if (this.GeneratorHandlerType != null)
            {
                yield return new KeyValuePair<string, object>("GeneratorHandlerType", this.GeneratorHandlerType);
            }

            if (this.AppName != null)
            {
                yield return new KeyValuePair<string, object>("AppName", this.AppName);
            }

            if (this.AppDescription != null)
            {
                yield return new KeyValuePair<string, object>("AppDescription", this.AppDescription);
            }

            if (this.OrganizationName != null)
            {
                yield return new KeyValuePair<string, object>("OrganizationName", this.OrganizationName);
            }

            if (this.AdditionalOptions != null)
            {
                yield return new KeyValuePair<string, object>("AdditionalOptions", this.AdditionalOptions);
            }

            if (this.RootWorkingDirectory != null)
            {
                yield return new KeyValuePair<string, object>("RootWorkingDirectory", this.RootWorkingDirectory);
            }

            if (this.AppLayoutSurveyor != null)
            {
                yield return new KeyValuePair<string, object>("AppLayoutSurveyor", this.AppLayoutSurveyor);
            }

            if (this.ResourceData != null)
            {
                yield return new KeyValuePair<string, object>("ResourceData", this.ResourceData);
            }

            if (this.WizardControl != null)
            {
                yield return new KeyValuePair<string, object>("WizardControl", this.WizardControl);
            }

            if (this.CommandLog != null)
            {
                yield return new KeyValuePair<string, object>("CommandLog", this.CommandLog);
            }

            if (this.HydraAppsAdminServicesClientConfig != null)
            {
                yield return new KeyValuePair<string, object>("HydraAppsAdminServicesClientConfig", this.HydraAppsAdminServicesClientConfig);
            }
        }
    }
}
