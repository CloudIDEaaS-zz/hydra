import { NgModule } from '@angular/core';
import { EditLabel } from './edit-label';

@NgModule({
  declarations: [
    EditLabel,
  ],
  exports: [
    EditLabel
  ]
})
export class EditLabelModule
{
  constructor() {
    console.log('');
  }
}
