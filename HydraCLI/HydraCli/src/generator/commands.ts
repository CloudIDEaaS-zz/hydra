import chalk = require("chalk");
import { type } from "os";
import { version } from "process";

export const rendererServerCommands = {
  CONNECT: "connect",
  INITIALIZE_RENDERER: "initialize_renderer",
  LOAD_MODULE: "load_module",
  UPDATE_INSTANCE: "update_instance",
  GET_FULL_SNAPSHOT: "get_full_snapshot",
  PING: "ping",
  TERMINATE: "terminate",
};

export const generateTargetDefinitions = [
  {
    name: "app",
    type: String,
    description:
      "Generates a fully funtional application. Previous steps are required.  See online help.",
  },
  {
    name: "workspace",
    type: String,
    description:
      "Generates a workspace, i.e. for .NET, a Visual Studio solution.",
  },
  {
    name: "businessmodel",
    type: String,
    description: "Generates a business model.",
  },
  {
    name: "entities",
    type: String,
    description:
      "Generates either an entities json file or if one exists, entity model classes in the workspace.",
  },
];

export const generateDefinitions = [
  {
    name: "target",
    defaultOption: true,
    description: "The target for the generate command.",
    innerDefinitions: generateTargetDefinitions,
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
    description:
      "The starter template file for either the business model or entity domain model. For default template, specify 'default'",
  },
  {
    name: "businessmodel",
    type: String,
    defaultValue: "",
    description:
      "The business model json input file for processing the entity domain model.",
  },
  {
    name: "json",
    type: String,
    defaultValue: "",
    description:
      "The json input file for processing the business model or entity domain model.",
  },
];

export const setPackagesDefinitions = [
  {
    name: "packageListArg",
    type: String,
    defaultOption: true,
    description:
      "List of packages in comma delimited format.  Package format: <packageName>@<version>.",
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

export const setDefinitions = [
  {
    name: "name",
    type: String,
    defaultOption: true,
    description: "The name of the working directory to set.",
  },
];

export const startDefinitions = [
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

export const buildDefinitions = [
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

export const platformDefinitions = [
  {
    name: "platform",
    type: String,
    defaultOption: true,
    description: "Name of the platform (i.e. android, ios, windows).",
  },
];
export const resourceDefinitions = [
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

export const mainDefinitions = [
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
    description:
      "Used to skip install of the Ionic Framework.  Applies to 'start' command.",
  },
  {
    name: "skipInstalls",
    type: Boolean,
    defaultValue: false,
    description:
      "Used to skip install of npm packages.  Applies to 'generate' command.",
  },
  {
    name: "showDesigner",
    type: String,
    description:
      "Provides a designer UI to manage themes and resources to be used later in the code generation process.",
  },
  {
    name: "addResource",
    type: String,
    description:
      "Adds a resource to be used later in the code generation process.",
    innerDefinitions: resourceDefinitions,
  },
  {
    name: "start",
    type: String,
    description:
      "Creates a front-end JavaScript framework starter implementation.  This will not run on its own.",
  },
  {
    name: "generate",
    type: String,
    description:
      "Used to generate various artifacts. Is the primary code generation command.",
    innerDefinitions: generateDefinitions,
  },
  {
    name: "setPackages",
    type: String,
    description:
      "Sets packages in package.json from the comma delimited list, and optionally clears packages",
    innerDefinitions: setPackagesDefinitions,
  },
  {
    name: "build",
    type: String,
    description: "Builds the front-end app.",
    innerDefinitions: buildDefinitions,
  },
  {
    name: "add",
    type: String,
    description: "Adds a platform to the front-end app.",
    innerDefinitions: platformDefinitions,
  },
  {
    name: "remove",
    type: String,
    description: "Removes a platform from the front-end app.",
    innerDefinitions: platformDefinitions,
  },
  {
    name: "update",
    type: String,
    description: "Updates a platform in the front-end app.",
    innerDefinitions: platformDefinitions,
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
    description:
      "Cleans a directory in the front-end location, leaving the package.json, but with Hydra specific data removed.",
  },
  {
    name: "set",
    type: String,
    description:
      "Sets the Hydra current working directory, used by the Package Cache Status utility.",
    innerDefinitions: setDefinitions,
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
    description:
      "Runs a no-impact test.  Tests the full connectivity of Hydra including the local Package Cache service.",
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
    description:
      "Runs command without actually generating files.  Used for testing.",
  },
];

export const g = chalk.rgb(10, 126, 121);
export const b = chalk.rgb(30, 30, 30);
export const w = chalk.white;
export const CLI_TITLE = chalk.bold.underline("Hydra CLI");
export const CLI_DESCRIPTION = "Hydra is an application generation product";
export const CLI_USAGE = "Usage: `hydra <command> [options ...]`";
export const CLIENT_VERSION = `Hydra ${version}, Copyright 2020 CloudIDEaaS`;
export const WEB_SITE = "http://www.cloudideaas.com/hydra";
export const HELP_HEADER = `

                ${b(".,/(%%&&&&%(*.")}      ${CLI_TITLE}  
        ${b(",/#%&@@&&&%%%#####")}${b(",,,.")}        
  ${b("./%&%%#")}${g("(((((((((((((((((.")}         ${CLI_DESCRIPTION}
${b("#")}${g("(((((((((((((((((((((((((.")}           
${b("#")}${g("((((((((((((((((((((((((((.")}        ${CLI_USAGE}
${b("#")}${g("((((((((((((((")}${b("##%%&&&&&&&%%%##/")}
${b("#")}${g("((((((((((")}${b("#%##")}${g(
  "((((((((((((("
)}        ${CLIENT_VERSION}
${b("#")}${g("(((((((((((((((((((((((((((.")}       ${WEB_SITE}
`;
