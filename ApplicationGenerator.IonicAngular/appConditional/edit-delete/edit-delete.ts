import { Component } from '@angular/core';
import { NavController, NavParams } from '@ionic/angular';
import { ICellRendererAngularComp } from 'ag-grid-angular';

/**
 * Generated class for the EditDeletePage page.
 *
 * See https://ionicframework.com/docs/components/#navigation for more info on
 * Ionic pages and navigation.
 */

@Component({
  selector: 'edit-delete',
  templateUrl: 'edit-delete.html',
})
export class EditDeleteButtonsComponent implements ICellRendererAngularComp {

  data: any;
  onEditClick: (data: any) => void;
  onDeleteClick: (data: any) => void;

  refresh(params: any): boolean {
    return true;
  }

  edit() {
    this.onEditClick(this.data);
  }

  delete() {
    this.onDeleteClick(this.data);
  }

  agInit(params: any): void {
    this.data = params.data;
    this.onEditClick = params.onEditClick;
    this.onDeleteClick = params.onDeleteClick;
  }

  afterGuiAttached?(params?: any): void {
  }

  constructor(public navCtrl: NavController, public navParams: NavParams) {
  }

  ionViewDidLoad() {
    console.log('ionViewDidLoad EditDeletePage');
  }

}
