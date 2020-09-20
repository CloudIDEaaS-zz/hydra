import { EditLabel } from "./edit-label";

export class EditChangeArgs {
  editLabel: EditLabel;
  editInput : HTMLInputElement;
  allowChange : boolean = true;
  oldValue: string;
  value: string;
}
