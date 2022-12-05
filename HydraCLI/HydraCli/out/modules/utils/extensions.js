"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const StringBuilder = require('stringbuilder');
const fs = require('fs');
const dateFormat = require('dateformat');
const path = require("path");
const { serializeError, deserializeError } = require('serialize-error');
const beautify = require('js-beautify');
let getEndingCount = function (ending) {
    let thisString = this;
    let length = thisString.length;
    let end = thisString.substring(length - 2, length);
    let x = 0;
    while (end === ending) {
        thisString = thisString.substring(0, length - Math.min(length, 2));
        length = thisString.length;
        end = thisString.substring(length - 2, length);
        x++;
    }
    return x;
};
let toJson = function () {
    let error = this;
    let json = "\r\n" + beautify(JSON.stringify(serializeError(error)), { indent_size: 2, space_in_empty_paren: true }).replace(/\\n/g, "\r\n") + "\r\n";
    return json;
};
Error.prototype["toJson"] = toJson;
String.prototype.getEndingCount = getEndingCount;
let oneOf = function (...values) {
    let equals = false;
    values.forEach(v => {
        if (this === v) {
            equals = true;
        }
    });
    return equals;
};
// Object.prototype.oneOf = oneOf;
// Number.prototype.oneOf = oneOf;
const stream_1 = require("stream");
function writeLog(logPath, outgoing, contents) {
    try {
        let fileName = dateFormat(Date.now(), "yyyymmdd_HHMMss_l") + (outgoing ? "_HydraCLI_Out" : "_HydraCLI_In") + ".json";
        let fullFileName = path.join(logPath, fileName);
        if (!fs.existsSync(logPath)) {
            fs.mkdirSync(logPath, { recursive: true });
        }
        fs.writeFileSync(fullFileName, contents, (err) => {
            if (err) {
                return console.log(err);
            }
        });
    }
    catch (err) {
        console.log(err);
    }
}
let readText = function (logPath = null, listener) {
    return this.on("data", listener);
};
let readJson = function (logPath = null, listener) {
    let dataListener = null;
    let errorListener = null;
    let incompleteData;
    errorListener = (e) => {
        listener(e);
        this.removeListener("error", errorListener);
    };
    dataListener = (data) => {
        let obj;
        if (incompleteData) {
            data = incompleteData + data;
        }
        if (logPath) {
            writeLog(logPath, false, data);
        }
        try {
            obj = JSON.parse(data);
            listener(obj);
            this.removeListener("data", dataListener);
            this.removeListener("error", errorListener);
        }
        catch (err) {
            incompleteData = data;
        }
    };
    this.on("error", errorListener);
    return this.on("data", dataListener);
};
let writeJson = function (logPath = null, obj) {
    let output = JSON.stringify(obj);
    if (logPath) {
        writeLog(logPath, true, output);
    }
    this.write(output + "\r\n\r\n");
};
stream_1.Readable.prototype.readJson = readJson;
stream_1.Readable.prototype.readText = readText;
stream_1.Writable.prototype.writeJson = writeJson;
stream_1.Duplex.prototype.readJson = readJson;
stream_1.Duplex.prototype.writeJson = writeJson;
const net_1 = require("net");
let writeLine = function (output) {
    this.write(output + "\r\n");
};
net_1.Socket.prototype.writeLine = writeLine;
//# sourceMappingURL=extensions.js.map