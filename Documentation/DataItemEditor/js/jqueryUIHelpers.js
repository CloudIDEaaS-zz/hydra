$(function () {

    jQuery.fn.tagName = function () {
        return this.prop("tagName");
    };

    jQuery.fn.appendElement = function (tagName, content) {

        let element;

        if (typeof content == "string") {
            element = $(`<${ tagName }>${ content }</${ tagName }>`);
        }
        else {

            element = $(`<${ tagName }></${ tagName }>`); 

            if (typeof content == "function") {
                content(element);
            }
            else if (typeof content == "object") {

                let innerContent = content["innerHtml"] || content["innerText"] || "";

                element = $(`<${ tagName }>${ innerContent }</${ tagName }>`); 
                element.addProperties(content);
            }
        }

        this.append(element);

        return element;
    };


    jQuery.fn.createElement = function (tagName, content) {

        let element;

        if (typeof content == "string") {
            element = $(`<${ tagName }>${ content }</${ tagName }>`);
        }
        else {

            element = $(`<${ tagName }></${ tagName }>`); 

            if (typeof content == "function") {
                content(element);
            }
            else if (typeof content == "object") {

                let innerContent = content["innerHtml"] || content["innerText"] || "";

                element = $(`<${ tagName }>${ innerContent }</${ tagName }>`); 
                element.addProperties(content);
            }
        }


        return element;
    };

    jQuery.fn.createDiv = function (content) {
        return this.createElement("div", content);
    };

    jQuery.fn.createIFrame = function(content) {
        return this.createElement("iframe", content);
    }

    jQuery.fn.appendLink = function(content) {
        return this.appendElement("a", content);
    }

    jQuery.fn.appendParagraph = function(content) {
        return this.appendElement("p", content);
    }

    jQuery.fn.appendLabel = function (content) {
        return this.appendElement("label", content);
    };

    jQuery.fn.appendOption = function (key, value, properties) {
        
        properties = Object.assign({
            value: key,
            innerText: value,
        }, properties);
        
        return this.appendElement("option", properties);
    }

    jQuery.fn.appendSelect = function (items, keyName, valueName, properties) {

        let select;
        let selectedIndex = properties ? properties.selectedIndex : null;
        let x = 0;

        if (!valueName && !properties) {
            properties = keyName;
        }

        select = this.appendElement("select", properties);

        if (selectedIndex && selectedIndex == -1)
        {
            select.appendOption("", "", { selected: "selected"});
        }

        $.each(items, (key, value) =>
        {
            if (keyName && (typeof value == "object")) {
                key = value[keyName];
            }

            if (valueName && (typeof value == "object")) {
                value = value[valueName];
            }

            if (selectedIndex && selectedIndex == x) {
                select.appendOption(key, value, { selected: "selected"});
            }
            else {
                select.appendOption(key, value);
            }

            x++;
        });

        return select;
    };

    jQuery.fn.selectText = function() {

        let range
        let selection;
        let elem = this[0];

        if (document.body.createTextRange) {

            range = document.body.createTextRange();
            range.moveToElementText(elem);
            range.select();
        } 
        else if (window.getSelection) {

            selection = window.getSelection();

            range = document.createRange();
            range.selectNodeContents(elem);

            selection.removeAllRanges();
            selection.addRange(range);
        }        
    }

    jQuery.fn.appendTextBox = function(content) {

        let textBox;

        if (typeof content == "string") {
            textBox = $(`<textarea type='text' value='${ content }'></textarea>`);
        }
        else {

            if (typeof content == "object") {

                let innerContent = content["innerHtml"] || content["innerText"] || "";

                textBox = $(`<input type='text'>${ innerContent }</input>`); 
                textBox.addProperties(content);
            }
            else {

                textBox = $("<input type='text'></input>"); 

                if (typeof content == "function") {
                    content(textBox);
                }
            }
        }

        this.append(textBox);

        return textBox;
    };

    jQuery.fn.appendTextArea = function(content) {

        let textArea;

        if (typeof content == "string") {
            textArea = $(`<textarea>'${ content }'></textarea>`);
        }
        else {

            if (typeof content == "object") {

                let innerContent = content["innerHtml"] || content["innerText"] || "";

                textArea = $(`<textarea>${ innerContent }</textarea>`); 
                textArea.addProperties(content);
            }
            else {

                textArea = $("<textarea></textarea>"); 

                if (typeof content == "function") {
                    content(textArea);
                }
            }
        }

        this.append(textArea);

        return textArea;
    };

    jQuery.fn.createUniqueId = function() {

        let id = (new Date().getTime()) * 1000 + Math.floor(Math.random() * 1001);

        return id;
    }

    jQuery.fn.appendCheckbox = function (text, checked) {

        let id = this.createUniqueId();
        let checkBox;
        let label;

        if (checked) {
            checkBox = $(`<input id='${ id }' class='checkbox' type='checkbox' checked='checked'></input>`);
        }
        else {
            checkBox = $(`<input id='${ id }' class='checkbox' type='checkbox'></input>`);
        }

        label = $(`<label class='checkboxLabel' for='${ id }'>${ text }</label>`);

        this.append(checkBox);  
        this.append(label);

        label.mousedown(() => {

            label.css({
                "background": "#318efb",
                "color": "white"
            });

            checkBox.focus();
        });

        checkBox.focusin(() => {

            label.css({
                "background": "#318efb",
                "color": "white"
            });
        });

        checkBox.focusout(() => {

            label.css({
                "background": "white",
                "color": "black"
            });
        });

        return checkBox;
    };

    jQuery.fn.appendListItem = function(content) {

        let listItem;

        if (typeof content == "string") {
            listItem = $(`<li>${ content }</li>`);
        }
        else {

            if (typeof content == "object") {

                let innerContent = content["innerHtml"] || content["innerText"] || "";

                listItem = $(`<li>${ innerContent }</li>`); 
                listItem.addProperties(content);
            }
            else
            {
                listItem = $("<li></li>"); 

                if (typeof content == "function") {
                    content(div);
                }
                else if (Array.isArray(content)) {
                    listItem.appendFromArrayContent(content, "li");
                }
            }
        }

        this.append(listItem);

        return listItem;
    }

    jQuery.fn.addProperties = function(obj) {

        let keys = Object.keys(obj);

        keys.forEach((k) =>{

            if (k === "style") {
                this.css(obj[k]);
            }
            else if (k != "innerHtml" && k !=="innerText") {
                this.prop(k, obj[k]);
            }
        });
    }

    jQuery.fn.appendFromArrayContent = function(array, childTag) {

        array.forEach((i) =>{

            let itemElement = $(`<${ childTag }>${ i }</${ childTag }>`);

            list.append(itemElement);
        });
    }

    jQuery.fn.appendListUnordered = function (content) {

        let list;

        if (typeof content == "string") {
            list = $(`<ul>${ content }</ul>`);
        }
        else {

            list = $("<ul></ul>"); 

            if (typeof content == "function") {
                content(div);
            }
            else if (typeof content == "object") {
                list.addProperties(content);
            }
            else if (Array.isArray(content)) {
                list.appendFromArrayContent(content, "li");
            }
        }

        this.append(list);

        return list;
    }

    jQuery.fn.innerMost = function() {

        let inner = $.merge(this, this.find("*")).last();
        
        return inner;
    };

    jQuery.fn.then = function() {

        let then = this.parent();
        
        return then;
    };

    jQuery.fn.above = function() {

        let above = this.parent();
        
        return above;
    };

    jQuery.fn.buildContainer = function (config, properties) {

        let div = this; 
        let style = config.style;
        let rows = config.rows;
        let cols = config.cols;
        let elementFunc = config.element;
        let label = config.label;
        let elementName = config.elementName;
        let labelFor = config.labelFor;
        let elementLookup = properties.elementLookup;
        let name = properties.name;
        let x = 0;
          
        if (rows)
        {
            div.css({
                "display": "flex",
                "flex-direction": "column",
                "flex-wrap": "nowrap"
            });

            rows.forEach((r) => {

                let rowName = r.name;
                let rowDiv;
                let rowFullName;

                if (!rowName) {
                    rowName = `[${ x }]`;
                }

                if (name.length > 0) {
                    rowFullName = name + "_" + rowName;
                }
                else {
                    rowFullName = rowName;
                }

                rowDiv = div.appendDiv({ 
                    "class": rowFullName,
                    "name": name
                });

                elementLookup[rowName] = rowDiv;

                rowDiv.buildContainer(r, Object.assign({ name: rowFullName, elementLookup: properties.elementLookup, root: properties.root }, properties));

                x++;
            });

            if (cols) {
                throw "A container element cannot contain both rows and columns";
            }

            if (elementFunc) {
                throw "A container element cannont rows or columns as well as an element";
            }
        }
        else if (cols)
        {
            div.css({
                "display": "flex",
                "flex-direction": "row",
                "flex-wrap": "nowrap"
            });

            cols.forEach((c) => {

                let colName = c.name;
                let colDiv;
                let colFullName;

                if (!colName) {
                    colName = `[${ x }]`;
                }

                if (name.length > 0) {
                    colFullName = name + "_" + colName;
                }
                else {
                    colFullName = colName;
                }

                colDiv = div.appendDiv({ 
                    "class": colFullName,
                    "name": name
                });

                elementLookup[colName] = colDiv;

                colDiv.buildContainer(c, Object.assign({ name: colFullName, elementLookup: properties.elementLookup, root: properties.root }, properties)); 

                x++
            });

            if (elementFunc) {
                throw "A container element cannont rows or columns as well as an element";
            }
        }
        else if (elementFunc) {

            let element = elementFunc(div);

            if (elementName) {

                let fullName = name + "_" + elementName;

                elementLookup[elementName] = element;
                
                element.attr("name", elementName);
                element.attr("id", fullName);
            }
        }
        else if (label) {

            let element = div.appendLabel(label);
            
            if (labelFor) {

                properties.root.on("containerInitialized", () => {

                    forElement = elementLookup[labelFor];
                    element["for"] = forElement.id;
                });
            }
        }

        if (style) {
         
            this.css(style);
        }
        
        this.append(div);

        return div;
    };


    jQuery.fn.enable = function () {
        this.prop("disabled", false);
    }

    jQuery.fn.disable = function () {
        this.prop("disabled", true);
    }

    jQuery.fn.appendContainer = function (config, properties) {

        let elementLookup;
        let name = "container";
        let div = $("<div></div>"); 

        if (properties) {

            elementLookup = properties.elementLookup;
            name = properties.name;

            div.addProperties(properties);
        } 
        else {
            properties = {};
        }

        if (!properties.name) {
            properties.name = name;
        }

        properties.root = div;

        if (!elementLookup) 
        {
            elementLookup = {};
            properties.elementLookup = elementLookup;
        }

        elementLookup[name] = div;
        div.buildContainer(config, properties);

        this.append(div);

        properties.root.trigger("containerInitialized");

        return elementLookup;
    };

    jQuery.fn.appendDiv = function (content) {

        let div;
        
        if (typeof content == "string") {
            div = $(`<div>${ content }</div>`);
        }
        else {

            div = $("<div></div>"); 

            if (typeof content == "function") {
                content(div);
            }
            else if (typeof content == "object") {
                div.addProperties(content);
            }
        }

        this.append(div);

        return div;
    };

    jQuery.fn.appendTableRow = function() {

        let row = $("<tr>");
        let tbody = this.find("tbody");

        if (tbody.length) {
            tbody.append(row);
        }
        else {
            this.append(row);
        }

        return row;
    }

    jQuery.fn.appendTableCell = function(content) {
        return this.appendElement("td", content);
    }

    jQuery.fn.appendTableHeader = function(content) {
        return this.appendElement("th", content);
    }

    jQuery.fn.appendTable = function (template) {

        let table = $("<table>");
        let headerRow = null;
        let data = template.data;

        template.columns.forEach((c) => {

            if (c.headerTitle) {

                if (!headerRow) {
                    headerRow = table.appendTableRow();
                }

                headerRow.appendTableHeader(c.headerTitle);
            }
        });

        data.forEach((d) => {

            let row = table.appendTableRow();
            let index = 0;
            let keys = Object.keys(d);
            let data = d;

            template.columns.forEach((c) => {

                if (c.itemTemplate) {

                    let itemTemplate = c.itemTemplate;

                    if (typeof itemTemplate === "string") {
                        row.appendTableCell(data[itemTemplate]);
                    }
                    else if (typeof itemTemplate === "function") {
                        td = row.appendTableCell();
                        itemTemplate(td, data);
                    }
                }
                else {
                    row.appendTableCell(data[keys[index]]);
                }

                index++;
            });
        });
  
        this.append(table);

        return table;
    }

    jQuery.fn.createDropPanel = function(content) {

        let input_position = this.offset();
        let targetHeight = this.height() + 6;
        let targetWidth = this.width();
        let body = $("body");
        let panel = body.appendDiv(content);
        let panelElement = panel[0];
        let element = this[0];
        let focusedElement;

        body.mousedown((e) => {

            if (e.target != panelElement && e.target != element && !panel.contains(e.target)) {

                focusedElement = null;
                panel.fadeOut();
            }
        });

        body.keydown((e) => {

            let key = event.key;

            if (key == "Escape") {
                panel.fadeOut();
            }
        });

        panel.blur(() => {
            focusedElement = null;
        });

        panel.mousedown(() => {
            focusedElement = panelElement;
        });

        window.setTimeout(() => {

            panel.find("*").mousedown(() => {
                focusedElement = panelElement;
            });

        }, 1);

        panel.css({
            'position': 'absolute',
            'padding': '3px',
            'display': 'none',
            'z-index': 2,
            'background-color': 'white',
            'top': (input_position.top + targetHeight) + 'px',
            'left': (input_position.left) + 'px',
            'border': '1px solid gray',
            'box-shadow': '2px 2px 2px #aaaaaa'
        });

        panel.makePinnable();

        this.focus(() => {
            panel.fadeIn();
        });

        this.click(() => {

            let target = event.target;

            if (panel && !focus) {
                panel.fadeToggle();
            }

            focus = false;

        });

        this.blur(() => {

            window.setTimeout(() => {

                let focused = $(":focus");
                let panelHasFocused = panel.has(focused).length > 0;

                if (focusedElement == panelElement) {

                    panel.focusout(() => {
                        if (focusedElement != panelElement) {
                            panel.fadeOut();
                        }
                    });
                } 
                else if (panel && !panelHasFocused) {
                    panel.fadeOut();
                }

            }, 1);
        });
    }

    jQuery.fn.makePinnable = function() {

        this.addClass("unpinned");

        this.on("click", function() {

            let panel = $(event.target);

            if (event.offsetX > panel.width() - 12 && event.offsetY < 12)
            {
                if (panel.hasClass("unpinned"))
                {
                    panel.removeClass("unpinned");
                    panel.addClass("pinned");
                }
                else
                {
                    panel.removeClass("pinned");
                    panel.addClass("unpinned");
                }
            }
        });
    }

    jQuery.fn.getSelectedText = function() {
        return this.find("option:selected").text();
    }

    jQuery.fn.getCurrentTableHeader = function() {
        return this.closest('table').find('th').eq(this.index());
    }

    jQuery.fn.contains = function(element) {
        return $.contains(this[0], element);
    }
})
