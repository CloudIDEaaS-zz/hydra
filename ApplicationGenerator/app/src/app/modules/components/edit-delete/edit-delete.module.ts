import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from 'ionic-angular';
import { EditDeleteButtons } from './edit-delete';

@NgModule({
  declarations: [

    // Components, directives and pipes that belong to this module

    EditDeleteButtons
  ],
  imports: [

    // Imported from other modules
    // These go in every component module that uses Ionic

    CommonModule,
    IonicModule.forRoot(EditDeleteButtons),

    // End
  ],
  providers: [
  ],
  exports: [

    // What is public to the outside

    EditDeleteButtons
  ]
})
export class EditDeleteButtonsModule
{
  constructor() {
    console.log('');
  }
}
