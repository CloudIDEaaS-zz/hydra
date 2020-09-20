import { AsyncPipe, DatePipe } from '@angular/common';
import { PipeTransform } from '@angular/core';
import { PropertyRead, ASTWithSource, CompileTypeMetadata, CompileSummaryKind, CompileReflector, Parser, Lexer, DomElementSchemaRegistry, CompilePipeSummary, ParseError, ParseSourceSpan, ParseLocation, ParseSourceFile, TemplateParser, BindingPipe, LiteralPrimitive } from '@angular/compiler';
import { PipeCollector, BindingParser, BoundProperty } from '../../utils/BindingParser';
import { DebugUtils } from '../../utils/DebugUtils';
import { BindingsParseResult, BindingTransform, TreeView, ExpandPipe } from './';
import { List } from 'linq-collections';

export class BindingsParser {

  private registry: DomElementSchemaRegistry;
  private lexer: Lexer;
  private parser : Parser;
  private pipes: CompilePipeSummary[]

  constructor(private treeView: TreeView) {

    this.registry = new DomElementSchemaRegistry();
    this.lexer = new Lexer();
    this.parser = new Parser(this.lexer);
    this.pipes = [
      {
        name: "async",
        pure: false,
        summaryKind: CompileSummaryKind.Pipe,
        type: { reference: AsyncPipe, diDeps: [], lifecycleHooks: [] },
      },
      {
        name: "onexpand",
        pure: false,
        summaryKind: CompileSummaryKind.Pipe,
        type: { reference: ExpandPipe, diDeps: [], lifecycleHooks: [] },
      },
      {
        name: "date",
        pure: false,
        summaryKind: CompileSummaryKind.Pipe,
        type: { reference: DatePipe, diDeps: [], lifecycleHooks: [] },
      },
     ];
  }

  parse(member: string, value: string) : BindingsParseResult {

    let errors: ParseError[] = [];
    let bindingParser = new BindingParser(this.parser, undefined, this.registry, this.pipes, errors);
    let source = member + "=" + value;
    let sourceFile = new ParseSourceFile(source, "parse://BindingsParser");
    let targetProps : any[] = [];
    let targetMatchableAttributes : any[] = [];
    let span = new ParseSourceSpan(
      new ParseLocation(sourceFile, 0, 0, 0),
      new ParseLocation(sourceFile, source.length, 0, source.length));
    let propertyName: string;
    let bindingsParseResult = new BindingsParseResult();

    bindingParser.parsePropertyBinding(member, value, false, span, targetMatchableAttributes, targetProps);

    targetProps.forEach(p => {

      if (p instanceof BoundProperty) {

        let boundProperty = <BoundProperty> p;
        let pipeCollector = new PipeCollector();
        let ast = <ASTWithSource>boundProperty.expression;
        let list = new List<BindingPipe>();

        let captureProperty = (p : PropertyRead) => {
          bindingsParseResult.propertyName = p.name;
        };

        ast.visit(pipeCollector);

        if (pipeCollector.pipes.size == 0) {

          let property = <PropertyRead>p.expression.ast;

          captureProperty(property);
        }
        else {

            pipeCollector.pipes.forEach((p : BindingPipe) => {

              let capturePipe = (p : BindingPipe) => {

                if (!list.contains(p)) {

                  let pipeName = p.name;
                  let pipeSummary = bindingParser.pipesByName.get(pipeName);
                  let pipeType = pipeSummary.type.reference;
                  let bindingTransform = new BindingTransform();
                  let pipeTransform : PipeTransform;

                  if (pipeType.name == "AsyncPipe" || pipeType.name == "ExpandPipe") {
                    pipeTransform = <PipeTransform> new pipeType({ markForCheck: () => bindingTransform.markForCheck() });
                  }
                  else {
                    pipeTransform = <PipeTransform> new pipeType();
                  }

                  bindingTransform.pipeName = pipeName;
                  bindingTransform.pipeTransform = pipeTransform;
                  bindingsParseResult.bindingTransforms.push(bindingTransform);

                  p.args.forEach(a => {

                    if (a instanceof LiteralPrimitive) {

                      var primitive = <LiteralPrimitive> a;

                      bindingTransform.args.push(primitive.value);
                    }
                    else {
                      DebugUtils.break();
                    }
                  })

                  if (p.exp instanceof PropertyRead) {

                    let property = <PropertyRead>p.exp;

                    captureProperty(property);
                  }
                  else if (p.exp instanceof BindingPipe) {

                    let pipe = <BindingPipe>p.exp;

                    capturePipe(pipe);
                  }
                  else {
                    DebugUtils.break();
                  }

                  list.push(p);
                }
              };

              capturePipe(p);
            });
          }
          }
      else {
        DebugUtils.break();
      }
    });

    return bindingsParseResult;
  }
}
