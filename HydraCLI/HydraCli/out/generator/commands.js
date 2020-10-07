"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.HELP_HEADER = exports.WEB_SITE = exports.CLIENT_VERSION = exports.CLI_USAGE = exports.CLI_DESCRIPTION = exports.CLI_TITLE = exports.w = exports.b = exports.g = exports.mainDefinitions = exports.startDefinitions = exports.setDefinitions = exports.setPackagesDefinitions = exports.generateDefinitions = exports.generateTargetDefinitions = void 0;
const chalk = require("chalk");
const process_1 = require("process");
exports.generateTargetDefinitions = [
    { name: 'app', type: String, description: "Generates a fully funtional application. Previous steps are required.  See online help." },
    { name: 'workspace', type: String, description: "Generates a workspace, i.e. for .NET, a Visual Studio solution." },
    { name: 'businessmodel', type: String, description: "Generates a business model." },
    { name: 'entities', type: String, description: "Generates either an entities json file or if one exists, entity model classes in the workspace." },
];
exports.generateDefinitions = [
    { name: 'target', defaultOption: true, description: "The target for the generate command.", innerDefinitions: exports.generateTargetDefinitions },
    { name: 'logToConsole', type: Boolean, defaultValue: false },
    { name: 'debug', type: Boolean, defaultValue: false },
    { name: 'skipInstalls', type: Boolean, defaultValue: false },
    { name: 'noFileCreation', type: Boolean, defaultValue: false },
    { name: 'appName', type: String, defaultValue: false, description: "The appplication name." },
    { name: 'template', type: String, defaultValue: "", description: "The starter template file for either the business model or entity domain model." },
    { name: 'json', type: String, defaultValue: false, description: "The json input file for the business model or entity domain model." },
];
exports.setPackagesDefinitions = [
    { name: 'packageListArg', type: String, defaultOption: true, description: "List of packages in comma delimited format.  Package format: <packageName>@<version>." },
    { name: 'logToConsole', type: Boolean, defaultValue: false },
    { name: 'save-dev', alias: 'd', type: Boolean, defaultValue: false, description: "Saves to devDependencies." },
    { name: 'clear', alias: 'x', type: Boolean, defaultValue: false, description: "Clears all packages." },
];
exports.setDefinitions = [
    { name: 'name', type: String, defaultOption: true, description: "The name of the working directory to set." }
];
exports.startDefinitions = [
    { name: 'name', type: String, defaultOption: true, description: "Name of the front-end application." },
    { name: 'logToConsole', type: Boolean, defaultValue: false },
    { name: 'skipIonicInstall', type: Boolean, defaultValue: false, description: "Skips installing Ionic." },
];
exports.mainDefinitions = [
    { name: 'command', type: String, defaultOption: true, description: "Runs a command (default)." },
    { name: 'debug', type: Boolean, defaultValue: false, description: "Sets developer debug flag." },
    { name: 'logToConsole', type: Boolean, defaultValue: false, description: "Logs information to console.log()." },
    { name: 'skipIonicInstall', type: Boolean, defaultValue: false, description: "Used to skip install of the Ionic Framework.  Applies to 'start' command." },
    { name: 'skipInstalls', type: Boolean, defaultValue: false, description: "Used to skip install of npm packages.  Applies to 'generate' command." },
    { name: 'start', type: String, description: "Creates a front-end JavaScript framework starter implementation.  This will not run on its own." },
    { name: 'generate', type: String, description: "Used to generate various artifacts. Is the primary code generation command.", innerDefinitions: exports.generateDefinitions },
    { name: 'setPackages', type: String, description: "Sets packages in package.json from the comma delimited list, and optionally clears packages", innerDefinitions: exports.setPackagesDefinitions },
    { name: 'build', type: String, description: "Builds the front-end app." },
    { name: 'delete', type: String, description: "Deletes a directory in the front-end location." },
    { name: 'clean', type: String, description: "Cleans a directory in the front-end location, leaving the package.json, but with Hydra specific data removed." },
    { name: 'set', type: String, description: "Sets the Hydra current working directory, used by the Package Cache Status utility.", innerDefinitions: exports.setDefinitions },
    { name: 'test', type: String, description: "Runs a no-impact test.  Tests the full connectivity of Hydra including the local Package Cache service." },
    { name: 'version', type: Boolean, defaultValue: false, description: "Shows the version of Hydra." },
    { name: 'help', type: Boolean, defaultValue: false, description: "Displays help." },
    { name: 'noFileCreation', type: Boolean, defaultValue: false, description: "Runs command without actually generating files.  Used for testing." },
];
exports.g = chalk.rgb(10, 126, 121);
exports.b = chalk.rgb(30, 30, 30);
exports.w = chalk.white;
exports.CLI_TITLE = chalk.bold.underline('Hydra CLI');
exports.CLI_DESCRIPTION = 'Hydra is an application generation product';
exports.CLI_USAGE = 'Usage: \`hydra <command> [options ...]\`';
exports.CLIENT_VERSION = `Hydra ${process_1.version}, Copyright 2020 CloudIDEaaS`;
exports.WEB_SITE = "http://www.cloudideaas.com/hydra";
exports.HELP_HEADER = `

                ${exports.b('.,/(%%&&&&%(*.')}      ${exports.CLI_TITLE}  
        ${exports.b(',/#%&@@&&&%%%#####')}${exports.b(',,,.')}        
  ${exports.b('./%&%%#')}${exports.g('(((((((((((((((((.')}         ${exports.CLI_DESCRIPTION}
${exports.b('#')}${exports.g('(((((((((((((((((((((((((.')}           
${exports.b('#')}${exports.g('((((((((((((((((((((((((((.')}        ${exports.CLI_USAGE}
${exports.b('#')}${exports.g('((((((((((((((')}${exports.b('##%%&&&&&&&%%%##/')}
${exports.b('#')}${exports.g('((((((((((')}${exports.b('#%##')}${exports.g('(((((((((((((')}        ${exports.CLIENT_VERSION}
${exports.b('#')}${exports.g('(((((((((((((((((((((((((((.')}       ${exports.WEB_SITE}
`;
//# sourceMappingURL=commands.js.map