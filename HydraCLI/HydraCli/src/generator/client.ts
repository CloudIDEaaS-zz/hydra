import { ApplicationGeneratorAgent } from "./agent";
import { CommandPacket, CommandPacketResponse } from "./commandPacket";
import "../modules/utils/extensions";
import { Socket } from "net";
import { Utils } from "../modules/utils/utils";
import * as commands from "./commands";
import { List, IEnumerable, Dictionary } from "linq-javascript";
import { InstallInfo } from "./installInfo";
import { EventEmitter } from "events";
import { ConnectPromise } from "./connectPromise";
import { npm } from "./npmInstall";
import { PackageCacheStatus } from "./packageCacheStatus";
import { InstallsFromCacheStatus } from "./InstallFromCacheStatus";
import { Error } from "../modules/utils/extensions";
import { Renderer } from "./renderer";
import { gray, magenta, red } from "colors";
import resourceManager from "../resources/resourceManager";
const reader = require("readline-sync");
const readJson = require("read-package-json");
const path = require("path");
const commandLineArgs = require("command-line-args");
const colors = require("colors/safe");
const fs = require("fs");
const beautify = require("js-beautify");
const rimraf = require("rimraf");
const readline = require("readline");
const child_process = require("child_process");
const yaml = require("js-yaml");
const regedit = require("regedit");
const { version } = require("../../package.json");
const commandLineUsage = require("command-line-usage");
const cliProgress = require("cli-progress");
const chalk = require("chalk");
const dateFormat = require("dateformat");
const stackTrace = require("stack-trace");
const fileUrl = require("file-url");
const dialog = require("dialog");

const hydraGreen = chalk.hex("#0a7e79");
const hydraWarning = chalk.hex("#ae6600");

const barPreset = {
  format: hydraGreen(" {bar}") + " {status} {percentage}% ",
  barCompleteChar: "\u25aa",
  barIncompleteChar: " ",
};

const multiBar = new cliProgress.MultiBar(
  {
    clearOnComplete: false,
    hideCursor: true,
  },
  barPreset
);

enum ReportedMessages {
  downloadingTemplate = 1,
  initializingTemplate = 1 << 2,
  installingDependencies = 1 << 3,
  finished = 1 << 4,
}
export class ApplicationGeneratorClient {
  agent: ApplicationGeneratorAgent;
  stdout: Socket;
  stderr: Socket;
  static client: ApplicationGeneratorClient;
  installs: Dictionary<string, InstallInfo>;
  onComplete: EventEmitter;
  logOutputToConsole: boolean;
  devInstalls: Dictionary<string, InstallInfo>;
  commands: any;
  hasError: boolean;
  pollingInstallFromCacheStatus: boolean = false;
  pollingInstallFromCacheStatusComplete: boolean = false;
  installTotalCount: number;
  logPath: any;
  logClient: boolean;
  progressBarNextUpdate: any;
  progressBarCacheStatus: any;
  progressBarNotUsed: any;
  reportedMessages: ReportedMessages = 0;
  finalLapStarted: any;
  close: () => void;
  watch: (next: () => void) => void;
  complete: () => void;
  runStart: () => void;
  generateApp: () => void;
  setInstallStatus: (status: string) => Promise<string>;
  getCacheStatus: (mode: string) => Promise<PackageCacheStatus>;
  pollInstallFromCacheStatus: (
    mode: string,
    listener: (installsFromCacheStatus: InstallsFromCacheStatus) => void
  ) => Promise<void>;
  getInstallFromCacheStatus: () => void;
  watch2: (installs: Dictionary<string, InstallInfo>, next: () => void) => void;
  agentDisposed: boolean;
  resourceManager: any;

  constructor() {
    this.agent = new ApplicationGeneratorAgent();
    this.stdout = <Socket>process.stdout;
    this.stderr = <Socket>process.stderr;
    this.installs = new Dictionary<string, InstallInfo>();
    this.devInstalls = new Dictionary<string, InstallInfo>();
    this.onComplete = new EventEmitter();
    this.installTotalCount = -1;
  }

  public static start() {
    let client: ApplicationGeneratorClient;
    let clientVersion = version;
 
    if (ApplicationGeneratorClient.client === undefined) {
      ApplicationGeneratorClient.client = new ApplicationGeneratorClient();
    }

    client = ApplicationGeneratorClient.client;

    if (!process.versions["electron"]) {
      client.writeLine(`Hydra client version v${clientVersion} `);
    }

    client.processCommand();

    client.onComplete.on(
      "onComplete",
      (status: string = null, isError: boolean = false) => {
        if (isError) {
          console.error(status);
        } else if (status) {
          console.log(status);
        }
      }
    );
  }

  processCommand() {
    const mainCommand = commandLineArgs(commands["mainDefinitions"], {
      stopAtFirstUnknown: true,
    });

    let debug = mainCommand.debug;
    let logClient = mainCommand.logClient;
    let logToConsole = mainCommand.logToConsole;
    let skipInstalls = mainCommand.skipInstalls;
    let skipIonicInstall = mainCommand.skipIonicInstall;
    let noFileCreation = mainCommand.noFileCreation;
    let version = mainCommand.version;
    let lang = mainCommand.lang;
    let help = mainCommand.help;
    let argv = mainCommand._unknown || [];
    let configFile = path.join(process.cwd(), "package.json");

    this.resourceManager = new resourceManager(lang);
    this.agent.resourceManager = this.resourceManager;

    if (logClient) {
      this.logClient = true;
      this.logPath = path.join(
        process.cwd(),
        "Logs\\CLI\\" + dateFormat(new Date(), "yyyymmdd_HHMMss_l")
      );
    }

    if (mainCommand.command === "launchRenderer") {
      Renderer.launchRenderer(this.resourceManager);
    } 
    else if (mainCommand.command === "install") {
      this.install(argv, debug, logToConsole);
    } 
    else if (mainCommand.command === "captcha") {
      this.captcha(argv);
    } 
    else if (mainCommand.command === "start") {
      this.start(argv, debug, logToConsole, skipIonicInstall);
    } 
    else if (mainCommand.command === "setPackages") {
      this.setPackages(argv, logToConsole, configFile);
    } 
    else if (mainCommand.command === "addResource") {
      this.addResource(argv, debug);
    } 
    else if (mainCommand.command === "showDesigner") {
      this.showDesigner(argv, debug);
    } 
    else if (mainCommand.command === "build") {
      this.build(argv, logToConsole);
    } 
    else if (mainCommand.command === "delete") {
      this.delete(argv, logToConsole);
    } 
    else if (mainCommand.command === "test") {
      this.test(debug, false);
    } 
    else if (mainCommand.command === "add") {
      this.add(argv);
    } 
    else if (mainCommand.command === "remove") {
      this.remove(argv);
    } 
    else if (mainCommand.command === "update") {
      this.update(argv);
    } 
    else if (mainCommand.command === "serve") {
      this.serve(argv);
    } 
    else if (mainCommand.command === "platforms") {
      this.platforms();
    } 
    else if (version || mainCommand.command === "version") {
      this.version(debug, false);
    } 
    else if (version || mainCommand.command === "launchServices") {
      this.launchServices(debug, true);
    } 
    else if (help || mainCommand.command === undefined || mainCommand.command === "help") {
      this.help();
    } 
    else {
      readJson(configFile, null, false, (error, data) => {
        let servicesProjectPath;
        let entitiesProjectPath;
        let packageCachePath;

        if (argv[0] === "app") {
          if (error) {
            this.writeError(
              this.resourceManager.HydraCli.There_was_an_error_reading_the_package +
                error +
                this.resourceManager.HydraCli.Did_you_run_hydra_start
            );
            this.writeLog("errors.log", error.toJson());
            this.onComplete.emit("onComplete");
            return;
          }

          if (!data.servicesProjectPath) {
            this.writeError(
              this.resourceManager.HydraCli.package_json_does_not_include + "'servicesProjectPath'"
            );
            this.onComplete.emit("onComplete");
            return;
          }

          if (!data.entitiesProjectPath) {
            this.writeError(
              "package.json does not include " + "'entitiesProjectPath'"
            );
            this.onComplete.emit("onComplete");
            return;
          }

          servicesProjectPath = Utils.expandPath(data.servicesProjectPath);
          entitiesProjectPath = Utils.expandPath(data.entitiesProjectPath);

          if (data.packageCachePath) {
            packageCachePath = Utils.expandPath(data.packageCachePath);
          }
        }
        
        if (mainCommand.command === "clean") {
          this.clean(argv, logToConsole, configFile, data);
        } 
        else if (mainCommand.command === "set") {
          this.set(argv, logToConsole, configFile, data);
        } 
        else if (mainCommand.command === "generate") {
          this.generate(
            argv,
            debug,
            logToConsole,
            skipInstalls,
            entitiesProjectPath,
            servicesProjectPath,
            packageCachePath,
            noFileCreation
          );
        } else {
          this.writeError(`Unknown command: ${mainCommand.command}`);
        }
      });
    }
  }

