"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.npm = void 0;
const Promise = require('bluebird');
const exec = require('child_process').exec;
const colors = require('colors/safe');
const path = require('path');
// see C:\Program Files\nodejs\node_modules\npm\lib\cli.js
class npm {
    static stdout;
    static stderr;
    static globalDir;
    static getErrorHandler() {
        // var errorHandler = require(path.join(npmPath, "lib/utils/error-handler.js"));
        // return errorHandler;
    }
    static getLoaded() {
        // var npm = <any> require(path.join(npmPath, "lib/npm.js"));
        // return npm.config.loaded;
        return false;
    }
    static install2(packages, opts) {
        // var Npm = <any> require(path.join(npmPath, "lib/npm.js"));
        // var npm = new Npm();
        // var npmconf = <any> require(path.join(npmPath, "lib/utils/config/index.js"));
        // var args = [];
        // var configDefs = npmconf.definitions;
        // var shorthands = configDefs.shorthands;
        // var types = configDefs.types;
        // var nopt = require('nopt');
        // if (opts.lean) {
        //     args = args.concat(["--no-optional"]);
        //     args = args.concat(["--prod"]);
        //     args = args.concat(["--no-progress"]);
        //     args = args.concat(["-loglevel"], ["silent"]);
        //     args = args.concat(["--prefer-offline"]);
        //     args = args.concat(["--no-audit"]);
        //     args = args.concat(["--no-fund"]);
        //  }
        // var conf = nopt(types, shorthands, args);
        // return new Promise((resolve, reject) => {
        //     npm.load(conf, (er) => {
        //         if (er) {
        //             return reject(er);
        //         }
        //         if (packages.length === 0 || !packages || !packages.length) {
        //             return reject("No packages found");
        //         }
        //         if (typeof packages === "string") {
        //             packages = [packages];
        //         }
        //         if (!opts) {
        //             opts = {};
        //         }
        //         if (opts.global) { 
        //             npm.config.set('global', true);
        //         }
        //         if (opts.saveDev) {
        //             npm.config.set('save-dev', true);
        //         }
        //         cmd.call(npm, packages, (err) => {
        //             if (err) {
        //                 return reject(err);
        //             }
        //             else {
        //                 resolve(true);
        //             }
        //         });
        //     });
        // });
    }
    static install(packages, opts) {
        if (packages.length === 0 || !packages || !packages.length) {
            return Promise.reject("No packages found");
        }
        if (typeof packages === "string") {
            packages = [packages];
        }
        if (!opts) {
            opts = {};
        }
        let rootCmd;
        rootCmd = "npm install";
        let cmdString = rootCmd + " " + packages.join(" ") + " "
            + (opts.global ? " -g" : "")
            + (opts.save ? " --save" : "")
            + (opts.saveDev ? " --save-dev" : "")
            + (opts.legacyBundling ? " --legacy-bundling" : "")
            + (opts.noOptional ? " --no-optional" : "")
            + (opts.ignoreScripts ? " --ignore-scripts" : "");
        return new Promise(function (resolve, reject) {
            let cmd = exec(cmdString, { cwd: opts.cwd ? opts.cwd : "/", maxBuffer: opts.maxBuffer ? opts.maxBuffer : 200 * 1024 }, (error, stdout, stderr) => {
                if (error) {
                    reject(error);
                }
                else {
                    resolve(true);
                }
            });
            if (opts.output) {
                cmd.stdout.on('data', (o) => { npm.writeLine(o); });
                cmd.stderr.on('data', (e) => { npm.writeError(e); });
            }
            npm.stdout = cmd.stdout;
            npm.stderr = cmd.stderr;
        });
    }
    static set(key, value) {
        let rootCmd = "npm set";
        let cmdString = rootCmd + " " + key + " " + value;
        return new Promise(function (resolve, reject) {
            let cmd = exec(cmdString, (error, stdout, stderr) => {
                if (error) {
                    reject(error);
                }
                else {
                    resolve(true);
                }
            });
            cmd.stdout.on('data', (o) => { npm.writeLine(o); });
            cmd.stderr.on('data', (e) => { npm.writeError(e); });
            npm.stdout = cmd.stdout;
            npm.stderr = cmd.stderr;
        });
    }
    static uninstall(packages, opts) {
        let stdout;
        let stderr;
        if (packages.length === 0 || !packages || !packages.length) {
            return Promise.reject("No packages found");
        }
        if (typeof packages === "string") {
            packages = [packages];
        }
        if (!opts) {
            opts = {};
        }
        let cmdString = "npm uninstall " + packages.join(" ") + " "
            + (opts.global ? " -g" : "")
            + (opts.save ? " --save" : "")
            + (opts.saveDev ? " --saveDev" : "");
        return new Promise(function (resolve, reject) {
            var cmd = exec(cmdString, { cwd: opts.cwd ? opts.cwd : "/" }, (error, stdout, stderr) => {
                if (error) {
                    reject(error);
                }
                else {
                    resolve(true);
                }
            });
            if (opts.output) {
                cmd.stdout.on('data', (o) => { npm.writeLine(o); });
                cmd.stderr.on('data', (e) => { npm.writeError(e); });
            }
            npm.stdout = cmd.stdout;
            npm.stderr = cmd.stderr;
        });
    }
    static list(path) {
        let global = false;
        let stdout;
        let stderr;
        if (!path) {
            global = true;
        }
        let cmdString = "npm ls --depth=0 " + (global ? "-g " : " ");
        return new Promise(function (resolve, reject) {
            exec(cmdString, { cwd: path ? path : "/" }, (error, stdout, stderr) => {
                if (stderr === "") {
                    if (stderr.indexOf("missing") === -1 && stderr.indexOf("required") === -1) {
                        return reject(error);
                    }
                }
                let packages = [];
                packages = stdout.split('\n');
                packages = packages.filter((item) => {
                    if (item.match(/^├──.+/g) !== null) {
                        return true;
                    }
                    if (item.match(/^└──.+/g) !== null) {
                        return true;
                    }
                    return undefined;
                });
                packages = packages.map(function (item) {
                    if (item.match(/^├──.+/g) !== null) {
                        return item.replace(/^├──\s/g, "");
                    }
                    if (item.match(/^└──.+/g) !== null) {
                        return item.replace(/^└──\s/g, "");
                    }
                });
                resolve(packages);
            });
        });
    }
    static writeLine(output) {
        npm.stdout.writeLine(output);
    }
    static writeError(output) {
        npm.stderr.writeLine(colors.red(output));
    }
}
exports.npm = npm;
//# sourceMappingURL=npmInstall.js.map