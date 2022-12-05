"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.InstallInfo = void 0;
class InstallInfo {
    Command;
    InstallAttempted;
    Succeeded;
    Failed;
    constructor(Command) {
        this.Command = Command;
        this.InstallAttempted = false;
        this.Succeeded = false;
        this.Failed = false;
    }
}
exports.InstallInfo = InstallInfo;
//# sourceMappingURL=installInfo.js.map