  captcha(argv: any) {
    const uuid = require("uuid");
    const electron = require("electron");
    const app = electron.app;
    const BrowserWindow = electron.BrowserWindow;
    const BrowserView = electron.BrowserView;
    let mainWindow;
    let view;

    app.on("ready", () => {
      mainWindow = new BrowserWindow({
        width: 1200,
        height: 800,
        frame: false,
        modal: true,
      });

      view = new BrowserView();
      mainWindow.setBrowserView(view);
      mainWindow.setAlwaysOnTop(true);

      view.setBounds({ x: 0, y: 0, width: 1200, height: 800 });
      view.webContents.loadURL(argv[0] + "?key=" + argv[1]);

      view.webContents.on("did-finish-load", (e) => {
        try {
          let title = view.webContents.getTitle();
          let uuidString = uuid.stringify(uuid.parse(title));

          process.stdout.write(uuidString);
        } catch { }
      });

      mainWindow.on("closed", () => {
        mainWindow = null;
      });
    });
  }

  serve(argv: any) {
    let commandLine = "ionic serve";
    let ionicProcess;

    ionicProcess = child_process.exec(commandLine);

    ionicProcess.stdout.on("data", (o) => {
      let p = process;
      let c = p.connected;
      let output = o.toString();
      this.write(output);
    });
    ionicProcess.stderr.on("data", (e) => {
      let p = process;
      let c = p.connected;
      this.writeError(e.toString());
    });
  }

  install(argv: any, debug: any, logToConsole: any) {
    let npmPackage = argv;
    let cwd = process.cwd();

    this.writeLine(`Installing ${npmPackage}...`);

    npm
      .install(npmPackage, {
        cwd: cwd,
        save: true,
        output: true,
        lean: true,
        global: true,
      })
      .then(() => {
        this.writeSuccess(`Finished installing package: ${npmPackage}`);
      })
      .catch((e: Error) => {
        this.writeError(
          `Unable to install package: ${npmPackage}, Error: ${e}`
        );
        this.writeLog("errors.log", e.toJson());
      });
  }

  showDesigner(argv: any, debug: any) {
    this.initializeAndConnect(debug, false).then(() => {
      this.agent.sendSimpleCommand(
        "showdesigner",
        (commandObject: CommandPacket) => {
          let response = commandObject.Response;
        }
      );
    });
  }

  addResource(argv: string, debug: boolean) {
    const resourceOptions = commandLineArgs(commands.resourceDefinitions, {
      argv,
      stopAtFirstUnknown: true,
    });

    if (!debug) {
      debug = resourceOptions.debug;
    }

    switch (argv[0]) {
      case "--splashScreen":
      case "splashScreen": {
        if (resourceOptions.appDescription) {
        } else {
          this.initializeAndConnect(debug, false).then(() => {
            this.agent.sendSimpleCommand(
              "addresourcebrowsefile",
              (commandObject: CommandPacket) => {
                let filePath = commandObject.Response;
              },
              {
                key: "filter",
                value:
                  "Image files|*.bmp;*.jpg;*.gif;*.png;*.tif|All files|*.*",
              },
              { key: "resourcename", value: "splashScreen" }
            );
          });
        }

        break;
      }
    }
  }

  setPackages(argv: string, logToConsole: boolean, configFile: string) {
    const generateOptions = commandLineArgs(commands.setPackagesDefinitions, {
      argv,
      stopAtFirstUnknown: true,
    });
    let packageListArg = <string>generateOptions.packageListArg;
    let saveDev = <boolean>generateOptions["save-dev"];
    let clear = <boolean>generateOptions.clear;

    this.readJson(configFile, console.error, false, (error, data) => {
      let content: string;
      let propertyName: string;
      let packageList = packageListArg ? packageListArg.split(",") : [];

      if (saveDev) {
        propertyName = "devDependencies";
      } else {
        propertyName = "dependencies";
      }

      if (error) {
        throw new Error(error);
      }

      for (var property in data) {
        if (property === propertyName) {
          let packageData = data[property];

          if (clear) {
            for (var x in packageData) {
              if (packageData.hasOwnProperty(x)) {
                delete packageData[x];
              }
            }
          }

          packageList.forEach((i) => {
            let packageName: string;
            let version: string;

            [packageName, version] = i.split("@");
            packageData[packageName] = version;
          });

          break;
        }
      }

      content = beautify(JSON.stringify(data), {
        indent_size: 2,
        space_in_empty_paren: true,
      });

      fs.writeFile(configFile, content, "utf8", (err) => {
        if (err) {
          throw Error(err);
        }

        this.writeSuccess(
          `Finished saving ${packageList.length} '${propertyName}' to '${configFile}'`
        );
      });
    });
  }
  readJson(
    configFile: string,
    error: {
      (...data: any[]): void;
      (message?: any, ...optionalParams: any[]): void;
    },
    arg2: boolean,
    arg3: (error: any, data: any) => void
  ) {
    throw new Error(this.resourceManager.HydraCli.Method_not_implemented);
  }

  set(argv: string, logToConsole: any, configFile: any, data: any) {
    const generateOptions = commandLineArgs(commands.setDefinitions, {
      argv,
      stopAtFirstUnknown: true,
    });
    let name = generateOptions.name;

    let directory = process.cwd();

    if (name) {
      regedit.putValue(
        {
          "HKCU\\SOFTWARE\\Hydra\\ApplicationGenerator": {
            CurrentWorkingDirectory: {
              value: name,
              type: "REG_SZ",
            },
          },
        },
        function (err) {
          throw new Error(err);
        }
      );
    } else {
      regedit.putValue(
        {
          "HKCU\\SOFTWARE\\Hydra\\ApplicationGenerator": {
            CurrentWorkingDirectory: {
              value: directory,
              type: "REG_SZ",
            },
          },
        },
        function (err) {
          throw new Error(err);
        }
      );
    }
  }

  delete(argv: string, logToConsole: boolean) {
    let rl = readline.createInterface(process.stdin, process.stdout);
    let directory = path.join(process.cwd(), argv[0]);
    let thisWriteError = (o) => {
      this.writeError(o);
    };
    let thisWriteSuccess = (o) => {
      this.writeSuccess(o);
    };

    rl.question(`Delete '${directory}'? [yes]/no: `, function (answer) {
      if (answer !== "yes" && answer !== "y") {
        return;
      }

      fs.readdir(directory, (err, items) => {
        items.forEach((f) => {
          let fileName = f;
          let fullName = path.join(directory, fileName);
          fs.stat(fullName, (err, result) => {
            rimraf(fullName, (err) => {
              if (err) {
                thisWriteError(err);
                return;
              }
            });
          });
        });
      });

      process.chdir("../");

      rimraf(directory, (err) => {
        if (err) {
          thisWriteError(err);
          return;
        }
        process.exit(1);
      });
    });
  }

  build(argv: string, name: string) {
    const buildOptions = commandLineArgs(commands.buildDefinitions, {
      argv,
      stopAtFirstUnknown: true,
    });
    let platform = buildOptions.platform;
    let prod = buildOptions.prod;
    let release = buildOptions.release;
    let ionicProcess;
    let commandLine;

    if (platform) {
      commandLine = "ionic cordova build";
      commandLine += " " + platform;
    } else {
      commandLine = "ionic build";
    }

    if (prod) {
      commandLine += " --prod";
    }

    if (release) {
      commandLine += " --release";
    }

    ionicProcess = child_process.exec(commandLine);

    ionicProcess.stdout.on("data", (o) => {
      let p = process;
      let c = p.connected;
      let output = o.toString();
      this.write(output);
    });
    ionicProcess.stderr.on("data", (e) => {
      let p = process;
      let c = p.connected;
      this.writeError(e.toString());
    });
  }

  add(argv: string) {
    const buildOptions = commandLineArgs(commands.buildDefinitions, {
      argv,
      stopAtFirstUnknown: true,
    });
    let platform = buildOptions.platform;
    let commandLine = this.resourceManager.HydraCli.ionic_cordova_platform_add + platform;
    let ionicProcess;

    ionicProcess = child_process.exec(commandLine);

    ionicProcess.stdout.on("data", (o) => {
      let p = process;
      let c = p.connected;
      let output = o.toString();
      this.write(output);
    });
    ionicProcess.stderr.on("data", (e) => {
      let p = process;
      let c = p.connected;
      this.writeError(e.toString());
    });
  }

