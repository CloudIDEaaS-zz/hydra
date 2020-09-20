import { Component, OnInit, Input, DoCheck, Inject, ElementRef, ViewChildren, NgZone, QueryList, Output, EventEmitter, ViewChild, ChangeDetectorRef } from '@angular/core';
import { NavController, NavParams, Platform, ToastController } from 'ionic-angular';
import { TreeNode } from './tree-node';
import { WINDOW } from "../../../app/core/services/window.service";
import * as $ from 'jquery';
import { DomRendererFactory2 } from '@angular/platform-browser/src/dom/dom_renderer';
import { NULL_EXPR } from '@angular/compiler/src/output/output_ast';
import { List, IEnumerable } from "linq-javascript";
import { DebugUtils } from '../../utils/DebugUtils';
import { DomSanitizer, SafeResourceUrl, SafeUrl } from '@angular/platform-browser';
import { BindingsParser, BindingsParseResult, BindingTransform } from './';
import { ForNodesDirective } from './forNodesDirective';
import { EditChangeArgs } from '../edit-label';
import { PromiseObservable } from 'rxjs/observable/PromiseObservable';

const uuidv1 : Function = require('uuid/v1');
const jsonpath : Function = require("JSONPath");

@Component({
  selector: 'tree-view',
  templateUrl: 'tree-view.html'
})
export class TreeView {
  @Input('configuration') configuration : any;
  @Input('nodes') nodes : any[];
  @Input('thisNode') thisNode: any;
  @ViewChildren('nodeElements') nodeElements;
  @ViewChild('childTree') childTree : TreeView;
  private _selectedNodeElement : HTMLLabelElement;
  private _selectedNode : any;
  private _rootTreeView : TreeView;
  document: Document;
  isRoot: boolean;
  isReady: boolean;
  bindingsParser : BindingsParser;
  refreshQueue : List<TreeNode>;
  @Output()
  onexpand: EventEmitter<TreeNode> = new EventEmitter<TreeNode>();

  constructor(private platform: Platform,
    @Inject(WINDOW) private window: Window,
    private elementRef: ElementRef,
    private sanitizer: DomSanitizer,
    public toastCtrl: ToastController,
    private zone: NgZone,
    private changeDetectorRef: ChangeDetectorRef) {

    this.bindingsParser = new BindingsParser(this);
    this.document = this.window.document;
    this.refreshQueue = new List<TreeNode>();

    console.log('constructor');

    elementRef.nativeElement.id = this.getId();

    if (this.thisNode) {

      if (this.rootTreeView == this) {
        DebugUtils.break();
      }
    }
    else {
      this.isRoot = true;
      this.window.addEventListener("keydown", (e) => this.onKeydown(e));

      if (this.rootTreeView != undefined) {
        DebugUtils.break();
      }

      this.rootTreeView = this;
    }

    platform.ready().then(() => {

      this.setElements();
      this.isReady = true;
    });
  }

  onLabelChange(n, args : EditChangeArgs) {

    let treeNode = <TreeNode> n.__treeNode__;
    let promise = treeNode.raiseChangeTitle(args.value);

    if (promise) {

      promise.catch(() => {
        treeNode.title = args.oldValue;
        treeNode.element.innerText = treeNode.title;
      });
    }
  }

  onEndEdit(n) {

    let treeNode = <TreeNode> n.__treeNode__;
    let element = treeNode.element;

    this.selectNode(<Event><any> { target: <Element> element }, n);
  }

  onCreate(node: TreeNode) {

	  this.nodes.forEach(n => {

      let treeNode = <TreeNode> n.__treeNode__;
      treeNode.parentTreeView = this;

    });
  }

  @Input('treeView')
  set rootTreeView(treeView : TreeView) {
    this._rootTreeView = treeView;
  }

  get rootTreeView()
  {
    if (this.isReady && this._rootTreeView && !this._rootTreeView.isRoot) {
      DebugUtils.break();
    }

    return this._rootTreeView;
  }

  getId() : string {
    return uuidv1().toString()
  }

  get selectedNode() {
    return this.rootTreeView._selectedNode;
  }

