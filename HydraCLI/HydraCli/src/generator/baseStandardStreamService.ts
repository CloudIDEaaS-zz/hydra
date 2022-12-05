const timespan = require('timespan');
const {EOL} = require('os');
const StringBuilder = require("string-builder");
import { Socket } from "net";
import { Utils } from "../modules/utils/utils";
import { BaseThreadedService } from "./baseThreadedService";
import { CommandPacket } from "./commandPacket";
import resourceManager, { HydraCli } from "../resources/resourceManager";

export abstract class BaseStandardStreamService extends BaseThreadedService {
    private stdout: Socket;
    private stdin: Socket;
    public resourceManager: resourceManager;
    protected abstract HandleCommand(commandPacket : CommandPacket);

    constructor(resourceManager: resourceManager) {
        super(new timespan.fromMilliseconds(100));
        this.stdout = <Socket>process.stdout;
        this.stdin = <Socket>process.stdin;
        this.resourceManager = resourceManager;
    }

    public async DoWork(stopping: boolean) : Promise<void> {
        
        try {
            
            let commandPacket = await this.readJsonCommand(this.stdin);

            this.WriteLine(`Handling command. ${ commandPacket.Command } `);

            this.HandleCommand(commandPacket);
        }
        catch (err) {
            console.error(err);
        }
    }

    async readJsonCommand(socket : Socket) : Promise<CommandPacket> {
        
        this.WriteLine(this.resourceManager.HydraCli.Waiting_for_json_command);

        let jsonText = await this.readUntil(socket, EOL + EOL);
        let commandPacket = <CommandPacket> JSON.parse(jsonText);

        this.WriteLine(`Command retrieved. ${ commandPacket.Command } `);

        return commandPacket;
    }

    writeJsonCommand(commandPacket: CommandPacket) {
        let json = JSON.stringify(commandPacket);
        this.stdout.write(json + EOL + EOL);
    }

    readUntil(socket: Socket, terminator: string, excludeTerminator: boolean = false, limit: number = -1) : Promise<string> {

        let promise = new Promise<string>((resolve, reject) => {

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
                                
                                let text : string;

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