  remove(argv: string) {
    const buildOptions = commandLineArgs(commands.buildDefinitions, {
      argv,
      stopAtFirstUnknown: true,
    });
    let platform = buildOptions.platform;
    let commandLine = this.resourceManager.HydraCli.ionic_cordova_platform_remove + platform;
    let ionicProcess;

    ionicProcess = child_process.exec(commandLine);

    ionicProcess.stdout.on("data", (o) => {
      let p = process;
      let c = p.connected;
      let output = o.toString();
      this.write(output);
    });
    ionicProcess.stderr.on("data", (e) => {
      let p = process;
      let c = p.connected;
      this.writeError(e.toString());
    });
  }

  update(argv: string) {
    const buildOptions = commandLineArgs(commands.buildDefinitions, {
      argv,
      stopAtFirstUnknown: true,
    });
    let platform = buildOptions.platform;
    let commandLine = this.resourceManager.HydraCli.ionic_cordova_platform_update + platform;
    let ionicProcess;

    ionicProcess = child_process.exec(commandLine);

    ionicProcess.stdout.on("data", (o) => {
      let p = process;
      let c = p.connected;
      let output = o.toString();
      this.write(output);
    });
    ionicProcess.stderr.on("data", (e) => {
      let p = process;
      let c = p.connected;
      this.writeError(e.toString());
    });
  }

  platforms() {
    let commandLine = this.resourceManager.HydraCli.ionic_cordova_platform_ls;
    let ionicProcess;

    ionicProcess = child_process.exec(commandLine);

    ionicProcess.stdout.on("data", (o) => {
      let p = process;
      let c = p.connected;
      let output = o.toString();
      this.write(output);
    });
    ionicProcess.stderr.on("data", (e) => {
      let p = process;
      let c = p.connected;
      this.writeError(e.toString());
    });
  }

  clean(
    argv: string,
    logToConsole: boolean,
    configFile: string,
    configData: any
  ) {
    let content;
    let directory = process.cwd();
    let rl = readline.createInterface(process.stdin, process.stdout);
    let thisWriteError = (o) => {
      this.writeError(o);
    };

    rl.question(
      `Clean contents of '${directory}'? [yes]/no: `,
      function (answer) {
        if (answer !== "yes" && answer !== "y") {
          return;
        }

        for (var property in configData) {
          if (configData.hasOwnProperty(property)) {
            switch (property) {
              case "servicesProjectPath":
              case "entitiesProjectPath":
                break;
              default:
                delete configData[property];
                break;
            }
          }
        }

        content = beautify(JSON.stringify(configData), {
          indent_size: 2,
          space_in_empty_paren: true,
        });

        fs.writeFile(configFile, content, "utf8", function (err) {
          if (err) {
            thisWriteError(err);
            return;
          }
        });

        fs.readdir(directory, (err, items) => {
          items.forEach((f) => {
            let fileName = f;
            let fullName = path.join(directory, fileName);

            fs.stat(fullName, (err, result) => {
              if (
                result.isDirectory() ||
                (fileName !== "package.json" &&
                  fileName !== "package.backup.json")
              ) {
                rimraf(fullName, (err) => {
                  if (err) {
                    thisWriteError(err);
                    return;
                  }
                });
              }
            });
          });
        });
      }
    );
  }

  start(
    argv: string,
    debug: boolean,
    logToConsole: boolean,
    skipIonicInstall: boolean
  ) {
    const generateOptions = commandLineArgs(commands.startDefinitions, {
      argv,
      stopAtFirstUnknown: true,
    });
    let argv2 = generateOptions._unknown || [];
    let name = generateOptions.name;
    let pause = generateOptions.pause;
    let useAgent = generateOptions.useAgent;
    let logClient = generateOptions.logClient;
    let logServiceMessages = generateOptions.logServiceMessages;
    let npmCommand = "@ionic/cli@6.20.3";
    let cwd = process.cwd();
    this.complete = () => {
      if (useAgent) {
        this.writeSuccess(this.resourceManager.HydraCli.Signalling_agent_to_end_processing);

        this.agent.endProcessing((commandObject) => {
          this.writeSuccess(this.resourceManager.HydraCli.Signalling_agent_to_close);

          this.agent.dispose((commandObject) => {
            this.writeSuccess(this.resourceManager.HydraCli.Agent_closed_successfully);

            try {
              this.writeSuccess("Finalized");

              this.onComplete.emit("onComplete");
            } catch (err) {
              this.writeError(err.toString());
            }
          });
        });
      }
    };
    this.runStart = () => {
      let ionicProcess;
      let modCleanProcess;
      ionicProcess = this.runIonicStart(name);

      ionicProcess.on("close", () => {
        if (!this.hasError) {
          let projectPath = path.join(cwd, name);
          let configFile = path.join(projectPath, "package.json");

          readJson(configFile, console.error, false, (error, data) => {
            let content;
            let configData = {};
            let hydraConfigFile = path.join(cwd, "hydra.json");

            if (error) {
              this.writeError(
                this.resourceManager.HydraCli.There_was_an_error_reading_the_package +
                error +
                "\nDid you run 'hydra start'?"
              );
              this.complete();
              return;
            }

            readJson(hydraConfigFile, console.error, false, (error2, data2) => {
              let servicesProjectPath = "";
              let entitiesProjectPath = "";
              let packageCachePath = "";
              let directories = [];
              let files = [];
              let directory = "";

              if (!error2) {
                servicesProjectPath = data2.servicesProjectPath;
                entitiesProjectPath = data2.entitiesProjectPath;
                packageCachePath = data2.packageCachePath;
              }

              for (var property in data) {
                if (property === "scripts") {
                  configData["servicesProjectPath"] = servicesProjectPath;
                  configData["entitiesProjectPath"] = entitiesProjectPath;
                  configData["config"] = {
                    ionic_sass: "./config/sass.config.js",
                    ionic_copy: "./config/copy.config.js",
                    ionic_generate_source_map: "true",
                    ionic_source_map_type: "source-map",
                  };
                }

                configData[property] = data[property];
              }

              content = beautify(JSON.stringify(configData), {
                indent_size: 2,
                space_in_empty_paren: true,
              });

              fs.writeFile(configFile, content, "utf8", (err: Error) => {
                if (err) {
                  this.writeError(err.toString());
                  this.writeLog("errors.log", err.toJson());
                  return;
                }

                process.chdir(projectPath);

                directories = [".git", ".github"];

                directories.forEach((d) => {
                  let directory = path.join(projectPath, d);

                  if (fs.existsSync(directory)) {
                    rimraf.sync(directory);
                  }
                });

                this.writeStatus("Cleaning modules");
                this.reportHydraStatusProgress(
                  "Template Generation",
                  "Cleaning modules",
                  99
                );

                modCleanProcess = this.runModClean();
                modCleanProcess.on("close", () => {
                  this.writeSuccess(
                    `Finished creating project '${name}'. Please set the following fields in package.json: servicesProjectPath, entitiesProjectPath`
                  );
                  this.reportHydraStatusProgress(
                    "Template Generation",
                    "Finished creating project",
                    100
                  );

                  this.complete();
                });
              });
            });
          });
        }
      });
    };

    if (pause) {
      Utils.pause(pause);
    }

    if (!debug) {
      debug = generateOptions.debug;
    }

    this.logClient = logClient;
    this.logOutputToConsole = generateOptions.logToConsole;
    skipIonicInstall = generateOptions.skipIonicInstall;

    if (useAgent) {
      this.initializeAndConnect(debug, logServiceMessages)
        .then(() => {
          this.reportHydraStatusProgress(
            "Template Generation",
            "Starting template generation",
            0
          );

          if (skipIonicInstall) {
            this.runStart();
          } else {
            this.writeLine(`Installing ${npmCommand}...`);

            this.reportHydraStatusProgress(
              "Template Generation",
              "Installing the Ionic Framework",
              1
            );

            npm.install(npmCommand, {
                cwd: cwd,
                save: true,
                output: true,
                lean: true,
                global: true,
              })
              .then(() => {
                this.reportHydraStatusProgress(
                  "Template Generation",
                  "Finished installing the Ionic Framework",
                  5
                );
                this.writeSuccess(`Finished installing package: ${npmCommand}`);

                this.runStart();
              })
              .catch((e: Error) => {
                this.writeError(
                  `Unable to install package: ${npmCommand}, Error: ${e}`
                );
                this.writeLog("errors.log", e.toJson());
              });
          }
        })
        .catch((e: Error) => {
          this.writeError(`Error: ${e}`);
          this.writeLog("errors.log", e.toJson());
        });
    } else {
      if (skipIonicInstall) {
        this.runStart();
      } else {
        this.writeLine(`Installing ${npmCommand}...`);

        npm
          .install(npmCommand, {
            cwd: cwd,
            save: true,
            lean: true,
            output: true,
            global: true,
          })
          .then(() => {
            this.writeSuccess(`Finished installing package: ${npmCommand}`);
            this.runStart();
          })
          .catch((e: Error) => {
            this.writeError(
              `Unable to install package: ${npmCommand}, Error: ${e}`
            );
            this.writeLog("errors.log", e.toJson());
          });
      }
    }
  }

