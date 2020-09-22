"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Utils = void 0;
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