export class CommandPacket {

    public Response : any;
    public InstallAttempted : boolean;
    
    constructor(public Type : string, public Command : string, public Arguments : any[], SentTimestamp : Date) {
    }
}
