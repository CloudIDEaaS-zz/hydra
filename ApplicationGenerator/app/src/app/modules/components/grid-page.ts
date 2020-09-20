import { Component, NgZone, OnInit, OnDestroy } from '@angular/core';
const queryString = require('query-string');

export abstract class GridPage {

  bindClickMethod(pageName: string, methodName: string, method: Function) {
    window.my = window.my || {};
    window.my.GridPageBase = window.my.GridPageBase || {};
    window.my.GridPageBase[pageName] = window.my.GridPageBase[pageName] || {};
    window.my.GridPageBase[pageName][methodName] = method.bind(this);
  }

  static renderLinkElement(pageName: string, methodName: string, linkText: string, params: any) {
    return '<a target="_blank" style="cursor: pointer;" onclick="my.GridPageBase.' + pageName + "." + methodName + '(\'' + queryString.stringify(params.value) + '\')">' + linkText + '</a>';
  }

  parseLinkParams(params: string) : any {
    return queryString.parse(params);
  }
}
