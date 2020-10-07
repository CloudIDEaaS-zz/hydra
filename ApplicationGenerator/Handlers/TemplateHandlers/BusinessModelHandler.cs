// file:	Handlers\TemplateHandlers\BusinessModelHandler.cs
//
// summary:	Implements the business model handler class

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX;
using Newtonsoft.Json.Serialization;
using Utils;

namespace AbstraX.Handlers.TemplateHandlers
{
    /// <summary>   The business model handler. </summary>
    ///
    /// <remarks>   Ken, 10/1/2020. </remarks>

    public class BusinessModelHandler : IBusinessModelGeneratorHandler
    {
        /// <summary>   Gets the priority. </summary>
        ///
        /// <value> The priority. </value>

        public float Priority => 1.0f;

        /// <summary>   Process this. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="businessModel">            The business model. </param>
        /// <param name="projectType">              Type of the project. </param>
        /// <param name="projectFolderRoot">        The project folder root. </param>
        /// <param name="templateFile">             The template file. </param>
        /// <param name="outputFileName">           Filename of the output file. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool Process(BusinessModel businessModel, Guid projectType, string projectFolderRoot, string templateFile, string outputFileName, IGeneratorConfiguration generatorConfiguration)
        {
            var configObject = (ConfigObject) generatorConfiguration.KeyValuePairs["ConfigObject"];
            var outputFile = new FileInfo(outputFileName);

            if (outputFile.Exists)
            {
                outputFile.MakeWritable();
                outputFile.Delete();
            }

            using (var writer = new StreamWriter(outputFile.OpenWrite()))
            {
                writer.WriteJson(businessModel.TopLevelObject, Newtonsoft.Json.Formatting.Indented, new CamelCaseNamingStrategy());
            }

            return true;
        }
    }
}
