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
const dateFormat = require("dateformat");
class ApplicationGeneratorAgent {
    generatorProcess;
    cacheServerProcess;
    stdout;
    textListener;
    onError;
    connected;
    api;
    logPath;
    resourceManager;
    constructor() {
        this.stdout = process.stdout;
        this.onError = new events_1.EventEmitter();
        this.api = new api_1.Api();
    }
    initialize(debug = false, logServiceMessages = false) {
        let programFilesPath;
        let generatorApp;
        let args;
        let hydraSolutionPath = process.env.HYDRASOLUTIONPATH;
        programFilesPath = process.env["PROGRAMFILES(x86)"];
        if (!programFilesPath) {
            programFilesPath = process.env["PROGRAMFILES"];
        }
        generatorApp = path.join(programFilesPath, "\\CloudIDEaaS\\Hydra\\ApplicationGenerator.exe");
        this.stdout.writeLine(this.resourceManager.HydraCli.Invoking_Hydra);
        if (!fs.existsSync(generatorApp)) {
            generatorApp = path.join(hydraSolutionPath, "\\ApplicationGenerator\\bin\\Debug\\ApplicationGenerator.exe");
        }
        if (!fs.existsSync(generatorApp)) {
            this.stdout.writeLine(this.resourceManager.HydraCli.You_must_install + "Hydra" + this.resourceManager.HydraCli.to_run_this_command);
            throw new Error(this.resourceManager.HydraCli.Application_not_fully_installed);
        }
        args = new Array();
        args.push("-waitForInput");
        if (debug) {
            args.push("-debug");
        }
        if (logServiceMessages) {
            this.logPath = path.join(process.cwd(), "Logs\\Messages\\" + dateFormat(new Date(), "yyyymmdd_HHMMss_l"));
            args.push(" -logServiceMessages");
        }
        this.generatorProcess = (0, child_process_1.execFile)(generatorApp, args);
        this.generatorProcess.stderr.on("data", (e) => {
            this.onError.emit("onError", e.toString());
        });
        this.generatorProcess.on("close", () => {
            this.stdout.writeLine("Exited");
        });
    }
    launchCacheServer(listener) {
        var commandLine = "verdaccio";
        this.stdout.writeLine(this.resourceManager.HydraCli.Launching_Cache_Server);
        this.cacheServerProcess = (0, child_process_1.execFile)(commandLine);
        this.cacheServerProcess.stderr.on("data", (e) => {
            this.onError.emit("onError", e.toString());
        });
        this.cacheServerProcess.stdout.readText(this.logPath, listener);
    }
    stopCacheServer(listener) {
        this.cacheServerProcess.kill();
        this.cacheServerProcess.on("close", () => {
            this.stdout.writeLine(this.resourceManager.HydraCli.Cache_server_shut_down);
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
    endProcessing(listener) {
        if (this.generatorProcess) {
            this.sendSimpleCommand("endprocessing", listener);
        }
        else {
            listener(null);
        }
    }
    stopListening() {
        this.generatorProcess.stdout.removeListener("data", this.textListener);
    }
    sendHydraStatus(status, alertLevel = "info") {
        if (this.connected) {
            let commandObject = new commandPacket_1.CommandPacket("request", "sendhydrastatus", [{ Status: status }, { AlertLevel: alertLevel }]);
            this.generatorProcess.stdin.writeJson(this.logPath, commandObject);
        }
    }
    sendSimpleCommand(command, listener = null, ...args) {
        if (this.generatorProcess.killed) {
            this.stdout.writeLine(this.resourceManager.HydraCli.ApplicationGeneratorAgent_process);
            listener(null);
            throw new Error(this.resourceManager.HydraCli.ApplicationGeneratorAgent_process);
        }
        this.write(command, ...args);
        if (listener) {
            this.read(this.logPath, listener);
        }
    }
    getInstallFromCacheStatus(mode, listener) {
        let promise = this.api.get("GetInstallFromCacheStatus", { key: "mode", value: mode });
        promise
            .then((status) => {
            listener(status);
        })
            .catch((reason) => {
            let status = {
                StatusText: reason,
                StatusIsError: true,
            };
            listener(status);
        });
    }
    getVersion(mode, listener) {
        let promise = this.api.get("GetInstallFromCacheStatus", { key: "mode", value: mode });
        promise
            .then((status) => {
            listener(status);
        })
            .catch((reason) => {
            let status = {
                StatusText: reason,
                StatusIsError: true,
            };
            listener(status);
        });
    }
    generateApp(entitiesProjectPath, servicesProjectPath, packageCachePath, noFileCreation, generatorPass, listener = null) {
        let commandObject = new commandPacket_1.CommandPacket("request", "generate", [
            { Kind: "app" },
            { GeneratorHandlerType: "Ionic/Angular" },
            { EntitiesProjectPath: entitiesProjectPath },
            { ServicesProjectPath: servicesProjectPath },
            { GeneratorPass: generatorPass },
            { PackageCachePath: packageCachePath },
            { NoFileCreation: noFileCreation },
        ]);
        this.generatorProcess.stdin.writeJson(this.logPath, commandObject);
        if (listener) {
            this.textListener = listener;
            this.readText(this.logPath, this.textListener);
        }
    }
    generateBusinessModel(templateFile, noFileCreation, generatorPass, listener = null) {
        let commandObject = new commandPacket_1.CommandPacket("request", "generate", [
            { Kind: "businessModel" },
            { GeneratorHandlerType: "Ionic/Angular" },
            { TemplateFile: templateFile },
            { GeneratorPass: generatorPass },
            { NoFileCreation: noFileCreation },
        ]);
        this.generatorProcess.stdin.writeJson(this.logPath, commandObject);
        if (listener) {
            this.textListener = listener;
            this.readText(this.logPath, this.textListener);
        }
    }
    generateEntities(templateFile, jsonFile, businessModelFile, entitiesProjectPath, noFileCreation, generatorPass, listener = null) {
        let commandObject = new commandPacket_1.CommandPacket("request", "generate", [
            { Kind: "entities" },
            { GeneratorHandlerType: "Ionic/Angular" },
            { TemplateFile: templateFile },
            { JsonFile: jsonFile },
            { BusinessModelFile: businessModelFile },
            { EntitiesProjectPath: entitiesProjectPath ?? "" },
            { GeneratorPass: generatorPass },
            { NoFileCreation: noFileCreation },
        ]);
        this.generatorProcess.stdin.writeJson(this.logPath, commandObject);
        if (listener) {
            this.textListener = listener;
            this.readText(this.logPath, this.textListener);
        }
    }
    generateWorkspace(appName, appDescription, organizationName, noFileCreation, generatorPass, listener = null) {
        let commandObject = new commandPacket_1.CommandPacket("request", "generate", [
            { Kind: "workspace" },
            { GeneratorHandlerType: "Ionic/Angular" },
            { AppName: appName },
            { AppDescription: appDescription },
            { OrganizationName: organizationName },
            { GeneratorPass: generatorPass },
            { NoFileCreation: noFileCreation },
        ]);
        this.generatorProcess.stdin.writeJson(this.logPath, commandObject);
        if (listener) {
            this.textListener = listener;
            this.readText(this.logPath, this.textListener);
        }
    }
    generateFrom(appName, appDescription, organizationName, noFileCreation, generatorPass, listener = null) {
        let commandObject = new commandPacket_1.CommandPacket("request", "generate", [
            { Kind: "from" },
            { GeneratorHandlerType: "Ionic/Angular" },
            { AppName: appName },
            { AppDescription: appDescription },
            { OrganizationName: organizationName },
            { GeneratorPass: generatorPass },
            { NoFileCreation: noFileCreation },
        ]);
        this.generatorProcess.stdin.writeJson(this.logPath, commandObject);
        if (listener) {
            this.textListener = listener;
            this.readText(this.logPath, this.textListener);
        }
    }
    write(command, ...args) {
        let commandObject;
        if (args.length) {
            let argsArray = [];
            args.forEach((a) => {
                let item = new Object();
                item[a.key] = a.value;
                argsArray.push(item);
            });
            commandObject = new commandPacket_1.CommandPacket("request", command, argsArray);
        }
        else {
            commandObject = new commandPacket_1.CommandPacket("request", command, null);
        }
        this.generatorProcess.stdin.writeJson(this.logPath, commandObject);
    }
    readText(logPath = null, listener) {
        this.generatorProcess.stdout.readText(logPath, listener);
    }
    read(logPath = null, listener) {
        this.generatorProcess.stdout.readJson(logPath, listener);
    }
}
exports.ApplicationGeneratorAgent = ApplicationGeneratorAgent;
//# sourceMappingURL=agent.js.map