  set selectedNode(value) {
    this.rootTreeView._selectedNode = value;
  }

  get selectedNodeElement() {
    return this.rootTreeView._selectedNodeElement;
  }

  set selectedNodeElement(value) {
    this.rootTreeView._selectedNodeElement = value;
  }

  getPadding() : string {
    return this.rootTreeView === this ? "5px" : "0px";
  }

  setElements(): any {

    let childIndex: number = 0;

    if (this.nodes) {

  	  this.nodes.forEach(n => {

        let treeNode = <TreeNode> n.__treeNode__;
        let nativeElement = this.nodeElements._results[childIndex].nativeElement;
        let element = <HTMLLabelElement> $(nativeElement).find("label")[0];

        treeNode.element = element;
        childIndex++;

      });
    }
  }

  ngDoCheck() {

    let previousNode : any;

    if (this.nodes) {

      let expressions : any[]

      if (this.configuration.expressions) {
        expressions = this.configuration.expressions;
      }

  	  this.nodes.forEach(n => {

        let treeNode = new TreeNode(this.rootTreeView);

        if (previousNode) {
          treeNode.previousNode = previousNode;
          previousNode.__treeNode__.nextNode = n;
        }

        previousNode = n;

        if (!n.__treeNode__) {

          let nodeConfiguration = Object.assign({}, this.configuration);

          n.__treeNode__ = treeNode;

          treeNode.object = n;
          treeNode.parentNode = this.thisNode;

          if (expressions && expressions.length) {

            let count = 0;
            let childJson = {
              nodes: n
            };
            let parentJson = {
              nodes: this.thisNode
            };

            expressions.forEach(e => {

              let parentExpression = e.parentExpression;
              let childExpression = e.childExpression;

              if (parentExpression) {
                let result : any[] = jsonpath({json: parentJson, path: parentExpression });

                if (result.length) {

                  count++;

                  if (count > 1) {
                    DebugUtils.break();
                  }

                  nodeConfiguration = Object.assign(nodeConfiguration, e.configuration);
                }
              }
              else if (childExpression) {

                let result : any[] = jsonpath({json: childJson, path: childExpression });

                if (result.length) {

                  count++;

                  if (count > 1) {
                    DebugUtils.break();
                  }

                  nodeConfiguration = Object.assign(nodeConfiguration, e.configuration);
                }
              }

            });
          }

          if (!nodeConfiguration) {
            throw "configuration not set for TreeView";
          }

          if (nodeConfiguration.titleProperty) {

            if (n[nodeConfiguration.titleProperty]) {
              treeNode.title = n[nodeConfiguration.titleProperty];
            }
          }
          else {
            throw "titleProperty not set in configuration for TreeView";
          }

          if (nodeConfiguration.childNodesMember) {

            let result = this.bindingsParser.parse("childNodesMember", nodeConfiguration.childNodesMember);

            if (result.bindingTransforms.length > 0) {

              if (n[result.propertyName]) {

                let list = new List<BindingTransform>(result.bindingTransforms);
                let latestValue = n[result.propertyName];

                for (let bindingTransform of list.toArray()) {

                  if (bindingTransform.pipeName == "onexpand") {
                    latestValue = bindingTransform.pipeTransform.transform(latestValue, [n, ...bindingTransform.args]);
                  }
                  else {
                    latestValue = bindingTransform.pipeTransform.transform(latestValue, bindingTransform.args);
                  }

                  list.remove(bindingTransform);

                  if (bindingTransform.pipeName == "async") {

                    let childNodes = [];

                    bindingTransform.subscribe({
                      next : (n) => {
                        childNodes.push(n);
                      },
                      error : (e) => {

                        let toast = this.toastCtrl.create({
                          message: e.message,
                          duration: 3000,
                          position: "top"
                        });
                        toast.present();

                      },
                      complete : () => {

                        latestValue = childNodes;

                        for (let remainingTransform of list.toArray()) {
                          latestValue = bindingTransform.pipeTransform.transform(childNodes, bindingTransform.args);
                        }

                        treeNode.childNodes = latestValue;
                      }
                    });

                    break;
                  }
                  else if (bindingTransform.pipeName == "onexpand") {

                    let childNodes = [];

                    bindingTransform.subscribe({
                      next : (n) => {
                        childNodes.push(n);
                      },
                      error : (e) => {

                        let toast = this.toastCtrl.create({
                          message: e.message,
                          duration: 3000,
                          position: "top"
                        });
                        toast.present();

                      },
                      complete : () => {

                        latestValue = childNodes;

                        for (let remainingTransform of list.toArray()) {
                          latestValue = bindingTransform.pipeTransform.transform(childNodes, bindingTransform.args);
                        }

                        treeNode.childNodes = latestValue;
                        this.rootTreeView.refreshQueue.push(n);
                      }
                    });

                    break;
                  }
                }
              }
            }
            else {
              if (n[result.propertyName]) {
                treeNode.childNodes = n[result.propertyName];
              }
            }
          }
          else
          {
            throw "childNodesMember not set in configuration for TreeView";
          }

          if (nodeConfiguration.iconProperty) {

            if (n[nodeConfiguration.iconProperty]) {
              treeNode.icon = n[nodeConfiguration.iconProperty];
            }
          }

          if (nodeConfiguration.imageProperty) {

            if (n[nodeConfiguration.imageProperty]) {
              treeNode.image = n[nodeConfiguration.imageProperty];
            }
          }

          if (nodeConfiguration.styleProperty) {

            if (n[nodeConfiguration.styleProperty]) {
              treeNode.style = this.sanitizer.bypassSecurityTrustStyle(n[nodeConfiguration.styleProperty]);
            }
          }

          if (nodeConfiguration.isExpandedProperty) {

            if (n[nodeConfiguration.isExpandedProperty]) {
              treeNode.isExpanded = n[nodeConfiguration.isExpandedProperty];
            }
          }

          if (nodeConfiguration.onTitleChangeMember) {

            if (n[nodeConfiguration.onTitleChangeMember]) {
              treeNode.onTitleChange = n[nodeConfiguration.onTitleChangeMember];
            }
          }
        }
      });
    }

    if (this.rootTreeView.refreshQueue.any(n => n == this.thisNode) ) {

      if (this.hasSpinsterNode) {

        this.changeDetectorRef.markForCheck();

        setTimeout(() => {
          this.changeDetectorRef.detectChanges();
        }, 100);
      }
      else {
        this.rootTreeView.refreshQueue.remove(this.thisNode);
        this.setElements();
      }
    }
  }

