import chalk = require("chalk");
import { version } from "process";

export const generateTargetDefinitions = [
    { name: 'app', type: String, description: "Generates a fully funtional application. Previous steps are required.  See online help." },
    { name: 'workspace', type: String, description: "Generates a workspace, i.e. for .NET, a Visual Studio solution." },
    { name: 'businessmodel', type: String, description: "Generates a business model." },
    { name: 'entities', type: String, description: "Generates either an entities json file or if one exists, entity model classes in the workspace." },
];

export const generateDefinitions = [
    { name: 'target', defaultOption: true, description: "The target for the generate command.", innerDefinitions: generateTargetDefinitions },
    { name: 'logToConsole', type: Boolean, defaultValue: false },
    { name: 'debug', type: Boolean, defaultValue: false },
    { name: 'skipInstalls', type: Boolean, defaultValue: false },
    { name: 'noFileCreation', type: Boolean, defaultValue: false },
    { name: 'appName', type: String, defaultValue: false, description: "The appplication name." },
    { name: 'appDescription', type: String, defaultValue: false, description: "The appplication description." },
    { name: 'template', type: String, defaultValue: "", description: "The starter template file for either the business model or entity domain model. For default template, specify 'default'" },
    { name: 'fromjson', type: String, defaultValue: "", description: "The json input file for the business model or entity domain model." },
];

export const setPackagesDefinitions = [
    { name: 'packageListArg', type: String, defaultOption: true, description: "List of packages in comma delimited format.  Package format: <packageName>@<version>." },
    { name: 'logToConsole', type: Boolean, defaultValue: false },
    { name: 'save-dev', alias: 'd', type: Boolean, defaultValue: false, description: "Saves to devDependencies." },
    { name: 'clear', alias: 'x', type: Boolean, defaultValue: false, description: "Clears all packages." },
];

export const setDefinitions = [
    { name: 'name', type: String, defaultOption: true, description: "The name of the working directory to set." }
];

export const startDefinitions = [
    { name: 'name', type: String, defaultOption: true, description: "Name of the front-end application." },
    { name: 'logToConsole', type: Boolean, defaultValue: false },
    { name: 'skipIonicInstall', type: Boolean, defaultValue: false, description: "Skips installing Ionic." },
];
export const mainDefinitions = [
    { name: 'command', type: String, defaultOption: true, description: "Runs a command (default)." },
    { name: 'debug', type: Boolean, defaultValue: false, description: "Sets developer debug flag." },
    { name: 'logToConsole', type: Boolean, defaultValue: false, description: "Logs information to console.log()." },
    { name: 'skipIonicInstall', type: Boolean, defaultValue: false, description: "Used to skip install of the Ionic Framework.  Applies to 'start' command." },
    { name: 'skipInstalls', type: Boolean, defaultValue: false, description: "Used to skip install of npm packages.  Applies to 'generate' command." },
    { name: 'start', type: String, description: "Creates a front-end JavaScript framework starter implementation.  This will not run on its own." },
    { name: 'generate', type: String, description: "Used to generate various artifacts. Is the primary code generation command.", innerDefinitions: generateDefinitions },
    { name: 'setPackages', type: String, description: "Sets packages in package.json from the comma delimited list, and optionally clears packages", innerDefinitions: setPackagesDefinitions },    
    { name: 'build', type: String, description: "Builds the front-end app." },    
    { name: 'delete', type: String, description: "Deletes a directory in the front-end location." },
    { name: 'clean', type: String, description: "Cleans a directory in the front-end location, leaving the package.json, but with Hydra specific data removed." },    
    { name: 'set', type: String, description: "Sets the Hydra current working directory, used by the Package Cache Status utility.", innerDefinitions: setDefinitions },    
    { name: 'test', type: String, description: "Runs a no-impact test.  Tests the full connectivity of Hydra including the local Package Cache service." },    
    { name: 'version', type: Boolean, defaultValue: false, description: "Shows the version of Hydra." },
    { name: 'help', type: Boolean, defaultValue: false, description: "Displays help." },
    { name: 'noFileCreation', type: Boolean, defaultValue: false, description: "Runs command without actually generating files.  Used for testing." },
];

export const g = chalk.rgb(10, 126, 121);
export const b = chalk.rgb(30, 30, 30);
export const w = chalk.white;
export const CLI_TITLE = chalk.bold.underline('Hydra CLI');
export const CLI_DESCRIPTION = 'Hydra is an application generation product';
export const CLI_USAGE = 'Usage: \`hydra <command> [options ...]\`';
export const CLIENT_VERSION = `Hydra ${ version }, Copyright 2020 CloudIDEaaS`;
export const WEB_SITE = "http://www.cloudideaas.com/hydra";
export const HELP_HEADER = `

                ${b('.,/(%%&&&&%(*.')}      ${CLI_TITLE}  
        ${b(',/#%&@@&&&%%%#####')}${b(',,,.')}        
  ${b('./%&%%#')}${g('(((((((((((((((((.')}         ${CLI_DESCRIPTION}
${b('#')}${g('(((((((((((((((((((((((((.')}           
${b('#')}${g('((((((((((((((((((((((((((.')}        ${CLI_USAGE}
${b('#')}${g('((((((((((((((')}${b('##%%&&&&&&&%%%##/')}
${b('#')}${g('((((((((((')}${b('#%##')}${g('(((((((((((((')}        ${CLIENT_VERSION}
${b('#')}${g('(((((((((((((((((((((((((((.')}       ${WEB_SITE}
`;