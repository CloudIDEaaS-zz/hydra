import { BaseStandardStreamService } from "./baseStandardStreamService";
import { ApplicationGeneratorClient } from "./client";
import { CommandPacket, CommandPacketResponse } from "./commandPacket";
import { rendererServerCommands } from "./commands";
import { Renderer } from "./renderer";

export class StandardStreamService extends BaseStandardStreamService {
    client: ApplicationGeneratorClient;

    constructor(public renderer: Renderer) {
        super();
        this.client = renderer.client;
    }

    private GetValue(keyPair : any) : any {
        return keyPair[Object.keys(keyPair)[0]];
    }

    protected HandleCommand(commandPacket: CommandPacket) {
        try {
            switch (commandPacket.Command) {
                case rendererServerCommands.CONNECT: {
                
                        commandPacket = new CommandPacketResponse(commandPacket.Command, "Connected successfully");
                        this.writeJsonCommand(commandPacket);
                    }
                    break;
                case rendererServerCommands.INITIALIZE_RENDERER: {
                
                        let rootPath: string = this.GetValue(commandPacket.Arguments[0]);
                        let startupScriptBlock: string = this.GetValue(commandPacket.Arguments[1]);
                        let rootWebAddress: string = this.GetValue(commandPacket.Arguments[2]);
                        let headless: boolean = this.GetValue(commandPacket.Arguments[3]);
                        let promise : Promise<string> = this.renderer.initialize(rootPath, startupScriptBlock, rootWebAddress, headless);

                        promise.then(s => {
                            commandPacket = new CommandPacketResponse(commandPacket.Command, s);

                            this.writeJsonCommand(commandPacket);
                        }, e => {
                            commandPacket = new CommandPacketResponse(commandPacket.Command, e);
                            commandPacket.Response = e;
                            
                            this.writeJsonCommand(commandPacket);
                        });

                    }
                    break;
                case rendererServerCommands.LOAD_MODULE: {
                
                        let moduleUrl: string = this.GetValue(commandPacket.Arguments[0]);
                        let componentName: string = this.GetValue(commandPacket.Arguments[1]);
                        let instanceId: number = parseInt(this.GetValue(commandPacket.Arguments[2]));
                        let promise : Promise<string> = this.renderer.loadModule(moduleUrl, componentName, instanceId);

                        promise.then(s => {
                            commandPacket = new CommandPacketResponse(commandPacket.Command, s);

                            this.writeJsonCommand(commandPacket);
                        }, e => {
                            commandPacket = new CommandPacketResponse(commandPacket.Command, e);
                            commandPacket.Response = e;
                            
                            this.writeJsonCommand(commandPacket);
                        });

                    }
                    break;
                case rendererServerCommands.UPDATE_INSTANCE: {
                
                        let instanceId: number = parseInt(this.GetValue(commandPacket.Arguments[0]));
                        let propertyValues: string = this.GetValue(commandPacket.Arguments[1]);
                        let promise : Promise<string> = this.renderer.updateInstance(instanceId, propertyValues);

                        promise.then(s => {
                            commandPacket = new CommandPacketResponse(commandPacket.Command, s);

                            this.writeJsonCommand(commandPacket);
                        }, e => {
                            commandPacket = new CommandPacketResponse(commandPacket.Command, e);
                            commandPacket.Response = e;
                            
                            this.writeJsonCommand(commandPacket);
                        });

                    }
                    break;
                case rendererServerCommands.GET_FULL_SNAPSHOT: {
                
                        let promise : Promise<string> = this.renderer.getFullSnapshot();

                        promise.then(s => {
                            commandPacket = new CommandPacketResponse(commandPacket.Command, s);

                            this.writeJsonCommand(commandPacket);
                        }, e => {
                            commandPacket = new CommandPacketResponse(commandPacket.Command, e);
                            commandPacket.Response = e;
                            
                            this.writeJsonCommand(commandPacket);
                        });

                    }
                    break;
                case rendererServerCommands.PING: {
                
                        commandPacket = new CommandPacketResponse(commandPacket.Command, "Success");
                        this.writeJsonCommand(commandPacket);
                    }
                    break;
                case rendererServerCommands.TERMINATE:

                    this.Stop();
                    process.abort();
                    break;
            }
        }
        catch (err) {
            console.error(err);
        }
    }
    
    public WriteLine(output: string) {
        this.client.writeLine(output);
    }
    public Write(output: string) {
        this.client.write(output);
    }
    public WriteError(output: string) {
        this.client.writeError(output);
    }
    public WriteWarning(output: string) {
        this.client.writeWarning(output);
    }
    public WriteSuccess(output: string) {
        this.client.writeSuccess(output);
    }

    public Start() { 
        super.Start();
    }
}