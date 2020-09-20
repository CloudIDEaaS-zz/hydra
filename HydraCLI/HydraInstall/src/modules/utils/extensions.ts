const StringBuilder = require('stringbuilder');

declare global {

  // export interface Object {
  //   oneOf(...values): boolean;
  // }

  // export interface Number {
  //   oneOf(...values): boolean;
  // }

  export interface String {
    getEndingCount(ending: string) : number;
  }
}

let getEndingCount = function(ending: string) {

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

let oneOf = function(...values) {

  let equals: boolean = false;

  values.forEach(v => {
    if (this === v) {
      equals = true;
    }
  });

  return equals;
};

// Object.prototype.oneOf = oneOf;
// Number.prototype.oneOf = oneOf;

import { Readable, Writable, Duplex } from "stream";

declare module "stream" {
  
  interface Readable {
    readJson<T>(listener: (T) => void);
    readText(listener: (response : string) => void);
  }

  interface Writable {
    writeJson(obj : any);
  }

  interface Duplex {
    readJson<T>(listener: (T) => void);
    readText(listener: (response : string) => void);
    writeJson(obj : any);
  }
}

let readText = function(listener: (response : string) => void) {
  return this.on("data", listener);
};

let readJson = function(listener: (T) => void) { 

  let dataListener : (data : string) => void = null;

  dataListener = (data : string) => {

    var obj = JSON.parse(data);

    listener(obj);
    this.removeListener("data", dataListener);
    
  };

  return this.on("data", dataListener);
};

let writeJson = function(obj : any) {

  let output: string = JSON.stringify(obj);
  this.write(output + "\r\n\r\n");
  
};

Readable.prototype.readJson = readJson;
Readable.prototype.readText = readText;
Writable.prototype.writeJson = writeJson;
Duplex.prototype.readJson = readJson;
Duplex.prototype.writeJson = writeJson;

import { Socket } from "net";

declare module "net" {

  interface Socket {
    writeLine(output : string);
  }
}

let writeLine = function(output : string) {
  this.write(output + "\r\n");
};

Socket.prototype.writeLine = writeLine;

export { };
