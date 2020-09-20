import { PipeTransform } from '@angular/core';
import { DebugUtils } from '../../utils/DebugUtils';
import { Observable } from 'rxjs';
import { PartialObserver, Observer } from 'rxjs/Observer';
import { AnonymousSubscription, Subscription } from 'rxjs/Subscription';
import { Subscribable } from 'rxjs/Observable';
import { BindingTransform } from './';

export class BindingsParseResult {

  bindingTransforms : BindingTransform[];
  propertyName: string;

  constructor() {
    this.bindingTransforms = [];
  }
}
