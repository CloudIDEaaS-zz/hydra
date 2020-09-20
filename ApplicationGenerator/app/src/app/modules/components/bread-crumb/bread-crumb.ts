import { Component, NgZone, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { IonicPage, ViewController, NavController, NavParams, ToastController, Toast, AlertController, PopoverController } from "ionic-angular";
import { List, IEnumerable } from "linq-javascript";
const prototypes = require("prototypes");

export class ViewInfo
{
  text: string;
  isNotLast: boolean;

  constructor(params: { text: string, isNotLast: boolean})  {
    this.text = params.text;
    this.isNotLast = params.isNotLast;
  }
}

@Component({
  selector: 'bread-crumb',
  templateUrl: 'bread-crumb.html'
})
export class BreadCrumb {

  constructor(private navCtrl: NavController,
    private navParams: NavParams) {
  
    }

    public getViews() : ViewInfo[] {

      let views = new List<ViewController>(this.navCtrl.getViews()).copy();
      let viewInfos : ViewInfo[] = [];
      let count = views.count();
      let x = 0;
      let navParams = this.navParams;

      views.forEach(v => {

        let page = <Component> v.component;
        let recordExpression : string = page["__recordExpression__"];
        let pageName : string = page["__pageName__"] || v.name;
        let isNotLast = x != count - 1;
        let isPrevious = x == count - 2;
        let text = isNotLast ? recordExpression : pageName;
        let rawText
        let expression = /\{(\w*?)\}/;

        if (isPrevious)
        {
          page["__navParams__"] = navParams;
        }
        else
        {
          navParams = page["__navParams__"];
        }

        if (recordExpression != undefined && expression.test(text))
        {
          let match = expression.exec(text);
          let param = match[1];
          let paramValue = navParams.get(param);

          text = text.replace(expression, paramValue);
        }

        viewInfos.push(new ViewInfo(
        {
          text : text,
          isNotLast: isNotLast
        }));

        x++;
      });
    
      return viewInfos;
    }
}