  get hasSpinsterNode() {

    if (this.nodeElements._results.length == 1) {

      let nativeElement = this.nodeElements._results[0].nativeElement
      let element = <HTMLLabelElement> $(nativeElement).find("label")[0];

      if (element.innerText == "Loading...") {
        return true;
      }
    }

    return false;
  }

  expandCollapseNode(event, node) {

    let treeNode = <TreeNode> node.__treeNode__;
    treeNode.isExpanded = !node.__treeNode__.isExpanded;

    if (!treeNode.isExpanded) {

      // if a node is selected within the expanded set, select the node being collapsed

      let eventTarget = $(event.target);
      let target : HTMLLabelElement;

      if (eventTarget.prop("tagName").toLowerCase() != "label") {

        let button = eventTarget.closest("button");                      // expand/collapse button

        if (!button[0] || button.prop("tagName").toLowerCase() != "button") {
          DebugUtils.break();
        }

        let parentRow = button.parent().parent();                        // parent row

        if (!parentRow[0] || parentRow.prop("tagName").toLowerCase() != "ion-row") {
          DebugUtils.break();
        }

        let lastCol = parentRow.children().last()                        // col containing label

        if (!lastCol[0] || lastCol.prop("tagName").toLowerCase() != "ion-col") {
          DebugUtils.break();
        }

        target = <HTMLLabelElement> lastCol.children().children()[0];    // label itself

        if (!target || target.tagName.toLowerCase() != "label") {
          DebugUtils.break();
        }

        treeNode.forEachDescendant(n => {
          if (n.__treeNode__.isSelected) {
            this.selectNode(<Event><any> { target: <Element> target }, n);
          }
        });
      }
    }

    this.rootTreeView.raiseOnExpand(treeNode);
  }

