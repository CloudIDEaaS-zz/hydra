// new

class PropertiesNode 
{
    name = "";
    data = {};
    childProperties = [];

    constructor(name, data) {
        this.name = name;
        this.data = data;
    }
};


$(function () {

    jQuery.fn.appendCheckboxListCarouselForm = function(config) {

        let items = config.items;

        let itemCarouselForm;
        let itemTemplate = (p, d) => {

            let checkBox = p.appendCheckbox(d.text, d.value);

            checkBox.change((e) => {
               
               let elem = e.target;
               let checked = elem.checked;
               let itemCarouselFormInstance = itemCarouselForm.itemCarouselForm("instance");

                itemCarouselFormInstance.raiseOnItemSelected(d.name, d, elem, checked);
            });
        };

        itemCarouselForm = this.itemCarouselForm({
            items: config.items,
            slides: config.slides,
            state: config.state,
            itemTemplate: itemTemplate,
        });

        return itemCarouselForm;
    };

    jQuery.fn.assignCarousel = function(carouselContainer, carouselForm) {

        let carouselList = this.getCarouselList();

        this.data("carouselAssignments", {
            carouselContainer : carouselContainer,
            carouselForm: carouselForm
        });

        carouselList.push(carouselForm);
    }

    jQuery.fn.getCarouselList = function() {

        let carouselList;
        let window$ = $(window);

        carouselList = window$.data("carouselList");

        if (!carouselList) {

            carouselList = [];

            window$.data("carouselList", carouselList);
        }

        return carouselList;
    }

    jQuery.fn.getCarouselAssignments = function(carouselContainer, carouselForm) {

        let assignments = this.data("carouselAssignments");

        return assignments;
    }

    jQuery.fn.getChildData = function(dataAttribute) {

        let foundData;

        this.find("*").each((i, e) => {

            data = $(e).data("inlineElementDropPanel");

            if (data) {
                foundData = data;
            }

        });

        return foundData;
    }

    jQuery.fn.checkCarouselFocusOut = function(callback) {

        let carouselFormList = this.find(".carouselForm ul");
        let dropPanel = this.getChildData("inlineElementDropPanel");
        let thisEvent = event;
        let eventTarget = thisEvent.target;

        if (carouselFormList) {

            setTimeout(() => {

                let focused = $(":focus");
                let listHasFocused = carouselFormList.has(focused).length > 0;
                let dropHasFocused = false;

                if (dropPanel) {

                    if (focused.length == 0 && thisEvent.constructor.name == "FocusEvent") {
                        listHasFocused = carouselFormList.has(eventTarget).length > 0;
                    }

                    dropHasFocused = dropPanel && dropPanel.has(focused).length > 0;
                }

                if (!listHasFocused && !dropHasFocused) {

/*
                    let td = carouselFormList.closest("td");
                    let assignments = td.getCarouselAssignments();
                    let carouselContainer = assignments.carouselContainer;

                    if (carouselContainer) {
                        carouselContainer.remove();
                    }

                    if (dropPanel) {
                        dropPanel.remove();
                    }

                    callback();
*/              } 
            }, 100);
        }
    }

    jQuery.fn.checkDropdownFocusOut = function () {

        let dropPanel = this.data(".inlineElementDropPanel");
        let thisEvent = event;
        let eventTarget = thisEvent.target;

        if (dropPanel) {

            setTimeout(() => {

                let focused = $(":focus");

                if (dropPanel) {

                    dropHasFocused = dropPanel && dropPanel.has(focused).length > 0;
                }

                if (dropHasFocused) {
                }

            }, 100);
        }
    }

    jQuery.fn.tabNextForm = function () {

        let td = this.closest("td");
        let next = td.next();
        let current = td;

        while (next.length == 0) {

            current = current.parent();
            next = current.next();

            if (next.length) {
                if (next[0].tagName.toLowerCase() == "tr") {
                    next = next.children("td");
                    next = next.find(":input")
                }
            }
        }

        event.cancelBubble = true;
        event.preventDefault();

        if (next.length) {

            if (next[0].tagName.toLowerCase() == "td") {
                    next = next.find(":input")
            }

            next.data("focusTimestamp", Date.now());
            next.focus();

            return true;
        }
        else {
            return false;
        }
    }

    jQuery.fn.tabPrevForm = function () {

        let td = this.closest("td");
        let prev = td.prev();
        let current = td;

        console.log("tabPrevForm");

        while (prev.length == 0) {

            current = current.parent();
            prev = current.prev();

            if (prev.length) {
                if (prev[0].tagName.toLowerCase() == "tr") {
                    prev = prev.children("td");
                    prev = prev.find(":input")
                }
            }
        }

        event.cancelBubble = true;
        event.preventDefault();

        if (prev.length) {

            if (prev[0].tagName.toLowerCase() == "td") {
                    prev = prev.find(":input")
            }

            prev.data("focusTimestamp", Date.now());
            prev.focus();

            return true;
        }
        else {
            return false;
        }
    }

    // new

    $.widget("custom.carouselForm", {

        currentState: {},
        slides: null,
        name: null,
        lostFocus: null,
        propertiesNode: null,
        _create: function () {

            let parentElement = $(this.element);
            
            this.slides = this.options.slides;

            this.currentState = this.options.state;
            this.propertiesNode = this.currentState.properties;
            this.name = this.propertiesNode.name;
            this.lostFocus = this.options.lostFocus;

            this.appendSlides(parentElement, this.slides, this.currentState);
        },
        getCarouselProperties: function (inlineElement, dropPanel, state) {

            let properties;
            let uiElements = this.slides.cases[state.case].uiElements;

            $.each(uiElements, (i, e) => {
                properties = e.getProperties(inlineElement, dropPanel, state);
            });

            return properties;
        },
        appendSlides: function(parentElement, slides, state) {

            let currentCase = state.case;
            let cases = slides.cases;
            let selectedCaseObject;
            let uiElements = [];
            let carouselFormList;
            let customData = state.customData;

            for (var property in cases) {
                if (property == currentCase) {
                    selectedCaseObject = cases[property];
                    break;
                }
            }

            if (selectedCaseObject) {

                let carousel;
                let wrapper;

                carouselFormList = this.createCarouselFormList(parentElement, customData);
                uiElements = selectedCaseObject.uiElements;

                wrapper = carouselFormList.parents('.jcarousel-wrapper');
                carousel = wrapper.find('.jcarousel');

                uiElements.forEach((e) => {

                    let name = e.name;
                    let createInline = e.createInline;
                    let createDropPanel = e.createDropPanel;
                    let inlineElement;
                    let dropPanel;
                    let targetProperty;
                    let newTarget;
                    let currentState = Object.assign(this.currentState, {

                        parentElement: this.createPropertyPanel(carouselFormList, customData),
                        showDropPanel: () => {

                        },
                        showNext: (name) => {

                            let target = carousel.jcarousel('target').first();
                            let last = carousel.jcarousel('last');

                            if (target[0] == last[0]) { 
                                currentState.tabNext(target);
                            } 
                            else {
                                carousel.on('jcarousel:animateend', function (e, c) {

                                    newTarget = carousel.jcarousel('target');
                                    targetProperty = newTarget.innerMost();
                                    targetProperty.focus();
                                });

                                carousel.jcarousel('scroll', '+=1');
                            }
                        },
                        showPrev: (name) => {

                            let target = carousel.jcarousel('target').first();
                            let first = carousel.jcarousel('first');

                            if (target[0] == first[0]) {
                                currentState.tabPrev(target);
                            } 
                            else {
                                carousel.on('jcarousel:animateend', function (event, c) {

                                    newTarget = carousel.jcarousel('target');
                                    targetProperty = newTarget.innerMost();
                                    targetProperty.focus();
                                });

                                carousel.jcarousel('scroll', '-=1');
                            }
                        },
                        end: () => {

                        },
                        setCaseValue: (caseValue) => {

                        }
                    });

                    if (createInline) {

                        inlineElement = createInline(currentState);

                        inlineElement.focusout(() => {
                            
                            parentElement.checkCarouselFocusOut(() => {

                                let properties = this.getCarouselProperties(inlineElement, dropPanel, this.currentState);

                                this.lostFocus(properties);
                            });
                        });

                        inlineElement.keydown(() => {

                            let key = event.key;
                            let inlinePlaceholder = inlineElement[0].placeholder;

                            switch (key) {

                                case "Tab": {

                                    if (event.shiftKey) {
                                        currentState.showPrev();
                                    } 
                                    else {
                                        currentState.showNext();
                                    }

                                    event.cancelBubble = true;
                                    event.preventDefault();

                                    break;
                                }
                                case "ArrowDown": {

                                    // new
                            
                                    dropPanel = inlineElement.data("inlineElementDropPanel");

                                    event.cancelBubble = true;
                                    event.preventDefault();

                                    if (dropPanel) {

                                        let firstChild = dropPanel.find(":input").first();

                                        firstChild.focus();
                                    }

                                    break;
                                }
                            }
                        });
                    }

                    if (inlineElement && createDropPanel) {

                        inlineElement.createDropPanel((p) => {

                            let dropPanel;
                            let dropState = Object.assign(currentState, {
                                parentElement: p
                            });

                            p.addClass("inlineElementDropPanel");
                            dropPanel = createDropPanel(dropState);

                            // new

                            inlineElement.data("inlineElementDropPanel", dropPanel)

                            dropPanel.find(":input").focusout(() => {

                                parentElement.checkCarouselFocusOut(() => {
                                    
                                    let properties = this.getCarouselProperties(inlineElement, dropPanel, this.currentState);

                                    this.lostFocus(properties);

                                });
                            });
                        });
                    }
                });

                carousel.jcarousel();
            }
        },
        createCarouselFormList(parent, customData) {

            let list = parent.appendDiv({
                    class: "carouselForm"
                })
                .appendDiv({
                    class: "jcarousel-wrapper"
                })
                .appendDiv({
                    class: "jcarousel"
                })
                .appendListUnordered();

            return list;
        },
        createPropertyPanel(list, customData) {

            let listItem = list.appendListItem();
            let propertyItem = listItem.appendDiv({
                class: "propertyItem",
                style: {
                    width: customData.cellWidth - 2 + "px",
                    height: customData.cellHeight - 2 + "px"
                }
            });

            return propertyItem;
        },
        _generateHtml: function () {

            let parentElement = $(this.element);
        },
        _setOption: function (key, value) {

            if (key === "value") {
                value = this._constrain(value);
            }

            this._super(key, value);
        },
        _setOptions: function (options) {

            this._super(options);
        }
    });

    $.widget("custom.itemCarouselForm", {
        // default options
        currentState: {},
        propertiesRootNode: null,
        container: null,
        items: null,
        onItemSelected: null,
        raiseOnItemSelected: function(item, data, elem, checked) {
            this.onItemSelected(item, data, elem, checked);
        },
        _create: function () {

            let itemTemplate = this.options.itemTemplate;
            let data = this.options.items;
            let parentElement = $(this.element);
            let slides = this.options.slides;

            this.items = data;
            this.currentState = this.options.state;
            this.propertiesRootNode = this.currentState.properties;

            this.container = parentElement.appendTable({
                columns:[
                    {
                        itemTemplate: itemTemplate
                    },
                    {
                        itemTemplate: (td, data) => {

                            let propertiesCell = td;
                            let propertiesRootNode = data.state.properties;
                            let carouselContainer;
                            let initialState;
                            let cellWidth;
                            let cellHeight;
                            let observer;
                            let state = data.state;
                            let originalCellWidth;
                            let carouselForm;
                            let itemPropertiesNode;
                            let propertiesCellWidth;
                            let currentState = this.currentState;

                            // instantiate and push only if new

                            itemPropertiesNode = new PropertiesNode(data.name, { /* todo - add properties */ });
                            propertiesRootNode.childProperties.push(itemPropertiesNode);

                            propertiesCell.addClass("overlay propertiesCell");
                            propertiesCell.text("");

                            cellWidth = 300;

                            state.addItemCaseActivatedHandler(data.name, (selectedCase, item, checkBox, checked) => {

                                let propertiesCellWidth = propertiesCell.width();
                                let addCarousel;

                                cellHeight = propertiesCell.height();

                                if (originalCellWidth == null) {
                                    originalCellWidth = propertiesCellWidth ? propertiesCellWidth : 1;
                                }

                                // new

                                let carouselContainerLostFocus = (checkBox) => {

                                    let propertiesNode = carouselForm.currentState.properties;

                                    propertiesCell.width(originalCellWidth);
                                };

                                propertiesCell.width(cellWidth);

                                initialState = {
                                    id: new Date().getTime(),
                                    case: selectedCase,
                                    properties: itemPropertiesNode,
                                    tabNext: (e) => {

                                         if (!e.tabNextForm()) {
                                            carouselContainer.remove();         
                                         }

                                    },
                                    tabPrev: (e) => {

                                        if (!e.tabPrevForm()) {
                                            carouselContainer.remove();
                                        }
                                    },
                                    customData: {
                                        cellWidth: cellWidth,
                                        cellHeight: cellHeight
                                    }
                                };

                                console.log(initialState.id);

                                if (carouselContainer) {
                                    carouselContainer.remove();
                                }

                                addCarousel = () => {

                                    carouselContainer = propertiesCell.appendDiv();

                                    carouselContainer.width(cellWidth - 1);
                                    carouselContainer.height(cellHeight - 1);

                                    carouselForm = carouselContainer.carouselForm({
                                        slides: slides,
                                        lostFocus: () => carouselContainerLostFocus(checkBox),
                                        state: initialState
                                    }).carouselForm("instance");

                                    propertiesCell.assignCarousel(carouselContainer, carouselForm);
                                }

                                $(checkBox).focusin(() => {

                                    if ($(checkBox).prop("checked")) {

                                        if (propertiesCell.has(carouselContainer).length == 0) {
                                            addCarousel();
                                        }
                                    }
                                });

                                $(checkBox).focusout(() => {
                                    $(this).checkDropdownFocusOut();
                                });

                                if (checked) {
                                    addCarousel();
                                }
                                else {
                                     propertiesCell.width(originalCellWidth);
                                }
                            });
                        }
                    }
                ],
                data: data
            });

            parentElement.find(":input").keydown((e) => {

                let key = event.key;
                let target = e.target;
                let inputs = parentElement.find(":input");
                let currentIndex = inputs.index(target);
                let newIndex = 0;

                switch (key) {

                    case "ArrowDown": {

                        newIndex = currentIndex + 1;

                        if (newIndex < inputs.length) {
                            inputs[newIndex].focus();
                        }

                        event.cancelBubble = true;
                        event.preventDefault();
                
                        break;
                    }
                    case "ArrowUp": {

                        newIndex = currentIndex - 1;

                        if (newIndex >= 0) {
                            inputs[newIndex].focus();
                        }
                        else {
                            parentElement.focus();
                        }

                        event.cancelBubble = true;
                        event.preventDefault();
                
                        break;
                    }
                }
            });
        }
    });
})
