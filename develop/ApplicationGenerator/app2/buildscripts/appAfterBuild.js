#!/usr/bin/env node

var fs = require('fs');
var path = require('path');
var ts = require('typescript');
var tsconfig = { json: require('../tsconfig.json') };

console.log(`Current directory: ${process.cwd()}`);

function compile(fileNames, options) {
    let program = ts.createProgram(fileNames, options);
    let emitResult = program.emit();

    let allDiagnostics = ts
        .getPreEmitDiagnostics(program)
        .concat(emitResult.diagnostics);

    allDiagnostics.forEach(diagnostic => {
        if (diagnostic.file) {
            let { line, character } = diagnostic.file.getLineAndCharacterOfPosition(
            );
            let message = ts.flattenDiagnosticMessageText(
                diagnostic.messageText,
                "\n"
            );
            console.log(
                `${diagnostic.file.fileName} (${line + 1},${character + 1}): ${message}`
            );
        } else {
            console.log(
                `${ts.flattenDiagnosticMessageText(diagnostic.messageText, "\n")}`
            );
        }
    });

    let exitCode = emitResult.emitSkipped ? 1 : 0;
    console.log(`Process exiting with code '${exitCode}'.`);
    process.exit(exitCode);
}

compile([ process.cwd() + '/src/jasmine/main.ts' ],
{
    allowSyntheticDefaultImports: true,
    declaration: false,
    emitDecoratorMetadata: true,
    experimentalDecorators: true,
    lib: [
      "dom",
      "es2015"
    ],
    module: "es2015",
    moduleResolution: ts.ModuleResolutionKind.NodeJs,
    sourceMap: true,
    target: "es6"
  });

