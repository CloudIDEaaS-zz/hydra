"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.HELP_HEADER = exports.WEB_SITE = exports.CLIENT_VERSION = exports.CLI_USAGE = exports.CLI_DESCRIPTION = exports.CLI_TITLE = exports.w = exports.b = exports.g = exports.mainDefinitions = exports.resourceDefinitions = exports.platformDefinitions = exports.buildDefinitions = exports.startDefinitions = exports.setDefinitions = exports.setPackagesDefinitions = exports.generateDefinitions = exports.generateTargetDefinitions = exports.rendererServerCommands = void 0;
const chalk = require("chalk");
const process_1 = require("process");
exports.rendererServerCommands = {
    CONNECT: "connect",
    INITIALIZE_RENDERER: "initialize_renderer",
    LOAD_MODULE: "load_module",
    UPDATE_INSTANCE: "update_instance",
    GET_FULL_SNAPSHOT: "get_full_snapshot",
    PING: "ping",
    TERMINATE: "terminate",
};
exports.generateTargetDefinitions = [
    {
        name: "app",
        type: String,
        description: "Generates a fully funtional application. Previous steps are required.  See online help.",
    },
    {
        name: "workspace",
        type: String,
        description: "Generates a workspace, i.e. for .NET, a Visual Studio solution.",
    },
    {
        name: "businessmodel",
        type: String,
        description: "Generates a business model.",
    },
    {
        name: "entities",
        type: String,
        description: "Generates either an entities json file or if one exists, entity model classes in the workspace.",
    },
];
exports.generateDefinitions = [
    {
        name: "target",
        defaultOption: true,
        description: "The target for the generate command.",
        innerDefinitions: exports.generateTargetDefinitions,
    },
    {
        name: "logServiceMessages",
        type: Boolean,
        defaultValue: false,
        description: "Saves restful service messages in the Logs folder.",
    },
    { name: "logToConsole", type: Boolean, defaultValue: false },
    {
        name: "logClient",
        type: Boolean,
        defaultValue: false,
        description: "Saves client processing in the Logs folder.",
    },
    { name: "debug", type: Boolean, defaultValue: false },
    {
        name: "pause",
        type: Number,
        defaultValue: 0,
        description: "Pauses to allow for debug attach.",
    },
    { name: "skipInstalls", type: Boolean, defaultValue: false },
    { name: "noFileCreation", type: Boolean, defaultValue: false },
    {
        name: "appName",
        type: String,
        defaultValue: "",
        description: "The application name.",
    },
    {
        name: "appDescription",
        type: String,
        defaultValue: "",
        description: "The appplication description.",
    },
    {
        name: "organizationName",
        type: String,
        defaultValue: "",
        description: "The organization name.",
    },
    {
        name: "template",
        type: String,
        defaultValue: "",
        description: "The starter template file for either the business model or entity domain model. For default template, specify 'default'",
    },
    {
        name: "businessmodel",
        type: String,
        defaultValue: "",
        description: "The business model json input file for processing the entity domain model.",
    },
    {
        name: "json",
        type: String,
        defaultValue: "",
        description: "The json input file for processing the business model or entity domain model.",
    },
];
exports.setPackagesDefinitions = [
    {
        name: "packageListArg",
        type: String,
        defaultOption: true,
        description: "List of packages in comma delimited format.  Package format: <packageName>@<version>.",
    },
    { name: "logToConsole", type: Boolean, defaultValue: false },
    {
        name: "save-dev",
        alias: "d",
        type: Boolean,
        defaultValue: false,
        description: "Saves to devDependencies.",
    },
    {
        name: "clear",
        alias: "x",
        type: Boolean,
        defaultValue: false,
        description: "Clears all packages.",
    },
];
exports.setDefinitions = [
    {
        name: "name",
        type: String,
        defaultOption: true,
        description: "The name of the working directory to set.",
    },
];
exports.startDefinitions = [
    {
        name: "name",
        type: String,
        defaultOption: true,
        description: "Name of the front-end application.",
    },
    { name: "logToConsole", type: Boolean, defaultValue: false },
    { name: "debug", type: Boolean, defaultValue: false },
    {
        name: "pause",
        type: Number,
        description: "Pauses to allow for debug attach.",
    },
    { name: "useAgent", type: Boolean, defaultValue: false },
    {
        name: "skipIonicInstall",
        type: Boolean,
        defaultValue: false,
        description: "Skips installing Ionic.",
    },
    {
        name: "logServiceMessages",
        type: Boolean,
        defaultValue: false,
        description: "Saves restful service messages in the Logs folder.",
    },
];
exports.buildDefinitions = [
    {
        name: "platform",
        type: String,
        defaultOption: true,
        description: "Name of the platform for build (i.e. android, ios, windows).",
    },
    {
        name: "prod",
        type: Boolean,
        defaultValue: false,
        description: "Build type of prod.",
    },
    {
        name: "release",
        type: Boolean,
        defaultValue: false,
        description: "Build target of release.",
    },
];
exports.platformDefinitions = [
    {
        name: "platform",
        type: String,
        defaultOption: true,
        description: "Name of the platform (i.e. android, ios, windows).",
    },
];
exports.resourceDefinitions = [
    {
        name: "appDescription",
        type: String,
        defaultValue: "",
        description: "Adds an app description from command line or from text file",
    },
    {
        name: "splashScreen",
        type: String,
        description: "Adds a splash screen image file",
    },
    { name: "logo", type: String, description: "Adds a logo image file" },
    { name: "icon", type: String, description: "Adds an icon image file" },
    {
        name: "license",
        type: String,
        defaultValue: "",
        description: "Adds an license hyperlink or text file",
    },
    {
        name: "aboutBanner",
        type: String,
        description: "Adds an about banner image file",
    },
    {
        name: "debug",
        type: Boolean,
        defaultValue: false,
        description: "Sets developer debug flag.",
    },
    {
        name: "logToConsole",
        type: Boolean,
        defaultValue: false,
        description: "Logs information to console.log().",
    },
];
exports.mainDefinitions = [
    {
        name: "command",
        type: String,
        defaultOption: true,
        description: "Runs a command (default).",
    },
    {
        name: "debug",
        type: Boolean,
        defaultValue: false,
        description: "Sets developer debug flag.",
    },
    {
        name: "pause",
        type: Number,
        description: "Pauses to allow for debug attach.",
    },
    {
        name: "logServiceMessages",
        type: Boolean,
        defaultValue: false,
        description: "Saves restful service messages in the Logs folder.",
    },
    {
        name: "logToConsole",
        type: Boolean,
        defaultValue: false,
        description: "Logs information to console.log().",
    },
    {
        name: "logClient",
        type: Boolean,
        defaultValue: false,
        description: "Saves client processing in the Logs folder.",
    },
    {
        name: "skipIonicInstall",
        type: Boolean,
        defaultValue: false,
        description: "Used to skip install of the Ionic Framework.  Applies to 'start' command.",
    },
    {
        name: "skipInstalls",
        type: Boolean,
        defaultValue: false,
        description: "Used to skip install of npm packages.  Applies to 'generate' command.",
    },
    {
        name: "showDesigner",
        type: String,
        description: "Provides a designer UI to manage themes and resources to be used later in the code generation process.",
    },
    {
        name: "addResource",
        type: String,
        description: "Adds a resource to be used later in the code generation process.",
        innerDefinitions: exports.resourceDefinitions,
    },
    {
        name: "start",
        type: String,
        description: "Creates a front-end JavaScript framework starter implementation.  This will not run on its own.",
    },
    {
        name: "generate",
        type: String,
        description: "Used to generate various artifacts. Is the primary code generation command.",
        innerDefinitions: exports.generateDefinitions,
    },
    {
        name: "setPackages",
        type: String,
        description: "Sets packages in package.json from the comma delimited list, and optionally clears packages",
        innerDefinitions: exports.setPackagesDefinitions,
    },
    {
        name: "build",
        type: String,
        description: "Builds the front-end app.",
        innerDefinitions: exports.buildDefinitions,
    },
    {
        name: "add",
        type: String,
        description: "Adds a platform to the front-end app.",
        innerDefinitions: exports.platformDefinitions,
    },
    {
        name: "remove",
        type: String,
        description: "Removes a platform from the front-end app.",
        innerDefinitions: exports.platformDefinitions,
    },
    {
        name: "update",
        type: String,
        description: "Updates a platform in the front-end app.",
        innerDefinitions: exports.platformDefinitions,
    },
    {
        name: "serve",
        type: String,
        description: "Launches and serves the front-end app.",
    },
    {
        name: "platforms",
        type: String,
        description: "Lists platforms added to the front-end app.",
    },
    {
        name: "delete",
        type: String,
        description: "Deletes a directory in the front-end location.",
    },
    {
        name: "clean",
        type: String,
        description: "Cleans a directory in the front-end location, leaving the package.json, but with Hydra specific data removed.",
    },
    {
        name: "set",
        type: String,
        description: "Sets the Hydra current working directory, used by the Package Cache Status utility.",
        innerDefinitions: exports.setDefinitions,
    },
    { name: "install", type: String, description: "Installs a package." },
    {
        name: "launchRenderer",
        type: String,
        description: "Does real time rendering of stencil components.",
    },
    {
        name: "test",
        type: String,
        description: "Runs a no-impact test.  Tests the full connectivity of Hydra including the local Package Cache service.",
    },
    {
        name: "version",
        type: Boolean,
        defaultValue: false,
        description: "Shows the version of Hydra.",
    },
    {
        name: "launchServices",
        type: Boolean,
        defaultValue: false,
        description: "Launches the web services for the current project.",
    },
    {
        name: "help",
        type: Boolean,
        defaultValue: false,
        description: "Displays help.",
    },
    {
        name: "noFileCreation",
        type: Boolean,
        defaultValue: false,
        description: "Runs command without actually generating files.  Used for testing.",
    },
];
exports.g = chalk.rgb(10, 126, 121);
exports.b = chalk.rgb(30, 30, 30);
exports.w = chalk.white;
exports.CLI_TITLE = chalk.bold.underline("Hydra CLI");
exports.CLI_DESCRIPTION = "Hydra is an application generation product";
exports.CLI_USAGE = "Usage: `hydra <command> [options ...]`";
exports.CLIENT_VERSION = `Hydra ${process_1.version}, Copyright 2020 CloudIDEaaS`;
exports.WEB_SITE = "http://www.cloudideaas.com/hydra";
exports.HELP_HEADER = `

                ${exports.b(".,/(%%&&&&%(*.")}      ${exports.CLI_TITLE}  
        ${exports.b(",/#%&@@&&&%%%#####")}${exports.b(",,,.")}        
  ${exports.b("./%&%%#")}${exports.g("(((((((((((((((((.")}         ${exports.CLI_DESCRIPTION}
${exports.b("#")}${exports.g("(((((((((((((((((((((((((.")}           
${exports.b("#")}${exports.g("((((((((((((((((((((((((((.")}        ${exports.CLI_USAGE}
${exports.b("#")}${exports.g("((((((((((((((")}${exports.b("##%%&&&&&&&%%%##/")}
${exports.b("#")}${exports.g("((((((((((")}${exports.b("#%##")}${exports.g("(((((((((((((")}        ${exports.CLIENT_VERSION}
${exports.b("#")}${exports.g("(((((((((((((((((((((((((((.")}       ${exports.WEB_SITE}
`;
//# sourceMappingURL=commands.js.map