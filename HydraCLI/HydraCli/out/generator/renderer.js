"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Renderer = void 0;
const utils_1 = require("../modules/utils/utils");
const standardStreamService_1 = require("./standardStreamService");
const puppeteer_1 = require("puppeteer");
const client_1 = require("./client");
const commandLineArgs = require('command-line-args');
const fs = require('fs');
class Renderer {
    stdout;
    stderr;
    static renderer;
    standardStreamService;
    browser;
    page;
    client;
    resourceManager;
    constructor() {
        this.stdout = process.stdout;
        this.stderr = process.stderr;
    }
    static launchRenderer(resourceManager) {
        let client;
        if (Renderer.renderer === undefined) {
            Renderer.renderer = new Renderer();
        }
        this.renderer = Renderer.renderer;
        this.renderer.client = client_1.ApplicationGeneratorClient.client;
        this.renderer.resourceManager = resourceManager;
        this.renderer.launchRenderer();
    }
    launchRenderer() {
        this.client.writeLine("Initializing renderer. Starting standardStreamService");
        this.standardStreamService = new standardStreamService_1.StandardStreamService(this, this.resourceManager);
        this.standardStreamService.Start();
        this.watchRenderer();
    }
    escapeRegExp(string) {
        return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); // $& means the whole matched string
    }
    replaceAll(str, find, replace) {
        return str.replace(new RegExp(this.escapeRegExp(find), 'g'), replace);
    }
    setErrorStatus(err) {
        this.page.evaluate(() => {
            document.body.innerHTML = null;
            document.writeln(JSON.stringify({ status: err }));
        });
    }
    initialize(rootPath, startupScriptBlock, rootWebAddress, headless) {
        let promise = new Promise((resolve, reject) => {
            (async () => {
                try {
                    let error;
                    this.client.writeLine("Launching headless browser");
                    this.browser = await (0, puppeteer_1.launch)({ headless: headless, args: ['--disable-web-security'] });
                    this.page = await this.browser.newPage();
                    await this.page.setRequestInterception(true);
                    this.page.on("pageerror", (err) => {
                        error = err.toString();
                        this.setErrorStatus(error);
                        this.client.writeError("Page error: " + error);
                    });
                    this.page.on("error", (err) => {
                        error = err.toString();
                        this.setErrorStatus(error);
                        this.client.writeError("Page error: " + error);
                    });
                    this.page.on('request', async (request) => {
                        let resourceType = request.resourceType();
                        if (resourceType === 'script') {
                            let url = request.url();
                            let relativePath = this.replaceAll(this.replaceAll(url, rootWebAddress, ""), "/", "\\");
                            let file = rootPath + relativePath;
                            this.client.writeLine(`Request received for ${url}`);
                            this.client.writeLine(`Translated to file ${file}`);
                            fs.readFile(file, "utf8", async (err, content) => {
                                if (err) {
                                    request.respond({ status: 404, body: err });
                                    this.setErrorStatus(error);
                                    this.client.writeError("File read error: " + err);
                                }
                                else {
                                    await request.respond({
                                        status: 200,
                                        contentType: 'application/javascript; charset=utf-8',
                                        body: content
                                    });
                                }
                            });
                        }
                        else {
                            let error = `Unexpected resource type ${resourceType}`;
                            this.client.writeWarning(error);
                            await request.respond({
                                contentType: 'text/html',
                                body: error
                            });
                        }
                    });
                    await this.page.setContent(startupScriptBlock);
                    await this.page.waitForTimeout(100);
                    let content = await this.page.content();
                    this.client.writeLine("Content ---------------------------------\r\n");
                    this.client.writeLine(content);
                    this.client.writeLine("\r\nEnd content ---------------------------------");
                    resolve(encodeURIComponent(content));
                }
                catch (e) {
                    this.setErrorStatus(e);
                    this.client.writeError(e);
                    reject(await this.page.content());
                }
            })();
        });
        return promise;
    }
    loadModule(moduleUrl, componentName, instanceId) {
        let promise = new Promise((resolve, reject) => {
            (async () => {
                try {
                    await this.page.evaluate((moduleUrl, componentName, instanceId) => {
                        window["cloudideaas"].loadModule(moduleUrl, componentName, instanceId);
                    }, moduleUrl, componentName, instanceId);
                    await this.page.waitForTimeout(100);
                    let content = await this.page.content();
                    this.client.writeLine("Content ---------------------------------\r\n");
                    this.client.writeLine(content);
                    this.client.writeLine("\r\nEnd content ---------------------------------");
                    resolve(encodeURIComponent(content));
                }
                catch (e) {
                    this.setErrorStatus(e);
                    this.client.writeError(e);
                    reject(await this.page.content());
                }
            })();
        });
        return promise;
    }
    updateInstance(instanceId, propertyValues) {
        let promise = new Promise((resolve, reject) => {
            (async () => {
                try {
                    await this.page.evaluate((instanceId, propertyValues) => {
                        window["cloudideaas"].updateInstance(instanceId, propertyValues);
                    }, instanceId, propertyValues);
                    await this.page.waitForTimeout(100);
                    let content = await this.page.content();
                    this.client.writeLine("Content ---------------------------------\r\n");
                    this.client.writeLine(content);
                    this.client.writeLine("\r\nEnd content ---------------------------------");
                    resolve(encodeURIComponent(content));
                }
                catch (e) {
                    this.setErrorStatus(e);
                    this.client.writeError(e);
                    reject(await this.page.content());
                }
            })();
        });
        return promise;
    }
    getFullSnapshot() {
        let promise = new Promise((resolve, reject) => {
            (async () => {
                try {
                    await this.page.evaluate((instanceId, propertyValues) => {
                        window["cloudideaas"].getFullSnapshot();
                    });
                    await this.page.waitForTimeout(100);
                    let content = await this.page.content();
                    this.client.writeLine("Content ---------------------------------\r\n");
                    this.client.writeLine(content);
                    this.client.writeLine("\r\nEnd content ---------------------------------");
                    resolve(encodeURIComponent(content));
                }
                catch (e) {
                    this.setErrorStatus(e);
                    this.client.writeError(e);
                    reject(await this.page.content());
                }
            })();
        });
        return promise;
    }
    watchRenderer() {
        utils_1.Utils.sleep(500).then(() => {
            if (this.standardStreamService.IsRunning) {
                this.watchRenderer();
            }
        });
    }
}
exports.Renderer = Renderer;
//# sourceMappingURL=renderer.js.map