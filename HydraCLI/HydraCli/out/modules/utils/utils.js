"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class Utils {
    static expandPath(path) {
        var replaced = path.replace(/%([^%]+)%/g, function (_, n) {
            return process.env[n];
        });
        return replaced;
    }
    static sleep(time) {
        return new Promise((resolve) => setTimeout(resolve, time));
    }
}
exports.Utils = Utils;
//# sourceMappingURL=utils.js.map