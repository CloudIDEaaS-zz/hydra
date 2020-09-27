import { CommandPacket } from './commandPacket';
import { exec, ChildProcess } from "child_process";
import * as path from "path";
import "../modules/utils/extensions";
import { Socket } from 'net';
import { EventEmitter } from 'events';
import { Api } from './api/api';
import { InstallsFromCacheStatus } from './InstallFromCacheStatus';
const fs = require("fs"); 

type ParmOptions = "generate" | "terminate" | "connect" | "ping" | "getversion" | "getfolder" | "getfile" | "getfolders" | "getfiles" | "getfilecontents" | "getfileicon" | "getpackageinstalls" | "getpackagedevinstalls" | "getcachestatus" | "setinstallstatus" | "getinstallfromcachestatus";

export class ApplicationGeneratorAgent {
    
    private generatorProcess : ChildProcess;
    private cacheServerProcess : ChildProcess;
    private stdout : Socket;
    private textListener: (response : string) => void;
    public onError: EventEmitter;
    api: Api;

    constructor() {
        this.stdout = <Socket>process.stdout;
        this.onError = new EventEmitter();
        this.api = new Api();
    }

    public initialize(debug : boolean = false)
    {
        let programFilesPath : string;
        let generatorApp : string;
        let commandLine: string;
        let hydraSolutionPath = <string> process.env.HYDRASOLUTIONPATH;

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

        this.generatorProcess = exec(commandLine);

        this.generatorProcess.stderr.on("data", (e) => {
            this.onError.emit("onError", e.toString());
        });

        this.generatorProcess.on("close",() => {
            this.stdout.writeLine("Exited");
        });
    }

    public launchCacheServer(listener: (response : string) => void) {

        var commandLine = "verdaccio";

        this.stdout.writeLine("Launching Cache Server");
        this.cacheServerProcess = exec(commandLine);

        this.cacheServerProcess.stderr.on("data", (e) => {
            this.onError.emit("onError", e.toString());
        });

        this.cacheServerProcess.stdout.readText(listener);
    }

    public stopCacheServer(listener: (response : string) => void) {

        this.cacheServerProcess.kill();

        this.cacheServerProcess.on("close",() => {
            this.stdout.writeLine("Cache server shut down");
        });
    }

    public dispose(listener: (commandObject : CommandPacket) => void) {
        
        if (this.generatorProcess) {
            this.sendSimpleCommand("terminate", listener);
        }
        else {
            listener(null);
        }
    }

    public stopListening() {
        this.generatorProcess.stdout.removeListener("data", this.textListener);
    }

    public sendSimpleCommand(command : ParmOptions, listener: (commandObject : CommandPacket) => void = null, ... args : { key: string, value: any}[]) {

        if (this.generatorProcess.killed) {
            this.stdout.writeLine("ApplicationGeneratorAgent process has exited!");
            listener(null);

            throw new Error("ApplicationGeneratorAgent process has exited!");
        }

        this.write(command, ...args);

        if (listener) {
            this.read<CommandPacket>(listener);
        }
    }

    public getInstallFromCacheStatus(mode : string, listener: (installsFromCacheStatus : InstallsFromCacheStatus) => void) {

        let promise = this.api.get<InstallsFromCacheStatus>("GetInstallFromCacheStatus", { key: "mode", value: mode });

        promise.then((status : InstallsFromCacheStatus) => {
            
            listener(status);

        }).catch((reason : any) => {

            let status = <InstallsFromCacheStatus>
            {
                StatusText : reason,
                StatusIsError : true
            };

            listener(status);
        });
    }

    public getVersion(mode : string, listener: (installsFromCacheStatus : InstallsFromCacheStatus) => void) {

        let promise = this.api.get<InstallsFromCacheStatus>("GetInstallFromCacheStatus", { key: "mode", value: mode });

        promise.then((status : InstallsFromCacheStatus) => {
            
            listener(status);

        }).catch((reason : any) => {

            let status = <InstallsFromCacheStatus>
            {
                StatusText : reason,
                StatusIsError : true
            };

            listener(status);
        });
    }

