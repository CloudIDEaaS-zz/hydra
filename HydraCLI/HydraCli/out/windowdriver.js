"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Driver = void 0;
const assert = require('assert');
const path = require('path');
const Application = require('spectron').Application;
const electronPath = require('electron');
const driverApp = new Application({
    path: electronPath,
    args: [path.join(__dirname, '..')]
});
class Driver {
    static async start() {
        driverApp.start();
        const count = await driverApp.client.getWindowCount();
    }
}
exports.Driver = Driver;
//# sourceMappingURL=windowdriver.js.map