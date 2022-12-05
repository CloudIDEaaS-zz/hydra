"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.StandardStreamService = void 0;
const baseStandardStreamService_1 = require("./baseStandardStreamService");
const commandPacket_1 = require("./commandPacket");
const commands_1 = require("./commands");
class StandardStreamService extends baseStandardStreamService_1.BaseStandardStreamService {
    renderer;
    client;
    constructor(renderer, resourceManager) {
        super(resourceManager);
        this.renderer = renderer;
        this.client = renderer.client;
    }
    GetValue(keyPair) {
        return keyPair[Object.keys(keyPair)[0]];
    }
    HandleCommand(commandPacket) {
        try {
            switch (commandPacket.Command) {
                case commands_1.rendererServerCommands.CONNECT:
                    {
                        commandPacket = new commandPacket_1.CommandPacketResponse(commandPacket.Command, "Connected successfully");
                        this.writeJsonCommand(commandPacket);
                    }
                    break;
                case commands_1.rendererServerCommands.INITIALIZE_RENDERER:
                    {
                        let rootPath = this.GetValue(commandPacket.Arguments[0]);
                        let startupScriptBlock = this.GetValue(commandPacket.Arguments[1]);
                        let rootWebAddress = this.GetValue(commandPacket.Arguments[2]);
                        let headless = this.GetValue(commandPacket.Arguments[3]);
                        let promise = this.renderer.initialize(rootPath, startupScriptBlock, rootWebAddress, headless);
                        promise.then(s => {
                            commandPacket = new commandPacket_1.CommandPacketResponse(commandPacket.Command, s);
                            this.writeJsonCommand(commandPacket);
                        }, e => {
                            commandPacket = new commandPacket_1.CommandPacketResponse(commandPacket.Command, e);
                            commandPacket.Response = e;
                            this.writeJsonCommand(commandPacket);
                        });
                    }
                    break;
                case commands_1.rendererServerCommands.LOAD_MODULE:
                    {
                        let moduleUrl = this.GetValue(commandPacket.Arguments[0]);
                        let componentName = this.GetValue(commandPacket.Arguments[1]);
                        let instanceId = parseInt(this.GetValue(commandPacket.Arguments[2]));
                        let promise = this.renderer.loadModule(moduleUrl, componentName, instanceId);
                        promise.then(s => {
                            commandPacket = new commandPacket_1.CommandPacketResponse(commandPacket.Command, s);
                            this.writeJsonCommand(commandPacket);
                        }, e => {
                            commandPacket = new commandPacket_1.CommandPacketResponse(commandPacket.Command, e);
                            commandPacket.Response = e;
                            this.writeJsonCommand(commandPacket);
                        });
                    }
                    break;
                case commands_1.rendererServerCommands.UPDATE_INSTANCE:
                    {
                        let instanceId = parseInt(this.GetValue(commandPacket.Arguments[0]));
                        let propertyValues = this.GetValue(commandPacket.Arguments[1]);
                        let promise = this.renderer.updateInstance(instanceId, propertyValues);
                        promise.then(s => {
                            commandPacket = new commandPacket_1.CommandPacketResponse(commandPacket.Command, s);
                            this.writeJsonCommand(commandPacket);
                        }, e => {
                            commandPacket = new commandPacket_1.CommandPacketResponse(commandPacket.Command, e);
                            commandPacket.Response = e;
                            this.writeJsonCommand(commandPacket);
                        });
                    }
                    break;
                case commands_1.rendererServerCommands.GET_FULL_SNAPSHOT:
                    {
                        let promise = this.renderer.getFullSnapshot();
                        promise.then(s => {
                            commandPacket = new commandPacket_1.CommandPacketResponse(commandPacket.Command, s);
                            this.writeJsonCommand(commandPacket);
                        }, e => {
                            commandPacket = new commandPacket_1.CommandPacketResponse(commandPacket.Command, e);
                            commandPacket.Response = e;
                            this.writeJsonCommand(commandPacket);
                        });
                    }
                    break;
                case commands_1.rendererServerCommands.PING:
                    {
                        commandPacket = new commandPacket_1.CommandPacketResponse(commandPacket.Command, "Success");
                        this.writeJsonCommand(commandPacket);
                    }
                    break;
                case commands_1.rendererServerCommands.TERMINATE:
                    this.Stop();
                    process.abort();
                    break;
            }
        }
        catch (err) {
            console.error(err);
        }
    }
    WriteLine(output) {
        this.client.writeLine(output);
    }
    Write(output) {
        this.client.write(output);
    }
    WriteError(output) {
        this.client.writeError(output);
    }
    WriteWarning(output) {
        this.client.writeWarning(output);
    }
    WriteSuccess(output) {
        this.client.writeSuccess(output);
    }
    Start() {
        super.Start();
    }
}
exports.StandardStreamService = StandardStreamService;
//# sourceMappingURL=standardStreamService.js.map