  runIonicStart(name: string) {
    let template =
      "https://github.com/CloudIDEaaS/hydra-ionic-angular-baseline-app";
    let type = "ionic-angular";
    let commandLine = `ionic start ${name} ${template} --no-git --type=${type} --cordova --no-link`;
    //let commandLine = `ionic start ${name} ${template}`;
    let ionicProcess = child_process.exec(commandLine);

    /* ATTENTION: you may get the unhandled exception ERROR_LOCAL_CLI_NOT_FOUND, just ignore it */

    ionicProcess.stdout.on("data", (o) => {
      let p = process;
      let c = p.connected;
      let output = o.toString();

      this.reportHydraStatusFromOutput("Template Generation", output);

      if (output.indexOf("Not erasing existing project") > -1) {
        this.hasError = true;
        this.writeError(output);

        process.exit();
      } else {
        this.write(output);
      }
    });

    ionicProcess.stderr.on("data", (e: Error) => {
      let p = process;
      let c = p.connected;
      let error = e.toString();

      this.reportHydraStatusFromOutput("Template Generation", error);
      this.write(error);
    });

    return ionicProcess;
  }

  agentComplete() {
    if (!this.agentDisposed) {
      this.agentDisposed = true;

      this.agent.endProcessing((commandObject) => {
        this.writeSuccess("Signalling agent to close");

        this.agent.dispose((commandObject) => {
          this.writeSuccess("Agent closed successfully. Finalizing");

          try {
            this.writeSuccess("Finalized");

            this.onComplete.emit("onComplete");
          } catch (err) {
            this.writeError(err.toString());
          }
        });
      });
    }
  }

  runModClean() {
    let commandLine = `modclean --ignore="cordova*,cordova*/**,platforms,platforms/**,plugins,plugins/**,@angular,@angular/**,@ionic,@ionic/**" --run --patterns default:caution`;
    let modCleanProcess = child_process.exec(commandLine);

    modCleanProcess.stdout.on("data", (o) => {
      let p = process;
      let c = p.connected;
      let output = o.toString();

      this.writeLine(output);
      this.writeLog("modclean", output);
    });

    modCleanProcess.stderr.on("data", (e: Error) => {
      let p = process;
      let c = p.connected;
      let error = e.toString();

      this.writeError(error);
      this.writeLog("errors.log", error);
    });

    return modCleanProcess;
  }

  reportHydraStatusFromOutput(name: string, output: string) {
    let message: string;
    let status: string;
    let percentComplete: number = 0;
    let alertLevel: "critical" | "info" | "important" = "info";

    if (output.indexOf("Not erasing existing project") > -1) {
      status = "Error: Not erasing existing project";
      this.writeError(output);
    } else if (
      !(this.reportedMessages & ReportedMessages.finished) &&
      output.indexOf("Go to your cloned project") > -1
    ) {
      status = "Finished creating project";
      percentComplete = 100;
      this.reportedMessages |= ReportedMessages.finished;
      // dialog.info(output, status, (exitCode) => {});
    } else if (
      !(this.reportedMessages & ReportedMessages.finished) &&
      output.indexOf("Finished creating project '") !== -1
    ) {
      status = "Finished creating project";
      percentComplete = 100;
      this.reportedMessages |= ReportedMessages.finished;
      // dialog.info(output, status, (exitCode) => {});
    } else if (
      !(this.reportedMessages & ReportedMessages.installingDependencies) &&
      output.indexOf("Installing dependencies may take several minutes") !== -1
    ) {
      status = "Installing dependencies may take several minutes";
      percentComplete = 30;
      this.reportedMessages |= ReportedMessages.installingDependencies;
      // dialog.info(output, status, (exitCode) => {});
    } else if (
      !(this.reportedMessages & ReportedMessages.initializingTemplate) &&
      output.indexOf("Resolving deltas:") !== -1
    ) {
      status = "Initializing template";
      percentComplete = 20;
      this.reportedMessages |= ReportedMessages.initializingTemplate;
      // dialog.info(output, status, (exitCode) => {});
    } else if (
      !(this.reportedMessages & ReportedMessages.downloadingTemplate) &&
      output.indexOf("Receiving objects:") !== -1
    ) {
      status = "Downloading template";
      percentComplete = 10;
      this.reportedMessages |= ReportedMessages.downloadingTemplate;
      // dialog.info(output, status, (exitCode) => {});
    }

    if (status) {
      this.reportHydraStatusProgress(name, status, percentComplete, alertLevel);
    }
  }

  reportHydraStatusProgress(
    name: string,
    status: string,
    percentComplete: number,
    alertLevel: "critical" | "info" | "important" = "info"
  ) {
    let message: string;

    message = `callback: name=${name}, status=${status}, percentComplete=${percentComplete}`;
    this.writeStatus(message);

    if (this.agent.connected) {
      this.agent.sendHydraStatus(message, alertLevel);
    }
  }

