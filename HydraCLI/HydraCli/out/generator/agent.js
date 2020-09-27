"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ApplicationGeneratorAgent = void 0;
const commandPacket_1 = require("./commandPacket");
const child_process_1 = require("child_process");
const path = require("path");
require("../modules/utils/extensions");
const events_1 = require("events");
const api_1 = require("./api/api");
const fs = require("fs");
class ApplicationGeneratorAgent {
    constructor() {
        this.stdout = process.stdout;
        this.onError = new events_1.EventEmitter();
        this.api = new api_1.Api();
    }
    initialize(debug = false) {
        let programFilesPath;
        let generatorApp;
        let commandLine;
        let hydraSolutionPath = process.env.HYDRASOLUTIONPATH;
        programFilesPath = process.env["PROGRAMFILES(x86)"];
        if (!programFilesPath) {
            programFilesPath = process.env["PROGRAMFILES"];
        }
        generatorApp = path.join(programFilesPath, "\\CloudIDEaaS\\Hydra\\ApplicationGenerator.exe");
        this.stdout.writeLine("Launching Hydra");
        if (!fs.existsSync(generatorApp)) {
            generatorApp = path.join(hydraSolutionPath, "\\ApplicationGenerator\\bin\\Debug\\ApplicationGenerator.exe");
        }
        if (!fs.existsSync(generatorApp)) {
            this.stdout.writeLine("You must install Hydra to run this command.");
            throw new Error("Application not fully installed.");
        }
        commandLine = `"${generatorApp}" -waitForInput`;
        if (debug) {
            commandLine += " -debug";
        }
        this.generatorProcess = child_process_1.exec(commandLine);
        this.generatorProcess.stderr.on("data", (e) => {
            this.onError.emit("onError", e.toString());
        });
        this.generatorProcess.on("close", () => {
            this.stdout.writeLine("Exited");
        });
    }
    launchCacheServer(listener) {
        var commandLine = "verdaccio";
        this.stdout.writeLine("Launching Cache Server");
        this.cacheServerProcess = child_process_1.exec(commandLine);
        this.cacheServerProcess.stderr.on("data", (e) => {
            this.onError.emit("onError", e.toString());
        });
        this.cacheServerProcess.stdout.readText(listener);
    }
    stopCacheServer(listener) {
        this.cacheServerProcess.kill();
        this.cacheServerProcess.on("close", () => {
            this.stdout.writeLine("Cache server shut down");
        });
    }
    dispose(listener) {
        if (this.generatorProcess) {
            this.sendSimpleCommand("terminate", listener);
        }
        else {
            listener(null);
        }
    }
    stopListening() {
        this.generatorProcess.stdout.removeListener("data", this.textListener);
    }
    sendSimpleCommand(command, listener = null, ...args) {
        if (this.generatorProcess.killed) {
            this.stdout.writeLine("ApplicationGeneratorAgent process has exited!");
            listener(null);
            throw new Error("ApplicationGeneratorAgent process has exited!");
        }
        this.write(command, ...args);
        if (listener) {
            this.read(listener);
        }
    }
    getInstallFromCacheStatus(mode, listener) {
        let promise = this.api.get("GetInstallFromCacheStatus", { key: "mode", value: mode });
        promise.then((status) => {
            listener(status);
        }).catch((reason) => {
            let status = {
                StatusText: reason,
                StatusIsError: true
            };
            listener(status);
        });
    }
    getVersion(mode, listener) {
        let promise = this.api.get("GetInstallFromCacheStatus", { key: "mode", value: mode });
        promise.then((status) => {
            listener(status);
        }).catch((reason) => {
            let status = {
                StatusText: reason,
                StatusIsError: true
            };
            listener(status);
        });
    }
    generateApp(entitiesProjectPath, servicesProjectPath, packageCachePath, noFileCreation, generatorPass, listener = null) {
        let commandObject = new commandPacket_1.CommandPacket("request", "generate", [
            { "Kind": "app" },
            { "EntitiesProjectPath": entitiesProjectPath },
            { "ServicesProjectPath": servicesProjectPath },
            { "GeneratorPass": generatorPass },
            { "PackageCachePath": packageCachePath },
            { "NoFileCreation": noFileCreation }
        ], new Date(Date.now()));
        this.generatorProcess.stdin.writeJson(commandObject);
        if (listener) {
            this.textListener = listener;
            this.readText(this.textListener);
        }
    }
    generateBusinessModel(templateFile, noFileCreation, generatorPass, listener = null) {
        let commandObject = new commandPacket_1.CommandPacket("request", "generate", [
            { "Kind": "businessModel" },
            { "TemplateFile": templateFile },
            { "GeneratorPass": generatorPass },
            { "NoFileCreation": noFileCreation }
        ], new Date(Date.now()));
        this.generatorProcess.stdin.writeJson(commandObject);
        if (listener) {
            this.textListener = listener;
            this.readText(this.textListener);
        }
    }
    generateEntities(templateFile, jsonFile, businessModelFile, entitiesProjectPath, noFileCreation, generatorPass, listener = null) {
        let commandObject = new commandPacket_1.CommandPacket("request", "generate", [
            { "Kind": "entities" },
            { "TemplateFile": templateFile },
            { "JsonFile": jsonFile },
            { "BusinessModelFile": businessModelFile },
            { "EntitiesProjectPath": entitiesProjectPath },
            { "GeneratorPass": generatorPass },
            { "NoFileCreation": noFileCreation }
        ], new Date(Date.now()));
        this.generatorProcess.stdin.writeJson(commandObject);
        if (listener) {
            this.textListener = listener;
            this.readText(this.textListener);
        }
    }
    generateWorkspace(appName, noFileCreation, generatorPass, listener = null) {
        let commandObject = new commandPacket_1.CommandPacket("request", "generate", [
            { "Kind": "workspace" },
            { "AppName": appName },
            { "GeneratorPass": generatorPass },
            { "NoFileCreation": noFileCreation }
        ], new Date(Date.now()));
        this.generatorProcess.stdin.writeJson(commandObject);
        if (listener) {
            this.textListener = listener;
            this.readText(this.textListener);
        }
    }
    write(command, ...args) {
        let commandObject;
        if (args.length) {
            let argsArray = [];
            args.forEach(a => {
                let item = new Object();
                item[a.key] = a.value;
                argsArray.push(item);
            });
            commandObject = new commandPacket_1.CommandPacket("request", command, argsArray, new Date(Date.now()));
        }
        else {
            commandObject = new commandPacket_1.CommandPacket("request", command, null, new Date(Date.now()));
        }
        this.generatorProcess.stdin.writeJson(commandObject);
    }
    readText(listener) {
        this.generatorProcess.stdout.readText(listener);
    }
    read(listener) {
        this.generatorProcess.stdout.readJson(listener);
    }
}
exports.ApplicationGeneratorAgent = ApplicationGeneratorAgent;
//# sourceMappingURL=agent.js.map