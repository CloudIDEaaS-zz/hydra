import { PipeTransform } from '@angular/core';
import { DebugUtils } from '../../utils/DebugUtils';
import { Observable } from 'rxjs';
import { PartialObserver, Observer } from 'rxjs/Observer';
import { AnonymousSubscription, Subscription } from 'rxjs/Subscription';
import { Subscribable } from 'rxjs/Observable';

export class BindingTransform implements Subscribable<any> {

  pipeName: string;
  pipeTransform: PipeTransform;
  args: any[];
  observers : Observer<any>[];
  private observable: Observable<any>;

  constructor() {
    this.observers = [];
    this.args = [];
  }

  subscribe(observerOrNext?: PartialObserver<any> | ((value: any) => void), error?: (error: any) => void, complete?: () => void): AnonymousSubscription {

    let subscription = new Subscription(() => this.unsubscribe());

    this.observers.push(<Observer<any>> observerOrNext);

    return subscription;
  }

  markForCheck() {

    if (!this.observable) {

      this.observable = <Observable<any>> this.pipeTransform["_obj"];

      this.observers.forEach(o => {
        o.next(this.pipeTransform["_latestValue"]);
      });

      this.observable.subscribe({
        next : (v) => {
          this.observers.forEach(o => {
            o.next(v);
          });
        },
        error : (e) => {
          this.observers.forEach(o => {
            o.error(e);
          });
        },
        complete : () => {
          this.observers.forEach(o => {
            o.complete();
          });
        }
      });
    }
  }

  unsubscribe() {
  }
}
