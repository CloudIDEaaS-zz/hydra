"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
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
        let directory = process.env["SystemRoot"];
        let msiExecPath;
        this.writeLine("Installing Hydra ApplicationGenerator");
        msiExecPath = path.join(directory, "msiexec.exe");
        if (fs.existsSync(msiExecPath)) {
            let msiLocation = path.normalize(path.join(__dirname, "\\..\\msi\\Hydra.Installer.msi"));
            let commandLine = `"${msiExecPath} /i" \"${msiLocation}\"`;
            let msiProcess;
            this.writeLine(`Running command ${commandLine} `);
            msiProcess = child_process_1.exec(commandLine);
            msiProcess.stderr.on("data", (e) => {
                this.stderr.writeLine(e.toString());
            });
            msiProcess.on("close", () => {
                this.stdout.writeLine(`Installer exited with exit code ${msiProcess.exitCode}`);
                process.exit(msiProcess.exitCode);
            });
            return;
        }
        else {
            this.writeWarning("Could not find msiexec, you can download installer from https://marketplace.visualstudio.com/items?itemName=CloudIDEaaS.Hydra");
        }
        fs.readdir(directory, (err, items) => {
            items.forEach((f) => {
                let fileName = f;
                let fullName = path.join(directory, fileName);
                fs.stat(fullName, (err, result) => {
                    if (result.isDirectory()) {
                        if (fullName.endsWith("Microsoft Visual Studio")) {
                        }
                    }
                });
            });
        });
    }
    install() {
        let directory = process.env["SystemRoot"];
        let msiExecPath;
        let msiLocation;
        let commandLine;
        let installerProcess;
        this.writeLine("Installing Hydra ApplicationGenerator");
        msiExecPath = path.join(directory, "\\System\\msiexec.exe");
        if (!fs.existsSync(msiExecPath)) {
            msiExecPath = path.join(directory, "\\System32\\msiexec.exe");
            if (!fs.existsSync(msiExecPath)) {
                this.writeWarning("Could not find msiexec, you can download installer from https://marketplace.visualstudio.com/items?itemName=CloudIDEaaS.Hydra");
            }
        }
        msiLocation = path.normalize(path.join(__dirname, "\\..\\msi\\Hydra.Installer.msi"));
        commandLine = `${msiExecPath} /i \"${msiLocation}\"`;
        this.writeLine(`Running command ${commandLine} `);
        installerProcess = child_process_1.exec(commandLine);
        installerProcess.stderr.on("data", (e) => {
            this.stderr.writeLine(e.toString());
        });
        installerProcess.on("close", () => {
            this.stdout.writeLine(`msiexec exited with exit code ${installerProcess.exitCode}`);
            process.exit(installerProcess.exitCode);
        });
        fs.readdir(directory, (err, items) => {
            items.forEach((f) => {
                let fileName = f;
                let fullName = path.join(directory, fileName);
                fs.stat(fullName, (err, result) => {
                    if (result.isDirectory()) {
                        if (fullName.endsWith("Microsoft Visual Studio")) {
                        }
                    }
                });
            });
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