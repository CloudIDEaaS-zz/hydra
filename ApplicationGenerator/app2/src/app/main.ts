import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app.module';
import { TestModule } from '../jasmine/tests.module'
import { TestBed } from '@angular/core/testing';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';
import { environment } from './environments/environment';

if (window["loadTestModule"])
{
    let platform = platformBrowserDynamicTesting();
    
    TestBed.initTestEnvironment(BrowserDynamicTestingModule, platform);

    platform.bootstrapModule(TestModule);
}
else
{
    if (environment.production) {
        enableProdMode();
    }

    window["describe"] = function() {};
    platformBrowserDynamic().bootstrapModule(AppModule)
        .catch(err => console.log(err));
}
