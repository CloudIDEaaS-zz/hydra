import 'rxjs/add/operator/toPromise';
import { Injectable, Injector, ComponentFactoryResolver, Type, Component } from '@angular/core';
import { AccessTokenInfo } from '../../models/accesstokeninfo.model';
import { List } from 'linq-collections';

@Injectable()
export class UserProvider {

  private roles: List<string>;

  constructor(private componentFactoryResolver: ComponentFactoryResolver, private injector: Injector)
  {
    this.roles = new List<string>();
    this.roles.push("{00000000-0000-0000-0000-000000000000}");
  }

  public canView(component: Component): Promise<boolean> {

    let name = component["name"];

    try {
      if (component["AuthorizedFor"]) {

        let authorizedFor = new List<string>(component["AuthorizedFor"]);

        if (authorizedFor.any(a => this.roles.any(r => r == a))) {
          return Promise.resolve(true);
        }
        else {
          throw `User not authorized for ${ name }`;
        }
      }
    }
    catch (e) {
      console.error(e);
      throw e;
    }

    return Promise.resolve(true);
  }

  public handleDefaultRoute(handler: (r: any) => string): string {
      let role = this.roles.last();

      return handler(role);
  }
}
