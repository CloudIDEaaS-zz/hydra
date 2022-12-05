"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.BaseStandardStreamService = void 0;
const timespan = require('timespan');
const { EOL } = require('os');
const StringBuilder = require("string-builder");
const baseThreadedService_1 = require("./baseThreadedService");
class BaseStandardStreamService extends baseThreadedService_1.BaseThreadedService {
    stdout;
    stdin;
    resourceManager;
    constructor(resourceManager) {
        super(new timespan.fromMilliseconds(100));
        this.stdout = process.stdout;
        this.stdin = process.stdin;
        this.resourceManager = resourceManager;
    }
    async DoWork(stopping) {
        try {
            let commandPacket = await this.readJsonCommand(this.stdin);
            this.WriteLine(`Handling command. ${commandPacket.Command} `);
            this.HandleCommand(commandPacket);
        }
        catch (err) {
            console.error(err);
        }
    }
    async readJsonCommand(socket) {
        this.WriteLine(this.resourceManager.HydraCli.Waiting_for_json_command);
        let jsonText = await this.readUntil(socket, EOL + EOL);
        let commandPacket = JSON.parse(jsonText);
        this.WriteLine(`Command retrieved. ${commandPacket.Command} `);
        return commandPacket;
    }
    writeJsonCommand(commandPacket) {
        let json = JSON.stringify(commandPacket);
        this.stdout.write(json + EOL + EOL);
    }
    readUntil(socket, terminator, excludeTerminator = false, limit = -1) {
        let promise = new Promise((resolve, reject) => {
            let data = new StringBuilder();
            let terminatorIndex = 0;
            let counter = 0;
            let read = true;
            socket.on('readable', (s) => {
                while (read) {
                    let size = 1;
                    let buffer = socket.read(size);
                    if (buffer) {
                        data.append(String.fromCharCode(...buffer));
                        counter++;
                        if (counter === limit) {
                            return data.toString();
                        }
                        if (buffer[0] === terminator.charCodeAt(terminatorIndex)) {
                            terminatorIndex++;
                            if (terminatorIndex === terminator.length) {
                                let text;
                                if (excludeTerminator) {
                                    data.remove(data.length - terminator.length, terminator.length);
                                }
                                text = data.toString();
                                if (text.trimEnd().length > 0) {
                                    read = false;
                                    socket.removeAllListeners();
                                    resolve(text);
                                }
                                else {
                                    return;
                                }
                            }
                        }
                        else {
                            terminatorIndex = 0;
                        }
                    }
                }
            });
        });
        return promise;
    }
}
exports.BaseStandardStreamService = BaseStandardStreamService;
//# sourceMappingURL=baseStandardStreamService.js.map