  generate(
    argv: string,
    debug: boolean,
    logToConsole: string,
    skipInstalls: boolean,
    entitiesProjectPath: string,
    servicesProjectPath: string,
    packageCachePath: string,
    noFileCreation: boolean
  ) {
    let generateOptions = commandLineArgs(commands.generateDefinitions, {
      argv,
      stopAtFirstUnknown: true,
    });

    let source: string;
    let unknown = <Array<string>> generateOptions._unknown;

    if (unknown) {
      source = unknown.shift();
      unknown.unshift("app");
      unknown.unshift("generate");

      argv = unknown.join(" ");

      generateOptions = commandLineArgs(commands.generateDefinitions, {
        argv,
      });      
    }

    let logServiceMessages = generateOptions.logServiceMessages;
    let logClient = generateOptions.logClient;
    let pause = generateOptions.pause;

    if (logClient) {
      this.logClient = true;
      this.logPath = path.join(
        process.cwd(),
        "Logs\\CLI\\" + dateFormat(new Date(), "yyyymmdd_HHMMss_l")
      );

      process.on("uncaughtException", (e: Error) => {
        this.writeError(e.toString());
        this.writeLog("errors.log", e.toJson());

        this.agent.sendHydraStatus(e.toString(), "critical");

        process.exit();
      });
    } else {
      process.on("uncaughtException", (e: Error) => {
        this.writeError(e.toString());
        this.writeLog("errors.log", e.toJson());

        process.exit();
      });
    }

    // being generateApp

    this.generateApp = () => {
      this.agent.generateApp(
        entitiesProjectPath,
        servicesProjectPath,
        packageCachePath,
        noFileCreation,
        "All",
        (response: string) => {
          let commandPacket = <CommandPacketResponse>JSON.parse(response);
          this.complete = () => {
            let loaded = npm.getLoaded();
            let errorHandler = npm.getErrorHandler();

            this.reportHydraStatusProgress(
              "Completion",
              "Generation completed",
              50
            );
            this.writeSuccess("Signalling agent to end processing");

            this.agent.endProcessing((commandObject) => {
              this.writeSuccess("Signalling agent to close");

              this.agent.dispose((commandObject) => {
                this.writeSuccess("Agent closed successfully. Finalizing");

                try {
                  Utils.sleep(1000).then(() => {
                    try {
                      if (loaded) {
                        errorHandler();
                      }
                    } catch { }

                    this.writeSuccess("Finalized");

                    this.onComplete.emit("onComplete", "Finalized");
                  });
                } catch { }
              });
            });
          };
          let cleanupAndComplete = (status: string) => {
            let modCleanProcess;

            this.agent.sendHydraStatus(status, "important");

            this.writeStatus("Cleaning modules");
            this.reportHydraStatusProgress("Completion", "Cleaning modules", 0);

            modCleanProcess = this.runModClean();
            modCleanProcess.on("close", () => {
              this.complete();
            });
          };

          if (!commandPacket.IsChainedStream) {
            this.agent.stopListening();
          } else {
            this.writeLine(commandPacket.Response);
          }

          if (!commandPacket.IsChainedStream) {
            this.agent.sendHydraStatus("Generating app");

            if (skipInstalls) {
              this.onComplete.emit("onComplete");
            } else {
              const installWatch = 1000;
              const statusWatch = 5000;
              const statusPrintInterval = 60000;
              let cacheProgressPercent = 0;
              let lastStatusPrint = 0;
              let usesPackageCache = false;
              let showCacheProcessingWarning = false;
              let installing = false;
              let watchMilliseconds = installWatch;
              let cacheStatusMode = "installs";
              let waitingForInstallStatus = false;
              let installsInProcess = false;

              this.setInstallStatus = (status: string): Promise<string> => {
                if (this.logClient) {
                  this.writeAllLogs("installStatus.log", status);
                }

                return new Promise<string>(
                  (
                    resolve: (result: string) => void,
                    reject: (reason: any) => void
                  ) => {
                    waitingForInstallStatus = true;

                    this.agent.sendSimpleCommand(
                      "setinstallstatus",
                      (commandObject: CommandPacket) => {
                        let cacheStatus = <string>commandObject.Response;
                        resolve(cacheStatus);
                        waitingForInstallStatus = false;
                      },
                      { key: "status", value: status }
                    );
                  }
                );
              };

              this.getCacheStatus = (
                mode: string
              ): Promise<PackageCacheStatus> => {
                return new Promise<PackageCacheStatus>(
                  (
                    resolve: (status: PackageCacheStatus) => void,
                    reject: (reason: any) => void
                  ) => {
                    this.agent.sendSimpleCommand(
                      "getcachestatus",
                      (commandObject: CommandPacket) => {
                        let cacheStatus = <PackageCacheStatus>(
                          commandObject.Response
                        );

                        if (this.logClient) {
                          this.writeAllLogs(
                            "cacheStatus.log",
                            "\r\n" + beautify(JSON.stringify(cacheStatus))
                          );
                        }

                        resolve(cacheStatus);
                      },
                      { key: "mode", value: mode }
                    );
                  }
                );
              };

              this.pollInstallFromCacheStatus = async (
                mode: string,
                listener: (
                  installsFromCacheStatus: InstallsFromCacheStatus
                ) => void
              ) => {
                this.getInstallFromCacheStatus = () => {
                  this.agent.getInstallFromCacheStatus(
                    mode,
                    (installsFromCacheStatus: InstallsFromCacheStatus) => {
                      if (this.logClient) {
                        this.writeAllLogs(
                          "installFromCacheStatus.log",
                          "\r\n" +
                          beautify(JSON.stringify(installsFromCacheStatus))
                        );
                      }

                      listener(installsFromCacheStatus);

                      if (installsFromCacheStatus.NothingToPoll) {
                        this.pollingInstallFromCacheStatusComplete = true;
                      } else if (installsFromCacheStatus.TotalRemaining > 0) {
                        let percent = Math.trunc(
                          (installsFromCacheStatus.TotalRemaining /
                            installsFromCacheStatus.Total) *
                          100
                        );

                        Utils.sleep(watchMilliseconds).then(() => {
                          this.getInstallFromCacheStatus();
                        });
                      } else {
                        this.pollingInstallFromCacheStatusComplete = true;
                      }
                    }
                  );
                };

                this.getInstallFromCacheStatus();
              };

              this.watch2 = (
                installs: Dictionary<string, InstallInfo>,
                next: () => void
              ) => {
                Utils.sleep(watchMilliseconds)
                  .then(() => {
                    installing = installs.any((i) => !i.value.InstallAttempted);

                    if (installing) {
                      if (
                        usesPackageCache &&
                        !this.pollingInstallFromCacheStatus
                      ) {
                        this.pollingInstallFromCacheStatus = true;
                        this.pollInstallFromCacheStatus(cacheStatusMode, (s) =>
                          this.installsFromCacheStatusListener(s)
                        );
                      }

                      this.watch2(installs, next);
                    } else if (usesPackageCache) {
                      watchMilliseconds = statusWatch;

                      if (showCacheProcessingWarning) {
                        lastStatusPrint = Date.now();

                        this.writeWarning(
                          "Package cache processing may take a while on first run.  Please be patient."
                        );

                        showCacheProcessingWarning = false;
                      }

                      if (waitingForInstallStatus || installsInProcess) {
                        this.watch2(installs, next);
                      } else if (
                        this.pollingInstallFromCacheStatus &&
                        !this.pollingInstallFromCacheStatusComplete
                      ) {
                        this.watch2(installs, next);
                      } else if (this.installTotalCount === 0) {
                        this.installTotalCount = -1;

                        if (!this.pollingInstallFromCacheStatus) {
                          this.pollingInstallFromCacheStatus = true;
                          this.pollInstallFromCacheStatus(
                            cacheStatusMode,
                            (s) => this.installsFromCacheStatusListener(s)
                          );
                        }

                        this.watch2(installs, next);
                      } else {
                        this.getCacheStatus(cacheStatusMode)
                          .then((cacheStatus: PackageCacheStatus) => {
                            let now = Date.now();
                            let diff = now - lastStatusPrint;
                            let statusText;

                            cacheProgressPercent =
                              cacheStatus.StatusProgressPercent;

                            if (!this.progressBarNextUpdate) {
                              this.progressBarNextUpdate = multiBar.create(
                                statusPrintInterval,
                                0,
                                { status: "Next status update" }
                              );
                            }

                            if (!this.progressBarCacheStatus) {
                              this.progressBarCacheStatus = multiBar.create(
                                100,
                                0,
                                { status: "Cache progress" }
                              );
                            }

                            if (!this.progressBarNotUsed) {
                              this.progressBarNotUsed = multiBar.create(
                                100,
                                0,
                                { status: "Cache progress" }
                              );
                            }

                            if (diff > statusPrintInterval) {
                              try {
                                this.progressBarNextUpdate.update(
                                  statusPrintInterval,
                                  { status: "Next status update" }
                                );
                                this.progressBarCacheStatus.update(
                                  Math.max(cacheProgressPercent, 1),
                                  { status: "Cache progress" }
                                );
                                this.progressBarNotUsed.update(100, {
                                  status: "Started",
                                });
                              } catch (e) {
                                this.writeLog("errors.log", e, true);
                              }

                              statusText = cacheStatus.StatusText.replace(
                                "{ lastRequest }",
                                dateFormat(new Date(), "m/d/yyyy h:MM:ss TT")
                              );

                              this.writeStatus(statusText);

                              lastStatusPrint = now;
                            } else {
                              try {
                                this.progressBarNextUpdate.update(diff, {
                                  status: "Next status update",
                                });
                                this.progressBarCacheStatus.update(
                                  Math.max(cacheProgressPercent, 1),
                                  { status: "Cache progress" }
                                );
                                this.progressBarNotUsed.update(100, {
                                  status: "Started",
                                });
                              } catch (e) {
                                this.writeLog("errors.log", e, true);
                              }
                            }

                            if (
                              cacheStatus.NoCaching ||
                              cacheStatus.CacheStatus === "EndOfProcessing"
                            ) {
                              try {
                                this.progressBarNextUpdate.update(
                                  statusPrintInterval,
                                  { status: "Next status update" }
                                );
                                this.progressBarCacheStatus.update(100, {
                                  status: "Cache progress",
                                });
                              } catch (e) {
                                this.writeLog("errors.log", e, true);
                              }

                              if (cacheStatusMode === "devInstalls") {
                                this.writeSuccess(
                                  `${cacheStatusMode} processing complete ************************************`
                                );

                                this.setInstallStatus("finalized").then(
                                  (result: string) => {
                                    cacheStatusMode = "finalizing";
                                    this.watch2(installs, next);
                                  }
                                );

                                return;
                              } else {
                                this.writeStatus(
                                  cacheStatus.StatusText.replace(
                                    "{ lastRequest }",
                                    dateFormat(
                                      new Date(),
                                      "m/d/yyyy h:MM:ss TT"
                                    )
                                  )
                                );
                                lastStatusPrint = now;

                                this.writeSuccess(
                                  `${cacheStatusMode} processing complete ************************************`
                                );

                                next();
                                return;
                              }
                            }

                            this.watch2(installs, next);
                          })
                          .catch((e: Error) => {
                            this.writeError(`Error: ${e}`);
                            this.writeLog("errors.log", e.toJson());
                          });
                      }
                    } else {
                      next();
                    }
                  })
                  .catch((e: Error) => {
                    this.writeError(`Error: ${e}`);
                    this.writeLog("errors.log", e.toJson());
                  });
              };

              if (packageCachePath) {
                usesPackageCache = true;
                showCacheProcessingWarning = true;
              }

              let installDev = () => {
                cacheStatusMode = "devInstalls";

                this.agent.sendSimpleCommand(
                  "getpackagedevinstalls",
                  (commandObject) => {
                    let packageInstalls = commandObject.Response;
                    let count = 0;
                    let totalCount = packageInstalls.length;

                    this.installTotalCount = totalCount;

                    if (
                      this.pollingInstallFromCacheStatus &&
                      !this.pollingInstallFromCacheStatusComplete
                    ) {
                      throw new Error(
                        "Unexpected incompletion of pollingInstallFromCacheStatus"
                      );
                    }

                    this.pollingInstallFromCacheStatusComplete = false;
                    this.pollingInstallFromCacheStatus = false;

                    this.agent.sendHydraStatus("Package dev installs started");
                    this.reportHydraStatusProgress(
                      "Final Lap",
                      "Package dev installs started",
                      0
                    );

                    this.setInstallStatus("installsStarted").then(
                      (result: string) => {
                        let installNext = () => {
                          if (packageInstalls.length) {
                            try {
                              let install = packageInstalls.pop();

                              installsInProcess = true;
                              installDevPackage(install);
                            } catch (err) {
                              console.log();
                            }
                          } else {
                            installsInProcess = false;
                            this.setInstallStatus("installsComplete").then(
                              (result: string) => { }
                            );
                          }
                        };

                        let installDevPackage = (i) => {
                          let message: string;
                          let percent: number;

                          this.devInstalls.set(i, new InstallInfo(i));
                          count++;

                          message = `Attempting to install ${count} of ${totalCount} dev packages`;
                          percent = Math.trunc(
                            (count / totalCount) * 0.25 * 100
                          );

                          this.reportHydraStatusProgress(
                            "Final Lap",
                            `Installing ${i}`,
                            percent
                          );
                          this.agent.sendHydraStatus(message);

                          this.writeLine(
                            "\n\n" + message + "*".repeat(25) + "\n"
                          );
                          this.writeLine(`npm install ${i} --save-dev`);

                          npm
                            .install(i, {
                              cwd: process.cwd(),
                              saveDev: true,
                              lean: true,
                              output: true,
                            })
                            .then(() => {
                              let info = this.devInstalls.get(i);

                              info.InstallAttempted = true;
                              info.Succeeded = true;
                              this.writeSuccess(
                                `Finished installing package: ${i}`
                              );

                              installNext();
                            })
                            .catch((e: Error) => {
                              let info = this.devInstalls.get(i);
                              info.InstallAttempted = true;

                              info.Failed = true;
                              this.writeError(
                                `Unable to install package: ${i}, Error: ${e}`
                              );
                              this.writeLog("errors.log", e.toJson());

                              installNext();
                            });
                        };

                        this.writeLine(
                          `\nInstalling ${totalCount} dev dependencies ` +
                          "*".repeat(50) +
                          "\n"
                        );
                        installNext();

                        this.watch2(this.devInstalls, () =>
                          cleanupAndComplete("\nHydra installs completed!")
                        );
                      }
                    );
                  }
                );
              };

              this.agent.sendSimpleCommand(
                "getpackageinstalls",
                (commandObject) => {
                  let packageInstalls = commandObject.Response;
                  let count = 0;
                  let totalCount = packageInstalls.length;
                  let installing = false;
                  let watchMilliseconds = installWatch;
                  let lastStatusPrint = 0;

                  this.installTotalCount = totalCount;

                  if (
                    this.pollingInstallFromCacheStatus &&
                    !this.pollingInstallFromCacheStatusComplete
                  ) {
                    throw new Error(
                      "Unexpected incompletion of pollingInstallFromCacheStatus"
                    );
                  }

                  this.pollingInstallFromCacheStatusComplete = false;
                  this.pollingInstallFromCacheStatus = false;

                  this.agent.sendHydraStatus("Package installs started");
                  this.reportHydraStatusProgress(
                    "Full Throttle",
                    "Package installs started",
                    0
                  );

                  this.setInstallStatus("installsStarted").then(
                    (result: string) => {
                      let installNext = () => {
                        if (packageInstalls.length) {
                          try {
                            let install = packageInstalls.pop();

                            installsInProcess = true;
                            installPackage(install);
                          } catch (err) {
                            console.log();
                          }
                        } else {
                          installsInProcess = false;
                          this.setInstallStatus("installsComplete").then(
                            (result: string) => { }
                          );
                        }
                      };

                      let installPackage = (i) => {
                        let message: string;
                        let percent: number;

                        this.installs.set(i, new InstallInfo(i));
                        count++;

                        message = `Attempting to install ${count} of ${totalCount} packages `;
                        percent = Math.trunc((count / totalCount) * 0.25 * 100);

                        this.reportHydraStatusProgress(
                          "Full Throttle",
                          `Installing ${i}`,
                          percent
                        );
                        this.agent.sendHydraStatus(message);

                        this.writeLine(
                          "\n\n" + message + "*".repeat(25) + "\n"
                        );
                        this.writeLine(`npm install ${i} --save`);

                        npm
                          .install(i, {
                            cwd: process.cwd(),
                            save: true,
                            lean: true,
                            output: true,
                          })
                          .then(() => {
                            let info = this.installs.get(i);

                            info.InstallAttempted = true;
                            info.Succeeded = true;
                            this.writeSuccess(
                              `Finished installing package: ${i}`
                            );

                            installNext();
                          })
                          .catch((e: Error) => {
                            let info = this.installs.get(i);

                            info.InstallAttempted = true;
                            info.Failed = true;
                            this.writeError(
                              `Unable to install package: ${i}, Error: ${e}`
                            );
                            this.writeLog("errors.log", e.toJson());

                            installNext();
                          });
                      };

                      this.writeLine(
                        `\nInstalling ${totalCount} dependencies ` +
                        "*".repeat(50) +
                        "\n"
                      );

                      installNext();
                      this.watch2(this.installs, installDev);
                    }
                  );
                }
              );
            }
          }
        }
      );
    };
   
    // end generateApp

    if (pause) {
      Utils.pause(pause);
    }

    if (!debug) {
      debug = generateOptions.debug;
    }

    if (!skipInstalls) {
      skipInstalls = generateOptions.skipInstalls;
    }

    if (!noFileCreation) {
      noFileCreation = generateOptions.noFileCreation;
    }

    if (logToConsole) {
      this.logOutputToConsole = true;
    } else {
      this.logOutputToConsole = generateOptions.logToConsole;
    }

    if (generateOptions.target === "app") {
      this.initializeAndConnect(debug, logServiceMessages)
        .then(() => {
          this.generateApp();
        })
        .catch((e: Error) => {
          this.writeError(`Error: ${e}`);
          this.writeLog("errors.log", e.toJson());
        });
    } else if (generateOptions.target === "workspace" || generateOptions.target === "from") {


      this.initializeAndConnect(debug, logServiceMessages)
        .then(() => {
          let appName = generateOptions.appName;
          let appDescription = generateOptions.appDescription;
          let organizationName = generateOptions.organizationName;
          let promises = [];

          if (!appName) {
            do {
              appName = reader.question(hydraGreen("?") + " App name : ");

              if (/^[a-zA-Z]{1}[a-zA-Z]{4,24}$/.test(appName)) {
                break;
              }

              this.writeExplicit(
                hydraWarning("No spaces or special characters. > 5 char, < 25")
              );
            } while (true);
          }

          if (!appDescription) {
            do {
              appDescription = reader.question(
                hydraGreen("?") + " App description: "
              );

              if (/^.{5,255}$/.test(appDescription)) {
                break;
              }

              this.writeExplicit(hydraWarning("> 5 char, < 255"));
            } while (true);
          }

          if (!organizationName) {
            do {
              organizationName = reader.question(
                hydraGreen("?") + " Organization name: "
              );

              if (/^[a-zA-Z]{1}[a-zA-Z]{4,24}$/.test(appName)) {
                break;
              }

              this.writeExplicit(
                hydraWarning("No spaces or special characters. > 5 char, < 25")
              );
            } while (true);
          }

          if (generateOptions.target === "workspace") {
            this.agent.generateWorkspace(
              appName,
              appDescription,
              organizationName,
              noFileCreation,
              "All",
              (response: string) => {
                this.handleResponse(response, generateOptions.target);
              }
            );
          }
          else if (generateOptions.target === "from") {
            this.agent.generateWorkspace(
              appName,
              appDescription,
              organizationName,
              noFileCreation,
              "All",
              (response: string) => {
                this.handleResponse(response, generateOptions.target);
              }
            );
          }
        })
        .catch((e: Error) => {
          this.writeError(`Error: ${e}`);
          this.writeLog("errors.log", e.toJson());
          this.agentComplete();
        });
    } else if (generateOptions.target === "businessmodel") {
      this.initializeAndConnect(debug, logServiceMessages)
        .then(() => {
          let templateFile = generateOptions.template;

          this.agent.generateBusinessModel(
            templateFile,
            noFileCreation,
            "All",
            (response: string) => {
              this.handleResponse(response, generateOptions.target);
            }
          );
        })
        .catch((e: Error) => {
          this.writeError(`Error: ${e}`);
          this.writeLog("errors.log", e.toJson());
          this.agentComplete();
        });
    } else if (generateOptions.target === "entities") {
      this.initializeAndConnect(debug, logServiceMessages)
        .then(() => {
          let templateFile = generateOptions.template;
          let businessModelFile = generateOptions.businessmodel;
          let jsonFile = generateOptions.json;

          this.agent.generateEntities(
            templateFile,
            jsonFile,
            businessModelFile,
            entitiesProjectPath,
            noFileCreation,
            "All",
            (response: string) => {
              this.handleResponse(response, generateOptions.target);
            }
          );
        })
        .catch((e: Error) => {
          this.writeError(`Error: ${e}`);
          this.writeLog("errors.log", e.toJson());
          this.agentComplete();
        });
    } else {
      this.writeError(`Unknown target: ${generateOptions.target}`);
    }
  }

