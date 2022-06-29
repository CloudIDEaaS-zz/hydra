import { Utils } from "../modules/utils/utils";

const timespan = require('timespan');
const events = require('events');

export abstract class BaseThreadedService {
    private iterationSleep: any;
    private firstRun : boolean;
    public Started = new events.EventEmitter();
    working: boolean;
    public abstract DoWork(stopping: boolean) : Promise<void>;
    public abstract WriteLine(output : string);
    public abstract Write(output : string);
    public abstract WriteError(output : string);
    public abstract WriteWarning(output : string);
    public abstract WriteSuccess(output : string);   
    private isRunning: boolean = false;
    protected processingHalted: boolean = false;
    public StartTime: Date;
    private intervalId: NodeJS.Timeout;

    public get IsRunning() : boolean {
        return this.isRunning;
    }
    
    constructor(iterationSleep: any) {
        this.iterationSleep = iterationSleep;
    }

    public Start() : void {
        
        this.firstRun = true;
        this.intervalId = setInterval(() => this.WorkerThreadProc(), this.iterationSleep.totalMilliseconds());
    }

    public WorkerThreadProc() : void {
        let running = true;
        this.isRunning = running;

        if (this.firstRun) {
            this.Started.emit(this);
        }

        let stopping = false;

        if (!this.working) {

            this.working = true;
    
            try {
                
                let promise = this.DoWork(stopping);
    
                promise.finally(() => {
                    this.working = false;
                });
    
            } catch(ex) {
                this.working = false;
            }
        }
    }

    public Stop() : void {
        clearInterval(this.intervalId);
        this.isRunning = false;
    }
}
