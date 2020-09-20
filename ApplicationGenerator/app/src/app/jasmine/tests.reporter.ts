import { Observable } from "rxjs";
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { ChangeDetectorRef } from '@angular/core';
import { TestsNode } from "./testsNode";
import { NodeType } from "./testsNode";

export class TestsReporter {

  private results: Array<TestsNode>;

  constructor(private changeDetectorRef: ChangeDetectorRef) {
      this.results = new Array<TestsNode>();
  }

  getResults(): TestsNode[] {

    if (this.count > 0) {
      return [ this.peek() ];
    }
    else {
      return null;
    }
  }

  private push(nodeType : NodeType, title : string) {

    let childNode = this.add(nodeType, title.trim());

    this.results.push(childNode);
  }

  private add(nodeType: NodeType, title: string) : TestsNode {

    let parentNode = this.peek();
    let childNode = new TestsNode(nodeType, title);

    if (parentNode) {
      parentNode.add(childNode);
    }

    return childNode;
  }

  private pop() : TestsNode {
    return this.results.pop();
  }

  private peek() : TestsNode {
    return this.results.length > 0 ? this.results[this.results.length - 1] : null;
  }

  private get count() : number {
    return this.results.length;
  }

  jasmineStarted(suiteInfo) {
      this.push(NodeType.JasmineRoot, "Jasmine");
  }

  suiteStarted(result) {
      this.push(NodeType.Suite, `Suite: ${result.description}`);
  }

  specStarted(result) {
    this.push(NodeType.Spec, `Spec: ${result.description}`);
  }

  specDone(result) {

      let node : TestsNode;

      for (let i = 0; i < result.failedExpectations.length; i++) {

        let expectation = result.failedExpectations[i];

        this.push(NodeType.Error, `Failure of "${expectation.matcherName}": ${expectation.message}`);
        this.add(NodeType.Stack, expectation.stack);
        this.pop();
      }

      for (let i = 0; i < result.passedExpectations.length; i++) {

        let expectation = result.passedExpectations[i];

        this.push(NodeType.Success, `Success of "${expectation.matcherName}": ${expectation.message}`);

        if (expectation.stack) {
          this.add(NodeType.Stack, expectation.stack);
        }

        this.pop();
      }

      node = this.pop();

      node.successes = result.passedExpectations.length;
      node.errors = result.failedExpectations.length;
  }

  suiteDone(result) {

    let node : TestsNode = this.pop();
    let errors: number = 0;
    let successes: number = 0;

    node.children.forEach(c => {

      successes += c.successes;
      errors += c.errors;

    });

    node.successes = successes;
    node.errors = errors;
  }

  jasmineDone() {

    let node : TestsNode = this.peek();
    let errors: number = 0;
    let successes: number = 0;

    node.children.forEach(c => {

      successes += c.successes;
      errors += c.errors;

    });

    node.successes = successes;
    node.errors = errors;

    this.changeDetectorRef.detectChanges();
  }
}
