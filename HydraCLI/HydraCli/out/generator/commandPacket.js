"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.CommandPacketResponse = exports.CommandPacket = void 0;
class CommandPacket {
    constructor(Type, Command, Arguments) {
        this.Type = Type;
        this.Command = Command;
        this.Arguments = Arguments;
        this.SentTimestamp = new Date(Date.now());
    }
}
exports.CommandPacket = CommandPacket;
class CommandPacketResponse extends CommandPacket {
    constructor(command, response) {
        super("response", command, []);
        this.Response = response;
    }
}
exports.CommandPacketResponse = CommandPacketResponse;
//# sourceMappingURL=commandPacket.js.map