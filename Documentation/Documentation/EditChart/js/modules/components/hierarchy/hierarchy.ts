import { Component, OnInit, Input, ElementRef } from '@angular/core';
import * as $ from 'jquery';
declare var require: any;
let orgchart = require("orgchart/src/js")

@Component({
  selector: 'hierarchy',
  templateUrl: './hierarchy.html',
  styleUrls: ['./hierarchy.scss'],
})
export class HierarchyComponent implements OnInit {

  @Input() nodeContentProperty: string;
  @Input() nodeChildrenProperty: string;
  @Input() nodes: any[];

  constructor(private elementRef: ElementRef) {

  }

  ngOnInit() {}

}
