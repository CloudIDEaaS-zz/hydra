import { Component, ContentChild, ElementRef, Input, Output, EventEmitter, Inject } from '@angular/core';
import { IonicPage, NavController, NavParams, Platform } from 'ionic-angular';
import { EditChangeArgs } from './';
import * as $ from 'jquery';
import { WINDOW } from '../../../app/core/services/window.service';
import { DebugUtils } from '../../utils/DebugUtils';

/**
 * Generated class for the EditLabelPage page.
 *
 * See https://ionicframework.com/docs/components/#navigation for more info on
 * Ionic pages and navigation.
 */

@Component({
  selector: 'editLabel',
  template:
  `<label (dblclick)="edit()">
    <ng-content></ng-content>
  </label>`,
})
export class EditLabel {

  @ContentChild('editLabel') innerText;
  label: HTMLLabelElement;
  nativeElement: HTMLElement;
  @Input() allowEdit: boolean;
  @Input() selectedStyle: string;
  @Output() onchange: EventEmitter<EditChangeArgs> = new EventEmitter<EditChangeArgs>();
  @Output() onendedit: EventEmitter<null> = new EventEmitter<null>();
  @Inject(WINDOW) private window: Window;

  constructor(private platform: Platform, private elementRef: ElementRef) {

    this.nativeElement = elementRef.nativeElement;
    this.allowEdit = true;
    this.selectedStyle = "selected";

    platform.ready().then(() => {

      let hasFocus = false;

      this.label = <HTMLLabelElement> $(this.nativeElement).find("label")[0];

      document.addEventListener("keydown", (e: KeyboardEvent) => {

        if (e.key == "F2") {

          let selectedLabel = $(".selected");

          if (selectedLabel.length && this.label == selectedLabel[0]) {

            this.edit();
            e.cancelBubble = true;
          }
        }
      });

    });
  }

  edit() {

    if (this.allowEdit) {

      let document = <HTMLDocument> this.label.ownerDocument;
      let inputWrapper = document.createElement("div");
      let editInput = document.createElement("input");
      let text = this.label.innerText;
      let parentElement = this.nativeElement.parentElement;
      let escaped = false;

      inputWrapper.style.position = "absolute";
      inputWrapper.style.left = "2px";
      inputWrapper.style.top = "0px";
      inputWrapper.style.width = this.label.offsetWidth - 2 + "px";
      inputWrapper.style.height = this.label.offsetHeight - 2 + "px";
      inputWrapper.style.backgroundColor = "white";
      inputWrapper.style.paddingLeft = "3px";

      editInput.style.top = "0px";
      editInput.style.margin = "0px;"
      editInput.style.borderStyle = "none";
      editInput.style.border = "0px;"
      editInput.style.width = this.label.offsetWidth - 5 + "px";
      editInput.style.height = this.label.offsetHeight - 2 + "px";
      editInput.value = text;

      inputWrapper.appendChild(editInput);
      parentElement.appendChild(inputWrapper);

      editInput.focus();
      editInput.setSelectionRange(0, text.length)

      editInput.onblur = (e: Event) => {

        inputWrapper.removeChild(editInput);
        parentElement.removeChild(inputWrapper);

        if (!escaped) {
          this.handleChange(editInput);
        }
      };

      this.label.onfocus = (e) => {
        this.handleBlur(editInput);
      }

      editInput.onkeydown = (e: KeyboardEvent) => {

        if (e.key == "Enter") {

          this.label.tabIndex = 0;
          this.label.focus();
        }
        else if (e.key == "Escape") {

          escaped = true;
          this.label.tabIndex = 0;
          this.label.focus();
        }

        e.cancelBubble = true;
      }
    }
  }

  handleBlur(editInput: HTMLInputElement) {

    let label = <HTMLLabelElement> window.document.activeElement;

    if (label.tagName.toLowerCase() != "label") {
      DebugUtils.break();
    }

    label.tabIndex = -1;
    this.onendedit.emit();
  }

  handleChange(editInput: HTMLInputElement) : boolean {

    if (this.label.innerText != editInput.value) {

      let editChangeArgs : EditChangeArgs = {
        editLabel: this,
        editInput: editInput,
        allowChange: true,
        oldValue: this.label.innerText,
        value: editInput.value
      }

      this.onchange.emit(editChangeArgs);

      if (editChangeArgs.allowChange) {
        this.label.innerText = editInput.value;
        return true;
      }
    }

    return false;
  }
}