  handleResponse(response: string, target: string) {
    if (response.startsWith("{")) {
      let commandPacket = <CommandPacketResponse>JSON.parse(response);

      if (!commandPacket.IsChainedStream) {
        this.writeSuccess(
          `Finished generating ${target}, response: ${commandPacket.Response}`
        );

        if (commandPacket.Response !== "End Processing") {
          this.agentComplete();
        }
      } else {
        this.writeLine(commandPacket.Response);
      }
    }
  }

  setPackageJsonConfig(
    configPath: string,
    packageList: Array<string>,
    callback: (error: string) => void
  ) { }

  setPackageYamlConfig(
    configPath: string,
    packageList: Array<string>,
    callback: (error: string) => void
  ) {
    let configData = yaml.safeLoad(fs.readFileSync(configPath, "utf8"));
    let packagesSection = configData["packages"];
    let configContents: string;
    let scopedAllProperty = "@*/*";
    let allProperty = "**";
    let scopedAll: any;
    let all: any;

    if (!packagesSection.hasOwnProperty(`'${scopedAllProperty}'`)) {
      scopedAll = packagesSection[scopedAllProperty];
      delete packagesSection[scopedAllProperty];
    }

    if (!packagesSection.hasOwnProperty(`'${allProperty}'`)) {
      all = packagesSection[allProperty];
      delete packagesSection[allProperty];
    }

    packageList.forEach((p) => {
      if (!packagesSection.hasOwnProperty(p)) {
        if (this.isQuotablePropertyName(p)) {
          packagesSection[`${p}`] = { access: "$all", publish: "$all" };
        } else {
          packagesSection[`\ua725${p}\ua725`] = {
            access: "$all",
            publish: "$all",
          };
        }
      }
    });

    if (scopedAll) {
      packagesSection[`${scopedAllProperty}`] = scopedAll;
    }

    if (all) {
      packagesSection[`${allProperty}`] = all;
    }

    configContents = yaml.safeDump(configData);
    configContents = configContents.replace(/\ua725/g, "'");

    fs.writeFile(configPath, configContents, function (err) {
      if (err) {
        callback(err);
      } else {
        callback(null);
      }
    });
  }

