import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from 'ionic-angular';
import { BreadCrumb } from './bread-crumb';

@NgModule({
  declarations: [

    // Components, directives and pipes that belong to this module

    BreadCrumb
  ],
  imports: [

    // Imported from other modules
    // These go in every component module that uses Ionic

    CommonModule,
    IonicModule.forRoot(BreadCrumb),

    // End
  ],
  providers: [
  ],
  exports: [

    // What is public to the outside

    BreadCrumb
  ]
})
export class BreadCrumbModule
{
  constructor() {
    console.log('');
  }
}
