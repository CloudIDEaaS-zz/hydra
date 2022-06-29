createInlineValidations = (state) => {

  let parentElement = state.parentElement;
  let customData = state.customData;
  let textBox;

  textBox = parentElement.appendTextBox({
    "style": {
      "valign": "baseline",
      "padding": "0px",
      "font-size": "14px",
      "height": customData.cellHeight - 4 + "px"
    },
    readonly: "readonly",
    placeholder: "Select validations"
  });

  return textBox;
}

createDropPanelInputValidations = (state) => {

  let parentElement = state.parentElement;
  let checkBoxList;
  let checkBoxListInstance;
  let itemCaseCallbacks = [];
  let addItemCaseActivatedHandler = (itemCase, callback) => {

    itemCaseCallbacks[itemCase] = callback;
  }

  state.addItemCaseActivatedHandler = addItemCaseActivatedHandler;

  checkBoxList = parentElement.appendCheckboxListCarouselForm({
    items: [{
      name: "required",
      text: "Required",
      value: false,
      state: state
    },
    {
      name: "range",
      text: "Range",
      value: false,
      state: state
    },
    ],
    slides: {
      cases: {
        "required": {
          uiElements: [{
            name: "requiredOptions",
            createInline: function (state) {

              let parentElement = state.parentElement;
              let customData = state.customData;
              let textBox;

              textBox = parentElement.appendTextBox({
                "style": {
                  "valign": "baseline",
                  "padding": "0px",
                  "font-size": "14px",
                  "height": customData.cellHeight - 4 + "px",
                  "width": "350px"
                },
                placeholder: "Enter error message"
              });

              return textBox;
            }
          }]
        },
        "range": {

        }
      }
    },
    state: state
  })

  checkBoxListInstance = checkBoxList.itemCarouselForm("instance");
  checkBoxListInstance.onItemSelected = (itemSelected, data, element, checked) => {

    let items = checkBoxListInstance.items;
    let item = items.filter(i => i.name == itemSelected);
    let callback = itemCaseCallbacks[item[0].name];

    callback(itemSelected, item, element, checked);
  }

  return checkBoxList;
}

createMaskedInputInline = (state) => {

  let parentElement = state.parentElement;
  let customData = state.customData;
  let textBox;

  textBox = parentElement.appendTextBox({
    "style": {
      "valign": "baseline",
      "padding": "0px",
      "font-size": "14px",
      "height": customData.cellHeight - 4 + "px"
    },
    placeholder: "Enter mask"
  });

  return textBox;
}
