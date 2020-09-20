import { Observable, Observer, Subscription, Subscriber } from "rxjs";
import { Subscribable } from "rxjs/Observable";
import { PartialObserver } from "rxjs/Observer";
import { AnonymousSubscription } from "rxjs/Subscription";
import { List, IEnumerable } from "linq-javascript";

export class TestObservable<T> implements Subscribable<T> {

  observers: Observer<T>[];
  values: List<T>;
  timer: NodeJS.Timer;

  constructor(values: T[] = null) {
    this.observers = [];
    this.values = new List<T>(values);
  }

  subscribe(observerOrNext?: PartialObserver<T> | ((value: T) => void), error?: (error: any) => void, complete?: () => void): AnonymousSubscription {

    let subscription = new Subscription(() => this.unsubscribe());

    if (this.timer == null) {

      this.timer = setInterval(() => {

        if (this.values.count()) {

          let value = this.values.removeAt(0);

          this.observers.forEach(o => {
            o.next(value);
          });

        }
        else {

          clearTimeout(this.timer);

          this.observers.forEach(o => {

            if (o.complete) {
              o.complete();
            }
          });
        }
      }, 1000);
    }

    this.observers.push(<Observer<T>>observerOrNext);

    return subscription;
  }

  unsubscribe() {
  }

  static from<T>(values: T[]): TestObservable<T> {
    return new TestObservable<T>(values);
  }
}