  isQuotablePropertyName(str: string) {
    return /^[@~`!#$%\^&*+=\-\[\]\\';,/{}|\\":<>\?]/g.test(str);
  }

  cachePackageData(
    packageCachePath: string,
    callback: (error: string) => void
  ) {
    // figure out versioning, refresh, etc.
    let packageDataFile = path.join(packageCachePath, ".verdaccio-db.json");

    this.writeLine("Caching package data");

    readJson(packageDataFile, console.error, false, (error2, data) => {
      let content;
      let packageList = <Array<string>>data["list"];

      fs.readdir(packageCachePath, (err, items) => {
        if (err) {
          callback(err);
        } else {
          let count = items.length;

          items.forEach((f) => {
            let fileName = f;
            let fullName = path.join(packageCachePath, fileName);

            fs.stat(fullName, (err, result) => {
              count--;

              if (err) {
                callback(err);
              } else {
                if (!result.isFile()) {
                  if (!packageList.includes(fileName)) {
                    packageList.push(fileName);
                  }
                }

                if (count === 0) {
                  content = beautify(JSON.stringify(data), {
                    indent_size: 2,
                    space_in_empty_paren: true,
                  });

                  fs.writeFile(packageDataFile, content, "utf8", (err) => {
                    let packageConfigPath = path.join(
                      packageCachePath,
                      "..\\",
                      "config.yaml"
                    );

                    if (err) {
                      callback(err);
                      return;
                    }

                    this.setPackageYamlConfig(
                      packageConfigPath,
                      packageList,
                      callback
                    );
                  });
                }
              }
            });
          });
        }
      });
    });
  }

  initializeAndConnect(debug: boolean, logServiceMessages: boolean) {
    let promise = new ConnectPromise();
    this.agent.initialize(debug, logServiceMessages);

    this.agent.onError.on("onError", (e) => {
      this.writeError(e);
      this.dispose();
    });

    this.agent.sendSimpleCommand("connect", (connectObject) => {
      if (connectObject.Response === "Connected successfully") {
        this.agent.sendHydraStatus("Connected successfully");
        this.agent.connected = true;

        this.writeSuccess("Connected");
        promise.resolve("Connected");
      } else {
        promise.reject(connectObject.Response);
      }
    });

    return promise;
  }

  dispose() { }

  launchServices(debug: boolean, logServiceMessages: boolean) {
    this.initializeAndConnect(debug, logServiceMessages).then(() => {
      this.agent.sendSimpleCommand(
        "launchservices",
        (commandObject: CommandPacket) => {
          let response = <string>commandObject.Response;
          this.writeLine(response);
          this.agent.dispose((commandObject) => {
            this.onComplete.emit("onComplete");
          });
        }
      );
    });
  }

  version(debug: boolean, logServiceMessages: boolean) {
    
    let clientVersion = version;
    this.logOutputToConsole = true;

    this.writeLine("     ./&.                                       *((                             ");
    this.writeLine("     ,(#.                                       (#*                             ");
    this.writeLine("     *((..*///*    .,,         .*.    .*////*.  (#,  ,*..*//,    ,*///*,  .*.   ");
    this.writeLine("     /%#(/*,,,/%/. ./%,       /#(. ./#(/,,,,*(#/%(  ,(&#(*,.. ,(#(*,,,*/#(#%,   ");
    this.writeLine("     (%*.      /#(  ,(%,    .##*  /#/.        ,%%/  *#%,    ,(#*         /%#    ");
    this.writeLine("    *#(        /(/   .%(,  *##,  *#(          .((*  /#*.   ./&,          ,((    ");
    this.writeLine("   ./(/        #(*    ,#( (#/    *#(          ,%/.  (#     ./&,          ((/    ");
    this.writeLine("   .#(,       .%(.     *#&#*      /#/.      ,/%&*  ,#(      ,##*      .*#&#,    ");
    this.writeLine("   .#*.       ,#*      *##.        ./##(((((/*/#,  *(*        *(#((((((/,#*     ");
    this.writeLine("                     .(#/                                                       ");
    this.writeLine("                    ,##,                                                        ");
    this.writeLine("                     .                                                          ");
    this.writeLine("                                       .,*//((##%%%&&&&&&&&&&&&&%%#((/*.        ");
    this.writeLine("                             .,,*/(#&&@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#,     ");
    this.writeLine("                      ..,/#&@@@@@@@@@@@@@@@@@@@&&&&&%%%%#########(//////*       ");
    this.writeLine("                 ./%&@@@@@@@@@@@@@&&%%##(((((((((((((((((((((((/,               ");
    this.writeLine("           *(#%&@@@@@&&%%%#####((((((((((((((((((((((((((((((((,                ");
    this.writeLine("     ,*(#%&&%%%####(((((((((((((((((((((((((((((((((((((((((((*.                ");
    this.writeLine(",,*(####((((((((((((((((((((((((((((((((((((((((((((((((((((((*.                ");
    this.writeLine("#(((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((*.                ");
    this.writeLine("#((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((,                ");
    this.writeLine("#((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((/*               ");
    this.writeLine("#((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((/,.            ");
    this.writeLine("#((((((((((((((((((((((((((((((((((((((((((((((((#####%%%%%&&&&&&&&%#(**,,.     ");
    this.writeLine("#((((((((((((((((((((((((((((((((((((((((###%&&@@@@@@@@@@@@@@@@@@@@@@&&&&@@&%,  ");
    this.writeLine("#(((((((((((((((((((((((((((((((((#%%&@@@@@@@@&&&%%%####((((((((((((/.          ");
    this.writeLine("#((((((((((((((((((((((((((##%&&&&%%%######((((((((((((((((((((((((*            ");
    this.writeLine("#((((((((((((((((((((((((####(((((((((((((((((((((((((((((((((((((/,            ");
    this.writeLine("#(((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((/,            ");
    this.writeLine("#((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((/.           ");
    this.writeLine("");
    this.writeLine(`Client version v${ clientVersion } `);

    this.initializeAndConnect(debug, logServiceMessages).then(() => {
      this.agent.sendSimpleCommand(
        "getversion",
        (commandObject: CommandPacket) => {
          let appGeneratorVersion = <string>commandObject.Response;

          this.writeLine(
            `ApplicationGenerator version v${appGeneratorVersion} `
          );
          this.writeLine("Copyright 2020 CloudIDEaaS");
          this.writeLine("visit: http://www.cloudideaas.com/hydra");

          this.agent.dispose((commandObject) => {
            this.onComplete.emit("onComplete");
          });
        }
      );
    });
  }

  help() {
    let mainDefinitions = new List<{
      name: String;
      type: any;
      description: String;
    }>(commands.mainDefinitions);
    let availableCommands = mainDefinitions.where(
      (d) => d.type.name === "String"
    );
    let globalOptions = mainDefinitions.where((d) => d.type.name !== "String");
    let definitionsWithInner = mainDefinitions.where((d) =>
      Object.keys(d).includes("innerDefinitions")
    );

    let sections = [
      {
        content: commands.HELP_HEADER,
        raw: true,
      },
      {
        header: "Available Commands",
        content: availableCommands
          .select((c) => <any>{ name: c.name, summary: c.description })
          .toArray(),
      },
      {
        header: "Global Options",
        content: globalOptions
          .select((c) => <any>{ name: c.name, summary: c.description })
          .toArray(),
      },
    ];

    this.addSections(sections, definitionsWithInner);

    const usage = commandLineUsage(sections);
    this.writeExplicit(usage);
  }

  addSections(
    sections: Array<any>,
    definitions: IEnumerable<{ name: String; type: any; description: String }>
  ) {
    definitions.forEach((d) => {
      let name = d.name;
      let innerDefinitions = new List<{
        name: String;
        type: any;
        description: String;
      }>(d["innerDefinitions"]);
      let definitionsWithInner = innerDefinitions.where((d) =>
        Object.keys(d).includes("innerDefinitions")
      );
      let section = {
        header: `Commands and options for ${name}`,
        content: innerDefinitions
          .where((c) => c.description !== undefined)
          .select((c) => <any>{ name: c.name, summary: c.description })
          .toArray(),
      };

      sections.push(section);

      this.addSections(sections, definitionsWithInner);
    });
  }

  test(debug: boolean, logServiceMessages: boolean) {
    const watchMilliseconds = 1000;

    let pollInstallFromCacheStatus = async (
      mode: string,
      listener: (installsFromCacheStatus: InstallsFromCacheStatus) => void
    ) => {
      let getInstallFromCacheStatus = () => {
        this.agent.getInstallFromCacheStatus(
          mode,
          (installsFromCacheStatus: InstallsFromCacheStatus) => {
            listener(installsFromCacheStatus);

            this.writeLine(installsFromCacheStatus.StatusText);

            if (installsFromCacheStatus.TotalRemaining > 0) {
              Utils.sleep(watchMilliseconds).then(() => {
                getInstallFromCacheStatus();
              });
            } else {
              this.pollingInstallFromCacheStatusComplete = true;
            }
          }
        );
      };

      getInstallFromCacheStatus();
    };

    this.initializeAndConnect(debug, logServiceMessages).then(() => {
      this.pollingInstallFromCacheStatus = false;
      this.pollingInstallFromCacheStatusComplete = false;
      this.close = () => {
        this.agent.dispose((commandObject) => {
          this.onComplete.emit("onComplete");
        });
      };

      this.watch = (next: () => void) => {
        Utils.sleep(watchMilliseconds).then(() => {
          if (this.pollingInstallFromCacheStatusComplete) {
            next();
          } else {
            this.watch(next);
          }
        });
      };

      this.writeStatus(
        "This is a test. Nothing will actually be installed.  This tests the full connectivity of Hydra including the local Package Cache service."
      );
      pollInstallFromCacheStatus("Testing", (s) =>
        this.installsFromCacheStatusListener(s)
      );

      this.watch(() => {
        this.writeStatus("Test complete.");
        close();
      });
    });
  }

  writeLog(fileName: string, output: string, addStack: boolean = false) {
    if (!this.logPath) {
      return;
    }

    try {
      let fullFileName = path.join(this.logPath, fileName);

      if (addStack) {
        let stack = this.getTraceInfo();
        output += "\r\nStack trace:\r\n" + stack;
      }

      if (!fs.existsSync(this.logPath)) {
        fs.mkdirSync(this.logPath, { recursive: true });
      }

      fs.appendFileSync(fullFileName, output, (err) => {
        if (err) {
          return console.log(err);
        }
      });
    } catch (err) {
      console.log(err);
    }
  }

  getTraceInfo(): string {
    try {
      let trace = stackTrace.get();
      let index = 0;
      let skip = true;
      let traceInfo: string;

      while (skip && trace.length - 1 > index) {
        let functionName: string;

        index++;
        functionName = trace[index].getFunctionName();

        if (
          functionName &&
          !functionName.startsWith("write") &&
          !functionName.startsWith("addChunk") &&
          !functionName.startsWith("readableAddChunk") &&
          !functionName.startsWith("processTicksAndRejections") &&
          !trace[index].toString().startsWith("Socket.")
        ) {
          let match: RegExpMatchArray;

          traceInfo = trace[index].toString();
          match = traceInfo.match(/(?<=\().*?(?=:\d*?\:\d*?\)$)/);

          if (match && match[0]) {
            let root = path.parse(match[0]).root;
            let url = fileUrl(match[0]);

            if (root.includes(":")) {
              skip = false;
            }
          }
        }
      }

      if (traceInfo) {
        return traceInfo + "\r\n";
      } else {
        traceInfo = trace[1].toString();

        return traceInfo;
      }
    } catch (e) {
      console.log(e);
    }
  }

  writeAllLogs(
    fileName: string,
    output: string,
    generalLog: string = "general.log"
  ) {
    this.writeLog(generalLog, output);
    this.writeLog(fileName, output);
  }

  writeStatus(output: string) {
    if (this.logOutputToConsole) {
      if (!output) {
        console.error("No writeStatus output provided");
      }
    }

    this.writeLine(output);

    if (this.logClient) {
      this.writeLog("general.log", output + "\r\n");
    }
  }

  writeLine(output: string) {
    if (this.logOutputToConsole) {
      this.stdout.writeLine(output);
    }

    if (this.logClient) {
      this.writeLog("general.log", output + "\r\n");
    }
  }

  write(output: string) {
    if (this.logOutputToConsole) {
      this.stdout.write(output);
    }

    if (this.logClient) {
      this.writeLog("general.log", output);
    }
  }

  writeExplicit(output: string) {
    this.stdout.write(output);

    if (this.logClient) {
      this.writeLog("general.log", output);
    }
  }

  writeConsole(output: string) {
    if (this.logOutputToConsole) {
      console.log(output);
    }

    if (this.logClient) {
      this.writeLog("general.log", output + "\r\n");
    }
  }

  writeError(output: string) {
    if (this.logOutputToConsole) {
      let coloredOutput = colors.red(output);
      this.stderr.writeLine(coloredOutput);
    }

    if (this.logClient) {
      this.writeLog("errors.log", output + "\r\n");
    }
  }

  writeWarning(output: string) {
    if (this.logOutputToConsole) {
      let coloredOutput = colors.yellow(output);
      this.stderr.writeLine(coloredOutput);
    }

    if (this.logClient) {
      this.writeLog("general.log", output + "\r\n");
    }
  }

  writeSuccess(output: string) {
    if (this.logOutputToConsole) {
      let coloredOutput = colors.green(output);
      this.stdout.writeLine(coloredOutput);
    }

    if (this.logClient) {
      this.writeLog("general.log", output + "\r\n");
    }
  }

  installsFromCacheStatusListener(
    installsFromCacheStatus: InstallsFromCacheStatus
  ) {
    if (installsFromCacheStatus.InstallFromCacheStatus.length > 0) {
      installsFromCacheStatus.InstallFromCacheStatus.forEach((i) => {
        if (i.StatusIsError) {
          this.writeError(i.StatusText);
        } else if (i.StatusIsSuccess) {
          this.writeSuccess(i.StatusText);
        } else {
          this.writeLine(i.StatusText);
        }
      });
    } else {
      if (installsFromCacheStatus.StatusIsError) {
        this.writeError(installsFromCacheStatus.StatusSummary);
      } else if (installsFromCacheStatus.StatusIsSuccess) {
        this.writeSuccess(installsFromCacheStatus.StatusSummary);
      } else if (installsFromCacheStatus.StatusSummary) {
        this.writeLine(installsFromCacheStatus.StatusSummary);
      }
    }
  }
}
