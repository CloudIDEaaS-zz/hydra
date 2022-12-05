import { Socket } from "net";
import path = require("path");
import { Utils } from "../modules/utils/utils";
import { StandardStreamService } from "./standardStreamService";
import resourceManager, { HydraCli } from "../resources/resourceManager";
import { Page, Browser, launch } from "puppeteer";
import { ApplicationGeneratorClient } from "./client";
const commandLineArgs = require('command-line-args');
const fs = require('fs');

export class Renderer {
    stdout: Socket;
    stderr: Socket;
    static renderer: Renderer;
    standardStreamService: StandardStreamService;
    browser: Browser;
    page: Page;
    client: ApplicationGeneratorClient;
    resourceManager: resourceManager;

    constructor() {
        this.stdout = <Socket>process.stdout;
        this.stderr = <Socket>process.stderr;
    }
   
    public static launchRenderer(resourceManager : resourceManager) {
        let client: Renderer;

        if (Renderer.renderer === undefined) {
            Renderer.renderer = new Renderer();
        }

        this.renderer = Renderer.renderer;
        this.renderer.client = ApplicationGeneratorClient.client;
        this.renderer.resourceManager = resourceManager;
        this.renderer.launchRenderer();
    }

    private launchRenderer() {

        this.client.writeLine("Initializing renderer. Starting standardStreamService");

        this.standardStreamService = new StandardStreamService(this, this.resourceManager);

        this.standardStreamService.Start();
        this.watchRenderer();
    }
        
    private escapeRegExp(string) {
        return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); // $& means the whole matched string
    }

    private replaceAll(str, find, replace) {
        return str.replace(new RegExp(this.escapeRegExp(find), 'g'), replace);
    }    

    private setErrorStatus(err: string) {
        this.page.evaluate(() => {
            document.body.innerHTML = null;
            document.writeln(JSON.stringify({ status: err }));
        });
    }

    public initialize(rootPath: string, startupScriptBlock: string, rootWebAddress: string, headless: boolean) : Promise<string> {

        let promise = new Promise<string>((resolve, reject) => {

            (async () => {

                try {
                    
                    let error: string;

                    this.client.writeLine("Launching headless browser"); 

                    this.browser = await launch({ headless: headless, args: ['--disable-web-security'] });
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
                            
                            this.client.writeLine(`Request received for ${ url }`);
                            this.client.writeLine(`Translated to file ${ file }`);
    
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
    
                            let error = `Unexpected resource type ${ resourceType }`;
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

    loadModule(moduleUrl: string, componentName: string, instanceId: number): Promise<string> {

        let promise = new Promise<string>((resolve, reject) => {

            (async() => {

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

    updateInstance(instanceId: number, propertyValues: any): Promise<string> {

        let promise = new Promise<string>((resolve, reject) => {

            (async() => {

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

    getFullSnapshot(): Promise<string> {

        let promise = new Promise<string>((resolve, reject) => {

            (async() => {

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

    private watchRenderer() {

        Utils.sleep(500).then(() => {
            
            if (this.standardStreamService.IsRunning) {
                this.watchRenderer();
            }
        });
    }
}