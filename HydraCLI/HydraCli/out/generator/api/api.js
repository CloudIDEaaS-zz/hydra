"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Api = void 0;
const http = require("http");
/**
 * Api is a generic REST Api handler. Set your API url first.
 */
class Api {
    baseUrl = "http://localhost:8043";
    servicesUrl = this.baseUrl + "/api/Status";
    get(endpoint, ...params) {
        return new Promise((resolve, reject) => {
            let queryString = "";
            if (params.length) {
                let argsArray = [];
                params.forEach(a => {
                    if (queryString.length) {
                        queryString += `&${a.key}=${a.value}`;
                    }
                    else {
                        queryString = `${a.key}=${a.value}`;
                    }
                });
            }
            http.get(this.servicesUrl + '/' + endpoint + (queryString ? "?" + queryString : ""), (res) => {
                const { statusCode } = res;
                const contentType = res.headers['content-type'];
                let error;
                if (statusCode !== 200) {
                    error = new Error('Request Failed.\n' + `Status Code: ${statusCode}`);
                }
                else if (!/^application\/json/.test(contentType)) {
                    error = new Error('Invalid content-type.\n' + `Expected application/json but received ${contentType}`);
                }
                if (error) {
                    reject(error.message);
                    // Consume response data to free up memory
                    res.resume();
                    return;
                }
                res.setEncoding('utf8');
                let rawData = '';
                res.on('data', (chunk) => { rawData += chunk; });
                res.on('end', () => {
                    try {
                        const parsedData = JSON.parse(rawData);
                        resolve(parsedData);
                    }
                    catch (e) {
                        reject(e.message);
                    }
                });
            }).on("error", (e) => {
                reject(e);
            });
        });
    }
}
exports.Api = Api;
//# sourceMappingURL=api.js.map