import { BrowserModule } from '@angular/platform-browser';
import { ErrorHandler, NgModule, Injector } from '@angular/core';
import { Tests } from './tests.component';
import { CoreModule } from '../app/core/core.module';
import { IonicApp, IonicErrorHandler, IonicModule } from 'ionic-angular';
import { ɵTestingCompilerFactory as TestingCompilerFactory } from '@angular/core/testing';
import { ɵa, ɵb } from '@angular/platform-browser-dynamic/testing';
import { JitCompilerFactory } from '@angular/platform-browser-dynamic';
import { CompilerFactory, COMPILER_OPTIONS } from '@angular/core';
import { WINDOW_PROVIDERS } from '../app/core/services/window.service';
import { TreeViewModule } from '../modules/components/tree-view';

@NgModule({
  declarations: [
    Tests
  ],
  imports: [
    BrowserModule,
    IonicModule.forRoot(Tests),
    CoreModule.forRoot(),
    TreeViewModule
  ],
  bootstrap: [IonicApp],
  entryComponents: [
    Tests
  ],
  providers: [
    WINDOW_PROVIDERS,
    { provide: COMPILER_OPTIONS, useValue: ɵa },
    { provide: ErrorHandler, useClass: IonicErrorHandler },
    { provide: TestingCompilerFactory, useClass: ɵb, deps: [ CompilerFactory, Injector ] }
  ]
})
export class TestModule { }
