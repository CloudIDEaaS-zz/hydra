import "./modules/utils/extensions";
const path = require('path');
const fs = require('fs');
const child_process = require("child_process");
const { Env, Target, ExpandedForm } = require('windows-environment');
import { exec, ChildProcess } from "child_process";
import { Socket } from "net";
const colors = require('colors/safe');

export class HydraInstaller {

    static installer: HydraInstaller;
    stdout: Socket;
    stderr: Socket;
    logOutputToConsole: any;

    constructor() {
        this.stdout = <Socket>process.stdout;
        this.stderr = <Socket>process.stderr;
      
    }

    public static install() {

        let installer: HydraInstaller;

        if (HydraInstaller.installer === undefined) {
            HydraInstaller.installer = new HydraInstaller();
        }

        installer = HydraInstaller.installer;
        installer.install();
    }

    uninstall() {

        let installerPath : string;
        let commandLine : string;
        let installerProcess: ChildProcess;

        this.writeLine("Installing Hydra Bundle for Windows");

        installerPath = path.join(__dirname, "\\..\\install\\Hydra.Installer.exe");

        if (!fs.existsSync(installerPath)) {
            this.writeWarning(`Could not find installer at ${ installerPath }, you can download installer from https://marketplace.visualstudio.com/items?itemName=CloudIDEaaS.Hydra`);
        }         

        commandLine = `${installerPath}`;

        this.writeLine(`Running command ${ commandLine } /uninstall`);

        installerProcess = exec(commandLine);

        installerProcess.stderr.on("data", (e) => {
            this.stderr.writeLine(e.toString());
        });

        installerProcess.on("close",() => {

            this.stdout.writeLine(`Installer exited with exit code ${ installerProcess.exitCode }`);
            process.exit(0);

        });
    }

    install() {

        let installerPath : string;
        let commandLine : string;
        let installerProcess: ChildProcess;
        let programFilesPath : string;
        let generatorApp : string;
        let hydraSolutionPath = <string> process.env.HYDRASOLUTIONPATH;

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
            this.writeWarning(`Could not find installer at ${ installerPath }, you can download installer from https://marketplace.visualstudio.com/items?itemName=CloudIDEaaS.Hydra`);
        }         

        commandLine = `${installerPath}`;

        this.writeLine(`Running command ${ commandLine } `);

        installerProcess = exec(commandLine);

        installerProcess.stderr.on("data", (e) => {
            this.stderr.writeLine(e.toString());
        });

        installerProcess.on("close",() => {

            this.stdout.writeLine(`Installer exited with exit code ${ installerProcess.exitCode }`);
            process.exit(0);

        });
    }
  
    writeLine(output : string) {

        this.stdout.writeLine(output);
        if (this.logOutputToConsole) {
            console.log(output);
        }
    }
    
    write(output : string) {
        this.stdout.write(output);
        if (this.logOutputToConsole) {
            console.log(output);
        }
    }
    
    writeConsole(output : string) {
        if (this.logOutputToConsole) {
            console.log(output);
        }
    }

    writeError(output : string) {
        let coloredOutput = colors.red(output);
        this.stderr.writeLine(coloredOutput);
        if (this.logOutputToConsole) {
            console.log(coloredOutput);
        }
    }

    writeWarning(output : string) {
        let coloredOutput = colors.yellow(output);
        this.stderr.writeLine(coloredOutput);
        if (this.logOutputToConsole) {
            console.log(coloredOutput);
        }
    }
    
    writeSuccess(output : string) {
        let coloredOutput = colors.green(output);
        this.stdout.writeLine(coloredOutput);
        if (this.logOutputToConsole) {
            console.log(coloredOutput);
        }
    }
}