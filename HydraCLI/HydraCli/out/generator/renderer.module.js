"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.RendererModule = void 0;
const fs = require('fs');
class RendererModule {
    loadRendererJsFile(jsFile) {
        try {
            let promise = Promise.resolve().then(() => require(jsFile));
            promise.then(c => {
                console.log();
            }, (err) => {
                throw err;
            });
            console.log();
        }
        catch (error) {
            throw error;
        }
    }
}
exports.RendererModule = RendererModule;
//# sourceMappingURL=renderer.module.js.map