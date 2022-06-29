// file:	GeneratorArgumentsKind.cs
//
// summary:	Implements the generator arguments kind class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   A generator arguments kind. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/1/2021. </remarks>

    public static class GeneratorArgumentsKind
    {
        /// <summary>   The generate workspace. </summary>
        [GeneratorArguments("Initial Setup", "Generates a development workspace for the selected generator handler type.")]
        public const string GenerateWorkspace = "GenerateWorkspace";
        /// <summary>   The generate workspace. Not to be used directly. </summary>
        [GeneratorArguments("Initial Setup", "Generates a development workspace for the selected generator handler type.")]
        public const string GenerateWorkspaceFromHydraCLI = "GenerateWorkspaceFromHydraCLI";
        /// <summary>   The generate application from hydra CLI. </summary>
        [GeneratorArguments("App Generation", "Generates an app for the selected generator handler type.")]
        public const string GenerateAppFromHydraCLI = "GenerateAppFromHydraCLI";
        /// <summary>   The show designer. </summary>
        [GeneratorArguments("Design", "Shows a UI designer. This allows you to set a theme and app defaults.", IsFavorite = true)]
        public const string ShowDesigner = "ShowDesigner";
        /// <summary>   The generate business model. </summary>
        [GeneratorArguments("Default Template", "")]
        public const string GenerateBusinessModelDefault = "GenerateBusinessModelDefault";
        /// <summary>   The generate entities from template. </summary>
        [GeneratorArguments("Default Template", "")]
        public const string GenerateEntitiesFromTemplateDefault = "GenerateEntitiesFromTemplateDefault";
        /// <summary>   The generate entities from template blank. </summary>
        [GeneratorArguments("Blank Template", "")]
        public const string GenerateEntitiesFromTemplateBlank = "GenerateEntitiesFromTemplateBlank";
        /// <summary>   The generate business model blank. </summary>
        [GeneratorArguments("Blank Template", "")]
        public const string GenerateBusinessModelBlank = "GenerateBusinessModelBlank";
        /// <summary>   The generate entities from JSON. </summary>
        [GeneratorArguments("Entities Generation", "")]
        public const string GenerateEntitiesFromJson = "GenerateEntitiesFromJson";
        /// <summary>   The generate application. </summary>
        [GeneratorArguments("App Generation", "")]
        public const string GenerateApp = "GenerateApp";
        /// <summary>   The generate application core. </summary>
        [GeneratorArguments("App Generation", "")]
        public const string GenerateAppCore = "GenerateAppCore";
        /// <summary>   The generate from configuration. </summary>
        [GeneratorArguments("App Generation", "")]
        public const string GenerateFromConfig = "GenerateFromConfig";
        /// <summary>   The generate all. </summary>
        [GeneratorArguments("Blank Template", "")]
        public const string GenerateAllNoFrontendBlank = "GenerateAllNoFrontendBlank";
        /// <summary>   The generate all base frontend blank. </summary>
        [GeneratorArguments("Blank Template", "")]
        public const string GenerateAllStarterFrontendOnlyBlank = "GenerateAllStarterFrontendOnlyBlank";
        /// <summary>   The generate all. </summary>
        [GeneratorArguments("Blank Template", "This generates a complete app based on the 'Blank' template.", GeneratorArgumentsKind.GenerateWorkspace, GeneratorArgumentsKind.GenerateBusinessModelBlank, GeneratorArgumentsKind.GenerateEntitiesFromTemplateBlank, GeneratorArgumentsKind.GenerateEntitiesFromJson, GeneratorArgumentsKind.GenerateStarterAppFrontend, GeneratorArgumentsKind.GenerateAppCore, IsFavorite = true)]
        public const string GenerateAllCoreBlank = "GenerateAllCoreBlank";
        /// <summary>   The generate all core blank launch. </summary>
        [GeneratorArguments("Blank Template", "This generates a complete app based on the 'Blank' template then launches the app in a web browser.", GeneratorArgumentsKind.GenerateWorkspace, GeneratorArgumentsKind.GenerateBusinessModelBlank, GeneratorArgumentsKind.GenerateEntitiesFromTemplateBlank, GeneratorArgumentsKind.GenerateEntitiesFromJson, GeneratorArgumentsKind.GenerateStarterAppFrontend, GeneratorArgumentsKind.GenerateAppCore, IsFavorite = true)]
        public const string GenerateAllCoreBlankAndLaunch = "GenerateAllCoreBlankAndLaunch";
        /// <summary>   The start application frontend. </summary>
        [GeneratorArguments("Hydra", "")]
        public const string GenerateStarterAppFrontend = "GenerateStarterAppFrontend";
        /// <summary>   The generate starter application blank to completion. </summary>
        [GeneratorArguments("Hydra", "This assumes entities are created and therefore generates a start app, creates the complete app.", GeneratorArgumentsKind.GenerateStarterAppFrontend, GeneratorArgumentsKind.GenerateAppCore)]
        public const string GenerateStarterAppToCompletion = "GenerateStarterAppToCompletion";
        /// <summary>   The generate starter to launch. </summary>
        [GeneratorArguments("Hydra", "This assumes entities are created and therefore generates a start app, creates the complete app, then launches the app in a web browser.", GeneratorArgumentsKind.GenerateStarterAppFrontend, GeneratorArgumentsKind.GenerateAppCore)]
        public const string GenerateStarterAppToCompletionAndLaunch = "GenerateStarterAppToCompletionAndLaunch";
        /// <summary>   The test generate application core. </summary>
        [GeneratorArguments("Testing", "")]
        public const string TestGenerateAppCore = "TestGenerateAppCore";
        /// <summary>   The REST service provider. </summary>
        [GeneratorArguments("Testing", "")]
        public const string RestServiceProvider = "RestServiceProvider";
        /// <summary>   The add resource capture image. </summary>
        [GeneratorArguments("Design", "")]
        public const string AddResourceCaptureImage = "AddResourceCaptureImage";
        /// <summary>   The add resource choose color. </summary>
        [GeneratorArguments("Design", "")]
        public const string AddResourceChooseColor = "AddResourceChooseColor";
        /// <summary>   The add resource browse file. </summary>
        [GeneratorArguments("Design", "")]
        public const string AddResourceBrowseFile = "AddResourceBrowseFile";
        /// <summary>   The generate handler argument inputs. </summary>
        [GeneratorArguments("Utility", "")]
        public const string GenerateHandlerArgumentInputs = "GenerateHandlerArgumentInputs";
        /// <summary>   The launch. </summary>
        [GeneratorArguments("Utility", "", IsFavorite = true)]
        public const string LaunchWeb = "LaunchWeb";
    }
}
