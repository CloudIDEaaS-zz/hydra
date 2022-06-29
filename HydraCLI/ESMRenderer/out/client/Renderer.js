"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.RendererClient = void 0;
class RendererClient {
    constructor() {
        this.stdout = process.stdout;
        this.stderr = process.stderr;
    }
    static start(tsxFile) {
        let client;
        if (RendererClient.client === undefined) {
            RendererClient.client = new RendererClient();
        }
        client = RendererClient.client;
        return client.getSource(tsxFile);
    }
    getSource(tsxFile) {
        return "Method not implemented.";
    }
}
exports.RendererClient = RendererClient;
//# sourceMappingURL=Renderer.js.map