import "../modules/utils/Extensions";
import { DebugUtils } from "../modules/utils/DebugUtils";

export enum NodeType {
  JasmineRoot,
  Suite,
  Spec,
  Error,
  Success,
  Stack
}

export class TestsNode {

  public children: TestsNode[];
  public errors: number;
  public successes: number;

  constructor(private nodeType : NodeType, private _title : string) {

    this.errors = 0;
    this.successes = 0;

    this.children = [];
  }

  public add(childNode: TestsNode): any {
    this.children.push(childNode);
  }

  public get title() : string {

    let title: string;

    switch (this.nodeType) {
      case NodeType.JasmineRoot:
      case NodeType.Suite:
      case NodeType.Spec:
        title = `${this._title}. successes: ${this.successes}, failures: ${this.errors}`;
        break;
      case NodeType.Error:
      case NodeType.Success:
      case NodeType.Stack:
        title = this._title;
        break;
    }

    return title;
  }

  public get image() : string {

    switch (this.nodeType) {
      case NodeType.JasmineRoot:
        return "assets/imgs/Jasmine.png";
      case NodeType.Suite:
        return "assets/imgs/Suite.png";
      case NodeType.Spec:
        return "assets/imgs/Test.png";
      case NodeType.Error:
        return "assets/imgs/Error.png";
      case NodeType.Success:
        return "assets/imgs/Success.png";
      case NodeType.Stack:
        return "assets/imgs/Stack.png";
      default:
        DebugUtils.break();
        return null;
    }
  }

  public get isExpanded() : boolean {

    let expanded: boolean;

    switch (this.nodeType) {
      case NodeType.JasmineRoot:
      case NodeType.Suite:
      case NodeType.Spec:
        return true;
      case NodeType.Error:
      case NodeType.Success:
      case NodeType.Stack:
        return false;
      }
  }

  public get style() : string {

    let style: string;

    if (this.errors > 0 || this.nodeType == NodeType.Error) {
      style = "color: Tomato";
    }
    else if (this.successes > 0) {
      style = "color: Green";
    }
    else {
      style = "color: Navy";
    }

    return style;
  }
}
