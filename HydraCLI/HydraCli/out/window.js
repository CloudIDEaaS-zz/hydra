const electron = require("electron");
const app = electron.app;
const BrowserWindow = electron.BrowserWindow;
const BrowserView = electron.BrowserView;
const uuid = require("uuid");
const net = require('net');
const { copyFileSync } = require("fs");
let mainWindow;
let view;
let args;
let url;
const port = parseInt(process.argv[3]);
app.on('ready', function () {
    mainWindow = new BrowserWindow({
        width: 600,
        height: 600,
        parent: "top",
        frame: false,
        movable: true,
        resizable: true,
        skipTaskbar: false,
        hasShadow: true,
        alwaysOnTop: true,
        modal: true,
        title: "Hydra Connect",
        icon: "icons/Hydra.ico"
    });
    view = new BrowserView();
    mainWindow.setBrowserView(view);
    view.setAutoResize({ width: true, height: true, horizontal: true, vertical: true });
    mainWindow.hookWindowMessage(0x004A, (w, l) => {
        console.log("received message");
        setTimeout(() => {
            const client = net.connect(port, () => {
                console.log("connected to parent service");
            })
                .on('data', (data) => {
                let keyBytes;
                let key;
                let urlBytes;
                let url;
                console.log("received data from parent");
                try {
                    keyBytes = data.slice(0, 16);
                    key = uuid.stringify(keyBytes);
                    console.log("extracted key");
                    urlBytes = data.slice(16, data.length);
                    console.log("extracted url");
                    url = String.fromCharCode(...urlBytes);
                    url = url + "/Verify?key=" + key;
                    console.log(`navigating to ${url}`);
                    view.webContents.on("did-start-navigation", function (e) {
                        view.webContents.session.webRequest.onCompleted({ urls: ["<all_urls>"] }, function (details) {
                            console.log(`${details.statusCode} response from ${details.url}`);
                        });
                        view.webContents.session.webRequest.onErrorOccurred({ urls: ["<all_urls>"] }, function (details) {
                            console.error(`Error: ${details.error} from ${details.url}`);
                        });
                        view.webContents.session.webRequest.onHeadersReceived({ urls: ["<all_urls>"] }, function (details, callback) {
                            let headers = details.responseHeaders;
                            if (headers.hasOwnProperty("set-cookie")) {
                                try {
                                    let cookies = headers["set-cookie"];
                                    if (cookies) {
                                        for (let cookie of cookies) {
                                            process.stdout.write(cookie + "\r\n");
                                        }
                                    }
                                }
                                catch (error) {
                                    console.log(error);
                                }
                            }
                            else if (headers.hasOwnProperty("Set-Cookie")) {
                                try {
                                    let cookies = headers["Set-Cookie"];
                                    if (cookies) {
                                        for (let cookie of cookies) {
                                            process.stdout.write(cookie + "\r\n");
                                        }
                                    }
                                }
                                catch (error) {
                                    console.log(error);
                                }
                            }
                            callback({ cancel: false, responseHeaders: headers });
                        });
                    });
                }
                catch (error) {
                    console.error(error);
                }
                view.webContents.on("did-fail-load", function (e) {
                    console.error(e.errorDescription);
                });
                view.webContents.on("did-finish-load", function (e) {
                    try {
                        let title = view.webContents.getTitle();
                        switch (title) {
                            case "Verify":
                                console.log(title);
                                break;
                            case "Log in - User Management Services":
                            case "Privacy Policy - User Management Services":
                            case "Home page - User Management Services":
                            case "Register - User Management Services":
                                {
                                    let newWidth = 720;
                                    let newHeight = 900;
                                    let bounds = mainWindow.getBounds();
                                    let deltaWidth = (newWidth / 2) - (bounds.width / 2);
                                    let deltaHeight = (newHeight / 2) - (bounds.height / 2);
                                    console.log(title);
                                    mainWindow.setBounds({ x: bounds.x - deltaWidth, y: bounds.y - deltaHeight, width: newWidth, height: newHeight });
                                    //view.setBounds({ x: 0, y: 0, width: newWidth, height: newHeight });
                                }
                                break;
                            default: {
                                let uuidString = uuid.stringify(uuid.parse(title));
                                process.stdout.write(uuidString);
                            }
                        }
                    }
                    catch (error) {
                        console.log(error);
                    }
                });
                view.webContents.loadURL(url);
            })
                .on('error', (err) => {
                console.log(err);
                throw err;
            });
        }, 100);
    });
    args = process.argv.slice(2);
    view.setBounds({ x: 0, y: 0, width: 600, height: 600 });
    mainWindow.on('closed', function () {
        mainWindow = null;
    });
});
//# sourceMappingURL=window.js.map