import { Component, Input, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { Platform } from 'ionic-angular';
import { PlatformRef } from '@angular/core';
import { } from 'jasmine';
import { TestBed } from '@angular/core/testing';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';
import { TestsReporter } from './tests.reporter';

declare var require: any;

@Component({
  templateUrl: 'tests.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Tests {

  @Input() window: Window;
  public reporter: TestsReporter;

  configuration = {
    childNodesMember : "children",
    titleProperty: "title",
    imageProperty: "image",
    styleProperty: "style",
    isExpandedProperty: "isExpanded"
  }

  constructor(platform: Platform, platformRef: PlatformRef, changeDetectorRef: ChangeDetectorRef) {

    this.reporter = new TestsReporter(changeDetectorRef);

    platform.ready().then(() => {

      console.log('Platform ready');

      try {
        let env = jasmine.getEnv();
        let appContext = require.context('../../src', true, /\.spec\.ts/);

        env.addReporter(this.reporter);

        appContext.keys().forEach(appContext);

      } catch (error) {
        console.log(error);
      }
    });
  }

  public getResults() {
    return this.reporter.getResults();
  }
}

