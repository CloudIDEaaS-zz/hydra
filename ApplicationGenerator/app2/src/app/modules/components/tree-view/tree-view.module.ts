import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TreeView, ForNodesDirective, OnCreateDirective } from './';
import { AnimationService, AnimatesDirective } from 'css-animator'
import { EditLabelModule } from '../edit-label';
import { IonicModule } from 'ionic-angular';

@NgModule({
  declarations: [

    // Components, directives and pipes that belong to this module

    TreeView,
    AnimatesDirective,
    ForNodesDirective,
    OnCreateDirective,
  ],
  imports: [

    // Imported from other modules

    EditLabelModule,

    // These go in every component module that uses Ionic

    CommonModule,
    IonicModule.forRoot(TreeView),

    // End
  ],
  providers: [
    AnimationService,
  ],
  exports: [

    // What is public to the outside

    TreeView
  ]
})
export class TreeViewModule
{
  constructor() {
    console.log('');
  }
}
