let dataTypes = {
  1: "string",
  2: "int",
  3: "datetime"
};

let maskedInput = {
  name: "maskedInput",
  createInline: (state) => {

    let parentElement = state.parentElement;
    let columnData = state.columnData;
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
  },
  getProperties: (inlineElement, dropPanel, state) => {

    let textBox = inlineElement;
    let value = textBox.val();

    return {
      mask: value
    };
  }
};

let slides = {
  cases: {
    "datetime": {
      uiElements: [{
        name: "validations",
        createInline: (state) => {

          let parentElement = state.parentElement;
          let customData = state.customData;
          let columnData = state.columnData;
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
        },
        createDropPanel: (state) => {

          let parentElement = state.parentElement;
          let columnData = state.columnData;
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
                    name: "errorMessage",
                    createInline: function (state) {

                      let parentElement = state.parentElement;
                      let customData = state.customData;
                      let columnData = state.columnData;
                      let textBox;

                      textBox = parentElement.appendTextBox({
                        "style": {
                          "valign": "top",
                          "margin": "0px -2px 0px 5px",
                          "font-size": "14px",
                          "height": customData.cellHeight - 2 + "px",
                          "width": "250px"
                        },
                        placeholder: "Enter error message"
                      });

                      return textBox;
                    },
                    getProperties: (inlineElement, dropPanel, state) => {

                      let textBox = inlineElement;
                      let value = textBox.val();

                      return {
                        errorMessage: value
                      };
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

            callback(itemSelected, item, data, element, checked);
          }

          return checkBoxList;
        },
        getProperties: (inlineElement, dropPanel, state) => {
          return null;
        }
      },
        maskedInput
      ]
    }
  }
};

return {
  dataTypes: dataTypes,
  slides: slides
};