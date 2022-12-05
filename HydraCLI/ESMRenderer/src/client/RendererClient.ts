import { Socket } from "net";
import { Page, Browser, launch } from "puppeteer";
import path = require("path");
const fs = require('fs');

export class RendererClient {

    stdout: Socket;
    stderr: Socket;
    static client: RendererClient;

    constructor() {
        this.stdout = <Socket>process.stdout;
        this.stderr = <Socket>process.stderr;
    }

    public static start() {

        let client: RendererClient;
        let jsFile = process.argv[2];

        if (RendererClient.client === undefined) {
            RendererClient.client = new RendererClient();
        }

        client = RendererClient.client;

        client.render(jsFile);
    }
        
    escapeRegExp(string) {
        return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); // $& means the whole matched string
    }

    replaceAll(str, find, replace) {
        return str.replace(new RegExp(this.escapeRegExp(find), 'g'), replace);
    }    

    render(jsFile: string) {

        (async () => {
            try {
                let browser = await launch({ headless: false, args: ['--disable-web-security'] });
                let page = await browser.newPage();
                let content = `
                    try {
                        let promise = import(\"${ jsFile }\");
    
                        promise.then(m => {
                            document.write(\"Success!\");
                        }, e => {
                            document.write(e);
                        });
                    }
                    catch (e) {
                        document.write(e);
                    }
                `;

                await page.setRequestInterception(true);

                page.on("pageerror", (err) => {  
                    let message = err.toString();
                    console.log("Page error: " + message); 
                });

                page.on("error", (err) => {  
                    let message = err.toString();
                    console.log("Page error: " + message); 
                });

                page.on('request', async (request) => {
                  if (request.resourceType() == 'script') {
                    let relativePath = this.replaceAll(this.replaceAll(request.url(), "http://localhost/", ""), "/", "\\");
                    let file = "D:\\MC\\CloudIDEaaS\\develop\\TestESM\\" + relativePath;
                    
                    fs.readFile(file, "utf8", async (err, content) => {
                        if (err) {
                            request.respond({ status: 404, body: err });
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
                    await request.respond({
                        contentType: 'application/json',
                        body: "Hellooo!"
                      })
                  }
                });

                //await page.setContent("<script src=\"http://localhost/script.js\"></script>");

                await page.addScriptTag({ content: content });
                await page.waitForTimeout(10000);

                content = await page.content();

                await browser.close();
                console.log();
            }
            catch (e) {
                console.error(e);
            }

            console.log();

          })();        
   }
}