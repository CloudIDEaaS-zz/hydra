export class CommandPacket {
  public Response: any;
  public SentTimestamp: Date;
  public IsChainedStream: boolean;

  constructor(
    public Type: string,
    public Command: string,
    public Arguments: any[]
  ) {
    this.SentTimestamp = new Date(Date.now());
  }
}

export class CommandPacketResponse extends CommandPacket {
  constructor(command: string, response: any) {
    super("response", command, []);
    this.Response = response;
  }
}