  raiseOnExpand(treeNode: TreeNode) {
    this.onexpand.emit(treeNode);
  }

  expandNode(event, node) {

    if (!node.__treeNode__.isExpanded) {
      this.expandCollapseNode(event, node);
    }
  }

  collapseNode(event, node) {

    if (node.__treeNode__.isExpanded) {
      this.expandCollapseNode(event, node);
    }
  }
  onKeydown(event : KeyboardEvent) {

    let activeElement = window.document.activeElement;

    if (activeElement) {

      let parentElementsAndSelf = new List([activeElement, ...$(activeElement).parents().toArray()]);

      if (parentElementsAndSelf.contains(this.elementRef.nativeElement)) {

        switch (event.key) {

          case "ArrowUp": {

            if (this.selectedNode == null && this.nodes != null && this.nodes.length > 0) {

              let nextNode = this.nodes[0];
              let nextElement = nextNode.__treeNode__.element;

              this.selectNode(<Event><any> { target: <Element> nextElement }, nextNode);
            }
            else if (this.selectedNode.__treeNode__.previousVisibleNode) {

              let previousNode = this.selectedNode.__treeNode__.previousVisibleNode;
              let previousTreeNode = <TreeNode> previousNode.__treeNode__;
              let previousElement = previousTreeNode.element;

              this.selectNode(<Event><any> { target: <Element> previousElement }, previousNode);
            }

            break;
          }
          case "ArrowDown": {

            if (this.selectedNode == null && this.nodes != null && this.nodes.length > 0) {

              let nextNode = this.nodes[0];
              let nextElement = nextNode.__treeNode__.element;

              this.selectNode(<Event><any> { target: <Element> nextElement }, nextNode);
            }
            else if (this.selectedNode.__treeNode__.nextVisibleNode) {

              let nextNode = this.selectedNode.__treeNode__.nextVisibleNode;
              let nextTreeNode = <TreeNode> nextNode.__treeNode__;
              let nextElement = nextTreeNode.element;

              this.selectNode(<Event><any> { target: <Element> nextElement }, nextNode);
            }

            break;
          }
          case "ArrowRight": {

            if (this.selectedNode != null) {

              if (!this.selectedNode.__treeNode__.isExpanded) {
                this.expandNode(<Event><any> { target: <Element> this.selectedNode.__treeNode__.element }, this.selectedNode);
              }
              else if (this.selectedNode.__treeNode__.firstChildNode) {

                let firstChild = this.selectedNode.__treeNode__.firstChildNode;
                let childTreeNode = <TreeNode> firstChild.__treeNode__;
                let childElement = childTreeNode.element;

                this.selectNode(<Event><any> { target: <Element> childElement }, firstChild);
              }
            }

            break;
          }
          case "ArrowLeft": {

            if (this.selectedNode != null) {

              if (this.selectedNode.__treeNode__.isExpanded) {
                this.collapseNode(<Event><any> { target: <Element> this.selectedNode.__treeNode__.element }, this.selectedNode);
              }
              else if (this.selectedNode.__treeNode__.parentNode) {

                let parentNode = this.selectedNode.__treeNode__.parentNode;
                let parentTreeNode = <TreeNode> parentNode.__treeNode__;
                let parentElement = parentTreeNode.element;

                this.selectNode(<Event><any> { target: <Element> parentElement }, parentNode);
              }
            }

            break;
          }
        }
      }
    }
  }

  selectNode(event : Event, node) {

    node.__treeNode__.isSelected = true;

    if (this.selectedNodeElement)
    {
      this.selectedNodeElement.classList.remove("selected")
      this.selectedNodeElement.classList.add("unselected")
    }

    this.selectedNodeElement = <HTMLLabelElement> event.target;
    this.selectedNodeElement.classList.add("selected");
    this.selectedNodeElement.classList.remove("unselected")
    this.selectedNodeElement.focus();

    this.selectedNode = node;
  }

  isExpanded(node) : boolean {
    return node.__treeNode__.isExpanded;
  }
}