    public generateApp(entitiesProjectPath : string, servicesProjectPath : string, packageCachePath : string, noFileCreation : boolean, generatorPass : "None" | "All" | "HierarchyOnly" | "Files", listener: (response : string) => void = null) {
        
        let commandObject : CommandPacket = new CommandPacket(
            "request",
            "generate",
            [
                { "Kind": "app" },
                { "EntitiesProjectPath": entitiesProjectPath },
                { "ServicesProjectPath": servicesProjectPath },
                { "GeneratorPass": generatorPass },
                { "PackageCachePath": packageCachePath },
                { "NoFileCreation": noFileCreation }
            ],
            new Date(Date.now())
        );

        this.generatorProcess.stdin.writeJson(commandObject);

        if (listener) {

            this.textListener = listener;

            this.readText(this.textListener);
        }
    }

    public generateBusinessModel(templateFile: string, noFileCreation : boolean, generatorPass : "None" | "All" | "HierarchyOnly" | "Files", listener: (response : string) => void = null) {
        
        let commandObject : CommandPacket = new CommandPacket(
            "request",
            "generate",
            [
                { "Kind": "businessModel" },
                { "TemplateFile": templateFile },
                { "GeneratorPass": generatorPass },
                { "NoFileCreation": noFileCreation }
            ],
            new Date(Date.now())
        );

        this.generatorProcess.stdin.writeJson(commandObject);

        if (listener) {

            this.textListener = listener;

            this.readText(this.textListener);
        }
    }

    public generateEntities(templateFile: string, jsonFile: string, businessModelFile: string, entitiesProjectPath: string, noFileCreation : boolean, generatorPass : "None" | "All" | "HierarchyOnly" | "Files", listener: (response : string) => void = null) {
        
        let commandObject : CommandPacket = new CommandPacket(
            "request",
            "generate",
            [
                { "Kind": "entities" },
                { "TemplateFile": templateFile },
                { "JsonFile": jsonFile },
                { "BusinessModelFile": businessModelFile },
                { "EntitiesProjectPath": entitiesProjectPath },
                { "GeneratorPass": generatorPass },
                { "NoFileCreation": noFileCreation }
            ],
            new Date(Date.now())
        );

        this.generatorProcess.stdin.writeJson(commandObject);

        if (listener) {

            this.textListener = listener;

            this.readText(this.textListener);
        }
    }

    public generateWorkspace(appName: string, noFileCreation : boolean, generatorPass : "None" | "All" | "HierarchyOnly" | "Files", listener: (response : string) => void = null) {
        
        let commandObject : CommandPacket = new CommandPacket(
            "request",
            "generate",
            [
                { "Kind": "workspace" },
                { "AppName": appName },
                { "GeneratorPass": generatorPass },
                { "NoFileCreation": noFileCreation }
            ],
            new Date(Date.now())
        );

        this.generatorProcess.stdin.writeJson(commandObject);

        if (listener) {

            this.textListener = listener;

            this.readText(this.textListener);
        }
    }

    private write(command : ParmOptions, ... args : {key : string, value : any }[]) {
        
        let commandObject : CommandPacket;

        if (args.length) {

            let argsArray = [];

            args.forEach(a => {

                let item = new Object();

                item[a.key] = a.value;
                argsArray.push(item);
            });

            commandObject = new CommandPacket(
                "request",
                command,
                argsArray,
                new Date(Date.now())
            );
        }
        else {
            
            commandObject = new CommandPacket(
                "request",
                command,
                null,
                new Date(Date.now())
            );

        }

        this.generatorProcess.stdin.writeJson(commandObject);
    }

    private readText(listener: (response : string) => void) {
        
        this.generatorProcess.stdout.readText(listener);
    }

    private read<T>(listener: (T) => void) {
        
        this.generatorProcess.stdout.readJson<CommandPacket>(listener);
    }
}
