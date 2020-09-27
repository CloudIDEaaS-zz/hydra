"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.HydraInstaller = void 0;
require("./modules/utils/extensions");
const path = require('path');
const fs = require('fs');
const child_process = require("child_process");
const { Env, Target, ExpandedForm } = require('windows-environment');
const child_process_1 = require("child_process");
const colors = require('colors/safe');
class HydraInstaller {
    constructor() {
        this.stdout = process.stdout;
        this.stderr = process.stderr;
    }
    static install() {
        let installer;
        if (HydraInstaller.installer === undefined) {
            HydraInstaller.installer = new HydraInstaller();
        }
        installer = HydraInstaller.installer;
        installer.install();
    }
    uninstall() {
        let installerPath;
        let commandLine;
        let installerProcess;
        this.writeLine("Installing Hydra Bundle for Windows");
        installerPath = path.join(__dirname, "\\..\\install\\Hydra.Installer.exe");
        if (!fs.existsSync(installerPath)) {
            this.writeWarning(`Could not find installer at ${installerPath}, you can download installer from https://marketplace.visualstudio.com/items?itemName=CloudIDEaaS.Hydra`);
        }
        commandLine = `${installerPath}`;
        this.writeLine(`Running command ${commandLine} /uninstall`);
        installerProcess = child_process_1.exec(commandLine);
        installerProcess.stderr.on("data", (e) => {
            this.stderr.writeLine(e.toString());
        });
        installerProcess.on("close", () => {
            this.stdout.writeLine(`Installer exited with exit code ${installerProcess.exitCode}`);
            process.exit(0);
        });
    }
    install() {
        let installerPath;
        let commandLine;
        let installerProcess;
        let programFilesPath;
        let generatorApp;
        let hydraSolutionPath = process.env.HYDRASOLUTIONPATH;
        programFilesPath = process.env["PROGRAMFILES(x86)"];
        if (!programFilesPath) {
            programFilesPath = process.env["PROGRAMFILES"];
        }
        generatorApp = path.join(programFilesPath, "\\CloudIDEaaS\\Hydra\\ApplicationGenerator.exe");
        if (fs.existsSync(generatorApp)) {
            return;
        }
        this.writeLine("Installing Hydra Bundle for Windows");
        installerPath = path.join(__dirname, "\\..\\install\\Hydra.Installer.exe");
        if (!fs.existsSync(installerPath)) {
            this.writeWarning(`Could not find installer at ${installerPath}, you can download installer from https://marketplace.visualstudio.com/items?itemName=CloudIDEaaS.Hydra`);
        }
        commandLine = `${installerPath}`;
        this.writeLine(`Running command ${commandLine} `);
        installerProcess = child_process_1.exec(commandLine);
        installerProcess.stderr.on("data", (e) => {
            this.stderr.writeLine(e.toString());
        });
        installerProcess.on("close", () => {
            this.stdout.writeLine(`Installer exited with exit code ${installerProcess.exitCode}`);
            process.exit(0);
        });
    }
    writeLine(output) {
        this.stdout.writeLine(output);
        if (this.logOutputToConsole) {
            console.log(output);
        }
    }
    write(output) {
        this.stdout.write(output);
        if (this.logOutputToConsole) {
            console.log(output);
        }
    }
    writeConsole(output) {
        if (this.logOutputToConsole) {
            console.log(output);
        }
    }
    writeError(output) {
        let coloredOutput = colors.red(output);
        this.stderr.writeLine(coloredOutput);
        if (this.logOutputToConsole) {
            console.log(coloredOutput);
        }
    }
    writeWarning(output) {
        let coloredOutput = colors.yellow(output);
        this.stderr.writeLine(coloredOutput);
        if (this.logOutputToConsole) {
            console.log(coloredOutput);
        }
    }
    writeSuccess(output) {
        let coloredOutput = colors.green(output);
        this.stdout.writeLine(coloredOutput);
        if (this.logOutputToConsole) {
            console.log(coloredOutput);
        }
    }
}
exports.HydraInstaller = HydraInstaller;
//# sourceMappingURL=hydrainstaller.js.map