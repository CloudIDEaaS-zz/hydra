"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.BaseThreadedService = void 0;
const timespan = require('timespan');
const events = require('events');
class BaseThreadedService {
    constructor(iterationSleep) {
        this.Started = new events.EventEmitter();
        this.isRunning = false;
        this.processingHalted = false;
        this.iterationSleep = iterationSleep;
    }
    get IsRunning() {
        return this.isRunning;
    }
    Start() {
        this.firstRun = true;
        this.intervalId = setInterval(() => this.WorkerThreadProc(), this.iterationSleep.totalMilliseconds());
    }
    WorkerThreadProc() {
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
            }
            catch (ex) {
                this.working = false;
            }
        }
    }
    Stop() {
        clearInterval(this.intervalId);
        this.isRunning = false;
    }
}
exports.BaseThreadedService = BaseThreadedService;
//# sourceMappingURL=baseThreadedService.js.map