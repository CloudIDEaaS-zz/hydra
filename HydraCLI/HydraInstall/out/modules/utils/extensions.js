"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const StringBuilder = require('stringbuilder');
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
let readText = function (listener) {
    return this.on("data", listener);
};
let readJson = function (listener) {
    let dataListener = null;
    dataListener = (data) => {
        var obj = JSON.parse(data);
        listener(obj);
        this.removeListener("data", dataListener);
    };
    return this.on("data", dataListener);
};
let writeJson = function (obj) {
    let output = JSON.stringify(obj);
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