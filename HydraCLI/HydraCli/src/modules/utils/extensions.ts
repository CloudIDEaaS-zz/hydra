const StringBuilder = require('stringbuilder');
const fs = require('fs');
const dateFormat = require('dateformat');
import * as path from "path";
const {serializeError, deserializeError} = require('serialize-error');
const beautify = require('js-beautify');

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

let toJson = function() {
  let error = this;
  let json = "\r\n" + beautify(JSON.stringify(serializeError(error)), { indent_size: 2, space_in_empty_paren: true }).replace(/\\n/g, "\r\n") + "\r\n";

  return json;
};

Error.prototype["toJson"] = toJson;
export interface Error {
  toJson() : string;
}

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

function writeLog(logPath: string, outgoing: boolean, contents: string) {

  try {
    let fileName = dateFormat(Date.now(), "yyyymmdd_HHMMss_l") + (outgoing ? "_HydraCLI_Out" : "_HydraCLI_In") + ".json";
    let fullFileName = path.join(logPath, fileName);
  
    if (!fs.existsSync(logPath)){
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

declare module "stream" {

  interface Readable {
    readJson<T>(logPath: string, listener: (T) => void);
    readText(logPath: string, listener: (response : string) => void);
  }

  interface Writable {
    writeJson(logPath: string, obj : any);
  }

  interface Duplex {
    readJson<T>(logPath: string, listener: (T) => void);
    readText(logPath: string, listener: (response : string) => void);
    writeJson(logPath: string, obj : any);
  }
}

let readText = function(logPath: string = null, listener: (response : string) => void) {
  return this.on("data", listener);
};

let readJson = function(logPath: string = null, listener: (T) => void) { 

  let dataListener : (data : string) => void = null;
  let errorListener : (e : Error) => void = null;
  let incompleteData: string;

  errorListener = (e : Error) => {
    listener(e);
    this.removeListener("error", errorListener);
  };

  dataListener = (data : string) => {

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

let writeJson = function(logPath: string = null, obj : any) {

  let output: string = JSON.stringify(obj);

  if (logPath) {
    writeLog(logPath, true, output);
  }
  
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
