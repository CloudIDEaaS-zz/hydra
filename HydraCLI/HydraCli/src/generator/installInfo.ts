export class InstallInfo 
{
    public InstallAttempted: boolean;
    public Succeeded: boolean;
    public Failed: boolean;

    constructor(public Command: string) {
    }
}
